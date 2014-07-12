using System.IO;

namespace SurveyInterfaces
{
    public interface IDataExporter
    {
        TextWriter Writer { set; }
        void Export();
        IDataExportSource Source { set; }
    }
}
