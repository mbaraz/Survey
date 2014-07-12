using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel.Univer;

namespace SurveyDomain.Repositories
{
    public class StudentRepository : RepositoryBase<Student>, IStudentRepository
    {
        public StudentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Student GetById(int id)
        {
            return DataSet.Single(s => s.StudentGroupId == id);
        }
    }
}
