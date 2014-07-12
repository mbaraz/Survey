using System;
using System.IO;
using SurveyDomain.DataExport;
using SurveyInterfaces;
using SurveyModel;

namespace SurveyDomain
{
    public class ExportService : IExportService
    {
        public void ExportProject(ExportMethod method, SurveyProject project, TextWriter writer, bool onlyCompleted)
        {
            var exporter = GetExporter(method);
            exporter.Writer = writer;
            exporter.Source = GetSource(project, onlyCompleted);
            exporter.Export();
        }

        private static IDataExportSource GetSource(SurveyProject project, bool onlyCompleted)
        {
            return onlyCompleted ? (IDataExportSource)new ExportSourceCompletedOnly(project) : new ExportSourceFullProject(project);
        }

        public IDataExporter GetExporter(ExportMethod method)
        {
            switch (method)
            {
                case ExportMethod.Spss:
                    return new SpssExporter();
                case ExportMethod.PlainText:
                    return new TextOnlyExporter();
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }
    }
}
