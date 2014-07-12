using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SurveyDomain.Univer;
using SurveyDomain.Univer.Uploaders;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyDomain
{
    public class UniverService : IUniverService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly ISurveyProjectRepository _surveyProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly IExportService _exportService;

        public UniverService(ICourseRepository courseRepository, IStudentGroupRepository studentGroupRepository, ISurveyProjectRepository surveyProjectRepository, ITagRepository tagRepository, IStudentRepository studentRepository, ISurveyQuestionRepository surveyQuestionRepository, IExportService exportService)
        {
            _courseRepository = courseRepository;
            _studentGroupRepository = studentGroupRepository;
            _surveyProjectRepository = surveyProjectRepository;
            _tagRepository = tagRepository;
            _studentRepository = studentRepository;
            _surveyQuestionRepository = surveyQuestionRepository;
            _exportService = exportService;
        }

        public Facility ReadFacilityConfig(string guid)
        {
            return new DiskFileStore().GetFacilityConfig(guid);
        }

        public IParseResult UploadFacilityFile(HttpPostedFileBase fileBase, Facility facility)
        {
            return new ExcelFileParserBase(facility).ProcessFacilityFile(fileBase);
        }

        public IParseResult AppendFacilityCsv(IEnumerable<KeyValuePair<int, string>> rowItems, Facility facility)
        {
            return new ExcelFileParserBase(facility).AppendCorrectedRows(rowItems);
        }

        public IEnumerable<string> GetSpecsList(int studentGroupId)
        {
            StudentGroup group = _studentGroupRepository.GetById(studentGroupId);
            return new DiskFileStore().GetSpecsList(group);
         }

        public void UploadCourses(string dataFile)
        {
            _courseRepository.AddRange(new CourseFileParser(dataFile).Parse());
        }

        public void UploadStudents(string dataFile)
        {
            _studentRepository.AddRange(new StudentFileParser(dataFile, _studentGroupRepository).Parse().Distinct());
        }

        public void Generate(int surveyProjectId)
        {
            var project = _surveyProjectRepository.GetById(surveyProjectId);
            if (!project.Active || project.Questions.Any())
            {
                throw new Exception();
            }
            var courses = _courseRepository.Get(project.SurveyProjectName);
            var generator = new UniverSurveyGenerator(project, courses.ToArray(), _surveyQuestionRepository);
            _tagRepository.AddRange(generator.GetNewTags());
            generator.Generate();
        }

        public IEnumerable<TagValue> GetTagValues(int project, int studentGroupId, int[] specCodes)
        {
            var tags = _tagRepository.GetTagsForProject(project).ToArray();
            var commonTagId = tags.Single(t => t.TagName == Constants.DispCommon).TagId;
            var optionalTagId = tags.Single(t => t.TagName == Constants.DispOptional).TagId;
            var group = _studentGroupRepository.GetById(studentGroupId);

            var tagValues = _courseRepository.Get(group, specCodes)
                .Select(course => new TagValue
                                      {
                                          TagId = course.IsOptional ? optionalTagId : commonTagId,
                                          Value = course.CourseId
                                      });
            return tagValues;
        }

        public FileStream ExportAll(string facility)
        {
            List<byte[]> results = new List<byte[]>();
            List<string> coursesNames = new List<string>();
            var coursesInfo = GetCoursesWithAnswers(facility);
            foreach (var courseInfo in coursesInfo) {
                Course course = courseInfo.Course;
                coursesNames.Add(course.CourseDispName);
                results.Add(GetSpsByteContent(course));
                results.Add(GetCommentsByteContent(course));
            }
            results.Add(GetFacilityCommentsByteContent(facility));
            return new ExportAllToZipStream(facility).MakeStream(results, coursesNames.ToArray());
        }

        public IEnumerable<CourseResultInfo> GetCoursesWithAnswers(string facility)
        {
            var courses = _courseRepository.Get(facility);
            var project = _surveyProjectRepository.GetByName(facility);
            var projectTags = _tagRepository.GetTagsForProject(project.SurveyProjectId).ToArray();
            return courses.Select(c => new CourseResultInfo
                                           {
                                               Course = c,
                                               AnswerCount =
                                                   new ExportForCourse(project, projectTags, c.CourseId).GetInterviews()
                                                   .Count()
                                           }).OrderByDescending( i => i.AnswerCount);
        }

        public byte[] GetSpsByteContent(Course course)
        {
            StringWriter writer = new StringWriter();
            exportCourse(ExportMethod.Spss, writer, course);
            return Encoding.UTF8.GetBytes(writer.ToString());
        }

        public byte[] GetCommentsByteContent(Course course)
        {
            StringWriter writer = new StringWriter();
            exportCourseComments(ExportMethod.PlainText, writer, course);
            return Encoding.UTF8.GetBytes(writer.ToString());
        }

        public byte[] GetFacilityCommentsByteContent(string facility)
        {
            StringWriter writer = new StringWriter();
            exportFacilityComments(ExportMethod.PlainText, writer, facility);
            return Encoding.UTF8.GetBytes(writer.ToString());
        }

        private void exportCourse(ExportMethod spss, StringWriter writer, Course course)
        {
            var exporter = _exportService.GetExporter(spss);
            exporter.Writer = writer;
            var project = _surveyProjectRepository.GetByName(course.Facility);
            var projectTags = _tagRepository.GetTagsForProject(project.SurveyProjectId).ToArray();
            exporter.Source = new ExportForCourse(project, projectTags, course.CourseId);
            exporter.Export();
        }

        private void exportCourseComments(ExportMethod spss, StringWriter writer, Course course)
        {
            var exporter = _exportService.GetExporter(spss);
            exporter.Writer = writer;
            var project = _surveyProjectRepository.GetByName(course.Facility);
            var projectTags = _tagRepository.GetTagsForProject(project.SurveyProjectId).ToArray();
            exporter.Source = new ExportCourseComments(project, projectTags, course.CourseId);
            exporter.Export();
        }

        private void exportFacilityComments(ExportMethod method, StringWriter writer, string facility)
        {
            var exporter = _exportService.GetExporter(method);
            exporter.Writer = writer;
            exporter.Source = new ExportComments(_surveyProjectRepository.GetByName(facility));
            exporter.Export();

        }

    }
}
