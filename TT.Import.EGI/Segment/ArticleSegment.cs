using System;
using System.Collections.Generic;

using KD.Model;
using KD.Analysis;

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
        private double _measure_T = 0.0;
        private double _measure_T1 = 0.0;
        private double _measure_H = 0.0;
        private double _angleZ = 0.0;

        private string _manufacturer = String.Empty;
        private string _name = String.Empty;
        private string _constructionType = String.Empty;
        private string _hinge = String.Empty;
        private int _hingeType = 0;
        private string _refNo = String.Empty;
        private string _refPos = String.Empty;
        private string _shape = String.Empty;
        private string _polyType = String.Empty;
        private string _polyCounter = String.Empty;

        private Plugin _plugin = null;        
        private string _section = String.Empty;
        private KD.Config.IniFile _currentFileEGI = null;
        private ManageCatalog _manageCatalog = null;

        //private List<String> notPlacedArticleList = new List<string>(0);
        //private Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>(0);

        public const string RefPointFormat = "0000";        
        
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
            this._currentFileEGI = fileEGI;            
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
            _measure_T = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_T));
            _measure_H = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_H));

            _angleZ = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.AngleZ));

            //_measure_B1 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Measure_B1));
            //_measure_B2 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Measure_B2));
            //_measure_B3 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Measure_B3));
            //_measure_T1 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Measure_T1));

            _polyType = _currentFileEGI.GetStringValue(_section, ItemKey.PolyType);
            _polyCounter = _currentFileEGI.GetStringValue(_section, ItemKey.PolyCounter);

        }



        public void PlaceArticle(string section, List<string> catalogsList)
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
                    this.PlaceObject();

                    if (this.Article != null && this.Article.IsValid)
                    {
                        this.MoveArticlePerRepere();
                        if (this.HasMeasureT1())
                        {
                            if (this.IsAngle())
                            {
                                this.SetAngleDimensionsFromEGI();
                                this.SetAngleChildDimensions();
                                this.SetAnglePositionAngle();
                            }
                            if (this.IsCorner())
                            {
                                this.SetCornerDimensions();
                            }
                        }

                        if (!String.IsNullOrEmpty(this.RefPos) && !this.RefPos.Contains(KD.StringTools.Const.Dot))
                        {
                            this.Article.Number = Convert.ToInt16(this.RefPos);
                        }
                        this.AddPlacedArticleDict(this.Article, this.RefPos);
                    }
                    else
                    {
                        if (catalogsList.Count > 1)
                        {
                            catalogsList.Remove(_manageCatalog.CatalogManufacturer);
                            this.PlaceArticle(section, catalogsList);
                        }
                        else
                        {
                            Plugin.notPlacedArticleList.Add(this.Name);
                        }
                    }
                }
            }
        }

        private int GetLinearType(string type)
        {
            PolytypeValue polytypeValue = new PolytypeValue(Convert.ToInt16(type));
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
        public void PlaceLinearArticle( string section, List<string> catalogsList)
        {
            string polyPoints = String.Empty;
            string typePoint = "0;";
            //polyType = this.GetPolyTypeFromEGI(section);
            //int linearType = this.GetLinearType(polyType); // not used actually
            //polyCounter = segment.SetPolyCounterFromEGI();
            int.TryParse(this.PolyCounter, out int valueCounter);

            if (valueCounter != 0)
            {
                for (int count = 1; count <= valueCounter; count++)
                {
                    string polyPntX = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntX + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat)) + KD.StringTools.Const.Comma;
                    string polyPntY = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntY + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat)) + KD.StringTools.Const.Comma;
                    string polyPntZ = _currentFileEGI.GetStringValue(section, ItemKey.PolyPntZ + KD.StringTools.Const.Underscore + count.ToString(ArticleSegment.RefPointFormat)) + KD.StringTools.Const.Comma;
                    
                    polyPoints += polyPntX + polyPntY + polyPntZ + typePoint;
                }
                polyPoints = polyPoints.Substring(0, polyPoints.Length - 1);
                _plugin.CurrentAppli.Scene.SceneAddShape(polyPoints);

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
                                isPlace = _plugin.CurrentAppli.Scene.EditPlaceLinearObject(_manageCatalog.CatalogManufacturer, linearReference, KD.StringTools.Const.Zero, true);
                                linearRef = linearReference;
                                break;
                            }
                        }
                        if (!isPlace)
                        {
                            if (catalogsList.Count > 1)
                            {
                                catalogsList.Remove(_manageCatalog.CatalogManufacturer);
                                this.PlaceLinearArticle(section, catalogsList);
                            }
                            else
                            {
                                Plugin.notPlacedArticleList.Add(this.Name);
                            }
                        }
                        else
                        {

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
                   // _plugin.SetReference();
                }

                //_plugin.CurrentAppli.Scene.SceneDeleteAllShapes();
            }
        }






        public bool HasPolytype()
        {
            if (!String.IsNullOrEmpty(_currentFileEGI.GetStringValue(_section, ItemKey.PolyType)))
            {
                return true;
            }
            return false;
        }
        public bool HasMeasureT1()
        {
            if (!String.IsNullOrEmpty(_currentFileEGI.GetStringValue(_section, ItemKey.Measure_T1)))
            {
                return true;
            }
            return false;
        }
        public bool IsAngle()
        {
            string value = _currentFileEGI.GetStringValue(_section, ItemKey.Shape);
            if (!String.IsNullOrEmpty(value))
            {
                if (value == "27")
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsCorner()
        {
            string value = _currentFileEGI.GetStringValue(_section, ItemKey.Shape);
            if (!String.IsNullOrEmpty(value))
            {
                if (value == "20")
                {
                    return true;
                }
            }
            return false;
        }

        public void PlaceObject()
        {
            _plugin.SetReference();           

            int objectID = _plugin.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.Name, this.HingeType, (int)this.Measure_B, 
                (int)this.Measure_T, (int)this.Measure_H, (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ, 
                0, this.AngleZ, false, false, false);

            if (objectID.Equals(KD.Const.UnknownId))
            {
                _plugin.SetReference();               

                objectID = _plugin.CurrentAppli.Scene.EditPlaceObject(this.CatalogManufacturer, this.Name, 0, (int)this.Measure_B,
                (int)this.Measure_T, (int)this.Measure_H, (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ,
                0, this.AngleZ, false, false, false);
            }

            if (!objectID.Equals(KD.Const.UnknownId))
            {
                _article = new Article(_plugin.CurrentAppli, objectID);
            }
            
        }
        public Article PlaceComponentObject()
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
                            if (child.IsValid && child.Ref.Equals(this.Name))
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
        public void MoveArticlePerRepere()
        {
            GlobalSegment globalSegment = new GlobalSegment(this._currentFileEGI);
            string version = globalSegment.GetVersion();
            _plugin.CurrentAppli.SceneComponent.SetReferenceFromObject(this.Article, false);
            switch (version)
            {
                case GlobalSegment.V1_50: //DISCAC
                    this.Article.PositionX += this.Measure_B; //w1
                    this.Article.AngleOXY += 180;
                    break;
                case GlobalSegment.V1_51: //FBD et Bauformat
                    this.Article.PositionX += this.Measure_B; // w1;
                    this.Article.AngleOXY += 180;
                    break;
                default:
                    this.Article.PositionX += this.Measure_B; // w1;
                    this.Article.AngleOXY += 180;
                    break;
            }
        }
        public void SetAngleChildDimensions()
        {
            this.Article.DimensionY = this.Measure_T1; // dB1;

            Articles childs = this.Article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    if (child.Name.ToUpper().Contains(_plugin.CurrentAppli.GetTranslatedText("Fileur".ToUpper()))) //Name.StartsWith("@F")) 
                    {
                        child.DimensionX = this.Measure_T - this.Measure_T1; // d1 - dB1; //angleFilerWidth;
                        child.DimensionY = this.Measure_B1; // wB1; // angleFilerDepth;                     
                    }
                }
            }
        }
        public void SetCornerDimensions()
        {
            this.Article.DimensionX = this.Measure_B - this.Measure_B1; // w1 - wB1; //angleFilerWidth;
            this.Article.DimensionY = this.Measure_T - this.Measure_T1; // d1 - dB1; // angleFilerDepth;            
        }
        public void SetAnglePositionAngle()
        {
            List<string> firstBraceParameters = KD.StringTools.Helper.ExtractParameters(this.Article.Script, String.Empty, KD.StringTools.Const.BraceOpen, KD.StringTools.Const.BraceClose);

            if (this.Article.Handing == _plugin.CurrentAppli.GetTranslatedText("G") && firstBraceParameters.Contains("SI".ToUpper()))
            {
                this.Article.AngleOXY += 90.0;
                _plugin.SetReference();              
                this.Article.PositionY += this.Measure_B; // w1;
            }
        }
        public void SetAngleDimensionsFromEGI()
        {
            _measure_B1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B1));
            _measure_B2 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B2));
            _measure_B3 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_B3));
            _measure_T1 = KD.StringTools.Convert.ToDouble(this._currentFileEGI.GetStringValue(_section, ItemKey.Measure_T1));
        }

        public void AddPlacedArticleDict(Article article, string refPos)
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
                //MessageBox.Show(listString, "Import EGI : Réf. non trouvé");
            }

            if (Plugin.articleAlreadyPlacedDict.Count > 0)
            {
                string listString = String.Empty;
                foreach (KeyValuePair<int, string> articleAlreadyPlaced in Plugin.articleAlreadyPlacedDict)
                {
                    listString += articleAlreadyPlaced.Key.ToString() + " : " + articleAlreadyPlaced.Value.ToString() + Environment.NewLine;
                }
                //MessageBox.Show(listString, "Import EGI : Réf. posé");
            }

        }
    }
}
