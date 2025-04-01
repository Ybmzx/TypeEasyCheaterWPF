using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TypeEasyCheaterWPF.Models;

namespace TypeEasyCheaterWPF.Services
{
    class XmlSettingService : ISettingService
    {
        public ProgramSetting Setting { get; set; } = new();
        public string Path { get; set; }

        public XmlSettingService(string path)
        {
            CreateOrLoad(path);
        }

        public bool CreateOrLoad(string path)
        {
            Path = path;
            XmlSerializer serializer = new(typeof(ProgramSetting));
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    serializer.Serialize(fs, Setting);
                }
                return true;
            }

            using (FileStream stream = new(path, FileMode.Open))
            {
                var result = (ProgramSetting?)serializer.Deserialize(stream);
                if (result is null) return false;
                Setting = result;
            }

            return true;
        }

        public void Save()
        {
            XmlSerializer serializer = new(typeof(ProgramSetting));
            using (FileStream fs = File.Create(Path))
            {
                serializer.Serialize(fs, Setting);
            }
        }

    }
}
