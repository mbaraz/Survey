using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface IInterviewRepository : IRepository<Interview>
    {
        Interview Get(int project, int currentRespondent);
        bool hasNotTestInterview(SurveyProject project);
        void DeleteAll(SurveyProject project);
    }
}
