using GymManagementSystem.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Attachment
{
    public interface IAttachmentService
    {
        Task<Result<string?>> UploadAsync(Stream stream, string fileName, string folderName, CancellationToken cancellationToken);

        bool DeleteAttachment(string fileName, string folderName);

        (Stream stream, string contentType)? GetAttachment(string fileName, string folderName);
    }
}
