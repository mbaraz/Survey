using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Common;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class SurveyQuestionController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly ITagRepository _tagRepository;
        private readonly ISurveyQuestionRepository _questionRepository;
        private readonly ISurveyProjectRepository _projectRepository;


        public SurveyQuestionController(IUserService userService, ISurveyService surveyService, IUnitOfWork unitOfWork, ITagRepository tagRepository, ISurveyQuestionRepository questionRepository, ISurveyProjectRepository projectRepository) : base(unitOfWork, userService)
        {
            _surveyService = surveyService;
            _tagRepository = tagRepository;
            _questionRepository = questionRepository;
            _projectRepository = projectRepository;
        }

        //
        // GET: /SurveyQuestion/Create
        [HttpGet]
        public ActionResult Create(int surveyProjectId)
        {
            var project = _projectRepository.GetById(surveyProjectId);
            var model = new SurveyQuestionCreateModel();
            SetupCreateModel(model, project);
            return View(model);
        }

        private void SetupCreateModel(SurveyQuestionCreateModel model, SurveyProject project)
        {
            SetupModel(model, project);
            model.Questions = CreateQuestionDropDown(project);
            if (model.QuestionOrder == 0)
            {
                model.QuestionOrder = project.DefaultOrder;
            }
        }

        private void SetupModel(SurveyQuestionModelBase model, SurveyProject project)
        {
            model.ProjectName = project.SurveyProjectName;
            model.ProjectId = project.SurveyProjectId;
            model.Tags = _tagRepository.GetTagsForProject(project.SurveyProjectId)
                .Select(
                    tag => new SelectListItem {Value = tag.TagId.ToString(CultureInfo.InvariantCulture), Text = tag.TagName}).
                    ToArray();
        }

        private static IEnumerable<SelectListItem> CreateQuestionDropDown(SurveyProject project)
        {
            return project.Questions.OrderBy(question => question.QuestionOrder).Select(question => new SelectListItem
                                                            {
                                                                Text = string.Format(" перед «{0}»", question.QuestionText),
                                                                Value =
                                                                    question.QuestionOrder.ToString
                                                                    (CultureInfo.InvariantCulture)
                                                            }).Union(new[]
                                                                         {
                                                                             new SelectListItem
                                                                                 {
                                                                                     Text =
                                                                                         "...В конце опроса",
                                                                                     Value =
                                                                                         project.DefaultOrder.ToString(CultureInfo.InvariantCulture),
                                                                                     Selected =  true
                                                                                 }
                                                                         });
        }

        //
        // POST: /SurveyQuestion/Create

        [Authorize(Roles="Staff")]
        [HttpPost]
        public ActionResult Create(SurveyQuestionCreateModel model)
        {
            try {
                var clearedLines = ValueList.FromString(model.QuestionText).ToArray();
                var questionText = clearedLines[0];
                var answerVars = clearedLines.Skip(1);
                var project = _surveyService.AddQuestion(model.ProjectId, questionText, answerVars, model.QuestionOrder, model.ConditionalTagId, model.ConditionalValue, model.Multiple, model.BoundTagId, model.MaxAnswers, model.MaxRank, model.FilterAnswersTagId, model.MinAnswers);
                UnitOfWork.Save();
                return RedirectToAction("Create", "SurveyQuestion", new {project.SurveyProjectId});
            } catch {
                var project = _projectRepository.GetById(model.ProjectId);
                SetupCreateModel(model, project);
                return View(model);
            }
        }

        //
        // GET: /SurveyQuestion/Edit/5
 
        public ActionResult Edit(int surveyQuestionId)
        {
            var question = _questionRepository.GetById(surveyQuestionId);
            var model = new SurveyQuestionEditModel
                            {
                                SurveyQuestionId = surveyQuestionId,
                                QuestionText = question.QuestionText,
                                Multiple = question.MultipleAnswerAllowed,
                                BoundTagId = question.BoundTagId,
                                ConditionalTagId = question.ConditionOnTagId,
                                ConditionalValue = question.ConditionOnTagValue,
                                MaxAnswers = question.MaxAnswers,
                                MinAnswers = question.MinAnswers,
                                MaxRank = question.MaxRank,
                                FilterAnswersTagId = question.FilterAnswersTagId
                            };
            SetupModel(model, question.SurveyProject);
            return View(model);
        }

        //
        // POST: /SurveyQuestion/Edit/5

        //TODO Check field for correct input
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SurveyQuestionEditModel model)
        {
            try {
                _surveyService.EditQuestion(model.SurveyQuestionId, model.QuestionText, model.ConditionalTagId,
                                            model.ConditionalValue, model.Multiple, model.BoundTagId, model.MaxAnswers, model.MaxRank, model.FilterAnswersTagId, model.MinAnswers);

                UnitOfWork.Save();
                return RedirectToAction("DetailsById", "SurveyQuestion", new { model.SurveyQuestionId });
            } catch {
                SetupModel(model, _projectRepository.GetById(model.ProjectId));
                return View(model);
            }
        }

        //
        // GET: /SurveyQuestion/Delete/5
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int surveyQuestionId)
        {
            return View(_questionRepository.GetById(surveyQuestionId));
        }

        //
        // POST: /SurveyQuestion/Delete/5
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Delete(int surveyQuestionId, FormCollection collection)
        {
            var surveyQuestion = _questionRepository.GetById(surveyQuestionId);
            try {
                _surveyService.DeleteQuestion(surveyQuestion);
                UnitOfWork.Save();
                return RedirectToAction("Details", "SurveyProject", new {surveyQuestion.SurveyProjectId});
            } catch {
                return View(surveyQuestion);
            }
        }

        public ActionResult Details(int questionorder, int surveyprojectid)
        {
            var project = _projectRepository.GetById(surveyprojectid);
            return View(project.Questions.Single(question => question.QuestionOrder == questionorder));
        }

        public ActionResult DetailsById(int surveyQuestionId)
        {
            return View("Details",  _questionRepository.GetById(surveyQuestionId));
        }
    }
}
