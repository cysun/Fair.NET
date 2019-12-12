using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Fair.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int SearchId { get; set; }
        public Search Search { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string CurrentPosition { get; set; }
        [MaxLength(255)]
        public string CurrentInstitution { get; set; }

        public List<ApplicationDegree> Degrees { get; set; }

        public List<ApplicationDocument> Documents { get; set; }

        public List<ApplicationReference> References { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateSubmitted { get; set; }

        [NotMapped]
        public bool IsSubmitted => DateSubmitted != null;

        public bool IsWithdrawn { get; set; } = false;

        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

        public bool? HaveMinimumQualifications { get; set; }
        public bool? HavePreferredQualifications { get; set; }

        public bool? IsAdvancedToPhoneInterview { get; set; }
        public bool? IsAdvancedToCampusInterview { get; set; }

        public string Notes { get; set; }

        public DateTime? DateEvaluated { get; set; }

        public Application CopyFrom(Application another)
        {
            FirstName = another.FirstName;
            LastName = another.LastName;
            Email = another.Email;
            Phone = another.Phone;
            CurrentPosition = another.CurrentPosition;
            CurrentInstitution = another.CurrentInstitution;
            if (Degrees?.Count == another.Degrees.Count)
            {
                for (int i = 0; i < Degrees.Count; ++i)
                    Degrees[i].CopyFrom(another.Degrees[i]);
            }

            return this;
        }

        public Application AddFieldsFromTemplate(ApplicationTemplate template)
        {
            if (Degrees?.Any() != true)
            {
                Degrees = template.Degrees.Select(d => new ApplicationDegree
                {
                    Application = this,
                    Index = d.Index,
                    Degree = d.Name
                }).ToList();
            }

            if (Documents?.Any() != true)
            {
                Documents = template.Documents.Select(d => new ApplicationDocument
                {
                    Application = this,
                    Index = d.Index,
                    Name = d.Name,
                    Description = d.Description
                }).ToList();
            }

            if (References?.Any() != true)
            {
                References = new List<ApplicationReference>();
                for (int i = 0; i < template.NumberOfReferences; ++i)
                    References.Add(new ApplicationReference
                    {
                        Application = this,
                        Index = i
                    });
            }

            return this;
        }
    }

    [Table("ApplicationDegrees")]
    public class ApplicationDegree
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int Index { get; set; }

        [Required]
        [MaxLength(255)]
        public string Degree { get; set; }

        [Required]
        [MaxLength(255)]
        public string Major { get; set; }

        [Required]
        [MaxLength(255)]
        public string Institution { get; set; }

        [Required]
        public int? Year { get; set; }

        public bool IsExpected { get; set; } = false;

        public ApplicationDegree CopyFrom(ApplicationDegree another)
        {
            Degree = another.Degree;
            Major = another.Major;
            Institution = another.Institution;
            Year = another.Year;
            IsExpected = another.IsExpected;

            return this;
        }
    }

    [Table("ApplicationDocuments")]
    public class ApplicationDocument
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int Index { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }
    }

    [Table("ApplicationReferences")]
    public class ApplicationReference
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int Index { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string Institution { get; set; }

        public int? ReportId { get; set; }
        public File Report { get; set; }
    }
}
