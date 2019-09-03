using System.Collections.Generic;
using System.Linq;
using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class DepartmentService
    {
        private readonly AppDbContext db;

        public DepartmentService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Department> GetDepartments()
        {
            return db.Departments.Include(d => d.Chair).OrderBy(d => d.Name).ToList();
        }

        public Department GetDepartment(int departmentId)
        {
            return db.Departments.Where(d => d.DepartmentId == departmentId).Include(d => d.Chair).SingleOrDefault();
        }

        public void AddDepartment(Department department)
        {
            db.Departments.Add(department);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
