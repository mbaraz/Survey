namespace SurveyModel.Univer
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int StudentGroupId { get; set; }
        public virtual StudentGroup StudentGroup { get; set; }

        public override bool Equals(object obj)
        {
            var rhs = obj as Student;
            return rhs != null && rhs.Name == Name && rhs.StudentGroup.Equals(StudentGroup);
        }

        public override int GetHashCode()
        {
            var hashCode = StudentGroup == null ? StudentGroupId :  StudentGroup.GetHashCode();
            return Name.GetHashCode() ^ hashCode;
        }
    }
}
