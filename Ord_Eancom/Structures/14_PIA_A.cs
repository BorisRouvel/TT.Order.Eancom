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
        #region //Table: les valeurs 1 à 99 sont conforment avec EDIGRAPH        
        //1 Plinth                                                  Socle
        //2 Worktop                                                 Plan de travail
        //3 Wall seal profile                                       Crédence
        //4 Light pelmet                                            Cache lumière
        //5 Cornice                                                 Corniche
        //6 Projecting cornice panel                                Panneau de corniche en saillie
        //7 Railing                                                 Garde-corps
        //8 Ceiling infill panel                                    Panneau de remplissage de plafond(faux plafond)
        //99 Obstacle                                               Obstacle
        //100 Block                                                 Bloc
        //101 Block credit note                                     Note de crédit du bloc
        //102 Surcharge at item level                               Supplément au niveau des articles
        //103 Surcharge added up for entire kitchen plan            Supplément ajouté pour le plan de cuisine entier
        #endregion

   OrderInformations _orderInformationsFromArticles = null;
        FileEDI _fileEDI = null;
        C212 c212 = null;
        UtilitySegment utility = null;

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
            public const string E7140_1 = "1";
            public const string E7140_2 = "2";
            public const string E7140_3 = "3";
            public const string E7140_4 = "4";
            public const string E7140_5 = "5";
            public const string E7140_6 = "6";
            public const string E7140_7 = "7";
            public const string E7140_8 = "8";
            public const string E7140_99 = "99";
            public const string E7140_100 = "100";
            public const string E7140_101 = "101";
            public const string E7140_102 = "102";
            public const string E7140_103 = "103";

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
            //public string AddBlock()
            //{
            //    return this.E7140 + Separator.DataElement + this.E7143 + Separator.DataElement + Separator.DataElement + this.E3055;
            //}

        }  
        
        public PIA_A(OrderInformations orderInformationsFromArticles, FileEDI fileEDI)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;
            _fileEDI = fileEDI;
            c212 = new C212();
            utility = new UtilitySegment();
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
                string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                if (articleInformation.Length > PairingTablePosition.ArticleEDPNumber)
                {
                    return articleInformation[PairingTablePosition.ArticleEDPNumber];
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
                string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                if (articleInformation.Length > PairingTablePosition.ArticleHinge)
                {
                    string hinge = articleInformation[PairingTablePosition.ArticleHinge];
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
                string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                if (articleInformation.Length > PairingTablePosition.ArticleHinge)
                {
                    hinge = articleInformation[PairingTablePosition.ArticleHinge];
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
                string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                if (articleInformation.Length > PairingTablePosition.ArticleConstructionId)
                {
                    string constructionID = articleInformation[PairingTablePosition.ArticleConstructionId];
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
                    string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                    if (articleInformation.Length > PairingTablePosition.ArticleSupplierId)
                    {
                        c212.E7140 = articleInformation[PairingTablePosition.ArticleSupplierId]; // "External Manufacturer ID";
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
                    string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                    if (articleInformation.Length > PairingTablePosition.ArticleSerieNo)
                    {
                        c212.E7140 = articleInformation[PairingTablePosition.ArticleSerieNo]; // "External Serie No"; 
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
            string reference = article.KeyRef;
                       
            if (utility.HasBlockSet(article))
            {
                reference = article.Ref;
            }
           
            _e4347 = PIA_A.E4347_5;
            c212.E7140 = Tools.DelCharAndAllAfter(reference, KD.StringTools.Const.Underscore);
            c212.E7143 = C212.E7143_SA;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_EDPNumber(Article article) //EDI
        {
            _e4347 = PIA_A.E4347_5;           
            c212.E7140 = String.Empty;//Tools.DelCharAndAllAfter(article.CodeNoDoublons, KD.StringTools.Const.Underscore);

            string articleReferenceKey = _fileEDI.ArticleReferenceKey(article.Ref, 1);            
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(Separator.ArticleFieldEDI);
                if (articleInformation.Length > PairingTablePosition.ArticleEDPNumber)
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
                    //foreach (string codeAndNameLine in codeAndNameList)
                    //{
                    for (int i = 0; i < codeAndNameList.Count; i++)
                    {
                        //string codeAndNameLine = codeAndNameList[i];                       
                        if (codeAndNameList[i].StartsWith(OrderConstants.IdemFinishCode) || !GenericCodeAndNameList.Contains(codeAndNameList[i]))
                        {
                            //string[] codeAndName = codeAndNameLine.Split(KD.CharTools.Const.SemiColon);
                            string[] codeAndName = codeAndNameList[i].Split(KD.CharTools.Const.SemiColon);
                            if (codeAndName.Length == 4)
                            {
                                string code = Tools.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                                code = Tools.DelCharAndAllAfter(code, KD.StringTools.Const.Colon);
                                code = _orderInformationsFromArticles.ReleaseChar(code);

                                if (utility.IsAssemblyWorktop(codeAndName[3]))
                                {
                                    continue;
                                }
                                string name = codeAndName[2];
                                name = _orderInformationsFromArticles.ReleaseChar(name);

                                // put the same code an name when code = IDEM
                                if (code == OrderConstants.IdemFinishCode)
                                {
                                    if (i >= 1 & codeAndNameList[i - 1] != null & !String.IsNullOrEmpty(codeAndNameList[i - 1]))
                                    {
                                        if (GenericCodeAndNameList.Contains(codeAndNameList[i - 1]))
                                        {
                                            continue;
                                        }
                                        string[] idemCodeAndName = codeAndNameList[i - 1].Split(KD.CharTools.Const.SemiColon);
                                        if (idemCodeAndName.Length == 4)
                                        {
                                            code = Tools.DelCharAndAllAfter(idemCodeAndName[0], KD.StringTools.Const.Underscore);
                                            code = Tools.DelCharAndAllAfter(code, KD.StringTools.Const.Colon);
                                            code = _orderInformationsFromArticles.ReleaseChar(code);

                                            name = idemCodeAndName[2];
                                            name = _orderInformationsFromArticles.ReleaseChar(name);
                                        }
                                    }
                                }

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
                                    nameCharStart += UtilitySegment.nameCharLen;

                                    name = utility.GetFollowingChar(name, UtilitySegment.nameCharLen);
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
            if (utility.HasBlockSet(article))
            {
                _e4347 = PIA_A.E4347_1;
                c212.E7140 = C212.E7140_100;
                c212.E7143 = C212.E7143_BK;
                c212.E3055 = C212.E3055_91;
               
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return this.BuildLine();               
            }
            else
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
            }
            return null;
        }

        public string Add_WorktopAssemblyTypeNo(string assemblyReference)
        {
            assemblyReference  = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

            _e4347 = PIA_A.E4347_5;           
            c212.E7140 = _orderInformationsFromArticles.ReleaseChar(assemblyReference);
            c212.E7143 = C212.E7143_SA;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_WorktopAssemblyEDPNumber(string assemblyReference) //EDI
        {
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

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
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

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
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Underscore);
            assemblyReference = Tools.DelCharAndAllAfter(assemblyReference, KD.StringTools.Const.Colon);

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
