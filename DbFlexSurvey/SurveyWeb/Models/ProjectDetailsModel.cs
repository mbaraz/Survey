using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SurveyModel;

namespace SurveyWeb.Models
{
    public class ProjectDetailsModel
    {
        public ProjectDetailsModel(SurveyProject surveyProject)
        {
            Project = surveyProject;
            Tickets =
                surveyProject.Invitations.Where(invitation => invitation.TicketId != null).Select(
                    invitation => invitation.Ticket).Distinct().ToArray();
            CreateTicketModel = new CreateTicketModel
                                    {
                                        SurveyProjectId = surveyProject.SurveyProjectId
                                    };
        }

        public SurveyProject Project { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public CreateTicketModel CreateTicketModel { get; set; }
    }
}