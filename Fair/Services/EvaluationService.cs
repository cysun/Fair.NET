using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class EvaluationService
    {
        private readonly AppDbContext db;

        public EvaluationService(AppDbContext db)
        {
            this.db = db;
        }

        public Evaluation GetEvaluation(int id)
        {
            return db.Evaluations.Where(e => e.Id == id)
                .Include(e => e.Application).ThenInclude(a => a.Search)
                .SingleOrDefault();
        }

        public Evaluation GetEvaluation(int applicatinId, int evaluatorId)
        {
            return db.Evaluations.Where(e => e.ApplicationId == applicatinId && e.EvaluatorId == evaluatorId)
                .SingleOrDefault();
        }

        public void AddEvaluation(Evaluation evaluation)
        {
            db.Evaluations.Add(evaluation);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
