using System;
using System.IO;

using KD.SDKComponent;
using KD.Model;
using KD.Analysis;

//using TT.Import.EGI;

namespace Ord_Eancom
{
    public class BuildCommon
    {
        AppliComponent _currentAppli = null;
        AppliComponent CurrentAppli
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

        private double _sceneDimX = 0.0;
        private double _sceneDimY = 0.0;
        private double _sceneDimZ = 0.0;
        private double _angleScene = 0.0;

        public double SceneDimX
        {
            get
            {
                return _sceneDimX;
            }
            set
            {
                _sceneDimX = value;
            }
        }
        public double SceneDimY
        {
            get
            {
                return _sceneDimY;
            }
            set
            {
                _sceneDimY = value;
            }
        }
        public double SceneDimZ
        {
            get
            {
                return _sceneDimZ;
            }
            set
            {
                _sceneDimZ = value;
            }
        }
        public double AngleScene
        {
            get
            {
                return _angleScene;
            }
            set
            {
                _angleScene = value;
            }
        }

        public BuildCommon(AppliComponent currentAppli)
        {
            _currentAppli = currentAppli;
        }

        public void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case ItemValue.V1_50: //DISCAC
                    _sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    _sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    _sceneDimZ = 0;
                    _angleScene = (0 * System.Math.PI) / 180;
                    break;
                case ItemValue.V1_51: //FBD , BAUFORMAT
                    _sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    _sceneDimY = -this.CurrentAppli.SceneDimY / 2;
                    _sceneDimZ = 0;
                    _angleScene = (0 * System.Math.PI) / 180;
                    break;
                //sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                //sceneDimY = this.CurrentAppli.SceneDimY / 2;
                //sceneDimZ = 0;
                //angleScene = (270 * System.Math.PI) / 180;
                default: //V1_51
                    _sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    _sceneDimY = -this.CurrentAppli.SceneDimY / 2;
                    _sceneDimZ = 0;
                    _angleScene = (0 * System.Math.PI) / 180;
                    break;
            }
        }
        public void SetReference()
        {
            this.CurrentAppli.Scene.SceneSetReference((int)_sceneDimX, (int)_sceneDimY, (int)_sceneDimZ, _angleScene);
        }
        public void ResetReference()
        {
            this.CurrentAppli.SceneComponent.ResetSceneReference();
        }

        public KD.SDK.SceneEnum.ViewMode GetView()
        {
            return this.CurrentAppli.Scene.ViewGetMode();
        }
        public bool SetView(KD.SDK.SceneEnum.ViewMode viewMode)
        {
            return this.CurrentAppli.Scene.ViewSetMode(viewMode);
        }
        public bool ZoomAdjusted()
        {
            return this.CurrentAppli.Scene.ZoomAdjusted();
        }
        public bool ExportImageJPG(int count, string exportName)
        {
            return this.CurrentAppli.Scene.FileExportImage(Path.Combine(Order.orderDir, exportName + "-" + count + OrderTransmission.ExtensionJPG), 1200, 1200, "255,255,255", true, 100, 3);
        }

        public string[] GetArticlePolyPoint(Article article)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);
            string shapePointList = String.Empty;
            string newShapePointList = string.Empty;

            this.SetSceneReference(OrderWrite.version);
            shapePointList = segmentClassification.GetShape();

            if (String.IsNullOrEmpty(shapePointList))
            {
                if (segmentClassification.IsArticlePlinth())
                {
                    if (article.HasParent())
                    {
                        SegmentClassification segmentParentClassification = new SegmentClassification(article.Parent);
                        if (segmentParentClassification.IsArticlePlinth())
                        {
                            shapePointList = segmentParentClassification.GetShape();
                        }
                    }
                    else
                    {
                        Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
                        if (childs.Count > 0)
                        {
                            foreach (Article child in childs)
                            {
                                SegmentClassification segmentChildClassification = new SegmentClassification(child);
                                if (segmentChildClassification.IsArticlePlinth())
                                {
                                    shapePointList = segmentChildClassification.GetShape();
                                }
                            }
                        }
                    }

                }
            }

            if (!String.IsNullOrEmpty(shapePointList))
            {
                string[] points = shapePointList.Split(KD.CharTools.Const.SemiColon); // 5 pts                
                foreach (string point in points)
                {                        
                    string[] coords = point.Split(KD.CharTools.Const.Comma);
                    if (coords.Length > 3)
                    {
                        newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[0]) - _sceneDimX) + KD.CharTools.Const.Comma;
                        newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[1]) - _sceneDimY) + KD.CharTools.Const.Comma;
                        newShapePointList += Convert.ToString(KD.StringTools.Convert.ToDouble(coords[2]) - _sceneDimZ) + KD.CharTools.Const.Comma;
                        newShapePointList += coords[3] + KD.CharTools.Const.SemiColon;
                    }
                }
                

                string[] shapeList = newShapePointList.Split(KD.CharTools.Const.SemiColon);
                if (shapeList.Length > 0)
                {
                    //Determine the position for linear because in Insitu is 0,0 and we need the first shape point.
                    if (segmentClassification.IsArticleLinear() || segmentClassification.IsArticleWorkTop())
                    {
                        OrderWrite.posX = KD.StringTools.Convert.ToDouble(shapeList[0].Split(KD.CharTools.Const.Comma)[0]);
                        OrderWrite.posY = KD.StringTools.Convert.ToDouble(shapeList[0].Split(KD.CharTools.Const.Comma)[1]);
                    }

                    return shapeList;
                }
            }
            return null;
        }
        public int GetArticlePolyType(Article article)
        {
            SegmentClassification segmentClassification = new SegmentClassification(article);

            int type = Convert.ToInt16(this.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.TYPE));

            if (type == (int)(KD.SDK.SceneEnum.ObjectType.PLANARTICLE) && segmentClassification.IsArticleWorkTop())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_WorkTop);
                return PolytypeValue.Polytype_WorkTop;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticlePlinth())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_Base);
                return PolytypeValue.Polytype_Base;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticleLightPelmet())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_LightPelmet);
                return PolytypeValue.Polytype_LightPelmet;
            }
            else if ((type == (int)(KD.SDK.SceneEnum.ObjectType.STANDARD) || type == (int)(KD.SDK.SceneEnum.ObjectType.LINEAR)) && segmentClassification.IsArticleCornice())
            {
                PolytypeValue polytypeValue = new PolytypeValue(PolytypeValue.Polytype_Cornice);
                return PolytypeValue.Polytype_Cornice;
            }

            return KD.Const.UnknownId;
        }
        public double GetRealPositionZByPosedOnOrUnder(Article article)
        {
            double positionZ = article.PositionZ;
            if (this.IsPosedUnder(article))
            {
                positionZ = article.PositionZ - article.DimensionZ;
            }
            return positionZ;
        }

        private bool IsPosedOn(Article article)
        {
            if (article.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.ON_OR_UNDER) == "0")
            {
                return true;
            }
            return false;
        }
        private bool IsPosedUnder(Article article)
        {
            if (article.CurrentAppli.Scene.ObjectGetInfo(article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.ON_OR_UNDER) == "1")
            {
                return true;
            }
            return false;
        }
       
    }
}
