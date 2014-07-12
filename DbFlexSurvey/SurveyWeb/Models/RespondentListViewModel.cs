using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class RespondentListViewModel
    {
        public IEnumerable<Respondent> Respondents { get; set;}
        public IEnumerable<SurveyProject> Projects { get; set; }
    }
}