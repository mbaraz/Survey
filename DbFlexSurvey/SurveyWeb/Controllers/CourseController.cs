using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyModel.Univer;
using SurveyWeb.Models.Univer;

namespace SurveyWeb.Controllers
{
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUniverService _univerService;
        private readonly ISurveyProjectRepository _surveyProjectRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IRespondentRepository _respondentRepository;

        public CourseController(IUnitOfWork unitOfWork, IUserService userService, ICourseRepository courseRepository, IUniverService univerService, ISurveyProjectRepository surveyProjectRepository, IStudentGroupRepository studentGroupRepository, IRespondentRepository respondentRepository) : base(unitOfWork,userService)
        {
            _courseRepository = courseRepository;
            _univerService = univerService;
            _surveyProjectRepository = surveyProjectRepository;
            _studentGroupRepository = studentGroupRepository;
            _respondentRepository = respondentRepository;
        }

        //
        // GET: /Course/

        [Authorize(Roles = "Staff")]
        public ActionResult Index()
        {
            return ShowIndex();
        }

        private ActionResult ShowIndex()
        {
            return View("Index", new UniverInfoModel(_surveyProjectRepository.GetActiveEmptyProjects()));
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Result(string facility)
        {
            ViewBag.Facility = facility;
            return View(_univerService.GetCoursesWithAnswers(facility));
        }

        //
        // POST: /Course/UploadCourse

        [HttpPost]
        [Authorize(Roles="Staff")]
        public ActionResult UploadCourse(HttpPostedFileBase file)
        {
            try
            {
                string datafile;
                using (var ts = new StreamReader(file.InputStream, Encoding.GetEncoding(1251))) {
                    datafile = ts.ReadToEnd();
                }
                _univerService.UploadCourses(datafile);
                UnitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = exc.Message;
                return ShowIndex();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult UploadStudents(HttpPostedFileBase file)
        {
            try
            {
                string datafile;
                using (var ts = new StreamReader(file.InputStream, Encoding.GetEncoding(1251))) {
                    datafile = ts.ReadToEnd();
                }
                _univerService.UploadStudents(datafile);
                UnitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = exc.Message;
                return ShowIndex();
            }
        }
        
        //
        // GET: /Course/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Course/Edit/5

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
        // GET: /Course/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Course/Delete/5

        public ActionResult Generate(UniverInfoModel model)
        {
            try
            {
                _univerService.Generate(model.SurveyProjectId);
                UnitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = exc.Message;
                return ShowIndex();
            }
        }

        [Authorize(Roles="Staff")]
        public ActionResult PasswordLists(string facility)
        {
            var groups = _studentGroupRepository.GetByFacility(facility);
            var respondents = _respondentRepository.GetStudents().ToArray();
            var list = (from studentGroup in groups
                        select new GroupPasswordInfoModel
                                   {
                                       StudentGroup = studentGroup,
                                       Students = studentGroup.Students.Select(s => s.Name).OrderBy(s=>s).ToList(),
                                       Passwords =
                                           respondents.Where(r => r.StudentGroupId == studentGroup.StudentGroupId).
                                           Select(r => new GroupPasswordInfoModel.PasswordInfo
                                                           {
                                                               Password = r.RespondentComment,
                                                               Username = r.MembershipUserName
                                                           }).ToList()
                                   }).ToArray();
            return View(list);
        }

        [Authorize(Roles="Staff")]
        [HttpPost]
        public ActionResult CreatePassword()
        {
            try
            {
                _respondentRepository.AddRange(CreateRespondents());
                UnitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = "Ошибка";
                while (exc != null)
                {
                    ViewBag.ErrorMsg += " " + exc.Message;
                    exc = exc.InnerException;
                }
                return ShowIndex();
            }
        }

        private IEnumerable<Respondent> CreateRespondents()
        {
            return
                _studentGroupRepository.GetAll().Select(
                    studentGroup =>
                    CreateRespondents(studentGroup, studentGroup.Students.Count(), 
                                      _respondentRepository.GetCountByGroup(studentGroup.StudentGroupId))).SelectMany(r => r);
        }

        private IEnumerable<Respondent> CreateRespondents(StudentGroup studentGroup, int requiredCount, int presentCount)
        {
            if (requiredCount == presentCount)
                yield break;

            var surveyProject = _surveyProjectRepository.GetByName(studentGroup.Facility);
            if (surveyProject == null)
                throw new Exception(string.Format("Проект {0} не найден", studentGroup.Facility));

            while (requiredCount > presentCount)
            {
                var username = string.Format("SG_{0}_{1}", studentGroup.StudentGroupId, (presentCount + 1));
                var password = Membership.GeneratePassword(6, 0);
                var memberUser = Membership.GetUser(username);
                if (memberUser != null)
                {
                    memberUser.ChangePassword(memberUser.ResetPassword(), password);
                }
                else
                {
                    Membership.CreateUser(username, password);
                }

                var respondent = new Respondent
                                     {
                                         MembershipUserName = username,
                                         Token = null,
                                         RespondentEmail = null,
                                         StudentGroupId = studentGroup.StudentGroupId,
                                         RespondentComment = password
                                     };


                respondent.Invitations.Add(
                    new SurveyInvitation
                        {
                            Respondent = respondent,
                            SurveyProject = surveyProject
                        });
                
                presentCount++;
                yield return respondent;
            }
        }

        public ActionResult Export(int id)
        {
            var course = _courseRepository.GetById(id);
            return File(_univerService.GetSpsByteContent(course), "application/sps", course.CourseDispName + ".sps");
        }

        public ActionResult ExportAll(string facility)
        {
            FileStream fileStream = _univerService.ExportAll(facility);
            return File(fileStream,  "application/zip", facility + " результаты опроса.zip");
        }

        public ActionResult ExportComments(int id)
        {
            var course = _courseRepository.GetById(id);
            return File(_univerService.GetCommentsByteContent(course), "text/plain", course.CourseDispName + ".txt");
        }

        public ActionResult ExportFacilityComments(string facility)
        {
            return File(_univerService.GetFacilityCommentsByteContent(facility), "text/plain", facility + ".txt");
        }
    }
}
