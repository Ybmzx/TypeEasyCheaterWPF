using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Xml;

namespace TypeEasyCheaterWPF.Services
{
    public class TypeEasyCoursesService : ITypeEasyCoursesService
    {
        Dictionary<string, string> pathsAndCoursesName = new();

        public string GetContent(string coursePath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(coursePath);
            string courseType = xmlDoc.SelectSingleNode("course/courseInfo")!.Attributes!["alias"]!.Value!;
            XmlNodeList items = xmlDoc!.SelectNodes("course/content/item")!;

            if (courseType.Contains("article"))
            {
                return string.Join("", items.Cast<XmlNode>()
                    .Select(x => x!.Attributes!["comparision"]!.Value));
            }

            return string.Join(" ", items.Cast<XmlNode>()
                    .Select(x => x!.Attributes!["comparision"]!.Value));
        }

        public Dictionary<string, string> GetPathsAndCourseNames()
        {
            return pathsAndCoursesName;
        }

        public bool Load(string typeEasyExePath)
        {
            pathsAndCoursesName.Clear();

            // 获得当前程序下所有目录, 不递归
            var dirs = Directory.GetDirectories(Path.GetDirectoryName(typeEasyExePath)!, "*", SearchOption.TopDirectoryOnly);
            string? courseFolderPath = null;
            foreach (var dir in dirs)
            {
                if (!Directory.Exists(Path.Combine(dir, "course"))) continue;
                courseFolderPath = Path.Combine(dir, "course");
                break;
            }
            if (courseFolderPath == null) return false;
            // 获得 course 目录下所有 xml 文件
            var xmlFiles = Directory.GetFiles(courseFolderPath, "*.xml", SearchOption.AllDirectories);

            foreach (var xmlFile in xmlFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                var title = xmlDoc.SelectSingleNode("course/courseInfo")?.Attributes?["alias"]?.Value;
                if (title is null) continue;
                pathsAndCoursesName.Add(xmlFile, title);
            }

            return true;
        }
    }
}
