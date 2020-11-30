using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KD.SDKComponent;

namespace Ord_Eancom
{
    public class BuildOrder
    {
        OrderInformations _orderInformations = null;
        AppliComponent _currentAppli = null;
       
        public BuildOrder(AppliComponent currentAppli, OrderInformations orderInformations)
        {
            _currentAppli = currentAppli;
            _orderInformations = orderInformations;
        }

        public void Generate()
        {
            int supplierRank = _currentAppli.GetSupplierRankFromIdent(_orderInformations.GetSupplierName());

            if (supplierRank != KD.Const.UnknownId)
            {
                this.ManagePdfFile(supplierRank);
            }         
        }
        private string GetSupplierFilePath()
        {
            string supplierId = Order._pluginWord.DocEngine.SupplierId();
            string sceneDocDir = Order._pluginWord.DocEngine.SceneDocDir;
            return Path.Combine(sceneDocDir, KD.Plugin.Word.Config.Const.SupplierOrderDirName, supplierId + KD.IO.File.Extension.Pdf);
        }
        private bool ManagePdfFile(int supplierRank)
        {
            string pdfFlagState = _currentAppli.SupplierGetInfo(supplierRank, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);
            
            if (pdfFlagState == KD.StringTools.Const.One)
            {
                string supplierFilePath = this.GetSupplierFilePath();               
                this.CopySupplierFile(supplierFilePath);               
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Afin de générer le fichier de '" + OrderTransmission.OrderName + OrderTransmission.ExtensionPDF + "'" + Environment.NewLine +
                    "Veuillez sélectionner 'Fichier PDF joint' dans votre fournisseurs", "Information");
            }
            //bPdfFlag = this.CurrentAppli.SupplierSetInfo(supplierRank, pdfFlagState, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);
            return false;
        }
        private void CopySupplierFile(string supplierFilePath)
        {
            if (File.Exists(supplierFilePath))
            {
                try
                {
                    File.Copy(supplierFilePath, Path.Combine(Order.orderDir, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF), true);                 
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Le fichier de '" + OrderTransmission.OrderName + OrderTransmission.ExtensionPDF + "'" +
                    " n'a pas pu être copié." + Environment.NewLine + ex.Message, "Information");                  
                }               
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Le fichier de '" + OrderTransmission.OrderName + OrderTransmission.ExtensionPDF + "'" +
                    " n'a pas pu être généré.", "Information");
            }         
        }
    }
}
