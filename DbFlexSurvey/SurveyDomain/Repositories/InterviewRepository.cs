using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class InterviewRepository : RepositoryBase<Interview>, IInterviewRepository
    {
        public InterviewRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Interview GetById(int id)
        {
            return DataSet.Single(interview => interview.InterviewId == id);
        }

        public Interview Get(int project, int currentRespondent)
        {
            return DataSet.Include("TagValues").SingleOrDefault(i => i.SurveyProjectId == project && i.RespondentId == currentRespondent);
        }

        public bool hasNotTestInterview(SurveyProject project)
        {
            return project.Interviews.Any(interview => !interview.TestInterview);
        }

        public void DeleteAll(SurveyProject project)
        {
            var interviews = project.Interviews;
            var i = interviews.Count - 1;
            while (i >= 0)
                Delete(interviews.ElementAt(i--));
        }
    }
}
