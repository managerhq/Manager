using ManagerServer.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Status : HttpHandler
    {
        public override Task Get()
        {
            Response.ContentType = "text/plain";
            var sb = new StringBuilder();
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            currentProcess.Refresh();
            sb.AppendLine($"WorkingSet64               : {currentProcess.WorkingSet64.ToString("#,#")}");
            sb.AppendLine($"PeakWorkingSet64           : {currentProcess.PeakWorkingSet64.ToString("#,#")}");
            sb.AppendLine($"NonpagedSystemMemorySize64 : {currentProcess.NonpagedSystemMemorySize64.ToString("#,#")}");
            sb.AppendLine($"PagedMemorySize64          : {currentProcess.PagedMemorySize64.ToString("#,#")}");
            sb.AppendLine($"PagedSystemMemorySize64    : {currentProcess.PagedSystemMemorySize64.ToString("#,#")}");
            sb.AppendLine($"PeakPagedMemorySize64      : {currentProcess.PeakPagedMemorySize64.ToString("#,#")}");
            sb.AppendLine($"PeakVirtualMemorySize64    : {currentProcess.PeakVirtualMemorySize64.ToString("#,#")}");
            sb.AppendLine($"PrivateMemorySize64        : {currentProcess.PrivateMemorySize64.ToString("#,#")}");
            sb.AppendLine($"VirtualMemorySize64        : {currentProcess.VirtualMemorySize64.ToString("#,#")}");
            sb.AppendLine($"StartTime                  : {(DateTime.UtcNow - currentProcess.StartTime.ToUniversalTime()).TotalHours} hours ago");
            sb.AppendLine($"IdleTime                   : {HttpContext.RequestServices.GetRequiredService<IdleTracker>().Idle.TotalSeconds} seconds");

            ThreadPool.GetMaxThreads(out int maxWorker, out int maxIO);
            ThreadPool.GetAvailableThreads(out int availWorker, out int availIO);
            ThreadPool.GetMinThreads(out int minWorker, out int minIO);

            int inUseWorker = maxWorker - availWorker;
            int inUseIO = maxIO - availIO;

            sb.AppendLine();
            sb.AppendLine($"ThreadCount:              {ThreadPool.ThreadCount}");
            sb.AppendLine($"PendingWorkItemCount:     {ThreadPool.PendingWorkItemCount}");
            sb.AppendLine($"CompletedWorkItemCount:   {ThreadPool.CompletedWorkItemCount}");
            sb.AppendLine($"Worker threads:           In use: {inUseWorker}, Available: {availWorker}, Max: {maxWorker}, Min: {minWorker}");
            sb.AppendLine($"I/O threads:              In use: {inUseIO}, Available: {availIO}, Max: {maxIO}, Min: {minIO}");
                        
            var gcInfo = GC.GetGCMemoryInfo();
            sb.AppendLine();
            sb.AppendLine($"Heap size: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
            sb.AppendLine($"High memory load threshold: {gcInfo.HighMemoryLoadThresholdBytes / 1024 / 1024} MB");
            sb.AppendLine($"Total available memory: {gcInfo.TotalAvailableMemoryBytes / 1024 / 1024} MB");

            sb.AppendLine();
            sb.AppendLine($"Is Server GC: {System.Runtime.GCSettings.IsServerGC}");
            sb.AppendLine($"GC Latency Mode: {System.Runtime.GCSettings.LatencyMode}");

            Write(sb.ToString());

            return Task.CompletedTask;
        }
    }
}