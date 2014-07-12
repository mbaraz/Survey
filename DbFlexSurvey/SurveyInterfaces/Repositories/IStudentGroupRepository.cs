using System.Collections.Generic;
using System.Linq;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyInterfaces.Repositories
{
    public interface IStudentGroupRepository : IRepository<StudentGroup>
    {
        IEnumerable<StudentGroup> GetByFacility(string facility);
        IEnumerable<StudentGroup> GetByFacilityTypeYear(string facility, string type, string year);
        IEnumerable<StudentGroup> GetStudentGroups();
        IEnumerable<StudentGroupInterviewInfo> GetInterviewInfo();
    }
}
