using Ord_Eancom;

namespace Eancom
{
    public class LIN_H
    {
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

        private const string E1222 = "0";       

        public class C212
        {
            private const string E7140 = "SPEZIFIKATION";
            private const string E7143 = "MF";
          
            public C212()
            {
            }

            public string Add()
            {
                return E7140 + Separator.DataElement + E7143 + Separator.DataGroup + Separator.DataGroup;
            }
        }

        public string _consecutiveNumbering;

        public LIN_H(string consecutiveNumbering)
        {
            _consecutiveNumbering = consecutiveNumbering;
        }

        public string Add()
        {
            C212 c212 = new C212();
            _e1082 = _consecutiveNumbering;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.LIN_H + Separator.DataGroup + this.E1082 + Separator.DataGroup + Separator.DataGroup + c212.Add() + E1222 + Separator.EndLine;
        }
    }
}
