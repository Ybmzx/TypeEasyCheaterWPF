using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TypeEasyCheaterWPF.Views
{
    /// <summary>
    /// SelectMidwayPosition.xaml 的交互逻辑
    /// </summary>
    public partial class SelectMidwayPositionWindow : Window
    {
        private bool isProgrammaticClose = false;

        public string TextBoxContent { get; set; }
        public int MidwayPosition { get; set; } = 0;
        public bool IsCancelled { get; set; } = false;

        public SelectMidwayPositionWindow(string content)
        {
            InitializeComponent();
            TextBoxContent = content;
            ArticleTextBox.Text = TextBoxContent;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isProgrammaticClose)
            {
                IsCancelled = true;
            }
        }

        private void ArticleTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var content = ArticleTextBox.Text;
            int index = ArticleTextBox.CaretIndex;
            MidwayPosition = index;
            (SelectedLeft.Text, SelectedRight.Text) = GetSurroundingChars(content, index);
        }

        public (string left, string right) GetSurroundingChars(string text, int index, int length = 10)
        {
            // 获取左侧子串（不包括索引位置）
            int leftStart = Math.Max(0, index - length);
            int leftPartLength = index - leftStart;
            string leftPart = text.Substring(leftStart, leftPartLength);

            // 获取右侧子串（包括索引位置）
            int rightPartLength = Math.Min(length, text.Length - index);
            string rightPart = text.Substring(index, rightPartLength);

            return (leftPart, rightPart);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancelled = true;
            ProgrammaticClose();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            ProgrammaticClose();
        }

        private void ProgrammaticClose()
        {
            isProgrammaticClose = true;
            Close();
        }

    }
}
