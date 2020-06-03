namespace Ord_Eancom
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
            this.MainFrame = new System.Windows.Forms.GroupBox();
            this.ChoiceOrder_CHB = new System.Windows.Forms.CheckBox();
            this.ChoiceElevation_CHB = new System.Windows.Forms.CheckBox();
            this.ChoicePlan_CHB = new System.Windows.Forms.CheckBox();
            this.ChoiceEGI_CHB = new System.Windows.Forms.CheckBox();
            this.Ok_BTN = new System.Windows.Forms.Button();
            this.version_LNK = new System.Windows.Forms.LinkLabel();
            this.MainFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainFrame
            // 
            this.MainFrame.Controls.Add(this.ChoiceOrder_CHB);
            this.MainFrame.Controls.Add(this.ChoiceElevation_CHB);
            this.MainFrame.Controls.Add(this.ChoicePlan_CHB);
            this.MainFrame.Controls.Add(this.ChoiceEGI_CHB);
            this.MainFrame.Location = new System.Drawing.Point(12, 12);
            this.MainFrame.Name = "MainFrame";
            this.MainFrame.Size = new System.Drawing.Size(136, 146);
            this.MainFrame.TabIndex = 0;
            this.MainFrame.TabStop = false;
            // 
            // ChoiceOrder_CHB
            // 
            this.ChoiceOrder_CHB.AutoSize = true;
            this.ChoiceOrder_CHB.Checked = true;
            this.ChoiceOrder_CHB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChoiceOrder_CHB.Location = new System.Drawing.Point(6, 88);
            this.ChoiceOrder_CHB.Name = "ChoiceOrder_CHB";
            this.ChoiceOrder_CHB.Size = new System.Drawing.Size(79, 17);
            this.ChoiceOrder_CHB.TabIndex = 5;
            this.ChoiceOrder_CHB.Text = "Commande";
            this.ChoiceOrder_CHB.UseVisualStyleBackColor = true;
            this.ChoiceOrder_CHB.CheckedChanged += new System.EventHandler(this.ChoiceOrder_CHB_CheckedChanged);
            // 
            // ChoiceElevation_CHB
            // 
            this.ChoiceElevation_CHB.AutoSize = true;
            this.ChoiceElevation_CHB.Checked = true;
            this.ChoiceElevation_CHB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChoiceElevation_CHB.Location = new System.Drawing.Point(6, 65);
            this.ChoiceElevation_CHB.Name = "ChoiceElevation_CHB";
            this.ChoiceElevation_CHB.Size = new System.Drawing.Size(75, 17);
            this.ChoiceElevation_CHB.TabIndex = 4;
            this.ChoiceElevation_CHB.Text = "Elévations";
            this.ChoiceElevation_CHB.UseVisualStyleBackColor = true;
            this.ChoiceElevation_CHB.CheckedChanged += new System.EventHandler(this.ChoiceElevation_CHB_CheckedChanged);
            // 
            // ChoicePlan_CHB
            // 
            this.ChoicePlan_CHB.AutoSize = true;
            this.ChoicePlan_CHB.Checked = true;
            this.ChoicePlan_CHB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChoicePlan_CHB.Location = new System.Drawing.Point(6, 42);
            this.ChoicePlan_CHB.Name = "ChoicePlan_CHB";
            this.ChoicePlan_CHB.Size = new System.Drawing.Size(47, 17);
            this.ChoicePlan_CHB.TabIndex = 3;
            this.ChoicePlan_CHB.Text = "Plan";
            this.ChoicePlan_CHB.UseVisualStyleBackColor = true;
            this.ChoicePlan_CHB.CheckedChanged += new System.EventHandler(this.ChoicePlan_CHB_CheckedChanged);
            // 
            // ChoiceEGI_CHB
            // 
            this.ChoiceEGI_CHB.AutoSize = true;
            this.ChoiceEGI_CHB.Enabled = false;
            this.ChoiceEGI_CHB.Location = new System.Drawing.Point(6, 19);
            this.ChoiceEGI_CHB.Name = "ChoiceEGI_CHB";
            this.ChoiceEGI_CHB.Size = new System.Drawing.Size(44, 17);
            this.ChoiceEGI_CHB.TabIndex = 2;
            this.ChoiceEGI_CHB.Text = "EGI";
            this.ChoiceEGI_CHB.UseVisualStyleBackColor = true;
            this.ChoiceEGI_CHB.CheckedChanged += new System.EventHandler(this.ChoiceEGI_CHB_CheckedChanged);
            // 
            // Ok_BTN
            // 
            this.Ok_BTN.Location = new System.Drawing.Point(43, 164);
            this.Ok_BTN.Name = "Ok_BTN";
            this.Ok_BTN.Size = new System.Drawing.Size(75, 23);
            this.Ok_BTN.TabIndex = 1;
            this.Ok_BTN.Text = "Ok";
            this.Ok_BTN.UseVisualStyleBackColor = true;
            this.Ok_BTN.Click += new System.EventHandler(this.Ok_BTN_Click);
            // 
            // version_LNK
            // 
            this.version_LNK.AutoSize = true;
            this.version_LNK.Location = new System.Drawing.Point(9, 175);
            this.version_LNK.Name = "version_LNK";
            this.version_LNK.Size = new System.Drawing.Size(13, 13);
            this.version_LNK.TabIndex = 6;
            this.version_LNK.TabStop = true;
            this.version_LNK.Text = "?";
            this.version_LNK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.version_LNK_LinkClicked);
            // 
            // MainForm
            // 
            this.AcceptButton = this.Ok_BTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(158, 197);
            this.ControlBox = false;
            this.Controls.Add(this.version_LNK);
            this.Controls.Add(this.Ok_BTN);
            this.Controls.Add(this.MainFrame);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commande";
            this.MainFrame.ResumeLayout(false);
            this.MainFrame.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox MainFrame;
        private System.Windows.Forms.CheckBox ChoiceOrder_CHB;
        private System.Windows.Forms.CheckBox ChoiceElevation_CHB;
        private System.Windows.Forms.CheckBox ChoicePlan_CHB;
        private System.Windows.Forms.CheckBox ChoiceEGI_CHB;
        private System.Windows.Forms.Button Ok_BTN;
        private System.Windows.Forms.LinkLabel version_LNK;
    }
}