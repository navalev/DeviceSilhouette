﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceRichState;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace PersistencyProviders.BlobStorage
{
    /// <summary>
    /// Store device messages in Azure Blob store. 
    /// Main container = silhouette-messages
    /// Meesages will be partioned based on message timestamp
    /// year=2016
    ///     month=07
    ///         day=18                
    /// </summary>
    public class BlobStorageProvider : IHistoryStorage
    {
        private string _storageConnectionString;
        private const string _storageContainer = "silhouette-messages";

        private CloudBlobClient _blobClient;

        public BlobStorageProvider(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
            createMainContainer();
        }

        private void createMainContainer()
        {           
            // Retrieve a reference to a container.
            CloudBlobContainer container = _blobClient.GetContainerReference(_storageContainer);
            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        private MemoryStream SerializeToStream(DeviceMessage stateMessage)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceMessage));
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(XmlWriter.Create(stream), stateMessage);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Stores a single state message to blob storage
        /// </summary>
        /// <param name="stateMessage">the device state message to store</param>
        /// <returns></returns>
        public async Task StoreStateMessageAsync(DeviceMessage stateMessage)
        {
            await internalStoreMessageAsync(stateMessage);
        }
        
        /// <summary>
        /// Stores a collection of state messages to blob storage
        /// </summary>
        /// <param name="stateMessages">device state messages to store</param>
        /// <returns></returns>
        public async Task StoreStateMessagesAsync(List<DeviceMessage> stateMessages)
        {
            var processingTasks = stateMessages.Select(internalStoreMessageAsync);
            await Task.WhenAll(processingTasks);
        }

        private async Task internalStoreMessageAsync(DeviceMessage stateMessage)
        {
            // create a unique name for the blob based on device id and message stamptime
            String folderPath = String.Concat(stateMessage.Timestamp.Year, "/", stateMessage.Timestamp.Month, "/", stateMessage.Timestamp.Day, "/", stateMessage.MessageType.ToString(), "_", "/");
            string blobName = String.Concat(folderPath, stateMessage.DeviceId, "_", Guid.NewGuid().ToString());
            
            CloudBlobContainer container = _blobClient.GetContainerReference(_storageContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            MemoryStream stream = SerializeToStream(stateMessage);
            await blockBlob.UploadFromStreamAsync(stream);
        }

    }
}
