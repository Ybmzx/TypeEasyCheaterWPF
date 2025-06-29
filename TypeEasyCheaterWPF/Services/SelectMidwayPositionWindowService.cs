using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeEasyCheaterWPF.Views;

namespace TypeEasyCheaterWPF.Services
{
    public class SelectMidwayPositionWindowService: ISelectMidwayPositionWindowService
    {
        public int? ShowDialog(string content)
        {
            SelectMidwayPositionWindow window = new(content);
            window.ShowDialog();
            return window.IsCancelled ? null : window.MidwayPosition;
        }
    }
}
