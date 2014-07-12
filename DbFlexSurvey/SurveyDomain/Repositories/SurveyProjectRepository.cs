using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class SurveyProjectRepository : RepositoryBase<SurveyProject>, ISurveyProjectRepository
    {
        public SurveyProjectRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public SurveyProject GetById(int id)
        {
            return DataSet.Single(project => project.SurveyProjectId == id);
        }

        public IEnumerable<SurveyProject> GetActiveProjects()
        {
            return DataSet.Where(project => project.Active);
        }

        public IEnumerable<SurveyProject> GetActiveEmptyProjects()
        {
            return DataSet.Where(project => project.Active && !project.Questions.Any());
        }

        public SurveyProject GetByName(string facility)
        {
            return DataSet.SingleOrDefault(p => p.Active && p.SurveyProjectName == facility);
        }
    }
}
