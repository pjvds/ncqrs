using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    /// <summary>
    /// A snapshot store. Can store and load snapshots from an <see cref="IEventSource"/>.
    /// </summary>
    /// <remarks>Implemented using Windows Azure Blob Storage</remarks>
    public class BlobSnapshotStore : ISnapshotStore {
        private CloudStorageAccount _account = null;

        private string _blobContainer = "NcqrsSnapshots".ToLowerInvariant();

        public BlobSnapshotStore(CloudStorageAccount account) : this(account, null) {
        }

        public BlobSnapshotStore(CloudStorageAccount account, string blobContainerPrefix) {
            if (blobContainerPrefix != null) {
                _blobContainer = blobContainerPrefix.ToLowerInvariant() + _blobContainer;
            }
            _account = account;
        }
       
        private static IList<string> _createdContainers = new List<string>();
        private CloudBlobClient GetBlobClient() {
            CloudBlobClient client = _account.CreateCloudBlobClient();
            if (!_createdContainers.Contains(_blobContainer)) {
                lock (_createdContainers) {
                    if (!_createdContainers.Contains(_blobContainer)) {
                        CloudBlobContainer container = client.GetContainerReference(_blobContainer);
                        container.CreateIfNotExist();
                        _createdContainers.Add(_blobContainer);
                    }
                }
            }
            return client;
        }
        public void SaveShapshot(Eventing.Sourcing.Snapshotting.Snapshot snapshot) {
            string filename = Utility.GetSnapshotFullFileName(_blobContainer, snapshot);
            CloudBlob snapshotBlob = GetBlobClient().GetBlobReference(filename);
            snapshotBlob.UploadText(Utility.Jsonize(snapshot.Payload, snapshot.Payload.GetType()));
            snapshotBlob.Metadata["Type"] = snapshot.Payload.GetType().AssemblyQualifiedName;
            snapshotBlob.Metadata["Version"] = snapshot.Version.ToString();
            snapshotBlob.SetMetadata();
        }

        public Eventing.Sourcing.Snapshotting.Snapshot GetSnapshot(Guid eventSourceId, long maxVersion) {
            CloudBlobDirectory directory = GetBlobClient().GetBlobDirectoryReference(Utility.GetSnapshotDirectoryName(_blobContainer, eventSourceId));
            IListBlobItem matchingItem = null;
            foreach (IListBlobItem item in directory.ListBlobs().OrderByDescending(i => i.Uri.ToString())) {
                matchingItem = item;
                string fileName = System.IO.Path.GetFileName(item.Uri.AbsolutePath);
                long currentVersion = long.Parse(fileName.Replace(".ncqrssnapshot", ""));
                if (currentVersion > maxVersion) {
                    continue;
                } else {
                    break;
                }
            }
            if (matchingItem == null) {
                return null;
            } else {
                CloudBlob matchingBlob = GetBlobClient().GetBlobReference(matchingItem.Uri.ToString());
                string snapshotText = matchingBlob.DownloadText();
                string versionMetadata = matchingBlob.Metadata["Version"];
                string typeMetadata = matchingBlob.Metadata["Type"];
                return new Snapshot(eventSourceId,
                    long.Parse(versionMetadata),
                    Utility.DeJsonize(snapshotText,
                        typeMetadata));
            }
        }
    }
}
