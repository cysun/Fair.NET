using System.Collections.Generic;
using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class ApplicationTemplateService
    {
        private readonly AppDbContext db;

        public ApplicationTemplateService(AppDbContext db)
        {
            this.db = db;
        }

        public List<ApplicationTemplate> GetApplicationTemplates()
        {
            return db.ApplicationTemplates.OrderBy(t => t.Name).ToList();
        }

        public ApplicationTemplate GetSearchApplicationTemplate(int searchId)
        {
            return db.ApplicationTemplates.Join(db.Searches, Template => Template.Id, Search => Search.ApplicationTemplateId,
                (Template, Search) => new { Template, Search })
                .Where(JoinResult => JoinResult.Search.Id == searchId)
                .Select(JoinResult => JoinResult.Template)
                .Include(t => t.Degrees)
                .Include(t => t.Documents)
                .SingleOrDefault();
        }
    }
}
