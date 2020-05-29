using System;
using Ord_Eancom;

namespace Eancom
{
    public class CTA
    {
        OrderInformations _orderInformations = null;

        private string _e3139;
        public string E3139
        {
            get
            {
                return _e3139;
            }
            set
            {
                _e3139 = value;
            }
        }

        public const string E3139_OC = "OC";
        public const string E3139_IC = "IC";
        public const string E3139_ODP = "ODP";

        public class C056
        {
            private const int sellerIDCharNb = 17;
            private const int sellerNamesCharNb = 35;

            private string _e3413;
            public string E3413
            {
                get
                {
                    return _e3413;
                }
                set
                {
                    _e3413 = value;
                }
            }

            private string _e3412;
            public string E3412
            {
                get
                {
                    return _e3412;
                }
                set
                {
                    _e3412 = value;
                }
            }

            public C056(string e3413, string e3412)
            {
                _e3413 = CharNumberLimit(e3413, sellerIDCharNb);
                _e3412 = CharNumberLimit(e3412, sellerNamesCharNb);
            }

            private string CharNumberLimit(string text, int value)
            {
                if (text.Length > value)
                {
                    text = text.Substring(0, value);
                }
                return text;
            }

            public string Add()
            {
                return this.E3413 + Separator.DataElement + this.E3412;
            }
        }


        public CTA(OrderInformations orderInformations) //string e3139, string e3413, string e3412)
        {
            _orderInformations = orderInformations;           
        }

        public string Add_Supplier_OC()
        {
            _e3139 = Eancom.CTA.E3139_OC;
            C056 c056 = new C056(String.Empty, _orderInformations.GetSupplierName1());

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.CTA + Separator.DataGroup + this.E3139 + Separator.DataGroup + c056.Add() + Separator.EndLine;          
        }
        public string Add_Seller_OC()
        {
            _e3139 = Eancom.CTA.E3139_OC;
            C056 c056 = new C056( _orderInformations.GetSellerID(),  _orderInformations.GetSellerInformations());

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.CTA + Separator.DataGroup + this.E3139 + Separator.DataGroup + c056.Add() + Separator.EndLine;
        }
    }
}
