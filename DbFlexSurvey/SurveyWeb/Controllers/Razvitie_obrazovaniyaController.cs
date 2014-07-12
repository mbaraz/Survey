using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SurveyWeb.Controllers
{
    public class Razvitie_obrazovaniyaController : Controller
    {
        //
        // GET: /We are/
        private readonly string client_secret = "EDDB1A6D680BDFF8E49A179C";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Odnkl()
        {
            return View();
        }

        public ActionResult GetAccessToken(string code)
        {
            string responseToString;
            string postedData = "client_id=201872896&grant_type=authorization_code&client_secret=" + client_secret + "&code=" + code + "&redirect_uri=http://localhost:50784/Razvitie_obrazovaniya/GetAccessToken";     //   "response_type=code&client_id=201872896"; // &scope=VALUABLE ACCESS &redirect_uri=
            string ApiUrl = "http://api.odnoklassniki.ru/oauth/token.do";
            var response = PostMethod(postedData, ApiUrl);
            if (response == null)
                return RedirectToAction("Odnkl");

            var strreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            responseToString = strreader.ReadToEnd();
            var t = FromJSON(responseToString, null);
            string accessToken = t["access_token"].ToString();
            Console.Write(accessToken, t["access_token"]);
            TempData["access_token"] = accessToken;
            return RedirectToAction("GetUserInfo", "Razvitie_obrazovaniya", accessToken);
        }

        public ActionResult GetUserInfo(string accessToken)
        {
            string access_token = (string)TempData["access_token"];
            var sigSecret = GetMD5Hash(string.Format("{0}{1}", access_token, client_secret));
            Console.Write(access_token, client_secret);
            var sig = GetMD5Hash(string.Format("{0}{1}", "application_key=CBAHFJANABABABABAmethod=users.getCurrentUser", sigSecret));
            string responseToString;
            string postedData = "application_key=CBAHFJANABABABABA&access_token=" + access_token + "&method=users.getCurrentUser&sig=" + sig;
            string ApiUrl = "http://api.odnoklassniki.ru/fb.do";
            var response = PostMethod(postedData, ApiUrl);
            var strreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            responseToString = strreader.ReadToEnd();
            var dict = FromJSON(responseToString, null);
            Console.Write(dict["first_name"].ToString());
            return View(dict);
        }
        
        public ActionResult ChangeStatus(string accessToken)
        {
            string access_token = (string)TempData["access_token"];
            var sigSecret = GetMD5Hash(string.Format("{0}{1}", access_token, client_secret));
            Console.Write(access_token, client_secret);
            var sig = GetMD5Hash(string.Format("{0}{1}", "application_key=CBAHFJANABABABABAmethod=users.setStatusstatus=Working", sigSecret));
            string responseToString;
            string postedData = "application_key=CBAHFJANABABABABA&access_token=" + access_token + "&method=users.setStatus&status=Working&sig=" + sig;
            string ApiUrl = "http://api.odnoklassniki.ru/fb.do";
            var response = PostMethod(postedData, ApiUrl);
            var strreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            responseToString = strreader.ReadToEnd();
            Console.Write(responseToString);
            return RedirectToAction("Odnkl");
        }

        private static HttpWebResponse PostMethod(string postedData, string postUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;

            UTF8Encoding encoding = new UTF8Encoding();
            var bytes = encoding.GetBytes(postedData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
            }
            return (HttpWebResponse)request.GetResponse();
        }

        private static string GetMD5Hash(string input)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (var b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
/*

        private static string ToJSON(this object thing, int? recursionDepth)
        {
            var serializer = new JavaScriptSerializer();
            if (recursionDepth.HasValue)
                serializer.RecursionLimit = recursionDepth.Value;
            return serializer.Serialize(thing);
        }
*/

        private static Dictionary<string, object> FromJSON(string source, int? recursionDepth)
        {
            var serializer = new JavaScriptSerializer();
            if (recursionDepth.HasValue)
                serializer.RecursionLimit = recursionDepth.Value;
            return serializer.Deserialize<Dictionary<string, object>>(source);
//            return serializer.DeserializeObject(source);
        }
    }
}
