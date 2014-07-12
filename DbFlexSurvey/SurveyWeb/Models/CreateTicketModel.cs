using System.ComponentModel.DataAnnotations;

namespace SurveyWeb.Models
{
    public class CreateTicketModel
    {
        public int SurveyProjectId { get; set; }
        [Display(Name = "Описание ссылки")]
        public string Description { get; set; }
    }
}