using System.Collections.Generic;
using System;
using System.Text;

using Ord_Eancom;

namespace Eancom
{
    public class PIA_H
    {
        OrderInformations _orderInformationsFromArticles = null;
        FileEDI _fileEDI = null;
        Utility utility = null;
        C212 c212 = null;

        private const string E4347 = "5";
        private const string plinthHeightFeature = "402";        

        List<string> codeAndNameList = new List<string>();        

        public class C212
        {            
            public const string E7143_36 = "36";
            public const string E7143_18 = "18";
            public const string E7143_AA = "AA";

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

            public string Add(OrderInformations orderInformationsFromArticles)
            {
                return orderInformationsFromArticles.ReleaseChar(this.E7140) + Separator.DataElement + this.E7143 + Separator.DataElement + Separator.DataElement + E3055;
            }

        }
        public PIA_H(OrderInformations orderInformationsFromArticles, FileEDI fileEDI)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;
            c212 = new C212();            
            _fileEDI = fileEDI;
            utility = new Utility();
        }

        private string BuildLine()
        {             
            return StructureEDI.PIA_H + Separator.DataGroup + E4347 + Separator.DataGroup + c212.Add(_orderInformationsFromArticles) + Separator.EndLine; 
        }

        public string Add_ManufacturerID()
        {
            c212.E7140 = _fileEDI.ManufacturerID();
            c212.E7143 = C212.E7143_36;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_SerieNo()
        {
            KD.Model.Article article = _orderInformationsFromArticles.GetArticleWithModel();           
            c212.E7140 = _orderInformationsFromArticles.CurrentAppli.CatalogGetCustomInfo(article.CatalogFileName, "SERIENO"); // 
            c212.E7143 = C212.E7143_18;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_CatalogID()
        {
            c212.E7140 = _fileEDI.CatalogID();
            c212.E7143 = C212.E7143_AA;
            c212.E3055 = C212.E3055_91;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return BuildLine();
        }
        public string Add_ModelCodeAndName()
        {
            string dataLine = null;
            string[] codeAndName = _orderInformationsFromArticles.GetCatalogModelCodeAndName().Split(KD.CharTools.Const.SemiColon);

            if (codeAndName.Length == 3)
            {
                string code = utility.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                string name = codeAndName[2];
                int nameCharStart = 0;                

                for (int c = 0; c < Utility.finishLineMaxNb; c++)
                {
                    code = utility.GetCodeLen(code);
                    
                    StringBuilder codeStringBuilder = utility.CodeStringBuilder(code);
                    StringBuilder nameStringBuilder = utility.NameStringBuilder(name, nameCharStart);

                    c212.E7140 = codeStringBuilder.ToString() + nameStringBuilder.ToString();
                    c212.E7143 = KD.StringTools.Const.One;
                    c212.E3055 = C212.E3055_91;
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
            return dataLine;
        }
        public string Add_FinishCodeAndName()
        {
            string dataLine = null;
            codeAndNameList = _orderInformationsFromArticles.GetGenericCatalogFinishCodeAndName();
            codeAndNameList.RemoveAt(0); //Del the index 0 cause we don't need model here

            if (codeAndNameList != null && codeAndNameList.Count > 0)
            {
                foreach (string codeAndNameLine in codeAndNameList)
                {
                    string[] codeAndName = codeAndNameLine.Split(KD.CharTools.Const.SemiColon);
                    if (codeAndName.Length == 4)
                    {
                        string code = utility.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                        if (utility.IsPlinth(codeAndName[3]))//Cause exept plinth height 402 //KD.StringTools.Const.TrueCamelCase
                        {
                            continue;
                        } 
                        string name = codeAndName[2];
                        int nameCharStart = 0;                       

                        for (int c = 0; c < Utility.finishLineMaxNb; c++)
                        {
                            code = utility.GetCodeLen(code);

                            StringBuilder codeStringBuilder = utility.CodeStringBuilder(code);
                            StringBuilder nameStringBuilder = utility.NameStringBuilder(name, nameCharStart);

                            c212.E7140 = codeStringBuilder.ToString() + nameStringBuilder.ToString();
                            string finishAppairage = _fileEDI.FinishTypeVariant(codeAndName[3]);

                            if (!String.IsNullOrEmpty(finishAppairage) && finishAppairage.Contains(KD.StringTools.Const.Pipe))
                            {
                                finishAppairage = String.Empty;
                            }

                            c212.E7143 = finishAppairage;
                            c212.E3055 = C212.E3055_91;

                            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                            dataLine += BuildLine();
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
            return dataLine;
        }
        public string Add_PlinthHeight()
        {
            string dataLine = null;           

            if (codeAndNameList.Count > 1)
            {
                foreach (string codeAndNameFinishLine in codeAndNameList)
                {
                    string[] codeAndName = codeAndNameFinishLine.Split(KD.CharTools.Const.SemiColon);
                    if (codeAndName.Length == 4)
                    {
                        string code = utility.DelCharAndAllAfter(codeAndName[0], KD.StringTools.Const.Underscore);
                        if (!utility.IsPlinth(codeAndName[3])) //Cause get only plinth height 402 //KD.StringTools.Const.FalseCamelCase
                        {
                            continue;
                        } 
                        string name = codeAndName[2];

                        c212.E7140 = code;
                        c212.E7143 = PIA_H.plinthHeightFeature;
                        c212.E3055 = C212.E3055_92;

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        dataLine += BuildLine();
                    }
                }
            }
            return dataLine;
        }
    }
}
