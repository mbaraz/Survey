using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class SurveyProjectController : ControllerBase
    {
        private const int mailsPerStep = 100;
        
        private readonly IRespondentService _respondentService;
        private readonly ISurveyProjectRepository _projectRepository;
        private readonly IExportService _exportService;
        private readonly ISurveyService _surveyService;
        //
        // GET: /SurveyProject/

        public SurveyProjectController(IUserService userService, ISurveyService surveyService, IRespondentService respondentService, IUnitOfWork unitOfWork, ISurveyProjectRepository projectRepository, IExportService exportService)
            : base(unitOfWork, userService)
        {
            _surveyService = surveyService;
            _respondentService = respondentService;
            _projectRepository = projectRepository;
            _exportService = exportService;
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Index()
        {
           ViewBag.ActiveOnly = false;
           return View(_projectRepository.GetAll());
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Active()
        {
            ViewBag.ActiveOnly = true;
            return View("Index", _projectRepository.GetActiveProjects());
        }

        //
        // GET: /SurveyProject/Details/5
        [Authorize(Roles = "Staff")]
        public ActionResult Details(int surveyProjectId, int? invitedUsers, int? newUsers)
        {
            if (invitedUsers != null)
                ViewBag.InvitedUsers = invitedUsers;
            if (newUsers != null)
                ViewBag.NewUsers = newUsers;

            var surveyProject = _projectRepository.GetById(surveyProjectId);
            var model = new ProjectDetailsModel(surveyProject);
            return View(model);
        }

        [Authorize(Roles = "Staff")]
        public ActionResult PrintQuestions(int surveyProjectId)
        {
            return View(new ProjectDetailsModel(_projectRepository.GetById(surveyProjectId)));
        }

        //
        // GET: /SurveyProject/Create
        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SurveyProject/Create
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Create(SurveyProject project)
        {
            try {
                project.Active = true;
                project.SurveyProjectName = project.SurveyProjectName.Trim();
                _projectRepository.Add(project);
                UnitOfWork.Save();

                return RedirectToAction("Details", new {project.SurveyProjectId});
            } catch {
                return View();
            }
        }

        [Authorize(Roles = "Staff")]
        public ActionResult MultiInvite(int surveyProjectId)
        {
            var surveyProject = _projectRepository.GetById(surveyProjectId);
            return View(new MultiInviteModel {SurveyProject =  surveyProject, InviteSubject = string.Format("Приглашаем принять участие в опросе «{0}»", surveyProject.SurveyProjectName)});
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult MultiInvite(MultiInviteModel model)
        {
            string excludingEmails = string.Empty;
            for (var i = 0; i < model.InstantEmails.Count(); i += mailsPerStep) {
                excludingEmails += _respondentService.MultiInvite(model.SurveyProjectId, model.InstantEmails.Skip(i).Take(mailsPerStep), model.InviteText, WebConfigurationManager.AppSettings["SiteName"], model.InviteSubject);
                UnitOfWork.Save();
            }
            if (excludingEmails.Length > 0) {
                TempData["ProjectId"] = model.SurveyProjectId;
                return RedirectToAction("EmailsError", new { errorEmails = excludingEmails });
            }
            return RedirectToAction("Details", new { model.SurveyProjectId });
        }
        //
        // GET: /SurveyProject/Edit/5
        [Authorize(Roles = "Staff")]
        public ActionResult Edit(int id)
        {
            var surveyProject = _projectRepository.GetById(id);
            return View(new SurveyProjectEditPropertiesModel
                            {
                                ProjectUserDescription = surveyProject.ProjectUserDescription,
                                SurveyProjectId = surveyProject.SurveyProjectId,
                                SurveyProjectName = surveyProject.SurveyProjectName,
                                InviteText = surveyProject.Invitation,
                                RemarkPrefix = surveyProject.RemarkPrefix
                            });
        }

        //
        // POST: /SurveyProject/Edit/5

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Edit(SurveyProjectEditPropertiesModel model)
        {
            try
            {
                var pr = _projectRepository.GetById(model.SurveyProjectId);
                pr.SurveyProjectName = model.SurveyProjectName.Trim();
                pr.ProjectUserDescription = model.ProjectUserDescription;
                pr.InviteText = model.InviteText + "#" + model.RemarkPrefix;
                _projectRepository.Edit(pr);
                UnitOfWork.Save();

                return RedirectToAction("Details", new {pr.SurveyProjectId});
            }
            catch
            {
                return View(model);
            }
        }

        //
        // GET: /SurveyProject/Delete/5
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int id)
        {
            return View(_projectRepository.GetById(id));
        }

        //
        // POST: /SurveyProject/Delete/5
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Delete(int id, SurveyProject project)
        {
            var surveyProject = _projectRepository.GetById(id);
            try
            {
                _projectRepository.Delete(surveyProject);
                UnitOfWork.Save();
                return RedirectToAction("Active");
            }
            catch (Exception exc)
            {
                ViewBag.DeleteError = exc.Message;
            }
            return View(surveyProject);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult InviteEmail(int surveyprojectid, string emails)
        {
            var result = _respondentService.InviteByEmail(surveyprojectid, emails);
            UnitOfWork.Save();
            return RedirectToAction("Details", new { surveyprojectid, result.InvitedUsers, result.NewUsers });
        }

        [Authorize(Roles="Staff")]
        public ActionResult Spss(int surveyProjectId, bool? onlyCompleted)
        {
            var writer = new StringWriter();
            _exportService.ExportProject(ExportMethod.Spss, _projectRepository.GetById(surveyProjectId), writer, onlyCompleted ?? false);
            
            return File(Encoding.UTF8.GetBytes(writer.ToString()), "application/sps", "result.sps");
        }

        //TODO Разобраться, как убрать [Bind]
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult CreateTicket([Bind(Prefix = "CreateTicketModel")]CreateTicketModel model)
        {
            try {
                _respondentService.CreateTicketForProject(model.SurveyProjectId, model.Description, null);
                UnitOfWork.Save();
                return RedirectToAction("Details", new { model.SurveyProjectId, TicketCreated = 1 });
            } catch (Exception) {
                throw;
            }
        }

        [Authorize(Roles="Staff")]
        public ActionResult DeleteAllQuestions(int surveyProjectId)
        {
            var surveyProject = _projectRepository.GetById(surveyProjectId);
            try {
                foreach (var item in surveyProject.OrderedQuestions)
                {
                    _surveyService.DeleteQuestion(item);
                    UnitOfWork.Save();
                }
                UnitOfWork.Save();
                return RedirectToAction("Details", new {surveyProjectId});
            } catch (Exception exc) {
                ViewBag.DeleteError = exc.Message;
            }
            return RedirectToAction("Details", new { surveyProjectId });
        }

        [Authorize(Roles = "Staff")]
        public ActionResult EmailsError(string errorEmails)
        {
            ViewBag.ProjectId = TempData["ProjectId"];
            string[] emailsArray = errorEmails.Split(',');
            return View(emailsArray);
        }

        [Authorize(Roles = "Staff")]
        public ActionResult ShowTickets(int surveyProjectId)
        {
            var project = _projectRepository.GetById(surveyProjectId);
            ViewBag.Title = project.SurveyProjectName;
            ViewBag.SurveyProjectId = surveyProjectId;
            return View(project.Invitations.Where(invitation => invitation.TicketId != null).Select(invitation => invitation.Ticket).Distinct().ToArray());
        }

        [Authorize(Roles = "Staff")]
        public ActionResult FxEditor(int surveyProjectId)
        {
/*
            if (CurrentRespondent.MembershipUserName != "asd" && CurrentRespondent.MembershipUserName != "mbaraz" && CurrentRespondent.MembershipUserName != "ksenia" && CurrentRespondent.MembershipUserName != "tmprus")
                return RedirectToAction("Details", new { surveyProjectId });
*/
            var project = _projectRepository.GetById(surveyProjectId);
            ViewBag.Title = project.SurveyProjectName;
            return View(surveyProjectId);
        }
    }
}
