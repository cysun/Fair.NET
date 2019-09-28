using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fair.Models
{
    public class ApplicationTemplate
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public List<ApplicationTemplateDegree> Degrees { get; set; }

        public List<ApplicationTemplateDocument> Documents { get; set; }

        public int NumberOfReferences { get; set; } = 3;

        public bool IsObsolete { get; set; } = false;
    }

    [Table("ApplicationTemplateDegrees")]
    public class ApplicationTemplateDegree
    {
        public int ApplicationTemplateId { get; set; }
        public ApplicationTemplate ApplicationTemplate { get; set; }

        public int Index { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }

    [Table("ApplicationTemplateDocuments")]
    public class ApplicationTemplateDocument
    {
        public int ApplicationTemplateId { get; set; }
        public ApplicationTemplate applicationTemplate { get; set; }

        public int Index { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
