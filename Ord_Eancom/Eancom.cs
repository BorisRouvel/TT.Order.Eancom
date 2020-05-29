using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using KD.SDKComponent;

using Ord_Eancom;

namespace Eancom
{
    public class Separator
    {
        public static readonly string NewLine = Environment.NewLine;
        public const string DataGroup = "+";
        public const string DataElement = ":";
        public static string EndLine = "'" + NewLine;
        public const string DecimalSep = ".";
        public const string FreeChar = "?";

    }

    public class StructureEDI
    {
        public static string UNA = "UNA";       //{Définition du caractère de séparation}
        public const string UNB = "UNB";        //{En-tête des fichiers EDI}
        public const string UNH = "UNH";        //{En-tête d'un message}
        public const string BGM = "BGM";        //{Début du message. Définition du numéro de commande, etc.}
        public const string DTM = "DTM";        //{champ Date - dates de transaction}
        public const string FTX_H = "FTX";      //{Informations supplémentaires au niveau de la commission}
        public const string RFF_H = "RFF";      //{Informations complémentaires au niveau de la commission}
        public const string NAD = "NAD";        //{Identification des parties impliquées (nom / adresse)}
        public const string CTA = "CTA";        //{Spécification de la personne de contact de cette partie}
        public const string COM = "COM";        //{Spécification de la manière d'atteindre la personne de contact, par exemple fax / téléphone}
        public const string LIN_H  = "LIN";     //(données d'en-tête) {Début d'une gamme de produits / d'une option}
        public const string PIA_H  = "PIA";     //{Spécification plus détaillée d//une option}
        public const string LIN_A = "LIN";      //(données d//article) {Début d//un nouvel élément}
        public const string PIA_A = "PIA";      //{Spécification plus détaillée de l'article: spécification des options, etc.}
        public const string IMD = "IMD";        //{Description de l'article}
        public const string MEA  = "MEA";     //{Dimensions d'article pour articles spécifiques à une dimension}
        public const string QTY = "QTY";      //{Spécification de la quantité commandée}
        public const string FTX_A = "FTX";    //{Informations supplémentaires au niveau de l'article}
        public const string RFF_A = "RFF";    //{Numéros de référence}
        public const string UNS = "UNS";        //{Séparateur entre l//item et la fin de section}
        public const string UNT = "UNT";        //{Fin du message}
        public const string UNZ = "UNZ";        //{Fin du fichier EDI}

    }

    public class IDMManufacturerID
    {
        public const string Discac = "1201";
    }

    public class FileEDI
    {
        private const string IniOrderFileName = "orders.ini";
        private const string EANCOMPAIRINGTABLES = "EANCOM_PAIRINGTABLES";

        private const string generalSection = "GENERAL";
        private const string finishTypeVariantSection = "FINISHTYPEVARIANTE";
        private const string articlesSection = "ARTICLES";

        private const string manufacturerIDKey = "MANUFACTURERID";
        private const string manufacturerGLNKey = "MANUFACTURERGLN";
        private const string retailerCustomerNumberKey = "RETAILERCUSTOMERNUMBER";
        private const string serieNoKey = "SERIENO";
        private const string catalogIDKey = "CATALOGID";
        private const string orderKey = "ORDER";
        private const string ordersKey = "ORDERS";
        private const string ordrspKey = "ORDRSP";
        private const string ostrptKey = "OSTRPT";

        public const char separatorArticleField = KD.CharTools.Const.Pipe;

        private const string numberSection = "Number";
        private const string nextKey = "Next";

        private AppliComponent _currentAppli;
        public AppliComponent CurrentAppli
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

        private string _supplierName;
        public string SupplierName
        {
            get
            {
                return _supplierName;
            }
            set
            {
                _supplierName = value;
            }
        }

        OrderInformations _orderInformationsFromArticles = null;
        public string appairingCatalogFileName = String.Empty;

        KD.Config.IniFile ordersIniFile = new KD.Config.IniFile(Path.Combine(Order.orderDir, IniOrderFileName));
        KD.CsvHelper.CsvFileReader csvFileReader = null;
        KD.CsvHelper.CsvRow rowList = new KD.CsvHelper.CsvRow() { };

        public FileEDI(AppliComponent appli, string supplierName, OrderInformations orderInformationsFromArticles)
        {
            this._currentAppli = appli;
            this._supplierName = supplierName;
            this._orderInformationsFromArticles = orderInformationsFromArticles;
            appairingCatalogFileName = this._orderInformationsFromArticles.GetPairingCatalogFileName(this.CsvFileName());
            rowList.Clear();

            try
            {
                bool ok = this.CurrentAppli.Catalog.FileExportResourceFromName(Path.Combine(this.CurrentAppli.CatalogDir, this.CsvFileName()), true);
                
                csvFileReader = new KD.CsvHelper.CsvFileReader(Path.Combine(this.CurrentAppli.CatalogDir, this.CsvFileName()));

                while (!csvFileReader.EndOfStream)
                {
                    rowList.Add(csvFileReader.ReadLine());
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Fichier introuvable: " + Path.Combine(this.CurrentAppli.CatalogDir, this.CsvFileName()), "Informations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }        

        public string GetNextOrdersNumberHex()
        {
            string nextOrderNumber = ordersIniFile.ReadValue(numberSection, nextKey);
            int.TryParse(nextOrderNumber, out int value);
            string valueHex = value.ToString("X");
            string valueHexa = String.Empty;
            for (int i = 0; i < (8 - valueHex.Length); i++)
            {
                valueHexa += KD.StringTools.Const.Zero;
            }
            return valueHexa + valueHex;
        }

        public string CsvFileName()
        {
            return (SupplierName + KD.StringTools.Const.Underscore + EANCOMPAIRINGTABLES + KD.IO.File.Extension.Csv);
        }
        public string GetCsvValue(string value, int position)
        {            
           foreach (string dataLine in rowList)
            {
                string[] datas = dataLine.Split(KD.CharTools.Const.SemiColon);
                if ((!String.IsNullOrEmpty(datas[0]) && datas[0] == value) || (this.IsDiscacAssembly(datas[0], value)))
                {
                    return datas[position];
                }                
            }
            return null;
        }
        private bool IsDiscacAssembly(string data, string value)
        {            
            if (data.EndsWith(KD.StringTools.Const.Slatch + value))
            {
                return true;
            }           
            return false;
        }

        public string IniFileName()
        {
            return (SupplierName + KD.StringTools.Const.Underscore + EANCOMPAIRINGTABLES + ".ini"); // KD.IO.File.Extension.Ini);
        }        
        public string ManufacturerID()
        {
            return this.GetCsvValue(manufacturerIDKey, 1);          
        }
        public string ManufacturerGLN()
        {
            return this.GetCsvValue(manufacturerGLNKey, 1);
        }
        public bool HasManufacturerGLNCode(string code)
        {
            if (String.IsNullOrEmpty(code) || code == "9999999999999")
            {
                return false;
            }
            return true;
        }
        public string RetailerNumber()
        {
            return this.GetCsvValue(retailerCustomerNumberKey, 1);
        }
        public string SerieNo()
        {
            return this.GetCsvValue(serieNoKey, 1);
        }
        public string CatalogID()
        {
            string catalogId = this.GetCsvValue(catalogIDKey, 1);
            if (catalogId.Contains(FileEDI.separatorArticleField.ToString()))
            {
                catalogId = catalogId.Replace(FileEDI.separatorArticleField, KD.CharTools.Const.SemiColon);
            }
            return catalogId;
        }
        public string Order_()
        {
            return this.GetCsvValue(FileEDI.orderKey, 1);
        }
        public string Orders()
        {
            return this.GetCsvValue(FileEDI.ordersKey, 1);
        }
        public string Ordrsp()
        {
            return this.GetCsvValue(FileEDI.ordrspKey, 1);
        }
        public string Ostrpt()
        {
            return this.GetCsvValue(FileEDI.ostrptKey, 1);
        }
        public string FinishTypeVariant(string finishTypeKey)
        {
            return this.GetCsvValue(finishTypeKey, 1);
        }
        public string ArticleReferenceKey(string referenceKey, int position)
        {
            return this.GetCsvValue(referenceKey, position);
        }
    }

    public class Utility
    {
        private const char whiteSpaceChar = ' ';
        public const int codeCharLen = 5;
        public const int nameCharLen = 30;
        public const int finishLineMaxNb = 2;
        public const int freelyWordCharLen = 70;
        public const int freelyLineMaxNb = 99;

        public Utility()
        {
        }

        public string DelCharAndAllAfter(string text, string car)
        {
            if (!String.IsNullOrEmpty(text))
            {
                if (text.Contains(car))
                {
                    int end = text.IndexOf(car);
                    text = text.Substring(0, end);
                }
            }
            return text;
        }
        public StringBuilder CodeStringBuilder(string code)
        {
            StringBuilder codeStringBuilder = new StringBuilder(codeCharLen);
            return codeStringBuilder.Append(code).Append(whiteSpaceChar, (codeCharLen - code.Length));
        }
        public StringBuilder NameStringBuilder(string name, int start)
        {
            StringBuilder nameStringBuilder = new StringBuilder(nameCharLen);
            if (name.Length > nameCharLen)
            {
                return nameStringBuilder.Append(name, start, nameCharLen);
            }
            else
            {
                return nameStringBuilder.Append(name).Append(whiteSpaceChar, (nameCharLen - name.Length));
            }
        }
        public string GetFollowingChar(string text, int charLen)
        {
            if (text.Length <= charLen)
            {
                return String.Empty; // break;
            }
            else
            {
                return text.Substring(charLen, (text.Length - charLen));
            }
        }
        public string GetCodeLen(string code)
        {
            if (code.Length > Utility.codeCharLen)
            {
                code = code.Substring(0, Utility.codeCharLen);
            }
            return code;
        }
        public string GetLongPartType(string code)
        {
            switch (code)
            {
                case "10":
                    return "1";
                case "8":
                    return "2";
                case "228":
                    return "3";
                case "12":
                    return "4";
                case "11":
                    return "5";
                //case "":
                //    return "6";
                //case "":
                //    return "7";
                //case "":
                //    return "8";
                //case "":
                //    return "99";
                //case "":
                //    return "100";
                //case "":
                //    return "101";
                //case "":
                //    return "102";
                //case "":
                //    return "103";
                default:
                    return null;                  
            }
        }
        public string GetQuantityByOccurrence(AppliComponent currentAppli, int objectId)
        {
            return currentAppli.Scene.ObjectGetInfo(objectId, KD.SDK.SceneEnum.ObjectInfo.OCCURRENCESNB);
        }

        public bool IsPlinth(string code) //Cause exept plinth height 402 and 19 and 20
        {
            if (code == OrderConstants.PlinthFinishType)
            {
                return true;
            }
            return false;
        }
        public bool IsAssemblyWorktop(string type) //Cause exept plinth height 402 and 19 and 20
        {
            if (type == OrderConstants.LeftAssemblyFinishType || type == OrderConstants.RightAssemblyFinishType)
            {
                return true;
            }
            return false;
        }
        public bool IsPlinthOrAssemblyWorktop(string code) //Cause exept plinth height 402 and 19 and 20
        {
            if (code == OrderConstants.PlinthFinishType || code == OrderConstants.LeftAssemblyFinishType || code == OrderConstants.RightAssemblyFinishType)
            {
                return true;
            }
            return false;
        }
        public bool IsModel(string code)
        {
            if (code == KD.StringTools.Const.Zero)
            {
                return true;
            }
            return false;
        }
        public bool IsDimensionValid(double dimension)
        {
            if (dimension != 0.0 && dimension != 1.0)
            {
                return true;
            }
            return false;
        }
        public bool IsLinearPlanType(int type)
        {
            if (type == 5 || type == 6)
            {
                return true;
            }
            return false;
        }
    }
}
