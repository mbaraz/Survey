using System.Linq;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;

namespace SurveyWeb.Controllers
{
    public class SurveyController : SurveyControllerBase
    {
        private const string itemName = "spec_";

        private readonly IUniverService _univerService;

        public SurveyController(IUserService userService, ISurveyService surveyService, IUnitOfWork unitOfWork, ISurveyProjectRepository projectRepository, IUniverService univerService)
            : base(unitOfWork, userService, surveyService, projectRepository)
        {
            _univerService = univerService;
        }

        public ActionResult Index(int surveyProjectId)
        {
            var project = ProjectRepository.GetById(surveyProjectId);
            var interview = project.GetInterviewForRespondent(CurrentRespondent.RespondentId);
            if (interview == null) {
                if (IsGet)
                    return View("Index", project);
                // Необходимо только для опроса студентов, чтобы дать возможность выбрать специализации
                if (CurrentRespondent.StudentGroupId != null) {
                    string[] specsList = _univerService.GetSpecsList((int)CurrentRespondent.StudentGroupId).ToArray();
                    if (specsList.Length > 0)
                        return showSpecs(surveyProjectId, specsList, false);
                }
                return ShowQuestion(surveyProjectId, false);
            }
            return RedirectToNextQuestion(interview);
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Test(int surveyProjectId)
        {
            var project = ProjectRepository.GetById(surveyProjectId);

            if (IsGet)
                return View("Index", project);
            // Необходимо только для опроса студентов, чтобы дать возможность выбрать специализации
            if (CurrentRespondent.StudentGroupId != null) {
                string[] specsList = _univerService.GetSpecsList((int) CurrentRespondent.StudentGroupId).ToArray();
                if (specsList.Length > 0)
                    return showSpecs(surveyProjectId, specsList, true);
            }
            return ShowQuestion(surveyProjectId, true);
        }

        [HttpPost]
        public ActionResult ShowQuestion(int surveyProjectId, bool isTest)
        {
            int[] specCodes = GetKeys(itemName).ToArray();
            var interview = isTest ? SurveyService.StartTestInterview(CurrentRespondent, surveyProjectId, specCodes) :
                SurveyService.StartInterview(CurrentRespondent, surveyProjectId, specCodes);
            UnitOfWork.Save();
            return RedirectToNextQuestion(interview);
        }

        private ActionResult showSpecs(int surveyProjectId, string[] specsList, bool isTest)
        {
            ViewBag.SurveyProjectId = surveyProjectId;
            ViewBag.SpecName = itemName;
            ViewBag.IsTest = isTest;
            return View("ChooseSpecs", specsList); ;
        }
    }
}
