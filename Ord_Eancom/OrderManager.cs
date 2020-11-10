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

using Eancom;
using TT.Import.EGI;


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

    public class OrderDescription
    {
        public const string EDIGRAPH = "EDIGRAPH";
        public const string FLOOR_PLAN = "Plan de base";
        public const string WALL_FRONT_VIEW = "Vue de face";
        public const string TILE_PLAN = "Plan de carrelage";
        public const string INSTALLATION_PLAN = "Plan d'installation";
        public const string PERSPECTIVE = "Perspective";
        public const string PLINTH_SKETCH = "Croquis de socle";
        public const string WORKTOP_SKETCH = "Croquis plan de travail";
        public const string WALL_SEALING_PROFILE_SKETCH = "Croquis du profil d'étanchéité murale";
        public const string LIGHT_PELMET_SKETCH = "Croquis de cache-lumière";
        public const string CORNICE_SKETCH = "Croquis de corniche";
        public const string FLOORING_SKETCH = "Croquis du sol";
        public const string OTHER = "Autre";
    }

    public class OrderKey
    {
        public const string ChoiceRetaillerDelivery = "IsChoiceRetaillerDelivery";
        public const string ChoiceCustomerDelivery = "IsChoiceCustomerDelivery";
        public const string MandatoryDeliveryInformation = "MandatoryDeliveryInformation";
        public const string ChoiceExportEGI = "IsChoiceExportEGI";
        public const string ChoiceExportPlan = "IsChoiceExportPlan";
        public const string ChoiceExportElevation = "IsChoiceExportElevation";
        public const string ChoiceExportPerspective = "IsChoiceExportPerspective";
        public const string ChoiceExportOrder = "IsChoiceExportOrder";
        public const string GenerateOrder = "IsGenerateOrder";
    }

    public class OrderConstants
    {
        public const string Insitu = "INSITU";
        public const string HandleName = "_HDL";

        public const string FormatDate_4yMd = "yyyyMMdd";
        public const string FormatDate_2yMd = "yyMMdd";
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

        public const double FrontDepth = 20.0;

        public const int ArticleSupplierId_InFile_Position = 0;
        public const int ArticleSerieNo_InFile_Position = 1;
        public const int ArticleEDPNumber_InFile_Position = 2;
        public const int ArticleEANNumber_InFile_Position = 3;
        public const int ArticleHinge_InFile_Position = 4;
        public const int ArticleConstructionId_InFile_Position = 5;
        public const int ArticleShape_InFile_Position = 6;
    }

    public class OrderTransmission
    {        
        public const string OrderEDIFileName = "Order.edi";
        public const string OrderEGIFileName = "Order.egi";
        public const string OrderName = "Commande";
        public const string OrderZipFileName = "Order.zip";

        public const string PlanName = "Plan";
        public const string ElevName = "Elevation";
        public const string PerspectiveName = "Perspective";

        public const string ExtensionEDI = ".edi";
        public const string ExtensionEGI = ".egi";
        public const string ExtensionZIP = ".zip";
        public const string ExtensionTXT = KD.IO.File.Extension.Txt;
        public const string ExtensionJPG = KD.IO.File.Extension.Jpg;
        public const string ExtensionPDF = KD.IO.File.Extension.Pdf;

        public const string VersionEancomOrder = "EANCOM_ORDER_V2.03";
        public const string VersionEdigraph_1_50 = "EDIGRAPH_V1.50";
        public const string VersionEdigraph_1_51 = "EDIGRAPH_V1.51";
        public const string HeaderSubject = "EDI-ORDER";

        public const string HeaderTagMandatoryDeliveryInformation = "{}";
    }

    public class OrderInformations
    {
        private AppliComponent _currentAppli;
        private int _callParamsBlock;
        private readonly Articles _articles;
        private readonly Article _article;       

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
        public Articles Articles
        {
            get
            {
                return _articles;
            }
        }      
        public Article Article
        {
            get
            {
                return _article;
            }
        }

        private SceneAnalysis _sceneAnalysis = null;
        private SegmentClassification _segmentClassification = null;

        public static string deliveryDate = String.Empty;
        public static string installationDate = String.Empty;
        private readonly CultureInfo provider = CultureInfo.InvariantCulture;

        private string supplierID = String.Empty;
        private readonly List<string> releaseList = new List<string>() { KD.StringTools.Const.QuestionMark, //"?",
                                                            KD.StringTools.Const.Colon, //":",
                                                            KD.StringTools.Const.PlusSign, //"+",
                                                            KD.StringTools.Const.SimpleQuote, //"'",
                                                            "’" };

        public OrderInformations(AppliComponent appli, int callParamsBlock)
        {
            _currentAppli = appli;
            _callParamsBlock = callParamsBlock;
        }
        public OrderInformations(AppliComponent appli, int callParamsBlock, Articles articles)
        {
            _currentAppli = appli;
            _callParamsBlock = callParamsBlock;
            _articles = articles;
            _sceneAnalysis = new SceneAnalysis(this.GetArticleWithModel());
        }
        public OrderInformations(Article article)
        {
            _article = article;            
        }
        public OrderInformations(Article article, SegmentClassification segmentClassification)
        {
            _article = article;
            _segmentClassification = segmentClassification;
        }

        public string ReleaseChar(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                foreach (string releaseChar in releaseList)
                {
                    if (text.Contains(releaseChar))
                    {
                        text = text.Replace(releaseChar, KD.StringTools.Const.QuestionMark + releaseChar);
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
        public string GetOrderDir()
        {
            return this.CurrentAppli.GetCallParamsInfoDirect(CallParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);
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
                DateTime result = DateTime.ParseExact(dateString, OrderConstants.FormatDate_4yMd, provider);
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
            string SupplierCountryTwoLetterISOName = CountryTable.GetTwoLetterISO(text);

            return this.ReleaseChar(SupplierCountryTwoLetterISOName);
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
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Fournisseur.CodeClient(" + supplierID + ")"));
            return this.ReleaseChar(text);
        }
        public string GetRetailerNumber()
        {
            string text = (this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteCode(" + supplierID + ")"));
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
            string SupplierCountryTwoLetterISOName = CountryTable.GetTwoLetterISO(text);

            return this.ReleaseChar(SupplierCountryTwoLetterISOName);
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
            string text = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.SiteLivraisonPays(" + supplierID + ")");
            string SupplierCountryTwoLetterISOName = CountryTable.GetTwoLetterISO(text);

            return this.ReleaseChar(SupplierCountryTwoLetterISOName);
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
            string text = this.CurrentAppli.Scene.SceneGetKeywordInfo("@Base.LivraisonPays(" + supplierID + ")");
            string SupplierCountryTwoLetterISOName = CountryTable.GetTwoLetterISO(text);

            return this.ReleaseChar(SupplierCountryTwoLetterISOName);
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
            bool IsGenerik = _sceneAnalysis.GetGenericFinishes(out string[] generikFinishTypes, out string[] generikFinishes);
            if (IsGenerik)
            {
                int.TryParse(generikFinishTypes[0], out int generikFinishType);
                int.TryParse(generikFinishes[0], out int generikFinish);
                return _sceneAnalysis.GetCatalogFinishCodeAndName(generikFinishType, generikFinish);
            }
            return null;
        }
        public List<string> GetGenericCatalogFinishCodeAndName()
        {
            List<string> finishesList = new List<string>();
            _sceneAnalysis = new SceneAnalysis(this.GetArticleWithModel());

            bool IsGenerik = _sceneAnalysis.GetGenericFinishes(out string[] generikFinishTypes, out string[] generikFinishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < generikFinishTypes.Length; fin++)
                {
                    int.TryParse(generikFinishTypes[fin], out int generikFinishType);
                    int.TryParse(generikFinishes[fin], out int generikFinish);
                    int type = _sceneAnalysis.GetFinishTypeNumber(generikFinishType);
                    finishesList.Add(_sceneAnalysis.GetCatalogFinishCodeAndName(generikFinishType, generikFinish) +
                        KD.StringTools.Const.SemiColon + type);
                }
                return finishesList;
            }
            return null;
        }
        public List<string> GetCatalogFinishCodeAndName()//Article article)
        {
            List<string> finishesList = new List<string>();
            _sceneAnalysis = new SceneAnalysis(this.Article);

            bool IsGenerik = _sceneAnalysis.GetFinishes(out string[] finishTypes, out string[] finishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < finishTypes.Length; fin++)
                {
                    int.TryParse(finishTypes[fin], out int finishType);
                    int.TryParse(finishes[fin], out int finish);
                    int type = _sceneAnalysis.GetFinishTypeNumber(finishType);
                    finishesList.Add(_sceneAnalysis.GetCatalogFinishCodeAndName(finishType, finish) +
                        KD.StringTools.Const.SemiColon + type);
                }
                return finishesList;
            }
            return null;
        }
        public List<string> GetFinishCodeAndName()//Article article)
        {
            List<string> finishesList = new List<string>();
            _sceneAnalysis = new SceneAnalysis(this.Article);

            bool IsGenerik = _sceneAnalysis.GetFinishes(out string[] finishTypes, out string[] finishes);
            if (IsGenerik)
            {
                for (int fin = 0; fin < finishTypes.Length; fin++)
                {
                    int.TryParse(finishTypes[fin], out int finishType);
                    int.TryParse(finishes[fin], out int finish);
                    int type = _sceneAnalysis.GetFinishTypeNumber(finishType);
                    finishesList.Add(_sceneAnalysis.GetFinishCodeAndName(finishType, finish) +
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
            int type = _sceneAnalysis.GetFinishTypeNumber(generikFinishType);
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

                return dateTime.ToString(OrderConstants.FormatDate_4yMd);
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
        //public bool IsWorktop()
        //{
        //    if (this.Article.Type == 5 && this.Article.Layer == 5)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsPlinth()
        //{
        //    if (this.Article.Type == 6 && this.Article.Layer == 2)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsLightpelmet()
        //{
        //    if (this.Article.Type == 6 && this.Article.Layer == 8)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsCornice()
        //{
        //    if (this.Article.Type == 6 && this.Article.Layer == 11)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsShape()
        //{
        //    if (this.Article.Type == 5)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsLinear()
        //{
        //    if (this.Article.Type == 6)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public bool IsOption_MEA()
        {
            if (_segmentClassification.IsArticleWorkTop() || _segmentClassification.IsArticleShape() || _segmentClassification.IsArticleLinear())
            {
                return true;
            }
            return false;
        }       

        public List<string> GetAssemblyWorktopCodeAndNameList()
        {
            if (_segmentClassification.IsArticleWorkTop())
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
        Encoding encodingOrder = Encoding.Default;
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
        double sceneDimZ = 0.0;
        double angleScene = 0.0;

        private readonly string x = String.Empty;
        private readonly string y = String.Empty;
        private readonly string z = String.Empty;
        private readonly string ab = String.Empty;

        private double posX = 0.0;
        private double posY = 0.0;
        private double posZ = 0.0;
        private double a = 0.0;

        private double dimX = 0.0;
        private double dimY = 0.0;
        private double dimZ = 0.0;

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
            _currentAppli = appli;
            _orderInformations = orderInformations;
            _orderInformationsFromArticles = orderInformationsFromArticles;
            _fileEDI = fileEDI;
            _utility = new Eancom.Utility();

            this.InitializeEancomStructure();

            version = OrderTransmission.VersionEdigraph_1_51.Split(KD.CharTools.Const.Underscore)[1];
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
            if (MainForm.IsChoiceRetaillerDelivery)
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
                    }
                }
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

        #region //Plan for EDI
        public void BuildPlan()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = this.GetView();
            this.SetView(KD.SDK.SceneEnum.ViewMode.TOP);
            this.ZoomAdjusted();
            this.ExportImageJPG(1, OrderTransmission.PlanName);
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
        private bool ExportImageJPG(int count, string exportName)
        {
            return this.CurrentAppli.Scene.FileExportImage(Path.Combine(Order.orderDir, exportName + "-" + count + OrderTransmission.ExtensionJPG), 1200, 1200, "255,255,255", true, 100, 3);
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
            SegmentClassification segmentClassification = new SegmentClassification(article);
           
            int type = Convert.ToInt16(this.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE));

            if (type == (int)(KD.SDK.SceneEnum.ObjectType.PLANARTICLE) && segmentClassification.IsArticleWorkTop()) 
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_WorkTop);
                return PolytypeValue.Polytype_WorkTop;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticlePlinth())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_Base);
                return PolytypeValue.Polytype_Base;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticleLightPelmet())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_LightPelmet);
                return PolytypeValue.Polytype_LightPelmet;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticleCornice())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_Cornice);
                return PolytypeValue.Polytype_Cornice;
            }

            return KD.Const.UnknownId;
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
        public string[] GetArticlePolyPoint(Article article)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);
            string shapePointList = String.Empty;
            string newShapePointList = string.Empty;

            SetSceneReference(version);
            shapePointList = segmentClassification.GetShape();

            if (String.IsNullOrEmpty(shapePointList))
            {
                if (segmentClassification.IsArticlePlinth())
                {                   
                    if (article.HasParent())
                    {
                        SegmentClassification segmentParentClassification = new SegmentClassification(article.Parent);
                        if (segmentParentClassification.IsArticlePlinth())
                        {
                            shapePointList = segmentParentClassification.GetShape();                            
                        }
                    }
                    else
                    {
                        Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
                        if (childs.Count > 0)
                        {
                            foreach (Article child in childs)
                            {
                                SegmentClassification segmentChildClassification = new SegmentClassification(child);
                                if (segmentChildClassification.IsArticlePlinth())
                                {
                                    shapePointList = segmentChildClassification.GetShape();                                   
                                }
                            }
                        }
                    }
                    
                }
            }                
            
            string[] points = shapePointList.Split(KD.CharTools.Const.SemiColon); // 5 pts
            foreach (string point in points)
            {
                string[] coords = point.Split(KD.CharTools.Const.Comma);
                newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[0]) - sceneDimX) + KD.CharTools.Const.Comma;
                newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[1]) - sceneDimY) + KD.CharTools.Const.Comma;
                newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[2]) - sceneDimZ) + KD.CharTools.Const.Comma;
                newShapePointList += coords[3] + KD.CharTools.Const.SemiColon;
            }            

            string[] shapeList = newShapePointList.Split(KD.CharTools.Const.SemiColon);
            if (shapeList.Length > 0)
            {
                //Determine the position for linear because in Insitu is 0,0 and we need the first shape point.
                if (segmentClassification.IsArticleLinear() || segmentClassification.IsArticleWorkTop())
                {
                    posX = KD.StringTools.Convert.ToDouble(shapeList[0].Split(KD.CharTools.Const.Comma)[0]);
                    posY = KD.StringTools.Convert.ToDouble(shapeList[0].Split(KD.CharTools.Const.Comma)[1]);
                }

                return shapeList;
            }
            return null;
        }
        #endregion

        #region //Elevation for EDI
        public void BuildElevation()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = this.GetView();

            Articles articles = this.CurrentAppli.GetArticleList(FilterArticle.FilterToGetWallByValid());
            if (articles != null && articles.Count > 0)
            {
                Walls walls = new Walls(articles);

                foreach (Wall wall in walls)
                {
                    Articles articlesAgainst = wall.AgainstMeASC;

                    if (articlesAgainst != null && articlesAgainst.Count > 0)
                    {
                        wall.IsActive = true;
                        this.SetView(KD.SDK.SceneEnum.ViewMode.VECTELEVATION);
                        this.ZoomAdjusted();
                        this.ExportImageJPG(walls.IndexOf(wall) + 1, OrderTransmission.ElevName);
                    }

                }
                this.SetView(currentViewMode);
            }
        }
        #endregion

        #region //Perspective for EDI
        public void BuildPerspective()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = this.GetView();

            this.SetView(KD.SDK.SceneEnum.ViewMode.OGLREAL);
            this.ZoomAdjusted();
            this.ExportImageJPG(1, OrderTransmission.PerspectiveName);
            this.SetView(currentViewMode);
        }
        #endregion

        #region //Commande (PDF) for EDI
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
                System.Windows.Forms.MessageBox.Show("Afin de générer le fichier de '" + OrderTransmission.OrderName + OrderTransmission.ExtensionPDF + "'" + Environment.NewLine +
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
            else
            {
                System.Windows.Forms.MessageBox.Show("Le fichier de '" + OrderTransmission.OrderName + OrderTransmission.ExtensionPDF + "'" +
                    " n'a pas pu être généré.", "Information");
            }
        }
        #endregion

        #region //ZIP for EDI
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
                    if (MainForm.IsChoiceExportPerspective)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGpersFile = Path.Combine(Order.orderDir, OrderTransmission.PerspectiveName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGpersFile, OrderTransmission.PerspectiveName + "-" + i + OrderTransmission.ExtensionJPG);
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
            //if (File.Exists(file))
            //{
            //    readmeEntry = archive.CreateEntryFromFile(file, entryFile);
            //    File.Delete(file);
            //}
        }
        #endregion


        #region //EGI
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
            fs.Write(encodingOrder.GetBytes(text), 0, encodingOrder.GetByteCount(text));
        }

        public void BuildEGI(Articles articles)
        {
            this.HeaderEGI();
            this.SetWallInformations(this.GetWallsList());
            this.SetDoorInformations(this.GetDoorsList());
            //this.SetWindowInformations();
            //this.SetRecessInformations();
            //this.SetHindranceInformations();
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
        public List<Article> GetDoorsList()
        {                 
            List<Article> doorList = new List<Article>(0);
            int objectNumber = this.CurrentAppli.Scene.SceneGetObjectsNb();
            for (int objectRank = 0; objectRank < objectNumber; objectRank++)
            {
                int objectID = this.CurrentAppli.Scene.SceneGetObjectId(objectRank);
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(objectID, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == (int)KD.SDK.SceneEnum.ObjectType.DOOR)
                {
                    Article door = new Article(this.CurrentAppli, objectID);
                    doorList.Add(door);
                }
            }
            return doorList;
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
            structureLineEGIList.Add(ItemKey.RoomHeight + KD.StringTools.Const.EqualSign + this.ConvertCommaToDot(room) + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.Manufacturer + KD.StringTools.Const.EqualSign + _fileEDI.ManufacturerID() + Separator.NewLine);
            structureLineEGIList.Add(ItemKey.System + KD.StringTools.Const.EqualSign + _orderInformations.GetNameAndVersionSoftware() + Separator.NewLine);
        }
        private void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case ItemValue.V1_50: //DISCAC
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    sceneDimZ = 0; 
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case ItemValue.V1_51: //FBD , BAUFORMAT
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = -this.CurrentAppli.SceneDimY / 2;
                    sceneDimZ = 0;
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                    //sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    //sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    //sceneDimZ = 0;
                    //angleScene = (270 * System.Math.PI) / 180;
                default: //V1_51
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = -this.CurrentAppli.SceneDimY / 2;
                    sceneDimZ = 0;
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
            }

        }
        //Wall
        public void SetWallInformations(List<Wall> wallList)
        {
            this.SetSceneReference(version);

            int index = 1;
            foreach (Wall wall in wallList)
            {
                this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);

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
        public void SetDoorInformations(List<Article> doorList)
        {
            this.SetSceneReference(version);

            int index = 1;
            foreach (Article door in doorList)
            {
                this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);

                structureLineEGIList.Add(Indexation(SegmentName.Door_, index));
                structureLineEGIList.Add(PositionX(door.PositionX));
                structureLineEGIList.Add(PositionY(door.PositionY));
                structureLineEGIList.Add(PositionZ(door.PositionZ));
                structureLineEGIList.Add(DimensionX(door.DimensionX));
                structureLineEGIList.Add(DimensionZ(door.DimensionZ));
                structureLineEGIList.Add(DimensionY(door.DimensionY));//
                structureLineEGIList.Add(Angle(door.AngleOXY));

                structureLineEGIList.Add(DoorHinge(door));
                structureLineEGIList.Add(DoorOpening(door));
                structureLineEGIList.Add(DoorWallRefNo(door));
                structureLineEGIList.Add(DoorRefPntXRel(door));
                structureLineEGIList.Add(DoorRefPntYRel(door));
                structureLineEGIList.Add(DoorRefPntZRel(door));

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
        private string DoorOpening(Article article)
        {
            string opening = String.Empty;
            if (article.Host != null)
            {
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(article.Host.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    if (article.AngleOXY == article.Host.AngleOXY)
                    {
                        opening = ItemValue.InWards;
                    }
                    else
                    {
                        opening = ItemValue.OutWard;
                    }
                }
                
            }
            return ItemKey.Opening + KD.StringTools.Const.EqualSign + opening + Separator.NewLine;
        }
        private string DoorWallRefNo(Article article)
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
        private string DoorRefPntXRel(Article article)
        {
            string refPntXRel = String.Empty;
            if (article.Host != null)
            {
                string objectType = this.CurrentAppli.Scene.ObjectGetInfo(article.Host.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE);
                if (Convert.ToInt16(objectType) == Wall.Const.TypeWall)
                {
                    //double value = this.CurrentAppli.Scene.ObjectGetDistanceFromObject(article.ObjectId, false, article.Host.ObjectId, false, KD.SDK.SceneEnum.ViewType.TOP, 0);
                    double unit = KD.StringTools.Convert.ToDouble(this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.UNITVALUE));
                    double value = article.GetObjectInfo(KD.SDK.SceneEnum.ObjectInfo.DISTLEFTWALL, unit);
                    refPntXRel = value.ToString();
                }
            }
            return ItemKey.RefPntXRel + KD.StringTools.Const.EqualSign + refPntXRel + Separator.NewLine;
        }
        private string DoorRefPntYRel(Article article)
        {
            return ItemKey.RefPntYRel + KD.StringTools.Const.EqualSign + "0" + Separator.NewLine;
        }
        private string DoorRefPntZRel(Article article)
        {
            return ItemKey.RefPntZRel + KD.StringTools.Const.EqualSign + "0" + Separator.NewLine;
        }

        //Article
        private void MoveArticlePerRepere(Article article)
        {
            posX = article.PositionX;
            posY = article.PositionY;
            posZ = article.PositionZ;
            a = article.AngleOXY - 180;

            double angle = 0.0;
            switch (version.ToUpper())
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

            this.SetSceneReference(version);

            int index = 1;

            foreach (Article article in articles)
            {
                SegmentClassification segmentClassification = new SegmentClassification(article);

                if ((!String.IsNullOrEmpty(article.Number.ToString()) && article.Number != KD.Const.UnknownId) || segmentClassification.IsArticleLinear())
                {                    
                    this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);
                    //_orderInformations.ReleaseChar
                    structureLineEGIList.Add(Indexation(SegmentName.Article_, index));
                    structureLineEGIList.Add(ArticleManufacturer(article.KeyRef));                    
                    structureLineEGIList.Add(ArticleRefNo(article.ObjectId.ToString())); //RFF.LI  
                    structureLineEGIList.Add(ArticleRefPos(article.ObjectId.ToString())); // RFF.ON

                    int polyType = this.GetArticlePolyType(article);
                    this.SetAllPositionsAndDimensions(segmentClassification, article, polyType);

                    bool hasPolytype = false;
                    string[] polyPoints = { };                    
                    if (polyType != KD.Const.UnknownId)
                    {
                        polyPoints = this.GetArticlePolyPoint(article);                       
                        hasPolytype = true;
                    }                                      

                    structureLineEGIList.Add(PositionX(posX));
                    structureLineEGIList.Add(PositionY(posY));                    
                    structureLineEGIList.Add(PositionZ(posZ));
                    structureLineEGIList.Add(ArticleDimensionX(dimX));
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
                    //structureLineEGIList.Add(ArticleKeyReference(article.KeyRef));
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

        private string ArticleManufacturer(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
                string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
                if (articleInfos != null)
                {
                    string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                    return ItemKey.Manufacturer + KD.StringTools.Const.EqualSign + articleInfo[OrderConstants.ArticleSupplierId_InFile_Position] + Separator.NewLine;
                }
            }
            return null;
        }
        private string ArticleReference(string keyRef)
        {
            if (!String.IsNullOrEmpty(keyRef))
            {
                return ItemKey.Name + KD.StringTools.Const.EqualSign + this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore) + Separator.NewLine; ;
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
            keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(FileEDI.separatorArticleField);
                if (articleInfo.Length > 7)
                {
                    for (int index = 7; index <= articleInfo.Length - 1; index++)
                    {
                        list.Add(ItemKey.Measure_ + this.ConvertCommaToDot(articleInfo[index].ToUpper()) + Separator.NewLine);
                    }
                }
            }
            return list;
        }
        private string ArticleDimensionX(double value)
        {
            string data = ItemKey.Measure_B + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionX(double value)
        {
            string data = ItemKey.Measure_B1 + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionXE(double value)
        {
            string data = ItemKey.Measure_BE + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionZ(double value)
        {
            string data = ItemKey.Measure_H + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleDimensionY(Article article, double value)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);
            if (segmentClassification.IsArticleUnit())
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
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionY(double value)
        {
            string data = ItemKey.Measure_T1 + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleAngleDimensionYE(double value)
        {
            string data = ItemKey.Measure_TE + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string ArticleConstructionType(string keyRef)
        {
            keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(KD.CharTools.Const.Pipe);
                return ItemKey.ConstructionType + KD.StringTools.Const.EqualSign + articleInfo[OrderConstants.ArticleConstructionId_InFile_Position] + Separator.NewLine;
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
            keyRef = this._utility.DelCharAndAllAfter(keyRef, KD.StringTools.Const.Underscore);
            string articleInfos = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (articleInfos != null)
            {
                string[] articleInfo = articleInfos.Split(FileEDI.separatorArticleField);
                return articleInfo[OrderConstants.ArticleShape_InFile_Position];
            }
            return KD.StringTools.Const.Zero;
        }        

        private void SetAllPositionsAndDimensions(SegmentClassification segmentClassification, Article article, int polytype)
        {
            posZ = this.GetRealPositionZByPosedOnOrUnder(article);

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

        private string Indexation(string sectionName, int index)
        {
            return KD.StringTools.Format.Bracketed(sectionName + index.ToString(SegmentFormat.FourZero)) + Separator.NewLine;
        }
        private string PositionX(double value)
        {
            string data = ItemKey.RefPntX + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string PositionY(double value)
        {
            string data = ItemKey.RefPntY + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string PositionZ(double value)
        {
            string data = ItemKey.RefPntZ + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionX(double value)
        {
            string data = ItemKey.Width + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionY(double value)
        {
            string data = ItemKey.Depth + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        private string DimensionZ(double value)
        {
            string data = ItemKey.Height + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }     
        private string Angle(double value)
        {
            string data = ItemKey.AngleZ + KD.StringTools.Const.EqualSign + value.ToString(SegmentFormat.DotDecimal);
            return this.ConvertCommaToDot(data) + Separator.NewLine;
        }
        #endregion
    }
}

