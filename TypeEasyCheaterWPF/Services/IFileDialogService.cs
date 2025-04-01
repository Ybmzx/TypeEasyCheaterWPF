using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEasyCheaterWPF.Services
{
    public interface IFileDialogService
    {
        string? OpenSingleFileDialog(string? filter = null,
                                    string? initialDirectory = null,
                                    bool? restoreDirectory = null,
                                    int? filterIndex = null,
                                    string? title = null);
    }
}
