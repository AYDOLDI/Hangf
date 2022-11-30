using Hangf.Jobs.Interfaces;
using System.Diagnostics;
using System.Text;

namespace Hangf.Jobs.Implementations
{
    public class CpuCounterJob: ICpuCounterJob
    {
        private readonly ILogger<CpuCounterJob> logger;

        public CpuCounterJob(ILogger<CpuCounterJob> logger)
        {
            this.logger = logger;
        }

        public async Task CpuCount()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(5000);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            logger.LogInformation($"CPU Usage: {cpuUsageTotal*100}");
        }
        
    }
}
