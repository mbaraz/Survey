using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SurveyModel;

namespace SurveyWeb.Models.Univer
{
    public class UniverInfoModel
    {
        public ICollection<SelectListItem> Projects { get; private set; }

        public int SurveyProjectId { get; set; }

        public UniverInfoModel()
        {
            
        }

        public UniverInfoModel(IEnumerable<SurveyProject> activeProjects)
        {
            
            Projects =
                activeProjects.Select(
                    ap => new SelectListItem {Text = ap.SurveyProjectName, Value = ap.SurveyProjectId.ToString(CultureInfo.InvariantCulture)}).
                    ToArray();
        }
    }
}