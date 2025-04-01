using Memory;
using System.Text;

namespace TypeEasyCheaterWPF.Services
{
    class TypeEasyProgramMemoryModifyService : ITypeEasyProgramMemoryModifyService
    {
        public async Task<bool> ModifyAsync(string processName, string originalSpeedString, string originalTimeString, string wantedSpeedString, string wantedTimeString)
        {
            if (wantedSpeedString.Length > originalSpeedString.Length ||
                wantedTimeString.Length > originalTimeString.Length) {
                return false;
            }

            Mem memLib = new();

            if (!memLib.OpenProcess(processName)) return false;

            var timeResult = await memLib.AoBScan(GetBytesArrayString(originalTimeString), true);
            var speedResult = await memLib.AoBScan(GetBytesArrayString(originalSpeedString), true);

            wantedSpeedString += new string(' ', originalSpeedString.Length - wantedSpeedString.Length);
            wantedTimeString += new string(' ', originalTimeString.Length - wantedTimeString.Length);

            foreach (var timeAddr in timeResult)
            {
                memLib.WriteMemory($"0x{timeAddr.ToString("X")}", "bytes", GetBytesArrayString(wantedTimeString));
            }

            foreach (var speedAddr in speedResult)
            {
                memLib.WriteMemory($"0x{speedAddr.ToString("X")}", "bytes", GetBytesArrayString(wantedSpeedString));
            }

            return true;
        }

        private string GetBytesArrayString(string str, Encoding? encoding = null)
        {
            if (encoding is null) encoding = System.Text.Encoding.Unicode;

            byte[] utf16Bytes = encoding.GetBytes(str);
            string aobPattern = BitConverter.ToString(utf16Bytes).Replace("-", " ");
            return aobPattern;
        }
    }
}
