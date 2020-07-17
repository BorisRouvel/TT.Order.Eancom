using System;
using System.IO;
using System.Windows.Forms;

namespace TT.Import.EGI
{
    public class FileEGI
    {
        KD.SDKComponent.AppliComponent _currentAppli = null;
        KD.SDKComponent.AppliComponent CurrentAppli
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

        private const string Dir = "orders";
        //private const string ManufacturerCustomFromCatalog = "MANUFACTURER";
        private const string FilterEGIFile = "Fichiers egi (*.egi)|*.egi";
        
        public static string orderDir = String.Empty;
        private string orderEGIFileName = String.Empty;

        public FileEGI(KD.SDKComponent.AppliComponent currentAppli)
        {
            _currentAppli = currentAppli;
            orderDir = Path.Combine(this.CurrentAppli.ExeDir, Dir);
        }

        public bool Open()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = FileEGI.FilterEGIFile;
            openFile.InitialDirectory = orderDir;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                orderEGIFileName = Path.GetFileName(openFile.FileName);
                return true;
            }
            return false;       
        }

        public KD.Config.IniFile Initialize()
        {
            if (Open())
            {
                KD.Config.IniFile iniFile = new KD.Config.IniFile(Path.Combine(orderDir, orderEGIFileName));
                return iniFile;
            }
            return null;
        }

    }
}
