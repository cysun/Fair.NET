using System.Collections.Generic;
using System.Security.Claims;
using Fair.Models;
using Fair.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly DepartmentService departmentService;
        private readonly ILogger<DepartmentsController> logger;

        public DepartmentsController(DepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            this.departmentService = departmentService;
            this.logger = logger;
        }

        public IActionResult List()
        {
            return View(departmentService.GetDepartments());
        }

        [HttpGet]
        [Authorize("IsSysAdmin")]
        public IActionResult Add()
        {
            return View(new Department());
        }

        [HttpPost]
        [Authorize("IsSysAdmin")]
        public IActionResult Add(Department department)
        {
            departmentService.AddDepartment(department);
            departmentService.SaveChanges();

            logger.LogInformation("{username} created department {departmentName}", User.FindFirst(ClaimTypes.Name).Value, department.Name);

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        [Authorize("IsSysAdmin")]
        public IActionResult Edit(int id)
        {
            return View(departmentService.GetDepartment(id));
        }

        [HttpPost]
        [Authorize("IsSysAdmin")]
        public IActionResult Edit(int id, Department update)
        {
            var department = departmentService.GetDepartment(id);
            department.Name = update.Name;
            department.ChairId = update.ChairId;
            departmentService.SaveChanges();

            logger.LogInformation("{username} updated department {departmentId}", User.FindFirst(ClaimTypes.Name).Value, id);

            return RedirectToAction(nameof(List));
        }

        [HttpGet("/api/departments/search")]
        public List<Department> Search([FromQuery(Name = "q")]string prefix)
        {
            return departmentService.SearchByPrefix(prefix);
        }
    }
}
