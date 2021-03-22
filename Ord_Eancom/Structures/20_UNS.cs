using Eancom;

namespace Ord_Eancom
{
    public class UNS
    {
        public const string E0081 = "S";       

        public UNS()
        {           
        }

        public string Add()
        {
            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.UNS + Separator.DataGroup + E0081 + Separator.EndLine;
        }
    }
}
