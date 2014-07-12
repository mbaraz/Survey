using System;

namespace SurveyCommon
{
    public class QuestionProblemException : Exception
    {
        protected QuestionProblemException(string message) : base(message)
        {
        }
        public virtual int AnswerCode
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }
    }

    public class AnswerProblemException : QuestionProblemException
    {
        protected AnswerProblemException(string message) : base(message)
        {
        }

        public override int AnswerCode { get; set; }
    }

    public class OpenAnswerHasNoOpenPartException : AnswerProblemException
    {
        public OpenAnswerHasNoOpenPartException() : base("Не раскрыт ответ")
        {
        }
    }

    public class QuestionTooManyAnswersException : QuestionProblemException
    {
        public QuestionTooManyAnswersException()
            : base("Выбрано больше, чем требуется, вариантов ответов")
        {
        }
    }

    public class QuestionTooMinAnswersException : QuestionProblemException
    {
        public QuestionTooMinAnswersException()
            : base("Вариантов ответов выбрано недостаточно")
        {
        }
    }

    public class QuestionHasUnexpectedRanksException : QuestionProblemException
    {
        public QuestionHasUnexpectedRanksException() : base("Неожиданный ранг")
        {
        }
    }

    public class QuestionHasNoAnswerException : QuestionProblemException
    {
        public QuestionHasNoAnswerException() : base("Выберите хотя бы один вариант ответа")
        {
        }
    }


    public class UnknownAnswerException : AnswerProblemException
    {
        public UnknownAnswerException() : base("Неизвестный ответ")
        {
        }
    }

    public class RankAnswerNoRankPartException : AnswerProblemException
    {
        public RankAnswerNoRankPartException() :base ("Необходимо оценить пункт")
        {
            
        }
    }

    public class RankAnswerWrongException : AnswerProblemException
    {
        public RankAnswerWrongException()
            : base("Неверное значение оценки")
        {

        }
    }
}
