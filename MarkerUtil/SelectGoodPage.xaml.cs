using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkerUtil
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class SelectGoodPage : Page
    {
        public DataView dvGoods;
        public SelectGoodPage()
        {
            InitializeComponent();
        }

        private void CreateButtons()
        {
            double btnWidth = this.Width / 2;
            double btnHeight = this.Height / 10;
            int _rows = dvGoods.Count / 2;
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < 2; j++)
                {
                    int _row = j + i * 2;
                    if (_row < dvGoods.Count)
                    {
                        Button _btn = new Button();
                        _btn.Content = dvGoods[_row]["Name"];
                        _btn.Height = btnHeight;
                        _btn.FontSize = btnHeight * 3 / 4;
                        _btn.Click += _btn_Click;
                        _btn.Tag = dvGoods[_row];
                        _btn.Margin = new Thickness(2, 2, 2, 2);

                        this.btnPanel.Children.Add(_btn);
                    }
                }
        }
        private void _btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow _mw = System.Windows.Application.Current.MainWindow as MainWindow;
            Button _btn = (Button)sender;
            _mw.currentData = (DataRowView)_btn.Tag;
            _mw.currentData["ProducedDate"] = DateTime.Now;
            _mw.MainFrame.Navigate(_mw.detailPage);
        }


    }
}
