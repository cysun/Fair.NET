﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fair.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? ChairId { get; set; }
        public User Chair { get; set; }
    }

    public class Search
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required]
        [MaxLength(255)]
        public string Position { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? CloseDate { get; set; }

        [NotMapped]
        public string Name => $"{Department?.Name} {Position}, {StartDate.Year}-{StartDate.Year + 1}";

        public int DepartmentChairId { get; set; }
        public User DepartmentChair { get; set; }

        public int CommitteeChairId { get; set; }
        public User CommitteeChair { get; set; }

        public List<CommitteeMember> CommitteeMembers { get; set; } = new List<CommitteeMember>();

        public int ApplicationTemplateId { get; set; }
        public ApplicationTemplate ApplicationTemplate { get; set; }

        public List<Document> Documents { get; set; }

        public List<Application> Applications { get; set; }
    }

    [Table("CommitteeMembers")]
    public class CommitteeMember
    {
        public CommitteeMember() { }

        public CommitteeMember(int searchId, int userId)
        {
            SearchId = searchId;
            UserId = userId;
        }

        public int SearchId { get; set; }
        public Search Search { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
