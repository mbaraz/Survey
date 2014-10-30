using System.Collections.Generic;
using System.Linq;
using SurveyCommon;

namespace SurveyModel
{
    public class SurveyQuestion
    {
        private const int RANK_FAILED_TO_ANSWER = -1;

        public int SurveyQuestionId { get; set; }
        public int SurveyProjectId { get; set; }
        public virtual SurveyProject SurveyProject { get; set; }
        public virtual ICollection<AnswerVariant> AnswerVariants { get; set; }

        public virtual ICollection<SubQuestion> SubQuestions { get; set; }

        public string QuestionName { get; set; }
        public int? QuestionType { get; set; }
        public string QuestionText { get; set; }
        public bool MultipleAnswerAllowed { get; set; }
        public int? AnswerOrdering { get; set; }
        public int? MaxAnswers { get; set; }
        public int? MinAnswers { get; set; }
        public int QuestionOrder { get; set; }

        public int? BoundTagId { get; set; }
        public virtual Tag BoundTag { get; set; }

        public string ConditionString { get; set; }
        public int? ConditionOnTagId { get; set; }
        public virtual Tag ConditionOnTag { get; set; }
        public int? ConditionOnTagValue { get; set; }

        public int? FilterAnswersTagId { get; set; }
        public virtual Tag FilterAnswersTag { get; set; }

        public int? MaxRank { get; set; }
        public int? MinRank { get; set; }

        public bool IsRankQuestion
        {
            get { return MaxRank != null && MaxRank != 0; }
        }
// Flex
//        private bool IsRatingOrRankingQuestion { get { return QuestionType == 2 || QuestionType == 3; } }

        public bool HasSingleAnswer { get { return QuestionType == 4 || QuestionType == 5; } }

        public bool HasNoAnswer { get { return QuestionType == 2 || QuestionType == 3 || QuestionType == 6; } }
// For Grid Question
        public bool IsGridQuestion { get { return QuestionType >= 7 || (IsRankQuestion && QuestionText.Contains(AnswerVariant.TextPartsDelimiter)); } }

        public bool IsCompositeQuestion { get { return QuestionType >= 2 && !HasSingleAnswer; } }

//        private bool IsDragQuestion { get { return QuestionType == 6; } }

        public string InstantText { get { return (QuestionText == null) ? QuestionText : QuestionText.Split(AnswerVariant.TextPartsDelimiter)[0]; } }

        public IEnumerable<string> SubitemsStrings
        {
            get {
                if (IsOldFashioned)
                    return QuestionText.Split(AnswerVariant.TextPartsDelimiter).Skip(1);

                return OrderedSubQuestions.Select(sq => sq.QuestionText);
            }
        }

        public bool IsOldFashioned { get { return QuestionType == null; } }
// End Flex
        public virtual ICollection<InterviewAnswer> InterviewAnswers { get; set; }

        public int ConditionOnTagOrder { get { return ConditionOnTag == null ? int.MaxValue : ConditionOnTag.BoundedQuestionOrder; } }

        public int FilterAnswersTagOrder { get { return FilterAnswersTag == null ? int.MaxValue : FilterAnswersTag.BoundedQuestionOrder; } }

        private int MaxAnswersCount
        {
            get {
//                return MultipleAnswerAllowed ? (MaxAnswers ?? int.MaxValue) : 1;
                if (!MultipleAnswerAllowed)
                    return 1;

                if (MaxAnswers == null || MaxAnswers == 1)
                    return int.MaxValue;

                return MaxAnswers.Value;
            }
        }

        private int MinAnswersCount
        {
            get { return MultipleAnswerAllowed ? (MinAnswers ?? 1) : 1; }
        }

        public void ValidateAnswer(IInterviewAnswer interviewAnswer, Interview interview)
        {
            // ' && QuestionName != "OptionalSelector"' - необходимо только для опроса студентов, чтобы в списке дисциплин по выбору была возможность "просто нажать кнопку "Длее""
            if (interviewAnswer.Answers.Count == 0 && QuestionName != "OptionalSelector")
                throw new QuestionHasNoAnswerException();

            if (interviewAnswer.Answers.Count < MinAnswersCount && QuestionName != "OptionalSelector")
                throw new QuestionTooMinAnswersException();
// Flex
            if (HasSingleAnswer || IsCompositeQuestion)
                return;
// End Flex
            if (interviewAnswer.Answers.Count > MaxAnswersCount)
                throw new QuestionTooManyAnswersException();
// Flex: For Grid Question
//  ?????            if (IsGridQuestion)
                return;

            CheckNoUncorrectAnswers<UnknownAnswerException>(interviewAnswer.Answers.Where(a => AnswerVariants.All(av => av.AnswerCode != a)));

/* Flex: For Rating or Ranking Question
            if (IsRatingOrRankingQuestion)
                return;
*/ 
// End Flex
            CheckNoUncorrectAnswers<OpenAnswerHasNoOpenPartException>(AnswerVariants.Where(
                av => av.IsOpenAnswer && interviewAnswer.Answers.Contains(av.AnswerCode) &&
                      !interviewAnswer.OpenAnswers.ContainsKey(av.AnswerCode)).Select(av => av.AnswerCode));

            //CheckNoUncorrectAnswers<OpenPartWithoutAnswerException>(
              //  interviewAnswer.OpenAnswers.Where(oa => !interviewAnswer.Answers.Contains(oa.Key)).Select(oa => oa.Key));

            if (!IsRankQuestion && interviewAnswer.Rank.Any())
                throw new QuestionHasUnexpectedRanksException();

            if (IsRankQuestion) {
                CheckNoUncorrectAnswers<RankAnswerNoRankPartException>(interview.GetFilteredAnswers(this).Where(av => !interviewAnswer.Rank.ContainsKey(av.AnswerCode)));
                CheckNoUncorrectAnswers<RankAnswerWrongException>(interviewAnswer.Rank.Where(r => r.Value > MaxRank).Select(r => r.Key));
                CheckNoUncorrectAnswers<RankAnswerWrongException>(interviewAnswer.Rank.Where(r => r.Value < RANK_FAILED_TO_ANSWER).Select(r => r.Key));
            }
        }

        private static void CheckNoUncorrectAnswers<T>(IEnumerable<int> incorrectAnswers) where T : AnswerProblemException, new()
        {
            var answersWithoutOpen = incorrectAnswers.ToArray();
            if (answersWithoutOpen.Any())
                throw new T {AnswerCode = answersWithoutOpen.First()};
        }

        private static void CheckNoUncorrectAnswers<T>(IEnumerable<AnswerVariant> incorrectAnswers) where T : AnswerProblemException, new()
        {
            CheckNoUncorrectAnswers<T>(incorrectAnswers.Select(av => av.AnswerCode));
        }

        public void InitAnswers()
        {
            var i = 1;
            foreach (var answerVariant in AnswerVariants) {
                answerVariant.AnswerCode = i;
                answerVariant.AnswerOrder = i;
                answerVariant.TagValue = i;
                answerVariant.SurveyQuestion = this;
                i++;
            }
        }

        public IEnumerable<AnswerVariant> OrderedAnswerVariants
        {
            get { return AnswerVariants.OrderBy(variant => variant.AnswerOrder); }
        }

        public IEnumerable<AnswerVariant> FxOrderedAnswerVariants
        {
            get { return AnswerVariants.OrderBy(variant => variant.AnswerOrder); }
        }

        public IEnumerable<SubQuestion> OrderedSubQuestions
        {
            get { return SubQuestions.OrderBy(sq => sq.SubOrder); }
        }
        
        public void copyFrom(SurveyQuestion question)
        {
            QuestionOrder = question.QuestionOrder;
            QuestionName = question.QuestionName;
            QuestionText = question.QuestionText;
            QuestionType = question.QuestionType;
            AnswerOrdering = question.AnswerOrdering;
            MultipleAnswerAllowed = question.MultipleAnswerAllowed;
            MaxAnswers = question.MaxAnswers;
            MinAnswers = question.MinAnswers;
            BoundTagId = question.BoundTagId;
            ConditionString = question.ConditionString;
            ConditionOnTagId = question.ConditionOnTagId;
            ConditionOnTagValue = question.ConditionOnTagValue;
            FilterAnswersTagId = question.FilterAnswersTagId;
            MaxRank = question.MaxRank;
            MinRank = question.MinRank;
        }

        public void SetBoundTagValue(int? boundTagId)
        {
            if (boundTagId == 0)
                CreateBoundTag();
            else
                BoundTagId = boundTagId;
        }

        public void updateBoundTag(Tag tag, IEnumerable<TagValueLabel> tagValueLabels)
        {
            BoundTag = tag;
            BoundTag.TagName = QuestionName;
            BoundTag.ValueLabels = tagValueLabels.ToList();
/*
            foreach (var tagValueLabel in tagValueLabels) {
                tagValueLabel.Tag = BoundTag;
                BoundTag.ValueLabels.Add(tagValueLabel);
            }

            if (BoundTagId.HasValue)
                BoundTag.TagId = BoundTagId.Value;*/
        }

        private void CreateBoundTag()
        {
            BoundTag = Tag.Create(QuestionText, SurveyProjectId, AnswerVariants.Select(answerVariant => new TagValueLabel {
                                                                                                                        Label = answerVariant.InstantText,
                                                                                                                        ValueCode = answerVariant.AnswerCode,
                                                                                                                    }));
        }
    }
}