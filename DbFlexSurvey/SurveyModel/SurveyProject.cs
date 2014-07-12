using System.Collections.Generic;
using System.Linq;
using SurveyCommon;

namespace SurveyModel
{
    public class SurveyProject
    {
        public const string DefaultRemarkPrefix = "нужно выбрать";
        
        public int SurveyProjectId { get; set; }
        public string SurveyProjectName { get; set; }
        public string InviteText { get; set; }

        public virtual ICollection<Interview> Interviews { get; set; }
        public virtual ICollection<SurveyInvitation> Invitations{ get; set; }
        public virtual ICollection<SurveyQuestion> Questions { get; set; }

        public string ProjectUserDescription { get; set; }

        public string Invitation
        {
            get
            {
                if (InviteText == null)
                    return null;
                return InviteText.Split('#')[0];
            }
        }

        public string RemarkPrefix
        {
            get
            {
                if (InviteText == null)
                    return DefaultRemarkPrefix;
                string[] arr = InviteText.Split('#');
                return arr.Length > 1 ? arr[1] : DefaultRemarkPrefix;
            }
        }

        public IEnumerable<SurveyQuestion> OrderedQuestions
        {
            get { return Questions.OrderBy(question => question.QuestionOrder); }
        }

        public IEnumerable<Interview> ActualInterviews
        {
            get { return Interviews.Where(interview => !interview.TestInterview); }
        }

        public bool Active { get; set; }

        public Interview GetInterviewForRespondent(int respondentId)
        {
            return Interviews.SingleOrDefault(interview1 => interview1.RespondentId == respondentId);
        }

        public void InsertQuestion(SurveyQuestion question)
        {
            if (Questions.Any(q => question.QuestionOrder == q.QuestionOrder))
            {
                foreach (
                    var surveyQuestion in Questions.Where(q => q.QuestionOrder >= question.QuestionOrder && q.SurveyQuestionId != question.SurveyQuestionId))
                {
                    surveyQuestion.QuestionOrder++;
                }
            }
            Questions.Add(question);
        }

        public int? GetOrderBefore(int? questionOrder)
        {
            var surveyQuestion = OrderedQuestions.LastOrDefault(q => q.QuestionOrder < questionOrder);
            return surveyQuestion == null ? (int?) null : surveyQuestion.QuestionOrder;
        }

        public int DefaultOrder
        {
            get { return (Questions.MaxOrDefault(question => question.QuestionOrder) + 1); }
        }
    }
}
