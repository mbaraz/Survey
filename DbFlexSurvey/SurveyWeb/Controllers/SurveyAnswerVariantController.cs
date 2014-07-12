using System;
using System.Linq;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class SurveyAnswerVariantController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly IAnswerVariantRepository _answerVariantRepository;
        //
        // GET: /SurveyAnswerVariant/Create

        public SurveyAnswerVariantController(IUserService userService, ISurveyService surveyService, IUnitOfWork unitOfWork, IAnswerVariantRepository answerVariantRepository) : base(unitOfWork, userService)
        {
            _surveyService = surveyService;
            _answerVariantRepository = answerVariantRepository;
        }

        //
        // POST: /SurveyAnswerVariant/Create

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Create(int surveyQuestionId, string text)
        {
            try {
                var answerVariant = _surveyService.CreateAnswerVariant(surveyQuestionId, text, Request.Form.AllKeys.Contains("isOpen"));
                UnitOfWork.Save();
                return RedirectToQuestion(answerVariant.SurveyQuestionId);
            } catch {
                return RedirectToQuestion(surveyQuestionId);
            }
        }
        
        //
        // GET: /SurveyAnswerVariant/Edit/5

        public ActionResult Edit(int answerVariantId)
        {
            AnswerVariant av = _answerVariantRepository.GetById(answerVariantId);
            AnswerVariantModel avm = new AnswerVariantModel(av);
            return View(avm);
        }

        //
        // POST: /SurveyAnswerVariant/Edit/5

        [HttpPost]
        public ActionResult Edit(AnswerVariantModel answerVariantModel)
        {
            try {
                AnswerVariant answerVariant = answerVariantModel.Variant;
                _answerVariantRepository.Edit(answerVariant);
                UnitOfWork.Save();
                return RedirectToQuestion(answerVariant.SurveyQuestionId);
            } catch {
                return View(answerVariantModel);
            }
        }

        private ActionResult RedirectToQuestion(int surveyQuestionId)
        {
            return RedirectToAction("DetailsById", "SurveyQuestion", new { surveyQuestionId });
        }

        //
        // GET: /SurveyAnswerVariant/Delete/5
                [Authorize(Roles = "Staff")]
        public ActionResult Delete(int answerVariantId)
        {
            return View(_answerVariantRepository.GetById(answerVariantId));
        }

        //
        // POST: /SurveyAnswerVariant/Delete/5
        
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Delete(AnswerVariant answerVariant)
        {
            try {
                _answerVariantRepository.Delete(_answerVariantRepository.GetById(answerVariant.AnswerVariantId));
                UnitOfWork.Save();
                return RedirectToQuestion(answerVariant.SurveyQuestionId);
            } catch (Exception) {
                return View();
            }
        }
    }
}
