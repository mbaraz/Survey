using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Web.Hosting;
using BinaryAnalysis.UnidecodeSharp;

namespace SurveyDomain.Univer
{
    internal class ExportAllToZipStream
    {
        private const int BufferSize = 4096;
        private readonly string zipFileName;

        internal ExportAllToZipStream(string facility)
        {
            zipFileName = Path.Combine(HostingEnvironment.MapPath(Constants.PathToFacilityFiles), facility + " результаты.zip");
        }
        
        internal FileStream MakeStream(List<byte[]> results, string[] coursesNames)
        {
            File.Delete(zipFileName);
            for (int i = 0; i < coursesNames.Length; i++) {
                string fileName = coursesNames[i].Unidecode().Replace(' ', '_').Replace(':', '_').Replace('/', '_').Replace("\"", string.Empty).Replace("«", string.Empty).Replace("»", string.Empty);
                addFile(fileName + ".sps", results[2 * i]);
                addFile(fileName + ".txt", results[2 * i + 1]);
            }
            addFile("Facility_comments.txt", results[results.Count - 1]);
            return new FileStream(zipFileName, FileMode.Open, FileAccess.Read);
        }

        private void addFile(string fileToAdd, byte[] content)
        {
            using (Package zip = Package.Open(zipFileName, FileMode.OpenOrCreate)) {
                string destFilename = ".\\" + Path.GetFileName(fileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                    zip.DeletePart(uri);

                PackagePart part = zip.CreatePart(uri, string.Empty, CompressionOption.Normal);
                using (Stream dest = part.GetStream()) {
                    copyString(content, dest);
                }
            }
        }

        private void copyString(byte[] content, Stream outputStream)
        {
            int bufferSize = content.Length < BufferSize ? content.Length : BufferSize;
            int bytesWritten = 0;
            while (bytesWritten < content.Length) {
                int lnth = Math.Min(bufferSize, content.Length - bytesWritten);
                outputStream.Write(content, bytesWritten, lnth);
                bytesWritten += lnth;
            }
        }
    }
}
