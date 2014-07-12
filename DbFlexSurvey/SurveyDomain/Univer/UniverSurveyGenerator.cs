using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyDomain.Univer
{
    class UniverSurveyGenerator
    {
        private readonly SurveyProject _project;
        private readonly List<SurveyQuestion> _questions = new List<SurveyQuestion>();
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly Tag _commonTag;
        private readonly Tag _optionalTag;
        private readonly IList<Course> _optionalCourses;
        private readonly IList<Course> _commonCourses;

        public UniverSurveyGenerator(SurveyProject project, Course[] courses, ISurveyQuestionRepository surveyQuestionRepository)
        {
            _project = project;
            _surveyQuestionRepository = surveyQuestionRepository;
            _optionalCourses = courses.Where(c => c.IsOptional && c.Facility == project.SurveyProjectName).ToList();
            _commonCourses = courses.Where(c => !c.IsOptional && c.Facility == project.SurveyProjectName).ToList();
            _commonTag = CreateTag(Constants.DispCommon, _commonCourses);
            _optionalTag = CreateTag(Constants.DispOptional, _optionalCourses);
        }

        public IEnumerable<Tag> GetNewTags()
        {
            yield return _commonTag;
            yield return _optionalTag;
        }

        public void Generate()
        {  
            foreach (var courseQuestionsGenerator in _commonCourses.Select(course => new CourseQuestionsGenerator(course, _project, _commonTag, _questions)))
            {
                courseQuestionsGenerator.Generate();
            }

            
            new OptionalSelectorGenerator(_project, _optionalCourses, _optionalTag, _questions).Generate();
            foreach (var courseQuestionsGenerator in _optionalCourses.Select(course => new CourseQuestionsGenerator(course, _project, _optionalTag, _questions)))
            {
                courseQuestionsGenerator.Generate();
            }
            new AskCommentsGenerator(_project, _questions).Generate();

            _surveyQuestionRepository.AddRange(_questions);
        }

        private Tag CreateTag(string tagName, IEnumerable<Course> courses)
        {
            return Tag.Create(tagName, _project.SurveyProjectId,
                              courses.Select(c => new TagValueLabel
                                                      {
                                                          Label = c.CourseDispName,
                                                          ValueCode = c.CourseId
                                                      }));
        }

        private sealed class AskCommentsGenerator : QuestionsGenerator
        {
            public AskCommentsGenerator(SurveyProject project, ICollection<SurveyQuestion> questions)
                : base(project, questions)
            {
                    
            }

            public void Generate()
            {
                AddQuestion("Comments", "Ваши предложения по совершенствованию качества преподавания на Вашем факультете в целом", "__", "Нет предложений и комментариев");
            }
        }

        private sealed class OptionalSelectorGenerator : QuestionsGenerator
        {
            private readonly IEnumerable<Course> _courses;
            private readonly Tag _filterTag;

            public OptionalSelectorGenerator(SurveyProject project, IEnumerable<Course> courses, Tag filterTag, ICollection<SurveyQuestion> questions) : base(project, questions)
            {
                _courses = courses;
                _filterTag = filterTag;
            }

            public void Generate()
            {
                var question = new SurveyQuestion
                {
                    QuestionOrder = Questions.Count + 1,
                    QuestionName = FormatQuestionName("OptionalSelector"),
                    QuestionText = FormatQuestionText("Выберите из приведенного ниже перечня дисциплин по выбору те, которые Вы фактически посещали в течение этого семестра"),
                    AnswerVariants = _courses.Select(c => new AnswerVariant
                                                              {
                                                                  AnswerCode = c.CourseId,
                                                                  AnswerText = c.CourseDispName,
                                                                  AnswerOrder = c.CourseId,
                                                                  TagValue = c.CourseId
                                                              }).ToArray(),
                    MultipleAnswerAllowed = true,
                    ConditionOnTag = GetConditionOnTag(),
                    ConditionOnTagValue = GetConditionOnTagValue(),
                    FilterAnswersTag = _filterTag,
                    BoundTag = _filterTag,
                    SurveyProjectId = Project.SurveyProjectId
                };
/*
                question.AnswerVariants.Add(new AnswerVariant { AnswerCode = 0,
                                                                AnswerText = "Не посещал(а) дисциплины по выбору в этом семестре",
                                                                AnswerOrder = question.AnswerVariants.Count,
                                                                TagValue = 0
                                                               });
*/
                foreach (var av in question.AnswerVariants)
                {
                    av.SurveyQuestion = question;
                }
                Questions.Add(question);
                
            }
        }

        private abstract class QuestionsGenerator
        {
            protected readonly SurveyProject Project;
            protected readonly ICollection<SurveyQuestion> Questions;

            protected QuestionsGenerator(SurveyProject project, ICollection<SurveyQuestion> questions)
            {
                Project = project;
                Questions = questions;
            }

            protected void AddQuestion(string nameSuffix, string questionText, params string[] answerVars)
            {
                var question = new SurveyQuestion
                                   {
                                       QuestionOrder = Questions.Count + 1,
                                       QuestionName = FormatQuestionName(nameSuffix),
                                       QuestionText = FormatQuestionText(questionText),
                                       AnswerVariants = AnswerVariant.Create(answerVars),
                                       ConditionOnTag = GetConditionOnTag(),
                                       ConditionOnTagValue = GetConditionOnTagValue(),
                                       SurveyProjectId = Project.SurveyProjectId
                                   };
                question.InitAnswers();
                Questions.Add(question);
            }

            protected virtual string FormatQuestionText(string questionText)
            {
                return questionText;
            }

            protected virtual string FormatQuestionName(string nameSuffix)
            {
                return nameSuffix;
            }

            protected virtual int? GetConditionOnTagValue()
            {
                return null;
            }
            protected virtual Tag GetConditionOnTag()
            {
                return null;
            }
        }

        private sealed class CourseQuestionsGenerator : QuestionsGenerator
        {
            private readonly Course _course;
            private readonly Tag _conditionTag;

            public CourseQuestionsGenerator(Course course, SurveyProject project, Tag conditionTag, ICollection<SurveyQuestion> questions) : base(project, questions)
            {
                _course = course;
                _conditionTag = conditionTag;
            }

            

            protected override int? GetConditionOnTagValue()
            {
                return _course.CourseId;
            }

            protected override Tag GetConditionOnTag()
            {
                return _conditionTag;
            }

            public void Generate()
            {
                AddQuestion("Attend", "Как много занятий по данной дисциплине Вы посетили?",
                    "Менее трети занятий", "Более трети, но менее половины занятий", "Более половины занятий, но не все", "Все занятия по дисциплине", "Эту дисциплину вел другой преподаватель", "Этой дисциплины не было");
                AddScaleQuestion("Target",
                                 "В начале семестра преподаватель четко сформулировал цели и задачи изучения дисциплины, разъяснил требования к ее освоению и принципы оценки знаний?",
                                 "Ничего не разъяснил", "Что-то разъяснил, что-то нет", "Разъяснил все", "Не был на первом занятии");
                AddScaleQuestion("Structure",
                                 "Занятия были четко структурированы (введение, последовательное освещение каждого вопроса, выводы)",
                                 "Не структурированы", "Только отдельные темы", "Все занятия четко структурированы");
                AddScaleQuestion("Tech",
                                 "На занятиях использовались технические средства обучения (мультимедийное и другое оборудование), облегчающие восприятие и усвоение материала",
                                 "Не использовались", "Использовались на некоторых занятиях", "Использовались на всех занятиях", "На этих занятиях технические средства обучения не нужны");
                AddScaleQuestion("Polite",
                                 "Преподаватель уважительно относится к студентам, в разговоре демонстрирует участие, внимательно выслушивает их вопросы, просьбы, не допускает грубости",
                                 "Не уважительно", "Иногда уважительно, а иногда нет", "Всегда уважительно");
                if (_course.IsPractice)
                    AddScaleQuestion("Practice",
                                 "На занятиях использовались практические примеры, анализ конкретных ситуаций, индивидуальные и групповые дискуссии, задания и т.п.",
                                 "Не было ни разу", "Были примерно на половине занятий", "Были на всех занятиях", "На этих занятиях практически примеры и т.п. не нужны");
                AddScaleQuestion("Utility",
                                 "Знания, умения и навыки, полученные на занятиях, полезны для моей будущей профессиональной деятельности",
                                 "Совершенно бесполезны", "В чем-то полезны, в чем-то нет", "Крайне полезны");
                AddScaleQuestion("Total",
                                  "В какой степени Вы удовлетворены дисциплиной в целом",
                                  "Не удовлетворен(а)", "В чем-то удовлетворен(а), в чем-то нет", "Удовлетворен(а) полностью");
                AddQuestion("Open", "Ваши предложения по преподаванию дисциплины", "__", "Нет предложений");
            }

            private void AddScaleQuestion(string name, string text, string min, string mid, string max, string not = null)
            {
                var answerVars = 
                    
                    new List<string>
                        {
                                         "1 (" + min + ")", "2", "3", "4 (" + mid + ")", "5",
                                         "6", "7 (" + max + ")"
                                     };
                if (not != null)
                {
                    answerVars.Add(not);
                }
                AddQuestion(name, text + "<br> (Выставите оценку от 1 до 7)", answerVars.ToArray());
            }

            protected override string FormatQuestionText(string questionText)
            {
                return string.Format("{1}<br> {0}", questionText, _course.CourseDispName);
            }

            protected override string FormatQuestionName(string nameSuffix)
            {
                return string.Format("Disp_{0}_{1}", _course.CourseId, nameSuffix);
            }
        }
    }
}
