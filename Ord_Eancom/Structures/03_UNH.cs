using Ord_Eancom;

namespace Eancom
{
    public class UNH
    {
        OrderInformations _orderInformations = null;

        private string _e0062;
        public string E0062
        {
            get
            {
                return _e0062;
            }
            set
            {
                _e0062 = value;
            }
        }

        public class S009
        {
            public const string E0065 = "ORDERS";
            public const string E0052 = "D";
            public const string E0054 = "97A";
            public const string E0051 = "UN";
            public const string E0057 = "EAN007";

            public S009()
            {                
            }

            public string Add()
            {
                return (E0065 + Separator.DataElement + E0052 + Separator.DataElement + E0054 + Separator.DataElement + E0051 + Separator.DataElement + E0057);
            }
        }
    

        public UNH(OrderInformations orderInformations)
        {
             _orderInformations = orderInformations;            
        }

        public string Add()
        {
            _e0062 = _orderInformations.GetReferenceNumberEDIMessage();
            S009 s009 = new S009();

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.UNH + Separator.DataGroup + this.E0062 + Separator.DataGroup + s009.Add() + Separator.EndLine;             
        }

    }
}
