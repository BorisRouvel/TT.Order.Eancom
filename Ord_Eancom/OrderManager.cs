using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.IO.Compression;

using KD.SDKComponent;
using KD.CatalogProperties;
using KD.Analysis;
using KD.Model;

using KD.Plugin.Word;

using Eancom;


namespace Ord_Eancom
{
    public class OrderEnum
    {
        public enum Type
        {
            EDIGRAPH = 1,
            FLOOR_PLAN = 11,
            WALL_FRONT_VIEW = 21,
            TILE_PLAN = 22,
            INSTALLATION_PLAN = 23,
            PERSPECTIVE = 31,
            PLINTH_SKETCH = 41,
            WORKTOP_SKETCH = 42,
            WALL_SEALING_PROFILE_SKETCH = 43,
            LIGHT_PELMET_SKETCH = 44,
            CORNICE_SKETCH = 45,
            FLOORING_SKETCH = 46,
            OTHER = 99
        }        

        public enum Format
        {
            EDIGRAPH = 1,
            JPEG = 11,
            PDF = 50,
            Others = 99
        }
    }

    public class OrderConstants
    {
        public const string Insitu = "INSITU";
        public const string HandleName = "_HDL";

        public const string FormatDate_yMd = "yyyyMMdd";
        public const string FormatDate_dMy = "ddMMyy";
        public const string FormatDate_d_M_y = "dd/MM/yyyy";
        public const string FormatDate_yW = "yyyyww";
        public const string FormatDate_y = "yyyy";
        public const string FormatTime_Hm = "HHmm";
        public const string FormatTime_H_m_s = "HH:mm:ss";

        public const int CommentSceneLinesMax = 99;
        public const int CommentSceneCharactersPerLineMax = 70;

        public const string PlinthFinishType = "7";
        public const string LeftAssemblyFinishType = "19";
        public const string RightAssemblyFinishType = "20";

        public const int ArticleSupplierId_InFile_Position = 0;
        public const int ArticleSerieNo_InFile_Position = 1;
        public const int ArticleEDPNumber_InFile_Position = 2;
        public const int ArticleEANNumber_InFile_Position = 3;
        public const int ArticleHinge_InFile_Position = 4;
        public const int ArticleConstructionId_InFile_Position = 5;
    }

    public class OrderTransmission
    {      
        public const string OrderEDIFileName = "Order.edi";
        public const string OrderEGIFileName = "Order.egi";
        public const string OrderName = "Commande";
        public const string OrderZipFileName = "Order.zip";

        public const string PlanName = "Plan";
        public const string ElevName = "Elevation";

        public const string ExtensionEDI = ".edi";
        public const string ExtensionEGI = ".egi";
        public const string ExtensionZIP = ".zip";
        public const string ExtensionTXT = KD.IO.File.Extension.Txt;
        public const string ExtensionJPG = KD.IO.File.Extension.Jpg;
        public const string ExtensionPDF = KD.IO.File.Extension.Pdf;

        //public const string VersionOrderDataFormat = "EANCOM_ORDER_V2.03"; //i must get the string in IDM
        public const string VersionEdigraph = "EDIGRAPH_V1.50";
        
    }

    public class OrderInformations
    {
        private AppliComponent _currentAppli;
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

        private int _callParamsBlock;
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

        private Articles _articles;
        public Articles Articles
        {
            get
            {
                return _articles;
            }
        }

        private Article _article;
        public Article Article
        {
            get
            {
                return _article;
            }
        }

        private SceneAnalysis sceneAnalysis = null;

        public static string deliveryDate = String.Empty;
        public static string installationDate = String.Empty;
        private CultureInfo provider = CultureInfo.InvariantCulture;

        private string supplierID = String.Empty;
        
        public OrderInformations(AppliComponent appli, int callParamsBlock)
        {
            this._currentAppli = appli;
            this._callParamsBlock = callParamsBlock;
        }
        public OrderInformations(AppliComponent appli, int callParamsBlock, Articles articles)
        {
            this._currentAppli = appli;
            this._callParamsBlock = callParamsBlock;
            this._articles = articles;
            sceneAnalysis = new SceneAnalysis(this.GetArticleWithModel());           
        }
        public OrderInformations(Article article)
        {          
            this._article = article;
        }

        public string ReleaseChar(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                List<string> releaseList = new List<string>() { "?", ":", "+", "'" };
                foreach (string releaseChar in releaseList)
                {
                    if (text.Contains(releaseChar))
                    {
                        text = text.Replace(releaseChar, "?" + releaseChar);
                    }
                }
                return text;
            }
            return String.Empty;
        }

        public string GetCatalogCode(int index)
        {
            string catalogCode = this.CurrentAppli.CatalogGetCode(this.Articles[index].CatalogFileName);
            return catalogCode;
        }
        public string GetCatalogFileName(int index)
        {
            string catalogFileName = this.Articles[index].CatalogFileName;
            return catalogFileName;
        }
        public string GetPairingCatalogFileName(string csvFileName)
        {
            for (int indexCat = 0; indexCat < this.Articles.Count; indexCat++)
            {
                string catalogFileName = this.GetCatalogFileName(indexCat);
                Reference reference = new Reference(this.CurrentAppli, catalogFileName);
                for (int indexRes = 0; indexRes < reference.RessourcesLinesNb; indexRes++)
                {
                    if (reference.Resource_Name(indexRes).Contains(csvFileName))
                    {
                        return catalogFileName;
                    }
                }
            }
            return String.Empty;
        }
        public Article GetArticleWithModel()
        {
            foreach (Article article in this.Articles)
            {
                if (article.HeadingRank <= 19)
                {
                    return article;
                }                    
            }
            return null;
        }
        public string GetSupplierName()
        {
            supplierID = this.CurrentAppli.GetCallParamsInfoDirect(CallParamsBlock, KD.SDK.AppliEnum.CallParamId.SUPPLIERID);
            return (supplierID);
        }
        public string GetReferenceNumberEDIFile(FileEDI fileEDI) //UNB 0020
        {           
            return fileEDI.GetNextOrdersNumberHex();
        }
        public string GetReferenceNumberEDIMessage() //UNH 0062
        {
            //return (this.CurrentAppli.GetCallParamsInfoDirect(CallParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERFILENAME));
            return (this.CurrentAppli.SceneName);
        }
        public string GetNameAndVersionSoftware()
        {
            return (OrderConstants.Insitu + KD.StringTools.Const.WhiteSpace + this.CurrentAppli.GetVersion());
        }
        public string GetOrderNumber()
        {
            return (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.DevisNuméro()"));
        }
        public string GetDeliveryDateType()
        {
            string exactDayDelivery = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonJour()");
            string latestDayDelivery = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonFin()");
            string earliestDayDelivery = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonDébut()");
            string weekDelivery = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonSemaine()");

            if (!String.IsNullOrEmpty(exactDayDelivery))
            {               
                deliveryDate = ConvertStringToDate(exactDayDelivery);
                return Eancom.DTM.C507.E2005_2;
            }
            else if (!String.IsNullOrEmpty(latestDayDelivery))
            {
                deliveryDate = ConvertStringToDate(latestDayDelivery);
                return Eancom.DTM.C507.E2005_63;
            }
            else if (!String.IsNullOrEmpty(earliestDayDelivery))
            {
                deliveryDate = ConvertStringToDate(earliestDayDelivery);
                return Eancom.DTM.C507.E2005_64;
            }
            else if (!String.IsNullOrEmpty(weekDelivery))
            {
                deliveryDate = ConvertStringToDate(weekDelivery);
                return Eancom.DTM.C507.E2005_2;
            }

            return String.Empty;
        }
        public string GetInstallationDateType()
        {
            string dayInstallation = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.InstallationJour()");
            string weekInstallation = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.InstallationSemaine()");

            if (!String.IsNullOrEmpty(dayInstallation))
            {                
                installationDate = ConvertStringToDate(dayInstallation);               
                return Eancom.DTM.C507.E2005_18;
            }
            else if (!String.IsNullOrEmpty(weekInstallation))
            {
                installationDate = ConvertStringToDate(weekInstallation);
                return Eancom.DTM.C507.E2005_18;
            }

            return String.Empty;
        }
        public string GetDateFormat(string dateString)
        {
            if (String.IsNullOrEmpty(dateString))
            {
                return String.Empty;
            }

            try
            {
                DateTime result = DateTime.ParseExact(dateString, OrderConstants.FormatDate_yMd, provider);               
                return Eancom.DTM.C507.E2379_102;
            }
            catch (FormatException)
            {                
                try
                {
                    DateTime result = DateTime.ParseExact(dateString.Substring(0, 4), OrderConstants.FormatDate_y, provider);
                    int.TryParse(dateString.Substring(dateString.Length - 2, 2), out int resultInt);
                    if (IsValidWeek(resultInt))
                    {
                        return Eancom.DTM.C507.E2379_616;
                    }
                }
                catch (FormatException)
                {
                    return String.Empty;
                }                
            }
            return String.Empty;
        }
        public string GetCommentScene()
        {
           string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Scène.Commentaire()"));
            return this.ReleaseChar(text);

        }
        public string GetCommissionNumber()
        {
            return (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.DevisNuméro()"));
        }
        public string GetCommissionName()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.ClientNom()"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierGLN()
        {
            return (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Code(" + supplierID + ")"));
        }
        public string GetSupplierName1()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Société(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierName2()
        {
            return String.Empty; // (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Société(" + supplierID + ")"));
        }
        public string GetSupplierAddress()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Adresse1(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Adresse2(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Adresse3(" + supplierID + ")"));
            return this.ReleaseChar(text);

        }
        public string GetSupplierCity()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Ville(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierPostCode()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.CodePostal(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierCountry()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Pays(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierPhone()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Téléphone(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierFax()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Fax(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetSupplierEmail()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.Email(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerID()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteIdentifiant(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerGLN()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteCode(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerNumber()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.CodeClient(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerName1()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteSociété(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerName2()
        {
            string text = String.Empty;
            return this.ReleaseChar(text);
        }
        public string GetRetailerAddress()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteAdresse1(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteAdresse2(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteAdresse3(" + supplierID + ")"));
            return this.ReleaseChar(text);

        }
        public string GetRetailerCity()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteVille(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerPostCode()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteCodePostal(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerCountry()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SitePays(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerPhone()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteTéléphone()"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerFax()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteFax()")));
        }
        public string GetRetailerEmail()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteEmail()")));
        }
        public string GetDeliveryName1()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonNom(" + supplierID + ")")));
        }
        public string GetDeliveryAddress()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonAdresse1(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonAdresse2(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonAdresse3(" + supplierID + ")")));

        }
        public string GetDeliveryCity()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonVille(" + supplierID + ")")));
        }
        public string GetDeliveryPostCode()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonCodePostal(" + supplierID + ")")));
        }
        public string GetDeliveryCountry()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonPays(" + supplierID + ")")));
        }
        public string GetCustomerDeliveryName1()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonNom(" + supplierID + ")")));
        }
        public string GetCustomerDeliveryAddress()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonAdresse1(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonAdresse2(" + supplierID + ")") + KD.StringTools.Format.Spaced(KD.StringTools.Const.MinusSign) +
                    this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonAdresse3(" + supplierID + ")")));

        }
        public string GetCustomerDeliveryCity()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonVille(" + supplierID + ")")));
        }
        public string GetCustomerDeliveryPostCode()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonCodePostal(" + supplierID + ")")));
        }
        public string GetCustomerDeliveryCountry()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonPays(" + supplierID + ")")));
        }
        public string GetSellerID()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.VendeurId()")));
        }
        public string GetSellerInformations()
        {
            return this.ReleaseChar((this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.VendeurNom()") + KD.StringTools.Const.SemiColon + this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.VendeurPrénom()")));
        }        
        public string GetCatalogModelCodeAndName()
        {
            string[] generikFinishTypes;
            string[] generikFinishes;           

            bool IsGenerik = sceneAnalysis.GetGenericFinishes(out generikFinishTypes, out generikFinishes);
            if (IsGenerik)
            {
                int.TryParse(generikFinishTypes[0], out int generikFinishType);
                int.TryParse(generikFinishes[0], out int generikFinish);
                return sceneAnalysis.GetCatalogFinishCodeAndName(generikFinishType, generikFinish);
            }
            return null;
        }
        public List<string> GetGenericCatalogFinishCodeAndName()
        {            
            List<string> finishesList = new List<string>();
            string[] generikFinishTypes;
            string[] generikFinishes;
            sceneAnalysis = new SceneAnalysis(this.GetArticleWithModel());

            bool IsGenerik = sceneAnalysis.GetGenericFinishes(out generikFinishTypes, out generikFinishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < generikFinishTypes.Length; fin++) 
                {
                    int.TryParse(generikFinishTypes[fin], out int generikFinishType);
                    int.TryParse(generikFinishes[fin], out int generikFinish);                   
                    int type = sceneAnalysis.GetFinishTypeNumber(generikFinishType);
                    finishesList.Add(sceneAnalysis.GetCatalogFinishCodeAndName(generikFinishType, generikFinish) + 
                        KD.StringTools.Const.SemiColon + type);
                }
                return finishesList;
            }
            return null;
        }
        public List<string> GetCatalogFinishCodeAndName()//Article article)
        {
            List<string> finishesList = new List<string>();
            string[] finishTypes;
            string[] finishes;
            sceneAnalysis = new SceneAnalysis(this.Article);
            
            bool IsGenerik = sceneAnalysis.GetFinishes(out finishTypes, out finishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < finishTypes.Length; fin++)
                {
                    int.TryParse(finishTypes[fin], out int finishType);
                    int.TryParse(finishes[fin], out int finish);
                    int type = sceneAnalysis.GetFinishTypeNumber(finishType);
                    finishesList.Add(sceneAnalysis.GetCatalogFinishCodeAndName(finishType, finish) +
                        KD.StringTools.Const.SemiColon + type);
                }
                return finishesList;
            }
            return null;
        }
        public List<string> GetFinishCodeAndName()//Article article)
        {
            List<string> finishesList = new List<string>();
            string[] finishTypes;
            string[] finishes;
            sceneAnalysis = new SceneAnalysis(this.Article);

            bool IsGenerik = sceneAnalysis.GetFinishes(out finishTypes, out finishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < finishTypes.Length; fin++)
                {
                    int.TryParse(finishTypes[fin], out int finishType);
                    int.TryParse(finishes[fin], out int finish);
                    int type = sceneAnalysis.GetFinishTypeNumber(finishType);
                    finishesList.Add(sceneAnalysis.GetFinishCodeAndName(finishType, finish) +
                        KD.StringTools.Const.SemiColon + type);
                }
                return finishesList;
            }
            return null;
        }
        public string GetSupplierCommentArticle()//Article article)
        {
            return this.ReleaseChar((this.Article.ObjectGetKeywordInfo("@Objet.CommentaireFournisseur()")));
        }
        public int GetComponentLevel()//Article article)
        {
            return this.Article.GetInteger32Info(KD.SDK.SceneEnum.ObjectInfo.COMPONENTLEVEL) + 1;
        }
        public int GetPriceType()//Article article)
        {
            return this.Article.GetInteger32Info(KD.SDK.SceneEnum.ObjectInfo.PRICETYPE);
        }

        private bool IsPlinthFinishType(int generikFinishType)
        {            
            int type = sceneAnalysis.GetFinishTypeNumber(generikFinishType);
            if (type.ToString() == OrderConstants.PlinthFinishType)
            {
                return true;
            }
            return false;
        }
        private string ConvertStringToDate(string dateString)
        {
            if (dateString.Contains(KD.StringTools.Const.MinusSign))
            {            
                return dateString.Replace(KD.StringTools.Const.MinusSign, String.Empty);
            }
            else if (dateString.Contains(KD.StringTools.Const.Slatch))
            {
                dateString = dateString.Replace(KD.StringTools.Const.Slatch, String.Empty);
                DateTime dateTime = DateTime.ParseExact(dateString, OrderConstants.FormatDate_dMy, provider);

                return dateTime.ToString(OrderConstants.FormatDate_yMd);
            }
            return dateString;
        }
        private bool IsValidWeek(int week)
        {
            if (week == 0) { return false; }
            return (1 <= week && week <= 52);
        }
        public bool IsRegroupPortion(int priceType)
        {
            if (priceType == 29)
            {
                return true;
            }
            return false;
        }
        public bool IsParent(int level)
        {
            if (level == 1)
            {
                return true;
            }
            return false;
        }
        public bool IsWorktop()
        {
            if (this.Article.Type == 5 && this.Article.Layer == 5) 
            {
                return true;
            }
            return false;
        }
        public bool IsPlinth()
        {
            if (this.Article.Type == 6 && this.Article.Layer == 2)
            {
                return true;
            }
            return false;
        }
        public bool IsLightpelmet()
        {
            if (this.Article.Type == 6 && this.Article.Layer == 8)
            {
                return true;
            }
            return false;
        }
        public bool IsCornice()
        {
            if (this.Article.Type == 6 && this.Article.Layer == 11)
            {
                return true;
            }
            return false;
        }
        public bool IsShape()
        {
            if (this.Article.Type == 5)
            {
                return true;
            }
            return false;
        }
        public bool IsLinear()
        {
            if (this.Article.Type == 6)
            {
                return true;
            }
            return false;
        }
        public bool IsOption_MEA()
        {
            if (this.IsWorktop() || this.IsPlinth() || this.IsLightpelmet() || this.IsCornice() || this.IsShape() || this.IsLinear())
            {
                return true;
            }
            return false;
        }

        public List<string> GetAssemblyWorktopCodeAndNameList()
        {
            if (IsWorktop())
            {
                List<string> codeList = new List<string>();
                List<string> codeAndNameList = new List<string>();

                codeAndNameList = this.GetFinishCodeAndName();// article);
                if (codeAndNameList != null && codeAndNameList.Count > 0)
                {
                    foreach (string codeAndNameLine in codeAndNameList)
                    {
                        string[] codeAndName = codeAndNameLine.Split(KD.CharTools.Const.SemiColon);
                        if (codeAndName.Length == 4)
                        {
                            Eancom.Utility utility = new Eancom.Utility();
                            if (utility.IsAssemblyWorktop(codeAndName[3]))
                            {
                                string code = utility.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                                string name = utility.DelCharAndAllAfter(codeAndName[2], KD.StringTools.Const.Underscore);
                                codeList.Add(code + KD.StringTools.Const.SemiColon + name);
                            }
                        }
                    }
                    return codeList;
                }
            }
            return null;
        }

        
    }


    public class OrderWrite
    {
        Encoding uniEncoding = Encoding.UTF8;
        static List<string> structureLineEDIList = new List<string>();
        static List<string> structureLineEGIList = new List<string>();
        readonly OrderInformations _orderInformations = null;
        readonly OrderInformations _orderInformationsFromArticles = null;
        readonly FileEDI _fileEDI = null;
        readonly Eancom.Utility _utility = null;

        private int consecutiveNumbering = 1;
        public static int segmentNumberBetweenUNHandUNT = 0;

        private AppliComponent _currentAppli;
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

        private int _callParamsBlock;
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

        double sceneDimX = 0.0;
        double sceneDimY = 0.0;
        string strSceneDimZ = String.Empty;
        double sceneDimZ = 0.0;
        double angleScene = 0.0;

        private string x = String.Empty;
        private string y = String.Empty;
        private string z = String.Empty;
        private string ab = String.Empty;       
        
        private double x1 = 0.0;
        private double y1 = 0.0;
        private double z1 = 0.0;
        private double a = 0.0;

        string version = String.Empty;


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

        public OrderWrite(AppliComponent appli, OrderInformations orderInformations, OrderInformations orderInformationsFromArticles, Articles articles, FileEDI fileEDI)
        {
            this._currentAppli = appli;
            this._orderInformations = orderInformations;
            this._orderInformationsFromArticles = orderInformationsFromArticles;
            this._fileEDI = fileEDI;
            this._utility = new Eancom.Utility();

            this.InitializeEancomStructure();

            version = OrderTransmission.VersionEdigraph.Split(KD.CharTools.Const.Underscore)[1];
        }

        //   EDI      
        private void InitializeEancomStructure()//OrderInformations orderInformations)
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
            mEA = new Eancom.MEA();
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
                structureLineEDIList.Add(text);
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
        private void WriteLineInFileEDI(FileStream fs, string text)
        {
            fs.Write(uniEncoding.GetBytes(text), 0, uniEncoding.GetByteCount(text));
        }

        public void BuildEDI(Articles articles)
        {
           this.HeaderEDI();
           this.HeaderData();
           this.LineData(articles);
           this.EndEDI(); //here set segement number between UNH and UNT include
        }

        public void HeaderEDI()
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

            SetLineEDIList(nAD.Add_BY());

            SetLineEDIList(cTA.Add_Seller_OC());

            SetLineEDIList(cOM.Add_Retailer_TE());
            SetLineEDIList(cOM.Add_Retailer_FX());
            SetLineEDIList(cOM.Add_Retailer_EM());

            SetLineEDIList(nAD.Add_Delivery_DP());
            SetLineEDIList(nAD.Add_CustomerDelivery_DP());
        }
        public void HeaderData()
        {
            SetLineEDIList(lIN_H.Add());

            SetLineEDIList(pIA_H.Add_ManufacturerID());
            SetLineEDIList(pIA_H.Add_SerieNo());
            SetLineEDIList(pIA_H.Add_CatalogID());
            SetLineEDIList(pIA_H.Add_ModelCodeAndName());
            SetLineEDIList(pIA_H.Add_FinishCodeAndName());
            SetLineEDIList(pIA_H.Add_PlinthHeight());
        }
        public void LineData(Articles articles)
        {
            foreach (Article article in articles)
            {
                if (!article.KeyRef.StartsWith(KD.StringTools.Const.Underscore))
                {
                    if (!article.KeyRef.EndsWith(OrderConstants.HandleName))
                    {
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

                        SetLineEDIList(iMD.Add(article));
                        SetLineEDIList(mEA.Add(article));
                        SetLineEDIList(qTY.Add(article));
                        SetLineEDIList(fTX_A.Add(article));
                        SetLineEDIList(rFF_A.Add_ReferenceNumber(article));
                        SetLineEDIList(rFF_A.Add_PlanningSystemItemNumber(article));

                        OrderInformations articleInformations = new OrderInformations(article);
                        if (articleInformations.IsWorktop())
                        {
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
                                        SetLineEDIList(rFF_A.Add_WorktopAssemblyReferenceNumber(article)); //we must find a solution for unique ref cause it s a finish not a artciel objectId
                                        SetLineEDIList(rFF_A.Add_WorktopAssemblyPlanningSystemItemNumber(article, assemblyCode));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void EndEDI()
        {
            SetLineEDIList(uNS.Add());
            SetLineEDIList(uNT.Add());
            SetLineEDIList(uNZ.Add());
        }

        public void EDIOrderFile()
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

        // EGI
        private void ClearStructureEGIList()
        {
            structureLineEGIList.Clear();
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
            fs.Write(uniEncoding.GetBytes(text), 0, uniEncoding.GetByteCount(text));
        }

        public void BuildEGI(Articles articles)
        {
            this.HeaderEGI();
            this.SetWallInformations(this.GetWallsList());
            this.SetArticleInformations(articles);

            this.CurrentAppli.SceneComponent.ResetSceneReference();
        }

        private string ConvertCommaToDot(string value)
        {
            value = value.Replace(KD.StringTools.Const.Comma, KD.StringTools.Const.Dot);
            return value;
        }
        public List<Wall> GetWallsList()
        {
            //Get wall number           
            List<Wall> wallList = new List<Wall>(0);
            int objectNumber = this.CurrentAppli.Scene.SceneGetObjectsNb();
            for (int objectRank = 0; objectRank < objectNumber; objectRank++)
            {
                int objectID = this.CurrentAppli.Scene.SceneGetObjectId(objectRank);
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(objectID, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    Wall wall = new Wall(this.CurrentAppli, objectID);
                    wallList.Add(wall);
                }
            }
            return wallList;
        }

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
            //soumettre au detecteur de char ??
            structureLineEGIList.Add("[Global]" + Separator.NewLine);
            structureLineEGIList.Add("Version=" + OrderTransmission.VersionEdigraph + Separator.NewLine);
            structureLineEGIList.Add(_orderInformations.ReleaseChar("Name=" + _orderInformations.GetCommissionNumber()) + Separator.NewLine);
            structureLineEGIList.Add(_orderInformations.ReleaseChar("Number=" + _orderInformations.GetOrderNumber()) + Separator.NewLine);
            structureLineEGIList.Add(_orderInformations.ReleaseChar("DrawDate=" + dateTime.ToString(OrderConstants.FormatDate_d_M_y)) + Separator.NewLine);
            structureLineEGIList.Add(_orderInformations.ReleaseChar("DrawTime=" + dateTime.ToString(OrderConstants.FormatTime_H_m_s)) + Separator.NewLine);

            string roomHeight = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
            string room = Convert.ToInt32(roomHeight).ToString("0.00");           
            structureLineEGIList.Add("RoomHeight=" + this.ConvertCommaToDot(room) + Separator.NewLine);
            structureLineEGIList.Add("Manufacturer=" + _fileEDI.ManufacturerID() + Separator.NewLine);
            structureLineEGIList.Add("System=" + _orderInformations.GetNameAndVersionSoftware() + Separator.NewLine);
        }
        private void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case "V1.50": //DISCAC
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case "V1.51": //FBD
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (270 * System.Math.PI) / 180;
                    break;
                default:
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (270 * System.Math.PI) / 180;
                    break;
            }

        }
        public void SetWallInformations(List<Wall> wallList)
        {            
            this.SetSceneReference(version);
            
            int index = 1;
            foreach (Wall wall in wallList)
            {
                this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);

                structureLineEGIList.Add(this.WallIndexation(index));
                structureLineEGIList.Add(WallPositionX(wall.PositionX));
                structureLineEGIList.Add(WallPositionY(wall.PositionY));
                structureLineEGIList.Add(WallPositionZ(wall.PositionZ));
                structureLineEGIList.Add(WallDimensionX(wall.DimensionX));
                structureLineEGIList.Add(WallDimensionZ(wall.DimensionZ));
                structureLineEGIList.Add(WallDimensionY(wall.DimensionY));
                structureLineEGIList.Add(WallAngle(wall.AngleOXY));
                index += 1;
            }
        }
        private string WallIndexation(int wallIndex)
        {
            return "[Wall_" + wallIndex.ToString("0000") + "]" + Separator.NewLine;
        }
        private string WallPositionX(double value)
        {           
            string data = "RefPntX=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallPositionY(double value)
        {            
            string data = "RefPntY=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallPositionZ(double value)
        {
            string data = "RefPntZ=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallDimensionX(double value)
        {
            string data = "Width=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallDimensionY(double value)
        {
            string data = "Depth=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallDimensionZ(double value)
        {
            string data = "Height=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string WallAngle(double angle)
        {
            string data = "AngleZ=" + angle.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }

        private void MoveArticlePerRepere(Article article)
        {
            x1 = article.PositionX;
            y1 = article.PositionY;
            z1 = article.PositionZ;
            a = article.AngleOXY - 180;

            switch (version.ToUpper())
            {
                case "V1.50": //DISCAC                    
                    double angle = 2 * Math.PI * (a / 360);
                    x1 -= article.DimensionX * Math.Cos(angle);
                    y1 -= article.DimensionX * Math.Sin(angle);
                    break;
                case "V1.51": //FBD
                    //x1 -= article.DimensionX;
                   // a -= article.AngleOXY;
                    break;
                default:
                   // x1 -= article.DimensionX;
                    //a -= article.AngleOXY;
                    break;
            }
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

            this.SetSceneReference(version);

            int index = 1;

            foreach (Article article in articles)
            {
                if (!String.IsNullOrEmpty(article.Number.ToString()) && article.Number != KD.Const.UnknownId)               
                {
                    this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);

                    structureLineEGIList.Add(this.ArticleIndexation(index));
                    structureLineEGIList.Add(_orderInformations.ReleaseChar(ArticleManufacturer(article.KeyRef)));
                    structureLineEGIList.Add(_orderInformations.ReleaseChar(ArticleReference(article.KeyRef)));
                    structureLineEGIList.Add(_orderInformations.ReleaseChar(ArticleKeyReference(article.KeyRef)));
                    structureLineEGIList.Add(ArticleRefNo(article.ObjectId.ToString())); //RFF.LI  
                    structureLineEGIList.Add(ArticleRefPos(article.ObjectId.ToString())); // RFF.ON

                    this.MoveArticlePerRepere(article);

                    structureLineEGIList.Add(ArticlePositionX(x1));
                    structureLineEGIList.Add(ArticlePositionY(y1));

                    double positionZ = this.GetRealPositionZByPosedOnOrUnder(article);
                    structureLineEGIList.Add(ArticlePositionZ(positionZ));

                    structureLineEGIList.Add(ArticleAngle(a));
                    structureLineEGIList.Add(ArticleShape("1"));

                    double dimX = article.DimensionX;
                    double dimY = article.DimensionY;

                    Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
                    if (childs != null)
                    {                        
                        foreach (Article child in childs)
                        {
                            if (child.Name.StartsWith ("@F"))
                            {
                                double angleFilerDepth = child.DimensionY;
                                structureLineEGIList.Add(ArticleAngleFilerDimensionX(child.DimensionY));
                                structureLineEGIList.Add(ArticleAngleDimensionY(article.DimensionY));
                                dimY = child.DimensionX + article.DimensionY;
                                break;
                            }
                        }
                    }
                    
                    structureLineEGIList.Add(ArticleDimensionX(dimX));
                    structureLineEGIList.Add(ArticleDimensionZ(article.DimensionZ));
                    structureLineEGIList.Add(ArticleDimensionY(dimY));
                   
                    structureLineEGIList.Add(ArticleConstructionType(article.KeyRef));
                    structureLineEGIList.Add(ArticleHinge(article));

                    int polyType = this.GetArticlePolyType(article);
                    if (polyType != KD.Const.UnknownId)
                    {
                        structureLineEGIList.Add(ArticlePolyType(polyType));
                        int polyCounter = this.GetArticlePolyCounter(article);
                        structureLineEGIList.Add(ArticlePolyCounter(polyCounter));

                        string[] polyPoints = this.GetArticlePolyPoint(article, polyType);
                        if (polyPoints.Length > 1)
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

        private string ArticleIndexation(int articleIndex)
        {
            return "[Article_" + articleIndex.ToString("0000") + "]" + Separator.NewLine;
        }
        private string ArticleManufacturer(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
                string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
                if (articleInfos != null)
                {
                    string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                    return "Manufacturer=" + articleInfo[OrderConstants.ArticleSupplierId_InFile_Position] + Separator.NewLine;
                }
            }
            return null;
        }
        private string ArticleReference(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                return "Name=" + this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore) + Separator.NewLine; ;
            }
            return null;
        }
        private string ArticleKeyReference(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                return "#Name=" + keyRef + Separator.NewLine; ;
            }
            return null;
        }
        private string ArticleRefNo(string iD)
        {
            if (!String.IsNullOrEmpty(iD))
            {
                return "RefNo=" + iD + Separator.NewLine; ;
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
                        return "RefPos=" + refPos + Separator.NewLine;
                    }
                }
            }
            return null;
        }
        private string ArticlePositionX(double value)
        {
            string data = "RefPntX=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticlePositionY(double value)
        {
            string data = "RefPntY=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticlePositionZ(double value)
        {
            string data = "RefPntZ=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngle(double value)
        {
            string data = "AngleZ=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleShape(string str)
        {            
            return "Shape=" + str.ToString() + Separator.NewLine;
        }
        private string ArticleDimensionX(double value)
        {
            string data = "Measure_B=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleFilerDimensionX(double value)
        {
            string data = "Measure_B1=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionZ(double value)
        {
            string data = "Measure_H=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionY(double value)
        {
            string data = "Measure_T=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionY(double value)
        {
            string data = "Measure_T1=" + value.ToString("0.00");
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleConstructionType(string keyRef)
        {
            keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                return "ConstructionType=" + articleInfo[OrderConstants.ArticleConstructionId_InFile_Position] + Separator.NewLine;
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
                    hinge = "L";
                    break;
                case "2":
                    hinge = "R";
                    break;
                default:
                    return null;                    
            }
            return "Hinge=" + hinge + Separator.NewLine;           
        }
        private string ArticlePolyType(int polyType)
        {
            return "PolyType=" + polyType.ToString() + Separator.NewLine;
        }
        private string ArticlePolyCounter(int polyCounter)
        {
            return "PolyCounter=" + polyCounter.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointX(int counter, string value)
        {
            return "PolyPntX_" + counter.ToString("0000") + "=" + value.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointY(int counter, string value)
        {
            return "PolyPntY_" + counter.ToString("0000") + "=" + value.ToString() + Separator.NewLine;
        }
        private string ArticlePolyPointZ(int counter, string value)
        {
            return "PolyPntZ_" + counter.ToString("0000") + "=" + value.ToString() + Separator.NewLine;
        }

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

        // Plan
        public void BuildPlan()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = this.GetView();
            this.SetView(KD.SDK.SceneEnum.ViewMode.TOP);
            this.ZoomAdjusted();           
            this.ExportTopImageJPG(1);           
            this.SetView(currentViewMode);
        }
        private KD.SDK.SceneEnum.ViewMode GetView()
        {
            return this.CurrentAppli.Scene.ViewGetMode();
        }
        private bool SetView(KD.SDK.SceneEnum.ViewMode viewMode)
        {
            return this.CurrentAppli.Scene.ViewSetMode(viewMode);
        }
        private bool ZoomAdjusted()
        {
            return this.CurrentAppli.Scene.ZoomAdjusted();
        }
        private bool ExportTopImageJPG(int count)
        {
            return this.CurrentAppli.Scene.FileExportImage(Path.Combine(Order.orderDir, OrderTransmission.PlanName + "-" + count + OrderTransmission.ExtensionJPG), 1200, 1200, "255,255,255", true, 100, 3);
        }
        private bool ExportElevImageJPG(int count)
        {
            return this.CurrentAppli.Scene.FileExportImage(Path.Combine(Order.orderDir, OrderTransmission.ElevName + "-" + count + OrderTransmission.ExtensionJPG), 1200, 1200, "255,255,255", true, 100, 3);
        }

        private bool IsPosedOn(Article article)
        {
            if (article.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.ON_OR_UNDER) == "0")
            {
                return true;
            }
            return false;
        }
        private bool IsPosedUnder(Article article)
        {
            if (article.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.ON_OR_UNDER) == "1")
            {
                return true;
            }
            return false;
        }
        private double GetRealPositionZByPosedOnOrUnder(Article article)
        {
            double positionZ = article.PositionZ;
            if (this.IsPosedUnder(article))
            {
                positionZ = article.PositionZ - article.DimensionZ;
            }
            return positionZ;
        }
        private int GetArticlePolyType(Article article)
        {
            if (article.Topic == 0)
            {
                string type = this.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                
                if (Convert.ToInt16(type) == Convert.ToInt16(KD.SDK.SceneEnum.ObjectType.PLANARTICLE) && article.Layer == 5) //Plinth
                {
                    return 2;
                }
                else if (Convert.ToInt16(type) == Convert.ToInt16(KD.SDK.SceneEnum.ObjectType.STANDARD) && article.Layer == 2) //Worktop
                {
                    return 1;
                }
            }

            return KD.Const.UnknownId;
        }
        private int GetArticlePolyCounter(Article article)
        {
            string shapePointList = this.CurrentAppli.Scene.ObjectGetShape(article.ObjectId);
            string[] shapeList = shapePointList.Split(KD.CharTools.Const.SemiColon);
            if (shapeList.Length > 0)
            {
                return shapeList.Length;
            }
            return 0;
        }
        private string[] GetArticlePolyPoint(Article article, int polyType)
        {
            if (polyType == 1)
            {
                Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
                foreach (Article child in childs)
                {
                    article = child;
                }

            }
            string shapePointList = this.CurrentAppli.Scene.ObjectGetShape(article.ObjectId);
            string[] shapeList = shapePointList.Split(KD.CharTools.Const.SemiColon);
            if (shapeList.Length > 0)
            {
                return shapeList;
            }
            return null;
        }

        // Elevation
        public void BuildElevation()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = this.GetView();

            Articles articles = this.CurrentAppli.GetArticleList(FilterArticle.FilterToGetWallByValid());
            Walls walls = new Walls(articles);
           
            foreach (Wall wall in walls)
            {
                Articles articlesAgainst = wall.AgainstMeASC;

                if (articlesAgainst != null && articlesAgainst.Count > 0)
                {
                    wall.IsActive = true;
                    this.SetView(KD.SDK.SceneEnum.ViewMode.VECTELEVATION);
                    this.ZoomAdjusted();                    
                    this.ExportElevImageJPG(walls.IndexOf(wall) + 1);                   
                }
             
            }
            this.SetView(currentViewMode);
        }

        // Commande (PDF)
        public void BuildOrder()
        {
            int supplierRank = this.CurrentAppli.GetSupplierRankFromIdent(this._orderInformations.GetSupplierName());

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
        private void ManagePdfFile(int supplierRank)
        {
            string pdfFlagState = this.CurrentAppli.SupplierGetInfo(supplierRank, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);

            if (pdfFlagState == KD.StringTools.Const.One)
            {
                //    bool bPdfFlag = this.CurrentAppli.SupplierSetInfo(supplierRank, KD.StringTools.Const.One, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);
                string supplierFilePath = this.GetSupplierFilePath();
                this.CopySupplierFile(supplierFilePath);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Afin de générer le fichier de 'Commande.pdf'" + Environment.NewLine +
                    "Veuillez sélectionner 'Fichier PDF joint' dans votre fournisseurs", "Information");
            }
            //bPdfFlag = this.CurrentAppli.SupplierSetInfo(supplierRank, pdfFlagState, KD.SDK.AppliEnum.SupplierInfo.ATTACHED_PDFFILE);
        }
        private void CopySupplierFile(string supplierFilePath)
        {
            if (File.Exists(supplierFilePath))
            {
                File.Copy(supplierFilePath, Path.Combine(Order.orderDir, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF), true);
            }
        }

        // ZIP
        public void ZIPOrderFile()
        {           
            ZipArchiveEntry readmeEntry = null;
            using (FileStream zipToOpen = new FileStream(Path.Combine(Order.orderDir, OrderTransmission.OrderZipFileName), FileMode.Create, FileAccess.ReadWrite))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    string EDIFile = Path.Combine(Order.orderDir, OrderTransmission.OrderEDIFileName);
                    this.EntryZipAndDeleteFile(readmeEntry, archive, EDIFile, OrderTransmission.OrderEDIFileName);                   

                    if (MainForm.IsChoiceExportEGI)
                    {
                        string EGIFile = Path.Combine(Order.orderDir, OrderTransmission.OrderEGIFileName);
                        this.EntryZipAndDeleteFile(readmeEntry, archive, EGIFile, OrderTransmission.OrderEGIFileName);
                    }
                    if (MainForm.IsChoiceExportPlan)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGplanFile = Path.Combine(Order.orderDir, OrderTransmission.PlanName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGplanFile, OrderTransmission.PlanName + "-" + i + OrderTransmission.ExtensionJPG);
                        }
                    }
                    if (MainForm.IsChoiceExportElevation)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGelevFile = Path.Combine(Order.orderDir, OrderTransmission.ElevName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGelevFile, OrderTransmission.ElevName + "-" + i + OrderTransmission.ExtensionJPG);
                        }
                    }
                    if (MainForm.IsChoiceExportOrder)
                    {
                        string OrderFile = Path.Combine(Order.orderDir, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF);
                        this.EntryZipAndDeleteFile(readmeEntry, archive, OrderFile, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF);
                    }
                }
            }            
        }
        private void EntryZipAndDeleteFile(ZipArchiveEntry readmeEntry, ZipArchive archive, string file, string entryFile)
        {           
            if (File.Exists(file))
            {
                readmeEntry = archive.CreateEntryFromFile(file, entryFile);
                File.Delete(file);
            }
        }
    }

}
