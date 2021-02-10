using System;
using System.Collections.Generic;
using System.Globalization;

using KD.SDKComponent;
using KD.CatalogProperties;
using KD.Analysis;
using KD.Model;

using Eancom;
using TT.Import.EGI;

namespace Ord_Eancom
{
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

        private const int furnitureHeadingMaxNb = 19; // meuble 0 à 19 = 20 rubriques meubles

        private string supplierID = String.Empty;
        private readonly List<string> releaseList = new List<string>() { KD.StringTools.Const.QuestionMark, //"?",
                                                            KD.StringTools.Const.Colon, //":",
                                                            KD.StringTools.Const.PlusSign, //"+",
                                                            KD.StringTools.Const.SimpleQuote, //"'",
                                                            "’"};

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
        //public string GetFirstPairingCatalogFileName(string csvFileName)
        //{
        //    for (int indexCat = 0; indexCat < this.Articles.Count; indexCat++)
        //    {
        //        string catalogFileName = this.GetCatalogFileName(indexCat);
        //        Reference reference = new Reference(this.CurrentAppli, catalogFileName);
        //        for (int indexRes = 0; indexRes < reference.RessourcesLinesNb; indexRes++)
        //        {
        //            if (reference.Resource_Name(indexRes).Contains(csvFileName))
        //            {
        //                return catalogFileName;
        //            }
        //        }
        //    }
        //    return String.Empty;
        //}
        //public string GetEachPairingCatalogFileName(string csvFileName, string keyRef)
        //{
        //    for (int indexCat = 0; indexCat < this.Articles.Count; indexCat++)
        //    {
        //        string currentKeyRef = this.Articles[indexCat].KeyRef;
        //        if (currentKeyRef == keyRef)
        //        {
        //            string catalogFileName = this.GetCatalogFileName(indexCat);
        //            Reference reference = new Reference(this.CurrentAppli, catalogFileName);
        //            for (int indexRes = 0; indexRes < reference.RessourcesLinesNb; indexRes++)
        //            {
        //                if (reference.Resource_Name(indexRes).Contains(csvFileName))
        //                {
        //                    return catalogFileName;
        //                }
        //            }
        //        }
        //    }
        //    return String.Empty;
        //}
        public string GetFirstPairingCSVFileName(string csvFileName)
        {
            for (int indexCat = 0; indexCat < this.Articles.Count; indexCat++)
            {
                string catalogFileName = this.GetCatalogFileName(indexCat);
                Reference reference = new Reference(this.CurrentAppli, catalogFileName);
                for (int indexRes = 0; indexRes < reference.RessourcesLinesNb; indexRes++)
                {
                    if (reference.Resource_Name(indexRes).Contains(csvFileName))
                    {
                        return reference.Resource_Name(indexRes);
                    }
                }
            }
            return String.Empty;
        }
        public string GetEachPairingCSVFileName(string csvFileName, string articleRef)
        {
            for (int indexCat = 0; indexCat < this.Articles.Count; indexCat++)
            {
                string currentRef = this.Articles[indexCat].Ref;
                if (currentRef == articleRef)
                {
                    string catalogFileName = this.GetCatalogFileName(indexCat);
                    Reference reference = new Reference(this.CurrentAppli, catalogFileName);
                    for (int indexRes = 0; indexRes < reference.RessourcesLinesNb; indexRes++)
                    {
                        if (reference.Resource_Name(indexRes).Contains(csvFileName))
                        {
                            return reference.Resource_Name(indexRes);
                        }
                    }
                }
            }
            return csvFileName;
        }
        public Article GetArticleWithModel()
        {
            foreach (Article article in this.Articles)
            {
                if (article.HeadingRank <= OrderInformations.furnitureHeadingMaxNb)
                {
                    return article;
                }
            }
            return null;
        }
        public string GetRootOrderDir()
        {
            string dir = this.CurrentAppli.GetCallParamsInfoDirect(CallParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);           
            return dir;
        }
        public string GetOrderDir()
        {
            string dir = this.CurrentAppli.GetCallParamsInfoDirect(CallParamsBlock, KD.SDK.AppliEnum.CallParamId.ORDERDIRECTORY);
            string dirEnd = System.IO.Path.Combine(dir, CurrentAppli.SceneName);

            if (!System.IO.Directory.Exists(dirEnd))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(dirEnd);
                }
                catch (Exception)
                {
                    return dir;   
                }                
            }
            return dirEnd;
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
        public bool SetSupplierEmail(string value)
        {
            int rank = this.CurrentAppli.GetSupplierRankFromIdent(supplierID);
            bool ok = this.CurrentAppli.SupplierSetInfo(rank, value, KD.SDK.AppliEnum.SupplierInfo.EMAIL);
            return ok;
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
            if (_sceneAnalysis.Article != null)
            {
                bool IsGenerik = _sceneAnalysis.GetGenericFinishes(out string[] generikFinishTypes, out string[] generikFinishes);
                if (IsGenerik)
                {
                    int.TryParse(generikFinishTypes[0], out int generikFinishType);
                    int.TryParse(generikFinishes[0], out int generikFinish);
                    return _sceneAnalysis.GetCatalogFinishCodeAndName(generikFinishType, generikFinish);
                }
            }
            return String.Empty;
        }
        public List<string> GetGenericCatalogFinishCodeAndName()
        {
            List<string> finishesList = new List<string>();
            _sceneAnalysis = new SceneAnalysis(this.GetArticleWithModel());

            if (_sceneAnalysis.Article != null)
            {
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
            if (_segmentClassification.IsArticleWorkTop() || _segmentClassification.IsArticleShape() || _segmentClassification.IsArticleLinear() ||
                (_segmentClassification.IsArticleSplashbackPanel() && _segmentClassification.IsMeasurementsChange()))
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
}
