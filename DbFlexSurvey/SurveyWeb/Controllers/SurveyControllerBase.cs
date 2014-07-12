using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyWeb.Controllers
{
    public abstract class SurveyControllerBase : ControllerBase
    {
        protected readonly ISurveyService SurveyService;
        protected readonly ISurveyProjectRepository ProjectRepository;

        protected SurveyControllerBase(IUnitOfWork unitOfWork, IUserService userService, ISurveyService surveyService, ISurveyProjectRepository projectRepository) : base(unitOfWork, userService)
        {
            SurveyService = surveyService;
            ProjectRepository = projectRepository;
        }

        protected ActionResult RedirectToNextQuestion(Interview interview)
        {
            var nextQuestionOrder = SurveyService.GetNextQuestionOrder(CurrentRespondent.RespondentId, interview.SurveyProjectId);
            UnitOfWork.Save();
            if (interview.Completed)
            {
                return RedirectToAction("Thanks");
            }
            return RedirectToAction("Index", "Question",
                                    new
                                        {
                                            surveyid = interview.SurveyProjectId,
                                            order = nextQuestionOrder
                                        });
        }

        public ActionResult Thanks()
        {
            return RedirectToAction("Thanks", "Home");
        }
    }
}