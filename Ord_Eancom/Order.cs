using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Mail;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------
using KD.Model;

using Eancom;
using TT.Import.EGI;


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
            orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);

            string generateOrder = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.GenerateOrder);

            if (!String.IsNullOrEmpty(generateOrder))
            {
                bool.TryParse(generateOrder, out bool isGenerateOrder);
               
                if (isGenerateOrder)
                {
                    string supplierName = orderInformations.GetSupplierName();
                    string retaillerNumber = orderInformations.GetRetailerNumber();
                    Order.orderDir = orderInformations.GetOrderDir();                   
                    KD.Config.IniFile ordersIniFile = new KD.Config.IniFile(Path.Combine(Order.orderDir, FileEDI.IniOrderFileName));

                    MainForm.EmailTo = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailToKey + supplierName);
                    MainForm.EmailCc = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailCcKey + supplierName);
                    MainForm.MandatoryDeliveryInformation = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.mandatoryDeliveryInformation + retaillerNumber);

                    string recipientAddresses = MainForm.EmailTo; // "commande-EDI@discac.fr;commandes@discac.fr;ETL@discac.fr";
                    string ccAdress = MainForm.EmailCc;

                    if (!String.IsNullOrEmpty(recipientAddresses))
                    {                       
                        this.SendMail(recipientAddresses, ccAdress);
                        return true;
                    }                    
                }               
            }
            
            MessageBox.Show("La commande n'a pas été généré.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);          
            return true;
        }

        private void SendMail(string recipientAddresses, string ccAdress)
        {            
            string customerNumber = orderInformations.GetRetailerGLN();
            string commissiontNumber = orderInformations.GetCommissionNumber();
            string softWareVersion = orderInformations.GetNameAndVersionSoftware();
            string attachedFilesPathsList = Path.Combine(Order.orderDir, OrderTransmission.OrderZipFileName);           
            

            DialogResult dialogResult = MessageBox.Show("Voulez-vous envoyer la commande ?", "InSitu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
               
                    bool bSend = this.CurrentAppli.EmailSend("", //Contact name if exist
                                                             recipientAddresses,
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
            
            orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
            Order.orderDir = orderInformations.GetOrderDir();

            Articles articles = SupplierArticleValidInScene();

            if (this.IsGenerateOrders(articles))
            {
                this.mainForm = new MainForm(orderInformations);
                this.Main(callParamsBlock, articles);
              
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
                //add linear articles cause not in heading and need to all real object in the scene
                string linearIDs = this.CurrentAppli.SceneComponent.SceneGetObjectIdList(null, KD.Analysis.FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
                Articles supplierLinearArticles = new Articles(CurrentAppli, linearIDs);

                foreach (Article article in supplierLinearArticles)
                {
                    SegmentClassification segmentClassification = new SegmentClassification(article);

                    if (segmentClassification.IsArticleLinear() && !article.HasParent() && article.IsValid)
                    {
                        if (!articles.Contains(article))
                        {
                            articles.Add(article);
                        }
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

            if (!String.IsNullOrEmpty(MainForm.EmailTo))
            {
                if (!this.IsMailAddressValid(MainForm.EmailTo) || !this.IsMailAddressValid(MainForm.EmailCc))
                {
                    MessageBox.Show("Adresse mail non valide.", "InSitu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Main(callParamsBlock, articles);                 
                }               
            }        

            if (!MainForm.Cancel)
            {
                OrderWrite.segmentNumberBetweenUNHandUNT = 0;
                RFF_A.refPosList.Clear();

                orderInformationsFromArticles = new OrderInformations(this.CurrentAppli, callParamsBlock, articles);

                fileEDI = new FileEDI(this.CurrentAppli, orderInformations.GetSupplierName(), orderInformationsFromArticles);
                if (fileEDI.csvPairingFileReader == null)
                {
                    _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.FalseLowerCase, OrderKey.GenerateOrder);
                    return;
                }

                orderWrite = new OrderWrite(this.CurrentAppli, orderInformations, orderInformationsFromArticles, articles, fileEDI);

                // put method at the end for ??
                // control this if miss some infos in EDI file
                orderWrite.BuildEDI(articles);
                orderWrite.EDIOrderFileStream();

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

                // put method ahead for list RFF_A object refpos number
                // control this if miss some infos in EDI file
                //orderWrite.BuildEDI(articles);
                //orderWrite.EDIOrderFileStream();

                orderWrite.ZIPOrderFile();
            }
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
        private bool IsMailAddressValid(string addresses)
        {
            //this test is for copy adress valid when empty. For the principal address the test is alreday do.
            if (String.IsNullOrEmpty(addresses))
            {
                return true;
            }

            addresses = addresses.Replace(KD.CharTools.Const.Comma, KD.CharTools.Const.SemiColon);
            string[] recipientsAddress = addresses.Split(KD.CharTools.Const.SemiColon);

            foreach (string address in recipientsAddress)
            {
                if (!address.Contains("@"))
                {
                    return false;
                }
            }
            return true;
        }
    }

}