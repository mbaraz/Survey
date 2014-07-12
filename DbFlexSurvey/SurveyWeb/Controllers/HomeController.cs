using System.Linq;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IUserService userService, IUnitOfWork unitOfWork) : base(unitOfWork, userService)
        {
        }

        [Authorize]
        public ActionResult Index()
        {
            var activeSurveys = CurrentRespondent.ActiveSurveys.ToDictionary(project => project.SurveyProjectId,
                                                                             project => project.SurveyProjectName);
            var surveyInvitatons = CurrentRespondent.ActiveInvitations.ToDictionary(invitation => invitation.SurveyProjectId, invitation => invitation.SurveyProject.SurveyProjectName);
            if (surveyInvitatons.Count == 1)
                return RedirectToAction("Index", "Survey", new {surveyProjectId = surveyInvitatons.Single().Key});

            return View("Index", new HomeModel
                                     {
                                         SurveyInvitatons =
                                             surveyInvitatons,
                                         ActiveSurveys =
                                             activeSurveys
                                     });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Error()
        {
            return ShowMessage("Во время опроса произошла какая-то ошибка.");
        }

        private ActionResult ShowMessage(string message)
        {
            ViewBag.Message = message;
            return Index();
        }

        public ActionResult Thanks()
        {
            return ShowMessage("Спасибо за прохождение опроса");
        }
    }
}
