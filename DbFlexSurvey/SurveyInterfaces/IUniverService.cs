using System.Collections.Generic;
using System.IO;
using System.Web;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyInterfaces
{
    public interface IUniverService
    {
        void UploadCourses(string dataFile);
        Facility ReadFacilityConfig(string guid);
        IParseResult UploadFacilityFile(HttpPostedFileBase fileBase, Facility facility);
        IParseResult AppendFacilityCsv(IEnumerable<KeyValuePair<int, string>> rowItems, Facility facility);
        IEnumerable<string> GetSpecsList(int studentGroupId);
        void Generate(int surveyProjectId);
        void UploadStudents(string dataFile);
        IEnumerable<TagValue> GetTagValues(int project, int studentGroupId, int[] specCodes);
        FileStream ExportAll(string facility);
        IEnumerable<CourseResultInfo> GetCoursesWithAnswers(string facility);
        byte[] GetSpsByteContent(Course course);
        byte[] GetCommentsByteContent(Course course);
        byte[] GetFacilityCommentsByteContent(string facility);
    }

    public class CourseResultInfo
    {
        public Course Course { get; set; }
        public int AnswerCount { get; set; }
    }
}
