using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private const string ManufacturerCustomFromCatalog = "MANUFACTURER";
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
            System.Windows.Forms.OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = FileEGI.FilterEGIFile;
            openFile.InitialDirectory = orderDir;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                orderEGIFileName = Path.GetFileName(openFile.FileName);
                return true;
            }
            return false;       
        }

        public KD.Config.IniFile Initialize()
        {
            if (Open())
            {
                return new KD.Config.IniFile(Path.Combine(orderDir, orderEGIFileName));       
            }
            return null;
        }

    }
}
