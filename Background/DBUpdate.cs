using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Background
{
    public class DBUpdate : BackgroundService
    {
        public ILogger Logger { get; }
        string srcPath { get; set; } = @"\\10.10.18.107\DiyHomeWeb\SQLiteDB\SQLiteDB.sqlite";
        string targetPath { get; set; } = @"D:\DIYHome\V3.6.2.05-R\SQLiteDB\SQLiteDB.sqlite";
        public DBUpdate(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<DBUpdate>();
        }
        public bool CopyDB()
        {
            try
            {
                if (File.Exists(srcPath))
                {
                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }
                    File.Copy(srcPath, targetPath);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error in Copy DB");
                return false;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("DBUpdate is starting.");

            stoppingToken.Register(() => Logger.LogInformation("DBUpdate is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("DBUpdate is doing background work.");
                bool bRet = CopyDB();
                Logger.LogInformation("CopyDB {0}", bRet);
                if (bRet)
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }

            Logger.LogInformation("DBUpdate has stopped.");
        }
    }
}
