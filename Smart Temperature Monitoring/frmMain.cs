using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Smart_Temperature_Monitoring
{
    public partial class frmMain : Form
    {
        //  Global varriables
        private Form _activeForm = null;
        Color[] _blinkBgTimeColor = { Color.FromArgb(37, 37, 38), Color.FromArgb(186, 218, 85) };

        //  Form load
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            //  Thread GUI-link Config
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            //  สร้าง Thread threadUpdateTime
            Thread threadUpdateTime = new Thread(ThreadUpdateTime);
            threadUpdateTime.IsBackground = true;
            threadUpdateTime.Start();            

            //  Open main child form
            openChildForm(new sfrmOverview());
        }
        //  Thread Portion
        private void ThreadUpdateTime()
        {

            while (true)
            {
                DateTime dt = DateTime.Now;
                BtnCurrentTime.Text = dt.ToString("HH:mm:ss");
                LbDate.Text = dt.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

                //  Blink LbTimeBlink
                if (LbTimeBlink.BackColor == _blinkBgTimeColor[0])
                    LbTimeBlink.BackColor = _blinkBgTimeColor[1];

                else
                    LbTimeBlink.BackColor = _blinkBgTimeColor[0];

                //  Delay
                Thread.Sleep(1000);
            }
        }        

        //  Button event
        private void btn_back_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.ExitThread();
            System.Environment.Exit(0);
        }
        private void btn_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //  Subform section
        internal void SelectSubAdminMenu(Form childForm)
        {
            openChildForm(childForm);
        }
        private void openChildForm(Form childForm)
        {
            if (_activeForm != null) _activeForm.Close();
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
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

    }
}
