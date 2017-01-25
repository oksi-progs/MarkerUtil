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

namespace MarkerUtil
{
    /// <summary>
    /// Interaction logic for AskDigitBox.xaml
    /// </summary>
    public partial class AskDigitBox : Window
    {
        TextBox tbDest;
        public AskDigitBox()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button)
            {
                Button _btn = (Button)sender;
                string _btnName = _btn.Name.Replace("btn_", "");
                if (_btnName.ToUpper() == "OK")
                    Close();
                else if (_btnName.ToUpper() == "C")
                    tbDest.Text = "0";
                else
                {
                    if (tbDest.Text == "0" || tbDest.SelectedText == tbDest.Text)
                        tbDest.Text = "";
                    tbDest.Text += _btnName;
                }



            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbDest.SelectAll();
            foreach (UIElement _el in this.panelBtns.Children)
                if (_el is Button)
                    ((Button)_el).FontSize = tbDest.FontSize;

            btn_Ok.Focus();
        }

        private void SetPosition()
        {
            Point relativePoint = tbDest.TransformToAncestor(Application.Current.MainWindow)
              .Transform(new Point(0, 0));
            double _y = relativePoint.Y + tbDest.ActualHeight;
            if (_y + this.Height > Application.Current.MainWindow.ActualHeight)
                _y = relativePoint.Y - this.Height;
            if (_y < 0) {
                this.Height = relativePoint.Y;
                this.Width = relativePoint.Y;
                _y = 0;
            }
            this.Left = relativePoint.X;
            this.Top = _y;
        }
        public void SetOwner(TextBox _tb)
        {
            tbDest = _tb;
            SetPosition();
        }

    }
}
