using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService
{
    public class BlobService : IBlobService
    {
        private readonly BlobEnvironmentVariables _envirovment;
        private readonly string _keyCrypt;
        private readonly string _blobPath;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly BlobServiceClient blobStorageClient;

        public BlobService(IOptions<BlobEnvironmentVariables> environment, BlobServiceClient blobStorageClient, IRepositoryWrapper? repositoryWrapper = null)
        {
            _envirovment = environment.Value;
            _keyCrypt = _envirovment.BlobStoreKey;
            _blobPath = _envirovment.BlobStorePath;
            _repositoryWrapper = repositoryWrapper!;
            this.blobStorageClient = blobStorageClient;
        }

        public async Task<MemoryStream> FindFileInStorageAsMemoryStream(string name)
        {
            string[] splitedName = name.Split('.');

            byte[] decodedBytes = await DecryptFile(splitedName[0], splitedName[1]);

            var image = new MemoryStream(decodedBytes);

            return image;
        }

        public async Task<string> FindFileInStorageAsBase64(string name)
        {
            string[] splitedName = name.Split('.');

            byte[] decodedBytes = await DecryptFile(splitedName[0], splitedName[1]);

            string base64 = Convert.ToBase64String(decodedBytes);

            return base64;
        }

        public async Task<string> SaveFileInStorage(string base64, string name, string mimeType)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            string createdFileName = $"{DateTime.Now}{name}"
                .Replace(" ", "_")
                .Replace(".", "_")
                .Replace(":", "_");

            string hashBlobStorageName = HashFunction(createdFileName);

            await EncryptFile(imageBytes, mimeType, hashBlobStorageName);

            return hashBlobStorageName;
        }

        public async Task SaveFileInStorageBase64(string base64, string name, string extension)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            await EncryptFile(imageBytes, extension, name);
        }

        public async Task DeleteFileInStorage(string name)
        {
            var containerClient = blobStorageClient.GetBlobContainerClient("streetcode");
            var blobClient = containerClient.GetBlobClient($"{_blobPath}{name}");

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UpdateFileInStorage(
            string previousBlobName,
            string base64Format,
            string newBlobName,
            string extension)
        {
            await DeleteFileInStorage(previousBlobName);

            string hashBlobStorageName = await SaveFileInStorage(
            base64Format,
            newBlobName,
            extension);

            return hashBlobStorageName;
        }

        public async Task CleanBlobStorage()
        {
            var base64Files = await GetAllBlobNames();

            var existingImagesInDatabase = await _repositoryWrapper.ImageRepository.GetAllAsync();
            var existingAudiosInDatabase = await _repositoryWrapper.AudioRepository.GetAllAsync();

            List<string> existingMedia = new();
            existingMedia.AddRange(existingImagesInDatabase.Select(img => img.BlobName)!);
            existingMedia.AddRange(existingAudiosInDatabase.Select(img => img.BlobName)!);

            var filesToRemove = base64Files.Except(existingMedia).ToList();

            foreach (var file in filesToRemove)
            {
                Console.WriteLine($"Deleting {file}...");
                await DeleteFileInStorage(file);
            }
        }

        private async Task<IEnumerable<string>> GetAllBlobNames()
        {
            var containerClient = blobStorageClient.GetBlobContainerClient("streetcode");

            var blobNames = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobNames.Add(blobItem.Name);
            }

            return blobNames;
        }

        private string HashFunction(string createdFileName)
        {
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(createdFileName));
                return Convert.ToBase64String(result).Replace('/', '_');
            }
        }

        private async Task EncryptFile(byte[] imageBytes, string type, string name)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);

            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = keyBytes;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                encryptedBytes = encryptor.TransformFinalBlock(imageBytes, 0, imageBytes.Length);
            }

            byte[] encryptedData = new byte[encryptedBytes.Length + iv.Length];
            Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);

            var fileName = $"{_blobPath}{name}.{type}";

            var containerClient = blobStorageClient.GetBlobContainerClient("streetcode");
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = new MemoryStream(encryptedData))
            {
                await blobClient.UploadAsync(stream);
            }
        }

        private async Task<byte[]> DecryptFile(string fileName, string type)
        {
            var containerClient = blobStorageClient.GetBlobContainerClient("streetcode");
            var blobClient = containerClient.GetBlobClient($"{_blobPath}{fileName}.{type}");

            var downloadResult = await blobClient.DownloadContentAsync();
            byte[] encryptedData = downloadResult.Value.Content.ToArray();

            byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);

            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);

            byte[] decryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = keyBytes;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                decryptedBytes = decryptor.TransformFinalBlock(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            }

            return decryptedBytes;
        }
    }
}