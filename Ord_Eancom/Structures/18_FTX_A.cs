using System;

using KD.Model;

using Ord_Eancom;

namespace Eancom
{ 
    public class FTX_A
    {       
        C108 c108 = null;
        Utility utility = null;

        public const string E4451 = "LIN";

        private readonly string E3453 = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();

        public class C108
        {
            OrderInformations _orderInformations = null;            

            private string _e4440;
            public string E4440
            {
                get
                {
                    return _e4440;
                }
                set
                {
                    value = _e4440;
                }
            }

            public C108(OrderInformations orderInformations)
            {
                _orderInformations = orderInformations;
            }           

            public string Add(Article article)
            {
                OrderInformations articleInformations = new OrderInformations(article);
                _e4440 = articleInformations.GetSupplierCommentArticle();// article);
                
                if (!String.IsNullOrEmpty(this.E4440))
                {
                    return this.E4440;
                }
                return null;
            }
        }

        public FTX_A(OrderInformations orderInformations)
        {
            c108 = new C108(orderInformations);
            utility = new Utility();
        }

        public string BuildLine(string text)
        {
            return StructureEDI.FTX_A + Separator.DataGroup + E4451 + Separator.DataGroup + Separator.DataGroup + Separator.DataGroup + 
                text + Separator.DataElement + this.E3453 + Separator.EndLine;
        }

        public string Add(Article article)
        {
            string allText = c108.Add(article);
            string dataLine = null;

            if (!String.IsNullOrEmpty(allText))
            {
                if (allText.Length <= Utility.freelyWordCharLen)
                {
                    OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                    return this.BuildLine(allText);
                }

                string partText = allText;                
                int startIndex = 0;

                for (int c = 0; c < Utility.freelyLineMaxNb; c++)
                {
                    if (partText.Length > Utility.freelyWordCharLen)
                    {
                        string text = allText.Substring(startIndex, Utility.freelyWordCharLen);
                        dataLine += this.BuildLine(text);

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        startIndex += Utility.freelyWordCharLen;

                        partText = utility.GetFollowingChar(partText, Utility.freelyWordCharLen);
                        if (String.IsNullOrEmpty(partText))
                        {                            
                            return dataLine;
                        }
                    }
                    else
                    {
                        dataLine += this.BuildLine(partText);

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        return dataLine;
                    }
                }              
            }
            return dataLine;
        }
    }
}
