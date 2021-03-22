using System;
using System.Windows.Forms;

using KD.Model;

using Ord_Eancom;

namespace TT.Import.EGI
{
    public class WindowSegment
    {
        private const string normalWindowRef = "F1V";
        private const string doubleWindowRef = "F2V";
        private const string slideWindowRef = "FCOUL";

        private Plugin _plugin = null;
        private KD.Config.IniFile _currentFileEGI = null;

        private string _section = String.Empty;
        private Article _window = null;

        private string _reference = String.Empty;

        private double _refPntX = 0.0;
        private double _refPntY = 0.0;
        private double _refPntZ = 0.0;
        private double _width = 0.0;
        private double _depth = 0.0;
        private double _height = 0.0;
        private double _angleZ = 0.0;
        private string _hinge = String.Empty; // None, Left, Right, Top, Bottom
        private int _hingeType = 0;
        private string _opening = String.Empty; // None, Inwards, Outward, Slide
        private double _wallRefNo = 0.0;
        private double _refPntXRel = 0.0;
        private double _refPntYRel = 0.0;
        private double _refPntZRel = 0.0;

        public string Section
        {
            get
            {
                return _section;
            }
            set
            {
                _section = value;
            }
        }
        public Article Window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
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
        public string Opening
        {
            get
            {
                return _opening;
            }
            set
            {
                _opening = value;
            }
        }
        public double WallRefNo
        {
            get
            {
                return _wallRefNo;
            }
            set
            {
                _wallRefNo = value;
            }
        }
        public double RefPntXRel
        {
            get
            {
                return _refPntXRel;
            }
            set
            {
                _refPntXRel = value;
            }
        }
        public double RefPntYRel
        {
            get
            {
                return _refPntYRel;
            }
            set
            {
                _refPntYRel = value;
            }
        }
        public double RefPntZRel
        {
            get
            {
                return _refPntZRel;
            }
            set
            {
                _refPntZRel = value;
            }
        }


        public WindowSegment(Plugin plugin, KD.Config.IniFile fileEGI, string section)
        {
            _plugin = plugin;
            _currentFileEGI = fileEGI;
            _section = section;

            this.InitMembers();
            this.SetMembers();
        }

        private void InitMembers()
        {
            _reference = String.Empty;

            _refPntX = 0.0;
            _refPntY = 0.0;
            _refPntZ = 0.0;
            _width = 0.0;
            _depth = 0.0;
            _height = 0.0;
            _angleZ = 0.0;
            _hinge = String.Empty;
            _hingeType = 0;
            _opening = String.Empty;
            _wallRefNo = 0.0;
            _refPntXRel = 0.0;
            _refPntYRel = 0.0;
            _refPntZRel = 0.0;
        }
        private void SetMembers()
        {
            _refPntX = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntX));
            _refPntY = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntY));
            _refPntZ = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntZ));
            _width = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.Width));
            _depth = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.Depth));
            _height = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.Height));
            _angleZ = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.AngleZ));
            _hinge = _currentFileEGI.GetStringValue(_section, ItemKey.Hinge);
            _hingeType = this.SetHingeType();

            _opening = _currentFileEGI.GetStringValue(_section, ItemKey.Opening);
            _reference = this.SetReference(); ;

            _wallRefNo = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.WallRefNo));
            _refPntXRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntXRel));
            _refPntYRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntYRel));
            _refPntZRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntZRel));
        }
        private int SetHingeType()
        {
            if (!String.IsNullOrEmpty(this.Hinge))
            {
                switch (this.Hinge)
                {
                    case ItemValue.Left_Hinge:
                        return 1;
                    case ItemValue.Right_Hinge:
                        return 2;
                    default:
                        return 0;
                }
            }
            return 0;
        }
        private string SetReference()
        {
            string keyRef = WindowSegment.normalWindowRef;

            if (!String.IsNullOrEmpty(this.Opening))
            {
                switch (this.Opening)
                {
                    case ItemValue.InWards:
                        keyRef = WindowSegment.normalWindowRef;
                        break;
                    case ItemValue.OutWard:
                        keyRef = WindowSegment.normalWindowRef;
                        break;
                    case ItemValue.Slide:
                        keyRef = WindowSegment.slideWindowRef;
                        break;
                    default:
                        keyRef = WindowSegment.normalWindowRef;
                        break;
                }
            }          

            if (keyRef == WindowSegment.normalWindowRef && (this.Hinge != ItemValue.Left_Hinge && this.Hinge != ItemValue.Right_Hinge))
            {
                keyRef = WindowSegment.doubleWindowRef;
            }
            return keyRef;
        }

        public void Add()
        {
            this.Place();

            if (this.Window != null && this.Window.IsValid)
            {
                //this.SetDimensions();
                //this.SetPositions();
                //this.SetAngle();
                this.InWardsOrOutWard();

                if (!this.IsWallRefValid())
                {
                    this.Delete();
                }
            }
        }

        private void Place()
        {
            _plugin.SetReference();
            int id = _plugin.CurrentAppli.Scene.EditPlaceObject(ManageCatalog.ConstraintCatalogName, this.Reference, this.HingeType, (int)this.Width, (int)this.Depth, (int)this.Height,
                                                                    (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ, 0, this.AngleZ, false, false, false);
            _window = new Article(_plugin.CurrentAppli, id);
        }
        //private void SetDimensions()
        //{
        //    _window.DimensionX = this.Width;
        //    _window.DimensionY = this.Depth;
        //    _window.DimensionZ = this.Height;
        //}
        //private void SetPositions()
        //{
        //    _plugin.SetReference();

        //    _window.PositionX = this.RefPntX;
        //    _window.PositionY = this.RefPntY;
        //    _window.PositionZ = this.RefPntZ;
        //}
        //private void SetAngle()
        //{
        //    _plugin.SetReference();

        //    _window.AngleOXY = this.AngleZ;
        //}
        private void InWardsOrOutWard()
        {
            if (!String.IsNullOrEmpty(this.Opening))
            {
                switch (this.Opening)
                {
                    case ItemValue.OutWard:
                        //execute reverse
                        this.Reverse();
                        break;
                }
            }
        }
        private void Reverse()
        {
            bool ok = _plugin.CurrentAppli.ExecuteMenuItem(KD.Const.UnknownId, (int)KD.SDK.AppliEnum.SelectionMenuItemsId.REVERSE);
        }
        private bool IsWallRefValid()
        {
            if (this.Window.Host.Number == (int)this.WallRefNo)
            {
                return true;
            }
            return false;
        }
        private void Delete()
        {
            DialogResult dialogResult = MessageBox.Show("La contrainte: " + this.Window.Ref + " n'est pas dans le bon mur: " + this.WallRefNo.ToString() + Environment.NewLine +
                                                   "Voulez-vous la supprimer ?", "Information",
                                                   System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                _window.DeleteFromScene();
            }

        }
    }
}
