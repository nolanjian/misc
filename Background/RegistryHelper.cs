using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Background
{
    public class RegistryHelper : BackgroundService
    {
        public ILogger Logger { get; }
        public RegistryHelper(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<RegistryHelper>();
        }
        public bool UpdateRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Scooter Software\Beyond Compare 4", true))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("CacheId");
                        if (o != null)
                        {
                            key.DeleteValue("CacheId");
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogInformation(e, "UpdateRegistry");
                return false;
            }

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("RegistryHelper is starting.");

            stoppingToken.Register(() => Logger.LogInformation("RegistryHelper is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("RegistryHelper is doing background work.");
                bool bRet = UpdateRegistry();
                Logger.LogInformation("Update Registry {0}", bRet);
                if (bRet)
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            Logger.LogInformation("RegistryHelper has stopped.");
        }
    }
}
