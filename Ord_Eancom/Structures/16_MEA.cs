using System;
using System.Collections.Generic;

using KD.Model;

using Ord_Eancom;

namespace Eancom
{
    public class MEA
    {
        C502 c502 = null;
        C174 c174 = null;
        Utility utility = null;

        public const string E6311 = "AAE"; //dimensions
        public const string Width = "WD";
        public const string Depth = "DP";
        public const string Height = "HT";

        public class C502
        {
            private string _e6313;
            public string E6313
            {
                get
                {
                    return _e6313;
                }
                set
                {
                    value = _e6313;
                }
            }

            public C502()
            {
            }

            public string Add(string mesurementCode)
            {
                _e6313 = mesurementCode;
                return Separator.DataGroup + this.E6313;
            }
        }

        public class C174
        {
            public const string E6411 = "MMT"; //millimeter
            private string _e6314;
            public string E6314
            {
                get
                {
                    return _e6314;
                }
                set
                {
                    value = _e6314;
                }
            }

            public C174()
            {
            }

            public string Add(double measurement)
            {
                _e6314 = measurement.ToString();
                return Separator.DataGroup + E6411 + Separator.DataElement + this.E6314;
            }          
        }

        public MEA()
        {
            c502 = new C502();
            c174 = new C174();
            utility = new Utility();
        }

        public string Add(Article article)
        {
            OrderInformations articleInformations = new OrderInformations(article);
            string dataLine = null;

            if (articleInformations.IsOption_MEA())
            {                
                List<double> dimensionList = new List<double>() { article.DimensionX, article.DimensionY, article.DimensionZ };
                List<string> measureCodeList = new List<string>() { Width, Depth, Height };
                int index = 0;

                foreach (double dimension in dimensionList)
                {
                    if (utility.IsDimensionValid(dimension))
                    {
                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        dataLine += StructureEDI.MEA + Separator.DataGroup + E6311 + c502.Add(measureCodeList[index]) + c174.Add(dimension) + Separator.EndLine;
                    }
                    index += 1;
                }
            }
            return dataLine;
        }
    }
}

