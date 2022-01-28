using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace Smart_Temperature_Monitoring
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 148, 241), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));


            //cartesianChart1.Series.Add(new LineSeries
            //{
            //    Values = GetData(),
            //    Fill = gradientBrush,
            //    StrokeThickness = 1,
            //    PointGeometry = null
            //});

            //cartesianChart1.Zoom = ZoomingOptions.X;

            //cartesianChart1.AxisX.Add(new Axis
            //{
            //    MinValue = 00,
            //    MaxValue = 287
                //LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm"),
                
            //});

            IList<string> labelX = new List<string>();
            for (int i = 0; i<=12; i++ )
                labelX.Add(System.DateTime.MinValue.AddHours(i*2).ToString("HH:mm"));

            cartesianChart1.AxisX.Add(new Axis
            {
                MinValue = 0,
                MaxValue = 12,
                Labels = labelX,              
                LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm")
            });



            //cartesianChart1.AxisY.Add(new Axis
            //{
            //    LabelFormatter = val => val.ToString("C")
            //});
        }
        private ChartValues<DateTimePoint> GetData()
        {
            //var r = new Random();
            //var trend = 24;
            var values = new ChartValues<DateTimePoint>();
            //values.Add(new DateTimePoint(System.DateTime.MinValue, trend));
            for (var i = 0; i < 24; i++)
            {
                //var seed = r.NextDouble();
                //if (seed > .8) trend += seed > .9 ? 50 : -50;
                values.Add(new DateTimePoint(System.DateTime.MinValue.AddHours(i), i));
            }
            return values;
        }
    }
}
