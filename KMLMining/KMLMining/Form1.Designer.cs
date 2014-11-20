namespace KMLMining
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Mining = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Mining
            // 
            this.btn_Mining.Location = new System.Drawing.Point(283, 299);
            this.btn_Mining.Name = "btn_Mining";
            this.btn_Mining.Size = new System.Drawing.Size(75, 23);
            this.btn_Mining.TabIndex = 0;
            this.btn_Mining.Text = "挖掘";
            this.btn_Mining.UseVisualStyleBackColor = true;
            this.btn_Mining.Click += new System.EventHandler(this.btn_Mining_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 386);
            this.Controls.Add(this.btn_Mining);
            this.Name = "Form1";
            this.Text = "KML挖掘";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Mining;
    }
}

