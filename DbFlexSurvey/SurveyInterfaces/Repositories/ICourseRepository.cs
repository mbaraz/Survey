using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyInterfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
//        IEnumerable<Course> Get(string facility, string programType, int yearNum);
        IEnumerable<Course> Get(StudentGroup group, int[] specCodes);
        IEnumerable<Course> Get(string facility);
    }
}
