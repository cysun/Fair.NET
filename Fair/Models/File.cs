using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fair.Models
{
    public class File
    {
        public int FileId { get; set; }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public byte[] Content { get; set; }

        [NotMapped]
        public int Length => Content.Length;

        public System.IO.Stream OpenReadStream()
        {
            return new System.IO.MemoryStream(Content, false);
        }

        public static File FromUploadedFile(IFormFile uploadedFile)
        {
            var file = new Models.File
            {
                Name = System.IO.Path.GetFileName(uploadedFile.FileName),
                ContentType = uploadedFile.ContentType
            };

            using (var memoryStream = new System.IO.MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);
                file.Content = memoryStream.ToArray();
            }

            return file;
        }
    }
}
