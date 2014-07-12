using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyModel.Univer;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class Students2013Controller : ControllerBase
    {
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ISurveyProjectRepository _surveyProjectRepository;
        private readonly IRespondentRepository _respondentRepository;
        private readonly string[] _facilities = new string[] { "Факультет международных отношений", "Факультет социологии", "Факультет политологии", "Экономический факультет"};
        private readonly string[] _programTypes = new string[] { "Бакалавриат", "Специалитет", "Магистратура" };
        //
        // GET: /Students2013/

        public Students2013Controller(IUnitOfWork unitOfWork, IUserService userService,
                                      ISurveyProjectRepository surveyProjectRepository, IRespondentRepository respondentRepository,
                                      IStudentGroupRepository studentGroupRepository, IStudentRepository studentRepository)
            : base(unitOfWork, userService)
        {
            _studentGroupRepository = studentGroupRepository;
            _studentRepository = studentRepository;
            _surveyProjectRepository = surveyProjectRepository;
            _respondentRepository = respondentRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid) {
                if (Membership.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var isReturnUriCorrect = Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 &&
                                             returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") &&
                                             !returnUrl.StartsWith("/\\");
                    return isReturnUriCorrect ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Ваш логин или пароль неправильны.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Register()
        {
            ViewData["Facilities"] = new SelectList(_facilities);
            ViewData["ProgramTypes"] = new SelectList(_programTypes);
            return View();
        }

        [HttpPost]
        public ActionResult Register(StudentRegisterModel model)
        {
            return RedirectToAction("RegisterContinuation", model);
        }

        public ActionResult RegisterContinuation(StudentRegisterModel model)
        {
            if (model.Facility != null) {
                IEnumerable<StudentGroup> groups = _studentGroupRepository.GetByFacilityTypeYear(model.Facility, model.ProgramType, model.YearNum);
                if (!groups.Any())
                    return RedirectToAction("RegisterError", model);

                ViewData["Groupes"] = new SelectList(groups, "StudentGroupId", "GroupCode");
                ViewData["HasGroup"] = !IsMagister(model);
                TempData["GroupId"] = IsMagister(model) ? groups.First().StudentGroupId : 0;
            } else {
                ViewData["Groupes"] = null;
                ViewData["HasGroup"] = false;
                TempData["GroupId"] = model.GroupId;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Reg(StudentRegisterModel model)
        {
            if (!model.Email.IsValidEmail())
                return RedirectToAction("RegisterContinuation", model);
            int groupID = model.GroupId == 0 ? (int)TempData["GroupId"] : model.GroupId;
            StudentGroup studentGroup = _studentGroupRepository.GetById(groupID);
            var respondent = CreateRespondent(studentGroup, model);
            if (respondent == null) {
                TempData["isRepeatedEmail"] = true;
                return RedirectToAction("RegisterError", model);
            }
            Student std = new Student { Name = model.Email, StudentGroup = studentGroup };
            _studentRepository.AddRange(new List<Student> {std});
            _respondentRepository.Add(respondent);
            UnitOfWork.Save();
            return RedirectToAction("Index");
        }

        public ActionResult RegisterError(StudentRegisterModel model)
        {
            ViewData["IsRepeatedEmail"] = TempData["isRepeatedEmail"];
            return View(model);
        }

        public ActionResult List()
        {
            return View(_studentGroupRepository.GetInterviewInfo());
        }

        private bool IsMagister(StudentRegisterModel model)
        {
            return model.ProgramType == _programTypes[2];
        }
        
        private Respondent CreateRespondent(StudentGroup studentGroup, StudentRegisterModel model)
        {
            var surveyProject = _surveyProjectRepository.GetByName(studentGroup.Facility);
            if (surveyProject == null)
                throw new Exception(string.Format("Проект {0} не найден", studentGroup.Facility));

            var username = model.Email;
            var password = model.Password;
            var memberUser = Membership.GetUser(username);
            if (memberUser != null)
                return null;

            Membership.CreateUser(username, password);

            var respondent = new Respondent
            {
                MembershipUserName = username,
                Token = null,
                RespondentEmail = model.Email,
                StudentGroupId = studentGroup.StudentGroupId,
                RespondentComment = password
            };

            respondent.Invitations.Add(
                new SurveyInvitation
                {
                    Respondent = respondent,
                    SurveyProject = surveyProject
                });

            return respondent;
        }
    }
}
