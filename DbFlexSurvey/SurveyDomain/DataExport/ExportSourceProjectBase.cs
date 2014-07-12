using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SurveyInterfaces;
using SurveyModel;

namespace SurveyDomain.DataExport
{
    internal abstract class ExportSourceProjectBase : IDataExportSource
    {
        private readonly SurveyProject _project;

        protected ExportSourceProjectBase(SurveyProject project)
        {
            _project = project;
        }

        protected SurveyProject Project
        {
            get { return _project; }
        }

        public abstract IEnumerable<Interview> GetInterviews();

        public IEnumerable<IVariable> GetVariables()
        {
            yield return new IdVariable();
            foreach (var variable in GetQuestions.SelectMany(GetVariablesForQuestion))
            {
                yield return variable;
            }
        }

        protected virtual IEnumerable<SurveyQuestion> GetQuestions
        {
            get { return Project.Questions; }
        }

        private class IdVariable : Variable
        {
            public override string Name
            {
                get { return "ID"; }
            }

            public override string Label
            {
                get { return "ID  интервью"; }
            }

            public override bool HasValueLabels
            {
                get { return false; }
            }

            public override IEnumerable<KeyValuePair<int, string>> ValueLabels
            {
                get { throw new NotImplementedException(); }
            }

            public override string GetValueForInterview(Interview interview)
            {
                return interview.InterviewId.ToString(CultureInfo.InvariantCulture);
            }
        }

        private abstract class Variable : IVariable
        {
            public abstract string Name { get; }
            public abstract string Label { get; }

            public virtual bool IsString
            {
                get { return false; }
            }

            public abstract bool HasValueLabels { get; }
            public abstract IEnumerable<KeyValuePair<int, string>> ValueLabels { get; }
            public abstract string GetValueForInterview(Interview interview);
        }

        private static IEnumerable<Variable> GetVariablesForQuestion(SurveyQuestion surveyQuestion)
        {
            if (surveyQuestion.MultipleAnswerAllowed) {
                if (surveyQuestion.IsGridQuestion) {
                    var cnt = 1;
                    foreach (var subitem in surveyQuestion.SubitemsStrings)
                        yield return new GridQuestionVariable(surveyQuestion, subitem, cnt++);
                } else
                    foreach (var answer in surveyQuestion.OrderedAnswerVariants) {
                        if (surveyQuestion.IsRankQuestion)
                            yield return new RankVariable(answer);
                        else
                            yield return new AnswerVariable(answer);
                        if (answer.IsOpenAnswer)
                            yield return new OpenAnswerVariable(answer);
                    }
            } else {
                yield return new QuestionVariable(surveyQuestion);
                foreach (var answer in surveyQuestion.OrderedAnswerVariants.Where(variant => variant.IsOpenAnswer))
                    yield return new OpenAnswerVariable(answer);
            }
        }

        private class QuestionVariable : Variable
        {
            protected SurveyQuestion SurveyQuestion { get; private set; }

            public QuestionVariable(SurveyQuestion surveyQuestion)
            {
                SurveyQuestion = surveyQuestion;
            }

            public override string Name { get { return SurveyQuestion.QuestionName; } }

            public override string Label { get { return SurveyQuestion.QuestionText; } }

            public override bool HasValueLabels { get { return true; } }

            public override IEnumerable<KeyValuePair<int, string>> ValueLabels
            {
                get
                {
                    return SurveyQuestion.OrderedAnswerVariants.Select(av => new KeyValuePair<int, string>(av.AnswerCode, av.InstantText));
                }
            }

            public override string GetValueForInterview(Interview interview)
            {
                var interviewAnswer = interview.GetInterviewAnswer(SurveyQuestion);
                if (interviewAnswer == null)
                    return null;

                return interviewAnswer.Answers.Single().ToString(CultureInfo.InvariantCulture);
            }
        }

// Flex Grid
        private class GridQuestionVariable : QuestionVariable
        {
            public GridQuestionVariable(SurveyQuestion surveyQuestion, string subitem, int index) : base(surveyQuestion)
            {
                Subitem = subitem;
                Index = index;
            }

            private string Subitem { get; set; }
            private int Index { get; set; }

            public override string Label { get { return Subitem; } }

            public override string Name { get { return SurveyQuestion.QuestionName + "_" + Index; } }

            public override string GetValueForInterview(Interview interview)
            {
                var interviewAnswer = interview.GetInterviewAnswer(SurveyQuestion);
                if (interviewAnswer == null || !interviewAnswer.Rank.ContainsKey(Index))
                    return null;

                return interviewAnswer.Rank[Index].ToString(CultureInfo.InvariantCulture);
            }
        }
// End Flex

        private abstract class AnswerVariableBase : Variable
        {
            protected AnswerVariableBase(AnswerVariant answer)
            {
                Answer = answer;
            }

            protected AnswerVariant Answer { get; private set; }

            protected InterviewAnswer GetInterviewAnswer(Interview interview)
            {
                return interview.GetInterviewAnswer(Answer.SurveyQuestion);
            }

            public override string Name
            {
                get
                {
//                    AnswerVariant answer = Answer;
                    return Answer.SurveyQuestion.QuestionName + "_" + Answer.AnswerCode;
                }
            }
        }

        private class AnswerVariable : AnswerVariableBase
        {
            public AnswerVariable(AnswerVariant answer)
                : base(answer)
            {
            }

            public override string Label { get { return Answer.InstantText; } }

            public override bool HasValueLabels { get { return true; } }

            public override IEnumerable<KeyValuePair<int, string>> ValueLabels
            {
                get
                {
                    yield return new KeyValuePair<int, string>(0, "Нет");
                    yield return new KeyValuePair<int, string>(1, "Да");
                }
            }

            public override string GetValueForInterview(Interview interview)
            {
                var interviewAnswer = GetInterviewAnswer(interview);
                if (interviewAnswer == null)
                    return null;

                return interviewAnswer.Answers.Contains(Answer.AnswerCode) ? "1" : "0";
            }
        }

        private class RankVariable : AnswerVariableBase
        {
            public RankVariable(AnswerVariant answer)
                : base(answer)
            {
            }

            public override string Label { get { return Answer.InstantText; } }

            public override bool HasValueLabels { get { return true; } }

            public override IEnumerable<KeyValuePair<int, string>> ValueLabels { get { yield return new KeyValuePair<int, string>(-1, "Затрудняюсь ответить"); } }

            public override string GetValueForInterview(Interview interview)
            {
                var interviewAnswer = GetInterviewAnswer(interview);
                if (interviewAnswer == null || !interviewAnswer.Rank.ContainsKey(Answer.AnswerCode))
                    return null;

                return interviewAnswer.Rank[Answer.AnswerCode].ToString(CultureInfo.InvariantCulture);
            }
        }

        private class OpenAnswerVariable : AnswerVariableBase
        {
            public OpenAnswerVariable(AnswerVariant answer)
                : base(answer)
            {
            }

            public override string Name { get { return base.Name + "_Open"; } }

            public override string Label { get { return Answer.InstantText + " ОТКРЫТЫЙ"; } }

            public override bool HasValueLabels { get { return false; } }

            public override IEnumerable<KeyValuePair<int, string>> ValueLabels
            {
                get { throw new NotImplementedException(); }
            }

            public override string GetValueForInterview(Interview interview)
            {
                var interviewAnswer = GetInterviewAnswer(interview);
                return interviewAnswer != null && interviewAnswer.OpenAnswers.ContainsKey(Answer.AnswerCode) ? interviewAnswer.OpenAnswers[Answer.AnswerCode] : null;
            }

            public override bool IsString { get { return true; } }
        }
    }
}