using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class RespondentController : ControllerBase
    {
        private readonly ISurveyProjectRepository _projectRepository;
        private readonly IRespondentRepository _respondentRepository;
        private readonly IRespondentService _respondentService;
        //
        // GET: /Respondent/

        public RespondentController(IUserService userService, IUnitOfWork unitOfWork, ISurveyProjectRepository projectRepository, IRespondentRepository respondentRepository, IRespondentService respondentService) : base(unitOfWork, userService)
        {
            _projectRepository = projectRepository;
            _respondentRepository = respondentRepository;
            _respondentService = respondentService;
        }

        [Authorize(Roles="Staff")]
        public ActionResult Index()
        {
            var model = new RespondentListViewModel
                            {
                                Projects =  _projectRepository.GetActiveProjects(),
                                Respondents = _respondentRepository.GetAll()
                            };
            return View(model);
        }

        //
        // GET: /Respondent/Details/5

        public ActionResult Details(int respondentId)
        {
            
            var model = RespondentViewModel.Create(_respondentRepository.GetById(respondentId));
            model.IsCurrentUserAdmin = IsCurrentUserAdmin;
            model.RespondentRoles = model.MembershipUserName == null ? new string[]{} : Roles.GetRolesForUser(model.MembershipUserName);
            model.PossibleRoles = _respondentService.GetPossibleRoles().Except(model.RespondentRoles).ToArray();
            return View(model);
        }

        //
        // GET: /Respondent/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Respondent/Create

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
        // GET: /Respondent/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Respondent/Edit/5

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
        // GET: /Respondent/Delete/5

        public ActionResult Delete(int respondentId)
        {
            return View(_respondentRepository.GetById(respondentId));
        }

        //
        // POST: /Respondent/Delete/5

        [HttpPost]
        public ActionResult Delete(int respondentId, FormCollection collection)
        {
            var respondent = _respondentRepository.GetById(respondentId);
            try
            {
                _respondentRepository.Delete(respondent);
                UnitOfWork.Save();
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View(respondent);
            }
        }

        [Authorize (Roles ="Staff")]
        [HttpPost]
        public ActionResult Invite(IEnumerable<int> respondentId, int surveyProjectId)
        {
            _respondentService.InviteRespondentsToProject(surveyProjectId, respondentId);
            UnitOfWork.Save();
            return View(_projectRepository.GetById(surveyProjectId));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Grant(int respondentId, string role)
        {
            var user = _respondentRepository.GetById(respondentId).MembershipUserName;
            Roles.AddUserToRole(user, role);
            return RedirectToAction("Details", new {respondentId});
        }

        public ActionResult SentInvite(int respondentId)
        {
            _respondentService.SendInviteEmail(_respondentRepository.GetById(respondentId), WebConfigurationManager.AppSettings["SiteName"]);
            UnitOfWork.Save();
            return RedirectToAction("Details", new { respondentId });
        }
    }
}
