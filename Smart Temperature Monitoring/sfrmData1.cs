using LiveCharts; //Core of the library
using LiveCharts.Wpf; //The WPF controls
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using static Smart_Temperature_Monitoring.InterfaceDB;
using Brushes = System.Windows.Media.Brushes;


namespace Smart_Temperature_Monitoring
{
    public partial class sfrmData1 : Form
    {
        //  Global varriable
        private static DataTable _pGet_Temp_data = new DataTable();
        private static DataTable _pGet_zone_name = new DataTable();
        public sfrmData1()
        {
            InitializeComponent();
            initTempData();

            get_zone_name();

            label3.Text = dtDateFrom.Value.Date.ToString();
            label5.Text = dtDateTo.Value.Date.ToString();
            label6.Text = dtDateFrom.Value.ToString();

            /*
            switch (sfrmOverview._selectedData)
            {
                case 1: cbbSelectedData.Text = "TEMP ZONE 1"; break;
                case 2: cbbSelectedData.Text = "TEMP ZONE 2"; break;
                case 3: cbbSelectedData.Text = "TEMP ZONE 3"; break;
                default: cbbSelectedData.Text = ""; break;
            }
            */

            /*
            cartesianChart1.AxisX.Add(new Axis
            {
                LabelFormatter = val => new System.DateTime((long)val).ToString("dd MMM")
            });
            
            cartesianChart1.AxisY.Add(new Axis
            {
                LabelFormatter = val => val.ToString("C")
            });
            */
        }

        private void initTempData()
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        {
            _pGet_Temp_data = new DataTable();
            _pGet_Temp_data = pGet_Temp_Range(dtDateFrom.Value.Date, dtDateTo.Value);
            if (_pGet_Temp_data != null)
            {
                var values1 = new ChartValues<double>();
                for (var i = 0; i < _pGet_Temp_data.Rows.Count; i++)
                {
                    values1.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp1"]));
                }

                cartesianChart1.Series.Add(new LineSeries
                {
                    Values = values1
                });

                cartesianChart1.AxisX.Add(new Axis
                {
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm"),
                    MinValue = 0,
                    MaxValue = _pGet_Temp_data.Rows.Count
                });

            }
        }

        private void get_zone_name()
        {
            _pGet_zone_name = new DataTable();
            _pGet_zone_name = pGet_zone_name();
            if (_pGet_zone_name != null)
            {
                cbbSelectedData.DisplayMember = "zone_name";
                cbbSelectedData.ValueMember = "ID";
                cbbSelectedData.DataSource = _pGet_zone_name;
            }
        }
        // Export DataTable into an excel file with field names in the header line
        // - Save excel file without ever making it visible if filepath is given
        // - Don't save excel file, just make it visible if no filepath is given
        private static void CreateCSVFile(ref DataTable dt, string strFilePath)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                int iColCount = dt.Columns.Count;
                for (int i = 0; i < iColCount; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static DataTable pGet_Temp_Range(DateTime date_from, DateTime date_to)
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value 2022-01-15 00:00:00.000 pGet_Temp_data
                SqlParameterCollection param = new SqlCommand().Parameters;
                param.AddWithValue("@start_datetime", SqlDbType.DateTime).Value = date_from;
                param.AddWithValue("@end_datetime", SqlDbType.DateTime).Value = date_to;
                ds = new DBClass().SqlExcSto("pGet_Temp_Range", "DbSet", param);
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

        private static DataTable pGet_zone_name()
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_zone_name
                SqlParameterCollection param = new SqlCommand().Parameters;
                ds = new DBClass().SqlExcSto("pGet_zone_name", "DbSet", param);
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

        private void btPrevious_Click(object sender, EventArgs e)
        {
            cartesianChart1.AxisX[0].MinValue -= 24;
            cartesianChart1.AxisX[0].MaxValue -= 24;
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            cartesianChart1.AxisX[0].MinValue += 24;
            cartesianChart1.AxisX[0].MaxValue += 24;
        }

        private void btZoom_Click(object sender, EventArgs e)
        {
            cartesianChart1.AxisX[0].MinValue = 5;
            cartesianChart1.AxisX[0].MaxValue = 10;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _pGet_Temp_data = new DataTable();
            _pGet_Temp_data = pGet_Temp_Range(dtDateFrom.Value, dtDateTo.Value);

            //Clrar chart
            cartesianChart1.Series.Clear();
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisY.Clear();

            if (_pGet_Temp_data != null)
            {
                cartesianChart1.Series.Clear();
                var values1 = new ChartValues<double>();
                for (var i = 0; i < _pGet_Temp_data.Rows.Count; i++)
                {
                    values1.Add(Convert.ToDouble(_pGet_Temp_data.Rows[i]["temp1"]));
                }

                cartesianChart1.Series.Add(new LineSeries
                {
                    Values = values1,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0,
                    StrokeThickness = 2
                });

                cartesianChart1.AxisX.Add(new Axis
                {
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm"),
                    MinValue = 0,
                    MaxValue = _pGet_Temp_data.Rows.Count
                });

            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string filePath;
            //เขียนเงื่อนไขว่ามี path นี้หรือป่าว (exist folder) ภ้าไม่มีให้สร้าง floder นี้ขึ้นมาแล้ว บันทึก
            // เอาชื่อ path ไปวไว้ที่ app.config
            string dt = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            filePath = "C:\\Users\\TEMP_EX1_" + dt + ".csv";
            CreateCSVFile(ref _pGet_Temp_data, filePath);
        }
    }
}
