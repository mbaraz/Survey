using System;
using System.Collections.Generic;
using System.Linq;
using SurveyModel;
using SurveyCommon;
using SurveyModel.Univer;

namespace SurveyDomain.Univer
{
    class CourseFileParser : CsvFileParserBase<Course>
    {
        public CourseFileParser(string dataFile) : base(dataFile)
        {
        }

        protected override IEnumerable<Course> ParseLine(IList<string> fields)
        {
            var teacher = fields[0].Trim();
            var name = fields[1].Trim().Trim('"').Replace("\"\"", "\"").ClearOrderListElement();
            var facility = GetFacilityName(fields[3]);
            var yearNum = GetYearNum(fields[4]);
            bool isOptional;
            string programType = GetProgramType(fields[5]);
            programType += getGroups(fields[2], out isOptional);
//            var isOptional = ParseBoolField(fields[6], new Dictionary<string, bool>{{"да", false}, {"нет", true}}, "Всем читается?");
            var cTypes =
                fields[6].Trim().Split(new[] {',', '\\', '/'}, StringSplitOptions.RemoveEmptyEntries).Select(
                    ct => GetCourseIsPractice(ct.Trim().Replace(")", ""))).ToArray();
            if (!cTypes.Any())
            {
                cTypes = new[] {true,false};
            }
            return
                cTypes.Select(
                        courseIsPractice =>

                        new Course
                            {
                                CourseName = name,
                                TeacherName = teacher,
                                IsPractice = courseIsPractice,
                                IsOptional = isOptional,
                                Facility = facility,
                                ProgramType = programType,
                                YearNum = yearNum
                            }
                    );


        }

        private static bool GetCourseIsPractice(string courseTypeString)
        {
            var dict = new Dictionary<string, bool>
                           {
                               {"лекция", false},
                               {"лекции", false},
                               {"семинар", true},
                               {"семинары", true},
                               {"практика", true}
                           };
            return ParseBoolField(courseTypeString, dict, "Лекция/Семинар?");
        }

        private static string getGroups(string line, out bool isOptional)
        {
            string[] groups = line.Trim().Split(',');
            isOptional = groups.Length == 1 && groups[0] == string.Empty;

            return isOptional ? string.Empty : groups.Aggregate(string.Empty, (current, @group) => current + ("#" + @group.Trim()));
        }
    }
}
