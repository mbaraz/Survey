using System.Collections.Generic;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface ISurveyService
    {
        int? GetNextQuestionOrder(int currentRespondent, int project);
        int? GetNextQuestionOrderFx(int currentRespondent, int project, int startOrder);
        bool AcceptAnswer(int respondentId, int questionId, IInterviewAnswer interviewAnswer);
        Interview StartInterview(Respondent currentRespondent, int project, int[] specCodes = null);
        Interview StartTestInterview(Respondent currentRespondent, int project, int[] specCodes = null);
        AnswerVariant CreateAnswerVariant(int surveyQuestionId, string text, bool isOpenAnswer);
        SurveyProject AddQuestion(int surveyProjectId, string questionText, IEnumerable<string> answerVars, int questionOrder, int? conditionOnTagId, int? conditionOnTagValue, bool multipleAnswerAllowed, int? boundTagId, int? maxAnswers, int? maxRank, int? filterAnswersTagId, int? minAnswers);
        SurveyQuestion EditQuestion(int surveyQuestionId, string questionText, int? conditionOnTagId, int? conditionOnTagValue, bool multipleAnswerAllowed, int? boundTagId, int? maxAnswers, int? maxRank, int? filterAnswersTagId, int? minAnswers);
        void DeleteQuestion(SurveyQuestion surveyQuestion);
    }
}
