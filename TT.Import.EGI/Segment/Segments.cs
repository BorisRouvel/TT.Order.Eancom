using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Import.EGI
{
    public class SegmentName
    {
        public const string Global = "Global";
        public const string Wall_ = "Wall_";
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
