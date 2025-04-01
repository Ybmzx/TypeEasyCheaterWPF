using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TypeEasyCheaterWPF.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenSingleFileDialog(string? filter = null, string? initialDirectory = null, bool? restoreDirectory = null, int? filterIndex = null, string? title = null)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter ?? openFileDialog.Filter;
            openFileDialog.InitialDirectory = initialDirectory ?? openFileDialog.InitialDirectory;
            openFileDialog.RestoreDirectory = restoreDirectory ?? openFileDialog.RestoreDirectory;
            openFileDialog.FilterIndex = filterIndex ?? openFileDialog.FilterIndex;
            openFileDialog.Title = title ?? openFileDialog.Title;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
