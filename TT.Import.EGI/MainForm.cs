using System;
using System.ComponentModel;

using System.Windows.Forms;

namespace TT.Import.EGI
{
    public partial class MainForm : Form
    {
        private Plugin _plugin = null;       
        private int _totalNb = 0;
        private string _statusText = String.Empty;

        public MainForm(Plugin plugin)
        {
            InitializeComponent();
            
            _plugin = plugin;
        }
     
        private void Initialize()
        {
            MainForm_GBX.Text = String.Empty;
            progressBar.Maximum = 0;
            progressBar.Value = 0;
            this.Refresh();
        }

        public void SetProgressBar(string statusText, int TotalNb)
        {
            _statusText = statusText;
            _totalNb = TotalNb;          
            backgroundWorker.ReportProgress(1);
        }       

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Initialize();
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = true;
            Close_BTN.Enabled = false;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {           
            BackgroundWorker worker = new BackgroundWorker();            
            e.Result = _plugin.Execute(worker, e);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!backgroundWorker.CancellationPending)
            {
                MainForm_GBX.Text = _statusText;
                progressBar.Maximum = _totalNb;
                progressBar.Increment(1);
                progressBar.Refresh();
            }
            else
            {
                this.Close();
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close_BTN.Enabled = true;
            //DialogResult dialog = System.Windows.Forms.MessageBox.Show("Terminé.", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        private void Close_BTN_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            this.Close();
        }
    }
}
