using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace FTP
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }
        struct ftpsetting
        {
            public string Filename { get; set; }
            public string Fullname { get; set; }

        }
        ftpsetting _inputPara;

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = ((ftpsetting)e.Argument).Filename;
            string fullName = ((ftpsetting)e.Argument).Fullname;
            string ftpaddres = "192.168.0.199";
            string ftpuser = "admin";
            string ftppassword = "123456";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}",ftpaddres,fileName)));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpuser,ftppassword);
            Stream ftpStream = request.GetRequestStream();
            FileStream fs = File.OpenRead(fullName);
            byte[] buffer = new byte[1024];
            double total = (double)fs.Length;
            int byteRead = 0;
            double read = 0;
            do
            {
                if (!backgroundWorker.CancellationPending)
                {
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpStream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
                    backgroundWorker.ReportProgress((int)percentage);
                }
            }
            while (byteRead != 0);
            fs.Close();
            ftpStream.Close();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.lblStatus.Text = $"Uploaded{e.ProgressPercentage}%";
            this.progressBarControl.EditValue = e.ProgressPercentage;
            this.progressBarControl.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.lblStatus.Text = "Upload Complete!";
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "All file | *.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(ofd.FileName);
                    _inputPara.Filename = fi.Name;
                    _inputPara.Filename = fi.FullName;
                }
            }
        }
    }
}
