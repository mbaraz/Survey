using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class MultiInviteModel
    {
        private const char separator = '\n';
        
        public MultiInviteModel()
        {
             
        }

        public SurveyProject SurveyProject
        {
            set
            {
                SurveyProjectId = value.SurveyProjectId;
                SurveyProjectName = value.SurveyProjectName;
                InviteText = value.Invitation;
            }
        }

        [Display(Name = "Имя проекта")]
        public string SurveyProjectName { get; set; }
        public int SurveyProjectId { get; set; }
        
        [Display(Name = "Электронная почта", Description = "Строчка - email")]
        [DataType(DataType.MultilineText)]
        public string Email { get; set; }

        [Display(Name = "Текст приглашения")]
        [DataType(DataType.MultilineText)]
        public string InviteText { get; set; }

        [Display(Name = "Заголовок письма")]
        public string InviteSubject { get; set; }

        public IEnumerable<string> InstantEmails { get { return Email.Split(separator).Where(em => !string.IsNullOrWhiteSpace(em)).Select(st => st.Trim()); } }
    }
}