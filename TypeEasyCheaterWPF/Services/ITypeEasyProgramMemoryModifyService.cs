using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEasyCheaterWPF.Services
{
    public interface ITypeEasyProgramMemoryModifyService
    {
        public Task<bool> ModifyAsync(string processName,
                                            string originalSpeedString,
                                            string originalTimeString,
                                            string wantedSpeedString,
                                            string wantedTimeString);
    }
}
