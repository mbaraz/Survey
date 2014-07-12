using System.IO;
using SurveyModel;

namespace SurveyInterfaces
{
    public interface IExportService
    {
        void ExportProject(ExportMethod method, SurveyProject project, TextWriter writer, bool onlyCompleted);
        IDataExporter GetExporter(ExportMethod method);
    }

    public enum ExportMethod
    {
        Spss,
        PlainText
    }
}
