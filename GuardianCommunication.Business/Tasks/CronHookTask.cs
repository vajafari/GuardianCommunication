using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Business.Tasks
{
    /// <summary>
    /// زمان بند: در هر تیک ردیف های فعال نوع Cron جدول com.HookDefinition از دیتابیس خوانده می شوند (ردیف یا Cron تازه بدون راه اندازی دوباره
    /// سرویس دیده می شود) و هر ردیفی که بعد از آخرین اجرایش زمانی از Cron آن رسیده است، جدا از بقیه اجرا می شود. ردیفی که هنوز اجرا نشده همان
    /// اولین تیک اجرا می شود، یک ردیف تا اجرای قبلی اش تمام نشده دوباره اجرا نمی شود و زمان های جاافتاده (مثلا وقتی سرویس خاموش بوده) روی هم
    /// فقط یک اجرا می شوند
    /// </summary>
    public class CronHookTask : TimedBaseTask
    {
        private readonly HookComponent _hookComponent;
        private readonly object _lock = new object();
        // ردیف هایی که اجرایشان هنوز تمام نشده است
        private readonly ConcurrentDictionary<Guid, bool> _runningHookDefinitionIds = new ConcurrentDictionary<Guid, bool>();
        // عبارت Cron نامعتبری که خطایش ثبت شده است، تا خطای هر عبارت یک بار ثبت شود نه در هر تیک
        private readonly ConcurrentDictionary<Guid, string> _loggedInvalidCrons = new ConcurrentDictionary<Guid, string>();

        public CronHookTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _hookComponent = new HookComponent(repositoryFactory);
            LoggingSystem.LogInfo($"CronHookTask started with interval {(int)interval.TotalSeconds} seconds");
        }

        public override void Process()
        {
            // اگر تیک قبلی هنوز ردیف ها را بررسی می کند، این تیک رد می شود
            if (!Monitor.TryEnter(_lock))
            {
                return;
            }
            try
            {
                var now = DateTime.Now;
                var hookDefinitions = _hookComponent.SearchHookDefinition(new PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration>
                {
                    Filter = new HookDefinitionFilter
                    {
                        HookTypes = new List<HookTypeEnumeration> { HookTypeEnumeration.Cron },
                        IsActive = 1,
                    }
                }, false);
                foreach (var hookDefinition in hookDefinitions)
                {
                    DateTime? nextRunAt;
                    try
                    {
                        nextRunAt = CronHelper.GetNextOccurrence(hookDefinition.Cron, hookDefinition.LastRunAt ?? now);
                    }
                    catch (FormatException exp)
                    {
                        if (!_loggedInvalidCrons.TryGetValue(hookDefinition.Id, out var loggedCron) || loggedCron != hookDefinition.Cron)
                        {
                            _loggedInvalidCrons[hookDefinition.Id] = hookDefinition.Cron;
                            LoggingSystem.LogHookError(exp, new { hookDefinition.Id, hookDefinition.EndPointUrl, hookDefinition.Cron });
                        }
                        continue;
                    }
                    var isDue = nextRunAt.HasValue && (!hookDefinition.LastRunAt.HasValue || nextRunAt.Value <= now);
                    if (!isDue || !_runningHookDefinitionIds.TryAdd(hookDefinition.Id, true))
                    {
                        continue;
                    }
                    // هر ردیف جدا اجرا می شود تا کار طولانی یک ردیف ردیف های دیگر را عقب نیندازد
                    Task.Run(() =>
                    {
                        try
                        {
                            _hookComponent.RunCronHook(hookDefinition, now);
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "CronHookTask run failed");
                        }
                        finally
                        {
                            _runningHookDefinitionIds.TryRemove(hookDefinition.Id, out _);
                        }
                    });
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "CronHookTask failed");
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
