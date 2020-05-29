using Ord_Eancom;

namespace Eancom
{
    public class UNT
    {
        OrderInformations _orderInformations = null;        

        public UNT(OrderInformations orderInformations) //string e0074, string e0062)
        {
            _orderInformations = orderInformations;            
        }


        public string Add()//int segmentsNb)
        {
            OrderWrite.segmentNumberBetweenUNHandUNT += 1;

            string e0074 = OrderWrite.segmentNumberBetweenUNHandUNT.ToString() + Separator.DataGroup;
            string e0062 = _orderInformations.GetReferenceNumberEDIMessage();
            
            return StructureEDI.UNT + Separator.DataGroup + e0074 + e0062 + Separator.EndLine;           
        }
    }
}
