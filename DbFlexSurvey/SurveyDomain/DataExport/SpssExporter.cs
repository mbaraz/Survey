using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurveyModel;

namespace SurveyDomain.DataExport
{
    internal class SpssExporter : ExporterBase
    {
        private const int SpssMaxStringLength = 250;

        protected override void DoExport()
        {
            Writer.WriteLine("SET UNICODE=YES.");

            WriteDataList();

            WriteVariableLabels();

            WriteValueLabels();
        }

        private void WriteVariableLabels()
        {
            Writer.WriteLine("VARIABLE LABELS");
            foreach (var variable in Variables)
            {
                Writer.WriteLine("{0} '{1}'", variable.Name, variable.Label);
            }
            Writer.WriteLine(".");
        }

        private void WriteValueLabels()
        {
            Writer.WriteLine("VALUE LABELS");
            foreach (var variable in Variables.Where(variable => variable.HasValueLabels))
            {
                Writer.WriteLine(variable.Name);
                foreach (var valueLabel in variable.ValueLabels)
                {
                    Writer.WriteLine("{0} \"{1}\"", valueLabel.Key, valueLabel.Value);
                }
                Writer.WriteLine("/");
            }
            Writer.WriteLine(".");
        }

        private void WriteDataList()
        {
            Writer.WriteLine("DATA LIST FREE (\";\") /\n{0}.",
                             string.Join("\n",
                                         Variables.Select(
                                             variable =>
                                             string.Format("{0} ({1})", variable.Name,
                                                           variable.IsString ? "A255" : "F1.0"))));
            Writer.WriteLine("BEGIN DATA");
            WriteLines(GetInterviews().SelectMany(GetStringForInterview));
            Writer.WriteLine("END DATA.");
        }

        private IEnumerable<string> GetStringForInterview(Interview interview)
        {
            var buffer = new StringBuilder();
            foreach (var variable in GetVariablesForInterview(interview).Select(EnquoteVariable))
            {
                if (buffer.Length + variable.Length > SpssMaxStringLength)
                {
                    yield return buffer.ToString();
                    buffer = new StringBuilder();
                }
                buffer.Append(variable);
                buffer.Append(";");

            }
            yield return buffer.ToString();
        }

        private static string EnquoteVariable(string s)
        {
            s = s.Replace('\'', '"');
            s = s.Replace(';', ',');
            return HasSpecialSymbols(s) ? string.Format("'{0}'", s) : s;
        }

        private static bool HasSpecialSymbols(string s)
        {
            return s.Contains(",") || s.Contains(" ") || s.Contains(";");
        }
    }
}