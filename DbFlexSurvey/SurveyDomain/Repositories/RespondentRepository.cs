using System;
using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class RespondentRepository : RepositoryBase<Respondent>, IRespondentRepository
    {
        public RespondentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Respondent GetById(int id)
        {
            return DataSet.Single(respondent => respondent.RespondentId == id);
        }

        public int GetCount()
        {
            return DataSet.Count();
        }

        public Respondent GetByName(string getUser)
        {
            return DataSet.SingleOrDefault(respondent => respondent.MembershipUserName == getUser);
        }

        public Respondent GetByEmail(string email)
        {
            return DataSet.SingleOrDefault(respondent => respondent.RespondentEmail == email);
        }

        public Respondent GetByToken(Guid token)
        {
            if (token == Guid.Empty)
            {
                return null;
            }
            return DataSet.SingleOrDefault(respondent => respondent.Token == token);
        }

        public IEnumerable<Respondent> GetRespondentsByIds(IEnumerable<int> respondentIds)
        {
            return DataSet.Where(respondent => respondentIds.Contains(respondent.RespondentId));
        }

        public IEnumerable<Respondent> GetUnInvited()
        {
            return DataSet.Where(r => r.RespondentEmail != null && r.Token == null && r.MembershipUserName == null);
        }

        public int GetCountByGroup(int studentGroupId)
        {
            return DataSet.Count(r => r.StudentGroupId == studentGroupId);
        }

        public IEnumerable<Respondent> GetStudents()
        {
            return DataSet.Where(r => r.StudentGroupId != null).OrderBy(r => r.StudentGroupId);
        }

        public IEnumerable<Respondent> GetByGroup(int studentGroupId)
        {
            return DataSet.Where(r => r.StudentGroupId == studentGroupId);
        }
    }
}
