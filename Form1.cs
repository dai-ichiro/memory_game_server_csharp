using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinForm
{
    public partial class Form1 : Form
    {
        private GameServer _server;

        public Form1()
        {
            InitializeComponent();
            UpdateUi();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            if (_server != null && _server.IsRunning)
                StopServer();
            else
                StartServer();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (_server == null || !_server.IsRunning) return;

            try
            {
                Process.Start(_server.Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "ブラウザの起動に失敗しました。" + Environment.NewLine + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartServer()
        {
            try
            {
                var server = new GameServer();
                server.Start(8080);
                _server = server;
            }
            catch (Exception ex)
            {
                _server = null;
                MessageBox.Show(this,
                    "サーバーの起動に失敗しました。" + Environment.NewLine + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateUi();
        }

        private void StopServer()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            UpdateUi();
        }

        private void UpdateUi()
        {
            bool running = _server != null && _server.IsRunning;
            btnServer.Text = running ? "stop" : "start";
            btnBrowser.Enabled = running;
            lblStatus.Text = running
                ? "起動中: " + _server.Url
                : "停止中";
        }

        /// <summary>
        /// 「閉じる」で終了するときにサーバーが起動していれば必ず停止させる。
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_server != null && _server.IsRunning)
            {
                _server.Stop();
                _server = null;
            }
            base.OnFormClosing(e);
        }
    }
}
