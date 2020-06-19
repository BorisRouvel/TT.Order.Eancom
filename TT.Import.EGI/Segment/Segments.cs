using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Import.EGI
{
    public class Segment
    {
        private double _positionX = 0.0;
        private double _positionY = 0.0;
        private double _positionZ = 0.0;
        private double _dimensionX = 0.0;
        private double _dimensionX1 = 0.0;
        private double _dimensionX2 = 0.0;
        private double _dimensionX3 = 0.0;
        private double _dimensionY = 0.0;
        private double _dimensionY1 = 0.0;
        private double _dimensionZ = 0.0;
        private double _angleZ = 0.0;

        private string _manufacturer = String.Empty;
        private string _name = String.Empty;
        private string _reference = String.Empty;
        private string _constructionType = String.Empty;
        private int _hingeType = 0;
        private string _refNo = String.Empty;
        private string _refPos = String.Empty;
        private string _polyType = String.Empty;
        private string _polyCounter = String.Empty;

        private KD.Config.IniFile CurrentFileEGI = null;
        private string Section = String.Empty;
               
        public const string Global = "Global";
        public const string Wall_ = "Wall_";
        public const string Article_ = "Article_";
        public const string V1_50 = "V1.50"; // Discac
        public const string V1_51 = "V1.51"; // FBD , Bauformat
        public const string RefPointFormat = "0000";

        public double PositionX
        {
            get
            {
                return _positionX;
            }
            set
            {
                _positionX = value;
            }
        }
        public double PositionY
        {
            get
            {
                return _positionY;
            }
            set
            {
                _positionY = value;
            }
        }
        public double PositionZ
        {
            get
            {
                return _positionZ;
            }
            set
            {
                _positionZ = value;
            }
        }
        public double DimensionX
        {
            get
            {
                return _dimensionX;
            }
            set
            {
                _dimensionX = value;
            }
        }
        public double DimensionX1
        {
            get
            {
                return _dimensionX1;
            }
            set
            {
                _dimensionX1 = value;
            }
        }
        public double DimensionX2
        {
            get
            {
                return _dimensionX2;
            }
            set
            {
                _dimensionX2 = value;
            }
        }
        public double DimensionX3
        {
            get
            {
                return _dimensionX3;
            }
            set
            {
                _dimensionX3 = value;
            }
        }
        public double DimensionY
        {
            get
            {
                return _dimensionY;
            }
            set
            {
                _dimensionY = value;
            }
        }
        public double DimensionY1
        {
            get
            {
                return _dimensionY1;
            }
            set
            {
                _dimensionY1 = value;
            }
        }
        public double DimensionZ
        {
            get
            {
                return _dimensionZ;
            }
            set
            {
                _dimensionZ = value;
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
        public string Reference
        {
            get
            {
                return _reference;
            }
            set
            {
                _reference = value;
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


        public Segment(KD.Config.IniFile fileEGI, string section)
        {
            this.CurrentFileEGI = fileEGI;
            this.Section = section;
            this.InitMembers();
        }

        private void InitMembers()
        {
            _positionX = 0.0;
            _positionY = 0.0;
            _positionZ = 0.0;
            _dimensionX = 0.0;
            _dimensionX1 = 0.0;
            _dimensionX2 = 0.0;
            _dimensionX3 = 0.0;
            _dimensionY = 0.0;
            _dimensionY1 = 0.0;
            _dimensionZ = 0.0;

            _manufacturer = String.Empty;
            _reference = String.Empty;
            _constructionType = String.Empty;
            _refNo = String.Empty;
            _refPos = String.Empty;
        }

        public string GetVersion()
        {
            string version = CurrentFileEGI.GetStringValue(Segment.Global, ItemKey.Version);

            if (!String.IsNullOrEmpty(version))
            {
                if (version.Split(KD.CharTools.Const.Underscore).Length > 1)
                {
                    return version.Split(KD.CharTools.Const.Underscore)[1];
                }
            }

            return String.Empty;
        }


        public void SetWallItems()
        {
            this.SetPositionsFromEGI();
            this.SetWallDimensionsFromEGI();
            this.SetAngleFromEGI();
        }
        public void SetArticleItems()
        {
            this.SetManufacturerFromEGI();
            this.SetReferenceFromEGI();
            this.SetConstructionFromEGI();
            this.SetHingeFromEGI();
            this.SetRefNoFromEGI();
            this.SetRefPosFromEGI();

            this.SetPositionsFromEGI();
            this.SetArticleDimensionsFromEGI();
            this.SetAngleFromEGI();
            this.SetPolyTypeFromEGI();
            this.SetPolyCounterFromEGI();
        }

        public bool HasPolytype()
        {
            if (!String.IsNullOrEmpty(CurrentFileEGI.GetStringValue(this.Section, ItemKey.PolyType)))
            {
                return true;
            }
            return false;
        }
        public bool HasMeasureT1()
        {
            if (!String.IsNullOrEmpty(CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_T1)))
            {
                return true;
            }
            return false;
        }
        public bool IsAngle()
        {
            string value = CurrentFileEGI.GetStringValue(this.Section, ItemKey.Shape);
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
            string value = CurrentFileEGI.GetStringValue(this.Section, ItemKey.Shape);
            if (!String.IsNullOrEmpty(value))
            {
                if (value == "20")
                {
                    return true;
                }
            }
            return false;
        }

        private void SetPositionsFromEGI()
        {
            _positionX = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.RefPntX));
            _positionY = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.RefPntY));
            _positionZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.RefPntZ));
        }
        private void SetWallDimensionsFromEGI()
        {
            _dimensionX = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Width));
            _dimensionY = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Depth));
            _dimensionZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Height));          
        }
        private void SetArticleDimensionsFromEGI()
        {
            _dimensionX = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_B));
            _dimensionY = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_T));
            _dimensionZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_H));
        }
        private void SetAngleFromEGI()
        {            
            _angleZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.AngleZ));
        }

        public void SetAngleDimensionsFromEGI()
        {
            _dimensionX1 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_B1));
            _dimensionX2 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_B2));
            _dimensionX3 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_B3));
            _dimensionY1 = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(this.Section, ItemKey.Measure_T1));           
        }

        private void SetManufacturerFromEGI()
        {
            _manufacturer = CurrentFileEGI.GetStringValue(this.Section, ItemKey.Manufacturer);
        }
        private void SetReferenceFromEGI()
        {
            _name = CurrentFileEGI.GetStringValue(this.Section, ItemKey.Name);
        }
        private void SetConstructionFromEGI()
        {
            _constructionType = CurrentFileEGI.GetStringValue(this.Section, ItemKey.ConstructionType);
        }
        private void SetHingeFromEGI()
        {
            string hinge = CurrentFileEGI.GetStringValue(this.Section, ItemKey.Hinge);

            if (String.IsNullOrEmpty(hinge))
            {
                hinge = _constructionType;
            }
            switch (hinge)
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
        }
        private void SetRefNoFromEGI()
        {
            _refNo = CurrentFileEGI.GetStringValue(this.Section, ItemKey.RefNo);
        }
        private void SetRefPosFromEGI()
        {
            _refPos = CurrentFileEGI.GetStringValue(this.Section, ItemKey.RefPos);
        }
        private void SetPolyTypeFromEGI()
        {
            _polyType = CurrentFileEGI.GetStringValue(this.Section, ItemKey.PolyType);
        }
        private void SetPolyCounterFromEGI()
        {
            _polyCounter = CurrentFileEGI.GetStringValue(this.Section, ItemKey.PolyCounter);
        }
    }

    public class ItemKey
    {
        public const string Version = "Version";
        public const string Manufacturer = "Manufacturer";

        public const string Width = "Width";
        public const string Depth = "Depth";
        public const string Height = "Height";

        public const string RefNo = "RefNo";
        public const string RefPos = "RefPos";
        public const string RefPntX = "RefPntX";
        public const string RefPntY = "RefPntY";
        public const string RefPntZ = "RefPntZ";
        public const string Measure_B = "Measure_B";
        public const string Measure_B1 = "Measure_B1";
        public const string Measure_B2 = "Measure_B2";
        public const string Measure_B3 = "Measure_B3";
        public const string Measure_H = "Measure_H";
        public const string Measure_T = "Measure_T";
        public const string Measure_T1 = "Measure_T1";
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
    }

    public class ItemValue
    {
        public const string Left_Hinge = "L";
        public const string Right_Hinge = "R";
    }

    public class PolytypeValue
    {
        private const string Meaning_Base = "Base";
        private const string Meaning_WorkTop = "Work Top";
        private const string Meaning_WallConnection = "Wall Connection";
        private const string Meaning_LightPelmet = "Light Pelmet";
        private const string Meaning_Cornice = "Cornice";
        private const string Meaning_MouldedFloor = "Moulded Floor";
        private const string Meaning_Railing = "Railing";
        private const string Meaning_Hindrance = "Hindrance";

        private const string Z_Coordinate_Top = "Top Edge";
        private const string Z_Coordinate_Bottom = "Bottom Edge";

        private const string Polyline_Open = "Open";
        private const string Polyline_Closed = "Closed";

        private const int Polytype_Base = 1;
        private const int Polytype_WorkTop = 2;
        private const int Polytype_WallConnection = 3;
        private const int Polytype_LightPelmet = 4;
        private const int Polytype_Cornice = 5;
        private const int Polytype_MouldedFloor = 6;
        private const int Polytype_Railing = 7;
        private const int Polytype_Hindrance = 99;

        private int _polytype = 0;
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

        private string _meaning;
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

        private string _z_Coordinate;
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

        private string _polyline;
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
            _polytype = polytype;

            this.SetMembers();
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
}
