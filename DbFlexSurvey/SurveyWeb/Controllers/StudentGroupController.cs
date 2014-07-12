using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel.Univer;

namespace SurveyWeb.Controllers
{
    public class StudentGroupController : ControllerBase
    {
        private readonly IStudentGroupRepository _studentGroupRepository;
        //
        // GET: /StudentGroup/

        public StudentGroupController(IUnitOfWork unitOfWork, IUserService userService, IStudentGroupRepository studentGroupRepository) : base(unitOfWork, userService)
        {
            _studentGroupRepository = studentGroupRepository;
        }

        public ActionResult Index()
        {
            return View(_studentGroupRepository.GetStudentGroups());
        }

        //
        // GET: /StudentGroup/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /StudentGroup/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /StudentGroup/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /StudentGroup/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /StudentGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /StudentGroup/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /StudentGroup/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
