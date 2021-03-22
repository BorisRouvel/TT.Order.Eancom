using System;
using System.Collections.Generic;

using KD.Model;
using KD.Analysis;

using Ord_Eancom;

namespace TT.Import.EGI
{ 
    public class ArticleSegment
    {
        private Article _article = null;
        //private string _catalogManufacturer = String.Empty;

        private double _refPntX = 0.0;
        private double _refPntY = 0.0;
        private double _refPntZ = 0.0;
        private double _measure_B = 0.0;
        private double _measure_B1 = 0.0;
        private double _measure_B2 = 0.0;
        private double _measure_B3 = 0.0;
        private double _measure_BE = 0.0;
        private double _measure_T = 0.0;
        private double _measure_T1 = 0.0;
        private double _measure_TE = 0.0;
        private double _measure_H = 0.0;
        private double _angleZ = 0.0;

        private double _dimX = 0.0;
        private double _dimY = 0.0;
        private double _dimZ = 0.0;
        private double _angZ = 0.0;

        private string _manufacturer = String.Empty;
        private string _name = String.Empty;
        private string _commentName = String.Empty;
        private string _constructionType = String.Empty;
        private string _hinge = String.Empty;
        private int _hingeType = 0;
        private string _refNo = String.Empty;
        private string _refPos = String.Empty;
        private string _shape = String.Empty;
        private string _polyType = String.Empty;
        private string _polyCounter = String.Empty;

        private Plugin _plugin = null;        
        private readonly string _section = String.Empty;
        private KD.Config.IniFile _currentFileEGI = null;
        private ManageCatalog _manageCatalog = null;
        private PolytypeValue polytypeValue = null;

        //private List<String> notPlacedArticleList = new List<string>(0);
        //private Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>(0);

        private const string RefPointFormat = SegmentFormat.FourZero;       

        public double RefPntX
        {
            get
            {
                return _refPntX;
            }
            set
            {
                _refPntX = value;
            }
        }
        public double RefPntY
        {
            get
            {
                return _refPntY;
            }
            set
            {
                _refPntY = value;
            }
        }
        public double RefPntZ
        {
            get
            {
                return _refPntZ;
            }
            set
            {
                _refPntZ = value;
            }
        }
        public double Measure_B
        {
            get
            {
                return _measure_B;
            }
            set
            {
                _measure_B = value;
            }
        }
        public double Measure_B1
        {
            get
            {
                return _measure_B1;
            }
            set
            {
                _measure_B1 = value;
            }
        }
        public double Measure_B2
        {
            get
            {
                return _measure_B2;
            }
            set
            {
                _measure_B2 = value;
            }
        }
        public double Measure_B3
        {
            get
            {
                return _measure_B3;
            }
            set
            {
                _measure_B3 = value;
            }
        }
        public double Measure_BE
        {
            get
            {
                return _measure_BE;
            }
            set
            {
                _measure_BE = value;
            }
        }
        public double Measure_T
        {
            get
            {
                return _measure_T;
            }
            set
            {
                _measure_T = value;
            }
        }
        public double Measure_T1
        {
            get
            {
                return _measure_T1;
            }
            set
            {
                _measure_T1 = value;
            }
        }
        public double Measure_TE
        {
            get
            {
                return _measure_TE;
            }
            set
            {
                _measure_TE = value;
            }
        }
        public double Measure_H
        {
            get
            {
                return _measure_H;
            }
            set
            {
                _measure_H = value;
            }
        }
        public double AngleZ
        {
            get
            {
                return _angleZ;
            }
            set
            {
                _angleZ = value;
            }
        }

        public double DimX
        {
            get
            {
                return _dimX;
            }
            set
            {
                _dimX = value;
            }
        }
        public double DimY
        {
            get
            {
                return _dimY;
            }
            set
            {
                _dimY = value;
            }
        }
        public double DimZ
        {
            get
            {
                return _dimZ;
            }
            set
            {
                _dimZ = value;
            }
        }
        public double AngZ
        {
            get
            {
                return _angZ;
            }
            set
            {
                _angZ = value;
            }
        }

        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }
            set
            {
                _manufacturer = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string CommentName
        {
            get
            {
                return _commentName;
            }
            set
            {
                _commentName = value;
            }
        }
        public string ConstructionType
        {
            get
            {
                return _constructionType;
            }
            set
            {
                _constructionType = value;
            }
        }
        public string Hinge
        {
            get
            {
                return _hinge;
            }
            set
            {
                _hinge = value;
            }
        }
        public int HingeType
        {
            get
            {
                return _hingeType;
            }
            set
            {
                _hingeType = value;
            }
        }
        public string RefNo
        {
            get
            {
                return _refNo;
            }
            set
            {
                _refNo = value;
            }
        }
        public string RefPos
        {
            get
            {
                return _refPos;
            }
            set
            {
                _refPos = value;
            }
        }
        public string Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                _shape = value;
            }
        }
        public string PolyType
        {
            get
            {
                return _polyType;
            }
            set
            {
                _polyType = value;
            }
        }
        public string PolyCounter
        {
            get
            {
                return _polyCounter;
            }
            set
            {
                _polyCounter = value;
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
        public string CatalogManufacturer = String.Empty;
       

        public ArticleSegment(Plugin plugin, KD.Config.IniFile fileEGI, string section, ManageCatalog manageCatalog)//, string catalogManufacturer
        {
            _plugin = plugin;            
            _currentFileEGI = fileEGI;            
            //_catalogManufacturer = catalogManufacturer;
            _section = section;
            _manageCatalog = manageCatalog;

            this.InitMembers();
            this.SetMembers();
        }

        private void InitMembers()
        {
            _refPntX = 0.0;
            _refPntY = 0.0;
            _refPntZ = 0.0;
            _measure_B = 0.0;
            _measure_B1 = 0.0;
            _measure_B2 = 0.0;
            _measure_B3 = 0.0;
            _measure_T = 0.0;
            _measure_T1 = 0.0;
            _measure_H = 0.0;
            _angleZ = 0;

            _manufacturer = String.Empty;
            _name = String.Empty;
            _constructionType = String.Empty;
            _hinge = KD.StringTools.Const.Zero;
            _hingeType = 0;
            _refNo = String.Empty;
            _refPos = String.Empty;
            _shape = String.Empty;
            _polyType = KD.StringTools.Const.Zero;
            _polyCounter = KD.StringTools.Const.Zero;
        }
        private void SetMembers()
        {
            _manufacturer = this._currentFileEGI.GetStringValue(_section, ItemKey.Manufacturer);
            _name = this._currentFileEGI.GetStringValue(_section, ItemKey.Name);
            _commentName = this._currentFileEGI.GetStringValue(_section, SegmentFormat.CommentChar + ItemKey.Name);
            _constructionType = this._currentFileEGI.GetStringValue(_section, ItemKey.ConstructionType);
            _hinge = this._currentFileEGI.GetStringValue(_section, ItemKey.Hinge);

            if (String.IsNullOrEmpty(this.Hinge))
            {
                _hinge = _constructionType;
            }
            switch (this.Hinge)
            {
                case ItemValue.Left_Hinge:
                    _hingeType = 1;
                    break;
                case ItemValue.Right_Hinge:
                    _hingeType = 2;
                    break;
                default:
                    _hingeType = 0;
                    break;
            }
            
            _refNo = this._currentFileEGI.GetStringValue(_section, ItemKey.RefNo);
            _refPos = this._currentFileEGI.GetStringValue(_section, ItemKey.RefPos);

            _refPntX = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.RefPntX));
            _refPntY = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.RefPntY));
            _refPntZ = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.RefPntZ));

            _measure_B = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B));
            _dimX = _measure_B;
            _measure_T = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_T));// + ArticleSegment.FrontDepth;
            _dimY = _measure_T;
            _measure_H = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_H));
            _dimZ = _measure_H;

            _angleZ = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.AngleZ));
            _angZ = _angleZ;

            _measure_B1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B1));
            _measure_B2 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B2));
            _measure_B3 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B3));
            _measure_T1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_T1));// + ArticleSegment.FrontDepth;
            _measure_BE = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_BE));
            _measure_TE = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_TE));

            _polyType = _currentFileEGI.GetStringValue(_section, ItemKey.PolyType);
            _polyCounter = _currentFileEGI.GetStringValue(_section, ItemKey.PolyCounter);

        }

        // i must organise SHAPE when 20 = filer, 27 = angle (file IDM17_sous-version2_Partie3_fr_nouvelleversion)
        public void Add(string section, List<string> catalogsList)
        {
            _manageCatalog.SetLastManufacturerCatalog(catalogsList);

            if (!String.IsNullOrEmpty(_manageCatalog.CatalogManufacturer))
            {
                Article component = this.PlaceComponentObject();
                if (component != null && component.IsValid)
                {
                    this.AddPlacedArticleDict(component, this.RefPos);
                    return;
                }
                else
                {
                    this.CatalogManufacturer = _manageCatalog.CatalogManufacturer;                    
                    _article = this.PlaceObject();

                    if (this.Article != null && this.Article.IsValid)
                    {
                        SegmentClassification segmentClassification = new SegmentClassification(this.Article, _section, _currentFileEGI);
                        if (segmentClassification.IsSectionCorner() && segmentClassification.IsArticleFiler()) //shape 20 and filer
                        {
                            if (segmentClassification.HasSectionCoin())
                            {
                                _name += CatalogBlockName.Coin;
                                _commentName += CatalogBlockName.Coin;
                                _article = this.ReplaceObject();
                                if (this.Article != null)
                                { 
                                this.ChangeFilerDimensions();
                                this.ChangeFilerChildDimensions();
                                }
                            }
                            else
                            {
                                this.ChangeFilerPositionsAndDimensions();
                            }                           
                        }
                        else
                        {
                            this.MoveArticlePerRepere();
                            this.MoveArticlePositionZPlinth();

                            if (segmentClassification.IsArticleUnit())
                            {
                                this.ChangeCarcasseDimY();
                            }
                        }                        
                       
                        if (segmentClassification.IsSectionAngleWithFiler()) //shape 27 angle
                        {
                            this.SetAngleDimensionsFromEGI();
                            this.SetAngleChildDimensions();
                            this.SetAnglePositionAngle();
                        }

                        this.SetArticleNumber();
                        this.AddPlacedArticleDict(this.Article, this.RefPos);
                    }
                    else
                    {
                        if (catalogsList.Count > 1)
                        {
                            catalogsList.Remove(_manageCatalog.CatalogManufacturer);
                            this.Add(section, catalogsList);
                        }
                        else
                        {
                            Plugin.notPlacedArticleList.Add(this.Name);
                        }
                    }
                }
            }
        } 
        public void AddLinear(string section, List<string> catalogsList)
        {
            string polyPoints = String.Empty;
            string typePoint = "0;";
            
            string polyType = this.GetPolyTypeFromEGI(section);
            int linearType = this.GetLinearType(polyType);            
            int.TryParse(this.PolyCounter, out int valueCounter);

            if (valueCounter != 0)
            {
                string polyFirstPoints = String.Empty;
                for (int count = 1; count <= valueCounter; count++)
                {
                    string polyPntX = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntX + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat));
                    string polyPntY = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntY + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat));
                    string polyPntZ = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntZ + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat));

                    if (linearType.Equals(PolytypeValue.Polytype_WorkTop) || linearType.Equals(PolytypeValue.Polytype_Base))
                    {                       
                        polyPntX = (KD.StringTools.Convert.ToDouble(polyPntX) + Plugin.sceneDimX).ToString();
                        polyPntY = (KD.StringTools.Convert.ToDouble(polyPntY) + Plugin.sceneDimY).ToString();
                    }

                    polyPntX += KD.StringTools.Const.Comma;
                    polyPntY += KD.StringTools.Const.Comma;
                    polyPntZ += KD.StringTools.Const.Comma;

                    polyPoints += polyPntX + polyPntY + polyPntZ + typePoint;

                    // For the closed linear like worktop, moulded floor and hindrance, stock the first point
                    if (count == 1)
                    {
                        polyFirstPoints = polyPoints;
                    }
                }
                // For the closed linear like worktop, moulded floor and hindrance, add first point
                if (polytypeValue.Polyline.Equals(PolytypeValue.Polyline_Closed))
                {
                    polyPoints += polyFirstPoints;
                }

                polyPoints = polyPoints.Substring(0, polyPoints.Length - 1);               
                _plugin.CurrentAppli.Scene.SceneAddShape(polyPoints);
                bool bshape = _plugin.CurrentAppli.Scene.SceneSetActiveShapeRank(0);

                bool isPlace = false;
                string linearReferences = String.Empty;

                _manageCatalog.SetLastManufacturerCatalog(catalogsList);

                if (!String.IsNullOrEmpty(_manageCatalog.CatalogManufacturer))
                {
                    string chapterList = _plugin.CurrentAppli.CatalogGetSectionsList(_manageCatalog.CatalogManufacturer, false, "@r" + KD.CharTools.Const.Comma);
                    int valuechapter = chapterList.Split(KD.CharTools.Const.Comma).Length;
                    string blockNb = _plugin.CurrentAppli.CatalogGetBlocksList(_manageCatalog.CatalogManufacturer, valuechapter, false, "@r" + KD.CharTools.Const.Comma);

                    if (blockNb.EndsWith(KD.StringTools.Const.Comma))
                    {
                        blockNb = blockNb.Remove(blockNb.Length - 1, 1);
                    }
                    string[] blockSplit = blockNb.Split(KD.CharTools.Const.Comma);
                    string blockList = blockSplit[blockSplit.Length - 1];
                    int.TryParse(blockList, out int valueBlockList);
                    for (int block = 0; block < valueBlockList; block++)
                    {
                        string referencesList = _plugin.CurrentAppli.CatalogGetArticlesList(_manageCatalog.CatalogManufacturer, block, false, "@n" + KD.CharTools.Const.SemiColon);
                        linearReferences += referencesList;
                    }

                    string[] linearReferenceList = linearReferences.Split(KD.CharTools.Const.SemiColon);
                    if (linearReferenceList.Length > 0)
                    {
                        string linearRef = String.Empty;
                        foreach (string linearReference in linearReferenceList)
                        {
                            if (linearReference.StartsWith(this.Name))
                            {
                                if (linearType.Equals(PolytypeValue.Polytype_WorkTop) || linearType.Equals(PolytypeValue.Polytype_Base))
                                {
                                    double positionX = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.RefPntX));
                                    double positionY = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.RefPntY));
                                    double positionZ = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.RefPntZ));
                                    double dimensionX = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.Measure_B));
                                    double dimensionY = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.Measure_T));
                                    double dimensionZ = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.Measure_H));
                                    double angleZ = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(section, ItemKey.AngleZ));
                                    //For plinth the position is under the value
                                    if (linearType.Equals(PolytypeValue.Polytype_Base))
                                    {
                                        positionZ -= dimensionZ;
                                    }
                                    // perhaps put the "_GAB" reference but there is not all references in catalog, often the most width AP10K120_GAB                                   
                                    int linearId = _plugin.CurrentAppli.Scene.EditPlaceObject(_manageCatalog.CatalogManufacturer, linearReference, 0, (int)dimensionX, (int)dimensionY, (int)dimensionZ, (int)positionX, (int)positionY, (int)positionZ, 0, angleZ, false, false, false);                                    

                                    if (!linearId.Equals(KD.Const.UnknownId))
                                    {
                                        _plugin.CurrentAppli.Scene.ObjectSetShape(linearId, polyPoints);
                                    }                                   
                                }
                                else
                                {
                                    _plugin.SetReference();
                                    isPlace = _plugin.CurrentAppli.Scene.EditPlaceLinearObject(_manageCatalog.CatalogManufacturer, linearReference, KD.StringTools.Const.Zero, true);                                    
                                }
                                linearRef = linearReference;
                                isPlace = true;
                                break;
                            }
                        }
                        if (!isPlace)
                        {
                            if (catalogsList.Count > 1)
                            {
                                catalogsList.Remove(_manageCatalog.CatalogManufacturer);
                                this.AddLinear(section, catalogsList);
                            }
                            else
                            {
                                Plugin.notPlacedArticleList.Add(this.Name);
                            }
                        }
                        else
                        {
                            _article = _plugin.CurrentAppli.ActiveArticle;
                            this.MoveArticlePositionZPlinth();
                            //KD.FilterBuilder.FilterClauseDict filterBuilder = new KD.FilterBuilder.FilterClauseDict();
                            //filterBuilder.Clear();
                            //filterBuilder.Add(KD.SDK.SceneEnum.ObjectInfo.REF, linearRef);

                            //Articles linearArticles = this.CurrentAppli.GetArticleList(filterBuilder);
                            //if (linearArticles[0].ObjectId != KD.Const.UnknownId)
                            //{
                            //    this.AddPlacedArticleDict(linearArticles[0]);
                            //}
                        }
                    }                   
                }

                _plugin.CurrentAppli.Scene.SceneDeleteAllShapes();
            }            
        }

        private int GetLinearType(string type)
        {
            polytypeValue = new PolytypeValue(Convert.ToInt16(type));
            return polytypeValue.Number();

            #region // First edit
            //switch (type)
            //{
            //    case "1": //Plinthe
            //        return 0;
            //    case "2": //WorkTop
            //        return 1;
            //    case "3": //Cache lumière
            //        return 2;
            //    case "4": //Corniche
            //        return 3;
            //    case "5": //Edge workTop
            //        return 4;
            //    default:
            //        return -1;
            //}
            #endregion
        }
        private string GetPolyTypeFromEGI(string section)
        {
            return _currentFileEGI.GetStringValue(section, ItemKey.PolyType);
        }

        private Article PlaceObject()
        {

            _plugin.SetReference();

            //The 20/01/2021 i change the reference to place the article with CommentName instead of Name cause the article with underscore doesnt work
            if (!String.IsNullOrEmpty(_commentName))
            {
                int objectID = _plugin.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.CommentName, this.HingeType, (int)this.Measure_B,
                    (int)this.Measure_T, (int)this.Measure_H, (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ,
                    0, this.AngleZ, false, false, false);

                if (objectID.Equals(KD.Const.UnknownId))
                {
                    _plugin.SetReference();
                    //The 20/01/2021 i change the reference to place the article with CommentName instead of Name cause the article with underscore doesnt work
                    objectID = _plugin.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.CommentName, 0, (int)this.Measure_B,
                    (int)this.Measure_T, (int)this.Measure_H, (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ,
                    0, this.AngleZ, false, false, false);
                }

                if (!objectID.Equals(KD.Const.UnknownId))
                {
                    return new Article(_plugin.CurrentAppli, objectID);
                }
            }
            return null;
        }
        private Article PlaceComponentObject()
        {
            if (this.RefPos.Contains(KD.StringTools.Const.Dot))
            {
                string refBase = this.RefPos.Split(KD.CharTools.Const.Dot)[0];

                foreach (KeyValuePair<int, string> kvp in Plugin.articleAlreadyPlacedDict)
                {
                    if (kvp.Value.Equals(refBase))
                    {
                        Article parent = new Article(_plugin.CurrentAppli, kvp.Key);
                        Articles childs = parent.GetChildren(FilterArticle.strFilterToGetValidNotPlacedHostedAndChildren());
                        foreach (Article child in childs)
                        {
                            //The 20/01/2021 i change the reference to place the article with CommentName instead of Name cause the article with underscore doesnt work
                            if (child.IsValid && child.KeyRef.Equals(this.CommentName))
                            {
                                bool find = false;  //Test with Bauformat Side panel maybe don't work for another
                                if (this.RefPntX == 0.0 && child.Handing == _plugin.CurrentAppli.GetTranslatedText("G"))
                                {
                                    find = true;
                                }
                                else if (this.RefPntX > 0.0 && child.Handing == _plugin.CurrentAppli.GetTranslatedText("D"))
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
                                    child.DimensionZ = this.Measure_H;
                                    //child.PositionX = x1;
                                    //child.PositionY = y1;
                                    child.PositionZ = this.RefPntZ;
                                    return child;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        private Article ReplaceObject()
        {
            _plugin.SetReference();
            //The 20/01/2021 i change the reference to place the article with CommentName instead of Name cause the article with underscore doesnt work
            if (!String.IsNullOrEmpty(_commentName))
            {
                bool bReplace = _plugin.CurrentAppli.Scene.EditReplaceSelection(this.CatalogManufacturer, this.CommentName, this.HingeType, (int)this.Measure_B,
                (int)this.Measure_T, (int)this.Measure_H, false);

                if (!bReplace)
                {
                    _plugin.SetReference();
                    //The 20/01/2021 i change the reference to place the article with CommentName instead of Name cause the article with underscore doesnt work
                    bReplace = _plugin.CurrentAppli.Scene.EditReplaceSelection(this.CatalogManufacturer, this.CommentName, 0, (int)this.Measure_B,
                     (int)this.Measure_T, (int)this.Measure_H, false);
                }
            }
            //if (bReplace)
            //{
                return _plugin.CurrentAppli.ActiveArticle;
            //}
            //return null;
        }
        private void MoveArticlePerRepere()
        {
            GlobalSegment globalSegment = new GlobalSegment(this._currentFileEGI);
            string version = globalSegment.GetVersion();
            _plugin.CurrentAppli.SceneComponent.SetReferenceFromObject(this.Article, false);
            switch (version)
            {
                case ItemValue.V1_50: //DISCAC
                    _article.PositionX += this.Measure_B;
                    _article.AngleOXY += 180;
                    break;
                case ItemValue.V1_51: //FBD et Bauformat
                    _article.PositionX += this.Measure_B;
                    _article.AngleOXY += 180;
                    break;
                default:
                    _article.PositionX += this.Measure_B; 
                    _article.AngleOXY += 180;
                    break;
            }
        }
        private void MoveArticlePositionZPlinth()
        {
            SegmentClassification segmentClassification = new SegmentClassification(this.Article);
            if (!segmentClassification.IsArticlePlinth())
            {
                if (this.Article.PositionZ != (double)this.RefPntZ)
                {
                    double gap = this.Article.PositionZ - (double)this.RefPntZ;
                    _article.PositionZ -= gap;
                }
            }
        }
        private void ChangeCarcasseDimY()
        {
            _article.DimensionY += CatalogConstante.FrontDepth;           
        }
      
        private void ChangeFilerDimensions() 
        {
            _article.DimensionX = this.Measure_B1; 
            _article.DimensionY = this.Measure_T1;      
            _article.AngleOXY -= 90.0;
        }
        private void ChangeFilerChildDimensions()
        {
            Articles childs = this.Article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    if (child.Name.ToUpper().Contains(_plugin.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper()))) 
                    {
                        child.DimensionX = (this.Measure_B - this.Measure_B1);
                        child.DimensionY = (this.Measure_T - this.Measure_T1);
                    }
                }
            }
        }
       
        private void ChangeFilerPositionsAndDimensions()
        {
            _article.PositionX -= CatalogConstante.FrontDepth;
            _article.PositionY -= CatalogConstante.FrontDepth;
            _article.DimensionX = this.Measure_B + CatalogConstante.FrontDepth;
            _article.DimensionY = this.Measure_T + CatalogConstante.FrontDepth;
            _article.AngleOXY -= 90.0;
        }

        private void SetAnglePositionAngle()
        {
            List<string> firstBraceParameters = KD.StringTools.Helper.ExtractParameters(this.Article.Script, String.Empty, KD.StringTools.Const.BraceOpen, KD.StringTools.Const.BraceClose);

            if (this.Article.Handing == _plugin.CurrentAppli.GetTranslatedText("G") && firstBraceParameters.Contains("SI".ToUpper()))
            {
                _article.AngleOXY += 90.0;
                _plugin.SetReference();
                _article.PositionY += this.Measure_B; // w1;
            }
        }
        private void SetAngleDimensionsFromEGI()
        {
            _measure_B1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B1)) + CatalogConstante.FrontDepth; //Distance between filer and vide sanitaire
            _measure_B2 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B2)); //Width of the front
            _measure_B3 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B3)); //Width of the carcasse
            _measure_T -= CatalogConstante.FrontDepth;
            _measure_T1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_T1)) + CatalogConstante.FrontDepth; //Depth of the carcasse
        }
        private void SetAngleChildDimensions()
        {
            _article.DimensionY = this.Measure_T1;

            Articles childs = this.Article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    if (child.Name.ToUpper().Contains(_plugin.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())))  
                    {
                        child.DimensionX = this.Measure_T - this.Measure_T1;
                        child.DimensionY = this.Measure_B1;
                    }
                }
            }
        }

        private void SetArticleNumber()
        {
            if (!String.IsNullOrEmpty(this.RefPos) && !this.RefPos.Contains(KD.StringTools.Const.Dot))
            {
                _article.Number = Convert.ToInt16(this.RefPos);
            }
        }

        private void AddPlacedArticleDict(Article article, string refPos)
        {
            if (article != null && article.IsValid && article.Number != KD.Const.UnknownId)
            {
                Plugin.articleAlreadyPlacedDict.Add(article.ObjectId, refPos);
            }
            else if (article == null)
            {
                Plugin.articleAlreadyPlacedDict.Add(Convert.ToInt32(refPos), refPos);
            }
        }

        public void NoPlacedArticleMessage()
        {
            if (Plugin.notPlacedArticleList.Count > 0)
            {
                string listString = String.Empty;
                foreach (string notPlacedArticle in Plugin.notPlacedArticleList)
                {
                    listString += notPlacedArticle + Environment.NewLine;
                }
                System.Windows.Forms.MessageBox.Show(listString, "Import EGI : Réf. non trouvé");
            }

            if (Plugin.articleAlreadyPlacedDict.Count > 0)
            {
                string listString = String.Empty;
                foreach (KeyValuePair<int, string> articleAlreadyPlaced in Plugin.articleAlreadyPlacedDict)
                {
                    listString += articleAlreadyPlaced.Key.ToString() + " : " + articleAlreadyPlaced.Value.ToString() + Environment.NewLine;
                }
                //System.Windows.Forms.MessageBox.Show(listString, "Import EGI : Réf. posé");
            }

        }
    }
}
