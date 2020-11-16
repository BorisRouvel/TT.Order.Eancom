using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Compression;

namespace Ord_Eancom
{
    public class OrderZip
    {
        public OrderZip()
        {
        }

        public void ZIPFile()
        {
            ZipArchiveEntry readmeEntry = null;
            using (FileStream zipToOpen = new FileStream(Path.Combine(Order.orderDir, OrderTransmission.OrderZipFileName), FileMode.Create, FileAccess.ReadWrite))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    string EDIFile = Path.Combine(Order.orderDir, OrderTransmission.OrderEDIFileName);
                    this.EntryZipAndDeleteFile(readmeEntry, archive, EDIFile, OrderTransmission.OrderEDIFileName);

                    if (MainForm.IsChoiceExportEGI)
                    {
                        string EGIFile = Path.Combine(Order.orderDir, OrderTransmission.OrderEGIFileName);
                        this.EntryZipAndDeleteFile(readmeEntry, archive, EGIFile, OrderTransmission.OrderEGIFileName);
                    }
                    if (MainForm.IsChoiceExportPlan)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGplanFile = Path.Combine(Order.orderDir, OrderTransmission.PlanName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGplanFile, OrderTransmission.PlanName + "-" + i + OrderTransmission.ExtensionJPG);
                        }
                    }
                    if (MainForm.IsChoiceExportElevation)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGelevFile = Path.Combine(Order.orderDir, OrderTransmission.ElevName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGelevFile, OrderTransmission.ElevName + "-" + i + OrderTransmission.ExtensionJPG);
                        }
                    }
                    if (MainForm.IsChoiceExportPerspective)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            string JPGpersFile = Path.Combine(Order.orderDir, OrderTransmission.PerspectiveName + "-" + i + OrderTransmission.ExtensionJPG);
                            this.EntryZipAndDeleteFile(readmeEntry, archive, JPGpersFile, OrderTransmission.PerspectiveName + "-" + i + OrderTransmission.ExtensionJPG);
                        }
                    }
                    if (MainForm.IsChoiceExportOrder)
                    {
                        string OrderFile = Path.Combine(Order.orderDir, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF);
                        this.EntryZipAndDeleteFile(readmeEntry, archive, OrderFile, OrderTransmission.OrderName + OrderTransmission.ExtensionPDF);
                    }
                }
            }
        }

        private void EntryZipAndDeleteFile(ZipArchiveEntry readmeEntry, ZipArchive archive, string file, string entryFile)
        {
            if (File.Exists(file))
            {
                readmeEntry = archive.CreateEntryFromFile(file, entryFile);
                File.Delete(file);
            }
        }
    }
}
