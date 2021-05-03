using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using KD.SDKComponent;
using KD.Analysis;
using KD.Model;

using Eancom;


namespace Ord_Eancom
{
    public class OrderWrite
    {
        public const string Key_SetCode = "KeySetCode";
        public const string Key_SetPrice = "KeySetPrice";
        public const string BlockSetCustomInfo = "BlockSet";

        private AppliComponent _currentAppli;
        private int _callParamsBlock;

        Encoding encodingOrder = Encoding.Default;
        static List<string> structureLineEDIList = new List<string>();
        static List<string> structureLineEGIList = new List<string>();       

        readonly OrderInformations _orderInformations = null;
        readonly OrderInformations _orderInformationsFromArticles = null;
        readonly BuildCommon _buildCommon = null;
        FileEDI _fileEDI = null;
        readonly Eancom.UtilitySegment _utility = null;

        private int consecutiveNumbering = 1;
        public static int segmentNumberBetweenUNHandUNT = 0;

        List<Wall> wallsList = new List<Wall>();
        List<Article> doorsList = new List<Article>();
        List<Article> windowsList = new List<Article>();
        List<Article> recessesList = new List<Article>();

        public AppliComponent CurrentAppli
        {
            get
            {
                return _currentAppli;
            }
            set
            {
                _currentAppli = value;

            }
        }       
        public int CallParamsBlock
        {
            get
            {
                return _callParamsBlock;
            }
            set
            {
                _callParamsBlock = value;

            }
        }

        //public static double sceneDimX = 0.0;
        //public static double sceneDimY = 0.0;        
        //public static double sceneDimZ = 0.0;
        //public static double angleScene = 0.0;

        private readonly string x = String.Empty;
        private readonly string y = String.Empty;
        private readonly string z = String.Empty;
        private readonly string ab = String.Empty;

        public static double posX = 0.0;
        public static double posY = 0.0;
        public static double posZ = 0.0;
        public static double a = 0.0;

        private double dimX = 0.0;
        private double dimY = 0.0;
        private double dimZ = 0.0;

        public static string version = String.Empty;


        #region //Structure EDI
        Eancom.UNA uNA = null;
        Eancom.UNB uNB = null;
        Eancom.UNH uNH = null;
        Eancom.BGM bGM = null;
        Eancom.DTM dTM = null;
        Eancom.FTX_H fTX_H = null;
        Eancom.RFF_H rFF_H = null;
        Eancom.NAD nAD = null;
        Eancom.CTA cTA = null;
        Eancom.COM cOM = null;

        Eancom.LIN_H lIN_H = null;
        Eancom.PIA_H pIA_H = null;

        Eancom.LIN_A lIN_A = null;
        Eancom.PIA_A pIA_A = null;
        Eancom.IMD iMD = null;
        Eancom.MEA mEA = null;
        Eancom.QTY qTY = null;
        Eancom.FTX_A fTX_A = null;
        Eancom.RFF_A rFF_A = null;

        Eancom.UNS uNS = null;
        Eancom.UNT uNT = null;
        Eancom.UNZ uNZ = null;
        #endregion

        public OrderWrite(AppliComponent appli, OrderInformations orderInformations, BuildCommon buildCommon, OrderInformations orderInformationsFromArticles, Articles articles, FileEDI fileEDI)
        {
            _currentAppli = appli;
            _orderInformations = orderInformations;
            _orderInformationsFromArticles = orderInformationsFromArticles;
            _fileEDI = fileEDI;
            _utility = new Eancom.UtilitySegment();

            this.InitializeEancomStructure();

            OrderWrite.version = OrderTransmission.VersionEdigraph_1_51.Split(KD.CharTools.Const.Underscore)[1];

            _buildCommon = buildCommon;
        }

        #region //EDI      
        private void InitializeEancomStructure()
        {
            this.ClearStructureEDIList();

            uNA = new Eancom.UNA();
            uNB = new Eancom.UNB(_orderInformations, _fileEDI);
            uNH = new Eancom.UNH(_orderInformations);
            bGM = new Eancom.BGM(_orderInformations);
            dTM = new Eancom.DTM(_orderInformations);
            fTX_H = new Eancom.FTX_H(_orderInformations);
            rFF_H = new Eancom.RFF_H(_orderInformations);
            nAD = new Eancom.NAD(_orderInformations, _fileEDI);
            cTA = new Eancom.CTA(_orderInformations);
            cOM = new Eancom.COM(_orderInformations, _fileEDI);

            lIN_H = new Eancom.LIN_H(Convert.ToString(this.consecutiveNumbering));
            pIA_H = new Eancom.PIA_H(_orderInformationsFromArticles, _fileEDI);

            this.consecutiveNumbering += 1;
            lIN_A = new Eancom.LIN_A(_orderInformationsFromArticles, Convert.ToString(this.consecutiveNumbering), _fileEDI);
            pIA_A = new Eancom.PIA_A(_orderInformationsFromArticles, _fileEDI);
            iMD = new Eancom.IMD(_orderInformationsFromArticles);
            mEA = new Eancom.MEA(_fileEDI);
            qTY = new Eancom.QTY();
            fTX_A = new Eancom.FTX_A(_orderInformationsFromArticles);
            rFF_A = new Eancom.RFF_A(_orderInformationsFromArticles);

            uNS = new Eancom.UNS();
            uNT = new Eancom.UNT(_orderInformations);
            uNZ = new Eancom.UNZ(_orderInformations, _fileEDI);

            this.ClearStructureEGIList();

        }
        private void ClearStructureEDIList()
        {
            structureLineEDIList.Clear();
        }
        private void SetLineEDIList(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                structureLineEDIList.Add(text); //OrderWrite.segmentNumberBetweenUNHandUNT + " // " + 
            }
        }
        private void SetLineEDIList(List<string> list)
        {
            if (list.Count > 0)
            {
                foreach (string line in list)
                {
                    structureLineEDIList.Add(line);
                }
            }
        }
        private void SetLine_NAD_Delivery()
        {
            if (MainForm.IsChoiceRetailerDelivery)
            {
                if (!nAD.IsBYequalDDP())
                {
                    SetLineEDIList(nAD.Add_Delivery_DP(1));
                }
            }
            else if (MainForm.IsChoiceCustomerDelivery)
            {
                SetLineEDIList(nAD.Add_CustomerDelivery_DP());
            }
        }
        private void WriteLineInFileEDI(FileStream fs, string text)
        {
            // Convert the string into a byte array.
            byte[] encodingBytes = encodingOrder.GetBytes(text);

            // Perform the conversion from one encoding to the other.
            byte[] encodingConvertByte = Encoding.Convert(encodingOrder, encodingOrder, encodingBytes);

            fs.Write(encodingConvertByte, 0, encodingOrder.GetByteCount(text));
        }

        public void BuildEDI(Articles articles)
        {
            this.HeaderEDI();
            this.HeaderData();
            this.LineData(articles);
            this.EndEDI(); //here set segment number between UNH and UNT include
        }

        private void HeaderEDI()
        {
            SetLineEDIList(uNA.Add());

            SetLineEDIList(uNB.Add());
            SetLineEDIList(uNH.Add());
            SetLineEDIList(bGM.Add());

            SetLineEDIList(dTM.Add());
            SetLineEDIList(dTM.Add_Delivery());
            SetLineEDIList(dTM.Add_Installation());

            SetLineEDIList(fTX_H.Add());
            SetLineEDIList(fTX_H.AddComment());

            SetLineEDIList(rFF_H.Add_CR());
            SetLineEDIList(rFF_H.Add_UC());

            SetLineEDIList(nAD.Add_SU());

            SetLineEDIList(cTA.Add_Supplier_OC());

            SetLineEDIList(cOM.Add_Supplier_TE());
            SetLineEDIList(cOM.Add_Supplier_FX());
            SetLineEDIList(cOM.Add_Supplier_EM());

            SetLineEDIList(nAD.Add_BY(1));

            SetLineEDIList(cTA.Add_Seller_OC());

            SetLineEDIList(cOM.Add_Retailer_TE());
            SetLineEDIList(cOM.Add_Retailer_FX());
            SetLineEDIList(cOM.Add_Retailer_EM());

            this.SetLine_NAD_Delivery();
        }
        private void HeaderData()
        {
            SetLineEDIList(lIN_H.Add());

            SetLineEDIList(pIA_H.Add_ManufacturerID());
            SetLineEDIList(pIA_H.Add_SerieNo());
            SetLineEDIList(pIA_H.Add_CatalogID());
            SetLineEDIList(pIA_H.Add_ModelCodeAndName());
            SetLineEDIList(pIA_H.Add_FinishCodeAndName());
            SetLineEDIList(pIA_H.Add_PlinthHeight());
        }
        private void LineData(Articles articles)
        {
            bool hasBlockSet = false;
            Article blockSetArticle = null;
            //string blockSetReference = CurrentAppli.Scene.ObjectGetCustomInfo(articles[0].ObjectId, OrderWrite.Key_SetCode);
            string blockSetReference = CurrentAppli.Scene.SceneGetCustomInfo(OrderWrite.Key_SetCode);           

            if (!hasBlockSet && !String.IsNullOrEmpty(blockSetReference))
            {
                hasBlockSet = true;
                int blockSetId = CurrentAppli.Scene.EditPlaceOpenObject(String.Empty, blockSetReference, 0, String.Empty, String.Empty, 0.0, 0.0, 0, 1.0, -1);
                if (blockSetId != KD.Const.UnknownId)
                {
                    blockSetArticle = new Article(CurrentAppli, blockSetId);                  
                    bool binfo = CurrentAppli.Scene.ObjectSetCustomInfo(blockSetId, OrderWrite.BlockSetCustomInfo, OrderWrite.Key_SetCode);
                    articles.Insert(0, blockSetArticle);
                }
            }

            foreach (Article article in articles)
            {
                if (!article.KeyRef.StartsWith(KD.StringTools.Const.Underscore))
                {
                    if (!article.KeyRef.EndsWith(OrderConstants.HandleName))
                    {
                        //Test to give access fileEDI each article
                        if (!hasBlockSet)
                        {
                            _fileEDI = new FileEDI(this.CurrentAppli, _orderInformationsFromArticles, article.Ref);
                            if (_fileEDI.csvPairingFileReader == null)
                            {
                                article.CurrentAppli.Scene.SceneSetCustomInfo(KD.StringTools.Const.FalseLowerCase, OrderKey.GenerateOrder);
                                return;
                            }
                        }                       

                        SetLineEDIList(lIN_A.Add_EN(article));
                        //SetLineEDIList(lIN_A.Add_SG(article));

                        SetLineEDIList(pIA_A.Add_ExternalManufacturerID(article));
                        SetLineEDIList(pIA_A.Add_ExternalSerieNo(article));
                        SetLineEDIList(pIA_A.Add_TypeNo(article));
                        SetLineEDIList(pIA_A.Add_EDPNumber(article));
                        SetLineEDIList(pIA_A.Add_Hinge(article));
                        SetLineEDIList(pIA_A.Add_ConstructionID(article));
                        SetLineEDIList(pIA_A.Add_VisibleSide(article));
                        SetLineEDIList(pIA_A.Add_FinishCodeAndName(article));
                        SetLineEDIList(pIA_A.Add_LongPartType(article));

                        consecutiveNumbering += 1;
                        Eancom.LIN_A._consecutiveNumbering = consecutiveNumbering.ToString();

                        if (!hasBlockSet)
                        {
                            SetLineEDIList(iMD.Add(article));
                            SetLineEDIList(mEA.Add(article));
                        }
                        SetLineEDIList(qTY.Add(article));
                        SetLineEDIList(fTX_A.Add(article));
                        SetLineEDIList(rFF_A.Add_ReferenceNumber(article));
                        SetLineEDIList(rFF_A.Add_PlanningSystemItemNumber(article));

                        SegmentClassification segmentClassification = new SegmentClassification(article);
                        if (segmentClassification.IsArticleWorkTop())
                        {
                            OrderInformations articleInformations = new OrderInformations(article, segmentClassification);
                            List<string> assemblyCodeAndNameList = articleInformations.GetAssemblyWorktopCodeAndNameList();

                            if (assemblyCodeAndNameList != null && assemblyCodeAndNameList.Count > 0)
                            {
                                foreach (string assemblyCodeAndName in assemblyCodeAndNameList)
                                {
                                    string assemblyCode = assemblyCodeAndName.Split(KD.CharTools.Const.SemiColon)[0];
                                    string articleReferenceKey = _fileEDI.ArticleReferenceKey(assemblyCode, 1);

                                    if (!String.IsNullOrEmpty(assemblyCode) && _fileEDI.ManufacturerID() == IDMManufacturerID.Discac)
                                    {
                                        assemblyCode = _fileEDI.ArticleReferenceKey(assemblyCode, 0);
                                        articleReferenceKey = _fileEDI.ArticleReferenceKey(assemblyCode, 1);
                                    }

                                    if (!String.IsNullOrEmpty(articleReferenceKey))
                                    {
                                        SetLineEDIList(lIN_A.Add_WorktopAssemblyNumberEN(article, assemblyCode));

                                        SetLineEDIList(pIA_A.Add_WorktopAssemblyTypeNo(assemblyCode));
                                        SetLineEDIList(pIA_A.Add_WorktopAssemblyEDPNumber(assemblyCode));
                                        SetLineEDIList(pIA_A.Add_WorktopAssemblyHinge(assemblyCode));
                                        SetLineEDIList(pIA_A.Add_WorktopAssemblyConstructionID(assemblyCode));

                                        consecutiveNumbering += 1;
                                        Eancom.LIN_A._consecutiveNumbering = consecutiveNumbering.ToString();

                                        string assemblyName = assemblyCodeAndName.Split(KD.CharTools.Const.SemiColon)[1];

                                        SetLineEDIList(iMD.Add_WorktopAssemblyNumber(assemblyName));
                                        SetLineEDIList(qTY.Add_WorktopAssemblyNumber(article));
                                        SetLineEDIList(rFF_A.Add_WorktopAssemblyReferenceNumber(article)); //we must find a solution for unique ref cause it s a finish not a article objectId
                                        SetLineEDIList(rFF_A.Add_WorktopAssemblyPlanningSystemItemNumber(article, assemblyCode));
                                    }
                                }
                            }
                        }
                        else if (segmentClassification.IsArticleLinear())
                        {
                            //For instead i have no idea to add number for a linear
                            SetLineEDIList(rFF_A.Add_LinearPlanningSystemItemNumber(article));
                        }

                        if (hasBlockSet)
                        {
                            hasBlockSet = false;
                        }
                    }
                }
            }

            if (blockSetArticle != null)
            {
                hasBlockSet = false;
                blockSetArticle.DeleteFromScene();
            }
        }
        private void EndEDI()
        {
            SetLineEDIList(uNS.Add());
            SetLineEDIList(uNT.Add());
            SetLineEDIList(uNZ.Add());
        }

        public void EDIOrderFileStream()
        {
            using (FileStream fs = new FileStream(Path.Combine(Order.orderDir, OrderTransmission.OrderEDIFileName), FileMode.Create))
            {
                foreach (string line in structureLineEDIList)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        WriteLineInFileEDI(fs, line);
                    }
                }
                fs.Close();
                fs.Dispose();
            }
        }

        #endregion


        #region //ZIP for EDI
    
        #endregion


        #region //EGI
        private void ClearStructureEGIList()
        {
            structureLineEGIList.Clear();
            wallsList.Clear();
            doorsList.Clear();
            windowsList.Clear();
            recessesList.Clear();
        }
        private void SetLineEGIList(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                structureLineEGIList.Add(text);
            }
        }
        private void SetLineEGIList(List<string> list)
        {
            if (list.Count > 0)
            {
                foreach (string line in list)
                {
                    structureLineEGIList.Add(line);
                }
            }
        }
        private void WriteLineInFileEGI(FileStream fs, string text)
        {
            fs.Write(encodingOrder.GetBytes(text), 0, encodingOrder.GetByteCount(text));
        }

        public void BuildEGI(Articles articles)
        {
            //Set Wall, Door, Windows, Recess and Hindrance List
            this.GetContraintsList();

            this.HeaderEGI();           
            this.SetWallInformations(wallsList);
            this.SetDoorInformations(doorsList);
            this.SetWindowInformations(windowsList);
            this.SetRecessInformations(recessesList);
            //this.SetHindranceInformations();
            this.SetArticleInformations(articles);

            _buildCommon.ResetReference();
        }

       
        private void GetContraintsList()
        {
            int objectNumber = this.CurrentAppli.Scene.SceneGetObjectsNb();
            for (int objectRank = 0; objectRank < objectNumber; objectRank++)
            {
                int objectID = this.CurrentAppli.Scene.SceneGetObjectId(objectRank);
                int objectType = Convert.ToInt16(this.CurrentAppli.Scene.ObjectGetInfo(objectID, KD.SDK.SceneEnum.ObjectInfo.TYPE));
                    
                if (objectType == Wall.Const.TypeWall)
                {                   
                    wallsList.Add(new Wall(this.CurrentAppli, objectID));
                }
                else if (objectType == (int)KD.SDK.SceneEnum.ObjectType.DOOR)
                {                   
                    doorsList.Add(new Article(this.CurrentAppli, objectID));
                }
                else if (objectType == (int)KD.SDK.SceneEnum.ObjectType.WINDOW)
                {                    
                    windowsList.Add(new Article(this.CurrentAppli, objectID));
                }
                else if (objectType == (int)KD.SDK.SceneEnum.ObjectType.RECESS)
                {
                    recessesList.Add(new Article(this.CurrentAppli, objectID));
                }
            }            
        }
       
        
        //Header
        public void HeaderEGI()
        {
            #region // INFO
            //[Global]
            //Version=EDIGRAPH_V1.50
            //Name=31/1/1.1
            //Number=31/1/1
            //DrawDate=28/03/2019
            //DrawTime=15?:17?:05
            //RoomHeight=2500.00
            //Manufacturer=1201
            //System=INSITU 9.0
            //[Wall_0001]
            //RefPntX=0.00
            //RefPntY=0.00
            //RefPntZ=0.00
            //Width=3200.00
            //Height=2500.00
            //Depth=300.00
            //AngleZ=0.00
            #endregion
            DateTime dateTime = DateTime.Now;
            //soumettre au detecteur de char ?? _orderInformations.ReleaseChar()
            structureLineEGIList.Add(KD.StringTools.Format.Bracketed(SegmentName.Global) + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.Version + KD.StringTools.Const.EqualSign + OrderTransmission.VersionEdigraph_1_51 + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.Name + KD.StringTools.Const.EqualSign + _orderInformations.GetCommissionNumber() + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.Number + KD.StringTools.Const.EqualSign + _orderInformations.GetOrderNumber() + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.DrawDate + KD.StringTools.Const.EqualSign + dateTime.ToString(OrderConstants.FormatDate_d_M_y) + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.DrawTime + KD.StringTools.Const.EqualSign + dateTime.ToString(OrderConstants.FormatTime_H_m_s) + Separator.NewLine);

            string roomHeight = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
            string room = Convert.ToInt32(roomHeight).ToString(SegmentFormat.DotDecimal);
            structureLineEGIList.Add(ItemKey.RoomHeight + KD.StringTools.Const.EqualSign + Tools.ConvertCommaToDot(room) + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.Manufacturer + KD.StringTools.Const.EqualSign + _fileEDI.ManufacturerID() + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.System + KD.StringTools.Const.EqualSign + _orderInformations.GetNameAndVersionSoftware() + Separator.NewLine);
        }
  
        //Wall
        public void SetWallInformations(List<Wall> list)
        {
            _buildCommon.SetSceneReference(OrderWrite.version);

            int index = 1;
            foreach (Wall wall in list)
            {
                _buildCommon.SetReference();

                structureLineEGIList.Add(Indexation(SegmentName.Wall_, index));
                structureLineEGIList.Add(PositionX(wall.PositionX));
                structureLineEGIList.Add(PositionY(wall.PositionY));
                structureLineEGIList.Add(PositionZ(wall.PositionZ));
                structureLineEGIList.Add(DimensionX(wall.DimensionX));
                structureLineEGIList.Add(DimensionZ(wall.DimensionZ));
                structureLineEGIList.Add(DimensionY(wall.DimensionY));
                structureLineEGIList.Add(Angle(wall.AngleOXY));
                index += 1;
            }
        }
  
        //Door
        public void SetDoorInformations(List<Article> list)
        {
            _buildCommon.SetSceneReference(OrderWrite.version);

            int index = 1;
            foreach (Article door in list)
            {
                _buildCommon.SetReference();

                structureLineEGIList.Add(Indexation(SegmentName.Door_, index));
                structureLineEGIList.Add(PositionX(door.PositionX));
                structureLineEGIList.Add(PositionY(door.PositionY));
                structureLineEGIList.Add(PositionZ(door.PositionZ));
                structureLineEGIList.Add(DimensionX(door.DimensionX));
                structureLineEGIList.Add(DimensionZ(door.DimensionZ));
                structureLineEGIList.Add(DimensionY(door.DimensionY));//
                structureLineEGIList.Add(Angle(door.AngleOXY));

                structureLineEGIList.Add(DoorHinge(door));
                structureLineEGIList.Add(Opening(door));
                structureLineEGIList.Add(WallRefNo(door));
                structureLineEGIList.Add(RefPntXRel(door));
                structureLineEGIList.Add(RefPntYRel(door));
                structureLineEGIList.Add(RefPntZRel(door));

                index += 1;
            }
        }

        private string DoorHinge(Article article)
        {
            string hinge = String.Empty;
            string hingeType = article.GetStringInfo(KD.SDK.SceneEnum.ObjectInfo.HANDINGTYPE);
            switch (hingeType)
            {
                case "0":
                    hinge = ItemValue.None_Hinge;
                    break;
                case "1":
                    hinge = ItemValue.Left_Hinge;
                    break;
                case "2":
                    hinge = ItemValue.Right_Hinge;
                    break;
                default:
                    return null;
            }
            return ItemKey.Hinge + KD.StringTools.Const.EqualSign + hinge + Separator.NewLine;
        }       
     
        //Windows
        public void SetWindowInformations(List<Article> list)
        {
            _buildCommon.SetSceneReference(OrderWrite.version);

            int index = 1;
            foreach (Article window in list)
            {
                _buildCommon.SetReference();

                structureLineEGIList.Add(Indexation(SegmentName.Window_, index));
                structureLineEGIList.Add(PositionX(window.PositionX));
                structureLineEGIList.Add(PositionY(window.PositionY));
                structureLineEGIList.Add(PositionZ(window.PositionZ));
                structureLineEGIList.Add(DimensionX(window.DimensionX));
                structureLineEGIList.Add(DimensionZ(window.DimensionZ));
                structureLineEGIList.Add(DimensionY(window.DimensionY));//
                structureLineEGIList.Add(Angle(window.AngleOXY));

                structureLineEGIList.Add(WindowHinge(window));
                structureLineEGIList.Add(Opening(window));
                structureLineEGIList.Add(WallRefNo(window));
                structureLineEGIList.Add(RefPntXRel(window));
                structureLineEGIList.Add(RefPntYRel(window));
                structureLineEGIList.Add(RefPntZRel(window));

                index += 1;
            }
        }

        private string WindowHinge(Article article)
        {
            string hinge = String.Empty;
            string hingeType = article.GetStringInfo(KD.SDK.SceneEnum.ObjectInfo.HANDINGTYPE);
            switch (hingeType)
            {
                case "0":
                    hinge = ItemValue.None_Hinge;
                    break;
                case "1":
                    hinge = ItemValue.Left_Hinge;
                    break;
                case "2":
                    hinge = ItemValue.Right_Hinge;
                    break;
                case "3": // doesnt exist, i dont know how to get this hinge value
                    hinge = ItemValue.Bottom_Hinge;
                    break;
                case "4": // doesnt exist, i dont know how to get this hinge value
                    hinge = ItemValue.Top_Hinge;
                    break;
                default:
                    return null;
            }
            return ItemKey.Hinge + KD.StringTools.Const.EqualSign + hinge + Separator.NewLine;
        }

        //Recess
        public void SetRecessInformations(List<Article> list)
        {
            _buildCommon.SetSceneReference(OrderWrite.version);

            int index = 1;
            foreach (Article recess in list)
            {
                structureLineEGIList.Add(Indexation(SegmentName.Recess_, index));

                _buildCommon.SetReference();
                double posX = recess.PositionX;
                double posY = recess.PositionY;
                double ang = recess.AngleOXY;

                structureLineEGIList.Add(PositionX(posX));                
                structureLineEGIList.Add(PositionY(posY));
                structureLineEGIList.Add(PositionZ(recess.PositionZ));
                structureLineEGIList.Add(DimensionX(recess.DimensionX));
                structureLineEGIList.Add(DimensionZ(recess.DimensionZ));
                structureLineEGIList.Add(DimensionY(recess.DimensionY));//
                structureLineEGIList.Add(Angle(ang));               
               
                structureLineEGIList.Add(WallRefNo(recess));
                structureLineEGIList.Add(RefPntXRel(recess));
                structureLineEGIList.Add(RefPntYRel(recess));
                structureLineEGIList.Add(RefPntZRel(recess));

                index += 1;
            }
        }
        //Article
        private void MoveArticlePerRepere(Article article)
        {
            posX = article.PositionX;
            posY = article.PositionY;
            posZ = article.PositionZ;           
            a = (article.AngleOXY - 180);
            if (a < 0)
            {
                a += 360;
            }            

            double angle = 0.0;
            switch (OrderWrite.version.ToUpper())
            {
                case ItemValue.V1_50: //DISCAC                    
                    angle = 2 * Math.PI * (a / 360);
                    posX -= article.DimensionX * Math.Cos(angle);
                    posY -= article.DimensionX * Math.Sin(angle);
                    break;
                case ItemValue.V1_51: //FBD //BAUFORMAT
                    angle = 2 * Math.PI * (a / 360);
                    posX -= article.DimensionX * Math.Cos(angle);
                    posY -= article.DimensionX * Math.Sin(angle);
                    //x1 -= article.DimensionX;
                    //a -= article.AngleOXY;
                    break;
                default: //V1_51
                    angle = 2 * Math.PI * (a / 360);
                    posX -= article.DimensionX * Math.Cos(angle);
                    posY -= article.DimensionX * Math.Sin(angle);
                    break;
            }
        }
        private int GetArticlePolyCounter(Article article)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);
            string[] shapeList = segmentClassification.GetShapePointsList();
            if (shapeList.Length > 0)
            {
                return shapeList.Length;
            }
            return 0;
        }
   

        public void SetArticleInformations(Articles articles)
        {
            #region //INFO
            //[Article_0001]
            //Manufacturer=1201
            //Name=B2T70/A120
            //RefNo=16
            //RefPos=1.0
            //RefPntX=-0.00
            //RefPntY=-2350.00
            //RefPntZ=150.00
            //AngleZ=450.00
            //Shape=1
            //Measure_B=900.00
            //Measure_H=700.00
            //Measure_T=560.00
            //ConstructionType=N
            #endregion

            ////Must relist the articles, del linears and add graphic linear
            //articles = this.DelLinearsArticles(articles);
            ////add linear articles cause not in heading and need to all real object in the scene
            //articles = this.AddLinearsGraphikArticles(articles);

            _buildCommon.SetSceneReference(OrderWrite.version);

            int index = 1;

            foreach (Article article in articles)
            {
                SegmentClassification segmentClassification = new SegmentClassification(article);

                if ((!String.IsNullOrEmpty(article.Number.ToString()) && article.Number != KD.Const.UnknownId) || segmentClassification.IsArticleLinear())
                {
                    _buildCommon.SetReference();
                    //_orderInformations.ReleaseChar
                    structureLineEGIList.Add(Indexation(SegmentName.Article_, index));
                    structureLineEGIList.Add(ArticleManufacturer(article.KeyRef));                    
                    structureLineEGIList.Add(ArticleRefNo(article.ObjectId.ToString())); //RFF.LI  
                    structureLineEGIList.Add(ArticleRefPos(article.ObjectId.ToString())); // RFF.ON

                    int polyType = _buildCommon.GetArticlePolyType(article);
                    this.SetAllPositionsAndDimensions(segmentClassification, article, polyType);

                    bool hasPolytype = false;
                    string[] polyPoints = { };                    
                    if (polyType != KD.Const.UnknownId)
                    {
                        polyPoints = _buildCommon.GetArticlePolyPoint(article);                       
                        hasPolytype = true;
                    }                                      

                    structureLineEGIList.Add(PositionX(posX));
                    structureLineEGIList.Add(PositionY(posY));                    
                    structureLineEGIList.Add(PositionZ(posZ));
                    structureLineEGIList.Add(ArticleDimensionX(dimX));

                    if (segmentClassification.IsArticleSplashbackPanelShape() || segmentClassification.IsArticleSplashbackPanelShape2())
                    {
                        double dimT = dimY;
                        dimY = dimZ;
                        dimZ = dimT;
                    }

                    structureLineEGIList.Add(ArticleDimensionZ(dimZ));                    
                    structureLineEGIList.Add(ArticleDimensionY(article, dimY));

                    string shape = this.GetShapeNumberByType(article.KeyRef);
                    if (!shape.Equals(KD.StringTools.Const.Zero) && !segmentClassification.HasArticleCoinParent())
                    {
                        structureLineEGIList.AddRange(ArticleMeasureShapeList(article.KeyRef));                       
                    }
                    else if (!shape.Equals(KD.StringTools.Const.Zero) && segmentClassification.HasArticleCoinParent())
                    {
                        structureLineEGIList.Add(ArticleAngleDimensionX(dimX - article.DimensionX));
                        structureLineEGIList.Add(ArticleAngleDimensionXE(0.0));
                        structureLineEGIList.Add(ArticleAngleDimensionY(dimY - article.DimensionY));                        
                        structureLineEGIList.Add(ArticleAngleDimensionYE(0.0));
                    }

                    structureLineEGIList.Add(Angle(a));
                    structureLineEGIList.Add(ArticleReference(article.KeyRef));

                    string keyRef = article.KeyRef;
                    // here for test filer with coin cause dim b,b1,t,t1 on Nobilia (UPE) and Bauformat (UPE78C) it's different , 
                    //if (article.HasParent())
                    //{
                    //    segmentClassification = new SegmentClassification(article.Parent);
                    //    if (segmentClassification.IsArticleCornerFilerWithCoin())
                    //    {
                    //        keyRef = article.Parent.KeyRef;
                    //    }
                    //}
                    structureLineEGIList.Add(ArticleKeyReference(keyRef));
                    structureLineEGIList.Add(ArticleConstructionType(article.KeyRef));
                    structureLineEGIList.Add(ArticleHinge(article));

                    if (!shape.Equals(KD.StringTools.Const.Zero))
                    {
                        structureLineEGIList.Add(ArticleShape(shape));
                    }
  
                    if (hasPolytype)
                    {                       
                        structureLineEGIList.Add(ArticlePolyType(polyType));

                        int polyCounter = this.GetArticlePolyCounter(article);
                        structureLineEGIList.Add(ArticlePolyCounter(polyCounter));
                        
                        if (polyPoints != null && polyPoints.Length > 1)
                        {
                            int counter = 1;
                            foreach (string polyPoint in polyPoints)
                            {
                                if (!String.IsNullOrEmpty(polyPoint))
                                {
                                    string[] point = polyPoint.Split(KD.CharTools.Const.Comma);

                                    structureLineEGIList.Add(ArticlePolyPointX(counter, point[0]));
                                    structureLineEGIList.Add(ArticlePolyPointY(counter, point[1]));
                                    structureLineEGIList.Add(ArticlePolyPointZ(counter, point[2]));
                                    counter++;
                                }
                            }
                        }
                    }                   
                    
                    index += 1;
                }
            }
        }       

        private string ArticleManufacturer(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                keyRef = Tools.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
                string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
                if (articleInfos != null)
                {
                    string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                    return ItemKey.Manufacturer + KD.StringTools.Const.EqualSign + articleInfo[PairingTablePosition.ArticleSupplierId] + Separator.NewLine;
                }
            }
            return null;
        }
        private string ArticleReference(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                return ItemKey.Name + KD.StringTools.Const.EqualSign + Tools.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore) + Separator.NewLine; ;
            }
            return null;
        }
        private string ArticleKeyReference(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                return SegmentFormat.CommentChar + ItemKey.Name + KD.StringTools.Const.EqualSign + keyRef + Separator.NewLine; ;
            }
            return null;
        }
        private string ArticleRefNo(string iD)
        {
            if (!String.IsNullOrEmpty(iD))
            {
                return ItemKey.RefNo + KD.StringTools.Const.EqualSign + iD + Separator.NewLine; ;
            }
            return null;
        }
        private string ArticleRefPos(string iD)
        {
            foreach (string refPoses in RFF_A.refPosList)
            {
                string[] IdRefPos = refPoses.Split(KD.CharTools.Const.SemiColon);
                if (IdRefPos.Length > 1)
                {
                    string id = IdRefPos[0];
                    string refPos = IdRefPos[1];
                    if (iD.Equals(id))
                    {
                        return ItemKey.RefPos + KD.StringTools.Const.EqualSign + refPos + Separator.NewLine;
                    }
                }
            }
            return null;
        }

        private string ArticleShape(string str)
        {
            return ItemKey.Shape + KD.StringTools.Const.EqualSign + str.ToString() + Separator.NewLine;
        }
        private List<string> ArticleMeasureShapeList(string keyRef)
        {
            List<string> list = new List<string>();
            keyRef = Tools.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(Separator.ArticleFieldEDI);
                if (articleInfo.Length >= PairingTablePosition.EndArticleMeasure + 1) // new pairing table
                {
                    for (int index = PairingTablePosition.StartArticleMeasure; index <= PairingTablePosition.EndArticleMeasure; index++)
                    {
                        if (!String.IsNullOrEmpty(articleInfo[index]))
                        {
                            list.Add(ItemKey.Measure_ + Tools.ConvertCommaToDot(articleInfo[index].ToUpper()) + Separator.NewLine);
                        }
                    }
                }
                else if (articleInfo.Length > PairingTablePosition.StartArticleMeasure) // old pairing table
                {
                    for (int index = PairingTablePosition.StartArticleMeasure; index <= articleInfo.Length - 1; index++)
                    {
                        list.Add(ItemKey.Measure_ + Tools.ConvertCommaToDot(articleInfo[index].ToUpper()) + Separator.NewLine);
                    }
                }
                //else
                //{
                //    System.Windows.Forms.MessageBox.Show("Certaines informations ne sont pas correctes." + Separator.NewLine +
                //                                         "Veuillez télécharger la mise à jour du catalogue !", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                //}
            }            
            return list;
        }
        private string ArticleDimensionX(double value)
        {
            string data = ItemKey.Measure_B + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionX(double value)
        {
            string data = ItemKey.Measure_B1 + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionXE(double value)
        {
            string data = ItemKey.Measure_BE + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionZ(double value)
        {
            string data = ItemKey.Measure_H + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionY(Article article, double value)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);
            if (segmentClassification.IsArticleUnit() && !segmentClassification.IsArticleCornerOrAngleUnit() && !segmentClassification.IsArticleSplashbackPanel() && !segmentClassification.IsArticleSplashbackPanel2() )
            {
                value -= OrderConstants.FrontDepth;
            }
           // this for shape 27 with filer
            Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    SegmentClassification childClassification = new SegmentClassification(child);
                    if (childClassification.IsArticleFiler())
                    {
                        value += child.DimensionX;
                        break;
                    }
                }
            }
            
            string data = ItemKey.Measure_T + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionY(double value)
        {
            string data = ItemKey.Measure_T1 + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionYE(double value)
        {
            string data = ItemKey.Measure_TE + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleConstructionType(string keyRef)
        {
            keyRef = Tools.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                return ItemKey.ConstructionType + KD.StringTools.Const.EqualSign + articleInfo[PairingTablePosition.ArticleConstructionId] + Separator.NewLine;
            }
            return null;
        }
        private string ArticleHinge(Article article)
        {
            string hinge = String.Empty;
            string hingeType = article.GetStringInfo(KD.SDK.SceneEnum.ObjectInfo.HANDINGTYPE);
            switch (hingeType)
            {
                case "0":
                    return null;
                case "1":
                    hinge = ItemValue.Left_Hinge;
                    break;
                case "2":
                    hinge = ItemValue.Right_Hinge;
                    break;
                default:
                    return null;
            }
            return ItemKey.Hinge + KD.StringTools.Const.EqualSign + hinge + Separator.NewLine;
        }
        private string ArticlePolyType(int polyType)
        {
            return ItemKey.PolyType + KD.StringTools.Const.EqualSign + polyType.ToString() + Separator.NewLine;
        }
        private string ArticlePolyCounter(int polyCounter)
        {
            return ItemKey.PolyCounter + KD.StringTools.Const.EqualSign + polyCounter.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointX(int counter, string value)
        {
            return ItemKey.PolyPntX + KD.StringTools.Const.Underscore + counter.ToString(SegmentFormat.FourZero) + KD.StringTools.Const.EqualSign + value.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointY(int counter, string value)
        {
            return ItemKey.PolyPntY + KD.StringTools.Const.Underscore + counter.ToString(SegmentFormat.FourZero) + KD.StringTools.Const.EqualSign + value.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointZ(int counter, string value)
        {
            return ItemKey.PolyPntZ + KD.StringTools.Const.Underscore + counter.ToString(SegmentFormat.FourZero) + KD.StringTools.Const.EqualSign + value.ToString() + Separator.NewLine;
        }
        //Others
        public void EGIOrderFile()
        {
            using (FileStream fs = new FileStream(Path.Combine(Order.orderDir, OrderTransmission.OrderEGIFileName), FileMode.Create))
            {
                foreach (string line in structureLineEGIList)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        WriteLineInFileEGI(fs, line);
                    }
                }
                fs.Close();
                fs.Dispose();
            }
        }

        private string GetShapeNumberByType(string keyRef)
        {
            keyRef = Tools.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(Separator.ArticleFieldEDI);
                if (articleInfo.Length > PairingTablePosition.ArticleShape)
                {
                    return articleInfo[PairingTablePosition.ArticleShape];
                }
            }
            return KD.StringTools.Const.Zero;
        }        

        private void SetAllPositionsAndDimensions(SegmentClassification segmentClassification, Article article, int polytype)
        {
            if (!segmentClassification.HasArticleCoinParent())
            {
                if (!segmentClassification.IsArticleCornerFilerWithoutCoin())
                {
                    this.MoveArticlePerRepere(article);
                    this.SetDimensions(article);
                }
                else if (segmentClassification.IsArticleCornerFilerWithoutCoin())
                {
                    this.SetFilerPositions(article);
                    this.SetDimensions(article);
                }
            }
            else if (segmentClassification.HasArticleCoinParent())
            {
                this.SetFilerWithCoinPositionsAndDimensions(article);
            }

            posZ = _buildCommon.GetRealPositionZByPosedOnOrUnder(article);

            PolytypeValue polytypeValue = new PolytypeValue(polytype);
            if (polytypeValue.Z_Coordinate == PolytypeValue.Z_Coordinate_Top)
            {               
                 posZ = dimZ;
            }
        }
        private void SetFilerPositions(Article article)
        {
            posX = article.PositionX + CatalogConstante.FrontDepth;
            posY = article.PositionY + CatalogConstante.FrontDepth;
            posZ = article.PositionZ;
            a = article.AngleOXY + 90;
        }
        private void SetDimensions(Article article)
        {
            dimX = article.DimensionX;
            dimY = article.DimensionY;
            dimZ = article.DimensionZ;
        }
        private void SetFilerWithCoinPositionsAndDimensions(Article article)
        {
            if (article.HasParent() && article.Parent.IsValid)
            {
                posX = article.Parent.PositionX;
                posY = article.Parent.PositionY;
                posZ = article.Parent.PositionZ;
                a = article.Parent.AngleOXY + 90;

                dimX = article.Parent.DimensionX + article.DimensionX;
                dimY = article.Parent.DimensionY + article.DimensionY;
                dimZ = article.DimensionZ;
            }
        }

        private string Indexation(string segmentName, int index)
        {
            return KD.StringTools.Format.Bracketed(segmentName + index.ToString(SegmentFormat.FourZero)) + Separator.NewLine;
        }
        private string PositionX(double value)
        {
            string data = ItemKey.RefPntX + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string PositionY(double value)
        {
            string data = ItemKey.RefPntY + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string PositionZ(double value)
        {
            string data = ItemKey.RefPntZ + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionX(double value)
        {
            string data = ItemKey.Width + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionY(double value)
        {
            string data = ItemKey.Depth + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionZ(double value)
        {
            string data = ItemKey.Height + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }     
        private string Angle(double value)
        {
            string data = ItemKey.AngleZ + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return Tools.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string Opening(Article article)
        {
            string opening = String.Empty;
            if (article.Host != null)
            {
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(article.Host.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    if (article.Description.ToUpper().Contains(this.CurrentAppli.GetTranslatedText(CatalogConstante.Slide.ToUpper())))
                    {
                        opening = ItemValue.Slide;
                    }
                    else if (article.Description.ToUpper().Contains(this.CurrentAppli.GetTranslatedText(CatalogConstante.Leaf.ToUpper())))
                    {
                        this.CurrentAppli.Scene.SceneSetReferenceFromObject(article.Host.ObjectId, false);
                        if ((article.AngleOXY == article.Host.AngleOXY))
                        {
                            opening = ItemValue.InWards;
                        }
                        else
                        {
                            opening = ItemValue.OutWard;
                        }
                    }
                    else
                    {
                        opening = ItemValue.None;
                    }
                }

            }
            return ItemKey.Opening + KD.StringTools.Const.EqualSign + opening + Separator.NewLine;
        }
        private string WallRefNo(Article article)
        {
            string refNo = String.Empty;
            if (article.Host != null)
            {
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(article.Host.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    refNo = article.Host.Number.ToString();
                }
            }
            return ItemKey.WallRefNo + KD.StringTools.Const.EqualSign + refNo + Separator.NewLine;
        }
        private string RefPntXRel(Article article)
        {
            string refPntXRel = String.Empty;
            if (article.Host != null)
            {
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(article.Host.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    double unit = KD.StringTools.Convert.ToDouble(this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.UNITVALUE));
                    double value = article.GetObjectInfo(KD.SDK.SceneEnum.ObjectInfo.DISTLEFTWALL, unit);
                    refPntXRel = value.ToString();
                }
            }
            return ItemKey.RefPntXRel + KD.StringTools.Const.EqualSign + refPntXRel + Separator.NewLine;
        }
        private string RefPntYRel(Article article)
        {
            return ItemKey.RefPntYRel + KD.StringTools.Const.EqualSign + "0" + Separator.NewLine;
        }
        private string RefPntZRel(Article article)
        {
            return ItemKey.RefPntZRel + KD.StringTools.Const.EqualSign + "0" + Separator.NewLine;
        }
        #endregion
    }
}

