using System;

using KD.Model;

using Eancom;

namespace TT.Import.EGI
{
    public class WallSegment
    {
        private Wall _wall = null;
        private string _section = String.Empty;

        private double _refPntX = 0.0;
        private double _refPntY = 0.0;
        private double _refPntZ = 0.0;
        private double _width = 0.0;
        private double _depth = 0.0;
        private double _height = 0.0;        
        private double _angleZ = 0.0;
        private string _wallType = string.Empty;

        private Plugin _plugin = null;
        private KD.Config.IniFile CurrentFileEGI = null;

        private string shapeWallPoint = "0,0,0,0;1000,150,2500,0";

        public Wall Wall
        {
            get
            {
                return _wall;
            }
            set
            {
                _wall = value;
            }
        }
     
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
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }
        public double Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
            }
        }
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
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
        public string WallType
        {
            get
            {
                return _wallType;
            }
            set
            {
                _wallType = value;
            }
        }

        public WallSegment(Plugin plugin, KD.Config.IniFile fileEGI, string section)
        {
            _plugin = plugin;
            this.CurrentFileEGI = fileEGI;            
            _section = section;

            this.InitMembers();
            this.SetMembers();
        }

        private void InitMembers()
        {
            _refPntX = 0.0;
            _refPntY = 0.0;
            _refPntZ = 0.0;
            _width = 0.0;
            _depth = 0.0;
            _height = 0.0;
            _angleZ = 0.0;
            _wallType = String.Empty;               
        }
        private void SetMembers()
        {
            _refPntX = KD.StringTools.Convert.ToDouble( this.CurrentFileEGI.GetStringValue(_section, ItemKey.RefPntX));
            _refPntY = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.RefPntY));
            _refPntZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.RefPntZ));
            _width = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Width));
            _depth = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Depth));
            _height = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.Height));
            _angleZ = KD.StringTools.Convert.ToDouble(this.CurrentFileEGI.GetStringValue(_section, ItemKey.AngleZ));
            _wallType = this.CurrentFileEGI.GetStringValue(_section, ItemKey.Walltype);
        }

        public void Add()
        {
            this.Place();
            this.SetDimensions();
            this.SetPositions();
            this.SetAngle();
        }
        
        private void Place()
        {
            int wallId = _plugin.CurrentAppli.Scene.EditPlaceWalls((int)this.Depth, (int)this.Height, shapeWallPoint);
            _wall = new Wall(_plugin.CurrentAppli, wallId);           
        }
        private void SetDimensions()
        {
            _wall.DimensionX = this.Width;
            _wall.DimensionY = this.Depth;
            _wall.DimensionZ = this.Height;
        }
        private void SetPositions()
        {
            _plugin.SetReference();

            _wall.PositionX = this.RefPntX;
            _wall.PositionY = this.RefPntY;
            _wall.PositionZ = this.RefPntZ;
        }
        private void SetAngle()
        {
            _plugin.SetReference();

            _wall.AngleOXY = this.AngleZ;
        }
     
    }
}
