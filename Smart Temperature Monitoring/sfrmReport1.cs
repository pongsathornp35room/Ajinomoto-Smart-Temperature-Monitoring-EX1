using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using GemBox.Spreadsheet;
using Microsoft.Data.SqlClient;
using static Smart_Temperature_Monitoring.InterfaceDB;




namespace Smart_Temperature_Monitoring
{
    public partial class sfrmReport1 : Form
    {
        private static DataTable _pGet_Temp_data = new DataTable();
        private static DataTable skyscrapers = new DataTable();

        //private Spreadsheet spreadsheet;
        public sfrmReport1()
        {
            InitializeComponent();
            calenReport.SelectionRange.Start = calenReport.TodayDate;
            txtDateSelected.Text = calenReport.SelectionRange.Start.ToString("dd/MM/yyyy");
        }

        private static DataTable pGet_Temp_Range(DateTime selected_date, int sampling_min)
        {
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                //  อ่านค่าจาก Store pGet_actual_value 2022-01-15 00:00:00.000 pGet_Temp_data
                SqlParameterCollection param = new SqlCommand().Parameters;
                param.AddWithValue("@selected_date", SqlDbType.DateTime).Value = selected_date;
                param.AddWithValue("@sampling_min", SqlDbType.DateTime).Value = sampling_min;
                ds = new DBClass().SqlExcSto("pGet_data_for_report", "DbSet", param);
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            // If using Professional version, put your serial key below.
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            // Create new empty workbook.
            var workbook = new ExcelFile();

            // Add new sheet.
            var worksheet = workbook.Worksheets.Add("Daily Report");
            worksheet.DefaultRowHeight = 25 * 20;

            skyscrapers = new DataTable();
            skyscrapers = pGet_Temp_Range(calenReport.SelectionRange.Start, 60);

            // Load Excel workbook from file's path.
            //ExcelFile workbook = ExcelFile.Load("E:\\ENGINEERING\\2565\\65PRO-001_Ajinomoto_smart_temp_monitoring\\ProjectBackup\\Project\\Report\\REPORT_Templet.xlsx");
            //ExcelWorksheet worksheet = workbook.Worksheets[0];

            // Set title cells formatting.
            var title_style = new CellStyle();
            title_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            title_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            title_style.Font.Weight = ExcelFont.BoldWeight;
            title_style.Font.Size = 18 * 20;

            // Write title to Excel cell.
            worksheet.Cells.GetSubrange("A1:J1").Merged = true;
            worksheet.Cells["A1"].Value = "DAILY REPORT FOR TEMP. EX1";
            worksheet.Cells["A1"].Style = title_style;

            worksheet.Cells.GetSubrange("A2:J2").Merged = true;
            worksheet.Cells["A2"].Value = "Ajinomoto Co., (Thailand) Ltd. Nong Khae Factory";
            worksheet.Cells["A2"].Style = title_style;

            // Set footer cells formatting.
            var footer_style = new CellStyle();
            footer_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            footer_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            footer_style.Font.Color = SpreadsheetColor.FromArgb(128, 128, 128);

            worksheet.Cells.GetSubrange("A36:J36").Merged = true;
            worksheet.Cells["A36"].Value = "DESIGN BY OMRON ELECTRONICS CO., LTD. & ROOM INNOVATION CO., LTD.";
            worksheet.Cells["A36"].Style = footer_style;            

            // Set columns width.
            worksheet.Columns["A"].SetWidth(15, LengthUnit.ZeroCharacterWidth); 
            worksheet.Columns["B"].SetWidth(15, LengthUnit.ZeroCharacterWidth); 
            worksheet.Columns["C"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth); 
            worksheet.Columns["D"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["E"].SetWidth(15, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["F"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["G"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["H"].SetWidth(15, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["I"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth);
            worksheet.Columns["J"].SetWidth(7.5, LengthUnit.ZeroCharacterWidth);

            // Set date cells formatting.
            var date_style = new CellStyle();
            date_style.Font.Weight = ExcelFont.BoldWeight;
            date_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            date_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            date_style.NumberFormat = "dd/MM/yyyy";

            // Write date to Excel cells.
            worksheet.Cells["H3"].Value = "Date";
            worksheet.Cells["H3"].Style = date_style;
            worksheet.Cells["I3"].Value = skyscrapers.Rows[0]["datetime"];            
            worksheet.Cells["I3"].Style = date_style;

            // Write header data to Excel cells.
            worksheet.Cells["A4"].Value = "Time";
            worksheet.Cells["B4"].Value = "Temp. #1";
            worksheet.Cells["C4"].Value = "Limit";
            worksheet.Cells["C5"].Value = "High";
            worksheet.Cells["D5"].Value = "Low";
            worksheet.Cells["E4"].Value = "Temp. #2";
            worksheet.Cells["F4"].Value = "Limit";
            worksheet.Cells["F5"].Value = "High";
            worksheet.Cells["G5"].Value = "Low";            
            worksheet.Cells["H4"].Value = "Temp. #3";
            worksheet.Cells["I4"].Value = "Limit";
            worksheet.Cells["I5"].Value = "High";
            worksheet.Cells["J5"].Value = "Low";

            // Set header cells Merged
            worksheet.Cells.GetSubrange("I3:J3").Merged = true;  // Date
            worksheet.Cells.GetSubrange("A4:A5").Merged = true;  // Time
            worksheet.Cells.GetSubrange("B4:B5").Merged = true;  // Temp. #1
            worksheet.Cells.GetSubrange("C4:D4").Merged = true;  // Limit #1
            worksheet.Cells.GetSubrange("E4:E5").Merged = true;  // Temp. #2
            worksheet.Cells.GetSubrange("F4:G4").Merged = true;  // Limit #2
            worksheet.Cells.GetSubrange("H4:H5").Merged = true;  // Temp. #3
            worksheet.Cells.GetSubrange("I4:J4").Merged = true;  // Limit #3

            // Set header cells formatting.
            var header_style = new CellStyle();
            header_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            header_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            header_style.FillPattern.SetSolid(SpreadsheetColor.FromArgb(255, 129, 129));
            header_style.Font.Weight = ExcelFont.BoldWeight;
            header_style.Font.Color = SpreadsheetColor.FromName(ColorName.White);
            header_style.WrapText = true;
            header_style.Borders.SetBorders(MultipleBorders.Right | MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
            worksheet.Cells.GetSubrange("A4:J5").Style = header_style;                       

            // Set time data cells formatting.
            var time_style = new CellStyle();
            time_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            time_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            time_style.NumberFormat = "HH:mm";
            worksheet.Cells.GetSubrange("A6:A29").Style = time_style;

            // Set temp normal cells formatting.
            var temp_normal_style = new CellStyle();
            temp_normal_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            temp_normal_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            temp_normal_style.NumberFormat = "00.00";
            worksheet.Cells.GetSubrange("B6:B29").Style = temp_normal_style;
            worksheet.Cells.GetSubrange("E6:E29").Style = temp_normal_style;
            worksheet.Cells.GetSubrange("H6:H29").Style = temp_normal_style;

            // Set temp alarm cells formatting.
            var temp_alarm_style = new CellStyle();
            temp_alarm_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            temp_alarm_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            temp_alarm_style.NumberFormat = "00.00";
            temp_alarm_style.Font.Color = SpreadsheetColor.FromArgb(255, 129, 129);

            // Set limit data cells formatting.
            var limit_style = new CellStyle();
            limit_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            limit_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            limit_style.NumberFormat = "00.00";            
            worksheet.Cells.GetSubrange("C6:D29").Style = limit_style;
            worksheet.Cells.GetSubrange("F6:G29").Style = limit_style;
            worksheet.Cells.GetSubrange("I6:J29").Style = limit_style;

            // Set summary data cells formatting.
            var summary_style = new CellStyle();
            summary_style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            summary_style.VerticalAlignment = VerticalAlignmentStyle.Center;
            summary_style.NumberFormat = "00.00";
            worksheet.Cells.GetSubrange("A30:J32").Style = summary_style;

            // Write sample data and formatting to Excel cells.
            double daily_avg_temp1 = 0, daily_max_temp1 = 0, daily_min_temp1 = 100;
            double daily_avg_temp2 = 0, daily_max_temp2 = 0, daily_min_temp2 = 100;
            double daily_avg_temp3 = 0, daily_max_temp3 = 0, daily_min_temp3 = 100;
            for (int row = 0; row < skyscrapers.Rows.Count; row++)
            {
                worksheet.Cells[row + 5, 0].Value = skyscrapers.Rows[row]["datetime"];                

                worksheet.Cells[row + 5, 1].Value = skyscrapers.Rows[row]["avg_temp1"];
                if (skyscrapers.Rows[row]["temp1_result"].ToString() == "NG")
                    worksheet.Cells[row + 5, 1].Style = temp_alarm_style;                
                worksheet.Cells[row + 5, 2].Value = skyscrapers.Rows[row]["temp1_lo"];
                worksheet.Cells[row + 5, 3].Value = skyscrapers.Rows[row]["temp1_hi"];

                worksheet.Cells[row + 5, 4].Value = skyscrapers.Rows[row]["avg_temp2"];
                if (skyscrapers.Rows[row]["temp2_result"].ToString() == "NG")
                    worksheet.Cells[row + 5, 4].Style = temp_alarm_style;
                worksheet.Cells[row + 5, 5].Value = skyscrapers.Rows[row]["temp2_lo"];
                worksheet.Cells[row + 5, 6].Value = skyscrapers.Rows[row]["temp2_hi"];

                worksheet.Cells[row + 5, 7].Value = skyscrapers.Rows[row]["avg_temp3"];
                if (skyscrapers.Rows[row]["temp3_result"].ToString() == "NG")
                    worksheet.Cells[row + 5, 7].Style = temp_alarm_style;
                worksheet.Cells[row + 5, 8].Value = skyscrapers.Rows[row]["temp3_lo"];
                worksheet.Cells[row + 5, 9].Value = skyscrapers.Rows[row]["temp3_hi"];

                //Accumulate valve
                daily_avg_temp1 += Convert.ToDouble(skyscrapers.Rows[row]["avg_temp1"]);
                daily_avg_temp2 += Convert.ToDouble(skyscrapers.Rows[row]["avg_temp2"]);
                daily_avg_temp3 += Convert.ToDouble(skyscrapers.Rows[row]["avg_temp3"]);

                //Fine Max valve
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp1"]) > daily_max_temp1)
                    daily_max_temp1 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp1"]);
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp2"]) > daily_max_temp2)
                    daily_max_temp2 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp2"]);
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp3"]) > daily_max_temp3)
                    daily_max_temp3 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp3"]);

                //Fine Min valve
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp1"]) < daily_min_temp1)
                    daily_min_temp1 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp1"]);
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp2"]) < daily_min_temp2)
                    daily_min_temp2 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp2"]);
                if (Convert.ToDouble(skyscrapers.Rows[row]["avg_temp3"]) < daily_min_temp3)
                    daily_min_temp3 = Convert.ToDouble(skyscrapers.Rows[row]["avg_temp3"]);

            }
            //Fine Avg valve
            daily_avg_temp1 /= (skyscrapers.Rows.Count + 1);
            daily_avg_temp2 /= (skyscrapers.Rows.Count + 1);
            daily_avg_temp3 /= (skyscrapers.Rows.Count + 1);

            //Write summary data
            worksheet.Cells["A30"].Value = "Avarage";
            worksheet.Cells["A31"].Value = "Maximum";
            worksheet.Cells["A32"].Value = "Minimum";

            worksheet.Cells["B30"].Value = daily_avg_temp1;
            worksheet.Cells["B31"].Value = daily_max_temp1;
            worksheet.Cells["B32"].Value = daily_min_temp1;

            worksheet.Cells["E30"].Value = daily_avg_temp2;
            worksheet.Cells["E31"].Value = daily_max_temp2;
            worksheet.Cells["E32"].Value = daily_min_temp2;

            worksheet.Cells["H30"].Value = daily_avg_temp3;
            worksheet.Cells["H31"].Value = daily_max_temp3;
            worksheet.Cells["H32"].Value = daily_min_temp3;

            //Border
            worksheet.Cells.GetSubrange("A4", "J5").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

            worksheet.Cells.GetSubrange("A6", "J29").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

            worksheet.Cells.GetSubrange("C4", "D29").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("C4", "D4").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("B4", "D29").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

            worksheet.Cells.GetSubrange("F4", "G29").Style.Borders.SetBorders(
               MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("F4", "G4").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("E4", "G29").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

            worksheet.Cells.GetSubrange("I4", "J29").Style.Borders.SetBorders(
               MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("I4", "J4").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            worksheet.Cells.GetSubrange("H4", "J29").Style.Borders.SetBorders(
                MultipleBorders.Outside, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);


            worksheet.PrintOptions.FitWorksheetWidthToPages = 1;
            try
            {
                // Save workbook as an Excel file.
                string dt = calenReport.SelectionRange.Start.ToString("ddMMyyyy");
                workbook.Save("E:\\ENGINEERING\\2565\\65WIA-001_Ajinomoto_smart_temp_monitoring\\ProjectBackup\\Project\\Report\\REPORT_" + dt + ".xlsx");
                MessageBox.Show("Report was generated by user");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception error\n" + ex.Message);
            }
            
        }

        private void calenReport_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDateSelected.Text = calenReport.SelectionRange.Start.ToString("dd/MM/yyyy");
        }
    }
   
    
}
