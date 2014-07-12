using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SurveyInterfaces;
using SurveyWeb.Models;

namespace SurveyWeb.Controllers
{
    public class SharedController : ControllerBase
    {
        public SharedController(IUserService userService, IUnitOfWork unitOfWork) : base(unitOfWork, userService)
        {
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            return PartialView(CheckMenuAccess(GetMenu()));
        }

        private static IEnumerable<MenuItemModel> CheckMenuAccess(IEnumerable<MenuItemModel> getMenu)
        {
            return getMenu.Where(menuItemModel => string.IsNullOrEmpty(menuItemModel.RequiredRole) || Roles.IsUserInRole(menuItemModel.RequiredRole));
        }

        private static IEnumerable<MenuItemModel> GetMenu()
        {
            yield return new MenuItemModel { Controller = "Home", Text = "На главную" };
            yield return new MenuItemModel { Controller = "Home", Text = "О сайте", Action = "About" };
            yield return new MenuItemModel {Controller = "SurveyProject", Text = "Проекты", RequiredRole = "Staff", Action = "Active"};
            yield return new MenuItemModel {Controller = "Respondent", Text = "Респонденты", RequiredRole = "Staff"};
            yield return new MenuItemModel {Controller = "Tag", Text = "Переменные", RequiredRole = "Staff"};
        }
    }
}