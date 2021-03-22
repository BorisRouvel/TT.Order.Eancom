using Eancom;

namespace Ord_Eancom
{
    public class BGM
    {
        OrderInformations _orderInformations = null;

        private string _e1004;
        public string E1004
        {
            get
            {
                return _e1004;
            }
            set
            {
                _e1004 = value;
            }
        }

        public class C002
        {
            public const string E1001_220 = "220";
            public const string E1001_226 = "226";
            public const string E1001_224 = "224";

            public C002()
            {
            }

            public string Add()
            {
                return E1001_220 + Separator.DataGroup;
            }
        }

        public const string E1225 = "9"; 

        public BGM(OrderInformations orderInformations)
        {
             _orderInformations = orderInformations;
        }

        public string Add()
        {
            C002 c002 = new C002();
            _e1004 = _orderInformations.GetOrderNumber();

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.BGM + Separator.DataGroup + c002.Add() + this.E1004 + Separator.DataGroup + E1225 + Separator.EndLine;
        }
    }
}
