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
        private static bool _isChoiceExportEGI;
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


        private bool IsChecked(CheckBox checkBox)
        {
            if (checkBox.Checked)
            {
                return true;
            }            
            return false;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void Ok_BTN_Click(object sender, EventArgs e)
        {
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

        private void version_LNK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(this.ProductVersion, "Version");
        }

        private void ChoiceOrder_CHB_CheckedChanged(object sender, EventArgs e)
        {
            IsChoiceExportOrder = this.IsChecked(ChoiceOrder_CHB);
        }

        private void ChoiceElevation_CHB_CheckedChanged(object sender, EventArgs e)
        {
            IsChoiceExportElevation = this.IsChecked(ChoiceElevation_CHB);
        }
    }
}
