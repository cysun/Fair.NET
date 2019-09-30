using System;

namespace Fair.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        // It's a committee evaluation where EvaluatorId is null
        public int? EvaluatorId { get; set; }
        public User Evaluator { get; set; }

        public bool? HaveMinimumQualifications { get; set; }
        public bool? HavePreferredQualifications { get; set; }

        public bool? IsAdvancedToPhoneInterview { get; set; }
        public bool? IsAdvancedToCampusInterview { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
