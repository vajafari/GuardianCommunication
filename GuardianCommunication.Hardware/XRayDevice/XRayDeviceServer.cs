using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;

namespace GuardianCommunication.Hardware.XRayDevice
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public class XRayDeviceServer : IDisposable
    {
        private XReaDeviceServerConfig _serverConfig;
        #region Singleton

        public static XRayDeviceServer Instance { get; }

        private XRayDeviceServer()
        {
        }

        static XRayDeviceServer()
        {
            Instance = new XRayDeviceServer();
        }

        #endregion


        public void StartDeviceServer(List<DtoXRayDevice> deviceInfos, XReaDeviceServerConfig config)
        {
            LoggingSystem.LogInfo("XRayDeviceServer started", config);
            _serverConfig = config;
            var thread = new Thread(() => DoStartProcessForFileBaseDevice(deviceInfos)) { IsBackground = true };
            thread.Start();
        }


        public void StopServer()
        {
            Dispose(true);
        }


        #region File base devices

        private readonly List<DtoXRayDevice> _fileBaseDevices = new List<DtoXRayDevice>();
        private readonly ConcurrentDictionary<int, CustomFileSystemWatcher> _directoryWatchers = new ConcurrentDictionary<int, CustomFileSystemWatcher>();
        private readonly ConcurrentQueue<XRayFileInfo> _allXRayDetectedFiles = new ConcurrentQueue<XRayFileInfo>();
        private Thread _fileQueueVerifierThread;
        private CancellationTokenSource _verifierCts;

        private void DoStartProcessForFileBaseDevice(List<DtoXRayDevice> deviceInfos)
        {
            SetDevicesListForFileBaseDevice(deviceInfos);
            StartListeningToDirectoriesForFileBaseDevices();
            _verifierCts = new CancellationTokenSource();
            _fileQueueVerifierThread = new Thread(VerifyFileQueue) { IsBackground = true };
            _fileQueueVerifierThread.Start();
        }

        private void SetDevicesListForFileBaseDevice(List<DtoXRayDevice> deviceInfos)
        {
            if (deviceInfos.IsCollectionNullOrEmpty())
            {
                return;
            }

            var pushDeviceInfos = deviceInfos.Where
                (row => row.DeviceType == XRayDeviceTypeEnumeration.SurfWay
                        && row.ConnectionMode == XRayDeviceConnectionModeEnumeration.File).ToList();

            if (AppConfigs.LogLevelXRay.HasFlag(LogLevelXRayEnumeration.DeviceList))
            {
                LoggingSystem.LogInfo("XRayServer File base device list", pushDeviceInfos);
            }
            lock (_fileBaseDevices)
            {
                try
                {
                    _fileBaseDevices.Clear();
                    _fileBaseDevices.AddRange(pushDeviceInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp
                        , "Error on SetFileBaseDevicesList");
                }
            }

        }

        private void StartListeningToDirectoriesForFileBaseDevices()
        {
            try
            {
                if (_fileBaseDevices.IsCollectionNotNullOrEmpty())
                {
                    foreach (var xRayDevice in _fileBaseDevices)
                    {
                        var watcher = new CustomFileSystemWatcher(xRayDevice.FilePath, xRayDevice)
                        {
                            NotifyFilter = NotifyFilters.Attributes
                                           | NotifyFilters.CreationTime
                                           | NotifyFilters.DirectoryName
                                           | NotifyFilters.FileName
                                           | NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.Security
                                           | NotifyFilters.Size,
                            Filter = "*.*",
                            IncludeSubdirectories = true,
                            EnableRaisingEvents = true,
                        };
                        watcher.Created += OnFileCreated;
                        if (!_directoryWatchers.TryAdd(watcher.GetHashCode(), watcher))
                        {
                            LoggingSystem.LogError("Error on add file watcher to system", $"Device id is {xRayDevice.Id}");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, $"Error on listening StartListeningToDirectories");
            }
        }

        private void StopListeningToDirectoriesForFileBaseDevices()
        {
            foreach (var customFileSystemWatcher in _directoryWatchers.Values)
            {
                customFileSystemWatcher.Dispose();
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {

                if (sender is CustomFileSystemWatcher fileWatcher)
                {
                    if (AppConfigs.LogLevelXRay.HasFlag(LogLevelXRayEnumeration.NewFile))
                    {
                        LoggingSystem.LogInfo("XRayServer new file dtected", new
                        {
                            DetectDateTime = DateTime.Now,
                            DeviceId = fileWatcher.XRayDevice.DeviceId,
                            FilePath = e.FullPath,
                        });
                    }
                    Thread.Sleep(_serverConfig.WaitBeforeAddToQueueInMilliSecond);
                    _allXRayDetectedFiles.Enqueue(new XRayFileInfo
                    {
                        DetectDateTime = DateTime.Now,
                        DeviceId = fileWatcher.XRayDevice.DeviceId,
                        FilePath = e.FullPath,
                    });
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on PublishXRayDeviceDataReceived on SurfWayXRayDeviceServer");
            }
        }

        private void VerifyFileQueue()
        {
            var token = _verifierCts.Token;
            while (!token.IsCancellationRequested)
            {
                XRayFileInfo currentItem = null;
                try
                {
                    if (_allXRayDetectedFiles.TryDequeue(out currentItem))
                    {
                        if (AppConfigs.LogLevelXRay.HasFlag(LogLevelXRayEnumeration.Publish))
                        {
                            LoggingSystem.LogInfo("XRayServer new file dtected", new
                            {
                                currentItem.DetectDateTime,
                                currentItem.DeviceId,
                                currentItem.FilePath,
                            });
                        }
                        HardwareEventPublisher.Instance.PublishXRayDeviceDataReceived
                            (currentItem.DeviceId, currentItem.DetectDateTime, File.ReadAllBytes(currentItem.FilePath));
                    }
                    else
                    {
                        token.WaitHandle.WaitOne(_serverConfig.SleepAfterNoFileInMilliSecond);
                    }

                }
                catch (IOException exp)
                {
                    if (currentItem != null)
                    {
                        if (Math.Abs(currentItem.DetectDateTime.Subtract(DateTime.Now).TotalSeconds) >
                            _serverConfig.IntervalToRetrySendInSecond)
                        {
                            LoggingSystem.LogError(exp, "Xray file cannot trasffered", currentItem);
                        }
                        else
                        {
                            _allXRayDetectedFiles.Enqueue(currentItem);
                        }
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on PublishXRayDeviceDataReceived on XRayDeviceServer", currentItem);
                }
            }
        }

        #endregion



        #region Types

        private class XRayFileInfo
        {
            public string DeviceId { get; set; }
            public DateTime DetectDateTime { get; set; }
            public string FilePath { get; set; }
        }

        #endregion


        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Managed teardown only on explicit Dispose — never on the finalizer thread.
                StopListeningToDirectoriesForFileBaseDevices();
                // Cooperative shutdown (replaces Thread.Abort): signal, wake the idle wait, join with a bound.
                _verifierCts?.Cancel();
                if (!(_fileQueueVerifierThread?.Join(5000) ?? true))
                {
                    LoggingSystem.LogInfo("XRayDeviceServer Dispose: verifier thread did not stop within timeout; continuing (background thread)");
                }
                _verifierCts?.Dispose();
                _verifierCts = null;
            }
            _disposed = true;
        }

        #endregion

    }


}
