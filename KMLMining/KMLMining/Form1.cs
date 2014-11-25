using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMLMining
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        void Init()
        {
            LoadWorker();
            txt_URL.Text = GetCurrentUrl();
        }
        /// <summary>
        /// 将爬取的最后一条信息保存到app.config中
        /// </summary>
        /// <param name="url"></param>
        void SaveCurrnetUrl(string url)
        {
            File.WriteAllText(CommonHelper.GetFilePath("app.config"), url);
        }
        /// <summary>
        /// 从app.config中读取爬取的最后一条记录
        /// </summary>
        /// <returns></returns>
        string GetCurrentUrl()
        {
            if (File.Exists(CommonHelper.GetFilePath("app.config")))
                return File.ReadAllText(CommonHelper.GetFilePath("app.config"));
            return string.Empty;
        }

        /// <summary>
        /// 初始化后台工作
        /// </summary>
        void LoadWorker()
        {
            WorkerHelper.BgWorker = new BackgroundWorker();
            WorkerHelper.BgWorker.ProgressChanged += Worker_ProgressChanged;
            WorkerHelper.BgWorker.WorkerReportsProgress = true;//后台报告进度
            WorkerHelper.BgWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            WorkerHelper.BgWorker.WorkerSupportsCancellation = true;//支持取消
        }

        private void Worker_DoURLWorker(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument.ToString();
            MiningHelper.DoMining(url);
        }

        private void Worker_DoRandomWorker(object sender, DoWorkEventArgs e)
        {
            MiningHelper.RandomMining();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lb_Mining.Text = e.ProgressPercentage + "个";
            if (MiningHelper.CurrentCharacter == null || string.IsNullOrWhiteSpace(MiningHelper.CurrentCharacter.ID))
                return;
            pb_img.ImageLocation = MiningHelper.CurrentCharacter.PhotoPath;
            lb_name.Text = MiningHelper.CurrentCharacter.Point.Name;
            lb_Lat.Text = MiningHelper.CurrentCharacter.Point.Lat.ToString();
            lb_Lng.Text = MiningHelper.CurrentCharacter.Point.Lng.ToString();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
                CommonHelper.ShowMessageBox("图片经纬度挖掘被取消！");
            else
            {
                CommonHelper.ShowMessageBox("图片经纬度挖掘完成！");
            }
            SaveCurrnetUrl(MiningHelper.CurrentCharacter.Url);
        }

        private void btn_Mining_Click(object sender, EventArgs e)
        {
            var url = txt_URL.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                CommonHelper.ShowMessageBox("url不能为空");
                return;
            }
            WorkerHelper.BgWorker.DoWork += Worker_DoURLWorker;
            WorkerHelper.BgWorker.DoWork -= Worker_DoRandomWorker;
            MiningHelper.Count = 0;
            WorkerHelper.BgWorker.RunWorkerAsync(url);
        }

        private void btn_Pause_Click(object sender, EventArgs e)
        {
            if (btn_Pause.Text == "暂停")
            {
                MiningHelper.manualReset.Reset();
                btn_Pause.Text = "继续";
            }
            else
            {
                MiningHelper.manualReset.Set();
                btn_Pause.Text = "暂停";
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            WorkerHelper.BgWorker.CancelAsync();
        }

        private void btn_RandomMining_Click(object sender, EventArgs e)
        {
            WorkerHelper.BgWorker.DoWork -= Worker_DoURLWorker;
            WorkerHelper.BgWorker.DoWork += Worker_DoRandomWorker;
            WorkerHelper.BgWorker.RunWorkerAsync();
        }

        
    }
}
