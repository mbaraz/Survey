using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SurveyCommon;
using SurveyInterfaces;
using SurveyModel;

namespace SurveyWeb.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IUnitOfWork UnitOfWork { get; private set; }
        protected readonly IUserService UserService;

        protected ControllerBase(IUnitOfWork unitOfWork, IUserService userService)
        {
            UnitOfWork = unitOfWork;
            UserService = userService;
        }

        private static MembershipUser MembershipUser
        {
            get
            {
                var membershipUser = Membership.GetUser();
                if (membershipUser == null)
                    throw new Exception("Should be authorized here");

                return membershipUser;
            }
        }

        protected Respondent CurrentRespondent { get { return UserService.GetRespondentByName(MembershipUser.UserName); } }

        protected bool IsGet { get { return Request.HttpMethod == "GET"; } }

        protected bool IsCurrentUserAdmin { get { return Roles.IsUserInRole(MembershipUser.UserName, "Admin"); } }

        protected IEnumerable<KeyValuePair<int, string>> GetDictionary(string prefix)
        {
            return GetKeys(prefix).ToDictionary(key => key, key => Request.Form[prefix + key]).Where(arg => !string.IsNullOrWhiteSpace(arg.Value));
        }

        protected IEnumerable<int> GetKeys(string prefix)
        {
            return KeysByPrefix(prefix).Select(pair => pair.ToNullableInt()).CheckForNulls();
        }

        private IEnumerable<string> KeysByPrefix(string prefix)
        {
            return Request.Form.AllKeys.Where(key => key.StartsWith(prefix)).Select(key => key.Replace(prefix, ""));
        }

    }
}