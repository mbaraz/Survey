using System;
using System.Collections;
using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface IRespondentRepository : IRepository<Respondent>
    {
        int GetCount();
        Respondent GetByName(string getUser);
        Respondent GetByEmail(string email);
        Respondent GetByToken(Guid token);
        IEnumerable<Respondent> GetRespondentsByIds(IEnumerable<int> respondentIds);
        IEnumerable<Respondent> GetUnInvited();
        int GetCountByGroup(int studentGroupId);
        IEnumerable<Respondent> GetStudents();
    }
}
