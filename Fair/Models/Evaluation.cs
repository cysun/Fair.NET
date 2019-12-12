using System.ComponentModel.DataAnnotations.Schema;

namespace Fair.Models
{
    public enum Rating
    {
        Outstanding = 4,
        Good = 3,
        Average = 2,
        Poor = 1
    }

    [Table("Evaluations")]
    public class Evaluation
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int EvaluatorId { get; set; }
        public User Evaluator { get; set; }

        public Rating? Rating { get; set; }
        public string Notes { get; set; }
    }
}
