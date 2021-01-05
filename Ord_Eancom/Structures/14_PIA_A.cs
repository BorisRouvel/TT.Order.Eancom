using System;
using System.Collections.Generic;
using System.Text;

using KD.Model;
using KD.Analysis;

using Ord_Eancom;

namespace Eancom
{
    public class PIA_A
    {
        OrderInformations _orderInformationsFromArticles = null;
        FileEDI _fileEDI = null;
        C212 c212 = null;
        Utility utility = null;

        public const string E4347_1 = "1";
        public const string E4347_5 = "5";

        private string _e4347;
        private string E4347
        {
            get
            {
                return _e4347;
            }
            set
            {
                _e4347 = value;
            }
        }

        public class C212
        {
            public const string E7143_36 = "36";
            public const string E7143_18 = "18";
            public const string E7143_SA = "SA";
            public const string E7143_MF = "MF";
            public const string E7143_6 = "6";
            public const string E7143_67 = "67";
            public const string E7143_68 = "68";
            public const string E7143_BK = "BK";

            public const string E3055_91 = "91";
            public const string E3055_92 = "92";

            private string _e7140;
            public string E7140
            {
                get
                {
                    return _e7140;
                }
                set
                {
                    _e7140 = value;
                }
            }

            private string _e7143;
            public string E7143
            {
                get
                {
                    return _e7143;
                }
                set
                {
                    _e7143 = value;
                }
            }

            private string _e3055;
            public string E3055
            {
                get
                {
                    return _e3055;
                }
                set
                {
                    _e3055 = value;
                }
            }

            public C212()
            {
            }

            public string Add()
            {
                return this.E7140 + Separator.DataElement + this.E7143 + Separator.DataElement + Separator.DataElement + this.E3055;
            }

        }  
        
        public PIA_A(OrderInformations orderInformationsFromArticles, FileEDI fileEDI)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;
            _fileEDI = fileEDI;
            c212 = new C212();
            utility = new Utility();
        }

        private string BuildLine()
        {
            return StructureEDI.PIA_A + Separator.DataGroup + this.E4347 + Separator.DataGroup + c212.Add() + Separator.EndLine;
        }

        private bool IsImportantCorrectCompletion(Article article)
        {
            if (article.Ref == "CorrectCompletion") //Provide somewhere in catalog
            {
                return true; 
            }
            return false;
        }
        private bool IsExternalSerieNo(Article article)
        {
            if (article.Ref == "ExternalSerieNo") //Provide somewhere in catalog
            {
                return true;
            }
            return false;
        }
        private string GetEDPNumber(string artRef)
        {
            string articleReferenceKey = _fileEDI.ArticleReferenceKey(artRef, 1);

            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                if (articleInformation.Length > OrderConstants.ArticleEDPNumber_InFile_Position)
                {
                    return articleInformation[OrderConstants.ArticleEDPNumber_InFile_Position];
                }
            }
            return String.Empty;
        }
        // Do not use IDMHinge
        private string GetIDMHinge(string artRef)
        {          
            string articleReferenceKey = _fileEDI.ArticleReferenceKey(artRef, 1);
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                if (articleInformation.Length > OrderConstants.ArticleHinge_InFile_Position)
                {
                    string hinge = articleInformation[OrderConstants.ArticleHinge_InFile_Position];
                    if (!String.IsNullOrEmpty(hinge))
                    //if (hinge == "L" || hinge == "R" || hinge == "M")
                    {
                        return hinge;
                    }
                }
            }
            return String.Empty;
        }
        private string GetHinge(Article article)
        {
            string hinge = String.Empty;
            string articleReferenceKey = _fileEDI.ArticleReferenceKey(article.Ref, 1);
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                if (articleInformation.Length > OrderConstants.ArticleHinge_InFile_Position)
                {
                    hinge = articleInformation[OrderConstants.ArticleHinge_InFile_Position];
                    if (hinge == "L" || hinge == "R" || hinge == "M")
                    {
                        return hinge;
                    }
                    else
                    {
                        if (article.Handing == article.CurrentAppli.GetTranslatedText("G"))
                        {
                            return "L";
                        }
                        else if (article.Handing == article.CurrentAppli.GetTranslatedText("D"))
                        {
                            return "R";
                        }
                    }
                }
            }           
            return String.Empty;
        }
        private string GetIDMConstructionID(string artRef)
        {
            string articleReferenceKey = _fileEDI.ArticleReferenceKey(artRef, 1);
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                if (articleInformation.Length > OrderConstants.ArticleConstructionId_InFile_Position)
                {
                    string constructionID = articleInformation[OrderConstants.ArticleConstructionId_InFile_Position];
                    if (constructionID == "L" || constructionID == "R")
                    {
                        return constructionID;
                    }
                }
            }
            return String.Empty;
        }
        private string GetVisibleSide(Article article) //How to set in catalog and explain top and bottom ?
        {
            string visibleSide = String.Empty;
            
            string filter = FilterArticle.strFilterToGetArticleByCodeValidPlaced(article.CatalogCode);
            Articles childs = article.GetChildren(filter);

            foreach (Article child in childs)
            {
                if (child.BlockCode  == "CPG")
                {
                    visibleSide += "L";
                }
                if (child.BlockCode == "CPD")
                {
                    visibleSide += "R";
                }
                if (child.BlockCode == "CPT")
                {
                    visibleSide += "O";
                }
                if (child.BlockCode == "CPB")
                {
                    visibleSide += "U";
                }
                if (child.BlockCode == "CPR")
                {
                    visibleSide += "H";
                }
            }
            return visibleSide;
        }

        public string Add_ExternalManufacturerID(Article article) //How to put this ID in catalog ?
        {
            if (IsImportantCorrectCompletion(article))
            {
                _e4347 = PIA_A.E4347_5;
                string articleReferenceKey = _fileEDI.ArticleReferenceKey(article.Ref, 1);
                if (!String.IsNullOrEmpty(articleReferenceKey))
                {
                    string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                    if (articleInformation.Length > OrderConstants.ArticleSupplierId_InFile_Position)
                    {
                        c212.E7140 = articleInformation[OrderConstants.ArticleSupplierId_InFile_Position]; // "External Manufacturer ID";
                        c212.E7143 = C212.E7143_36;
                        c212.E3055 = C212.E3055_91;

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        return BuildLine();
                    }
                }
            }
            return null;
        }
        public string Add_ExternalSerieNo(Article article)//How to put this Serie No in catalog ?
        {
            if (IsImportantCorrectCompletion(article) || IsExternalSerieNo(article))
            {
                _e4347 = PIA_A.E4347_5;
                string articleReferenceKey = _fileEDI.ArticleReferenceKey(article.Ref, 1);
                if (!String.IsNullOrEmpty(articleReferenceKey))
                {
                    string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                    if (articleInformation.Length > OrderConstants.ArticleSerieNo_InFile_Position)
                    {
                        c212.E7140 = articleInformation[OrderConstants.ArticleSerieNo_InFile_Position]; // "External Serie No"; 
                        c212.E7143 = C212.E7143_18;
                        c212.E3055 = C212.E3055_91;

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        return BuildLine();
                    }
                }
            }
            return null;
        }
        public string Add_TypeNo(Article article)
        {
            _e4347 = PIA_A.E4347_5;
            c212.E7140 = utility.DelCharAndAllAfter(article.KeyRef, KD.StringTools.Const.Underscore);
            c212.E7143 = C212.E7143_SA;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_EDPNumber(Article article) //EDI
        {
            _e4347 = PIA_A.E4347_5;           
            c212.E7140 = String.Empty;//utility.DelCharAndAllAfter(article.CodeNoDoublons, KD.StringTools.Const.Underscore);

            string articleReferenceKey = _fileEDI.ArticleReferenceKey(article.Ref, 1);            
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(FileEDI.separatorArticleField);
                if (articleInformation.Length > OrderConstants.ArticleEDPNumber_InFile_Position)
                {
                    c212.E7140 = articleInformation[2];
                }
            }

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                c212.E7143 = C212.E7143_MF;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;        
        }
        public string Add_Hinge(Article article)
        {
            c212.E7140 = this.GetHinge(article);

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7143 = C212.E7143_6;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
        public string Add_ConstructionID(Article article)
        {
            c212.E7140 = this.GetIDMConstructionID(article.Ref);

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7143 = C212.E7143_67;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
        public string Add_VisibleSide(Article article)
        {
            c212.E7140 = this.GetVisibleSide(article);

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7143 = C212.E7143_68;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
        public string Add_FinishCodeAndName(Article article)
        {
            string dataLine = null;
            List<string> GenericCodeAndNameList = new List<string>();
            GenericCodeAndNameList = _orderInformationsFromArticles.GetGenericCatalogFinishCodeAndName();

            if (GenericCodeAndNameList != null && GenericCodeAndNameList.Count > 0)
            {
                OrderInformations articleInformations = new OrderInformations(article);
                List<string> codeAndNameList = new List<string>();

                codeAndNameList = articleInformations.GetFinishCodeAndName();// article);
                if (codeAndNameList != null && codeAndNameList.Count > 0)
                {                   
                    foreach (string codeAndNameLine in codeAndNameList)
                    {
                        if (!GenericCodeAndNameList.Contains(codeAndNameLine))
                        {
                            string[] codeAndName = codeAndNameLine.Split(KD.CharTools.Const.SemiColon);
                            if (codeAndName.Length == 4)
                            {
                                string code = utility.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                                code = utility.DelCharAndAllAfter(code, KD.StringTools.Const.Colon);
                                code = _orderInformationsFromArticles.ReleaseChar(code);

                                if (utility.IsAssemblyWorktop(codeAndName[3]))
                                {
                                    continue;
                                }
                                string name = codeAndName[2];
                                name = _orderInformationsFromArticles.ReleaseChar(name);

                                int nameCharStart = 0;

                                for (int c = 0; c < 2; c++)
                                {
                                    _e4347 = PIA_A.E4347_5;
                                    code = utility.GetCodeLen(code);

                                    StringBuilder codeStringBuilder = utility.CodeStringBuilder(code);
                                    StringBuilder nameStringBuilder = utility.NameStringBuilder(name, nameCharStart);

                                    c212.E7140 = codeStringBuilder.ToString() + nameStringBuilder.ToString();
                                    c212.E7143 = KD.StringTools.Const.One;
                                    if (!utility.IsModel(codeAndName[3]))
                                    {
                                        c212.E7143 = _fileEDI.FinishTypeVariant(codeAndName[3]);
                                        if (!String.IsNullOrEmpty(c212.E7143) && c212.E7143.Contains(KD.StringTools.Const.Pipe))
                                        {
                                            c212.E7143 = String.Empty;
                                        }
                                    }
                                                                       
                                    c212.E3055 = C212.E3055_91;

                                    if (String.IsNullOrEmpty(code))
                                    {
                                        c212.E3055 = C212.E3055_92;
                                    }

                                    dataLine += BuildLine();

                                    OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                                    nameCharStart += Utility.nameCharLen;

                                    name = utility.GetFollowingChar(name, Utility.nameCharLen);
                                    if (String.IsNullOrEmpty(name))
                                    {
                                        break;
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            return dataLine;
        }
        public string Add_LongPartType(Article article)
        {
            OrderInformations articleInformations = new OrderInformations(article);
            List<string> codeAndNameList = new List<string>();

            codeAndNameList = articleInformations.GetFinishCodeAndName();// article);
            if (codeAndNameList != null && codeAndNameList.Count > 0)
            {
                foreach (string codeAndNameLine in codeAndNameList)
                {
                    string[] codeAndName = codeAndNameLine.Split(KD.CharTools.Const.SemiColon);
                    if (codeAndName.Length == 4)
                    {
                        _e4347 = PIA_A.E4347_1;
                        c212.E7140 = utility.GetLongPartType(codeAndName[3]);
                        c212.E7143 = C212.E7143_BK;
                        c212.E3055 = C212.E3055_91;

                        if (!String.IsNullOrEmpty(c212.E7140))
                        {
                            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                            return this.BuildLine();
                        }
                    }
                }
            }
            return null;
        }

        public string Add_WorktopAssemblyTypeNo(string assemblyReference)
        {
            assemblyReference  = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

            _e4347 = PIA_A.E4347_5;           
            c212.E7140 = _orderInformationsFromArticles.ReleaseChar(assemblyReference);
            c212.E7143 = C212.E7143_SA;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_WorktopAssemblyEDPNumber(string assemblyReference) //EDI
        {
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

            _e4347 = PIA_A.E4347_5;
            c212.E7140 = _orderInformationsFromArticles.ReleaseChar(this.GetEDPNumber(assemblyReference));

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                c212.E7143 = C212.E7143_MF;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
        public string Add_WorktopAssemblyHinge(string assemblyReference)
        {
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

            c212.E7140 = _orderInformationsFromArticles.ReleaseChar(this.GetIDMHinge(assemblyReference));

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7143 = C212.E7143_6;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
        public string Add_WorktopAssemblyConstructionID(string assemblyReference)
        {
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = utility.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

            c212.E7140 = _orderInformationsFromArticles.ReleaseChar(this.GetIDMConstructionID(assemblyReference));

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7143 = C212.E7143_67;
                c212.E3055 = C212.E3055_91;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return BuildLine();
            }
            return null;
        }
    }
}
