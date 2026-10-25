using GymManagementSystem.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly string[] _allowedExtensions = [".jpg", ".png", ".jpeg"];
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment environment, ILogger<AttachmentService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<Result<string?>> UploadAsync(Stream stream, string fileName, string folderName, CancellationToken cancellationToken)
        {
            if (stream == null || stream.Length == 0) return Result<string?>.Validation("Cannot Read Empty File");
            if (!stream.CanRead) return Result<string?>.Validation("Cannot Read the Attached File");
            if (stream.Length > _maxFileSize) return Result<string?>.Validation($"File exceeded the max size allowed, your file size is {stream.Length}");

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension.ToLower()))
                return Result<string?>.Validation($"File Rejected :extension {extension} Not Allowed");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);

            var storedfileName = $"{Guid.NewGuid()}{fileName}";
            var fullPath = Path.Combine(uploadsFolder, storedfileName);

            try
            {
                using var fileStream = new FileStream(fullPath,FileMode.Create,FileAccess.Write);
                await stream.CopyToAsync(fileStream,cancellationToken);
                return Result<string?>.Ok(storedfileName);
            }
            catch (Exception ex)
            {
                return Result<string?>.Fail(ex.Message);
            }

        }

        public bool DeleteAttachment(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_environment.ContentRootPath,folderName,fileName);
            try
            {
                if (!File.Exists(fullPath)) return false;

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To Delete File {fileName}");
                return false;
            }
          
        }

        public (Stream stream, string contentType)? GetAttachment(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName)) return null;
            var fullPath = Path.Combine(_environment.ContentRootPath, folderName, fileName);

            try
            {
                if (!File.Exists(fullPath)) return null;

                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                var contentType = Path.GetExtension(fileName).ToLower() switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return (stream, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To Get File {fileName}");
                return null;
            }
        }
    }
}
