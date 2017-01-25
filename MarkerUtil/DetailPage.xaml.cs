using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using fyiReporting.RdlViewer;
using System.Drawing.Printing;
using System.IO;
using System.Printing;

namespace MarkerUtil
{
    /// <summary>
    /// Interaction logic for DetailPage.xaml
    /// </summary>
    public partial class DetailPage : Page
    {
        public DataRowView currentData;
        public DataTable _dt;
        public DataView dvGoods;
        public double dFontSize;

        public DetailPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dFontSize = cbGoods.FontSize;
            dvGoods = _dt.AsDataView();
            dvGoods.Sort = "Id ASC";
            DataRow[] _rows = _dt.Select("Id=" + Properties.Settings.Default.LastGoodID.ToString());
            if(_rows.Count()>0)
                currentData = dvGoods.FindRows(_rows[0]["Id"])[0];
            this.DataContext = currentData;
            MainWindow _mw = Application.Current.MainWindow as MainWindow;
            _mw.SetFontSize(this.panelDetailCaptions.Children);
            _mw.SetFontSize(this.panelDetailData.Children);
            _mw.currentData = currentData;
            this.tbCopies.PreviewTextInput += TbCopies_PreviewTextInput;
            tbCopies.Text = Properties.Settings.Default.LastCopies.ToString();
            cbGoods.MaxDropDownHeight = this.Height - cbGoods.Height - 10;
        }

        private void TbCopies_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox _cb = (ComboBox)sender;
            _cb.ItemsSource = dvGoods;
            _cb.SelectedItem = currentData;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox _cb = (ComboBox)sender;
            currentData = (DataRowView)_cb.SelectedItem;
            currentData["ProducedDate"] = DateTime.Now;
            this.DataContext = currentData;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            currentData["UseBy"] = ((DateTime)currentData["ProducedDate"]).AddDays((int)currentData["Valid"]);
        }

        private void TimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DateTime _dt = (DateTime)currentData["UseBy"];
            DateTime _dt_new = (DateTime)currentData["ProducedDate"];
            TimeSpan _ts = new TimeSpan(_dt_new.Hour, _dt_new.Minute, 0);
            _dt = _dt.Date + _ts;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                MainWindow _mw = System.Windows.Application.Current.MainWindow as MainWindow;
                AskDigitBox _adb = _mw._adb;
                if (_adb == null || !_adb.IsVisible)
                    _adb = new AskDigitBox();

                _adb.SetOwner((TextBox)sender);
                _adb.Show();
                _mw._adb = _adb;
            }
        }

        private void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LastGoodID = (Int32)currentData["Id"];
            Properties.Settings.Default.Save();
            string _mask = currentData["Mask"].ToString().Trim();
            if (!_mask.EndsWith(".rdl"))
                _mask += ".rdl";

            string filepath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory() + @"\Masks", _mask);

            if(!File.Exists(filepath))
            {
                MessageBox.Show("Файл шаблона этикетки не найден! "+filepath);
                return;
            }
            DataTable _dt_toprint = _dt.Clone();

            DataRow _row = _dt_toprint.NewRow();

            foreach (DataColumn _dc in currentData.DataView.Table.Columns)
                _row[_dc.ColumnName] = currentData[_dc.ColumnName];

            _dt_toprint.Rows.Add(_row);

            RdlViewer rdlView = new RdlViewer();
            rdlView.SourceFile = new Uri(filepath);
            rdlView.Report.DataSets["Data"].SetData(_dt_toprint);
            rdlView.Rebuild();
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.Copies = Int16.Parse(this.tbCopies.Text);
            if (Properties.Settings.Default.StickerPrinter.Length != 0)
            {
                pd.PrinterSettings.PrinterName = Properties.Settings.Default.StickerPrinter;
            }
            try
            {
                using (PrintServer ps = new PrintServer())
                {
                    using (PrintQueue pq = new PrintQueue(ps, pd.PrinterSettings.PrinterName,
                          PrintSystemDesiredAccess.AdministratePrinter))
                    {
                        pq.Purge();
                    }
                }
            }catch(Exception ex)
            { MessageBox.Show(ex.Message); }

            rdlView.Print(pd);
        }
    }
}
