using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class TagCreateModel
    {
        [Display(Name = "Имя переменной")]
        [Required(ErrorMessage = "Необходимо ввести имя переменной")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Метки значений (каждая метка на отдельной строке")]
        public string Values { get; set; }

        public ICollection<SelectListItem> Projects { get; private set; }

        public TagCreateModel()
        {
            
        }

        public TagCreateModel(IEnumerable<SurveyProject> getActiveProjects)
        {
            Projects = new Collection<SelectListItem>
                           {
                               new SelectListItem()
                                   {
                                       Text = "—— Глобальная переменная",
                                       Value = ""
                                   }
                           };
            foreach (var surveyProject in getActiveProjects)
            {
                Projects.Add(new SelectListItem
                                 {
                                     Text = surveyProject.SurveyProjectName, Value = surveyProject.SurveyProjectId.ToString(CultureInfo.InvariantCulture)
                                 });
            }
        }

        [Display(Name = "Проект")]
        public int? ProjectId { get; set; }
    }
}