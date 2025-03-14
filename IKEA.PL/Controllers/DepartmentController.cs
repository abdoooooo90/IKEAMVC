﻿using System.Linq.Expressions;
using System.Security.Policy;
using IKEA.BLL.Models;
using IKEA.BLL.Services;
using IKEA.PL.Models.Departments;
using Microsoft.AspNetCore.Mvc;

namespace IKEA.PL.Controllers
{
    public class DepartmentController : Controller
    {
        #region Services
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger, IWebHostEnvironment environment)
        {
            _departmentService = departmentService;
            _logger = logger;
            _environment = environment;
            _environment = environment;
        }
        #endregion
        #region Index
        [HttpGet]
        //BaseUrl/Department/Index
        public IActionResult Index()
        {
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }
        #endregion
        #region Create
        #region Get
        [HttpGet]
         public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken] //علشان اللي لازم يعدل من الابلكيشن نفسه
        public IActionResult Create(CreatedDepartmentDto department)
        {
            if (!ModelState.IsValid)
                return View(department);
            var message = string.Empty;
            try
            {
                var result = _departmentService.CreateDerpartmenet(department);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    message = "Sorry The Department Has Not Been Created";
                    ModelState.AddModelError(string.Empty, message);
                    return View(department);
                }
                 
            }
            
            catch(Exception ex)
            {
                //1- Log Exception
                _logger.LogError(ex, ex.Message);
                //2- Set Frindly Message
                if(_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View(department);
                }
                else
                {
                    message = "Sorry The Department Has Not Been Created";
                    return View("Error", message);
                }

            }
        }
        #endregion
        #endregion
        #region Details
        [HttpGet]
         public IActionResult Details(int? id)
        {
            if(id is null)
            {
                return BadRequest();
            }
            var department = _departmentService.GetDepartmentById(id.Value);
            if(department is null)
            {
                return NotFound();
            }
            return View(department);

        }
        #endregion
        #region  Edit
        #region Get
        [HttpGet]
         public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();//400

            }
            var department = _departmentService.GetDepartmentById(id.Value);
            if(department is null)
            {
                return NotFound();//404
            }
            var viewModel = new DepartmentEditViewModel()
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                Description = department.Description,
                CreationDate = department.CreationDate
            };
            return View(viewModel);

        }
        #endregion
        #region  Post
        [HttpPost]
        [ValidateAntiForgeryToken] //علشان اللي لازم يعدل من الابلكيشن نفسه
        public IActionResult Edit (int id, DepartmentEditViewModel departmentVM)
        {
            if (!ModelState.IsValid)
                return View(departmentVM);
            var message = string.Empty;
            try
            {
                var updatedDepartment = new UpdateDepartmentDto()
                {
                    Id = id,
                    Code = departmentVM.Code,
                    Name = departmentVM.Name,
                    Description = departmentVM.Description,
                    CreationDate = departmentVM.CreationDate


                };
                var updated = _departmentService.UpdateDepartment(updatedDepartment)>0;
                if(updated)
                {
                    return RedirectToAction(nameof(Index));
                }
                message = "Sorry , An Error Occured While Updating The Department";

            }
             catch(Exception ex)
            {
                //1-
                _logger.LogError(ex, ex.Message);
                message = _environment.IsDevelopment() ? ex.Message : "Sorry , An Error Occured While Updating The Department";

            }
            ModelState.AddModelError(string.Empty, message);
            return View(departmentVM);


        }
        #endregion
        #endregion
        #region Delete
        [HttpGet]
         public IActionResult Delete (int? id)
        {
            if(id is null)
            {
                return BadRequest();
            }
            var department = _departmentService.GetDepartmentById(id.Value);
            if(department is null)
            {
                return NotFound();
            }
            return View(department);
        }
        //---------------------
        [HttpPost]
        [ValidateAntiForgeryToken] //علشان اللي لازم يعدل من الابلكيشن نفسه
        public  IActionResult Delete(int id)
        {
            var message = string.Empty;
            try
            {
                var deleted = _departmentService.DeletedDepartment(id);
                if (deleted)
                {
                    return RedirectToAction(nameof(Index));

                }
                message = "An Error Ocurred During Deleting The Department";
            }
             catch(Exception ex)
            {
                //1-
                _logger.LogError(ex, ex.Message);
                //2- 
                message = _environment.IsDevelopment() ? ex.Message : "An Error Ocurred During Deleting The Department";


            }
            return RedirectToAction(nameof(Index));
        }
        #endregion


    } 
}
