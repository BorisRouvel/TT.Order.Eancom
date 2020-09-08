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
            this.ChoicePerspective_CHB = new System.Windows.Forms.CheckBox();
            this.ChoiceOrder_CHB = new System.Windows.Forms.CheckBox();
            this.ChoiceElevation_CHB = new System.Windows.Forms.CheckBox();
            this.ChoicePlan_CHB = new System.Windows.Forms.CheckBox();
            this.ChoiceEGI_CHB = new System.Windows.Forms.CheckBox();
            this.Ok_BTN = new System.Windows.Forms.Button();
            this.version_LNK = new System.Windows.Forms.LinkLabel();
            this.Delivery_GBX = new System.Windows.Forms.GroupBox();
            this.CustomerDelivery_RBN = new System.Windows.Forms.RadioButton();
            this.RetaillerDelivery_RBN = new System.Windows.Forms.RadioButton();
            this.AdressEmail_GBX = new System.Windows.Forms.GroupBox();
            this.EmailCc_LAB = new System.Windows.Forms.Label();
            this.EmailTo_LAB = new System.Windows.Forms.Label();
            this.EmailCc_TBX = new System.Windows.Forms.TextBox();
            this.EmailTo_TBX = new System.Windows.Forms.TextBox();
            this.MandatoryDeliveryInformation_TBX = new System.Windows.Forms.TextBox();
            this.MandatoryDeliveryInformation_GBX = new System.Windows.Forms.GroupBox();
            this.MainFrame.SuspendLayout();
            this.Delivery_GBX.SuspendLayout();
            this.AdressEmail_GBX.SuspendLayout();
            this.MandatoryDeliveryInformation_GBX.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainFrame
            // 
            this.MainFrame.Controls.Add(this.ChoicePerspective_CHB);
            this.MainFrame.Controls.Add(this.ChoiceOrder_CHB);
            this.MainFrame.Controls.Add(this.ChoiceElevation_CHB);
            this.MainFrame.Controls.Add(this.ChoicePlan_CHB);
            this.MainFrame.Controls.Add(this.ChoiceEGI_CHB);
            this.MainFrame.Location = new System.Drawing.Point(218, 12);
            this.MainFrame.Name = "MainFrame";
            this.MainFrame.Size = new System.Drawing.Size(94, 152);
            this.MainFrame.TabIndex = 15;
            this.MainFrame.TabStop = false;
            this.MainFrame.Text = "Fichiers Joints :";
            // 
            // ChoicePerspective_CHB
            // 
            this.ChoicePerspective_CHB.AutoSize = true;
            this.ChoicePerspective_CHB.Checked = true;
            this.ChoicePerspective_CHB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChoicePerspective_CHB.Location = new System.Drawing.Point(6, 88);
            this.ChoicePerspective_CHB.Name = "ChoicePerspective_CHB";
            this.ChoicePerspective_CHB.Size = new System.Drawing.Size(82, 17);
            this.ChoicePerspective_CHB.TabIndex = 10;
            this.ChoicePerspective_CHB.Text = "Perspective";
            this.ChoicePerspective_CHB.UseVisualStyleBackColor = true;
            this.ChoicePerspective_CHB.CheckedChanged += new System.EventHandler(this.ChoicePerspective_CHB_CheckedChanged);
            // 
            // ChoiceOrder_CHB
            // 
            this.ChoiceOrder_CHB.AutoSize = true;
            this.ChoiceOrder_CHB.Checked = true;
            this.ChoiceOrder_CHB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChoiceOrder_CHB.Location = new System.Drawing.Point(6, 111);
            this.ChoiceOrder_CHB.Name = "ChoiceOrder_CHB";
            this.ChoiceOrder_CHB.Size = new System.Drawing.Size(79, 17);
            this.ChoiceOrder_CHB.TabIndex = 11;
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
            this.ChoiceElevation_CHB.TabIndex = 9;
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
            this.ChoicePlan_CHB.TabIndex = 8;
            this.ChoicePlan_CHB.Text = "Plan";
            this.ChoicePlan_CHB.UseVisualStyleBackColor = true;
            this.ChoicePlan_CHB.CheckedChanged += new System.EventHandler(this.ChoicePlan_CHB_CheckedChanged);
            // 
            // ChoiceEGI_CHB
            // 
            this.ChoiceEGI_CHB.AutoSize = true;
            this.ChoiceEGI_CHB.Location = new System.Drawing.Point(6, 19);
            this.ChoiceEGI_CHB.Name = "ChoiceEGI_CHB";
            this.ChoiceEGI_CHB.Size = new System.Drawing.Size(44, 17);
            this.ChoiceEGI_CHB.TabIndex = 7;
            this.ChoiceEGI_CHB.Text = "EGI";
            this.ChoiceEGI_CHB.UseVisualStyleBackColor = true;
            this.ChoiceEGI_CHB.CheckedChanged += new System.EventHandler(this.ChoiceEGI_CHB_CheckedChanged);
            // 
            // Ok_BTN
            // 
            this.Ok_BTN.Location = new System.Drawing.Point(124, 240);
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
            this.version_LNK.Location = new System.Drawing.Point(12, 245);
            this.version_LNK.Name = "version_LNK";
            this.version_LNK.Size = new System.Drawing.Size(13, 13);
            this.version_LNK.TabIndex = 12;
            this.version_LNK.TabStop = true;
            this.version_LNK.Text = "?";
            this.version_LNK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.version_LNK_LinkClicked);
            // 
            // Delivery_GBX
            // 
            this.Delivery_GBX.Controls.Add(this.CustomerDelivery_RBN);
            this.Delivery_GBX.Controls.Add(this.RetaillerDelivery_RBN);
            this.Delivery_GBX.Location = new System.Drawing.Point(12, 95);
            this.Delivery_GBX.Name = "Delivery_GBX";
            this.Delivery_GBX.Size = new System.Drawing.Size(200, 69);
            this.Delivery_GBX.TabIndex = 14;
            this.Delivery_GBX.TabStop = false;
            this.Delivery_GBX.Text = "Livraison :";
            // 
            // CustomerDelivery_RBN
            // 
            this.CustomerDelivery_RBN.AutoSize = true;
            this.CustomerDelivery_RBN.Location = new System.Drawing.Point(6, 41);
            this.CustomerDelivery_RBN.Name = "CustomerDelivery_RBN";
            this.CustomerDelivery_RBN.Size = new System.Drawing.Size(88, 17);
            this.CustomerDelivery_RBN.TabIndex = 5;
            this.CustomerDelivery_RBN.TabStop = true;
            this.CustomerDelivery_RBN.Text = "Chez le client";
            this.CustomerDelivery_RBN.UseVisualStyleBackColor = true;
            this.CustomerDelivery_RBN.CheckedChanged += new System.EventHandler(this.CustomerDelivery_CHB_CheckedChanged);
            // 
            // RetaillerDelivery_RBN
            // 
            this.RetaillerDelivery_RBN.AutoSize = true;
            this.RetaillerDelivery_RBN.Checked = true;
            this.RetaillerDelivery_RBN.Location = new System.Drawing.Point(6, 18);
            this.RetaillerDelivery_RBN.Name = "RetaillerDelivery_RBN";
            this.RetaillerDelivery_RBN.Size = new System.Drawing.Size(105, 17);
            this.RetaillerDelivery_RBN.TabIndex = 4;
            this.RetaillerDelivery_RBN.TabStop = true;
            this.RetaillerDelivery_RBN.Text = "Chez le détaillant";
            this.RetaillerDelivery_RBN.UseVisualStyleBackColor = true;
            this.RetaillerDelivery_RBN.CheckedChanged += new System.EventHandler(this.RetaillerDelivery_CHB_CheckedChanged);
            // 
            // AdressEmail_GBX
            // 
            this.AdressEmail_GBX.Controls.Add(this.EmailCc_LAB);
            this.AdressEmail_GBX.Controls.Add(this.EmailTo_LAB);
            this.AdressEmail_GBX.Controls.Add(this.EmailCc_TBX);
            this.AdressEmail_GBX.Controls.Add(this.EmailTo_TBX);
            this.AdressEmail_GBX.Location = new System.Drawing.Point(12, 12);
            this.AdressEmail_GBX.Name = "AdressEmail_GBX";
            this.AdressEmail_GBX.Size = new System.Drawing.Size(200, 77);
            this.AdressEmail_GBX.TabIndex = 16;
            this.AdressEmail_GBX.TabStop = false;
            this.AdressEmail_GBX.Text = "Adresse mail :";
            // 
            // EmailCc_LAB
            // 
            this.EmailCc_LAB.AutoSize = true;
            this.EmailCc_LAB.Location = new System.Drawing.Point(6, 48);
            this.EmailCc_LAB.Name = "EmailCc_LAB";
            this.EmailCc_LAB.Size = new System.Drawing.Size(26, 13);
            this.EmailCc_LAB.TabIndex = 14;
            this.EmailCc_LAB.Text = "Cc :";
            // 
            // EmailTo_LAB
            // 
            this.EmailTo_LAB.AutoSize = true;
            this.EmailTo_LAB.Location = new System.Drawing.Point(6, 22);
            this.EmailTo_LAB.Name = "EmailTo_LAB";
            this.EmailTo_LAB.Size = new System.Drawing.Size(20, 13);
            this.EmailTo_LAB.TabIndex = 13;
            this.EmailTo_LAB.Text = "A :";
            // 
            // EmailCc_TBX
            // 
            this.EmailCc_TBX.Location = new System.Drawing.Point(38, 45);
            this.EmailCc_TBX.Name = "EmailCc_TBX";
            this.EmailCc_TBX.Size = new System.Drawing.Size(156, 20);
            this.EmailCc_TBX.TabIndex = 3;
            this.EmailCc_TBX.TextChanged += new System.EventHandler(this.EmailCc_TBX_TextChanged);
            // 
            // EmailTo_TBX
            // 
            this.EmailTo_TBX.Location = new System.Drawing.Point(38, 19);
            this.EmailTo_TBX.Name = "EmailTo_TBX";
            this.EmailTo_TBX.Size = new System.Drawing.Size(156, 20);
            this.EmailTo_TBX.TabIndex = 2;
            this.EmailTo_TBX.TextChanged += new System.EventHandler(this.EmailTo_TBX_TextChanged);
            // 
            // MandatoryDeliveryInformation_TBX
            // 
            this.MandatoryDeliveryInformation_TBX.Location = new System.Drawing.Point(6, 19);
            this.MandatoryDeliveryInformation_TBX.MaxLength = 120;
            this.MandatoryDeliveryInformation_TBX.Multiline = true;
            this.MandatoryDeliveryInformation_TBX.Name = "MandatoryDeliveryInformation_TBX";
            this.MandatoryDeliveryInformation_TBX.Size = new System.Drawing.Size(288, 39);
            this.MandatoryDeliveryInformation_TBX.TabIndex = 17;
            this.MandatoryDeliveryInformation_TBX.TextChanged += new System.EventHandler(this.MandatoryDeliveryInformation_TBX_TextChanged);
            // 
            // MandatoryDeliveryInformation_GBX
            // 
            this.MandatoryDeliveryInformation_GBX.Controls.Add(this.MandatoryDeliveryInformation_TBX);
            this.MandatoryDeliveryInformation_GBX.Location = new System.Drawing.Point(12, 170);
            this.MandatoryDeliveryInformation_GBX.Name = "MandatoryDeliveryInformation_GBX";
            this.MandatoryDeliveryInformation_GBX.Size = new System.Drawing.Size(300, 64);
            this.MandatoryDeliveryInformation_GBX.TabIndex = 18;
            this.MandatoryDeliveryInformation_GBX.TabStop = false;
            this.MandatoryDeliveryInformation_GBX.Text = "Impératif de livraison :";
            // 
            // MainForm
            // 
            this.AcceptButton = this.Ok_BTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 271);
            this.ControlBox = false;
            this.Controls.Add(this.MandatoryDeliveryInformation_GBX);
            this.Controls.Add(this.AdressEmail_GBX);
            this.Controls.Add(this.Delivery_GBX);
            this.Controls.Add(this.version_LNK);
            this.Controls.Add(this.Ok_BTN);
            this.Controls.Add(this.MainFrame);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commande";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainFrame.ResumeLayout(false);
            this.MainFrame.PerformLayout();
            this.Delivery_GBX.ResumeLayout(false);
            this.Delivery_GBX.PerformLayout();
            this.AdressEmail_GBX.ResumeLayout(false);
            this.AdressEmail_GBX.PerformLayout();
            this.MandatoryDeliveryInformation_GBX.ResumeLayout(false);
            this.MandatoryDeliveryInformation_GBX.PerformLayout();
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
        private System.Windows.Forms.GroupBox Delivery_GBX;
        private System.Windows.Forms.RadioButton CustomerDelivery_RBN;
        private System.Windows.Forms.RadioButton RetaillerDelivery_RBN;
        private System.Windows.Forms.CheckBox ChoicePerspective_CHB;
        private System.Windows.Forms.GroupBox AdressEmail_GBX;
        private System.Windows.Forms.Label EmailCc_LAB;
        private System.Windows.Forms.Label EmailTo_LAB;
        private System.Windows.Forms.TextBox EmailCc_TBX;
        private System.Windows.Forms.TextBox EmailTo_TBX;
        private System.Windows.Forms.TextBox MandatoryDeliveryInformation_TBX;
        private System.Windows.Forms.GroupBox MandatoryDeliveryInformation_GBX;
    }
}