using System;

using Eancom;

namespace TT.Import.EGI
{
    public class GlobalSegment
    {
        private string _version = String.Empty;
        private string _system = String.Empty;
        private string _name = String.Empty;
        private string _number = String.Empty;
        private string _drawDate = String.Empty;
        private string _drawTime = String.Empty;
        private string _roomHeight = String.Empty;
        private string _manufacturer = String.Empty;

        private readonly KD.Config.IniFile CurrentFileEGI = null;   
       
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
        public string System
        {
            get
            {
                return _system;
            }
            set
            {
                _system = value;
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
        public string Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
        public string DrawDate
        {
            get
            {
                return _drawDate;
            }
            set
            {
                _drawDate = value;
            }
        }
        public string DrawTime
        {
            get
            {
                return _drawTime;
            }
            set
            {
                _drawTime = value;
            }
        }
        public string RoomHeight
        {
            get
            {
                return _roomHeight;
            }
            set
            {
                _roomHeight = value;
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

        public GlobalSegment(KD.Config.IniFile fileEGI)
        {
            this.CurrentFileEGI = fileEGI;           
            this.InitMembers();
            this.SetMembers();
        }

        private void InitMembers()
        { 
            _version = String.Empty;
            _system = String.Empty;
            _name = String.Empty;
            _number = String.Empty;
            _drawDate = String.Empty;
            _drawTime = String.Empty;
            _roomHeight = String.Empty;
            _manufacturer = String.Empty;
        }
        private void SetMembers()
        {
            this.SetVersion();
            this.SetSystem();
            this.SetName();
            this.SetNumber();
            this.SetDrawDate();
            this.SetDrawTime();
            this.SetRoomHeight();
            this.SetManufacturer();
        }

        public string GetVersion()
        {
            _version = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.Version);

            if (!String.IsNullOrEmpty(this.Version))
            {
                if (this.Version.Split(KD.CharTools.Const.Underscore).Length > 1)
                {
                    return this.Version.Split(KD.CharTools.Const.Underscore)[1];
                }
            }

            return String.Empty;
        }

        private void SetVersion()
        {
            _version = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.Version);
        }
        private void SetSystem()
        {
            _system = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.System);
        }
        private void SetName()
        {
            _name = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.Name);
        }
        private void SetNumber()
        {
            _number = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.Number);
        }
        private void SetDrawDate()
        {
            _drawDate = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.DrawDate);
        }
        private void SetDrawTime()
        {
            _drawTime = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.DrawTime);
        }
        private void SetRoomHeight()
        {
            _roomHeight = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.RoomHeight);
        }
        private void SetManufacturer()
        {
            _manufacturer = this.CurrentFileEGI.GetStringValue(SegmentName.Global, ItemKey.Manufacturer);
        }
    }
}
