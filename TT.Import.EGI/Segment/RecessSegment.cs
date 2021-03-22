using System;
using System.Windows.Forms;

using KD.Model;

using Ord_Eancom;

namespace TT.Import.EGI
{
    public class RecessSegment
    {
        private const string normalRecessRef = "NICHERECT";
       
        private Plugin _plugin = null;
        private KD.Config.IniFile _currentFileEGI = null;

        private string _section = String.Empty;
        private Article _recess = null;

        private string _reference = String.Empty;

        private double _refPntX = 0.0;
        private double _refPntY = 0.0;
        private double _refPntZ = 0.0;
        private double _width = 0.0;
        private double _depth = 0.0;
        private double _height = 0.0;
        private double _angleZ = 0.0;       
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
        public Article Recess
        {
            get
            {
                return _recess;
            }
            set
            {
                _recess = value;
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

        public RecessSegment(Plugin plugin, KD.Config.IniFile fileEGI, string section)
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

            _reference = normalRecessRef;

            _wallRefNo = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.WallRefNo));
            _refPntXRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntXRel));
            _refPntYRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntYRel));
            _refPntZRel = KD.StringTools.Convert.ToDouble(_currentFileEGI.GetStringValue(_section, ItemKey.RefPntZRel));
        }

        public void Add()
        {
            this.Place();

            if (this.Recess != null && this.Recess.IsValid)
            {                if (!this.IsWallRefValid())
                {
                    this.Delete();
                }
            }
        }

        private void Place()
        {
            _plugin.SetReference();
            int id = _plugin.CurrentAppli.Scene.EditPlaceObject(ManageCatalog.ConstraintCatalogName, this.Reference, 0, (int)this.Width, (int)this.Depth, (int)this.Height,
                                                                    (int)this.RefPntX, (int)this.RefPntY, (int)this.RefPntZ, 0, this.AngleZ, false, false, false);

            if (id != KD.Const.UnknownId)
            {
                _recess = new Article(_plugin.CurrentAppli, id);
            }
            else
            {
                _recess = null;
            }
        }
    
        private bool IsWallRefValid()
        {
            if (this.Recess.Host.Number == (int)this.WallRefNo)
            {
                return true;
            }
            return false;
        }
        private void Delete()
        {
            DialogResult dialogResult = MessageBox.Show("La contrainte: " + this.Recess.Ref + " n'est pas dans le bon mur: " + this.WallRefNo.ToString() + Environment.NewLine +
                                                   "Voulez-vous la supprimer ?", "Information",
                                                   System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                _recess.DeleteFromScene();
            }

        }
    }
}
