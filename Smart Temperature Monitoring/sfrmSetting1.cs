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

namespace Smart_Temperature_Monitoring
{
    public partial class sfrmSetting1 : Form
    {
        //  Local varriable
        private static DataTable _pGet_setting = new DataTable();
        
        public sfrmSetting1()
        {
            InitializeComponent();
            switch (sfrmOverview._selectedSetting)
                {
                case 1: lbSettingNo.Text = "SYSTEM SETTING ZONE 1"; break;
                case 2: lbSettingNo.Text = "SYSTEM SETTING ZONE 2"; break;
                case 3: lbSettingNo.Text = "SYSTEM SETTING ZONE 3"; break;
                default: lbSettingNo.Text = ""; break;
            }
          
            limitTemp();
        }

        private void limitTemp()
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        {
            _pGet_setting = new DataTable();
            _pGet_setting = pGet_setting();
            if (_pGet_setting != null)
            {
                txtZone.Text = _pGet_setting.Rows[(sfrmOverview._selectedSetting) - 1]["zone_name"].ToString();
                txtLow.Text = _pGet_setting.Rows[(sfrmOverview._selectedSetting) - 1]["limit_low"].ToString();
                txtHigh.Text = _pGet_setting.Rows[(sfrmOverview._selectedSetting) - 1]["limit_hi"].ToString();

                //gvLimit.DataSource = _pGet_setting;
            }
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
    }
}
