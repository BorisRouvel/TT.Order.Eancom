using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.IO;

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
        BuildCommon buildCommon = null;
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

            string supplierName = orderInformations.GetSupplierName();
            string generateOrder = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.GenerateOrder);
            string supplierEmail = Order._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(OrderKey.SupplierEmail);

            if (!String.IsNullOrEmpty(supplierEmail))
            {
                orderInformations.SetSupplierEmail(supplierEmail);
            }

            if (!String.IsNullOrEmpty(generateOrder))
            {
                bool.TryParse(generateOrder, out bool isGenerateOrder);
               
                if (isGenerateOrder)
                {                    
                    string retailerNumber = orderInformations.GetRetailerNumber();
                    Order.orderDir = orderInformations.GetOrderDir();                   
                    KD.Config.IniFile ordersIniFile = new KD.Config.IniFile(Path.Combine(Order.orderDir, FileEDI.IniOrderFileName));

                    MainForm.EmailTo = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailToKey + supplierName);
                    MainForm.EmailCc = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, Eancom.FileEDI.emailCcKey + supplierName);
                    MainForm.MandatoryDeliveryInformation = ordersIniFile.ReadValue(Eancom.FileEDI.ediSection, OrderKey.MandatoryDeliveryRetailerInformation + retailerNumber);

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

        private string GetCustomerNumber()
        {
            string customerNumber = orderInformations.GetRetailerGLN();
            if (String.IsNullOrEmpty(customerNumber))
            {
                customerNumber = orderInformations.GetRetailerNumber();
            }
            return customerNumber;
        }
        private void SendMail(string recipientAddresses, string ccAdress)
        {
            string customerNumber = this.GetCustomerNumber();
            string commissiontNumber = orderInformations.GetCommissionNumber();
            string softWareVersion = orderInformations.GetNameAndVersionSoftware();
            string attachedFilesPathsList = Path.Combine(Order.orderDir, OrderTransmission.OrderZipFileName);            

            DialogResult dialogResult = MessageBox.Show("Voulez-vous envoyer la commande ?", "InSitu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (String.IsNullOrEmpty(customerNumber))
                {
                    MessageBox.Show("Vous devez renseigner le 'Numéro client'." , "InSitu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.NoSendMailMessage();

                    return;
                }               

                bool bSend = this.CurrentAppli.EmailSend("", //Contact name if exist
                                                             recipientAddresses,
                                                             ccAdress, //cc
                                                             OrderTransmission.HeaderSubject + "<" + customerNumber + "><" + commissiontNumber + "><" + softWareVersion + ">",
                                                             String.Empty, //Body message
                                                             attachedFilesPathsList, //AttachedFilesPathsList
                                                             String.Empty, //AttachedFilesNamesList
                                                             true); // show dialog

                if (bSend)
                {
                    this.SendMailMessage();
                }
                else
                {
                    this.NoSendMailMessage();
                }
            }           
        }

        private void SendMailMessage()
        {
            MessageBox.Show("Commande envoyée !", "InSitu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void NoSendMailMessage()
        {
            MessageBox.Show("Commande non envoyée !", "InSitu", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                //Get all supplier articles of all heading except linears
                Articles supplierArticles = new Articles(CurrentAppli, IDs);
                foreach (Article article in supplierArticles)
                {
                    SegmentClassification segmentClassification = new SegmentClassification(article);
                    if (article.IsValid  && article.Type != 17)//&& !segmentClassification.IsArticleLinear()) //
                    {
                        articles.Add(article);
                    }
                }

                //Must relist the articles, del linears and add graphic linear
                articles = this.DelLinearsArticles(articles);
                //add linear articles cause not in heading and need to all real object in the scene
                articles = this.AddLinearsGraphikArticles(articles);

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

            string supplierEmail = orderInformations.GetSupplierEmail();            
            //if (!String.IsNullOrEmpty(supplierEmail))
            //{
            //    string supplierName = orderInformations.GetSupplierName();
            //    MessageBox.Show("Veuillez supprimer l'adresse email dans le fournisseur '" + supplierName + "'", "InSitu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.FalseLowerCase, OrderKey.GenerateOrder);
            //    _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(supplierEmail, OrderKey.SupplierEmail);

            //    orderInformations.SetSupplierEmail(String.Empty);
            //    //supplierEmail = orderInformations.GetSupplierEmail();
                
            //    return false;
            //}
            //else
            //{
            //    _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(null, OrderKey.SupplierEmail);
            //}

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

                buildCommon = new BuildCommon(this.CurrentAppli);
                orderWrite = new OrderWrite(this.CurrentAppli, orderInformations, buildCommon, orderInformationsFromArticles, articles, fileEDI);

                // the order of the choice is important cause BuildEdi need jpg file and EGI need EDI number's article like 21 and 21.1
                if (MainForm.IsChoiceExportPlan)
                {
                    BuildPlan buildPlan = new BuildPlan(this.CurrentAppli, buildCommon);
                    buildPlan.Generate();
                }
                if (MainForm.IsChoiceExportElevation)
                {
                    BuildElevation buildElevation = new BuildElevation(this.CurrentAppli, buildCommon);
                    buildElevation.Generate();
                }
                if (MainForm.IsChoiceExportPerspective)
                {
                    BuildPerspective buildPerspective = new BuildPerspective(this.CurrentAppli, buildCommon);
                    buildPerspective.Generate();
                }
                if (MainForm.IsChoiceExportOrder)
                {
                    BuildOrder buildOrder = new BuildOrder(this.CurrentAppli, orderInformations);
                    buildOrder.Generate();
                }

                // put method after for take generate file information (*.jpg...)
                // control this if miss some infos in EDI file
                orderWrite.BuildEDI(articles);
                orderWrite.EDIOrderFileStream();

                if (MainForm.IsChoiceExportEGI)
                {
                    orderWrite.BuildEGI(articles);
                    orderWrite.EGIOrderFile();
                }
  
                // put method ahead for list RFF_A object refpos number
                // control this if miss some infos in EDI file
                //orderWrite.BuildEDI(articles);
                //orderWrite.EDIOrderFileStream();

                OrderZip orderZip = new OrderZip();
                orderZip.ZIPFile();
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

        private Articles DelLinearsArticles(Articles articles)
        {
            Articles articlesReturn = new Articles(articles);

            foreach (Article article in articles)
            {
                SegmentClassification segmentClassification = new SegmentClassification(article);
                if (segmentClassification.IsArticleLinear())
                {
                    articlesReturn.Remove(article);
                }
            }
            return articlesReturn;
        }
        private Articles AddLinearsGraphikArticles(Articles articles)
        {
            string IDs = this.CurrentAppli.SceneComponent.SceneGetObjectIdList(null, KD.Analysis.FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            Articles articlesList = new Articles(CurrentAppli, IDs);
            foreach (Article article in articlesList)
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
            return articles;
        }
    }

}