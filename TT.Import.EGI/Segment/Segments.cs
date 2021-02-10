using System;
using System.Collections.Generic;

using KD.Model;
using KD.Config;
using KD.CatalogProperties;

namespace TT.Import.EGI
{
    public class SegmentFormat
    {
        public const string CommentChar = "#";
        public const string DotDecimal = "0.00";
        public const string FourZero = "0000";
    }

    public class SegmentName
    {
        public const string Global = "Global";
        public const string Wall_ = "Wall_";
        public const string Door_ = "Door_";
        public const string Window_ = "Window_";
        public const string Recess_ = "Recess_";
        public const string Hindrance_ = "Hindrance_";
        public const string Article_ = "Article_";
    }

    public class ItemKey
    {
        public const string Version = "Version";
        public const string System = "System";
        public const string Number = "Number";
        public const string DrawDate = "DrawDate";
        public const string DrawTime = "DrawTime";
        public const string RoomHeight = "RoomHeight";
        public const string Manufacturer = "Manufacturer";

        public const string Width = "Width";
        public const string Depth = "Depth";
        public const string Height = "Height";
        public const string Walltype = "Walltype";

        public const string RefNo = "RefNo";
        public const string RefPos = "RefPos";
        public const string RefPntX = "RefPntX";
        public const string RefPntY = "RefPntY";
        public const string RefPntZ = "RefPntZ";
        public const string Measure_ = "Measure_";
        public const string Measure_B = "Measure_B";
        public const string Measure_B1 = "Measure_B1";
        public const string Measure_B2 = "Measure_B2";
        public const string Measure_B3 = "Measure_B3";
        public const string Measure_BE = "Measure_BE";
        public const string Measure_H = "Measure_H";
        public const string Measure_T = "Measure_T";
        public const string Measure_T1 = "Measure_T1";
        public const string Measure_TE = "Measure_TE";
        public const string AngleZ = "AngleZ";
        public const string Name = "Name";
        //public const string Name_ = "#Name"; // # mean comment
        public const string ConstructionType = "ConstructionType";
        public const string Hinge = "Hinge";
        public const string Shape = "Shape";
        public const string PolyType = "PolyType";
        public const string PolyCounter = "PolyCounter";
        public const string PolyPntX = "PolyPntX";
        public const string PolyPntY = "PolyPntY";
        public const string PolyPntZ = "PolyPntZ";

        public const string WallRefNo = "WallRefNo";
        public const string Opening = "Opening";
        public const string RefPntXRel = "RefPntXRel";
        public const string RefPntYRel = "RefPntYRel";
        public const string RefPntZRel = "RefPntZRel";
    }

    public class ItemValue
    {
        public const string V1_50 = "V1.50"; //DISCAC
        public const string V1_51 = "V1.51"; //FBD //BAUFORMAT
        public const string Left_Hinge = "L";
        public const string Right_Hinge = "R";
        public const string None_Hinge = "N";
        public const string Top_Hinge = "T";
        public const string Bottom_Hinge = "B";
        public const string InWards = "I";
        public const string OutWard = "O";
        public const string Slide = "S";
        public const string None = "N";

        public const string Shape_20_CornerOrFiler = "20";
        public const string Shape_27_AngleWithFiler = "27";
    }

    public class PolytypeValue
    {
        public const string Meaning_Base = "Base";
        public const string Meaning_WorkTop = "Work Top";
        public const string Meaning_WallConnection = "Wall Connection";
        public const string Meaning_LightPelmet = "Light Pelmet";
        public const string Meaning_Cornice = "Cornice";
        public const string Meaning_MouldedFloor = "Moulded Floor";
        public const string Meaning_Railing = "Railing";
        public const string Meaning_Hindrance = "Hindrance";

        public const string Z_Coordinate_Top = "Top Edge";
        public const string Z_Coordinate_Bottom = "Bottom Edge";

        public const string Polyline_Open = "Open";
        public const string Polyline_Closed = "Closed";

        public const int Polytype_Base = 1;
        public const int Polytype_WorkTop = 2;
        public const int Polytype_WallConnection = 3;
        public const int Polytype_LightPelmet = 4;
        public const int Polytype_Cornice = 5;
        public const int Polytype_MouldedFloor = 6;
        public const int Polytype_Railing = 7;
        public const int Polytype_Hindrance = 99;

        private int _polytype = 0;
        private string _meaning;
        private string _z_Coordinate;
        private string _polyline;

        public int Polytype
        {
            get
            {
                return _polytype;
            }
            set
            {
                _polytype = value;
            }
        }      
        public string Meaning
        {
            get
            {
                return _meaning;
            }
            set
            {
                _meaning = value;
            }
        }       
        public string Z_Coordinate
        {
            get
            {
                return _z_Coordinate;
            }
            set
            {
                _z_Coordinate = value;
            }
        }       
        public string Polyline
        {
            get
            {
                return _polyline;
            }
            set
            {
                _polyline = value;
            }
        }


        public PolytypeValue(int polytype)
        {
            this.InitMembers();

            _polytype = polytype;            
            this.SetMembers();
        }

        private void InitMembers()
        {
            _polytype = 0;
            _meaning = String.Empty;
            _z_Coordinate = String.Empty;
            _polyline = String.Empty;
        }
        public int Number()
        {
            return this.Polytype;
        }
        private void SetMembers()
        {
            switch (this.Polytype)
            {
                case Polytype_Base:
                    _meaning = Meaning_Base;
                    _z_Coordinate = Z_Coordinate_Top;
                    _polyline = Polyline_Open;
                    break;
                case Polytype_WorkTop:
                    _meaning = Meaning_WorkTop;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Closed;
                    break;
                case Polytype_WallConnection:
                    _meaning = Meaning_WallConnection;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Open;
                    break;
                case Polytype_LightPelmet:
                    _meaning = Meaning_LightPelmet;
                    _z_Coordinate = Z_Coordinate_Top;
                    _polyline = Polyline_Open;
                    break;
                case Polytype_Cornice:
                    _meaning = Meaning_Cornice;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Open;
                    break;
                case Polytype_MouldedFloor:
                    _meaning = Meaning_MouldedFloor;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Closed;
                    break;
                case Polytype_Railing:
                    _meaning = Meaning_Railing;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Open;
                    break;
                case Polytype_Hindrance:
                    _meaning = Meaning_Hindrance;
                    _z_Coordinate = Z_Coordinate_Bottom;
                    _polyline = Polyline_Closed;
                    break;
                default:
                    break;
            }

        }
    } 

    public class CatalogBlockName
    {
        public const string Filer = "Fileur";
        public const string FilerCode = "@F";
        public const string Coin = "_COIN";
        public const string Angle = "angle";
    }

    public class CatalogConstante
    {
        public const double FrontDepth = 20.0;
        public const string Slide = "coulissant";
        public const string Leaf = "vantail";
    }

    public class BlockScriptCode
    {
        public const string PDH = "PDH";
        public const string PDVFH = "PDVFH";
        public const string PDVFV = "PDVFV";
        public const string PDG = "PDG";
        public const string PC = "PC";
        public const string PCM = "PCM";
    }

    public class SegmentClassification
    {
        private Article _article;

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

        private readonly string _section;
        private IniFile _currentFileEGI = null;

        private Reference reference = null;

        private readonly List<string> splashbackPanelScriptCodeList = new List<string> { BlockScriptCode.PDH, BlockScriptCode.PDVFH,
                                                                                         BlockScriptCode.PDVFV, BlockScriptCode.PDG,
                                                                                         BlockScriptCode.PC, BlockScriptCode.PCM };

        public SegmentClassification(Article article)
        {
            _article = article;

            this.SetMembers();
        }
        public SegmentClassification(string section, IniFile currentFileEGI)
        {           
            _section = section;
            _currentFileEGI = currentFileEGI;
        }
        public SegmentClassification(Article article, string section, IniFile currentFileEGI)
        {
            _article = article;
            _section = section;
            _currentFileEGI = currentFileEGI;

            this.SetMembers();
        }
        public void SetMembers()
        {
            string catalogFilePath = System.IO.Path.Combine(this.Article.CurrentAppli.CatalogDir, this.Article.CatalogFileName + KD.CatalogProperties.Const.CatalogExtension);
            reference = new Reference(this.Article.CurrentAppli, catalogFilePath);
        }

        public bool HasSectionPolytype()
        {
            if (!String.IsNullOrEmpty(_currentFileEGI.GetStringValue(_section, ItemKey.PolyType)))
            {
                return true;
            }
            return false;
        }
        public bool HasSectionMeasureT1()
        {
            if (!String.IsNullOrEmpty(_currentFileEGI.GetStringValue(_section, ItemKey.Measure_T1)))
            {
                return true;
            }
            return false;
        }
        public bool IsSectionSpecialShape()
        {
            if (IsSectionAngleWithFiler() || IsSectionCorner())
            {
                return true;
            }
            return false;
        }
        public bool IsSectionAngleWithFiler() //Shape 27 Angle
        {
            string value = _currentFileEGI.GetStringValue(_section, ItemKey.Shape);
            if (!String.IsNullOrEmpty(value))
            {
                if (value == ItemValue.Shape_27_AngleWithFiler)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsSectionCorner() //Shape 20 filer
        {
            string value = _currentFileEGI.GetStringValue(_section, ItemKey.Shape);
            if (!String.IsNullOrEmpty(value))
            {
                if (value == ItemValue.Shape_20_CornerOrFiler)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasSectionCoin()
        {
            string value = _currentFileEGI.GetStringValue(_section, ItemKey.Measure_B1);
            if (!String.IsNullOrEmpty(value))
            {
                if (KD.StringTools.Convert.ToDouble(value) > 300.0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasArticleCoinParent()
        {
            if (this.Article.HasParent())
            {
                Article parent = this.Article.Parent;
                if (parent != null && parent.IsValid)
                {
                    if (parent.KeyRef.ToUpper().EndsWith(CatalogBlockName.Coin.ToUpper()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsArticleFiler()
        {
            if (this.Article.Name.ToUpper().Contains(this.Article.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())) ||
                this.Article.Name.ToUpper().StartsWith(CatalogBlockName.FilerCode.ToUpper()))
            {
                return true;
            }
            return false;
        }
        public bool IsArticleCornerFilerWithoutCoin()
        {
            if (this.Article.Name.ToUpper().Contains(this.Article.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())) ||
                this.Article.Name.ToUpper().StartsWith(CatalogBlockName.FilerCode.ToUpper()))
            {
                if (this.Article.Name.ToUpper().Contains("90".ToUpper()) && !this.Article.KeyRef.ToUpper().EndsWith(CatalogBlockName.Coin.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleCornerFilerWithCoin()
        {
            if (this.Article.Name.ToUpper().Contains(this.Article.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())) ||
                this.Article.Name.ToUpper().StartsWith(CatalogBlockName.FilerCode.ToUpper()) || this.Article.KeyRef.ToUpper().EndsWith(CatalogBlockName.Coin.ToUpper()))
            {
                if (this.Article.Name.ToUpper().Contains("90".ToUpper()) && this.Article.KeyRef.ToUpper().EndsWith(CatalogBlockName.Coin.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleCornerOrAngleUnit()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 3 || this.Article.Layer == 4 || this.Article.Layer == 9 || this.Article.Layer == 10)
                {
                    if (this.Article.Name.ToUpper().Contains(this.Article.CurrentAppli.GetTranslatedText(CatalogBlockName.Angle.ToUpper())))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsArticleUnit()
        {
            if (this.Article.CurrentAppli.Scene.ObjectGetInfo(this.Article.ObjectId, KD.SDK.SceneEnum.ObjectInfo.ISGRAPHIC) == KD.StringTools.Const.One)// the 08-02-21 //add this control cause many component have same layer below
            {
                if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
                {
                    //Level1 = Meubles bas, 3
                    //Level2 = Armoires, 4
                    //Level3 = Meubles hauts, 9
                    //Level4 = Sur - meubles, 10
                    if (this.Article.Layer == 3 || this.Article.Layer == 4 || this.Article.Layer == 9 || this.Article.Layer == 10)
                    {
                        if (!this.Article.Name.ToUpper().Contains(this.Article.CurrentAppli.GetTranslatedText(CatalogBlockName.Filer.ToUpper())))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool IsArticleUnitFloor()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {               
                if (this.Article.Type == 0 && (this.Article.Layer == 3 || (this.Article.Layer == 4)))
                {
                    return true;
                }               
            }
            return false;
        }
        public bool IsArticleWorkTop()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 5)//for instead don't test the type 5 because must be test type 0 also (this.Article.Type == 5 && )
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticlePlinth()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 2) //for instead don't test the type 6 because must be test type 0 also (this.Article.Type == 6 && )
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleMoulure()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 16)//for instead don't test the type 6 because must be test type 0 also (this.Article.Type == 6 && )
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleLightPelmet()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 8)//for instead don't test the type 6 because must be test type 0 also (this.Article.Type == 6 && )
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleCornice()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Layer == 11)//for instead don't test the type 6 because must be test type 0 also (this.Article.Type == 6 && )
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleLinear()
        {
            if (IsArticlePlinth() || IsArticleMoulure() || IsArticleLightPelmet() || IsArticleCornice())
            {
                return true;
            }
            return false;
        }
        public bool IsArticleShape()
        {
            if (this.Article.Topic == (int)KD.SDK.SceneEnum.TopicId.KITCHEN)
            {
                if (this.Article.Type == 5)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleSplashbackPanel()
        {
            foreach (string scriptCode in splashbackPanelScriptCodeList)
            {
                if (this.Article.Script.StartsWith(scriptCode + KD.StringTools.Const.BraceOpen))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsArticleSplashbackPanelShape()
        {           
            if (this.Article.Script.StartsWith(BlockScriptCode.PDG + KD.StringTools.Const.BraceOpen))
            {
                return true;
            }           
            return false;
        }
        public bool IsMeasurementsChange()
        {            
            int line = reference.GetArticleLineIndexFromReference(this.Article.KeyRef);

            if (line != KD.Const.UnknownId)
            {
                reference = new Reference(this.Article.CurrentAppli, (int)KD.SDK.CatalogEnum.ClusterRankType.CLUSTER_FROM_ITEM, line);
                if ((this.Article.DimensionX != reference.Article_Width) || (this.Article.DimensionY != reference.Article_Depth) || (this.Article.DimensionZ != reference.Article_Height))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetShape()
        {
            return this.Article.CurrentAppli.Scene.ObjectGetShape(this.Article.ObjectId);
        }
        public string[] GetShapePointsList()
        {
            string shapePointList = this.GetShape();
            return shapePointList.Split(KD.CharTools.Const.SemiColon);
        }

        
    }
}

