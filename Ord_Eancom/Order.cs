using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.IO;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------
using KD.Model;
using KD.Analysis;

using Eancom;


namespace Ord_Eancom
{
    
    public class Order : KD.Plugin.PluginBase
    {
        OrderInformations orderInformations = null;
        OrderInformations orderInformationsFromArticles = null;
        OrderWrite orderWrite = null;
        FileEDI fileEDI = null;
        MainForm mainForm = null;

        public static KD.Plugin.Word.Plugin _pluginWord = null;
        public static string orderDir = String.Empty;
        public static string orderFile = String.Empty;

        private const string ManufacturerCustomFromCatalog = "MANUFACTURER";

        private string referenceNoValid = String.Empty;

        public Order()
        {
           
        }


        public bool ProcessOrder(int callParamsBlock)
        {
            _pluginWord = new KD.Plugin.Word.Plugin();
           
            string generateOrder = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.GenerateOrder);

            if (!String.IsNullOrEmpty(generateOrder))
            {
                bool.TryParse(generateOrder, out bool isGenerateOrder);
               
                if (isGenerateOrder)
                {                    
                    orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
                   
                    Order.orderDir = this.CurrentAppli.GetCallParamsInfoDirect(callParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);                   
                    KD.Config.IniFile ordersIniFile = new KD.Config.IniFile(Path.Combine(Order.orderDir, FileEDI.IniOrderFileName));

                    MainForm.EmailTo = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailToKey);
                    MainForm.EmailCc = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailCcKey);
                   
                    string recipientAddresses = MainForm.EmailTo; // "commande-EDI@discac.fr, commandes@discac.fr, ETL@discac.fr";
                    
                    if (!String.IsNullOrEmpty(recipientAddresses))
                    {                       
                        this.SendMail(recipientAddresses);
                        return true;
                    }                    
                }               
            }
            
            MessageBox.Show("La commande n'a pas été généré.");
          
            return true;
        }

        private void SendMail(string recipientAddresses)
        {           
            string ccAdress = MainForm.EmailCc;
            string customerNumber = orderInformations.GetRetailerGLN();
            string commissiontNumber = orderInformations.GetCommissionNumber();
            string softWareVersion = orderInformations.GetNameAndVersionSoftware();
            string attachedFilesPathsList = Path.Combine(Order.orderDir, OrderTransmission.OrderZipFileName);
           
            DialogResult dialogResult = MessageBox.Show("Voulez-vous envoyer la commande ?", "InSitu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string[] recipientsAddress = recipientAddresses.Split(KD.CharTools.Const.SemiColon);
                string recipientAddress = String.Empty;
                string recipientName = String.Empty;

                System.Net.Mail.MailAddressCollection mailAddresses = new System.Net.Mail.MailAddressCollection();

                foreach (string address in recipientsAddress)
                {
                    //System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(address);

                    //recipientName += mailAddress.DisplayName + KD.StringTools.Const.WhiteSpace;
                    recipientAddress += "<" + address + ">" + KD.CharTools.Const.SemiColon;//"Name" + KD.CharTools.Const.SemiColon +
                    
                }
                
                recipientAddress = recipientAddress.TrimEnd(KD.CharTools.Const.SemiColon);

                //System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage("moi@gmail.com", recipientAddress);

                //string mailTo = mailMessage.To.ToString();
                //mailAddresses.Add(recipientAddress);
                //System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(recipientAddress);
               
               

                bool bSend = this.CurrentAppli.EmailSend(recipientName, //Contact name if exist
                                                         recipientAddress,
                                                         ccAdress, //cc
                                                         OrderTransmission.HeaderSubject + "<" + customerNumber + "><" + commissiontNumber + "><" + softWareVersion + ">",
                                                         String.Empty, //Body message
                                                         attachedFilesPathsList, //AttachedFilesPathsList
                                                         String.Empty, //AttachedFilesNamesList
                                                         true); // show dialog
            }
        }




        /// <summary>
        /// The "GenerateOrder" function belonging to the "Order" class is the entry point of the supplier orders management module.
        /// If the DLL is selected in the "Computer file" option box corresponding to a supplier in the "Suppliers" dialog box,
        /// the following "GenerateOrder" function is called when the user generates an order for that supplier running 
        /// the "File | Orders to suppliers | Generate".
        /// </summary>
        /// <param name="callParamsBlock"></param>
        /// <returns></returns>
        public bool GenerateOrder(int callParamsBlock)
        {
            _pluginWord = new KD.Plugin.Word.Plugin();
            _pluginWord.InitializeAll(callParamsBlock);

            Order.orderDir = this.CurrentAppli.GetCallParamsInfoDirect(callParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);
            orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
            Articles articles = SupplierArticleValidInScene();

            if (this.IsGenerateOrders(articles))
            {
                this.mainForm = new MainForm();
                this.Main(callParamsBlock, articles);

                //MessageBox.Show("Terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);                          
                Cursor.Current = Cursors.Arrow;
                return true;
            }            
            return false;
        }

        private Articles SupplierArticleValidInScene()
        {
            Articles articles = new Articles();

            string supplierId = orderInformations.GetSupplierName();
            string IDs = String.Empty;

            int objSupplierNb = this.GetObjectNbFromHeading();
            if (objSupplierNb > 0)
            {
                for (int rank = 0; rank < objSupplierNb; rank++)
                {
                    int objId = this.CurrentAppli.Scene.SupplierGetObjectId(supplierId, (int)KD.SDK.SceneEnum.ObjectList.ALLHEADINGS, false, rank);
                    IDs += objId + KD.StringTools.Const.Comma;
                }

                Articles supplierArticles = new Articles(CurrentAppli, IDs);

                foreach (Article article in supplierArticles)
                {
                    if (article.IsValid && article.Type != 17)
                    {
                        articles.Add(article);
                    }
                }
            }
            return articles;
        }
        private bool IsGenerateOrders(Articles articles)
        {
            referenceNoValid = String.Empty;
            if (articles == null | articles.Count <= 0 | !this.IsSceneComplete(articles))
            {
                MessageBox.Show("L'article '" + referenceNoValid + "' de la commande n'est pas valide." + Environment.NewLine + "La commande est annulée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.FalseLowerCase, OrderKey.GenerateOrder);                
                return false;
            }
            _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.TrueLowerCase, OrderKey.GenerateOrder);
            return true;
        }

        private void Main(int callParamsBlock, Articles articles)
        {
            this.mainForm.ShowDialog();

            OrderWrite.segmentNumberBetweenUNHandUNT = 0;
            RFF_A.refPosList.Clear();                   
           
            orderInformationsFromArticles = new OrderInformations(this.CurrentAppli, callParamsBlock, articles);

            fileEDI = new FileEDI(this.CurrentAppli, orderInformations.GetSupplierName(), orderInformationsFromArticles);
            orderWrite = new OrderWrite(this.CurrentAppli, orderInformations, orderInformationsFromArticles, articles, fileEDI);

            if (MainForm.IsChoiceExportEGI)
            {
                orderWrite.BuildEGI(articles);
                orderWrite.EGIOrderFile();
            }
            if (MainForm.IsChoiceExportPlan)
            {
                orderWrite.BuildPlan();
            }
            if (MainForm.IsChoiceExportElevation)
            {
                orderWrite.BuildElevation();
            }
            if (MainForm.IsChoiceExportPerspective)
            {
                orderWrite.BuildPerspective();
            }
            if (MainForm.IsChoiceExportOrder)
            {
                orderWrite.BuildOrder();
            }

            orderWrite.BuildEDI(articles);
            orderWrite.EDIOrderFileStream();

            orderWrite.ZIPOrderFile();
        }
      
        private int GetObjectNbFromHeading()
        {
            return this.CurrentAppli.Scene.HeadingGetObjectsNb((int)KD.SDK.SceneEnum.ObjectList.ALLHEADINGS, false);
        }
     

        private bool IsSceneComplete(Articles articles)
        {
            List<string> catalogsList = this.CatalogsByManufacturerValidList();

            foreach(Article article in articles)
            {
                string catalogFileName = article.CatalogFileName.ToUpper();
                if (!catalogsList.Contains(catalogFileName))
                {
                    referenceNoValid = article.Ref;
                    return false;
                }
            }            
            return true;
        }

        private IEnumerable<string> CatalogsBaseList()
        {
            return Directory.EnumerateFiles(this.CurrentAppli.CatalogDir, KD.StringTools.Const.Wildcard + KD.IO.File.Extension.Cat);
        }
        private string GetCatalogCustomInfo(string catalog, string info)
        {
            return this.CurrentAppli.CatalogGetCustomInfo(catalog, info);
        }
        private List<string> CatalogsByManufacturerValidList()
        {
            List<string> list = new List<string>();
            list.Clear();

            foreach (string catalogPath in this.CatalogsBaseList())
            {
                string manufacturerCat = this.GetCatalogCustomInfo(catalogPath, ManufacturerCustomFromCatalog);

                if (!String.IsNullOrEmpty(manufacturerCat) && !manufacturerCat.Equals("0"))
                {
                    list.Add(Path.GetFileNameWithoutExtension(catalogPath.ToUpper()));
                }
            }
            return list;
        }
    }

}