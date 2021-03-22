using System;

using KD.Model;

using Ord_Eancom;

namespace Eancom
{
    public class LIN_A
    {
        OrderInformations _orderInformationsFromArticles = null;
        FileEDI _fileEDI = null;
        C212 c212 = null;

        private string _e1082;
        public string E1082
        {
            get
            {
                return _e1082;
            }
            set
            {
                _e1082 = value;
            }
        }

        private string _e1222;
        public string E1222
        {
            get
            {
                return _e1222;
            }
            set
            {
                _e1222 = value;
            }
        }       

        public class C212
        {
            public const string E7143_EN = "EN";
            public const string E7143_SG = "SG";

            private string _e7140 = String.Empty;
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

            private string _e7143 = String.Empty;
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

            public C212()
            {
            }

            public string Add()
            {
                return this.E7140 + this.E7143 + Separator.DataGroup + Separator.DataGroup;
            }

        }

        public static string _consecutiveNumbering;

        public LIN_A(OrderInformations orderInformationsFromArticles, string consecutiveNumbering, FileEDI fileEDI)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;
            _fileEDI = fileEDI;
            c212 = new C212();
            _consecutiveNumbering = consecutiveNumbering;
        }

        private string BuildLine()
        {
            return StructureEDI.LIN_A + Separator.DataGroup + this.E1082 + Separator.DataGroup + Separator.DataGroup + c212.Add() + this.E1222 + Separator.EndLine;
        }

        private string GetEANNumber(string keyRef)
        {
            string articleReferenceKey = _fileEDI.ArticleReferenceKey(keyRef, 1);
            if (!String.IsNullOrEmpty(articleReferenceKey))
            {
                string[] articleInformation = articleReferenceKey.Split(KD.CharTools.Const.SemiColon);
                if (articleInformation.Length > PairingTablePosition.ArticleEANNumber)
                {
                    return articleInformation[PairingTablePosition.ArticleEANNumber]; //"EAN_NUMBER"; //Provide EAN_NUMBER somewhere in catalog
                }
            }
            return String.Empty;
        }
        private string GetItemLevel(Article article)
        {
            OrderInformations articleInformations = new OrderInformations(article);
            int itemLevel = articleInformations.GetComponentLevel();// article);

            Article parent = article.Parent;
            if (parent != null && parent.IsValid)
            {
                OrderInformations parentInformations = new OrderInformations(parent);
                int priceType = parentInformations.GetPriceType();// parent);
                while (parent.IsValid && parent.KeyRef.StartsWith(KD.StringTools.Const.Underscore) || _orderInformationsFromArticles.IsRegroupPortion(priceType))
                {
                    itemLevel -= 1;
                    parent = parent.Parent;
                    if (parent == null) { break; }
                    parentInformations = new OrderInformations(parent);
                    priceType = parentInformations.GetPriceType();// parent);
                }
            }
            return itemLevel.ToString();
        }

        public string Add_EN(Article article)//Provide EAN_NUMBER somewhere in catalog ?
        {            
            _e1082 = _consecutiveNumbering;

            c212.E7140 = this.GetEANNumber(article.KeyRef);
            c212.E7143 = String.Empty;

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                c212.E7143 = Separator.DataElement + C212.E7143_EN;
            }

            _e1222 = this.GetItemLevel(article);

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_SG(Article article)//Provide EAN_NUMBER somewhere in catalog
        {
            _e1082 = _consecutiveNumbering;

            c212.E7140 = this.GetEANNumber(article.KeyRef); 

            c212.E7143 = C212.E7143_SG;
            _e1222 = KD.StringTools.Const.One;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
       
        public string Add_WorktopAssemblyNumberEN(Article article, string assemblyReference)//Provide EAN_NUMBER somewhere in catalog ?
        {
            _e1082 = _consecutiveNumbering;

            c212.E7140 = this.GetEANNumber(assemblyReference);
            c212.E7143 = String.Empty;

            if (!String.IsNullOrEmpty(c212.E7140))
            {
                c212.E7143 = Separator.DataElement + C212.E7143_EN;
            }

            _e1222 = this.GetItemLevel(article);

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
    }
}