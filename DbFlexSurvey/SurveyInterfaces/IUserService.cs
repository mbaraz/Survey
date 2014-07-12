using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface IUserService
    {
        Respondent AfterRegister(string userName, Guid inviteGuid, string email);
        Respondent GetRespondentByName(string getUser);
    }
}
