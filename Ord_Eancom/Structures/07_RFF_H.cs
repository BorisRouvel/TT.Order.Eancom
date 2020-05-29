using System;
using Ord_Eancom;

namespace Eancom
{ 

    public class RFF_H
    {
        OrderInformations _orderInformations = null;       

        public class C506
        {
            public const string E1153_CR = "CR";
            public const string E1153_UC = "UC";

            private string _e1153;
            public string E1153
            {
                get
                {
                    return _e1153;
                }
                set
                {
                    _e1153 = value;
                }
            }

            private string _e1154;
            public string E1154
            {
                get
                {
                    return _e1154;
                }
                set
                {
                    _e1154 = value;
                }
            }

            public C506(string e1153, string e1154)
            {
                _e1153 = e1153;
                _e1154 = e1154;
            }

            public string Add()
            {
                return this.E1153 + Separator.DataElement + this.E1154;
            }
        }

        
        public RFF_H(OrderInformations orderInformations)
        {
             _orderInformations = orderInformations;
        }

        public string Add_CR()
        {
            C506 c506 = new C506(Eancom.RFF_H.C506.E1153_CR, _orderInformations.GetCommissionNumber());

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.RFF_H + Separator.DataGroup + c506.Add() + Separator.EndLine;           
        }
        public string Add_UC()
        {
            string commissionName = _orderInformations.GetCommissionName();
            C506 c506 = new C506(Eancom.RFF_H.C506.E1153_UC, commissionName);

            if (!String.IsNullOrEmpty(commissionName))
            {
                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return StructureEDI.RFF_H + Separator.DataGroup + c506.Add() + Separator.EndLine;
            }

            return null;                    
        }
    }
}
