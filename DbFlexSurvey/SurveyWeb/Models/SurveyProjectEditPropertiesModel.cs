using System.ComponentModel.DataAnnotations;

namespace SurveyWeb.Models
{
    public class SurveyProjectEditPropertiesModel
    {
        public int SurveyProjectId { get; set; }
        
        [Display(Name = "Название проекта")]
        public string SurveyProjectName { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание для респондентов")]
        public string ProjectUserDescription { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст приглашения")]
        public string InviteText { get; set; }

        [Display(Name = "Текст префикса легенды")]
        public string RemarkPrefix  { get; set; }
    }
}