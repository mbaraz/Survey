using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using SurveyModel.Univer;

namespace SurveyDomain.Univer.Uploaders
{
    class DiskFileStore
    {
        private const string specSuffix = "специализации";
        private const string courseSuffix = "курсы";
        private const string csvExtension = ".csv";
        private const string configsPath = "~/Content/Facillities";

        private readonly string _uploadsFolder = HostingEnvironment.MapPath(Constants.PathToFacilityFiles);
        private readonly string _depsFolder = HostingEnvironment.MapPath(configsPath);

        internal string SaveUploadedFile(HttpPostedFileBase fileBase, Facility facility)
        {
            string fullPath = getUploadDiskLocation(facility, getFileExtension(fileBase.FileName));
            fileBase.SaveAs(fullPath);
            return fullPath;
        }

        internal void SaveCorrectData(List<string> correctRows, Facility facility, bool isAppended)
        {
            string fullPath = getUploadDiskLocation(facility, csvExtension);
            using (StreamWriter writer = new StreamWriter(fullPath, isAppended, Encoding.GetEncoding(1251)))
            {
                foreach (var csvString in correctRows)
                    writer.WriteLine(csvString);
            }
        }

        internal Facility GetFacilityConfig(string guid)
        {
            try {
                XDocument document = readFacilityConfig(guid);
                XElement xmlContent = document.Element("department");
                string facilityName = xmlContent.Element("name").Value;
                int step = int.Parse(xmlContent.Element("step").Value);
                Facility facility = new Facility(facilityName, guid, step);
                IEnumerable<string> query = from item in xmlContent.Element("specs").Elements() select item.Value;
                facility.SpecsCodes = query.ToList();
                return facility;
                } catch {
                    return null;
                }
        }

        internal void IncreaseConfigStep(Facility facility)
        {
            XDocument document = readFacilityConfig(facility.Guid);
            XElement xmlContent = document.Element("department");
            int step = (int) xmlContent.Element("step");
            xmlContent.Element("step").SetValue(++step);
            saveFacilityConfig(document, facility);
        }

        internal List<string> GetSpecsList(StudentGroup group)
        {
            List<string> result = new List<string>();
            string nameWithoutExtension = Facility.GetFullName(group.Facility) + " " + specSuffix;
            string fileName = nameWithoutExtension + csvExtension;
            string fullPath = Path.Combine(_uploadsFolder, fileName);

            using (StreamReader reader = new StreamReader(fullPath, Encoding.GetEncoding(1251)))
            {
                while (reader.Peek() > -1) {
                    string[] line = reader.ReadLine().Split(';');
                    if (line[3] == group.ProgramType) // "магистратура"
                        result.Add(line[1] + "#" + line[2]);
                    }
            }
            return result; ;
        }

        private string getSuffix(bool isSpecStage)
        {
            return isSpecStage ? specSuffix : courseSuffix;
        }

        private XDocument readFacilityConfig(string guid)
        {
            string fullPath = getConfigDiskLocation(guid);
            return XDocument.Load(fullPath);
        }

        private void saveFacilityConfig(XDocument document, Facility facility)
        {
            if (facility.IsSpecStage) {
                List<string> specs = readSpecs(facility);
                XElement xmlSpecs = document.Element("department").Element("specs");
                foreach (string spec in specs)
                    xmlSpecs.Add(new XElement ("item", spec));
            }
            string fullPath = getConfigDiskLocation(facility.Guid);
            document.Save(fullPath);
        }

        private List<string> readSpecs(Facility facility)
        {
            List<string> result = new List<string>();
            string fullPath = getUploadDiskLocation(facility, csvExtension);
//            int i = 0;
            using (StreamReader reader = new StreamReader(fullPath, Encoding.GetEncoding(1251)))
            {
                while (reader.Peek() > -1)
                    result.Add(reader.ReadLine().Split(';')[1]);
            }
            return result;
        }

        private string getUploadDiskLocation(Facility facility, string extension)
        {
            string nameWithoutExtension = facility.Name + " " + getSuffix(facility.IsSpecStage);
            string fileName = nameWithoutExtension + extension;
            return Path.Combine(_uploadsFolder, fileName);
        }

        private string getConfigDiskLocation(string guid)
        {
            return Path.Combine(_depsFolder, getConfigName(guid));
        }

        private static string getFileExtension(string fileName)
        {
            int indx = fileName.LastIndexOf('.');
            return fileName.Substring(indx);
        }

        private static string getConfigName(string guid)
        {
            return "dep-" + guid + "-01.xml";
        }
    }
}