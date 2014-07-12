using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyWeb.Models
{
    public class MenuItemModel
    {
        public MenuItemModel()
        {
            Action = "Index";
        }
        public string Text { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string RequiredRole { get; set; } 
    }
}