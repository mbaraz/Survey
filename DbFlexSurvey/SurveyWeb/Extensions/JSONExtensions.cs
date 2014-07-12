using System.Web.Script.Serialization;

namespace SurveyWeb.Extensions
{
    public static class JSONExtensions
    {
        public static string ToJSON(this object thing, int? recursionDepth)
        {
            var serializer = new JavaScriptSerializer();
            if (recursionDepth.HasValue)
                serializer.RecursionLimit = recursionDepth.Value;
            return serializer.Serialize(thing);
        }

        public static object FromJSON(string source, int? recursionDepth)
        {
            var serializer = new JavaScriptSerializer();
            if (recursionDepth.HasValue)
                serializer.RecursionLimit = recursionDepth.Value;
            return serializer.DeserializeObject(source);
        }
    }
}