using KD.Model;

using Ord_Eancom;

namespace Eancom
{
    public class QTY
    {
        C186 c186 = null;
        
        public class C186
        {
            Utility utility = null;

            private string _e6060;
            public string E6060
            {
                get
                {
                    return _e6060;
                }
                set
                {
                    value = _e6060;
                }
            }

            public const string E6063 = "21";
            public const string E6411 = "PCE";

            public C186()
            {
                utility = new Utility();
            }

            public string Add(Article article)
            {
                _e6060 = article.Quantity.ToString();

                Article parent = article.Parent;
                if (parent != null && parent.IsValid)
                {
                    if (utility.IsLinearPlanType(parent.Type))
                    {
                        _e6060 = parent.Quantity.ToString();
                    }
                }
                if (utility.IsLinearPlanType(article.Type))
                {
                    _e6060 = utility.GetQuantityByOccurrence(article.CurrentAppli, article.ObjectId);
                }
                
                return E6063 + Separator.DataElement + this.E6060 + Separator.DataElement + E6411;
            }
        }

        public QTY()
        {
            c186 = new C186();            
        }

        public string Add(Article article)
        {
            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.QTY + Separator.DataGroup + c186.Add(article) + Separator.EndLine;
        }

        
        public string Add_WorktopAssemblyNumber(Article article)
        {
            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.QTY + Separator.DataGroup + c186.Add(article) + Separator.EndLine;
        }
    }
}
