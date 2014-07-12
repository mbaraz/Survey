using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;

namespace SurveyWeb.Controllers
{
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly ISurveyProjectRepository _projectRepository;
        //
        // GET: /Interview/

        public InterviewController(IUserService userService, IUnitOfWork unitOfWork, IInterviewRepository interviewRepository, ISurveyProjectRepository projectRepository)
            : base(unitOfWork, userService)
        {
            _interviewRepository = interviewRepository;
            _projectRepository = projectRepository;
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Index(int surveyProjectId)
        {
            return View(_projectRepository.GetById(surveyProjectId));
        }

        //
        // GET: /Interview/Details/5
        [Authorize(Roles = "Staff")]
        public ActionResult Details(int interviewId)
        {
            return View(_interviewRepository.GetById(interviewId));
        }

        //
        // GET: /Interview/Delete/5
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int interviewId)
        {
            return View(_interviewRepository.GetById(interviewId));
        }

        //
        // POST: /Interview/Delete/5
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Delete(int interviewId, FormCollection collection)
        {
            try
            {
                var interviewById = _interviewRepository.GetById(interviewId);
                _interviewRepository.Delete(interviewById);
                UnitOfWork.Save();
                return RedirectToAction("Index", new {interviewById.SurveyProjectId});
            }
            catch
            {
                return View();
            }
        }
    }
}
