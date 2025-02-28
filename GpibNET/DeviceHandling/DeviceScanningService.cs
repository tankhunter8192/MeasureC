using System;
using System.Threading;
using System.Threading.Tasks;
using Gpib.Web.Data;
using Gpib.Web.Data.DBClasses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Runtime.InteropServices;
using IronPython.Modules;
using Ivi.Visa;
//using NationalInstruments.Visa; #<- Alternative to Ivi.Visa.Interop maybe usable for Linux
using Ivi.Visa.Interop;
using Timeout = System.Threading.Timeout;

namespace Gpib.Web.DeviceHandling
{
    public class DeviceScanningService : IHostedService, IDisposable
    {
        private readonly ILogger<DeviceScanningService> _logger;
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        public DateTime LastScanTime { get; private set; }
        public TimeSpan ScanDuration { get; private set; }
        public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMinutes(1);
        public DeviceScanningService(ILogger<DeviceScanningService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _timer = new Timer(ScanDevices, null, uint.MaxValue, uint.MaxValue);
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Device scanning service running.");
            _timer.Change(TimeSpan.Zero, ScanInterval);
            return Task.CompletedTask;
        }

        private void ScanDevices(object state)
        {
            DateTime startTime = DateTime.Now;
            _logger.LogInformation("Scanning devices..." + startTime.ToString());
            /*
            if (GlobalStaticVariables.PyVisaWrapper is null)
            {
                GlobalStaticVariables.PyVisaWrapper = new PyVisaWrapper();

            }
            GlobalStaticVariables.PyVisaWrapper.ScanDevices();
            */
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var rm = new ResourceManager();
                    var devices = rm.FindRsrc("?*INSTR");
                    foreach (var device in devices)
                    {
                        _logger.LogInformation("Found device: " + device);
                        if (!(dbContext.Devices.Any(d=> d.ConnectionString == device)))
                        {
                            _logger.LogInformation("Found new Device: " + device);
                            dbContext.Devices.Add(new GPIBDevice() { ConnectionString = device });
                            dbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while scanning devices: " + e.Message);
                }
            }
            
            
            ScanDuration = DateTime.Now - startTime;
            LastScanTime = DateTime.Now;
            GlobalStaticVariables.LastScanDateTime = LastScanTime;
            _logger.LogInformation("Scan completed in " + ScanDuration.ToString() + " at " + LastScanTime.ToString());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            _logger.LogInformation("Device scanning service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
