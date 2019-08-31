using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fair.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int CommentId { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
