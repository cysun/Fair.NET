using System;
using System.Collections.Generic;
using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class SearchService
    {
        private readonly AppDbContext db;

        public SearchService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Search> GetSearches()
        {
            return db.Searches.OrderByDescending(s => s.StartDate).ThenBy(s => s.Name).ToList();
        }

        public List<Search> GetSearches(User user)
        {
            if (user.IsAdmin || user.IsSysAdmin)
                return GetSearches();

            return db.Searches.Include(s => s.CommitteeMembers)
                .Where(s =>
                   s.DepartmentChairId == user.UserId ||
                   s.CommitteeChairId == user.UserId ||
                   s.CommitteeMembers.Select(m => m.UserId).Contains(user.UserId))
                .OrderByDescending(s => s.StartDate).ThenBy(s => s.Name)
                .ToList();
        }

        public Search GetSearch(int searchId)
        {
            return db.Searches.Where(s => s.SearchId == searchId)
                .Include(s => s.DepartmentChair)
                .Include(s => s.CommitteeChair)
                .Include(s => s.CommitteeMembers).ThenInclude(m => m.User)
                .Include(s => s.Documents)
                .SingleOrDefault();
        }

        public void AddSearch(Search search)
        {
            db.Searches.Add(search);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
