using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class QuestionController : SurveyControllerBase
    {
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;

        public QuestionController(IUserService userService, ISurveyService surveyService, IUnitOfWork unitOfWork, ISurveyProjectRepository projectRepository, ISurveyQuestionRepository surveyQuestionRepository)
             : base(unitOfWork, userService, surveyService, projectRepository)
        {
            _surveyQuestionRepository = surveyQuestionRepository;
        }
        //
        // GET: /Question/
        public ActionResult Index(int surveyId, int order, bool isFlex = true)
        {
            var project = ProjectRepository.GetById(surveyId);
            var interview = project.GetInterviewForRespondent(CurrentRespondent.RespondentId);
            if (interview == null)
                return RedirectToAction("Error", "Home"); // Мы это интервью даже не начинали

            if (IsGet && isFlex && !isOldFashioned(surveyId)) {
                ViewBag.QuestionOrder = order;
                return View("Findex", project);
            }
            var question = GetSurveyQuestion(order, interview, project);
            if (question == null)
                return RedirectToAction("Error", "Home"); // Какой-то косяк с вопросом
            
            var model = new QuestionModel {
                                Question = question,
                                AnswerVariants = interview.GetFilteredAnswers(question).ToArray()
                            };
            if (IsGet)
                return RenderQuestion(model);

            var answers = GetKeys("Answer_").ToList();
            var openAnswers = GetDictionary("Open_");
            var ranks = GetDictionary("Rank_").ToDictionary(pair => pair.Key, pair => int.Parse(pair.Value));
            IInterviewAnswer interviewAnswer = new InterviewAnswer(answers, openAnswers, ranks);
            try {
                var isAnswerAccepted = SurveyService.AcceptAnswer(CurrentRespondent.RespondentId, question.SurveyQuestionId, interviewAnswer);
                UnitOfWork.Save();
                return isAnswerAccepted ? RedirectToNextQuestion(interview) : RenderQuestion(model);
            } catch (QuestionProblemException exception) {
                ModelState.AddModelError(exception.AnswerCode.ToString(CultureInfo.InvariantCulture), exception.Message);
                return RenderQuestion(model);
            }
        }

        private SurveyQuestion GetSurveyQuestion(int order, Interview interview, SurveyProject project)
        {
            var question = _surveyQuestionRepository.GetQuestion(project.SurveyProjectId, order);
            if (question == null)
                return null; // Нет такого вопроса

            if (interview.ShouldSkip(question))
                return null; //А этот вопрос мы должны пропустить, непонятно как мы сюда попали

            return question;
        }

        private ActionResult RenderMultipleQuestion(QuestionModel question)
        {
            return View("RenderMultipleQuestion", question);
        }

        private ActionResult RenderSingleQuestion(QuestionModel question)
        {
            return View("RenderSingleQuestion", question);
        }

        private ActionResult RenderQuestion(QuestionModel model)
        {
            return model.Question.MultipleAnswerAllowed ? RenderMultipleQuestion(model) : RenderSingleQuestion(model);
        }

        private bool isOldFashioned(int surveyId)
        {
            return surveyId >= 48 && surveyId <= 51;
        }


        private class InterviewAnswer : IInterviewAnswer
        {
            private readonly List<int> _answers;
            private readonly Dictionary<int, string> _openAnswers;
            private readonly Dictionary<int, int> _ranks;

            public ICollection<int> Answers { get { return _answers; } }
            public IDictionary<int, string> OpenAnswers { get { return _openAnswers; } }
            public Dictionary<int, int> Rank { get { return _ranks; } }

            public InterviewAnswer(List<int> answers, IEnumerable<KeyValuePair<int, string>> openAnswers, Dictionary<int, int> ranks)
            {
                _answers = answers;
                _openAnswers = openAnswers.ToDictionary();
                _ranks = ranks;
                AddMissingAnswers(_openAnswers);
                AddMissingAnswers(_ranks);
            }

            private void AddMissingAnswers<T>(Dictionary<int, T> source)
            {
                _answers.AddRange(source.Keys.Where(k => !_answers.Contains(k)));
            }
        }
    }
}
