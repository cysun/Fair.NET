using System;
using Fair.Models;

namespace Fair.Services
{
    public class FileService
    {
        private readonly AppDbContext db;

        public FileService(AppDbContext db)
        {
            this.db = db;
        }

        public File GetFile(int? id)
        {
            if (id == null) throw new ArgumentNullException();
            return db.Files.Find(id);
        }

        public void AddFile(File file)
        {
            db.Files.Add(file);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
