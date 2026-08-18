using System.Runtime;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Gc : HttpHandler
    {
        public override Task Get()
        {
            var gcInfo = GC.GetGCMemoryInfo();
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                Write($"Collection count {i}: {GC.CollectionCount(0).ToString()}");
                Br();
            }
            Write($"Total pause duration: {GC.GetTotalPauseDuration().TotalSeconds.ToString()}s");
            Br();
            Br();
            Write($"Heap size: {(gcInfo.HeapSizeBytes / (1024 * 1024)).ToString("#,#")} MB");
            Br();
            Write($"Fragmented memory: {(gcInfo.FragmentedBytes / (1024 * 1024)).ToString("#,#")} MB");
            Br();
            Write($"Memory Load: {(gcInfo.MemoryLoadBytes / (1024 * 1024)).ToString("#,#")} MB");            
            Br();
            Write($"Total committed memory: {(gcInfo.TotalCommittedBytes / (1024 * 1024)).ToString("#,#")} MB");
            Br();
            Write($"Total available memory: {(gcInfo.TotalAvailableMemoryBytes / (1024 * 1024)).ToString("#,#")} MB");
            Br();
            Write($"Index: {gcInfo.Index.ToString("#,#")}");
            Br();
            Br();

            using (Form(method: "POST"))
            {
                InputSubmit(value: "GC Collect");
            }

            return Task.CompletedTask;
        }

        public override Task Post()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, mode: GCCollectionMode.Forced, blocking: true, compacting: true);
            Response.Redirect(this.ToUrl());
            return Task.CompletedTask;
        }
    }
}
