using System.Collections.Generic;
using System.Linq;
using SurveyModel;

namespace SurveyDomain.Univer
{
    class ExportCourseComments : ExportForCourse
    {
        public ExportCourseComments(SurveyProject project, ICollection<Tag> projectTags, int courseId) : base(project, projectTags, courseId)
        {
        }

        protected override IEnumerable<SurveyQuestion> GetQuestions
        {
            get
            {
                return base.GetQuestions.Where(q=>q.AnswerVariants.Any(av=>av.IsOpenAnswer));
            }
        }
    }
}
