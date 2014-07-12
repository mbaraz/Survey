using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SurveyInterfaces;
using SurveyModel;

namespace SurveyDomain.DataExport
{
    internal abstract class ExporterBase : IDataExporter
    {
        public TextWriter Writer { protected get; set; }
        public IDataExportSource Source { set; private get; }
        protected IList<IVariable> Variables { get; private set; }

        public virtual void Export()
        {
            Variables = GetVariables().ToArray();
            Check();

            DoExport();
        }

        protected virtual IEnumerable<IVariable> GetVariables()
        {
            return Source.GetVariables();
        }

        private void Check()
        {
            var duplicates = GetDuplicates();
            if (duplicates.Any())
            {
                throw new Exception("Duplicate: " + string.Join(", ", duplicates));
            }
        }

        protected abstract void DoExport();

        private string[] GetDuplicates()
        {
            return Variables.Select(v => v.Name).GroupBy(v => v).SelectMany(grp => grp.Skip(1)).Distinct().ToArray();
        }

        protected IEnumerable<Interview> GetInterviews()
        {
            return Source.GetInterviews();
        }

        protected IEnumerable<string> GetVariablesForInterview(Interview interview)
        {
            return Variables.Select(v => v.GetValueForInterview(interview) ?? "");
        }

        protected void WriteLines(IEnumerable<string> interviewStrings)
        {
            foreach (var interviewString in interviewStrings)
            {
                Writer.WriteLine(interviewString);
            }
        }
    }
}