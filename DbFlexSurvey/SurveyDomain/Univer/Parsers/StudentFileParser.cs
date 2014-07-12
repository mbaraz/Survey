using System.Collections.Generic;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel.Univer;

namespace SurveyDomain.Univer
{
    public class StudentFileParser : CsvFileParserBase<Student>
    {


        private readonly IMapper<StudentGroup> _groupMapper;

        public StudentFileParser(string dataFile, IRepository<StudentGroup> repository) : base(dataFile)
        {
            _groupMapper = new Mapper<StudentGroup>(repository);
        }

        protected override IEnumerable<Student> ParseLine(IList<string> fields)
        {
            var studentName = fields[0].Trim();
            var facility = GetFacilityName(fields[1]);
            var year = GetYearNum(fields[2]);
            var programType = GetProgramType(fields[3]);
            var groupCode = fields[4].Trim();

            var group =
                _groupMapper.FindOrCreateGroup(
                    sg =>
                    sg.Facility == facility && sg.YearNum == year && sg.ProgramType == programType &&
                    sg.GroupCode == groupCode, () => new StudentGroup
                                                         {
                                                             Facility = facility,
                                                             GroupCode = groupCode,
                                                             ProgramType = programType,
                                                             YearNum = year
                                                         });
            yield return new Student
                             {
                                 Name = studentName,
                                 StudentGroup = group
                             };
        }
    }
}
