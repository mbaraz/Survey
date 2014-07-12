using System.Collections.Generic;

namespace SurveyModel.Univer
{
    public class StudentGroup
    {
        
        public StudentGroup()
        {
            Students = new HashSet<Student>();
        }
        public int StudentGroupId { get; set; }

        public string Facility { get; set; }
        public int YearNum { get; set; }
        public string ProgramType { get; set; }
        public string GroupCode { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public string DispName
        {
            get { return string.Format("{0} {1} {2} курс группа {3}", Facility, ProgramType, YearNum, GroupCode); }
        }
    }
}
