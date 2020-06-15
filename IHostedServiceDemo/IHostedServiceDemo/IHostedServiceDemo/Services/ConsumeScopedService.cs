using IHostedServiceDemo.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IHostedServiceDemo.Services
{
    public class ConsumeScopedService : IHostedService, IDisposable
    {
        public ConsumeScopedService(IServiceProvider services)
        {
            Services = services;
        }

        private Timer _timer;

        public IServiceProvider Services { get; }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);//Desactivado

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var message = "ConsumeScopedService. Received message at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                var log = new HostedServiceLogs() { Message = message };

                context.HostedServiceLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}
