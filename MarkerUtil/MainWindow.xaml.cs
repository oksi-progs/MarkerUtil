using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Data;
using System.Data.SqlServerCe;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.Common;
using Xceed.Wpf.Toolkit;
using System.IO;

namespace MarkerUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable dtGoods;
        public DetailPage detailPage;
        public DataRowView currentData;
        public AskDigitBox _adb = null;


        public MainWindow()
        {
            InitializeComponent();
            detailPage = new DetailPage();
            MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            MainFrame.Navigate(detailPage);
        }


        private bool ReadDatabaseCE()
        {
            SqlCeConnection _conn;
            try
            {
                _conn = new SqlCeConnection(Properties.Settings.Default.databaseConnectionString);
                _conn.Open();
            }catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка подключения к базе данных товаров. " + ex.Message);
                throw ex;
            }
            if (_conn.State == ConnectionState.Open)
            {
                SqlCeDataAdapter _adtr = new SqlCeDataAdapter("SELECT * FROM goods", _conn);
                dtGoods = new DataTable();
                try
                {
                    _adtr.Fill(dtGoods);
                }
                catch (DbException ex)
                {
                    if (ex.ErrorCode == -2146232060)
                    {
                        SqlCeCommand _comm = new SqlCeCommand(getGoodsSQL(), _conn);
                        _comm.ExecuteNonQuery();
                        _adtr.Fill(dtGoods);
                    }
                }
            }
            return (_conn.State == ConnectionState.Open);
        }

        private bool ReadDatabase()
        {
            SqlConnection _conn;
            try
            {
                _conn = new SqlConnection(Properties.Settings.Default.databaseConnectionString);
                _conn.Open();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка подключения к базе данных товаров. " + ex.Message);
                throw ex;
            }

            if (_conn.State == ConnectionState.Open)
            {

                SqlDataAdapter _adtr = new SqlDataAdapter("SELECT * FROM goods", _conn);
                dtGoods = new DataTable();
                try
                {
                    _adtr.Fill(dtGoods);
                }
                catch (DbException ex)
                {
                    if (ex.ErrorCode == -2146232060)
                    {
                        SqlCommand _comm = new SqlCommand(getGoodsSQL(), _conn);
                        _comm.ExecuteNonQuery();
                        _adtr.Fill(dtGoods);
                    }
                }
            }
            return (_conn.State == ConnectionState.Open);
        }

        private string getGoodsSQL()
        {
            return
               "CREATE TABLE [goods] (" +
               "[Id] int IDENTITY(1,1) NOT NULL" +
               ", [Name] nvarchar(200) NOT NULL" +
               ", [Label] nvarchar(150) NULL" +
               ", [Trademark] nvarchar(200) NOT NULL" +
               ", [Code] nvarchar(20) NOT NULL" +
               ", [Valid] int NOT NULL" +
               ", [Standart] nvarchar(50) NULL" +
               ", [Manufacturer] nvarchar(150) NULL" +
               ", [Content] nvarchar(25) NULL" +
               ", [Conditions] nvarchar(250) NULL" +
               ", [Calories] nvarchar(100) NULL" +
               ", [ProducedDate] datetime NULL" +
               ", [Consignment] nvarchar(25) NULL" +
               ", [UseBy] datetime NULL" +
               ", [Barcode1] nvarchar(5) NULL" +
               ", [Barcode2] nvarchar(3) NULL" +
               ", [Barcode3] nvarchar(5) NULL" +
               ", [Mask] nvarchar(50) NULL" +
               ");";
        }
        public void SetFontSize(UIElementCollection _controls, bool bUseInternals=false)
        {
            foreach (UIElement _element in _controls)
            {
                SetElementFontSize(_element, bUseInternals);
            }
        }
        public void SetElementFontSize(UIElement _element, bool bUseInternals = false)
        {
            double btnHeight = this.Height / 9;
            double _fs = (btnHeight - 20) * 3 / 4;
            if (_element is Button)
            {
                ((Button)_element).Height = btnHeight;
                ((Button)_element).FontSize = _fs;
            }
            else if (_element is Label)
            {
                ((Label)_element).Height = btnHeight;
                ((Label)_element).FontSize = _fs - 5;
            }
            else if (_element is TextBox)
            {
                ((TextBox)_element).Height = btnHeight;
                ((TextBox)_element).FontSize = _fs;
            }
            else if (_element is TimePicker)
            {
                ((TimePicker)_element).MinHeight = btnHeight;
                ((TimePicker)_element).FontSize = _fs;
            }
            else if (_element is ComboBox)
            {
                ((ComboBox)_element).Height = btnHeight;
                ((ComboBox)_element).FontSize = _fs;
            }
            else if (_element is Viewbox)
            {
                ((Viewbox)_element).Height = btnHeight;
//                SetElementFontSize(((Viewbox)_element).Child, true);
            }
            else if (_element is DockPanel)
            {
                ((DockPanel)_element).Height = btnHeight;
                SetFontSize(((DockPanel)_element).Children, true);
            }
            else if (_element is StackPanel && bUseInternals)
            {
                SetFontSize(((StackPanel)_element).Children);
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string filepath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Logo.png");
            if (File.Exists(filepath))
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(filepath, UriKind.Absolute);
                bi3.EndInit();
                imageLogo.Stretch = Stretch.Fill;
                imageLogo.Source = bi3;
            }
            bool bResult = false;
            if (Properties.Settings.Default.databaseType.ToUpper() == "EXPRESS")
                bResult = ReadDatabase();
            else if (Properties.Settings.Default.databaseType.ToUpper() == "COMPACT")
                bResult = ReadDatabaseCE();


            if (bResult)
                detailPage._dt = dtGoods;
            else
                detailPage._dt = BlankDatabase();

            this.labelCaption.FontSize = this.Height / 30;
            this.PreviewTouchDown += MainWindow_PreviewTouchDown;

        }

        private void MainWindow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (_adb != null && _adb.IsVisible && _adb.ActualWidth > 0)
            {

                TouchPoint _point = e.GetTouchPoint(_adb);
                if (_point.Action == TouchAction.Down)
                {
                    Point _pos = _point.Position;
                    if (_pos.X < 0 || _pos.Y < 0 || _pos.X > _adb.ActualWidth || _pos.Y > _adb.ActualHeight)
                    {
                        //_adb.Close();

                        IntPtr hWnd = _adb.GetWindowHandle();
                        if (hWnd.ToInt32() > 0)
                        {
                            bool ret = LowLevelUtils.CloseWindow(hWnd);
                            _adb = null;
                        }
                    }
                }
            }
        }

        private DataTable BlankDatabase()
        {
            dtGoods = new DataTable();
            DataColumn _dc = dtGoods.Columns.Add("Id",Type.GetType("Integer"));
            return dtGoods;
        }

        private void Event(object sender, EventArgs e) {
            if(_adb!=null && _adb.IsVisible && _adb.ActualWidth>0)
            {
                Point _pos = Mouse.GetPosition(_adb);
                if (_pos.X < 0 || _pos.Y < 0 || _pos.X>_adb.ActualWidth || _pos.Y>_adb.ActualHeight)
                {
                    IntPtr hWnd = _adb.GetWindowHandle();
                    if (hWnd.ToInt32() > 0) { 
                        bool ret = LowLevelUtils.CloseWindow(hWnd);
                        _adb = null;
                    }
                }
            }
        }
    }
    public static class LowLevelUtils
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static uint WM_CLOSE = 0x10;
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public static bool CloseWindow(IntPtr hWnd)
        {
            bool returnValue = PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            if (!returnValue)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return true;
        }
    }


}
