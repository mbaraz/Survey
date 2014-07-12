using System.Linq;
namespace SurveyModel.Univer
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public bool IsPractice { get; set; }
        public bool IsOptional { get; set; }
        public string Facility { get; set; }
        public int YearNum { get; set; }
        public string ProgramType { get; set; }

        public string CourseDispName
        {
            get { return string.Format("{0} ({1}) — {2}", CourseName, TeacherName, IsPractice? "семинар" : "лекция"); }
        }

        public bool IsSuitable(string groupCode, int[] specCodes)
        {
            string[] groups = ProgramType.Split('#');
            if (IsOptional || groups.Contains(groupCode))
                return true;
            foreach (int specCode in specCodes)
                if (groups.Contains(specCode.ToString()))
                    return true;

            return false;
        }
    }
}
