using System;

using Eancom;

namespace Ord_Eancom
{
    public class DTM
    {
        OrderInformations _orderInformations = null;       

        private string _e2005;
        public string E2005
        {
            get
            {
                return _e2005;
            }
            set
            {
                _e2005 = value;
            }
        }
        private string _e2380;
        public string E2380
        {
            get
            {
                return _e2380;
            }
            set
            {
                _e2380 = value;
            }
        }
        private string _e2379;
        public string E2379
        {
            get
            {
                return _e2379;
            }
            set
            {
                _e2379 = value;
            }
        }

        C507 c507 = null;

        public class C507
        {
            public const string E2005_137 = "137";
            public const string E2005_2 = "2";
            public const string E2005_63 = "63";
            public const string E2005_64 = "64";
            public const string E2005_18 = "18";

            public const string E2379_102 = "102";
            public const string E2379_616 = "616";            

            public C507()
            {
            }

            public string Add(string e2005, string e2380, string e2379)
            {                        
                return e2005 + Separator.DataElement + e2380 + Separator.DataElement + e2379;
            }
        }

        public DTM(OrderInformations orderInformations)
        {
            _orderInformations = orderInformations;
            c507 = new C507();
        }

        public string Add()
        {            
            DateTime dateTime = DateTime.Now;
            _e2005 = DTM.C507.E2005_137;
            _e2380 = dateTime.ToString(OrderConstants.FormatDate_4yMd);
            _e2379 = DTM.C507.E2379_102;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.DTM + Separator.DataGroup + c507.Add(this.E2005, this.E2380, this.E2379) + Separator.EndLine;         
        }
        public string Add_Delivery()
        {
            _e2005 = _orderInformations.GetDeliveryDateType();
            _e2379 = _orderInformations.GetDateFormat(OrderInformations.deliveryDate);
            if (!String.IsNullOrEmpty(this.E2005) && !String.IsNullOrEmpty(this.E2379))
            {               
                _e2380 = OrderInformations.deliveryDate;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.DTM + Separator.DataGroup + c507.Add(this.E2005, this.E2380, this.E2379) + Separator.EndLine;
            }

            return null;
        }
        public string Add_Installation()
        {
            _e2005 = _orderInformations.GetInstallationDateType();
            _e2379 = _orderInformations.GetDateFormat(OrderInformations.installationDate);
            if (!String.IsNullOrEmpty(this.E2005) && !String.IsNullOrEmpty(this.E2379))
            {
                _e2380 = OrderInformations.installationDate;

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.DTM + Separator.DataGroup + c507.Add(this.E2005, this.E2380, this.E2379) + Separator.EndLine;
            }

            return null;
        }
    }
}
