using System;
using System.Collections.Generic;
using System.Linq;
using SurveyInterfaces;
using SurveyModel.Univer;

namespace SurveyDomain.Univer
{
    public abstract class CsvFileParserBase<T> : ICsvFileParser<T> where T: class
    {
        private readonly string _dataFile;

        protected CsvFileParserBase(string dataFile)
        {
            _dataFile = dataFile;
        }

        protected static bool ParseBoolField(string val, IDictionary<string, bool> dict, string fieldName = "Bool")
        {
            val = val.Trim().ToLower();
            if (dict.ContainsKey(val))
            {
                return dict[val];
            }
            throw new Exception(string.Format("{0} :«{1}»", fieldName, val));
        }

        private static IEnumerable<string> GetLines(string dataFile)
        {
            return dataFile.Split('\n').Select(l => l.Trim()).Where(l => l != "");  // dataFile.Split('\n').Skip(1).Select(l => l.Trim()).Where(l => l != "");
        }

        protected abstract IEnumerable<T> ParseLine(IList<string> fields);

        public  IEnumerable<T> Parse()
        {
            var dataFile = _dataFile;
            var result = new List<T>();
            foreach (var line in GetLines(dataFile))
            {
                try
                {
                    result.AddRange(ParseLine(line.Split(';')));
                }
                catch (Exception exc)
                {
                    throw new Exception(String.Format("Failed to parse {1} \n: '{0}'", line, exc.Message), exc);
                }
            }
            return result;
        }

        protected static string GetFacilityName(string field)
        {
            return field.Trim().Replace("международных отношений", "международные отношения").Replace("экономики", "экономический");    // Facility.GetTrimedName(field); ЗАМЕНИТЬ!
        }

        protected static int GetYearNum(string field)
        {
            return Convert.ToInt32(field.Trim());
        }

        protected static string GetProgramType(string field)
        {
            return field.Trim().ToLower();
        }
    }
}