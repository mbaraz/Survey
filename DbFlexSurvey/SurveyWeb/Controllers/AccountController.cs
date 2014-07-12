using System;
using System.Web.Mvc;
using System.Web.Security;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IRespondentRepository _respondentRepository;
        private readonly ITicketRepository _ticketRepository;

        public AccountController(IUserService userService, IUnitOfWork unitOfWork, IRespondentRepository respondentRepository, ITicketRepository ticketRepository) : base(unitOfWork, userService)
        {
            _respondentRepository = respondentRepository;
            _ticketRepository = ticketRepository;
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);    // "tsarev", model.RememberMe); 
                    var isReturnUriCorrect = Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\");
                    return isReturnUriCorrect ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Ваш логин или пароль неправильны.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterWithTicket(Guid ticketGuid)
        {
            var respondent = _respondentRepository.GetByToken(ticketGuid);
            if (respondent != null)
            {
                return ShowRegisterForm(ticketGuid, respondent.RespondentEmail);
            }
            var ticket = _ticketRepository.GetByToken(ticketGuid);
            if (ticket == null)
            {
                return RedirectToAction("IncorrectInvitation");
            }
            if (ticket.ExpireDate != null && ticket.ExpireDate < DateTime.Now)
            {
                return RedirectToAction("TicketExpired", "Account");
            }
            return ShowRegisterForm(ticketGuid, null);
        }

        [Obsolete]
        public ActionResult RegisterWithInvite(Guid inviteGuid)
        {
            return RedirectToAction("RegisterWithTicket", "Account", new {ticketGuid = inviteGuid});
        }

        private ActionResult ShowRegisterForm(Guid inviteGuid, string email)
        {
            return View("Register", new RegisterModel
                                        {
                                            Invite = inviteGuid,
                                            Email = email,
                                            Password = Membership.GeneratePassword(10, 0),
                                            UserName = "r" + Membership.GetAllUsers().Count + 1
                                        });
        }

        [HttpPost]
        public ActionResult RegisterWithInvite(RegisterModel model)
        {
            return Register(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var createStatus = RegisterUser(model.UserName, model.Password, model.Email, model.Invite);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View("Register", model);
        }

        private MembershipCreateStatus RegisterUser(string userName, string password, string email, Guid inviteGuid)
        {
// Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(userName, password, email, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(userName, false /* createPersistentCookie */);
                UserService.AfterRegister(userName, inviteGuid, email);
                UnitOfWork.Save();
            }
            return createStatus;
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                var changePasswordSucceeded = TryChangePasswordSucceeded(model);

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool TryChangePasswordSucceeded(ChangePasswordModel model)
        {
// ChangePassword will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                var currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                return currentUser != null && currentUser.ChangePassword(model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                return false;
            }
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Пользователь с таким именем уже зарегистрирован. Пожалуйста, введите другое имя пользователя.";

                //TODO Сделать ссылку
                case MembershipCreateStatus.DuplicateEmail:
                    return "Пользователь с этим email уже есть. Воспользуйтесь функцией напоминания пароля.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Неверный пароль. Повторите ввод пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Некорректный e-mail адрес. Пожалуйста, повторите ввод.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Неправильное имя пользователя. Пожалуйста, повторите ввод.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult IncorrectInvitation()
        {
            return View();
        }

        public ActionResult TicketExpired()
        {
            return View();
        }
    }
}
