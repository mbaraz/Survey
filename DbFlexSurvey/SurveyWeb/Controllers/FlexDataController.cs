using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Common;
using SurveyWeb.Extensions;
using SurveyWeb.Models;
using SurveyWeb.Models.Wrappers;

namespace SurveyWeb.Controllers
{
    public class FlexDataController : SurveyControllerBase
    {
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly ISubQuestionRepository _subQuestionRepository;
        private readonly IAnswerVariantRepository _answerVariantRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IInterviewRepository _interviewRepository;

        public FlexDataController(IUserService userService, ISurveyService surveyService, IUnitOfWork unitOfWork, ISurveyProjectRepository projectRepository, ISurveyQuestionRepository surveyQuestionRepository, ISubQuestionRepository subQuestionRepository, IAnswerVariantRepository answerVariantRepository, ITagRepository tagRepository, IInterviewRepository interviewRepository)
            : base(unitOfWork, userService, surveyService, projectRepository)
        {
            _surveyQuestionRepository = surveyQuestionRepository;
            _subQuestionRepository = subQuestionRepository;
            _answerVariantRepository = answerVariantRepository;
            _tagRepository = tagRepository;
            _interviewRepository = interviewRepository;
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ContentResult GetAllQuestions(int surveyProjectId)
        {
            if (surveyProjectId == 0)
                return Content("No project");

            if (_interviewRepository.hasNotTestInterview(ProjectRepository.GetById(surveyProjectId)))
                return Content("No edit");

            var orderedQuestions = ProjectRepository.GetById(surveyProjectId).OrderedQuestions;
            var modelsArray = new QuestionModelRestricted[orderedQuestions.Count()];
            var modelIndex = 0;
            foreach (var question in orderedQuestions) {
                var model = new QuestionModelRestricted {
                                                            Question = new SurveyQuestionRestricted(question),
                                                            SubQuestions = question.OrderedSubQuestions.Select(sq => new SubQuestionRestricted(sq)).ToArray(),
                                                            AnswerVariants = question.OrderedAnswerVariants.Select(av => new AnswerVariantRestricted(av)).ToArray()
                                                        };
                modelsArray[modelIndex++] = model;
            }
            return Content(modelsArray.ToJSON(null));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateInput(false)]
        public ContentResult ReceiveAllQuestions(int surveyId, string models, string deletedQuestionIds, string deletedSubQuestionIds, string deletedAnswerIds, string deletedTagIds)
        {
            var project = ProjectRepository.GetById(surveyId);
            if (_interviewRepository.hasNotTestInterview(project))
                return Content("No edit");

            _interviewRepository.DeleteAll(project);
            fillDeleted(deletedQuestionIds, deletedSubQuestionIds, deletedAnswerIds, deletedTagIds);
            var questions = saveModels(JSONObject.CreateFromString(models).Array);
            removeDeleted();
            return Content(trySaveAll(questions));
        }

        [HttpPost]
        public ContentResult ReceiveAnswers(int surveyId, string answers, int startOrder = 0)
        {
            if (answers == "null")
                return GetQuestion(surveyId, startOrder);

            var jAnswers = JSONObject.CreateFromString(answers);
            int questionId = jAnswers.Dictionary["QuestionId"].Integer;
            var answersList = makeAnswersList(jAnswers.Dictionary["Answers"].Array);
            var openAnswers = makeOpenDictionary(jAnswers.Dictionary["OpenAnswers"].Array);
            var ranks = makeRankDictionary(jAnswers.Dictionary["Rank"].Array);
            IInterviewAnswer interviewAnswer = new InterviewAnswer(answersList, openAnswers, ranks);
            try {
                var isAnswerAccepted = SurveyService.AcceptAnswer(CurrentRespondent.RespondentId, questionId, interviewAnswer);
                if (!isAnswerAccepted)
                    return Content("Answer is not accepted");
            } catch (QuestionProblemException exception) {
                ModelState.AddModelError(exception.AnswerCode.ToString(CultureInfo.InvariantCulture), exception.Message);
                return Content("System error: " + exception.Message);
            }
            return GetQuestion(surveyId, startOrder);
        }

        [Authorize(Roles = "Staff")]
        public ActionResult TestFromQuestion(int surveyProjectId, int questionOrder)
        {
            var interview = SurveyService.StartTestInterview(CurrentRespondent, surveyProjectId);
            UnitOfWork.Save();
            return RedirectToAction("Index", "Question",
                                    new {
                                        surveyid = interview.SurveyProjectId,
                                        order = questionOrder
                                    });
        }

        private ContentResult GetQuestion(int surveyId, int startOrder)
        {
            var project = ProjectRepository.GetById(surveyId);
            var interview = project.GetInterviewForRespondent(CurrentRespondent.RespondentId);
            if (interview == null)
                return Content("No interview");

            var nextQuestionOrder = SurveyService.GetNextQuestionOrderFx(CurrentRespondent.RespondentId, surveyId, startOrder);
            if (nextQuestionOrder < startOrder)
                nextQuestionOrder = startOrder;

            UnitOfWork.Save();
            if (interview.Completed)
                return Content("By!");

            var question = GetSurveyQuestion(Convert.ToInt32(nextQuestionOrder), interview, project, startOrder);
            if (question == null)
                return Content("No question");

            var variants = interview.GetFilteredAnswersFx(question, startOrder).Select(av => new AnswerVariantRestricted(av));
            if ((!question.IsCompositeQuestion || question.IsGridQuestion) && !variants.Any())
                return Content("No answers");

            var subitems = interview.GetFilteredSubitemsFx(question, startOrder).Select(sq => new SubQuestionRestricted(sq));
            var model = new QuestionModelRestricted {
                                                    Question = new SurveyQuestionRestricted(question),  //  , interview.GetFilteredSubitemsFx(question, startOrder)),
                                                    SubQuestions = subitems.ToArray(),
                                                    AnswerVariants = variants.ToArray() //  question.IsGridQuestion ? question.OrderedAnswerVariants.Select(av => new AnswerVariantRestricted(av)).ToArray() : variants.ToArray()
                                                };
            return Content(model.ToJSON(null));
        }

        private SurveyQuestion GetSurveyQuestion(int order, Interview interview, SurveyProject project, int startOrder)
        {
            var question = _surveyQuestionRepository.GetQuestion(project.SurveyProjectId, order);
/*  ????????????
            if (question == null)
                return null;

            if (interview.ShouldSkip(question) && interview.shouldSkipForTestFx(question, startOrder))    //  !interview.GetFilteredAnswersFx(question, startOrder).Any())
                return null;
*/
            return question;
        }

        private List<int> makeAnswersList(IEnumerable<JSONObject> answers)
        {
            return answers.Select(answer => answer.Integer).ToList();
        }

        private IEnumerable<KeyValuePair<int, string>> makeOpenDictionary(IEnumerable<JSONObject> opensArray)
        {
            return opensArray.ToDictionary(open => open.Array[0].Integer, open => open.Array[1].String);
        }

        private Dictionary<int, int> makeRankDictionary(IEnumerable<JSONObject> ranks)
        {
            return ranks.ToDictionary(rank => rank.Array[0].Integer, rank => rank.Array[1].Integer);
        }

        private SurveyQuestion[] saveModels(IEnumerable<JSONObject> models)
        {
            var result = new SurveyQuestion[models.Count()];
            var i = 0;
            foreach (var model in models) {
                var qm = model.QuestionModel;
                var question = qm.Question;

                _surveyQuestionRepository.save(ref question);
                question.SubQuestions = new Collection<SubQuestion>();
                _subQuestionRepository.save(question, qm.SubQuestions);
                question.AnswerVariants = new Collection<AnswerVariant>();
                _answerVariantRepository.save(question, qm.AnswerVariants);
                _tagRepository.save(question, qm.SubQuestions);
                result[i++] = question;
            }
            return result;
        }

        private void fillDeleted(string deletedQuestionIds, string deletedSubQuestionIds, string deletedAnswerIds, string deletedTagIds)
        {
            _surveyQuestionRepository.deletedIds = JSONObject.CreateFromString(deletedQuestionIds).IntArray;
            _subQuestionRepository.deletedIds = JSONObject.CreateFromString(deletedSubQuestionIds).IntArray;
            _answerVariantRepository.deletedIds = JSONObject.CreateFromString(deletedAnswerIds).IntArray;
            _tagRepository.deletedIds = JSONObject.CreateFromString(deletedTagIds).IntArray;
        }

        private void removeDeleted()
        {
            _tagRepository.deleteDeleted();
            _answerVariantRepository.deleteDeleted();
            _subQuestionRepository.deleteDeleted();
            _surveyQuestionRepository.deleteDeleted();
        }

        private string trySaveAll(IEnumerable<SurveyQuestion> questions)
        {
            var result = string.Empty;
            try {
                UnitOfWork.Save();
                foreach (var question in questions) {
                    _tagRepository.updateConditionString(question);
                    _surveyQuestionRepository.Edit(question);
                }
                UnitOfWork.Save();

            } catch (InvalidOperationException exception) {
                result = "Can't save all. " + exception.Message;
            }
            return result;
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
