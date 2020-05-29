using Ord_Eancom;


namespace Eancom
{
    public class UNZ
    {
        OrderInformations _orderInformations = null;
        FileEDI _fileEDI = null;


        public UNZ(OrderInformations orderInformations, FileEDI fileEDI) //string e0036, string e0020)
        {
            _orderInformations = orderInformations;
            _fileEDI = fileEDI;
        }

        public string Add()
        {            
            string e0036 = "1" + Separator.DataGroup;
            string e0020 = _orderInformations.GetReferenceNumberEDIFile(_fileEDI);

            return StructureEDI.UNZ + Separator.DataGroup + e0036 + e0020 + Separator.EndLine;
        }
    }
}
