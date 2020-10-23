using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Import.EGI
{
    public class ManageCatalog
    {
        private const string ManufacturerCustomFromCatalog = "MANUFACTURER";
        public const string ConstraintCatalogName = "@CONSTR";

        private string _catalogManufacturer = String.Empty;
        public string CatalogManufacturer
        {
            get
            {
                return _catalogManufacturer;
            }
            set
            {
                _catalogManufacturer = value;
            }
        }

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

        public ManageCatalog(KD.SDKComponent.AppliComponent currentAppli)
        {
            _currentAppli = currentAppli;
        }

        private IEnumerable<string> CatalogsBaseList()
        {
            return Directory.EnumerateFiles(this.CurrentAppli.CatalogDir, KD.StringTools.Const.Wildcard + KD.IO.File.Extension.Cat);
        }
        private string GetCatalogCustomInfo(string catalog, string info)
        {
            return this.CurrentAppli.CatalogGetCustomInfo(catalog, info);
        }
        public List<string> CatalogsByManufacturerList(string manufacturer)
        {
            List<string> list = new List<string>();
            list.Clear();

            foreach (string catalogPath in this.CatalogsBaseList())
            {
                string manufacturerCat = this.GetCatalogCustomInfo(catalogPath, ManufacturerCustomFromCatalog);

                if (!String.IsNullOrEmpty(manufacturerCat) && manufacturer.Equals(manufacturerCat))
                {
                    list.Add(catalogPath);
                }
            }
            return list;
        }
        private List<string> CatalogsByFirst4LettersList(List<string> catalogPathList)
        {
            string first4LettersBase = String.Empty;
            List<string> list = new List<string>();
            list.Clear();

            string catalogBase = Path.GetFileName(catalogPathList[0]);
            if (catalogBase.Length > 3)
            {
                first4LettersBase = catalogBase.Substring(0, 4);
            }

            foreach (string catalogPath in catalogPathList)
            {
                string catalog = Path.GetFileName(catalogPath);
                if (!String.IsNullOrEmpty(first4LettersBase) && catalog.Length > 3)
                {
                    string first4Letters = catalog.Substring(0, 4);
                    if (first4LettersBase.Equals(first4Letters))
                    {
                        list.Add(catalogPath);
                    }
                }
            }
            return list;
        }
        private string CatalogsByDateList(List<string> catalogsPathList)
        {
            string lastCatalog = String.Empty;
            int lastDate = 0;

            foreach (string catalogPath in catalogsPathList)
            {
                int date = this.CurrentAppli.CatalogGetModificationTime(catalogPath);

                if (date > lastDate)
                {
                    lastDate = date;
                    lastCatalog = catalogPath;
                }
            }
            return lastCatalog;
        }

        private string GetLastManufacturerFromCatalogs(List<string> catalogsPathList)
        {
            List<string> catalogsFirst4LettersList = new List<string>();
            List<string> catalogsManufacturerList = catalogsPathList;

            if (catalogsManufacturerList.Count > 0)
            {
                catalogsFirst4LettersList.Clear();
                catalogsFirst4LettersList = this.CatalogsByFirst4LettersList(catalogsManufacturerList);
            }

            string lastManufacturerDateCatalog = String.Empty;

            if (catalogsFirst4LettersList.Count > 0)
            {
                lastManufacturerDateCatalog = this.CatalogsByDateList(catalogsFirst4LettersList);
            }

            return lastManufacturerDateCatalog;
        }
        public void SetLastManufacturerCatalog(List<string> catalogsPathList)
        {
            _catalogManufacturer = this.GetLastManufacturerFromCatalogs(catalogsPathList);
        }
    }
}
