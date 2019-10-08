using System.Collections.Generic;
using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class ApplicationService
    {
        private readonly AppDbContext db;

        public ApplicationService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Application> GetSearchApplications(int searchId)
        {
            return db.Applications.Where(a => a.SearchId == searchId).ToList();
        }

        public Application GetApplication(int id)
        {
            var application = db.Applications.Where(d => d.Id == id)
                .Include(a => a.Degrees).Include(a => a.Documents).Include(a => a.References)
                .SingleOrDefault();

            if (application != null)
            {
                if (application.Degrees.Count > 1)
                    application.Degrees = application.Degrees.OrderBy(d => d.Index).ToList();
                if (application.Documents.Count > 1)
                    application.Documents = application.Documents.OrderBy(d => d.Index).ToList();
                if (application.References.Count > 1)
                    application.References = application.References.OrderBy(d => d.Index).ToList();
            }

            return application;
        }

        public void AddApplication(Application application)
        {
            db.Applications.Add(application);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
