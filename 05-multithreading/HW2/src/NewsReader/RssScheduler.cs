using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASK3.src.NewsReader
{
    public class RssScheduler
    {
        public async Task RssSchedulerJob()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogger());

            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<RssNewsJob>().WithIdentity("rssJob", "rssGroup").Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("rssTrigger", "rssGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(3)
                    //.WithIntervalInSeconds(20)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
