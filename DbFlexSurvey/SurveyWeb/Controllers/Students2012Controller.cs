using System.Web.Mvc;
using System.Web.Security;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class Students2012Controller : ControllerBase
    {

        private readonly IStudentGroupRepository _studentGroupRepository;
        //
        // GET: /Students2012/

        public Students2012Controller(IUnitOfWork unitOfWork, IUserService userService, IStudentGroupRepository studentGroupRepository) : base(unitOfWork, userService)
        {
            _studentGroupRepository = studentGroupRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var isReturnUriCorrect = Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\");
                    return isReturnUriCorrect ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Ваш логин или пароль неправильны.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult List()
        {
            return View(_studentGroupRepository.GetInterviewInfo());
        }

    }
}
