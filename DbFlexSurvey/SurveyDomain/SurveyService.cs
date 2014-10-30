using System;
using System.Collections.Generic;
using System.Linq;
using SurveyCommon;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain
{
    public class SurveyService : ISurveyService
    {
        private readonly IRespondentService _respondentService;
        private readonly ISurveyProjectRepository _surveyProjectRepository;
        private readonly ISurveyQuestionRepository _questionRepository;
        private readonly ITagValueRepository _tagValueRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IInterviewAnswerRepository _interviewAnswerRepository;
        private readonly IUniverService _univerService;
        private int[] _specCodes;

        public SurveyService(ISurveyProjectRepository surveyProjectRepository, ISurveyQuestionRepository questionRepository, ITagValueRepository tagValueRepository, IRespondentService respondentService, IInterviewRepository interviewRepository, IInterviewAnswerRepository interviewAnswerRepository, IUniverService univerService)
        {
            _surveyProjectRepository = surveyProjectRepository;
            _questionRepository = questionRepository;
            _tagValueRepository = tagValueRepository;
            _respondentService = respondentService;
            _interviewRepository = interviewRepository;
            _interviewAnswerRepository = interviewAnswerRepository;
            _univerService = univerService;
        }

        public int? GetNextQuestionOrder(int currentRespondent, int project)
        {
            var interview = _interviewRepository.Get(project,currentRespondent);
            var tagValues = interview.TagValues.Select(tv => tv.Value).ToArray();
            int? order = null;
            if (!interview.Completed) {
                int? lastOrder = interview.GetLastQuestionOrder();
                SurveyQuestion surveyQuestion;
                do {
                    surveyQuestion = _questionRepository.GetQuestionAfter(project, (int) lastOrder, tagValues);
                    lastOrder = surveyQuestion == null ? (int?)null : surveyQuestion.QuestionOrder;
                } while (surveyQuestion != null && (interview.ShouldSkip(surveyQuestion)));
                order = lastOrder;
            }
            var nextQuestionOrder = order;
            if (nextQuestionOrder == null && !interview.Completed) {
                interview.Completed = true;
                _interviewRepository.Edit(interview);
            }
            return nextQuestionOrder;
        }

        public int? GetNextQuestionOrderFx(int currentRespondent, int project, int startOrder)
        {
            var interview = _interviewRepository.Get(project, currentRespondent);
            if (!interview.TestInterview)
                return GetNextQuestionOrder(currentRespondent, project);
// Only for Test interview:
            var tagValues = interview.TagValues.Select(tv => tv.Value).ToArray();
            var tagIds = interview.TagValues.Select(tv => tv.TagId).ToArray();
            int? order = null;
            if (!interview.Completed) {
                int? lastOrder = interview.GetLastQuestionOrder();
                SurveyQuestion surveyQuestion;
                do {
                    surveyQuestion = _questionRepository.GetQuestionAfterFx(project, (int)lastOrder, tagValues, tagIds);
                    lastOrder = surveyQuestion == null ? (int?)null : surveyQuestion.QuestionOrder;
                } while (surveyQuestion != null && (interview.ShouldSkip(surveyQuestion) && interview.shouldSkipForTestFx(surveyQuestion, startOrder)));
                order = lastOrder;
            }
            var nextQuestionOrder = order;
            if (nextQuestionOrder == null && !interview.Completed) {
                interview.Completed = true;
                _interviewRepository.Edit(interview);
            }
            return nextQuestionOrder;
        }
        
        public bool AcceptAnswer(int respondentId, int questionId, IInterviewAnswer interviewAnswer)
        {
            var question = _questionRepository.GetById(questionId);
            var interview = _surveyProjectRepository.GetById(question.SurveyProjectId).Interviews.Single(interv => interv.RespondentId == respondentId);
            question.ValidateAnswer(interviewAnswer, interview);
            SaveAnswer(questionId, interviewAnswer, question, interview);
            if (question.BoundTagId.HasValue)
                if (question.HasSingleAnswer)
                    SaveTagValue(interview, question.BoundTag, interviewAnswer.Rank.Single(rank => rank.Key == 1).Value);
                else
                    SaveTagValue(interview, question.BoundTag, interviewAnswer.Answers.Select(answer => question.AnswerVariants.Single(variant => variant.AnswerCode == answer)));

            foreach (SubQuestion subQuestion in question.SubQuestions)
                if (subQuestion.BoundTagId.HasValue)
                    if (question.HasNoAnswer)
                        SaveTagValue(interview, subQuestion.BoundTag, interviewAnswer.Rank.SingleOrDefault(rank => rank.Key == subQuestion.SubOrder).Value);
                    else
                        SaveTagValue(interview, subQuestion.BoundTag, interviewAnswer.Rank.Where(rank => rank.Key == subQuestion.SubOrder).Select(rank => question.AnswerVariants.Single(variant => (variant.AnswerCode == rank.Value))));
            
            return true;
        }

        private void SaveAnswer(int questionId, IInterviewAnswer interviewAnswer, SurveyQuestion question, Interview interview)
        {
            var prevAnswers = interview.Answers.Where(a => a.SurveyQuestionId == question.SurveyQuestionId).ToArray();
            foreach (var prevAnswer in prevAnswers)
                _interviewAnswerRepository.Delete(prevAnswer);

            _interviewAnswerRepository.Add(new InterviewAnswer {
                                                   Answer = interviewAnswer,
                                                   Interview = interview,
                                                   InterviewId = interview.InterviewId,
                                                   SurveyQuestion = question,
                                                   SurveyQuestionId = questionId
                                               });
        }

        private void SaveTagValue(Interview interview, Tag boundTag, IEnumerable<AnswerVariant> answerVariants)
        {
            var tagValues = interview.TagValues.Where(tv => tv.TagId == boundTag.TagId).ToArray();
            _tagValueRepository.DeleteRange(tagValues);
            foreach (var tagValue in answerVariants.Select(answerVariant => answerVariant.TagValue).Where(tagValue => tagValue != null).Distinct())
                _tagValueRepository.Add(new TagValue {
                                                Interview = interview,
                                                InterviewId = interview.InterviewId,
                                                Tag = boundTag,
                                                TagId = boundTag.TagId,
                                                Value = tagValue
                                            });
        }

        private void SaveTagValue(Interview interview, Tag boundTag, int value)
        {
            var tagValues = interview.TagValues.Where(tv => tv.TagId == boundTag.TagId).ToArray();
            _tagValueRepository.DeleteRange(tagValues);
            _tagValueRepository.Add(new TagValue {
                Interview = interview,
                InterviewId = interview.InterviewId,
                Tag = boundTag,
                TagId = boundTag.TagId,
                Value = value
            });
        }

        public Interview StartInterview(Respondent currentRespondent, int project, int[] specCodes = null)
        {
            _specCodes = specCodes;
            return StartInterview(currentRespondent, project, false);
        }

        private Interview StartInterview(Respondent currentRespondent, int project, bool testInterview)
        {
//TODO allow start survey only if got invite
            var interview = new Interview {
                                    RespondentId = currentRespondent.RespondentId,
                                    SurveyProjectId = project,
                                    Answers = new List<InterviewAnswer>(),
                                    TestInterview = testInterview,
                                };

            if (currentRespondent.StudentGroupId != null) {
                var tagValues = _univerService.GetTagValues(project, (int)currentRespondent.StudentGroupId, _specCodes).ToArray();
                foreach (var tv in tagValues)
                    tv.Interview = interview;

                _tagValueRepository.AddRange(tagValues);
            }
            _interviewRepository.Add(interview);
            _respondentService.DeleteInvitationIfNeeded(currentRespondent.RespondentId, project);
            return interview;
        }

        public Interview StartTestInterview(Respondent currentRespondent, int project, int[] specCodes)
        {
            _specCodes = specCodes;
            DeleteInterviewIfPresent(currentRespondent.RespondentId, project);
            return StartInterview(currentRespondent, project, true);
        }

        private void DeleteInterviewIfPresent(int currentRespondent, int project)
        {
            var interview = _interviewRepository.Get(project,currentRespondent);
            if (interview != null)
                _interviewRepository.Delete(interview);
        }

        public AnswerVariant CreateAnswerVariant(int surveyQuestionId, string text, bool isOpenAnswer)
        {
            var question = _questionRepository.GetById(surveyQuestionId);
            var code = question.AnswerVariants.MaxOrDefault(variant => variant.AnswerOrder) + 1;
            var answerVariant = new AnswerVariant {
                                        SurveyQuestion = question,
                                        SurveyQuestionId = question.SurveyQuestionId,
                                        AnswerCode =code,
                                        AnswerOrder = code,
                                        AnswerText = text,
                                        IsOpenAnswer = isOpenAnswer,
                                        TagValue =  code
                                    };
            question.AnswerVariants.Add(answerVariant);
            _questionRepository.Edit(question);
            return answerVariant;
        }

        public SurveyQuestion EditQuestion(int surveyQuestionId, string questionText, int? conditionOnTagId, int? conditionOnTagValue, bool multipleAnswerAllowed, int? boundTagId, int? maxAnswers, int? maxRank, int? filterAnswersTagId, int? minAnswers)
        {
            var question = _questionRepository.GetById(surveyQuestionId);
            question.QuestionText = questionText;
            question.MultipleAnswerAllowed = multipleAnswerAllowed;
            question.ConditionOnTagId = conditionOnTagId;
            question.ConditionOnTagValue = conditionOnTagValue;
            question.MaxAnswers = maxAnswers;
            question.MinAnswers = minAnswers;
            question.SetBoundTagValue(boundTagId);
            question.MaxRank = maxRank;
            question.FilterAnswersTagId = filterAnswersTagId;
            _questionRepository.Edit(question);
            return question;
        }

        public void DeleteQuestion(SurveyQuestion surveyQuestion)
        {
            foreach (var answer in surveyQuestion.InterviewAnswers.ToArray())
                _interviewAnswerRepository.Delete(answer);

            _questionRepository.Delete(surveyQuestion);
        }

        public SurveyProject AddQuestion(int surveyProjectId, string questionText, IEnumerable<string> answerVars, int questionOrder, int? conditionOnTagId, int? conditionOnTagValue, bool multipleAnswerAllowed, int? boundTagId, int? maxAnswers, int? maxRank, int? filterAnswersTagId, int? minAnswers)
        {
            var project = _surveyProjectRepository.GetById(surveyProjectId);
            var questionName = string.Format("Q{0}", Math.Max(project.Questions.Count, project.Questions.MaxOrDefault(q => q.QuestionOrder)) + 1);
            var question = new SurveyQuestion {
                                   QuestionName = questionName,
                                   QuestionText = questionText,
                                   AnswerVariants = AnswerVariant.Create(answerVars),
                                   QuestionOrder = questionOrder,
                                   ConditionOnTagId = conditionOnTagId,
                                   ConditionOnTagValue = conditionOnTagValue,
                                   MultipleAnswerAllowed = multipleAnswerAllowed,
                                   SurveyProjectId = surveyProjectId,
                                   MaxAnswers = maxAnswers,
                                   MinAnswers = minAnswers,
                                   MaxRank = maxRank,
                                   FilterAnswersTagId = filterAnswersTagId
                               };
            question.InitAnswers();
            question.SetBoundTagValue(boundTagId);
            _questionRepository.Add(question);
            project.InsertQuestion(question);
            _surveyProjectRepository.Edit(project);
            return project;
        }

        public SurveyProject AddQuestion(int surveyProjectId, SurveyQuestion question, IEnumerable<string> answerVars, int? conditionOnTagId, int? conditionOnTagValue, int? boundTagId, int? filterAnswersTagId)
        {
            var project = _surveyProjectRepository.GetById(surveyProjectId);
            question.InitAnswers();
            question.SetBoundTagValue(boundTagId);
            _questionRepository.Add(question);
            project.InsertQuestion(question);
            _surveyProjectRepository.Edit(project);
            return project;
        }
    }
}