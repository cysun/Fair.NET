using System.Collections.Generic;
using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class DocumentService
    {
        private readonly AppDbContext db;

        public DocumentService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Document> GetDocuments(int searchId)
        {
            return db.Documents.Where(d => d.SearchId == searchId)
                .Include(d => d.LatestRevision).ThenInclude(r => r.Author)
                .OrderByDescending(d => d.LatestRevision.Timestamp).ToList();
        }

        public Document GetDocument(int id)
        {
            var document = db.Documents.Where(d => d.Id == id)
                .Include(d => d.LatestRevision).ThenInclude(r => r.Author)
                .Include(d => d.Revisions).ThenInclude(r => r.Author)
                .SingleOrDefault();

            if (document != null && document.Revisions.Count > 1)
                document.Revisions = document.Revisions.OrderByDescending(r => r.Number).ToList();

            return document;
        }

        public void AddDocument(Document document)
        {
            db.Documents.Add(document);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
