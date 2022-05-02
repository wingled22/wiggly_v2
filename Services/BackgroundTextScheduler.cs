using EasyExtensions.BackgroundServiceExtensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wiggly.Services
{
    public class BackgroundTextScheduler : ScheduledServiceBase
    {
        private readonly IServiceProvider _serviceProvider;
        public BackgroundTextScheduler(IServiceProvider serviceProvider ,ScheduledServiceOptions<BackgroundTextScheduler> options): base(options.Expression)
        {
            _serviceProvider = serviceProvider;
        }
        public override Task ExecuteJobAsync(CancellationToken cancellationToken) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //throw new NotImplementedException();
                Console.WriteLine("scheduledtext");
            }
           return Task.CompletedTask;
        }
    }
}
