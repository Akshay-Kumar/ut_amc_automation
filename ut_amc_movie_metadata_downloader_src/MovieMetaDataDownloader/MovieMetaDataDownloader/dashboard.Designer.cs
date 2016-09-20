namespace MovieMetaDataDownloader
{
    partial class dashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDownload = new System.Windows.Forms.Button();
            this.movieList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(570, 226);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 0;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // movieList
            // 
            this.movieList.FormattingEnabled = true;
            this.movieList.Location = new System.Drawing.Point(12, 9);
            this.movieList.Name = "movieList";
            this.movieList.Size = new System.Drawing.Size(633, 199);
            this.movieList.TabIndex = 1;
            // 
            // dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 261);
            this.Controls.Add(this.movieList);
            this.Controls.Add(this.btnDownload);
            this.Name = "dashboard";
            this.Text = "dashboard";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ListBox movieList;
    }
}