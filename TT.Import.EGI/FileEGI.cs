using System;
using System.IO;
using System.Windows.Forms;

namespace TT.Import.EGI
{
    public class FileEGI
    {
        private KD.SDKComponent.AppliComponent _currentAppli = null;
       
        private const string Dir = "orders";        
        private const string FilterEGIFile = "Fichiers egi (*.egi)|*.egi";
        
        public static string orderDir = String.Empty;
        private string orderEGIFilePath = String.Empty;      

        public FileEGI(KD.SDKComponent.AppliComponent currentAppli)
        {
            _currentAppli = currentAppli;
            orderDir = Path.Combine(_currentAppli.ExeDir, Dir);
        }

        public bool Open()
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = FileEGI.FilterEGIFile,
                RestoreDirectory = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                orderEGIFilePath = openFile.FileName;
               
                return true;
            }
            return false;       
        }

        public KD.Config.IniFile Initialize()
        {
            if (Open())
            {
                KD.Config.IniFile iniFile = new KD.Config.IniFile(orderEGIFilePath);
                return iniFile;
            }
            return null;
        }

    }
}
