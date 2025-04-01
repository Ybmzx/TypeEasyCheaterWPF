using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TypeEasyCheaterWPF.Services
{
    public class TypeEasyRecordModifyService : ITypeEasyRecordModifyService
    {
        public void ModifyLastRecord(int speed)
        {
            string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TypeEasyData", "user");

            var lastRecord = Directory.GetFiles(userPath, "*.xml", SearchOption.AllDirectories)
                    .Where(x => x.Contains("score.xml"))
                    .OrderByDescending(x => new FileInfo(x).LastWriteTime)
                    .FirstOrDefault()!;

            XmlDocument recordDoc = new();
            recordDoc.Load(lastRecord);
            recordDoc!.SelectNodes("records/item")!.Cast<XmlNode>().Last()!.Attributes!["speed"]!.Value = speed.ToString();

            recordDoc.Save(lastRecord);
        }
    }
}
