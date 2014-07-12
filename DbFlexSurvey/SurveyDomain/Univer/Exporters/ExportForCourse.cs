using System.Collections.Generic;
using System.Linq;
using SurveyModel;

namespace SurveyDomain.Univer
{
    internal class ExportForCourse : UniverExportBase
    {
        private readonly int _courseId;
        private readonly Tag _commonTag;
        private readonly Tag _optionalTag;

        public ExportForCourse(SurveyProject project, ICollection<Tag> projectTags, int courseId)
            : base(project)
        {
            _courseId = courseId;
            _commonTag = projectTags.Single(tag => tag.TagName == Constants.DispCommon);
            _optionalTag = projectTags.Single(tag => tag.TagName == Constants.DispOptional);

        }

        protected override IEnumerable<SurveyQuestion> GetQuestions
        {
            get
            {
                return
                    base.GetQuestions.Where(q => q.ConditionOnTagValue == _courseId);
            }
        }

        public override IEnumerable<Interview> GetInterviews()
        {
            return base.GetInterviews().Where(IsCourseInterview);
        }

        private bool IsCourseInterview(Interview i)
        {
            return i.TagValues.Where(t => t.TagId == _commonTag.TagId || t.TagId == _optionalTag.TagId).Any(t => t.Value == _courseId);
        }
    }
}
