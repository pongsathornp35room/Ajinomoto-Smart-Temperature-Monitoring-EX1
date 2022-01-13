using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static Smart_Temperature_Monitoring.InterfaceDB;
using Microsoft.Data.SqlClient;

using LiveCharts; //Core of the library
using LiveCharts.Wpf; //The WPF controls
using LiveCharts.WinForms; //the WinForm wrappers
using System.Net;
using System.IO;
using System.Threading;

namespace Smart_Temperature_Monitoring
{
    public partial class sfrmOverview : Form
    {
        //  Global varriable
        public static string _selectedExport = "";
        public static string _selectedEvent = "";
        public static int _selectedData = 0;
        public static int _selectedSetting = 0;

        //  Local varriable
        private static DataTable _pGet_actual_value = new DataTable();
        private static DataTable _pGet_setting = new DataTable();
        private static DataTable _pGet_24hr_value = new DataTable();

        //  Form load
        public sfrmOverview()
        {
            InitializeComponent();
        }
        private void sfrmOverview_Load(object sender, EventArgs e)
        {
            //  Intial
            initH24Temp();

            //  สร้าง Thread threadUpdateTime
            Thread threadUpdateTime = new Thread(ThreadUpdateTime);
            threadUpdateTime.IsBackground = true;
            threadUpdateTime.Start();
        }

        //  Thread Portion
        private void ThreadUpdateTime()
        {

            while (true)
            {
                _pGet_24hr_value = new DataTable();
                _pGet_24hr_value = pGet_24hr_value();
                if (_pGet_24hr_value != null)
                {
                    //  update line
                    chTemp1.Series[0].Values.Clear();
                    chTemp2.Series[0].Values.Clear();
                    chTemp3.Series[0].Values.Clear();

                    var values1 = new ChartValues<double>();
                    var values2 = new ChartValues<double>();
                    var values3 = new ChartValues<double>();
                    for (var i = 0; i < 24; i++)
                    {
                        chTemp1.Series[0].Values.Add(Convert.ToDouble((_pGet_24hr_value.Rows[i]["temp1"])));
                        chTemp2.Series[0].Values.Add(Convert.ToDouble((_pGet_24hr_value.Rows[i]["temp2"])));
                        chTemp3.Series[0].Values.Add(Convert.ToDouble((_pGet_24hr_value.Rows[i]["temp3"])));
                    }

                    //  update actual

                }

                //  Delay
                Thread.Sleep(1000);
            }
        }

        //  Display function
        private void actualTemp()
        {
            _pGet_actual_value = new DataTable();
            _pGet_actual_value = pGet_actual_value();
            if (_pGet_actual_value != null)
            {
                lbValue1.Text = _pGet_actual_value.Rows[0]["temp1"].ToString();
                lbValue2.Text = _pGet_actual_value.Rows[0]["temp2"].ToString();
                lbValue3.Text = _pGet_actual_value.Rows[0]["temp3"].ToString();
            }
        }
        private void limitTemp()
        {
            _pGet_setting = new DataTable();
            _pGet_setting = pGet_setting();
            if (_pGet_actual_value != null)
            {
                lbZone1.Text = _pGet_setting.Rows[0]["zone_name"].ToString();
                lbLow1.Text = _pGet_setting.Rows[0]["limit_low"].ToString();
                lbHigh1.Text = _pGet_setting.Rows[0]["limit_hi"].ToString();

                lbZone2.Text = _pGet_setting.Rows[1]["zone_name"].ToString();
                lbLow2.Text = _pGet_setting.Rows[1]["limit_low"].ToString();
                lbHigh2.Text = _pGet_setting.Rows[1]["limit_hi"].ToString();

                lbZone3.Text = _pGet_setting.Rows[2]["zone_name"].ToString();
                lbLow3.Text = _pGet_setting.Rows[2]["limit_low"].ToString();
                lbHigh3.Text = _pGet_setting.Rows[2]["limit_hi"].ToString();

                //gvLimit.DataSource = _pGet_setting;
            }
        }
        private void initH24Temp()
        {
            _pGet_24hr_value = new DataTable();
            _pGet_24hr_value = pGet_24hr_value();
            if (_pGet_24hr_value != null)
            {
                var values1 = new ChartValues<double>();
                var values2 = new ChartValues<double>();
                var values3 = new ChartValues<double>();
                for (var i = 0; i < 24; i++)
                {
                    values1.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp1"]));
                    values2.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp2"]));
                    values3.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp3"]));
                }

                chTemp1.Series.Add(new LineSeries
                {
                    Values = values1
                });

                chTemp2.Series.Add(new LineSeries
                {
                    Values = values2
                });

                chTemp3.Series.Add(new LineSeries
                {
                    Values = values3
                });

                chTemp1.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 23
                });

                chTemp2.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 23
                });

                chTemp3.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 23
                });
            }
        }

        //  SQL interface section
        private static DataTable pGet_actual_value()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_actual_value", "DbSet", param);
                dataTable = ds.Tables[0];
            }
            catch (SqlException)
            {
                dataTable = null;
            }
            catch (Exception)
            {
                dataTable = null;
            }
            return dataTable;
        }
        private static DataTable pGet_setting()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_setting", "DbSet", param);
                dataTable = ds.Tables[0];
            }
            catch (SqlException)
            {
                dataTable = null;
            }
            catch (Exception)
            {
                dataTable = null;
            }
            return dataTable;
        }
        private static DataTable pGet_24hr_value()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_24hr_value", "DbSet", param);
                dataTable = ds.Tables[0];
            }
            catch (SqlException)
            {
                dataTable = null;
            }
            catch (Exception)
            {
                dataTable = null;
            }
            return dataTable;
        }

        //  Button event
        private void btnExport1_Click(object sender, EventArgs e)
        {
            _selectedExport = "A";
            sfrmExport1 sfrmExport1 = new sfrmExport1();
            sfrmExport1.Show();
        }
        private void btnData1_Click(object sender, EventArgs e)
        {
            _selectedData = 1;
            sfrmData1 sfrmData1 = new sfrmData1();
            sfrmData1.Show();
        }
        private void btnData2_Click(object sender, EventArgs e)
        {
            _selectedData = 2;
            sfrmData1 sfrmData1 = new sfrmData1();
            sfrmData1.Show();
        }
        private void btnData3_Click(object sender, EventArgs e)
        {
            _selectedData = 3;
            sfrmData1 sfrmData1 = new sfrmData1();
            sfrmData1.Show();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            actualTemp();
            limitTemp();
        }
        private void btnSetting1_Click(object sender, EventArgs e)
        {
            _selectedSetting = 1;
            sfrmSetting1 sfrmSetting1 = new sfrmSetting1();
            sfrmSetting1.Show();
        }
        private void btnSetting2_Click(object sender, EventArgs e)
        {
            _selectedSetting = 2;
            sfrmSetting1 sfrmSetting1 = new sfrmSetting1();
            sfrmSetting1.Show();
        }
        private void btnSetting3_Click(object sender, EventArgs e)
        {
            _selectedSetting = 3;
            sfrmSetting1 sfrmSetting1 = new sfrmSetting1();
            sfrmSetting1.Show();
        }

        //  Local function
        private void LineNotifyMsg(string lineToken, string message)
        {
            try
            {
                //  hj1TGTJOwYq8L78D2fYbhPKQhOAsgaG1KfJ1QRLa3Tb
                //message = System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8);
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", message);
                var data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + lineToken);
                var stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //  Label test line notify
        private void lbHigh_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            string zone = string.Empty;
            if (label.Name == "lbHigh1") { zone = "Zone A"; }
            else if (label.Name == "lbHigh2") { zone = "Zone B"; }
            else if (label.Name == "lbHigh3") { zone = "Zone C"; }

            string message = string.Format("\r\n------Temperature Over Notify !!!------\r\n" +
                "Datetime : {0}\r\n" +
                "{1} has value over than {2}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                , zone, label.Text);
            LineNotifyMsg("hj1TGTJOwYq8L78D2fYbhPKQhOAsgaG1KfJ1QRLa3Tb", message);
        }
        private void lbLow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            string zone = string.Empty;
            if (label.Name == "lbLow1") { zone = "Zone A"; }
            else if (label.Name == "lbLow2") { zone = "Zone B"; }
            else if (label.Name == "lbLow3") { zone = "Zone C"; }

            string message = string.Format("\r\n------Temperature Lower Notify !!!------\r\n" +
                "Datetime : {0}\r\n" +
                "{1} has value lower than {2}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                , zone, label.Text);
            LineNotifyMsg("hj1TGTJOwYq8L78D2fYbhPKQhOAsgaG1KfJ1QRLa3Tb", message);
        }
        private void lbValue_DoubleClick(object sender, EventArgs e)
        {
            var label = (Label)sender;
            string zone = string.Empty;
            if (label.Name == "lbValue1") { zone = "Zone A"; }
            else if (label.Name == "lbValue2") { zone = "Zone B"; }
            else if (label.Name == "lbValue3") { zone = "Zone C"; }

            string message = string.Format("\r\n------Temperature Back Notify !!!------\r\n" +
                "Datetime : {0}\r\n" +
                "{1} has value back to lenght at {2}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                , zone, label.Text);
            LineNotifyMsg("hj1TGTJOwYq8L78D2fYbhPKQhOAsgaG1KfJ1QRLa3Tb", message);
        }

        //  Label change
        private void lbValue_TextChanged(object sender, EventArgs e)
        {
            var label = (Label)sender;

            var isNumeric = double.TryParse(label.Text, out double n);
            if (isNumeric)
            {
                double value = 0.0; double upper = 0.0; double lower = 0.0;

                if (label.Name == "lbValue1")
                {
                    value = (double)Convert.ToDouble(lbValue1.Text);
                    upper = (double)Convert.ToDouble(lbHigh1.Text);
                    lower = (double)Convert.ToDouble(lbLow1.Text);
                    if (lower <= value && value <= upper)
                    {
                        panelMain1.BackColor = Color.FromArgb(128, 255, 128);
                        lbZone1.BackColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        panelMain1.BackColor = Color.FromArgb(255, 128, 128);
                        lbZone1.BackColor = Color.Red;
                    }
                }
                else if (label.Name == "lbValue2")
                {
                    value = (double)Convert.ToDouble(lbValue2.Text);
                    upper = (double)Convert.ToDouble(lbHigh2.Text);
                    lower = (double)Convert.ToDouble(lbLow2.Text);
                    if (lower <= value && value <= upper)
                    {
                        panelMain2.BackColor = Color.FromArgb(128, 255, 128);
                        lbZone2.BackColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        panelMain2.BackColor = Color.FromArgb(255, 128, 128);
                        lbZone2.BackColor = Color.Red;
                    }
                }
                else if (label.Name == "lbValue3")
                {
                    value = (double)Convert.ToDouble(lbValue3.Text);
                    upper = (double)Convert.ToDouble(lbHigh3.Text);
                    lower = (double)Convert.ToDouble(lbLow3.Text);
                    if (lower <= value && value <= upper)
                    {
                        panelMain3.BackColor = Color.FromArgb(128, 255, 128);
                        lbZone3.BackColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        panelMain3.BackColor = Color.FromArgb(255, 128, 128);
                        lbZone3.BackColor = Color.Red;
                    }
                }
            }



        }
    }
}
