using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using SurveyCommon;

namespace SurveyModel
{
    public class InterviewAnswer : IInterviewAnswer
    {
        private AnswerObj _answer;
        public int InterviewAnswerId { get; set; }

        public int InterviewId { get; set; }
        public virtual Interview Interview { get; set; }

        public int SurveyQuestionId { get; set; }
        public virtual SurveyQuestion SurveyQuestion { get; set; }

        public string AnswerSerialized { get; set; }

        public ICollection<int> Answers
        {
            get { return Answer.Answers; }
        }

        public IDictionary<int, string> OpenAnswers
        {
            get { return Answer.OpenAnswers; }
        }

        public Dictionary<int, int> Rank
        {
            get { return Answer.Rank; }
        }

        public IEnumerable<AnswerPart> Parts
        {
            get {
// Flex Grid
                if (!SurveyQuestion.IsGridQuestion)
                    return Answers.Select(answer => new AnswerPart(SurveyQuestion.AnswerVariants.Single(av => av.AnswerCode == answer), OpenAnswers.ContainsKey(answer) ? OpenAnswers[answer]: null, Rank.ContainsKey(answer) ? Rank[answer] : (int?) null));

                var result = new AnswerPart[Rank.Count];
                for (var i = 0; i < Rank.Count; i++)
                    result[i] = new AnswerPart(null, SurveyQuestion.SubitemsStrings.ElementAt(Rank.Keys.ElementAt(i) - 1), Rank.Values.ElementAt(i));

                return result;
            }
        }
// End Flex
        public class AnswerPart
        {
            public AnswerVariant Answer { get; private set; }
            public string Open { get; private set; }
            public int? Rank { get; private set; }

            public AnswerPart(AnswerVariant single, string open, int? rank)
            {
                Rank = rank;
                Answer = single;
                Open = open;
            }
        }

        [NotMapped]
        public IInterviewAnswer Answer
        {
            get
            {
                if (_answer != null)
                {
                    return _answer;
                }
                return _answer = AnswerObj.CreateAnswerFromString(AnswerSerialized);
            }
            set
            {
                _answer = new AnswerObj(value);
                AnswerSerialized = _answer.ToString();
            }
        }

        [DataContract]
        private class AnswerObj : IInterviewAnswer
        {
            [DataMember(Name="Answers")]
            internal int[] AnswersImpl;

            [DataMember(Name="OpenAnswers", IsRequired = false)]
            internal Dictionary<int, string> OpenAnswersImpl;

            [DataMember(Name="Rank", IsRequired = false)]
            internal Dictionary<int, int> RankImpl;

            public AnswerObj(IInterviewAnswer value)
            {
                AnswersImpl = value.Answers.ToArray();
                OpenAnswersImpl = value.OpenAnswers.ToDictionary();
                RankImpl = value.Rank.ToDictionary();
            }

// ReSharper disable UnusedMember.Local
            public AnswerObj()
// ReSharper restore UnusedMember.Local
            {
                RankImpl = new Dictionary<int, int>();
                OpenAnswersImpl = new Dictionary<int, string>();
                AnswersImpl = new int[0];
            }

            public ICollection<int> Answers
            {
                get { return AnswersImpl; }
            }

            public IDictionary<int, string> OpenAnswers
            {
                get { return OpenAnswersImpl; }
            }

            public Dictionary<int, int> Rank
            {
                get { return RankImpl; }
            }

            public override string ToString()
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof (AnswerObj));
                    serializer.WriteObject(ms, this);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }

            internal static AnswerObj CreateAnswerFromString(string answerSerialized)
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(answerSerialized)))
                {
                    var serializer = new DataContractJsonSerializer(typeof (AnswerObj));
                    var obj = (AnswerObj) serializer.ReadObject(ms);
                    if (obj.RankImpl == null)
                    {
                        obj.RankImpl = new Dictionary<int, int>();
                    }
                    return obj;
                }
            }
        }


    }
}
