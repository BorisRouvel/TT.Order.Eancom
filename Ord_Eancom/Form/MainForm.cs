using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ord_Eancom
{
    public partial class MainForm : Form
    {
        private static bool _isChoiceExportEGI ;
        public static bool IsChoiceExportEGI
        {
            get
            {
                return _isChoiceExportEGI;
            }
            set
            {
                _isChoiceExportEGI = value;
            }
        }

        private static bool _isChoiceExportPlan;
        public static bool IsChoiceExportPlan
        {
            get
            {
                return _isChoiceExportPlan;
            }
            set
            {
                _isChoiceExportPlan = value;
            }
        }

        private static bool _isChoiceExportElevation;
        public static bool IsChoiceExportElevation
        {
            get
            {
                return _isChoiceExportElevation;
            }
            set
            {
                _isChoiceExportElevation = value;
            }
        }

        private static bool _isChoiceExportPerspective;
        public static bool IsChoiceExportPerspective
        {
            get
            {
                return _isChoiceExportPerspective;
            }
            set
            {
                _isChoiceExportPerspective = value;
            }
        }

        private static bool _isChoiceExportOrder;
        public static bool IsChoiceExportOrder
        {
            get
            {
                return _isChoiceExportOrder;
            }
            set
            {
                _isChoiceExportOrder = value;
            }
        }

        private static bool _isChoiceRetaillerDelivery;
        public static bool IsChoiceRetaillerDelivery
        {
            get
            {
                return _isChoiceRetaillerDelivery;
            }
            set
            {
                _isChoiceRetaillerDelivery = value;
            }
        }

        private static bool _isChoiceCustomerDelivery;
        public static bool IsChoiceCustomerDelivery
        {
            get
            {
                return _isChoiceCustomerDelivery;
            }
            set
            {
                _isChoiceCustomerDelivery = value;
            }
        }

        //private static string _informationInstallation;
        //public static string InformationInstallation
        //{
        //    get
        //    {
        //        return _informationInstallation;
        //    }
        //    set
        //    {
        //        _informationInstallation = value;
        //    }
        //}

        private static string _emailTo;
        public static string EmailTo
        {
            get
            {
                return _emailTo;
            }
            set
            {
                _emailTo = value;
            }
        }

        private static string _emailCc;
        public static string EmailCc
        {
            get
            {
                return _emailCc;
            }
            set
            {
                _emailCc = value;
            }
        }


        public MainForm()
        {
            InitializeComponent();

            this.InitializeMembers();
        }

        // Event
        private void InitializeMembers()
        {
            _isChoiceExportEGI = this.ChoiceEGI_CHB.Checked;
            _isChoiceExportPlan = this.ChoicePlan_CHB.Checked;
            _isChoiceExportElevation = this.ChoiceElevation_CHB.Checked;
            _isChoiceExportPerspective = this.ChoicePerspective_CHB.Checked;
            _isChoiceExportOrder = this.ChoiceOrder_CHB.Checked;
            _isChoiceRetaillerDelivery = this.RetaillerDelivery_RBN.Checked;
            _isChoiceCustomerDelivery = this.CustomerDelivery_RBN.Checked;
            //_informationInstallation = String.Empty;
        }
        private bool IsChecked(Control control)
        {
            if (control is RadioButton)
            {
                RadioButton controlBox = (RadioButton)control;
                if (controlBox.Checked)
                {
                    return true;
                }
            }
            else if (control is CheckBox)
            {
                CheckBox controlBox = (CheckBox)control;
                if (controlBox.Checked)
                {
                    return true;
                }
            }

            return false;
        }      
        private void SetCheckedControl(Control control, string choice)
        {            
            if (!String.IsNullOrEmpty(choice) )
            {
                 if (bool.TryParse(choice, out bool bChoice))
                {
                    if (control is RadioButton)
                    {
                        RadioButton controlBox = (RadioButton)control;
                        controlBox.Checked = bChoice;
                        
                    }
                    else if (control is CheckBox)
                    {
                        CheckBox controlBox = (CheckBox)control;
                        controlBox.Checked = bChoice;
                    }
                }
                
            }
        }
        private void LoadCustomInfo()
        {
            string choiceRetaillerDelivery = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceRetaillerDelivery);
            string choiceCustomerDelivery = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceCustomerDelivery);
            string informationInstallation = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.InformationInstallation);
            string choiceExportEGI = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportEGI);
            string choiceExportPlan = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportPlan);
            string choiceExportElevation = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportElevation);
            string choiceExportPerspective = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportPerspective);
            string choiceExportOrder = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportOrder);

            this.SetCheckedControl(this.RetaillerDelivery_RBN, choiceRetaillerDelivery);
            this.SetCheckedControl(this.CustomerDelivery_RBN, choiceCustomerDelivery);            
            //this.InformationInstallation_TXB.Text = informationInstallation;
            this.SetCheckedControl(this.ChoiceEGI_CHB, choiceExportEGI);
            this.SetCheckedControl(this.ChoicePlan_CHB, choiceExportPlan);
            this.SetCheckedControl(this.ChoiceElevation_CHB, choiceExportElevation);
            this.SetCheckedControl(this.ChoicePerspective_CHB, choiceExportPerspective);
            this.SetCheckedControl(this.ChoiceOrder_CHB, choiceExportOrder);            
        }
        private void SaveCustomInfo()
        {
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceRetaillerDelivery), OrderKey.ChoiceRetaillerDelivery);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceCustomerDelivery), OrderKey.ChoiceCustomerDelivery);
            //Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.InformationInstallation), OrderKey.InformationInstallation);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportEGI), OrderKey.ChoiceExportEGI);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportPlan), OrderKey.ChoiceExportPlan);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportElevation), OrderKey.ChoiceExportElevation);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportPerspective), OrderKey.ChoiceExportPerspective);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportOrder), OrderKey.ChoiceExportOrder);
        }
        private void LoadInfoFromIni()
        {
            _emailTo = Eancom.FileEDI.ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailToKey);
            this.EmailTo_TBX.Text = EmailTo;

            _emailCc = Eancom.FileEDI.ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailCcKey);
            this.EmailCc_TBX.Text = EmailCc;
        }
        private void SaveInfoToIni()
        {          
            Eancom.FileEDI.ordersIniFile.WriteValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailToKey, EmailTo);
            Eancom.FileEDI.ordersIniFile.WriteValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailCcKey, EmailCc);           
        }      
        private void UpdateForm()
        {
            this.LoadCustomInfo();
            this.LoadInfoFromIni();
        }

        // Form
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.UpdateForm();           
        }
      
        private void Ok_BTN_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EmailTo))
            {
                MessageBox.Show("Vous devez renseigner une adresse email.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.EmailTo_TBX.Focus();
                return;
            }
            this.SaveCustomInfo();
            this.SaveInfoToIni();
            Cursor.Current = Cursors.WaitCursor;

            this.Close();
        }

        private void ChoiceEGI_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceExportEGI = this.IsChecked(ChoiceEGI_CHB);            
        }

        private void ChoicePlan_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceExportPlan = this.IsChecked(ChoicePlan_CHB);
        }
       
        private void ChoiceOrder_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceExportOrder = this.IsChecked(ChoiceOrder_CHB);
        }

        private void ChoiceElevation_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceExportElevation = this.IsChecked(ChoiceElevation_CHB);
        }

        private void ChoicePerspective_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceExportPerspective = this.IsChecked(ChoicePerspective_CHB);
        }

        private void RetaillerDelivery_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceRetaillerDelivery = this.IsChecked(RetaillerDelivery_RBN);
        }

        private void CustomerDelivery_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceCustomerDelivery = this.IsChecked(CustomerDelivery_RBN);
        }

        private void version_LNK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(this.ProductVersion, "Version");
        }

        private void EmailTo_TBX_TextChanged(object sender, EventArgs e)
        {
            _emailTo = this.EmailTo_TBX.Text;
        }

        private void EmailCc_TBX_TextChanged(object sender, EventArgs e)
        {
            _emailCc = this.EmailCc_TBX.Text;
        }
    }
}
