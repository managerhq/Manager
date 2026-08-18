using Amazon.S3;
using Amazon.S3.Model;
using ManagerServer.Storage;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer
{
    public class S3(string bucket, string bucketPrefix, string accessKey, string secretKey, string region) : IFileSystem
    {
        readonly ConcurrentDictionary<string, long> sizeCache = new();
        readonly AmazonS3Client s3 = new(accessKey, secretKey, new AmazonS3Config
        {
            UseDualstackEndpoint = true,
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
        });

        public static S3 FromUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return null;
            var u = new System.Uri(uri);
            var bucket = u.Host;
            var prefix = u.AbsolutePath.TrimStart('/');
            var accessKey = u.UserInfo.Split(':').First();
            var secretKey = u.UserInfo.Split(':').Last();
            var region = u.Query.TrimStart('?');
            return new S3(bucket, prefix, accessKey, secretKey, region);
        }

        string ToS3Key(string key) => $"{bucketPrefix}/{key}";

        public bool IsCloud => true;

        public async Task<string[]> GetKeysAsync(string prefix)
        {
            var keys = new System.Collections.Generic.List<string>();
            string continuationToken = null;
            var s3Prefix = ToS3Key(prefix);

            do
            {
                var response = await s3.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = bucket,
                    Prefix = s3Prefix,
                    ContinuationToken = continuationToken
                });

                var prefixLength = bucketPrefix.Length + 1;
                if (response.KeyCount > 0)
                {
                    foreach (var obj in response.S3Objects)
                    {
                        if (obj.Key == s3Prefix) continue;
                        var key = obj.Key[prefixLength..];
                        keys.Add(key);
                        if (obj.Size.HasValue) sizeCache[key] = obj.Size.Value;
                    }
                }

                continuationToken = response.IsTruncated == true ? response.NextContinuationToken : null;
            }
            while (continuationToken != null);

            return keys.ToArray();
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                await s3.GetObjectMetadataAsync(bucket, ToS3Key(key));
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<Stream> ReadAsync(string key, long? offset = null, int? length = null)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = ToS3Key(key)
                };

                if (offset.HasValue && length.HasValue)
                {
                    request.ByteRange = new ByteRange(offset.Value, offset.Value + length.Value - 1);
                }

                var response = await s3.GetObjectAsync(request);
                return new SubStream(response.ResponseStream, response.ContentLength);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task WriteAsync(string key, Stream stream)
        {
            const long maxPutSize = 5L * 1024 * 1024 * 1024; // S3 caps a single PUT at 5 GB
            if (stream.Length < maxPutSize)
            {
                await s3.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = ToS3Key(key),
                    InputStream = stream,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    AutoCloseStream = false
                });
                return;
            }

            var s3Key = ToS3Key(key);
            var partSize = System.Math.Max(64L * 1024 * 1024, stream.Length / 10000 + 1); // S3 allows at most 10,000 parts
            var upload = await s3.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
            {
                BucketName = bucket,
                Key = s3Key,
                StorageClass = S3StorageClass.IntelligentTiering
            });

            try
            {
                var partETags = new System.Collections.Generic.List<PartETag>();
                var buffer = new byte[partSize];
                var partNumber = 1;
                while (true)
                {
                    // fill the part buffer with sequential reads only — SubStream claims CanSeek but its Seek throws
                    var filled = 0;
                    while (filled < buffer.Length)
                    {
                        var read = await stream.ReadAsync(buffer.AsMemory(filled));
                        if (read == 0) break;
                        filled += read;
                    }
                    if (filled == 0) break;

                    using var partStream = new MemoryStream(buffer, 0, filled);
                    var part = await s3.UploadPartAsync(new UploadPartRequest
                    {
                        BucketName = bucket,
                        Key = s3Key,
                        UploadId = upload.UploadId,
                        PartNumber = partNumber,
                        PartSize = filled,
                        InputStream = partStream
                    });
                    partETags.Add(new PartETag(partNumber, part.ETag));
                    partNumber++;
                    if (filled < buffer.Length) break;
                }

                await s3.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
                {
                    BucketName = bucket,
                    Key = s3Key,
                    UploadId = upload.UploadId,
                    PartETags = partETags
                });
            }
            catch
            {
                await s3.AbortMultipartUploadAsync(bucket, s3Key, upload.UploadId);
                throw;
            }
        }

        public async Task DeleteAsync(string key)
        {
            await s3.DeleteObjectAsync(bucket, ToS3Key(key));
        }

        public async Task<long> GetSizeAsync(string key)
        {
            if (sizeCache.TryRemove(key, out var cached)) return cached;
            var metadata = await s3.GetObjectMetadataAsync(bucket, ToS3Key(key));
            return metadata.ContentLength;
        }
    }
}
