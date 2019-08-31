using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fair.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        public int SearchId { get; set; }
        public Search Search { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public List<Revision> Revisions { get; set; } = new List<Revision>();
    }

    [Table("Revisions")]
    public class Revision
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public int Number { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }

    public class File
    {
        public int FileId { get; set; }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int OwnerId { get; set; }
        public User Owner { get; set; }

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
