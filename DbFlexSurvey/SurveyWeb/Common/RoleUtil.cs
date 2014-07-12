using System;

namespace SurveyWeb.Common
{
    public static class RoleUtil
    {
        public static string GetDesciption(string role)
        {
            switch (role)
            {
                case "Staff":
                    return "Сотрудник РЦ";
                case "Admin":
                    return "Администратор РЦ";
                case "Interviewer":
                    return "Интервьюер";
                default:
                    throw new Exception("Unknown role");

            }
        }
    }
}