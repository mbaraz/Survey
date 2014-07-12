using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel.Univer;

namespace SurveyDomain.Repositories
{
    public class CourseRepository : RepositoryBase<Course>, ICourseRepository
    {
        public CourseRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Course GetById(int id)
        {
            return DataSet.Single(course => course.CourseId == id);
        }

        public IEnumerable<Course> Get(StudentGroup group, int[] specCodes)
        {
            List<Course> result = new List<Course>();
            IEnumerable<Course> courseList = DataSet.Where(c => c.Facility == group.Facility && c.ProgramType.Contains(group.ProgramType) && c.YearNum == group.YearNum);
            foreach (var course in courseList)
                if (course.IsSuitable(group.GroupCode, specCodes))
                    result.Add(course);

            return result;
        }

        public IEnumerable<Course> Get(string facility)
        {
            return DataSet.Where(c => c.Facility == facility);
        }
    }
}
