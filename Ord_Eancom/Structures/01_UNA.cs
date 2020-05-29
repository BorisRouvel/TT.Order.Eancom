
namespace Eancom
{
    public class UNA
    {
        public UNA()
        {            
        }

        public string Add()
        {
            return StructureEDI.UNA + Separator.DataElement + Separator.DataGroup + Separator.DecimalSep + Separator.FreeChar + KD.StringTools.Const.WhiteSpace + Separator.EndLine; ;
        }
    }
}
