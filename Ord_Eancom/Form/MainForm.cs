using System;
using System.Windows.Forms;
using Eancom;

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

        private static bool _isChoiceRetailerDelivery;
        public static bool IsChoiceRetailerDelivery
        {
            get
            {
                return _isChoiceRetailerDelivery;
            }
            set
            {
                _isChoiceRetailerDelivery = value;
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

        private static string _mandatoryDeliveryInformation;
        public static string MandatoryDeliveryInformation
        {
            get
            {
                return _mandatoryDeliveryInformation;
            }
            set
            {
                _mandatoryDeliveryInformation = value;
            }
        }

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

        private static bool _cancel;
        public static bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }

        OrderInformations _orderInformations = null;
        string _supplierName = null;
        string _retailerNumber = null;

        public MainForm(OrderInformations orderInformations)
        {
            InitializeComponent();

            _orderInformations = orderInformations;
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
            _isChoiceRetailerDelivery = this.RetaillerDelivery_RBN.Checked;
            _isChoiceCustomerDelivery = this.CustomerDelivery_RBN.Checked;
            _mandatoryDeliveryInformation = String.Empty;
            _supplierName = _orderInformations.GetSupplierName();
            _retailerNumber = _orderInformations.GetRetailerNumber();
            _cancel = false;
        }
        private bool IsChecked(Control control)
        {
            if (control is RadioButton controlBox)
            {
                if (controlBox.Checked)
                {
                    return true;
                }
            }
            else if (control is CheckBox controlCBox)
            {
                if (controlCBox.Checked)
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
                    if (control is RadioButton controlBox)
                    {
                        controlBox.Checked = bChoice;

                    }
                    else if (control is CheckBox controlCBox)
                    {
                        controlCBox.Checked = bChoice;
                    }
                }
                
            }
        }
        private void SetMandatoryDeliveryTextBox()
        {
            if (MainForm.IsChoiceCustomerDelivery)
            {
                _mandatoryDeliveryInformation = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.MandatoryDeliveryCustomerInformation);
                this.MandatoryDeliveryInformation_TBX.Text = MandatoryDeliveryInformation;
            }
            if (MainForm.IsChoiceRetailerDelivery)
            {
                _mandatoryDeliveryInformation = FileEDI.ordersIniFile.ReadValue(FileEDI.ediSection, OrderKey.MandatoryDeliveryRetailerInformation + _retailerNumber);
                this.MandatoryDeliveryInformation_TBX.Text = MandatoryDeliveryInformation;
            }
        }
        private void LoadCustomInfo()
        {
            string choiceRetaillerDelivery = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceRetaillerDelivery);
            string choiceCustomerDelivery = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceCustomerDelivery);
            string choiceExportEGI = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportEGI);
            string choiceExportPlan = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportPlan);
            string choiceExportElevation = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportElevation);
            string choiceExportPerspective = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportPerspective);
            string choiceExportOrder = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.ChoiceExportOrder);

            this.SetCheckedControl(this.RetaillerDelivery_RBN, choiceRetaillerDelivery);
            this.SetCheckedControl(this.CustomerDelivery_RBN, choiceCustomerDelivery);
            this.SetCheckedControl(this.ChoiceEGI_CHB, choiceExportEGI);
            this.SetCheckedControl(this.ChoicePlan_CHB, choiceExportPlan);
            this.SetCheckedControl(this.ChoiceElevation_CHB, choiceExportElevation);
            this.SetCheckedControl(this.ChoicePerspective_CHB, choiceExportPerspective);
            this.SetCheckedControl(this.ChoiceOrder_CHB, choiceExportOrder);

            this.SetMandatoryDeliveryTextBox();
        }
        private void SaveCustomInfo()
        {
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceRetailerDelivery), OrderKey.ChoiceRetaillerDelivery);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceCustomerDelivery), OrderKey.ChoiceCustomerDelivery);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportEGI), OrderKey.ChoiceExportEGI);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportPlan), OrderKey.ChoiceExportPlan);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportElevation), OrderKey.ChoiceExportElevation);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportPerspective), OrderKey.ChoiceExportPerspective);
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(Convert.ToString(MainForm.IsChoiceExportOrder), OrderKey.ChoiceExportOrder);

            if (MainForm.IsChoiceCustomerDelivery)
            {
                Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(MandatoryDeliveryInformation, OrderKey.MandatoryDeliveryCustomerInformation);
            }
        }
        private void LoadInfoFromIni()
        {
            _emailTo = FileEDI.ordersIniFile.ReadValue(FileEDI.ediSection, FileEDI.emailToKey + _supplierName);
            this.EmailTo_TBX.Text = EmailTo;

            _emailCc = FileEDI.ordersIniFile.ReadValue(FileEDI.ediSection, FileEDI.emailCcKey + _supplierName);
            this.EmailCc_TBX.Text = EmailCc;

            this.SetMandatoryDeliveryTextBox();
        }
        private void SaveInfoToIni()
        {            
            FileEDI.ordersIniFile.WriteValue(FileEDI.ediSection, FileEDI.emailToKey + _supplierName, EmailTo);
            FileEDI.ordersIniFile.WriteValue(FileEDI.ediSection, FileEDI.emailCcKey + _supplierName, EmailCc);

            if (MainForm.IsChoiceRetailerDelivery)
            {
                FileEDI.ordersIniFile.WriteValue(FileEDI.ediSection, OrderKey.MandatoryDeliveryRetailerInformation + _retailerNumber, MandatoryDeliveryInformation);
            }
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
            _cancel = false;
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.TrueLowerCase, OrderKey.GenerateOrder);
            this.Close();
        }

        private void Cancel_BTN_Click(object sender, EventArgs e)
        {
            _cancel = true;
            Order._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.FalseLowerCase, OrderKey.GenerateOrder);
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
            _isChoiceRetailerDelivery = this.IsChecked(RetaillerDelivery_RBN);
            this.SetMandatoryDeliveryTextBox();
        }

        private void CustomerDelivery_CHB_CheckedChanged(object sender, EventArgs e)
        {
            _isChoiceCustomerDelivery = this.IsChecked(CustomerDelivery_RBN);
            this.SetMandatoryDeliveryTextBox();
        }

        private void Version_LNK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        private void MandatoryDeliveryInformation_TBX_TextChanged(object sender, EventArgs e)
        {
            _mandatoryDeliveryInformation = this.MandatoryDeliveryInformation_TBX.Text;
        }       
    }
}
