using System;
using System.Collections.Generic;

using KD.Model;
using KD.Analysis;

namespace TT.Import.EGI
{
    public class ArticleSegment
    {
        KD.SDKComponent.AppliComponent _currentAppli = null;
        private Article _article = null;
        private Segment _segment = null;
        private string _catalogManufacturer = String.Empty;

        Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>();

        KD.SDKComponent.AppliComponent CurrentAppli
        {
            get
            {
                return _currentAppli;
            }
            set
            {
                _currentAppli = value;
            }
        }
        public Article Article
        {
            get
            {
                return _article;
            }
            set
            {
                _article = value;
            }
        }
        public Segment Segment
        {
            get
            {
                return _segment;
            }
            set
            {
                _segment = value;
            }
        }
        public string CatalogManufacturer
        {
            get
            {
                return _catalogManufacturer;
            }
            set
            {
                _catalogManufacturer = value;
            }
        }

        public ArticleSegment(KD.SDKComponent.AppliComponent currentAppli, Segment segment, string catalogManufacturer)
        {
            _currentAppli = currentAppli;
            _segment = segment;
            _catalogManufacturer = catalogManufacturer;
        }

        private void SetReference()
        {
            this.CurrentAppli.Scene.SceneSetReference((int)Plugin.sceneDimX, (int)Plugin.sceneDimY, (int)Plugin.sceneDimZ, Plugin.angleScene);
        }
        private void ResetReference()
        {
            this.CurrentAppli.SceneComponent.ResetSceneReference();
        }

        public void PlaceObject()
        {
            this.SetReference();

            int objectID = this.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.Segment.Reference, this.Segment.HingeType, (int)this.Segment.DimensionX, 
                (int)this.Segment.DimensionY, (int)this.Segment.DimensionZ, (int)this.Segment.PositionX, (int)this.Segment.PositionY, (int)this.Segment.PositionZ, 
                0, this.Segment.AngleZ, false, false, false);

            if (objectID.Equals(KD.Const.UnknownId))
            {
                this.SetReference();

                objectID = this.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.Segment.Reference, 0, (int)this.Segment.DimensionX,
                (int)this.Segment.DimensionY, (int)this.Segment.DimensionZ, (int)this.Segment.PositionX, (int)this.Segment.PositionY, (int)this.Segment.PositionZ,
                0, this.Segment.AngleZ, false, false, false);
            }

            if (!objectID.Equals(KD.Const.UnknownId))
            {
                _article = new Article(this.CurrentAppli, objectID);
            }
            
        }
        public Article PlaceComponentObject()
        {
            if (this.Segment.RefPos.Contains(KD.StringTools.Const.Dot))
            {
                string refBase = this.Segment.RefPos.Split(KD.CharTools.Const.Dot)[0];

                foreach (KeyValuePair<int, string> kvp in articleAlreadyPlacedDict)
                {
                    if (kvp.Value.Equals(refBase))
                    {
                        Article parent = new Article(this.CurrentAppli, kvp.Key);
                        Articles childs = parent.GetChildren(FilterArticle.strFilterToGetValidNotPlacedHostedAndChildren());
                        foreach (Article child in childs)
                        {
                            if (child.IsValid && child.Ref.Equals(this.Segment.Reference))
                            {
                                bool find = false;  //Test with Bauformat Side panel maybe don't work for another
                                if (this.Segment.PositionX == 0.0 && child.Handing == this.CurrentAppli.GetTranslatedText("G"))
                                {
                                    find = true;
                                }
                                else if (this.Segment.PositionX > 0.0 && child.Handing == this.CurrentAppli.GetTranslatedText("D"))
                                {
                                    find = true;
                                }
                                else
                                {
                                    find = true;
                                }

                                if (find)
                                {
                                    child.IsPlaced = true;
                                    //child.DimensionX = w1;
                                    //child.DimensionY = d1;
                                    child.DimensionZ = this.Segment.DimensionZ;
                                    //child.PositionX = x1;
                                    //child.PositionY = y1;
                                    child.PositionZ = this.Segment.PositionZ;
                                    return child;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public void MoveArticlePerRepere()
        {
            this.CurrentAppli.SceneComponent.SetReferenceFromObject(this.Article, false);
            switch (this.Segment.GetVersion())
            {
                case Segment.V1_50: //DISCAC
                    this.Article.PositionX += this.Segment.DimensionX; //w1
                    this.Article.AngleOXY += 180;
                    break;
                case Segment.V1_51: //FBD et Bauformat
                    this.Article.PositionX += this.Segment.DimensionX; // w1;
                    this.Article.AngleOXY += 180;
                    break;
                default:
                    this.Article.PositionX += this.Segment.DimensionX; // w1;
                    this.Article.AngleOXY += 180;
                    break;
            }
        }
        public void SetAngleChildDimensions()
        {
            this.Article.DimensionY = this.Segment.DimensionY1; // dB1;

            Articles childs = this.Article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    if (child.Name.ToUpper().Contains(this.CurrentAppli.GetTranslatedText("Fileur".ToUpper()))) //Name.StartsWith("@F")) 
                    {
                        child.DimensionX = this.Segment.DimensionY - this.Segment.DimensionY1; // d1 - dB1; //angleFilerWidth;
                        child.DimensionY = this.Segment.DimensionX1; // wB1; // angleFilerDepth;                     
                    }
                }
            }
        }
        public void SetCornerDimensions()
        {
            this.Article.DimensionX = this.Segment.DimensionX - this.Segment.DimensionX1; // w1 - wB1; //angleFilerWidth;
            this.Article.DimensionY = this.Segment.DimensionY - this.Segment.DimensionY1; // d1 - dB1; // angleFilerDepth;            
        }
        public void SetAnglePositionAngle()
        {
            List<string> firstBraceParameters = KD.StringTools.Helper.ExtractParameters(this.Article.Script, String.Empty, KD.StringTools.Const.BraceOpen, KD.StringTools.Const.BraceClose);

            if (this.Article.Handing == this.CurrentAppli.GetTranslatedText("G") && firstBraceParameters.Contains("SI".ToUpper()))
            {
                this.Article.AngleOXY += 90.0;
                this.SetReference();
                this.Article.PositionY += this.Segment.DimensionX; // w1;
            }
        }

        public void AddPlacedArticleDict(Article child, string refPos)
        {
            if (child != null && child.IsValid && child.Number != KD.Const.UnknownId)
            {
                articleAlreadyPlacedDict.Add(child.ObjectId, refPos);
            }
            else if (child == null)
            {
                articleAlreadyPlacedDict.Add(Convert.ToInt32(refPos), refPos);
            }
        }
    }
}
