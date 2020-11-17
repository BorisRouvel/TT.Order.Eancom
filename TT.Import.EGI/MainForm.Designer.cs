namespace TT.Import.EGI
{
    partial class MainForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.Close_BTN = new System.Windows.Forms.Button();
            this.MainForm_GBX = new System.Windows.Forms.GroupBox();
            this.MainForm_GBX.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(6, 19);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(333, 23);
            this.progressBar.TabIndex = 1;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // Close_BTN
            // 
            this.Close_BTN.Location = new System.Drawing.Point(137, 54);
            this.Close_BTN.Name = "Close_BTN";
            this.Close_BTN.Size = new System.Drawing.Size(75, 23);
            this.Close_BTN.TabIndex = 2;
            this.Close_BTN.Text = "Fermer";
            this.Close_BTN.UseVisualStyleBackColor = true;
            this.Close_BTN.Click += new System.EventHandler(this.Close_BTN_Click);
            // 
            // MainForm_GBX
            // 
            this.MainForm_GBX.Controls.Add(this.progressBar);
            this.MainForm_GBX.Controls.Add(this.Close_BTN);
            this.MainForm_GBX.Location = new System.Drawing.Point(12, 12);
            this.MainForm_GBX.Name = "MainForm_GBX";
            this.MainForm_GBX.Size = new System.Drawing.Size(345, 83);
            this.MainForm_GBX.TabIndex = 3;
            this.MainForm_GBX.TabStop = false;
            this.MainForm_GBX.Text = "Status";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 104);
            this.ControlBox = false;
            this.Controls.Add(this.MainForm_GBX);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import EGI";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MainForm_VisibleChanged);
            this.MainForm_GBX.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar;
        public System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button Close_BTN;
        private System.Windows.Forms.GroupBox MainForm_GBX;
    }
}