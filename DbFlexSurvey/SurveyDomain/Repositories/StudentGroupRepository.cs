using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyDomain.Repositories
{
    public class StudentGroupRepository : RepositoryBase<StudentGroup>, IStudentGroupRepository
    {
        private IRespondentRepository _respondentRepository;
        public StudentGroupRepository(IUnitOfWork unitOfWork, IRespondentRepository respondentRepository) : base(unitOfWork)
        {
            _respondentRepository = respondentRepository;
        }

        public StudentGroup GetById(int id)
        {
            return DataSet.Single(sg => sg.StudentGroupId == id);
        }

        public IEnumerable<StudentGroup> GetByFacility(string facility)
        {
            return Get(sg => sg.Facility == facility);
        }

        public IEnumerable<StudentGroup> GetByFacilityTypeYear(string facility, string type, string year)
        {
            string facilityTrimedName = Facility.GetTrimedName(facility);
            int yearNumber = Int32.Parse(year);
            return Get(sg => sg.Facility == facilityTrimedName && sg.ProgramType == type && sg.YearNum == yearNumber);
        }

        private IEnumerable<StudentGroup> Get(Expression<Func<StudentGroup, bool>> expression)
        {
            return
                DataSet.Include("Students").Where(expression).OrderBy(sg => sg.Facility).ThenBy(
                    sg => sg.ProgramType).ThenBy(sg => sg.YearNum).ThenBy(sg => sg.GroupCode);
        }

        public IEnumerable<StudentGroup> GetStudentGroups()
        {
            return Get(sg => true);
        }

        public IEnumerable<StudentGroupInterviewInfo> GetInterviewInfo()
        {
            return DataSet.Select(sg => new StudentGroupInterviewInfo()
                                            {
                                                StudentGroup = sg,
                                                StudentsCount = sg.Students.Count(),
                                                InterviewsCount =
                                                    _context.Set<Respondent>().Count(r => r.Interviews.Any(i => i.Completed) && r.StudentGroupId == sg.StudentGroupId)
                                            }).OrderBy(i => i.StudentGroup.Facility).ThenBy(i => i.StudentGroup.ProgramType).ThenBy(i => i.StudentGroup.YearNum).ThenBy(i => i.StudentGroup.GroupCode);
        }
    }
}
