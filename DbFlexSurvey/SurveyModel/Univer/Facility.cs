using System.Collections.Generic;

namespace SurveyModel.Univer
{
    public class Facility
    {
        private enum ProcessStage { Specs, Courses, Nothing };

        public readonly string Name;
        public readonly string Guid;
        private readonly ProcessStage _stage;
        public List<string> SpecsCodes = new List<string>();
        public bool IsSpecStage { get { return _stage == ProcessStage.Specs; } }
        public bool IsCourseStage { get { return _stage == ProcessStage.Courses; } }
        public bool IsFinished { get { return _stage == ProcessStage.Nothing; } }
        public string TrimedName { get { return Name.ToLower().Replace(" факультет", "").Replace("факультет ", ""); } }

        public Facility(string facilityName, string guid, int step)
        {
            Name = facilityName;
            Guid = guid;
            _stage = (ProcessStage)step;
        }

        public static string GetFullName(string facilityName)
        {
            if (facilityName.ToLower() == "экономический")
                return "Экономический факультет";
            return "Факультет " + facilityName;
        }

        public static string GetTrimedName(string facilityName)
        {
            return facilityName.Trim().ToLower().Replace(" факультет", "").Replace("факультет ", "").Replace("международных отношений", "международные отношения").Replace("экономики", "экономический");
        }
    }
}
