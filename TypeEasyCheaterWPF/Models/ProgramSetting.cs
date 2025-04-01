using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEasyCheaterWPF.Models
{
    public class ProgramSetting
    {
        public string TypeEasyExePath { get; set; } = string.Empty;
        public double InputDelay { get; set; } = 0.01;
        public string WantedTimeString { get; set; } = string.Empty;
        public string WantedSpeedString { get; set; } = string.Empty;
    }
}
