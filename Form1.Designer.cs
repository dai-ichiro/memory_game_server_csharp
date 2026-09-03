namespace WinForm
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnServer = new System.Windows.Forms.Button();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // btnServer
            //
            this.btnServer.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.btnServer.Location = new System.Drawing.Point(24, 24);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(150, 60);
            this.btnServer.TabIndex = 0;
            this.btnServer.Text = "start";
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            //
            // btnBrowser
            //
            this.btnBrowser.Enabled = false;
            this.btnBrowser.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.btnBrowser.Location = new System.Drawing.Point(190, 24);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(190, 60);
            this.btnBrowser.TabIndex = 1;
            this.btnBrowser.Text = "ブラウザで開く";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(26, 100);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 19);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "停止中";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 136);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.btnServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "メモリーゲーム サーバー";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnServer;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Label lblStatus;
    }
}
