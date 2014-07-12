using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqToExcel;
using LinqToExcel.Domain;
using SurveyDomain.Univer.Rows;
using SurveyDomain.Univer.Parsers.Tools;
using SurveyDomain.Univer.Results;
using SurveyDomain.Univer.Uploaders;
using SurveyModel.Univer;

namespace SurveyDomain.Univer
{
    class ExcelFileParserBase
    {
        private readonly Facility _facility;
        private List<string> _correctRows;

        internal ExcelFileParserBase(Facility facility)
        {
            _facility = facility;
        }

        internal ResultsBase ProcessFacilityFile(HttpPostedFileBase fileBase)
        {
            return saveAndParseFile(fileBase);
        }

        internal ResultsBase AppendCorrectedRows(IEnumerable<KeyValuePair<int, string>> rowItems)
        {
            var rows = ExcelRowTools.MakeRowList(rowItems.ToList(), _facility.IsSpecStage);
            ResultsBase results = checkContent(rows);
            saveCorrectData(results, true);
            return results;
        }

        private ResultsBase saveAndParseFile(HttpPostedFileBase fileBase)
        {
            string path = new DiskFileStore().SaveUploadedFile(fileBase, _facility);
            var rows = getRows(path, _facility.IsSpecStage);
            return checkAndSaveRows(rows);
        }

        private ResultsBase checkAndSaveRows(List<BaseRow> rows)
        {
            ResultsBase results = checkRows(rows);
            saveCorrectData(results);
            return results;
        }

        private ResultsBase checkRows(List<BaseRow> rows)
        {
            ResultsBase results = checkColumnNames(rows.First());
            if (results.NoError)
                results = checkContent(rows);
            return results;
        }

        private void saveCorrectData(ResultsBase results, bool isAppended = false)
        {
            if (results.NoError)
            {
                new DiskFileStore().SaveCorrectData(_correctRows, _facility, isAppended);
                new DiskFileStore().IncreaseConfigStep(_facility);
            }
            else if (!results.ShowErrorTextOnly)
                new DiskFileStore().SaveCorrectData(_correctRows, _facility, isAppended);
        }

        private NamesResults checkColumnNames(BaseRow row)
        {
            NamesResults namesResults = new NamesResults();
            List<int> errors = row.CheckColumnNames();
            foreach (int i in errors)
                namesResults.AddErrorRow(i, row);
            return namesResults;
        }

        private ContentResults checkContent(List<BaseRow> rows)
        {
            _correctRows = new List<string>();
            ContentResults contentResults = new ContentResults();
            int rowCount = 2;
            foreach (var row in rows)
            {
                string errorMessage;
                if (row.IsCorrect(out errorMessage, _facility.SpecsCodes))
                    _correctRows.Add(row.ParceToCsv(_facility.TrimedName));
                else
                    contentResults.AddErrorRow(rowCount, row, errorMessage);
                if (contentResults.IsEnough)
                    break;
                rowCount++;
            }
            return contentResults;
        }

        private static List<BaseRow> getRows(string path, bool isSpecStage)
        {
            var excel = new ExcelQueryFactory(path);
            excel.DatabaseEngine = DatabaseEngine.Ace;
            var rows = from c in excel.Worksheet(0) select new RowFactory().GetRow(c, isSpecStage);
            return rows.ToList();
        }
    }
}
