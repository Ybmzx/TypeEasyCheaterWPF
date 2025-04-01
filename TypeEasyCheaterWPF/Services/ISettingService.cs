using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeEasyCheaterWPF.Models;

namespace TypeEasyCheaterWPF.Services
{
    public interface ISettingService
    {
        public ProgramSetting Setting { get; set; }
        bool CreateOrLoad(string path);
        void Save();
    }
}
