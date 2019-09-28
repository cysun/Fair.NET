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

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(255)]
        public string CurrentPosition { get; set; }
        [MaxLength(255)]
        public string CurrentEmployer { get; set; }

        public List<ApplicationDegree> Degrees { get; set; }

        public List<ApplicationDocument> Documents { get; set; }

        public List<ApplicationReference> References { get; set; }

        public bool IsWithdrawn { get; set; } = false;
        public bool IsDisqualified { get; set; } = false;

        public static Application FromTemplate(ApplicationTemplate template)
        {
            var application = new Application();

            application.Degrees = template.Degrees.Select(d => new ApplicationDegree
            {
                Index = d.Index,
                Degree = d.Name
            }).ToList();

            application.Documents = template.Documents.Select(d => new ApplicationDocument
            {
                Index = d.Index,
                Name = d.Name,
                Description = d.Description
            }).ToList();

            application.References = new List<ApplicationReference>();
            for (int i = 0; i < template.NumberOfReferences; ++i)
                application.References.Add(new ApplicationReference());

            return application;
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

        public int Year { get; set; }

        public bool IsExpected { get; set; } = false;
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

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Institution { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Phone { get; set; }

        public int? ReportId { get; set; }
        public File Report { get; set; }
    }
}
