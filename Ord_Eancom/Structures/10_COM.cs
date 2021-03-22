using System;
using Eancom;

namespace Ord_Eancom
{
    public class COM
    {
        OrderInformations _orderInformations = null;
        FileEDI _fileEDI = null;

        public class C076
        {
            private string _e3148;
            public string E3148
            {
                get
                {
                    return _e3148;
                }
                set
                {
                    _e3148 = value;
                }
            }

            private string _e3155;
            public string E3155
            {
                get
                {
                    return _e3155;
                }
                set
                {
                    _e3155 = value;
                }
            }

            public const string E3155_TE = "TE";
            public const string E3155_FX = "FX";
            public const string E3155_EM = "EM";

            public C076(string e3148, string e3155)
            {
                _e3148 = e3148;
                _e3155 = e3155;
            }

            public string Add()
            {
                return this.E3148 + Separator.DataElement + this.E3155;
            }
        }

        public COM(OrderInformations orderInformations, FileEDI fileEDI)
        {
            _orderInformations = orderInformations;
            _fileEDI = fileEDI;
        }

        public string Add_Supplier_TE()
        {
            string e3148 = _orderInformations.GetSupplierPhone();
            C076 c076 = new C076(e3148, COM.C076.E3155_TE);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;           
        }
        public string Add_Supplier_FX()
        {
            string e3148 = _orderInformations.GetSupplierFax();
            C076 c076 = new C076(e3148,  COM.C076.E3155_FX);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;            
        }
        public string Add_Supplier_EM()
        {
            string e3148 = _fileEDI.Email(); // _orderInformations.GetSupplierEmail(); Change 03/06/2020 cause supplier email want to send the order and here its only for informations
            C076 c076 = new C076(e3148, COM.C076.E3155_EM);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;
        }

        public string Add_Retailer_TE()
        {
            string e3148 = _orderInformations.GetRetailerPhone();
            C076 c076 = new C076(e3148, COM.C076.E3155_TE);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;
        }
        public string Add_Retailer_FX()
        {
            string e3148 = _orderInformations.GetRetailerFax();          
            C076 c076 = new C076(e3148,COM.C076.E3155_FX);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;
        }
        public string Add_Retailer_EM()
        {
            string e3148 = _orderInformations.GetRetailerEmail();           
            C076 c076 = new C076(e3148, COM.C076.E3155_EM);

            if (!String.IsNullOrEmpty(e3148))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.COM + Separator.DataGroup + c076.Add() + Separator.EndLine;
            }
            return null;
        }
    }
}
