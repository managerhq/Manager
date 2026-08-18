using ManagerServer.Storage;
using System;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class StorageStats : Template
    {
        [ProtoMember(1)] public string Action;
        [ProtoMember(2)] public int Remaining;

        protected override async Task InnerGet()
        {
            var stats = await ApplicationData.Storage.Stats();

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            if (Remaining > 0)
                                using (Div(@class: "text-sm text-neutral-500")) Write($"{Remaining} remaining to pull from S3");

                            using (Table(@class: "w-full text-sm text-left"))
                            {
                                using (Tr(@class: "text-xs uppercase text-neutral-500 border-b border-neutral-200 dark:border-neutral-700"))
                                {
                                    using (Th(@class: "px-4 py-3")) Write("Component");
                                    using (Th(@class: "px-4 py-3 text-right")) Write("Count");
                                    using (Th(@class: "px-4 py-3 text-right")) Write("Size");
                                }
                                using (Tr(@class: "border-b border-neutral-100 dark:border-neutral-800"))
                                {
                                    using (Td(@class: "px-4 py-3 font-medium")) Write("Blobs");
                                    using (Td(@class: "px-4 py-3 text-right")) Write(stats.BlobCount.ToString("#,#"));
                                    using (Td(@class: "px-4 py-3 text-right")) Write(FormatSize(stats.BlobSize));
                                }
                                using (Tr(@class: "border-b border-neutral-100 dark:border-neutral-800"))
                                {
                                    using (Td(@class: "px-4 py-3 font-medium text-neutral-500")) Write("Oversized blobs");
                                    using (Td(@class: "px-4 py-3 text-right text-neutral-500")) Write(stats.OversizedBlobCount.ToString("#,#"));
                                    using (Td(@class: "px-4 py-3 text-right text-neutral-500")) Write(FormatSize(stats.OversizedBlobSize));
                                }
                                using (Tr(@class: "border-b border-neutral-100 dark:border-neutral-800"))
                                {
                                    using (Td(@class: "px-4 py-3 font-medium")) Write("Packs");
                                    using (Td(@class: "px-4 py-3 text-right")) Write(stats.PackCount.ToString("#,#"));
                                    using (Td(@class: "px-4 py-3 text-right")) Write(FormatSize(stats.PackSize));
                                }
                                foreach (var bucket in stats.PackBuckets)
                                {
                                    var lower = 1L << bucket.Bucket;
                                    var upper = 1L << (bucket.Bucket + 1);
                                    var sealedBucket = lower >= stats.MaxPackSize;
                                    var rowClass = sealedBucket
                                        ? "border-b border-neutral-100 dark:border-neutral-800 text-neutral-400"
                                        : "border-b border-neutral-100 dark:border-neutral-800 text-neutral-500";
                                    using (Tr(@class: rowClass))
                                    {
                                        using (Td(@class: "px-4 py-2 pl-8 text-xs")) Write($"{FormatSize(lower)} – {FormatSize(upper)}");
                                        using (Td(@class: "px-4 py-2 text-right text-xs")) Write(bucket.Count.ToString("#,#"));
                                        using (Td(@class: "px-4 py-2 text-right text-xs")) Write(FormatSize(bucket.Size));
                                    }
                                }
                                using (Tr(@class: "border-b border-neutral-100 dark:border-neutral-800"))
                                {
                                    using (Td(@class: "px-4 py-3 font-medium")) Write("Filters");
                                    using (Td(@class: "px-4 py-3 text-right")) Write(stats.FilterCount.ToString("#,#"));
                                    using (Td(@class: "px-4 py-3 text-right")) Write(FormatSize(stats.FilterSize));
                                }
                                using (Tr())
                                {
                                    using (Td(@class: "px-4 py-3 font-medium")) Write("Indexes");
                                    using (Td(@class: "px-4 py-3 text-right")) Write(stats.IndexCount.ToString("#,#"));
                                    using (Td(@class: "px-4 py-3 text-right")) Write(FormatSize(stats.IndexSize));
                                }
                            }

                            Hr();

                            using (Div(@class: "flex gap-2"))
                            {
                                using (Form(action: new StorageStats { Action = "pack" }.ToUrl(), method: "POST", hxBoost: true))
                                    using (Button(@class: "btn btn-primary")) Write("Pack");

                                using (Form(action: new StorageStats { Action = "consolidate" }.ToUrl(), method: "POST", hxBoost: true))
                                    using (Button(@class: "btn btn-primary")) Write("Consolidate");
                            }
                        }
                    }
                }
            }
        }

        protected override async Task InnerPost()
        {
            if (Action == "pack")
                await ApplicationData.Storage.Pack();
            else if (Action == "consolidate")
                await ApplicationData.Storage.Consolidate();

            Response.Redirect(new StorageStats().ToUrl());
        }        

        static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
