using LiveCharts; //Core of the library
using LiveCharts.Wpf; //The WPF controls
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static Smart_Temperature_Monitoring.InterfaceDB;
using Brushes = System.Windows.Media.Brushes;
using System.Collections.Generic;

namespace Smart_Temperature_Monitoring
{
    
    public partial class sfrmOverview : Form
    {        
        //  Global varriable
        public static string _selectedExport = "";
        public static string _selectedEvent = "";
        public static int _selectedData = 0;
        public static int _SettingId = 0;
        public static int _SettingZoneId = 0;
        public static int _Setting1 = 0;
        public static int _Setting2 = 0;
        public static int _Setting3 = 0;
        public static bool _newSetting = false;
        public static int _EventZoneId = 0;

        //  Local varriable
        private static DataTable _pGet_Temp_actual = new DataTable();
        private static DataTable _pGet_setting_actual = new DataTable();
        private static DataTable _pGet_Temp_data = new DataTable();
        private static DataTable _pGet_event_all = new DataTable();

        private static int actualTempID = 0;
        private static int actualGvRow = 0;

        //  Form load
        public sfrmOverview()
        {
            InitializeComponent();
        }
        private void sfrmOverview_Load(object sender, EventArgs e)
        {
            //  Intial
            initTempData();

            //Create Thread threadSamplingTime --> Update Actual temp. & Trend & Grid status
            Thread threadUpdateTrend = new Thread(ThreadUpdateTime);
            threadUpdateTrend.IsBackground = true;
            threadUpdateTrend.Start();

            //  Create Thread threadUpdateSetting --> Update setting
            Thread threadUpdateSetting = new Thread(ThreadUpdateSetting);
            threadUpdateSetting.IsBackground = true;
            threadUpdateSetting.Start();
        }


        //  Thread Portion
        private void ThreadUpdateTime()
        {
            _actualTemp();
            //_get_event_all();   

            //  Delay
            Thread.Sleep(300000);            
        }

        private void ThreadUpdateSetting()
        {
            _actual_setting();
            
            //  Delay
            Thread.Sleep(1000);        
        }

        //  Display function
        public void _actualTemp()
        {
            _pGet_Temp_actual = new DataTable();
            _pGet_Temp_actual = pGet_Temp_actual();
            if (_pGet_Temp_actual != null)
            {
                // actual temp
                lbValue1.Text = _pGet_Temp_actual.Rows[0]["temp1"].ToString();
                lbValue2.Text = _pGet_Temp_actual.Rows[0]["temp2"].ToString();
                lbValue3.Text = _pGet_Temp_actual.Rows[0]["temp3"].ToString();

                // Keep actual setting id
                _Setting1 = Convert.ToInt32(_pGet_Temp_actual.Rows[0]["ct_setting_t1"]);
                _Setting2 = Convert.ToInt32(_pGet_Temp_actual.Rows[0]["ct_setting_t2"]);
                _Setting3 = Convert.ToInt32(_pGet_Temp_actual.Rows[0]["ct_setting_t3"]);

                // Keep actual ID
                actualTempID = Convert.ToInt32(_pGet_Temp_actual.Rows[0]["ID"]);


                // change background coler 
                if ((_pGet_Temp_actual.Rows[0]["temp1_result"]).ToString() == "OK")
                {
                    panelMain1.BackColor = Color.FromArgb(128, 255, 128);
                    lbZone1.BackColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    panelMain1.BackColor = Color.FromArgb(255, 128, 128);
                    lbZone1.BackColor = Color.Red;
                }

                if ((_pGet_Temp_actual.Rows[0]["temp2_result"]).ToString() == "OK")
                {
                    panelMain2.BackColor = Color.FromArgb(128, 255, 128);
                    lbZone2.BackColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    panelMain2.BackColor = Color.FromArgb(255, 128, 128);
                    lbZone2.BackColor = Color.Red;
                }

                if ((_pGet_Temp_actual.Rows[0]["temp3_result"]).ToString() == "OK")
                {
                    panelMain3.BackColor = Color.FromArgb(128, 255, 128);
                    lbZone3.BackColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    panelMain3.BackColor = Color.FromArgb(255, 128, 128);
                    lbZone3.BackColor = Color.Red;
                }

                // plot graph
                chTemp1.Series[0].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp1"])));
                chTemp1.Series[1].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp1_hi"])));
                chTemp1.Series[2].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp1_lo"])));

                chTemp2.Series[0].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp2"])));
                chTemp2.Series[1].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp2_hi"])));
                chTemp2.Series[2].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp2_lo"])));

                chTemp3.Series[0].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp3"])));
                chTemp3.Series[1].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp3_hi"])));
                chTemp3.Series[2].Values.Add(Convert.ToDouble((_pGet_Temp_actual.Rows[0]["temp3_lo"])));

                // plot gv status                
                gvData1.ClearSelection();
                gvData2.ClearSelection();
                gvData3.ClearSelection();

                if ((_pGet_Temp_actual.Rows[0]["temp1_result"]).ToString() == "NG")
                {
                    gvData1.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(255, 128, 128);
                    gvData1.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(255, 128, 128);
                }
                else
                {
                    gvData1.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(128, 255, 128);
                    gvData1.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(128, 255, 128);
                }

                if ((_pGet_Temp_actual.Rows[0]["temp2_result"]).ToString() == "NG")
                {
                    gvData2.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(255, 128, 128);
                    gvData2.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(255, 128, 128);
                }
                else
                {
                    gvData2.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(128, 255, 128);
                    gvData2.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(128, 255, 128);
                }

                if ((_pGet_Temp_actual.Rows[0]["temp3_result"]).ToString() == "NG")
                {
                    gvData3.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(255, 128, 128);
                    gvData3.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(255, 128, 128);
                }
                else
                {
                    gvData3.Rows[0].Cells[actualGvRow].Style.BackColor = Color.FromArgb(128, 255, 128);
                    gvData3.Rows[0].Cells[actualGvRow].Style.ForeColor = Color.FromArgb(128, 255, 128);
                }
                actualGvRow += 1;
            }
        }

        public void _actual_setting()
        {
            _pGet_setting_actual = new DataTable();
            _pGet_setting_actual = pGet_setting_actual();
            if (_pGet_setting_actual != null)
            {
                lbZone1.Text = _pGet_setting_actual.Rows[0]["t1_name"].ToString();
                lbLow1.Text = _pGet_setting_actual.Rows[0]["temp1_lo"].ToString();
                lbHigh1.Text = _pGet_setting_actual.Rows[0]["temp1_hi"].ToString();

                lbZone2.Text = _pGet_setting_actual.Rows[0]["t2_name"].ToString();
                lbLow2.Text = _pGet_setting_actual.Rows[0]["temp2_lo"].ToString();
                lbHigh2.Text = _pGet_setting_actual.Rows[0]["temp2_hi"].ToString();

                lbZone3.Text = _pGet_setting_actual.Rows[0]["t3_name"].ToString();
                lbLow3.Text = _pGet_setting_actual.Rows[0]["temp3_lo"].ToString();
                lbHigh3.Text = _pGet_setting_actual.Rows[0]["temp3_hi"].ToString();
            }
        }

        public void _get_event_all()
        {
            _pGet_event_all = new DataTable();
            _pGet_event_all = pGet_event_all();
            if (_pGet_event_all != null)
            {
                //  Clear gv
                //gvEventAll.Columns.Clear();
                //gvEventAll.Rows.Clear();
                //gvEventAll.DataSource = null;
                gvEventAll.ClearSelection();

                gvEventAll.DataSource = _pGet_event_all;
            }
        }

        private void initTempData()
        {
            _pGet_Temp_data = new DataTable();
            _pGet_Temp_data = pGet_Temp_data();
            if (_pGet_Temp_data != null)
            {
                var values1 = new ChartValues<double>();
                var values2 = new ChartValues<double>();
                var values3 = new ChartValues<double>();

                var hi1 = new ChartValues<double>();
                var lo1 = new ChartValues<double>();

                var hi2 = new ChartValues<double>();
                var lo2 = new ChartValues<double>();

                var hi3 = new ChartValues<double>();
                var lo3 = new ChartValues<double>();


                // plot graph
                for (var i = 0; i < _pGet_Temp_data.Rows.Count; i++)
                {
                    values1.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp1"]));
                    hi1.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp1_hi"]));
                    lo1.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp1_lo"]));

                    values2.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp2"]));
                    hi2.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp2_hi"]));
                    lo2.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp2_lo"]));

                    values3.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp3"]));
                    hi3.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp3_hi"]));
                    lo3.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp3_lo"]));
                }

                chTemp1.Series.Add(new LineSeries
                {
                    Name = "Actual",
                    Values = values1,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp1.Series.Add(new LineSeries
                {
                    Name = "High",
                    Values = hi1,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp1.Series.Add(new LineSeries
                {
                    Name = "Low",
                    Values = lo1,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                    //Stroke = Brushes.LightSalmon

                });

                chTemp2.Series.Add(new LineSeries
                {
                    Values = values2,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp2.Series.Add(new LineSeries
                {
                    Values = hi2,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp2.Series.Add(new LineSeries
                {
                    Values = lo2,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                    //Stroke = Brushes.LightSalmon
                });

                chTemp3.Series.Add(new LineSeries
                {
                    Values = values3,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp3.Series.Add(new LineSeries
                {
                    Values = hi3,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                chTemp3.Series.Add(new LineSeries
                {
                    Values = lo3,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                    //Stroke = Brushes.LightSalmon
                });


                IList<string> labelX = new List<string>();
                for (int i = 0; i <= 287; i++)
                    labelX.Add(System.DateTime.MinValue.AddMinutes(i*5).ToString("HH:mm"));

                chTemp1.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 287,
                    Labels = labelX
                    //LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm")
                });

                chTemp1.AxisY.Add(new Axis
                {
                    MinValue = 25,
                    MaxValue = 40
                });

                chTemp2.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 287,
                    Labels = labelX,
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm")
                });

                chTemp2.AxisY.Add(new Axis
                {
                    MinValue = 25,
                    MaxValue = 40
                });

                chTemp3.AxisX.Add(new Axis
                {
                    MinValue = 0,
                    MaxValue = 287,
                    Labels = labelX,
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm")
                });

                chTemp3.AxisY.Add(new Axis
                {
                    MinValue = 25,
                    MaxValue = 40
                });


                // plot gv status

                //Declare array for keep data
                string[] status1 = new string[288];
                string[] status2 = new string[288];
                string[] status3 = new string[288];

                //  Clear gv
                //gvData1.Columns.Clear();
                //gvData1.Rows.Clear();
                //gvData1.DataSource = null;

                //Keep data to array
                for (int i = 0; i < _pGet_Temp_data.Rows.Count; i++)
                {
                    status1[i] = _pGet_Temp_data.Rows[i]["temp1_result"].ToString();
                    status2[i] = _pGet_Temp_data.Rows[i]["temp2_result"].ToString();
                    status3[i] = _pGet_Temp_data.Rows[i]["temp3_result"].ToString();
                }

                gvData1.Rows.Clear();
                gvData2.Rows.Clear();
                gvData3.Rows.Clear();

                //gvData1.ClearSelection();
                //gvData2.ClearSelection();
                //gvData3.ClearSelection();

                //Add array to DataGridView
                gvData1.Rows.Add(status1);
                gvData2.Rows.Add(status2);
                gvData3.Rows.Add(status3);

                //Manage DataGridView
                //gvData1.Rows[0].DefaultCellStyle.BackColor = Color.LightGray;
                //gvData1.Rows[0].DefaultCellStyle.ForeColor = Color.LightGray;

                //gvData2.Rows[0].DefaultCellStyle.BackColor = Color.LightGray;
                //gvData2.Rows[0].DefaultCellStyle.ForeColor = Color.LightGray;

                //gvData3.Rows[0].DefaultCellStyle.BackColor = Color.LightGray;
                //gvData3.Rows[0].DefaultCellStyle.ForeColor = Color.LightGray;
                for (int i = 0; i < _pGet_Temp_data.Rows.Count; i++)
                {
                    if (status1[i] == "NG")
                    {
                        gvData1.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(255, 128, 128);
                        gvData1.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(255, 128, 128);
                    }
                    else 
                    {
                        gvData1.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(128, 255, 128);
                        gvData1.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(128, 255, 128);
                    }

                    if (status2[i] == "NG")
                    {
                        gvData2.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(255, 128, 128);
                        gvData2.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(255, 128, 128);
                    }
                    else
                    {
                        gvData2.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(128, 255, 128);
                        gvData2.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(128, 255, 128);
                    }

                    if (status3[i] == "NG")
                    {
                        gvData3.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(255, 128, 128);
                        gvData3.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(255, 128, 128);
                    }
                    else
                    {
                        gvData3.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(128, 255, 128);
                        gvData3.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(128, 255, 128);
                    }

                    actualGvRow = _pGet_Temp_data.Rows.Count;
                }

                // Keep setting id
                if (_pGet_Temp_data.Rows.Count > 0)
                {
                    _Setting1 = Convert.ToInt32(_pGet_Temp_data.Rows[_pGet_Temp_data.Rows.Count - 1]["ct_setting_t1"]);
                    _Setting2 = Convert.ToInt32(_pGet_Temp_data.Rows[_pGet_Temp_data.Rows.Count - 1]["ct_setting_t2"]);
                    _Setting3 = Convert.ToInt32(_pGet_Temp_data.Rows[_pGet_Temp_data.Rows.Count - 1]["ct_setting_t3"]);

                    actualTempID = Convert.ToInt32(_pGet_Temp_data.Rows[_pGet_Temp_data.Rows.Count - 1]["ID"]);
                }

            }

        }

        //  SQL interface section
        private static DataTable pGet_Temp_actual()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_Temp_actual
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_Temp_actual", "DbSet", param);
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

        private static DataTable pGet_Temp_data()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value 2022-01-15 00:00:00.000 pGet_Temp_data
                SqlParameterCollection param = new SqlCommand().Parameters;
                //param.AddWithValue("@start_datetime", SqlDbType.DateTime).Value = "2022-01-13 00:00:00.000";
                //param.AddWithValue("@end_datetime", SqlDbType.DateTime).Value = "2022-01-13 01:00:00.000";
                ds = new DBClass().SqlExcSto("pGet_Temp_data", "DbSet", param);
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

        private static DataTable pGet_setting_actual()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_setting_actual", "DbSet", param);
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

        private static DataTable pGet_event_all()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_event_all", "DbSet", param);
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
            sfrmReport1 sfrmExport1 = new sfrmReport1();
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
            //actualTemp();
            //limitTemp();
        }
        private void btnSetting1_Click(object sender, EventArgs e)
        {
            _SettingId = _Setting1;
            _SettingZoneId = 1;
            sfrmSetting1 sfrmSetting1 = new sfrmSetting1();
            sfrmSetting1.Show();
        }
        private void btnSetting2_Click(object sender, EventArgs e)
        {
            _SettingId = _Setting2;
            _SettingZoneId = 2;
            sfrmSetting1 sfrmSetting1 = new sfrmSetting1();
            sfrmSetting1.Show();
        }
        private void btnSetting3_Click(object sender, EventArgs e)
        {
            _SettingId = _Setting3;
            _SettingZoneId = 3;
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

        }

        private void btnEven1_Click(object sender, EventArgs e)
        {
            _EventZoneId = 1;
            sfrmEvent1 sfrmEvent1 = new sfrmEvent1();
            sfrmEvent1.Show();
        }

        private void btnEven2_Click(object sender, EventArgs e)
        {
            _EventZoneId = 2;
            sfrmEvent1 sfrmEvent1 = new sfrmEvent1();
            sfrmEvent1.Show();
        }

        private void btnEven3_Click(object sender, EventArgs e)
        {
            _EventZoneId = 3;
            sfrmEvent1 sfrmEvent1 = new sfrmEvent1();
            sfrmEvent1.Show();
        }
    }
}
