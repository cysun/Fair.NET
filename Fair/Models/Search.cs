using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fair.Models
{
    public class Search
    {
        public int SearchId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int DepartmentChairId { get; set; }
        public User DepartmentChair { get; set; }

        public int CommitteeChairId { get; set; }
        public User CommitteeChair { get; set; }

        public List<CommitteeMember> CommitteeMembers { get; set; }

        public List<Document> Documents { get; set; }
    }

    [Table("CommitteeMembers")]
    public class CommitteeMember
    {
        public int SearchId { get; set; }
        public Search Search { get; set; }

        public int MemberId { get; set; }
        public User Member { get; set; }
    }
}
