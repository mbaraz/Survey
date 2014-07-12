using System.Collections.Generic;
using SurveyModel.Univer;

namespace SurveyWeb.Models.Univer
{
    public class GroupPasswordInfoModel
    {
        public GroupPasswordInfoModel()
        {
            Students = new List<string>();
            Passwords = new List<PasswordInfo>();
        }
        public StudentGroup StudentGroup { get; set; }
        public List<string> Students { get; set; }
        public List<PasswordInfo> Passwords { get; set; }

        public struct PasswordInfo
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}