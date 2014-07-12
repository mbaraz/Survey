using System.Collections.ObjectModel;
using System.Web.Mvc;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Common;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly ISurveyQuestionRepository _questionRepository;
        private readonly ISurveyProjectRepository _projectRepository;
        //
        // GET: /Tag/

        public TagController(IUserService userService, IUnitOfWork unitOfWork, ITagRepository tagRepository, ISurveyQuestionRepository questionRepository, ISurveyProjectRepository projectRepository) : base(unitOfWork, userService)
        {
            _tagRepository = tagRepository;
            _questionRepository = questionRepository;
            _projectRepository = projectRepository;
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Index()
        {
            var model = new TagListModel
                            {
                                Tags = _tagRepository.GetAll(),
                                CreateModel = new TagCreateModel(_projectRepository.GetActiveProjects())
                            };
            return View(model);
        }

        //
        // GET: /Tag/Details/5
        [Authorize(Roles = "Staff")]
        public ActionResult Details(int tagId)
        {
            return View(GetTagViewModel(tagId));
        }
        
        //
        // GET: /Tag/Edit/5
 [Authorize(Roles = "Staff")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Tag/Edit/5
        [Authorize(Roles = "Staff")]
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
        // GET: /Tag/Delete/5
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int tagId)
        {
            return View(GetTagViewModel(tagId));
        }

        private TagViewModel GetTagViewModel(int tagId)
        {
            var model = new TagViewModel
                            {
                                Tag = _tagRepository.GetById(tagId),
                                BoundQuestions = _questionRepository.GetQuestionsBoundToTag(tagId),
                                ConditionalQuestions = _questionRepository.GetQuestionsConditionalOnTag(tagId),
                                FilteredQuestions = _questionRepository.GetFilteredQuestionsOnTag(tagId)
                            };
            return model;
        }

        //
        // POST: /Tag/Delete/5

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Delete(int tagId, FormCollection collection)
        {
            var model = GetTagViewModel(tagId);
            try
            {
               
                _tagRepository.Delete(model.Tag);
                UnitOfWork.Save();
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Create(TagCreateModel model)
        {
            var tag = new Tag
                          {
                              TagName = model.Name,
                              SurveyProjectId = model.ProjectId,
                              ValueLabels = new Collection<TagValueLabel>()

                          };
            var code = 1;
            foreach (var value in ValueList.FromString(model.Values))
            {
                tag.ValueLabels.Add(new TagValueLabel
                                        {
                                            Label = value, ValueCode = code++
                                        });
            }
            _tagRepository.Add(tag);
            UnitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
