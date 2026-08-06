using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.PrintService
{
    public class PrintService : IDisposable
    {
        private PrintServiceSetting _setting;
        private KarnamaComponent _karnamaComponent;

        #region Singleton

        public static PrintService Instance { get; }

        private PrintService()
        {

            #region Self

            _selfLinesFont = new Font(FontFamilyName, 11.0f, FontStyle.Bold);
            _selfLinesLineHeight = _selfLinesFont.GetHeight() + LineHeading;

            _selfHeaderTitleFont = new Font(FontFamilyName, 11.0f, FontStyle.Regular);
            _selfHeaderTitleLineHeight = _selfHeaderTitleFont.GetHeight() + LineHeading;


            _selfIssueDateFont = new Font(FontFamilyName, 11.0f, FontStyle.Regular);
            _selfIssueDateLineHeight = _selfIssueDateFont.GetHeight() + LineHeading;

            _selfEmployeeTitleFont = new Font(FontFamilyName, 11.5f, FontStyle.Bold);
            _selfEmployeeTitleLineHeight = _selfEmployeeTitleFont.GetHeight() + LineHeading;

            _selfFoodTitleFont = new Font(FontFamilyName, 11.0f, FontStyle.Bold);
            _selfFoodTitleLineHeight = _selfFoodTitleFont.GetHeight() + LineHeading;

            _selfFoodTitleDescriptionFont = new Font(FontFamilyName, 11.0f, FontStyle.Regular);
            _selfFoodTitleDescriptionLineHeight = _selfFoodTitleDescriptionFont.GetHeight() + LineHeading;

            _selfFishNumberFont = new Font(FontFamilyName, 9.0f, FontStyle.Regular);
            _selfFishNumberLineHeight = _selfFishNumberFont.GetHeight() + LineHeading;

            _selfManualBillFont = new Font("Tahoma", 13.0f, FontStyle.Bold);
            //_selfManualBillLineHeight = _selfManualBillFont.GetHeight() + LineHeading;


            #endregion
        }

        static PrintService()
        {
            Instance = new PrintService();
        }

        #endregion


        public void StartPrintService(PrintServiceSetting setting)
        {
            _setting = setting;
            var repositoryFactory = new RepositoryFactory();
            _karnamaComponent = new KarnamaComponent(repositoryFactory);
            _printerPingThreads = new Thread(PrintersPingProcess);
            _printerPingThreads.Start();
            LoggingSystem.LogInfo("PrintService is started", _setting);
        }


        #region Self

        private const string FontFamilyName = "B Nazanin";
        private const string OfflinePrinterErrorMessage = "Printer is not online";
        private const int YMargin = 5;
        private const int XMargin = 5;
        private const float LineHeading = 2;
        private const int PrintAreaWidth = 580;

        private readonly ConcurrentDictionary<string, string> _selfRegisteredPrinters = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, bool> _selfPingPrinterResult = new ConcurrentDictionary<string, bool>();
        private readonly List<ConcurrentQueue<DtoSelfBillInfo>> _printQueues = new List<ConcurrentQueue<DtoSelfBillInfo>>();
        private readonly List<Thread> _selfQueueProcessorThreads = new List<Thread>();
        private Thread _printerPingThreads;

        private readonly Random _randomGenerator = new Random();

        private readonly Font _selfLinesFont;
        private readonly Font _selfManualBillFont;
        private readonly Font _selfHeaderTitleFont;
        private readonly Font _selfEmployeeTitleFont;
        private readonly Font _selfIssueDateFont;
        private readonly Font _selfFoodTitleFont;
        private readonly Font _selfFoodTitleDescriptionFont;
        private readonly Font _selfFishNumberFont;


        private readonly float _selfLinesLineHeight;
        private readonly float _selfHeaderTitleLineHeight;
        private readonly float _selfEmployeeTitleLineHeight;
        private readonly float _selfIssueDateLineHeight;
        private readonly float _selfFoodTitleLineHeight;
        private readonly float _selfFoodTitleDescriptionLineHeight;
        private readonly float _selfFishNumberLineHeight;


        public DtoSelfPrintResult DirectPrintSelfBill(DtoSelfBillInfo billInfo)
        {
            if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
            {
                LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Direct Print Data Received", billInfo);
            }

            if (ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Self))
            {
                try
                {
                    var printRequirement = GetSelfPrintRequirement(billInfo, new SelfPrintAssets());
                    using (printRequirement.PrintDocument)
                    {
                        printRequirement.PrintDocument.Print();
                    }
                    return new DtoSelfPrintResult
                    {
                        IsSuccessful = true,
                        ErrorMessage = string.Empty,
                        Id = billInfo.Id,
                        AdditionalData = billInfo.AdditionalData,
                    };
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, $"PrintService Id: [{billInfo.Id}]-Error on print self bill", billInfo);
                    return new DtoSelfPrintResult
                    {
                        IsSuccessful = false,
                        ErrorMessage = exp.GetFullExceptionMessage(),
                        Id = billInfo.Id,
                        AdditionalData = billInfo.AdditionalData,
                    };
                }
            }

            return new DtoSelfPrintResult
            {
                IsSuccessful = false,
                ErrorMessage = "Self software not supported",
                Id = billInfo.Id,
                AdditionalData = billInfo.AdditionalData,
            };
        }

        public void AddToSelfBillQueue(DtoSelfBillInfo billInfo)
        {
            if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
            {
                LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Print Data Received", billInfo);
            }
            if (ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Self))
            {
                if (!_selfRegisteredPrinters.TryGetValue(billInfo.PrinterName, out _))
                {
                    if (!NetworkHelpers.PingHost(billInfo.PrinterIp, _setting.PingTimeoutInSecond))
                    {
                        if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
                        {
                            LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Printer not registered and printer is NOT online", billInfo);
                        }

                        ReportResultToKarnama(new DtoSelfPrintResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = OfflinePrinterErrorMessage,
                            Id = billInfo.Id,
                            AdditionalData = billInfo.AdditionalData,
                        });
                        return;
                    }
                    if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
                    {
                        LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Printer not registered but is online", billInfo);
                    }
                    _selfRegisteredPrinters[billInfo.PrinterName] = billInfo.PrinterIp;
                    _selfPingPrinterResult[billInfo.PrinterName] = true;
                    if (_selfRegisteredPrinters.Count > _printQueues.Count * _setting.PrinterPerQueue)
                    {
                        var newQueue = new ConcurrentQueue<DtoSelfBillInfo>();
                        _printQueues.Add(newQueue);
                        var selfPrinterThread = new Thread(() => QueueProcessor(newQueue));
                        _selfQueueProcessorThreads.Add(selfPrinterThread);
                        selfPrinterThread.Start();
                    }
                }

                var printerIsOnline = false;
                if (_selfPingPrinterResult.TryGetValue(billInfo.PrinterName, out var pingResult))
                {
                    if (pingResult)
                    {
                        printerIsOnline = true;
                    }
                }

                if (printerIsOnline)
                {
                    var index = _randomGenerator.Next(0, _printQueues.Count);
                    _printQueues[index].Enqueue(billInfo);

                    if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
                    {
                        LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Print Registered in queue", new
                        {
                            BillInfo = billInfo,
                            Index = index,
                        });
                    }

                }
                else
                {
                    if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceCall))
                    {
                        LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Printer is not online", new
                        {
                            BillInfo = billInfo,
                        });
                    }
                    ReportResultToKarnama(new DtoSelfPrintResult
                    {
                        IsSuccessful = false,
                        ErrorMessage = OfflinePrinterErrorMessage,
                        Id = billInfo.Id,
                        AdditionalData = billInfo.AdditionalData,
                    });
                }
            }

        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private void QueueProcessor(ConcurrentQueue<DtoSelfBillInfo> printQueue)
        {
            var printAssets = new SelfPrintAssets();

            while (true)
            {
                DtoSelfBillInfo billInfo = null;
                try
                {
                    if (printQueue.IsEmpty)
                    {
                        Thread.Sleep(_setting.SleepWhenQueueIsEmptyInMillisecond);
                        continue;
                    }
                    if (printQueue.TryDequeue(out billInfo))
                    {
                        var printerIsOnline = false;
                        if (_selfPingPrinterResult.TryGetValue(billInfo.PrinterName, out var pingResult))
                        {
                            if (pingResult)
                            {
                                printerIsOnline = true;
                            }
                        }
                        if (printerIsOnline)
                        {
                            if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceQueueProcess))
                            {
                                LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-Process of print is started", billInfo);
                            }
                            var printRequirement = GetSelfPrintRequirement(billInfo, printAssets);
                            using (printRequirement.PrintDocument)
                            {
                                printRequirement.PrintDocument.Print();
                            }
                        }
                        else
                        {
                            if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceQueueProcess))
                            {
                                LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-During process of print, printer is not online", billInfo);
                            }
                            ReportResultToKarnama(new DtoSelfPrintResult
                            {
                                IsSuccessful = false,
                                ErrorMessage = OfflinePrinterErrorMessage,
                                Id = billInfo.Id,
                                AdditionalData = billInfo.AdditionalData,
                            });
                        }
                    }
                }
                catch (Exception exp)
                {
                    if (billInfo != null)
                    {
                        ReportResultToKarnama(new DtoSelfPrintResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = exp.GetFullExceptionMessage(),
                            Id = billInfo.Id,
                            AdditionalData = billInfo.AdditionalData,
                        });
                    }
                    LoggingSystem.LogError(exp, "PrintService Error on print self bill", billInfo);
                }
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private void PrintersPingProcess()
        {
            while (true)
            {
                try
                {
                    var printerNames = _selfRegisteredPrinters.Keys;
                    foreach (var printerName in printerNames)
                    {
                        try
                        {
                            _selfPingPrinterResult[printerName] = NetworkHelpers.PingHost(_selfRegisteredPrinters[printerName], _setting.PingTimeoutInSecond);
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "PrintService Error on ping device");
                        }
                    }
                    Thread.Sleep(_setting.SleepAfterPingCircleInMillisecond);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "PrintService Error on ping device");
                }
            }
        }



        private void PrintPageSelfBill(PrintPageEventArgs e, DtoSelfBillInfo billInfo, SelfPrintAssets printAssets)
        {
            try
            {
                if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServicePrintProcess))
                {
                    LoggingSystem.LogInfo($"PrintService Id: [{billInfo.Id}]-START print page process", billInfo);
                }

                #region Graphic settings

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.PageUnit = GraphicsUnit.Pixel;

                #endregion

                var currentLineOffset = _selfLinesLineHeight * 0.5F;
                SizeF currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfLinesLineHeight);
                RectangleF currentLineRectangle;
                if (billInfo.HeaderTitles.IsCollectionNotNullOrEmpty())
                {
                    currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfHeaderTitleLineHeight);
                    foreach (var foodTitle in billInfo.HeaderTitles)
                    {
                        currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset),
                            currentLineSize);
                        e.Graphics.DrawString(foodTitle, _selfHeaderTitleFont, printAssets.PrintBrushHeader,
                            currentLineRectangle,
                            printAssets.TextFormatter);
                        currentLineOffset += _selfHeaderTitleLineHeight * 1.75F;
                    }
                }
                currentLineOffset += _selfHeaderTitleLineHeight * 0.5F;

                currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfEmployeeTitleLineHeight);
                currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset), currentLineSize);
                e.Graphics.DrawString(billInfo.EmployeeTitle, _selfEmployeeTitleFont,
                    printAssets.PrintBrushEmployee,
                    currentLineRectangle,
                    printAssets.TextFormatter);
                currentLineOffset += _selfEmployeeTitleLineHeight * 2F;


                currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfIssueDateLineHeight);
                currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset), currentLineSize);
                e.Graphics.DrawString(billInfo.GetIssueDateString(), _selfIssueDateFont,
                    printAssets.PrintBrushDate,
                    currentLineRectangle,
                    printAssets.TextFormatter);
                currentLineOffset += _selfIssueDateLineHeight * 1.5F;

                e.Graphics.DrawLine(printAssets.PenLine, 0, currentLineOffset, PrintAreaWidth - XMargin, (int)currentLineOffset);
                currentLineOffset += _selfLinesLineHeight * 1F;


                if (billInfo.FoodInfos.IsCollectionNotNullOrEmpty())
                {
                    for (var index = 0; index < billInfo.FoodInfos.Count; index++)
                    {
                        var foodInfo = billInfo.FoodInfos[index];
                        if (!foodInfo.IsAllowed)
                        {
                            // Draw Invalid Back rectangle
                            var invalidFoodAreaSize = foodInfo.Description.IsNotNullOrEmpty()
                                ? new SizeF(PrintAreaWidth - XMargin * 2, (float)((_selfFoodTitleLineHeight + _selfFoodTitleDescriptionLineHeight) * 1.7) + 10)
                                : new SizeF(PrintAreaWidth - XMargin * 2, (float)((_selfFoodTitleLineHeight * 1.2) + 10));
                            currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset - 10), invalidFoodAreaSize);
                            e.Graphics.FillRectangle(printAssets.PrintBrushFoodTitleBackgroundInvalid, currentLineRectangle);


                            // Now we begin to print 
                            currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfFoodTitleLineHeight);
                            currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + (currentLineOffset)),
                                currentLineSize);
                            e.Graphics.DrawString(foodInfo.GetShowString(), _selfFoodTitleFont,
                                printAssets.PrintBrushFoodTitleInvalid,
                                currentLineRectangle,
                                printAssets.TextFormatter);
                            currentLineOffset += _selfFoodTitleLineHeight * 2F;

                            if (foodInfo.Description.IsNotNullOrEmpty())
                            {
                                currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfFoodTitleDescriptionLineHeight);
                                currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset),
                                    currentLineSize);
                                e.Graphics.DrawString(foodInfo.Description,
                                    _selfFoodTitleDescriptionFont,
                                    printAssets.PrintBrushFoodTitleInvalid,
                                    currentLineRectangle,
                                    printAssets.TextFormatter);
                                currentLineOffset += _selfFoodTitleDescriptionLineHeight * 2F;
                            }
                        }
                        else
                        {
                            currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfFoodTitleLineHeight);
                            currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset), currentLineSize);
                            e.Graphics.DrawString(foodInfo.GetShowString(), _selfFoodTitleFont,
                                printAssets.PrintBrushFoodTitle,
                                currentLineRectangle,
                                printAssets.TextFormatter);
                            currentLineOffset += _selfFoodTitleLineHeight * 2F;

                            if (foodInfo.Description.IsNotNullOrEmpty())
                            {
                                currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfFoodTitleDescriptionLineHeight);
                                currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset),
                                    currentLineSize);
                                e.Graphics.DrawString(foodInfo.Description,
                                    _selfFoodTitleDescriptionFont,
                                    printAssets.PrintBrushFoodTitle,
                                    currentLineRectangle,
                                    printAssets.TextFormatter);
                                currentLineOffset += _selfFoodTitleDescriptionLineHeight * 2F;
                            }
                        }

                        if (index != billInfo.FoodInfos.Count - 1)
                        {
                            e.Graphics.DrawLine(printAssets.PenLineFoodSeparator, 0, currentLineOffset,
                                PrintAreaWidth - XMargin, (int)currentLineOffset);
                            currentLineOffset += _selfLinesLineHeight * 0.5F;
                        }
                    }
                }
                currentLineSize = new SizeF(PrintAreaWidth - XMargin * 2, _selfFishNumberLineHeight);
                currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset),
                    currentLineSize);
                e.Graphics.DrawString(billInfo.FishNumber, _selfFishNumberFont,
                    printAssets.PrintBrushFishNumber, currentLineRectangle,
                    printAssets.TextFormatter);
                currentLineOffset += _selfFishNumberLineHeight * 2F;

                e.Graphics.DrawRectangle(printAssets.PenRectangle,
                    new Rectangle(0, 0, PrintAreaWidth - XMargin, (int)currentLineOffset));

                if (billInfo.ManualBill.IsNotNullOrEmpty())
                {
                    currentLineOffset += _selfLinesLineHeight * 1F;
                    currentLineRectangle = new RectangleF(new PointF(XMargin, YMargin + currentLineOffset),
                        currentLineSize);
                    e.Graphics.DrawString(billInfo.ManualBill, _selfManualBillFont,
                        printAssets.PrintBrushManualBill, currentLineRectangle,
                        printAssets.TextFormatter);
                }


                ReportResultToKarnama(new DtoSelfPrintResult
                {
                    IsSuccessful = true,
                    ErrorMessage = string.Empty,
                    Id = billInfo.Id,
                    AdditionalData = billInfo.AdditionalData,
                });
                if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServicePrintProcess))
                {
                    LoggingSystem.LogInfo($"PrintServiceId: [{billInfo.Id}]-END of print page process", new
                    {
                        BillInfo = billInfo,
                    });
                }
            }
            catch (Exception exp)
            {

                LoggingSystem.LogError(exp, "PrintService Error on print self bill", billInfo);
                ReportResultToKarnama(new DtoSelfPrintResult
                {
                    IsSuccessful = false,
                    ErrorMessage = string.Empty,
                    Id = billInfo.Id,
                    AdditionalData = billInfo.AdditionalData,
                });
            }
            finally
            {
                e.HasMorePages = false;
            }
        }

        private void ReportResultToKarnama(DtoSelfPrintResult result)
        {
            var thread = new Thread(() => ReportResultToKarnamaAction(result));
            thread.Start();
        }

        private void ReportResultToKarnamaAction(DtoSelfPrintResult result)
        {
            try
            {
                if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceSendResult))
                {
                    LoggingSystem.LogInfo("PrintService Print Result sending to karnama", new
                    {
                        PrintResult = result,
                    });
                }
                if (AppConfigs.LogLevelPrintService.HasFlag(LogLevelPrintServiceEnumeration.PrintServiceErrorsOnSend) && !result.IsSuccessful)
                {
                    LoggingSystem.LogInfo("PrintService error", new
                    {
                        PrintResult = result,
                    });
                }
                _karnamaComponent.ReportSelfPrintResult(result);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "PrintService Error on send print result", result);
            }
        }

        private SelfPrintRequirement GetSelfPrintRequirement(DtoSelfBillInfo billInfo, SelfPrintAssets printAssets)
        {
            var pd = new PrintDocument
            {
                PrinterSettings = new PrinterSettings
                {
                    PrinterName = billInfo.PrinterName,
                    DefaultPageSettings =
                    {
                        Margins =
                        {
                            Bottom = 0,
                            Top = 0,
                            Left = 0,
                            Right = 0
                        }
                    }
                },
                DefaultPageSettings =
                {
                    Margins =
                    {
                        Bottom = 0,
                        Top = 0,
                        Left = 0,
                        Right = 0
                    }
                }
            };
            pd.PrintPage += (sender, e) => PrintPageSelfBill(e, billInfo, printAssets);
            return new SelfPrintRequirement
            {
                PrintDocument = pd,
            };
        }

        #endregion


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Free any other managed objects here.
            }
            // Free Unmanaged resource

            if (_selfQueueProcessorThreads.IsCollectionNotNullOrEmpty())
            {
                foreach (var queueProcessorThread in _selfQueueProcessorThreads)
                {
                    queueProcessorThread.Join();
                }
            }
            _printerPingThreads.Join();

            _selfLinesFont.Dispose();
            _selfManualBillFont.Dispose();
            _selfHeaderTitleFont.Dispose();

            _selfEmployeeTitleFont.Dispose();
            _selfIssueDateFont.Dispose();

            _selfFoodTitleFont.Dispose();
            _selfFishNumberFont.Dispose();

            _disposed = true;
        }

        ~PrintService()
        {
            Dispose(false);
        }

        #endregion

        private class SelfPrintRequirement
        {
            public PrintDocument PrintDocument { get; set; }
        }

        private class SelfPrintAssets
        {
            public Pen PenLine { get; } = new Pen(Color.Black, 5);
            public Pen PenLineFoodSeparator { get; } = new Pen(Color.Black, 2);
            public Pen PenRectangle { get; } = new Pen(Color.Black, 5);
            public Brush PrintBrushHeader { get; } = Brushes.Black;
            public Brush PrintBrushDate { get; } = Brushes.Black;
            public Brush PrintBrushEmployee { get; } = Brushes.Black;
            public Brush PrintBrushFoodTitle { get; } = Brushes.Black;
            //public Brush PrintBrushFoodTitleInvalid { get; } = new HatchBrush(
            //    HatchStyle.DashedDownwardDiagonal,
            //    Color.Black,
            //    Color.White);
            public Brush PrintBrushFoodTitleBackgroundInvalid { get; } = Brushes.Black;
            public Brush PrintBrushFoodTitleInvalid { get; } = Brushes.White;

            // public TextureBrush InvalidTextureBrush = new TextureBrush(yourImage, WrapMode.Tile)
            //public Brush PrintBrushFishCount { get; } = Brushes.te;
            public Brush PrintBrushFishNumber { get; } = Brushes.Black;
            public Brush PrintBrushManualBill { get; } = Brushes.Black;

            public StringFormat TextFormatter { get; } = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                //FormatFlags = 
            };
        }

    }
}
