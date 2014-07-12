using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces.Repositories
{
    public interface ISurveyProjectRepository : IRepository<SurveyProject>
    {
        IEnumerable<SurveyProject> GetActiveProjects();
        IEnumerable<SurveyProject> GetActiveEmptyProjects();
        SurveyProject GetByName(string facility);
    }
}
