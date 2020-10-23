using System;
using Ord_Eancom;

namespace Eancom
{
    public class NAD
    {
        OrderInformations _orderInformations = null;
        FileEDI _fileEDI = null;
        C082 c082 = null;
        C080 c080 = null;
        C059 c059 = null;

        public const string E3035_SU = "SU";
        public const string E3035_BY = "BY";
        public const string E3035_DP = "DP";
        public const string E3035_AB = "AB"; //Not use yet
        public const string E3035_MA = "MA"; //Not use yet

        private string nadBYCompare = String.Empty;
        private string nadDPCompare = String.Empty;

        private string _e3035;
        public string E3035
        {
            get
            {
                return _e3035;
            }
            set
            {
                _e3035 = value;
            }
        }

        private string _e3164;
        public string E3164
        {
            get
            {
                return _e3164;
            }
            set
            {
                _e3164 = value;
            }
        }

        private string _e3251;
        public string E3251
        {
            get
            {
                return _e3251;
            }
            set
            {
                _e3251 = value;
            }
        }

        private string _e3207;
        public string E3207
        {
            get
            {
                return _e3207;
            }
            set
            {
                _e3207 = value;
            }
        }

        public class C082
        {
            // private string _e3039;
            public string E3039;
            //{
            //    get
            //    {
            //        return _e3039;
            //    }
            //    set
            //    {
            //        _e3039 = value;
            //    }
            //}

            public const string E3055_9 = "9";
            public const string E3055_91 = "91";

            private string _e3055;
            public string E3055
            {
                get
                {
                    return _e3055;
                }
                set
                {
                    _e3055 = value;
                }
            }

            public C082()// string e3039, string e3055)
            {
                //_e3039 = e3039;
                //_e3055 = e3055;                               
            }

            public string Add()
            {
                return this.E3039 + Separator.DataElement + Separator.DataElement + this.E3055 + Separator.DataGroup + Separator.DataGroup;
            }
        }
        public class C080
        {
            //private string _e3036_1;
            public string E3036_1;
            //{
            //    get
            //    {
            //        return _e3036_1;
            //    }
            //    set
            //    {
            //        _e3036_1 = value;
            //    }
            //}

            //private string _e3036_2;
            public string E3036_2;
            //{
            //    get
            //    {
            //        return _e3036_2;
            //    }
            //    set
            //    {
            //        _e3036_2 = value;
            //    }
            //}

            public C080()//string e3036_1, string e3036_2)
            {
                //_e3036_1 = e3036_1;
                //_e3036_2 = e3036_2;
            }

            public string Add()
            {
                return this.E3036_1 + Separator.DataGroup + this.E3036_2 + Separator.DataGroup;
            }
        }
        public class C059
        {
            //private string _e3042;
            public string E3042;
            //{
            //    get
            //    {
            //        return _e3042;
            //    }
            //    set
            //    {
            //        _e3042 = value;
            //    }
            //}

            public C059() //string e3042)
            {
              // _e3042 = e3042;
            }

            public string Add()
            {
                return this.E3042 + Separator.DataGroup;
            }
        }
             

        public NAD(OrderInformations orderInformations, FileEDI fileEDI)
        {
            _orderInformations = orderInformations;
            _fileEDI = fileEDI;
            c082 = new C082();
            c080 = new C080();
            c059 = new C059();

        }

        public string Add_SU()
        {            
            _e3035 = Eancom.NAD.E3035_SU;
            _e3164 = _orderInformations.GetSupplierCity();
            _e3251 = _orderInformations.GetSupplierPostCode();
            _e3207 = _orderInformations.GetSupplierCountry();

            ////c082 = new C082(_orderInformations.GetSupplierGLN(), Eancom.NAD.C082.E3055_9);
            //c082.E3039 = _orderInformations.GetSupplierGLN();

            c082.E3039 = String.Empty; // Discac want the manufacturer Id (IDM 1201) instead of _fileEDI.ManufacturerGLN() pos44 first line 100.
            c082.E3055 = Eancom.NAD.C082.E3055_9;
            c080.E3036_1 = _orderInformations.GetSupplierName1();
            c080.E3036_2 = _orderInformations.GetSupplierName2();
            c059.E3042 = _orderInformations.GetSupplierAddress();
           
            if (!_fileEDI.HasManufacturerGLNCode(c082.E3039))
            {
                c082.E3039 = _fileEDI.ManufacturerID();
                c082.E3055 = Eancom.NAD.C082.E3055_91;              
            }

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.NAD + Separator.DataGroup + this.E3035 + Separator.DataGroup + c082.Add() + c080.Add() + c059.Add() +
                this.E3164 + Separator.DataGroup + Separator.DataGroup + this.E3251 + Separator.DataGroup + this.E3207 + Separator.EndLine;
        }
        public string Add_BY(int seg)
        {
            _e3035 = Eancom.NAD.E3035_BY;            
            _e3164 = _orderInformations.GetRetailerCity();
            _e3251 = _orderInformations.GetRetailerPostCode();
            _e3207 = _orderInformations.GetRetailerCountry();

            //c082 = new C082(_orderInformations.GetRetailerGLN(), Eancom.NAD.C082.E3055_9);
            //C080 c080 = new C080(_orderInformations.GetRetailerName1(), String.Empty);
            //C059 c059 = new C059(_orderInformations.GetRetailerAddress());

            c082.E3039 = _orderInformations.GetRetailerGLN();
            c082.E3055 = Eancom.NAD.C082.E3055_9;
            c080.E3036_1 = _orderInformations.GetRetailerName1();
            c080.E3036_2 = _orderInformations.GetRetailerName2();
            c059.E3042 = _orderInformations.GetRetailerAddress();

            if (String.IsNullOrEmpty(c082.E3039))
            {                
                //c082 = new C082(_fileEDI.RetailerNumber(), Eancom.NAD.C082.E3055_91);
                c082.E3039 = _orderInformations.GetRetailerNumber(); //_fileEDI.RetailerNumber();
                c082.E3055 = Eancom.NAD.C082.E3055_91;
            }

            OrderWrite.segmentNumberBetweenUNHandUNT += seg;
            return StructureEDI.NAD + Separator.DataGroup + this.E3035 + Separator.DataGroup + c082.Add() + c080.Add() + c059.Add() +
                this.E3164 + Separator.DataGroup + Separator.DataGroup + this.E3251 + Separator.DataGroup + this.E3207 + Separator.EndLine;
        }
        public string Add_Delivery_DP(int seg)
        {
            _e3035 = Eancom.NAD.E3035_DP;

            if (!String.IsNullOrEmpty(_orderInformations.GetDeliveryName1()))
            {
                _e3164 = _orderInformations.GetDeliveryCity();
                _e3251 = _orderInformations.GetDeliveryPostCode();
                _e3207 = _orderInformations.GetDeliveryCountry();

                //c082 = new C082(_orderInformations.GetRetailerGLN(), Eancom.NAD.C082.E3055_9);
                //C080 c080 = new C080(_orderInformations.GetDeliveryName1(), String.Empty);
                //C059 c059 = new C059(_orderInformations.GetDeliveryAddress());

                c082.E3039 = _orderInformations.GetRetailerGLN();
                c082.E3055 = Eancom.NAD.C082.E3055_9;
                c080.E3036_1 = _orderInformations.GetDeliveryName1();
                c080.E3036_2 = String.Empty;
                c059.E3042 = _orderInformations.GetDeliveryAddress();

                if (String.IsNullOrEmpty(c082.E3039))
                {
                    //c082 = new C082(_fileEDI.RetailerNumber(), Eancom.NAD.C082.E3055_91);
                    c082.E3039 = _orderInformations.GetRetailerNumber(); //_fileEDI.RetailerNumber();
                    c082.E3055 = Eancom.NAD.C082.E3055_91;
                }

                OrderWrite.segmentNumberBetweenUNHandUNT += seg;
                return StructureEDI.NAD + Separator.DataGroup + this.E3035 + Separator.DataGroup + c082.Add() + c080.Add() + c059.Add() +
                    this.E3164 + Separator.DataGroup + Separator.DataGroup + this.E3251 + Separator.DataGroup + this.E3207 + Separator.EndLine;
            }

            return null;
        }
        public string Add_CustomerDelivery_DP()
        {
            _e3035 = Eancom.NAD.E3035_DP;
            string customerDeliveryName1 = _orderInformations.GetCustomerDeliveryName1();
            if (!String.IsNullOrEmpty(customerDeliveryName1))
            {               
                _e3164 = _orderInformations.GetCustomerDeliveryCity();
                _e3251 = _orderInformations.GetCustomerDeliveryPostCode();
                _e3207 = _orderInformations.GetCustomerDeliveryCountry();

                //C082 c082 = new C082(String.Empty, String.Empty);
                //C080 c080 = new C080(customerDeliveryName1, String.Empty);
                //C059 c059 = new C059(_orderInformations.GetCustomerDeliveryAddress());

                c082.E3039 = String.Empty;
                c082.E3055 = String.Empty;
                c080.E3036_1 = customerDeliveryName1;
                c080.E3036_2 = String.Empty;
                c059.E3042 = _orderInformations.GetCustomerDeliveryAddress();

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.NAD + Separator.DataGroup + this.E3035 + Separator.DataGroup + c082.Add() + c080.Add() + c059.Add() +
                    this.E3164 + Separator.DataGroup + Separator.DataGroup + this.E3251 + Separator.DataGroup + this.E3207 + Separator.EndLine;
            }

            return null;
        }

        //If delivery adress is different of buyer adress
        //Be careful to don't add a segement line number (0)
        public bool IsBYequalDDP() 
        {
            nadBYCompare = this.Add_BY(0).Replace(StructureEDI.NAD + Separator.DataGroup + Eancom.NAD.E3035_BY + Separator.DataGroup, String.Empty);
            nadDPCompare = this.Add_Delivery_DP(0).Replace(StructureEDI.NAD + Separator.DataGroup + Eancom.NAD.E3035_DP + Separator.DataGroup, String.Empty);

            if (nadBYCompare.Equals(nadDPCompare))
            {
                return true;
            }
            return false;
        }
    }
}
