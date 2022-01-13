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

using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace Smart_Temperature_Monitoring
{
    public partial class sfrmData1 : Form
    {
        //  Global varriable
        private static DataTable _pGet_24hr_value = new DataTable();

        public sfrmData1()
        {
            InitializeComponent();
            h24Temp();

            switch (sfrmOverview._selectedData)
            {
                case 1: cbbSelectedData.Text = "TEMP ZONE 1"; break;
                case 2: cbbSelectedData.Text = "TEMP ZONE 2"; break;
                case 3: cbbSelectedData.Text = "TEMP ZONE 3"; break;
                default: cbbSelectedData.Text = ""; break;
            }
            
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

        private void h24Temp()
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        {
            _pGet_24hr_value = new DataTable();
            _pGet_24hr_value = pGet_24hr_value();
            if (_pGet_24hr_value != null)
            {
                var values1 = new ChartValues<double>();
                var values2 = new ChartValues<double>();
                var values3 = new ChartValues<double>();
                var valuesX = new ChartValues<DateTime>();
                for (var i = 0; i < 24; i++)
                {
                    values1.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp1"]));
                    values2.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp2"]));
                    values3.Add(Convert.ToDouble(_pGet_24hr_value.Rows[i]["temp3"]));

                    //valuesX.Add((_pGet_24hr_value.Rows[i]["create_datetime"]));
                }

                cartesianChart1.Series.Add(new LineSeries
                {
                    Values = values1
                });

                cartesianChart1.AxisX.Add(new Axis
                {
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm"),
                    MinValue = 0,
                    MaxValue = 23
                });

            }
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

    }
}
