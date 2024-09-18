using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using System;
using System.IO;
using System.Linq;

namespace BookStoreApp.Services
{
    public class FileUploaderService
    {
        private readonly string _filePath;
        private readonly EncryptionService _encryptionService;

        public FileUploaderService(IWebHostEnvironment env, EncryptionService encyption)
        {
            _filePath = Path.Combine(env.WebRootPath, "Uploads");
            _encryptionService = encyption;
        }


        public void SaveFile(IFormFile file)
        {
            //Create variable to hold our file name
            string fileName = file.FileName;
            //Create variable to hold our file contents once converted to byte array.
            byte[] fileContents;
            //CReate a memory stream to convert the file contents to a byte array.
            using (MemoryStream memStream = new MemoryStream())
            {
                //Pass the file into the memory stream
                file.CopyTo(memStream);
                //Get the file contents back out of the stream as a byte array.
                fileContents = memStream.ToArray();
            }

            //Pass the file contents to the encryption service to be encrypted and get back the encrypted data.
            byte[] encryptedData = _encryptionService.EncryptData(fileContents);

            //Create a memory array to mamnage the encrypted data for file writing.
            using (MemoryStream memStream = new MemoryStream(encryptedData))
            {
                //Combine the filepath and file name to get the complete file path
                string fullFilePath = Path.Combine(_filePath, fileName);
                //Craete a file stream to store the encrypted file in our directory back in the files format
                using (FileStream fileStream = new FileStream(fullFilePath,FileMode.Create))
                {
                    //Pass the byte array of file data through from the moemory stream and through the file
                    //stream to the specified location. 
                    memStream.WriteTo(fileStream);
                }
            }
        }

        private FileInfo LoadFile(string filename)
        {
            //Gets the directory details up to the uploads folder.
            DirectoryInfo directoryInfo= new DirectoryInfo(_filePath);
            //Cycle through the directory to see if the filename exists
            if (directoryInfo.EnumerateFiles()
                        .Any(f => f.Name.Equals(filename,StringComparison.OrdinalIgnoreCase)))
            {
                //Retrieve the file and retrun it to where the method was called from.
                return directoryInfo.EnumerateFiles()
                        .Where(f => f.Name.Equals(filename, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
            }
            //Return null if it doesn't exist.
            return null;
        }

        private byte[] ReadFileIntoMemory(string filename)
        {
            //Get the file details from the storage location
            var file = LoadFile(filename);
            //If the file retrieval returned nothing, this method will reteun nothing.
            if (file == null)
            {
                return null;
            }
            //Open a memory stream to manage the data of the file
            using (var memStream = new MemoryStream())
            {
                //Create a file stream to open the file.
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    //Copy the file into the memory stream, this will break it down into bytes
                    fileStream.CopyTo(memStream);
                    //Return the bytes from the memory stream as an array.
                    return memStream.ToArray();
                }
            }
        }

        public byte[] DownloadFile(string fileName)
        {
            //Get the selected file from the upload folder
            var encryptedFile = ReadFileIntoMemory(fileName);
            //If the file didn't exist or it was empty.
            if (encryptedFile == null || encryptedFile.Length == 0) 
            {
                return null;
            }
            //Pass the encrypted file to the encryption service to be decrypted.
            var decryptedData = _encryptionService.DecryptData(encryptedFile);
            //Return the file after it is decrypted.
            return decryptedData;
        }

    }
}
