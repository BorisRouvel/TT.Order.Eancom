using System;

using Ord_Eancom;

namespace Eancom
{
    public class UNB
    {
        OrderInformations _orderInformations = null;
        FileEDI _fileEDI = null;
        S001 s001 = null;
        S002 s002 = null;
        S003 s003 = null;
        S004 s004 = null;

        private string _e0020;
        public string E0020
        {
            get
            {
                return _e0020;
            }
            set
            {
                _e0020 = value;
            }
        }
        private string _e0026;
        public string E0026
        {
            get
            {
                return _e0026;
            }
            set
            {
                _e0026 = value;
            }
        }
        private string _e0032;
        public string E0032
        {
            get
            {
                return _e0032;
            }
            set
            {
                _e0032 = value;
            }
        }
        private string _e0035;
        public string E0035
        {
            get
            {
                return _e0035;
            }
            set
            {
                _e0035 = value;
            }
        }


        public class S001
        {
            public const string e0001 = "UNOC";
            public const string e0002 = "3";

            public S001()
            {
            }

            public string E0001
            {
                get
                {
                    return e0001;
                }               
            }
            public string E0002
            {
                get
                {
                    return e0002;
                }
            }
            public string Add()
            {
                return (this.E0001 + Separator.DataElement + this.E0002 + Separator.DataGroup);
            }
        }
        public class S002
        {
            private string _e0004 = String.Empty;
            public string E0004
            {
                get
                {
                    return _e0004;
                }
                set
                {
                    _e0004 = value;
                }
            }

            public S002()
            {               
            }

            public string Add()
            {
                return (this.E0004 + Separator.DataGroup);
            }
        }
        public class S003
        {
            private string _e0010 = String.Empty;
            public string E0010
            {
                get
                {
                    return _e0010;
                }
                set
                {
                    _e0010 = value;
                }
            }

            public S003()
            {               
            }

            public string Add()
            {
                return (this.E0010 + Separator.DataGroup);
            }
        }
        public class S004
        {
            private string _e0017 = String.Empty;
            public string E0017
            {
                get
                {
                    return _e0017;
                }
                set
                {
                    _e0017 = value;
                }
            }

            private string _e0019 = String.Empty;
            public string E0019
            {
                get
                {
                    return _e0019;
                }
                set
                {
                    _e0019 = value;
                }
            }

            public S004()
            {               
            }

            public string Add()
            {
                return (this.E0017 + Separator.DataElement + this.E0019 + Separator.DataGroup);
            }
        }   
      
        public UNB(OrderInformations orderInformations, FileEDI fileEDI)
        {
            _orderInformations = orderInformations;
            _fileEDI = fileEDI;
            s001 = new S001();
            s002 = new S002();
            s003 = new S003();
            s004 = new S004();
        }

        public string Add()
        {
            s002.E0004 = _orderInformations.GetRetailerName1();  //GetSupplierName
            s003.E0010 = _fileEDI.ManufacturerGLN();

            if (!_fileEDI.HasManufacturerGLNCode(s003.E0010))
            {
                s003.E0010 = _fileEDI.ManufacturerID();
            }

            DateTime dateTime = DateTime.Now;
            s004.E0017 = dateTime.ToString(OrderConstants.FormatDate_2yMd);
            s004.E0019 = dateTime.ToString(OrderConstants.FormatTime_Hm);
            
            Order.orderFile = _orderInformations.GetReferenceNumberEDIFile(_fileEDI);
            _e0020 = Order.orderFile + Separator.DataGroup + Separator.DataGroup;
            _e0026 = _orderInformations.GetNameAndVersionSoftware() + Separator.DataGroup + Separator.DataGroup + Separator.DataGroup;

            string EDIversion = OrderTransmission.VersionEancomOrder; // _fileEDI.Order_();
            if (String.IsNullOrEmpty(EDIversion))
            {
                EDIversion = _fileEDI.Orders();
            }
            _e0032 = EDIversion + Separator.DataGroup;
            _e0035 = "1";           

            return StructureEDI.UNB + Separator.DataGroup + s001.Add() + s002.Add() + s003.Add() + s004.Add() +
                this.E0020 + this.E0026 + this.E0032 + this.E0035 + Separator.EndLine;
            
        }
    }
}
