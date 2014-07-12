using System;
using System.Web;
using System.Web.Mvc;
using SurveyDomain.Univer.Results;
using SurveyInterfaces;
using SurveyModel.Univer;

namespace SurveyWeb.Controllers
{
    public class FacilityController : ControllerBase
    {
        private const string itemName = "item_";
        private const string specLegend = "специализаций";
        private const string сourseLegend = "курсов";

        private readonly IUniverService _univerService;

        public FacilityController(IUniverService univerService) : base(null, null)
        {
            _univerService = univerService;
        }
        //
        // GET: /Facility/

        public ActionResult Index(string guid)
        {
            Facility facility = _univerService.ReadFacilityConfig(guid);
            if (facility == null)
                return null;
            ViewBag.FacilityName = facility.Name;
            if (facility.IsFinished)
                return View("Finish");
            
            ViewBag.Legend = facility.IsSpecStage ? specLegend : сourseLegend;
            TempData["facility"] = facility;
            return View();
        }

        [HttpPost]
        public ActionResult CourseUploaded(HttpPostedFileBase uploadingFile)
        {
            try
            {
                Facility facility = TempData["facility"] as Facility;
                if (facility == null)
                   return showError("Произошла ошибка, связанная с нажатием  кнопки \"Назад\". Не используйте кнопок \"Назад\" и \"Вперед\".", "Войдите на сайт еще раз, используя ссылку, указанную в полученном Вами электронном письме.");

                ResultsBase parseResult = _univerService.UploadFacilityFile(uploadingFile, facility) as ResultsBase;
                if (parseResult.NoError)
                    return RedirectToAction("Index", new { guid = facility.Guid });
                
                if (parseResult.ShowErrorTextOnly)
                    return showError(parseResult.ErrorMessage, "Внесите необходимые исправления в файл Excel и загрузите его еще раз.", facility.Name);

                ViewBag.FacilityName = facility.Name;
                ViewBag.ErrorMsg = parseResult.ErrorMessage;
                ViewBag.IsCourseStage = facility.IsCourseStage;
                ViewBag.ItemName = itemName;
                ViewBag.Guid = facility.Guid;
                TempData["facility"] = facility;
                return View(parseResult);
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = exc.Message;
                return showIndex();
            }
        }

        [HttpPost]
        public ActionResult CoursesCorrected()
        {
            try
            {
                Facility facility = TempData["facility"] as Facility;
                var rowItems = GetDictionary(itemName);
                ResultsBase parseResult = _univerService.AppendFacilityCsv(rowItems, facility) as ResultsBase;
                if (parseResult.NoError)
                    return RedirectToAction("Index", new { guid = facility.Guid });
                TempData["facility"] = facility;
                ViewBag.IsCourseStage = facility.IsCourseStage;
                ViewBag.ItemName = itemName;
                ViewBag.Guid = facility.Guid;
                return View("CourseUploaded", parseResult);
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMsg = exc.Message;
                return showIndex();
            }
        }

        private ActionResult showError(string errorMsg, string directionMsg, string facilityName = "")
        {
            ViewBag.ErrorMsg = errorMsg;
            ViewBag.DirectionMsg = directionMsg;
            ViewBag.FacilityName = facilityName;
            return View("ParseError");
        }

        private ActionResult showIndex()
        {
            return View("Index");
        }
    }
}
