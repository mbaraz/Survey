using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using SurveyInterfaces;

namespace SurveyWeb.Controllers
{
    public class PeriodicController : Controller
    {
        private readonly IRespondentService _respondentService;
        private readonly IUnitOfWork _unitOfWork;

        public PeriodicController(IRespondentService respondentService, IUnitOfWork unitOfWork)
        {
            _respondentService = respondentService;
            _unitOfWork = unitOfWork;
        }

        public string Update()
        {
            var sendMessages = _respondentService.SendInvites(WebConfigurationManager.AppSettings["SiteName"]);
            _unitOfWork.Save();
           return sendMessages.ToString(CultureInfo.InvariantCulture);
       }
    }
}
