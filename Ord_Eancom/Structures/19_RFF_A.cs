using System;
using System.Collections.Generic;

using KD.Model;

using Ord_Eancom;
using TT.Import.EGI;

namespace Eancom
{
    public class RFF_A
    {
        C506 c506 = null;
        Utility utility = null;
        OrderInformations _orderInformationsFromArticles = null;

        List<string> childLevelList = new List<string>() { };
        public static List<string> refPosList = new List<string>(0);
        private const string BlockSetNumber_LI = "0.1";
        private const string BlockSetNumber_ON = "1";

        public class C506
        {
            public const string E1153_LI = "LI";
            public const string E1153_ON = "ON";
            public const string E1153_ACD = "ACD";

            public string E1153 = String.Empty;
            public string E1154 = String.Empty;           

            public C506()
            {
            }

            public string Add()
            {
                return this.E1153 + Separator.DataElement + this.E1154;
            }
        }


        public RFF_A(OrderInformations orderInformationsFromArticles)
        {
            _orderInformationsFromArticles = orderInformationsFromArticles;
            c506 = new C506();
            utility = new Utility();
           
        }

        private string BuildLine()
        {
            return StructureEDI.RFF_A + Separator.DataGroup + c506.Add() + Separator.EndLine;
        }

        public string Add_ReferenceNumber(Article article)
        {
            c506.E1153 = C506.E1153_LI;

            if (utility.HasBlockSet(article))
            {
                c506.E1154 = RFF_A.BlockSetNumber_LI;
            }
            else
            {
                c506.E1154 = article.ObjectId.ToString();
            }          
            
            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return this.BuildLine();
        }
        public string Add_PlanningSystemItemNumber(Article article)
        {
            if (!String.IsNullOrEmpty(article.Number.ToString())) // && article.Number != KD.Const.UnknownId)               
            {
                OrderInformations articleInformations = new OrderInformations(article);

                c506.E1153 = C506.E1153_ON;
                if (utility.HasBlockSet(article))
                {
                    c506.E1154 = RFF_A.BlockSetNumber_ON;
                }
                else
                {
                    int itemLevel = articleInformations.GetComponentLevel();// article);
                    int childLevel = 1;

                    if (_orderInformationsFromArticles.IsParent(itemLevel))
                    {
                        c506.E1154 = this.SetLineNumber(article.Number.ToString(), KD.StringTools.Const.Zero);
                    }
                    else
                    {
                        //case of a filer is a component of a Coin, set the number yet
                        if (article.Name.ToUpper().Contains(article.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())))
                        {
                            c506.E1154 = this.SetLineNumber(article.Number.ToString(), KD.StringTools.Const.Zero);
                        }
                        else
                        {
                            Article parent = this.GetParent(article);
                            string parentChildLevel = this.GetChildLevel(parent, childLevel);

                            childLevelList.Add(parentChildLevel);
                            c506.E1154 = parentChildLevel;
                        }
                    }
                }

                if (c506.E1154.StartsWith(KD.Const.UnknownId.ToString()))
                {
                    return null;
                }

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                //créer une liste pour EGI REF_POS
                refPosList.Add(article.ObjectId.ToString() + KD.StringTools.Const.SemiColon + c506.E1154);
                return this.BuildLine();
            }
            return null;
        }

        public string Add_WorktopAssemblyReferenceNumber(Article article)
        {
            c506.E1153 = C506.E1153_LI;
            int childLevel = 1;
            string levelID = this.GetChildLevel(article, childLevel);
            c506.E1154 = article.ObjectId.ToString() + levelID.Replace(KD.StringTools.Const.Dot, String.Empty); // "REF_UNIQUE";//must find a solution like objectID

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return this.BuildLine();
        }
        public string Add_WorktopAssemblyPlanningSystemItemNumber(Article article, string assemblyReference)
        {
            if (!String.IsNullOrEmpty(assemblyReference))
            {
                c506.E1153 = C506.E1153_ON;              
                int childLevel = 1;
               
                Article parent = this.GetParent(article);
                string parentChildLevel = this.GetChildLevel(parent, childLevel);

                childLevelList.Add(parentChildLevel);
                c506.E1154 = parentChildLevel;

                if (c506.E1154.StartsWith(KD.Const.UnknownId.ToString()))
                {
                    return null;
                }

                OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                return this.BuildLine();
            }           
            return null;
        }

        public string Add_LinearPlanningSystemItemNumber(Article article)
        {
            //if (article.IsValid)
            //{
            //    c506.E1153 = C506.E1153_ON;
            //    int childLevel = 1;

            //    Article parent = this.GetParent(article);
            //    string parentChildLevel = this.GetChildLevel(parent, childLevel);

            //    childLevelList.Add(parentChildLevel);
            //    c506.E1154 = parentChildLevel;

            //    if (c506.E1154.StartsWith(KD.Const.UnknownId.ToString()))
            //    {
            //        return null;
            //    }

            //    OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            //    return this.BuildLine();
            //}
            return null;
        }

        private string SetLineNumber(string number, string level)
        {
            return number + KD.StringTools.Const.Dot + level;
        }
        private Article GetParent(Article article)
        {
            OrderInformations articleInformations = new OrderInformations(article);

            int itemParentLevel = articleInformations.GetComponentLevel();// article);
            if (_orderInformationsFromArticles.IsParent(itemParentLevel))
            {
                itemParentLevel = articleInformations.GetComponentLevel();// article);
                return article;
            }            

            Article parent = article.Parent;

            if (parent != null && parent.IsValid)
            {
                OrderInformations parentInformations = new OrderInformations(parent);

                itemParentLevel = parentInformations.GetComponentLevel();// parent);
                while (!_orderInformationsFromArticles.IsParent(itemParentLevel))
                {
                    parent = parent.Parent;
                    parentInformations = new OrderInformations(parent);
                    itemParentLevel = parentInformations.GetComponentLevel();// parent);
                }
                return parent;
            }
            return article;
        }
        private string GetChildLevel(Article article, int level)
        {
            string childLevel = this.SetLineNumber(article.Number.ToString(), level.ToString());
            while (childLevelList.Contains(childLevel))
            {
                level += 1;
                childLevel = this.SetLineNumber(article.Number.ToString(), level.ToString());
            }
            return childLevel;
        }
       
    }
}
