﻿using System.Windows.Forms;
using System.IO;
using System;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------
using KD.Model;

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

        /// <summary>
        /// The "GenerateOrder" function belonging to the "Order" class is the entry point of the supplier orders management module.
        /// If the DLL is selected in the "Computer file" option box corresponding to a supplier in the "Suppliers" dialog box,
        /// the following "GenerateOrder" function is called when the user generates an order for that supplier running 
        /// the "File | Orders to suppliers | Generate".
        /// </summary>
        /// <param name="iCallParamsBlock"></param>
        /// <returns></returns>
        public bool GenerateOrder(int callParamsBlock)
        {
            _pluginWord = new KD.Plugin.Word.Plugin();
            _pluginWord.InitializeAll(callParamsBlock);

            this.mainForm = new MainForm();
            this.Main(callParamsBlock);

            MessageBox.Show("Terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
        public bool ProcessOrder(int callParamsBlock)
        {
            this.DelOrderFile(orderFile);

            return true;
        }

        //private void PDF(int callParamsBlock)
        //{
        //    orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
        //    int supplierRank = this.CurrentAppli.GetSupplierRankFromIdent(this.orderInformations.GetSupplierName());

        //    if (supplierRank != KD.Const.UnknownId)
        //    {
        //        string pdfFlagState = this.CurrentAppli.SupplierGetInfo(supplierRank, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);

        //        bool bPdfFlag = this.CurrentAppli.SupplierSetInfo(supplierRank, KD.StringTools.Const.One, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);
        //    }
        //}

        private void Main(int callParamsBlock)
        {
            this.mainForm.ShowDialog();

            OrderWrite.segmentNumberBetweenUNHandUNT = 0;
            RFF_A.refPosList.Clear();

            orderDir = this.CurrentAppli.GetCallParamsInfoDirect(callParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);

            orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
            Articles articles = SupplierArticleValidInScene(orderInformations);
            orderInformationsFromArticles = new OrderInformations(this.CurrentAppli, callParamsBlock, articles);

            fileEDI = new FileEDI(this.CurrentAppli, orderInformations.GetSupplierName(), orderInformationsFromArticles);

            orderWrite = new OrderWrite(this.CurrentAppli, orderInformations, orderInformationsFromArticles, articles, fileEDI);

            orderWrite.BuildEDI(articles);
            orderWrite.EDIOrderFile();

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
                //orderWrite.;
            }
            if (MainForm.IsChoiceExportOrder)
            {
                orderWrite.BuildOrder();
            }

            orderWrite.ZIPOrderFile();
        }
        private void DelOrderFile(string file)
        {
            if (!String.IsNullOrEmpty(file))
            {
                file += OrderTransmission.ExtensionTXT;
                if (File.Exists(Path.Combine(Order.orderDir, file)))
                {
                    File.Delete(Path.Combine(Order.orderDir, file));
                }
            }
        }
        private Articles SupplierArticleValidInScene(OrderInformations orderInformations)
        {
            Articles articles = new Articles();
            //OrderInformations orderInformations = new OrderInformations(this.CurrentAppli, callParamsBlock);
            string supplierId = orderInformations.GetSupplierName();
            string IDs = String.Empty;

            int objSupplierNb = this.CurrentAppli.Scene.HeadingGetObjectsNb((int)KD.SDK.SceneEnum.ObjectList.ALLHEADINGS, false);
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
                    if (article.IsValid)
                    {
                        articles.Add(article);
                    }
                }
            }
            return articles;
        }

    }

}