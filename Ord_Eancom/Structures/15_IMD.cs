using KD.Model;
using Eancom;

namespace Ord_Eancom
{
    public class IMD
    {
        OrderInformations _orderInformationsFromArticles = null;       
        C273 c273 = null;        

        public const string E7077 = "F";

       
        public class C273
        {
            public string _e7008;
            public string E7008
            {
                get
                {
                    return _e7008;
                }
                set
                {
                    _e7008 = value;
                }
            }

            public C273()
            {
            }

            public string Add()
            {
                return Separator.DataElement + Separator.DataElement + Separator.DataElement + this.E7008;
            }
        }

        public IMD(OrderInformations orderInformationsFromArticles)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;            
            c273 = new C273();           
        }

        public string Add(Article article)
        {
            c273.E7008 = article.Name;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.IMD + Separator.DataGroup + E7077 + Separator.DataGroup + Separator.DataGroup + c273.Add() + Separator.EndLine;
        }
        
        public string Add_WorktopAssemblyNumber(string assemblyName)
        {           
            c273.E7008 = assemblyName;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.IMD + Separator.DataGroup + E7077 + Separator.DataGroup + Separator.DataGroup + c273.Add() + Separator.EndLine;
        }
    }
}
