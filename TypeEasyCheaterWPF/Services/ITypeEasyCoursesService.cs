using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEasyCheaterWPF.Services
{
    public interface ITypeEasyCoursesService
    {
        bool Load(string typeEasyExePath);
        Dictionary<string, string> GetPathsAndCourseNames();
        string GetContent(string coursePath);
    }
}
