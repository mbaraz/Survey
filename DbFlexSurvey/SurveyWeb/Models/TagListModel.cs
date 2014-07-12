using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class TagListModel
    {
        public IEnumerable<Tag> Tags { get; set; }
        public TagCreateModel CreateModel { get; set; }
    }
}