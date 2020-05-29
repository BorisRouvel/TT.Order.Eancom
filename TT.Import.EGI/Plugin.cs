using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------

using KD.Model;
using KD.Plugin;
using KD.Analysis;



namespace TT.Import.EGI
{
    public class Plugin : KD.Plugin.PluginBase
    {
        private string _language = string.Empty;
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }


        private Translate translate = null;
        private KD.Plugin.MainAppMenuItem InSituMenuItem = null;
        private KD.Config.IniFile EGIIniFile = null;
        private IEnumerable<string> catalogsList = null;

        public const string Dir = "orders";
        public static string orderDir = String.Empty;

        public string versionEGI = String.Empty;

        double sceneDimX = 0.0;
        double sceneDimY = 0.0;
        string strSceneDimZ = String.Empty;
        double sceneDimZ = 0.0;
        double angleScene = 0.0;

        private string xb = String.Empty;
        private string yb = String.Empty;
        private string zb = String.Empty;
        private string x = String.Empty;
        private string y = String.Empty;
        private string z = String.Empty;
        private string wb = String.Empty;
        private string db = String.Empty;
        private string hb = String.Empty;
        private string w = String.Empty;
        private string d = String.Empty;
        private string h = String.Empty;
        private string ab = String.Empty;
        private double a = 0.0;

        private string wb1 = String.Empty;
        private string wb2 = String.Empty;
        private string wb3 = String.Empty;
        private string db1 = String.Empty;
        private double wB1 = 0.0;
        private double wB2 = 0.0;
        private double wB3 = 0.0;
        private double dB1 = 0.0;

        private double x1 = 0.0;
        private double y1 = 0.0;
        private double z1 = 0.0;
        private double w1 = 0.0;
        private double d1 = 0.0; 
        private double h1 = 0.0;

        private double angleFilerWidth = 0.0;
        private double angleFilerDepth = 0.0;

        private string manufacturer = String.Empty;
        private string reference = String.Empty;
        private string constructionType = String.Empty;
        private string polyType = String.Empty;
        private string polyCounter = String.Empty;
        private string PolyPntX = String.Empty;
        private string PolyPntY = String.Empty;
        private string PolyPntZ = String.Empty;

        private int hingeType = 0;

        private string shapeWallPoint = "0,0,0,0;1000,150,2500,0";

        public Plugin()
           : base(true)
        {            
            this._language = this.CurrentAppli.GetLanguage();
            this.translate = new Translate(this.Language);

            this.InSituMenuItem = new MainAppMenuItem((int)KD.SDK.AppliEnum.FileMenuItemsId.IMPORT_WMF, translate.PluginFunctionLanguageTranslate(),
                this.AssemblyFileName, KD.Plugin.Const.PluginClassName, "InSituFunction");
           
        }

        ~Plugin()
        {
        }

        public new bool OnPluginLoad(int iCallParamsBlock)
        {
            if (!this.CurrentAppli.IsUnknown())
            {
                this.InSituMenuItem.Insert(this.CurrentAppli);
                this.InSituMenuItem.Enable(this.CurrentAppli, false);
            }
            return true;
        }
        public new bool OnPluginUnload(int iCallParamsBlock)
        {
            if (!this.CurrentAppli.IsUnknown())
            {
                this.InSituMenuItem.Remove(this.CurrentAppli);
            }

            return true;
        }

        public bool OnFileNewAfter(int iCallParamsBlock)
        {
            this.InSituMenuItem.Enable(this.CurrentAppli, true);
            return true;
        }
        public bool OnFileOpenAfter(int iCallParamsBlock)
        {
            this.InSituMenuItem.Enable(this.CurrentAppli, true);
            return true;
        }
        public bool OnFileCloseAfter(int iCallParamsBlock)
        {
            this.InSituMenuItem.Enable(this.CurrentAppli, false);
            return true;
        }
        public new bool OnAppStartAfter(int iCallParamsBlock)
        {
            orderDir = Path.Combine(this.CurrentAppli.ExeDir, Dir);
            return true;
        }

        public void InSituFunction(int iCallParamsBlock)
        {
            string orderEGIFileName = this.OpenEGIFile();
            this.Initialize(orderEGIFileName);

            if (EGIIniFile != null)
            {
                Cursor.Current = Cursors.WaitCursor;
                versionEGI = this.GetVersionEGI();
                this.SetSceneReference(versionEGI);

                this.PlaceWallInScene();
                this.PlaceArticlesInScene();

                this.ResetReference();
                this.ImportTerminate();
            }
            else
            {
                this.ImportNoValid();
            }
            Cursor.Current = Cursors.Arrow;
        }

        private string ConvertToDecimalSeparatorCurrentCulture(string str)
        {
            string decimalSeparator = System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;

            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains(KD.StringTools.Const.Dot))
                {
                    return str.Replace(KD.StringTools.Const.Dot, decimalSeparator);
                }
                else if (str.Contains(KD.StringTools.Const.Comma))
                {
                    return str.Replace(KD.StringTools.Const.Comma, decimalSeparator);
               }
            }
            return str;
        }
        private int ConvertToInt(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                double value = Convert.ToDouble(str);
               return Convert.ToInt32(value);
            }
            return 0;
        }
        private double ConvertToDouble(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                return Convert.ToDouble(str);                
            }
            return 0.0;
        }

        private string OpenEGIFile()
        {
            System.Windows.Forms.OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Fichiers egi (*.egi)|*.egi";
            openFile.InitialDirectory = orderDir;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                return Path.GetFileName(openFile.FileName);
            }
            return String.Empty;
        }
        private void Initialize(string orderEGIFileName)
        {
            if (!String.IsNullOrEmpty(orderEGIFileName))
            {
                EGIIniFile = new KD.Config.IniFile(Path.Combine(orderDir, orderEGIFileName));
                string catalogDir = this.CurrentAppli.CatalogDir; // Path.Combine(this.CurrentAppli.CatalogDir, this.TemporisNumber());               
                catalogsList = Directory.EnumerateFiles(catalogDir, "*.cat");                
                return;
            }
            EGIIniFile = null;
        }
        private string TemporisNumber()
        {
            string accountNumber = this.CurrentAppli.GetAccountNumber();
            if (Directory.Exists(Path.Combine(this.CurrentAppli.CatalogDir, accountNumber)))
            {
                return accountNumber;
            }
            return String.Empty;
        }
        private string GetVersionEGI()
        {
            string version = EGIIniFile.GetStringValue("Global", "Version");
            return version.Split(KD.CharTools.Const.Underscore)[1];
        }
        private List<string> WallSectionsList()
        {
            List<string> sectionsList = EGIIniFile.GetSectionsNames();
            List<string> wallList = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith("Wall_"))
                {
                    wallList.Add(section);
                }
            }
            return wallList;
        }
        private List<string> ArticleSectionsList()
        {
            List<string> sectionsList = EGIIniFile.GetSectionsNames();
            List<string> articleList = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith("Article_"))
                {
                    articleList.Add(section);
                }
            }
            return articleList;
        }
        private void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case "V1.50": //DISCAC
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case "V1.51": //FBD
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (270 * System.Math.PI) / 180;
                    break;
                default:
                    sceneDimX = -this.CurrentAppli.SceneDimX / 2;
                    sceneDimY = this.CurrentAppli.SceneDimY / 2;
                    strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
            }
            
        }
       
        private void SetReference()
        {
            this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);
        }
        private void ResetReference()
        {
            this.CurrentAppli.SceneComponent.ResetSceneReference();
        }
        private void SetPositionsFromEGI(string section)
        {
            xb = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, "RefPntX"));
            yb = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, "RefPntY"));
            zb = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, "RefPntZ"));
            x1 = this.ConvertToDouble(xb);
            y1 = this.ConvertToDouble(yb);
            z1 = this.ConvertToDouble(zb);
        }
        private void SetDimensionsFromEGI(string section)
        {
            string strX = string.Empty;
            string strY = string.Empty;
            string strZ = string.Empty;

            if (section.StartsWith("Wall_"))
            {
                strX = "Width";
                strY = "Depth";
                strZ = "Height";
            }
            else if (section.StartsWith("Article_"))
            {
                strX = "Measure_B";
                strY = "Measure_T";
                strZ = "Measure_H";
            }

            wb = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strX));
            db = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strY));
            hb = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strZ));
            w1 = this.ConvertToDouble(wb);
            d1 = this.ConvertToDouble(db);
            h1 = this.ConvertToDouble(hb);
        }
        private void SetAngleFromEGI(string section)
        {
            ab = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, "AngleZ"));
            a = this.ConvertToDouble(ab);
        }
        private void SetAngleDimensionsFromEGI(string section)
        {
            string strX1 = "Measure_B1";
            string strX2 = "Measure_B2";
            string strX3 = "Measure_B3";
            string strY1 = "Measure_T1";                   

            wb1 = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strX1));
            wb2 = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strX2));
            wb3 = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strX3));
            db1 = this.ConvertToDecimalSeparatorCurrentCulture(EGIIniFile.GetStringValue(section, strY1));
            wB1 = this.ConvertToDouble(wb1);
            wB2 = this.ConvertToDouble(wb2);
            wB3 = this.ConvertToDouble(wb3);
            dB1 = this.ConvertToDouble(db1);

            angleFilerWidth = d1 - dB1;            
            angleFilerDepth = wB1;
        }
        private Wall SceneAddWall()
        {
            //this.CurrentAppli.Scene.SceneAddShape(shapeWallPoint);
            int wallId = this.CurrentAppli.Scene.EditPlaceWalls((int)d1, (int)h1, shapeWallPoint);
            //this.CurrentAppli.ExecuteMenuItem(KD.Const.UnknownId, (int)KD.SDK.AppliEnum.PlaceMenuItemsId.WALL);
            //this.CurrentAppli.Scene.SceneDeleteAllShapes();
            
            return new Wall(this.CurrentAppli.ActiveArticle);
        }
        private void SetDimensions(Article wall)
        {
            wall.DimensionX = w1;
            wall.DimensionY = d1;
            wall.DimensionZ = h1;
        }
        private void SetPositions(Wall wall)
        {
            wall.PositionX = x1;
            wall.PositionY = y1;
            wall.PositionZ = z1;
        }
        private void SetAngle(Wall wall)
        {
            wall.AngleOXY = a;
        }

        private void PlaceWallInScene()
        {
            foreach (string wallSection in this.WallSectionsList())
            {
                this.SetPositionsFromEGI(wallSection);
                this.SetDimensionsFromEGI(wallSection);
                this.SetAngleFromEGI(wallSection);                

                Wall wall = this.SceneAddWall();
                this.SetDimensions(wall);
                this.SetReference();
                this.SetPositions(wall);
                this.SetAngle(wall);                
                this.ResetReference();
            }
        }

        private bool HasPolytype(string section)
        {
           if (!String.IsNullOrEmpty(EGIIniFile.GetStringValue(section, "PolyType")))
            {
                return true;
            }
            return false;
        }
        private bool HasMeasureT1(string section)
        {
            if (!String.IsNullOrEmpty(EGIIniFile.GetStringValue(section, "Measure_T1")))
            {
                return true;
            }
            return false;
        }
        private string GetPolyTypeFromEGI(string section)
        {
            return EGIIniFile.GetStringValue(section, "PolyType");
        }
        private string GetPolyCounterFromEGI(string section)
        {
            return EGIIniFile.GetStringValue(section, "PolyCounter");
        }
        private string GetManufacturerFromEGI(string section)
        {
            return EGIIniFile.GetStringValue(section, "Manufacturer");
        }
        private string GetReferenceFromEGI(string section)
        {
            string name = EGIIniFile.GetStringValue(section, "#Name");
            if (!String.IsNullOrEmpty(name))
            {
                return name;
            }
            else
            { 
                return EGIIniFile.GetStringValue(section, "Name");
            }
        }
        private string GetConstructionFromEGI(string section)
        {
            return EGIIniFile.GetStringValue(section, "ConstructionType");
        }
        private int GetHingeFromEGI(string section)
        {
            hingeType = 0;
            string hinge = EGIIniFile.GetStringValue(section, "Hinge");           

            if (String.IsNullOrEmpty(hinge))
            {
                hinge = constructionType;
            }
            switch (hinge)
            {
                case "L":
                    return 1;
                case "R":
                    return 2;
                default:
                    return 0;
            }
        }
        private void PlaceArticle(string section)
        {
            int objectID = KD.Const.UnknownId;           
            foreach (string catalog in catalogsList)
            {               
                string manufacturerCat = this.CurrentAppli.CatalogGetCustomInfo(catalog, "MANUFACTURER");
                if (!String.IsNullOrEmpty(manufacturerCat) && manufacturer.Equals(manufacturerCat))
                {                    
                    this.SetReference();
                    objectID = this.CurrentAppli.Scene.EditPlaceObject(catalog, reference, hingeType, (int)w1, (int)d1, (int)h1,
                        (int)x1, (int)y1, (int)z1, 0, a, false, false, false);

                    if (!objectID.Equals(KD.Const.UnknownId))
                    {
                        Article article = new Article(this.CurrentAppli, objectID);
                        if (article != null && article.IsValid)
                        {
                            this.MoveArticlePerRepere(article);
                            if (HasMeasureT1(section))
                            {
                                this.SetAngleDimensionsFromEGI(section);
                                this.SetAngleArticleDimensions(article);
                            }
                            break;
                        }
                    }
                }
            }
        }
        private void MoveArticlePerRepere(Article article)
        {
            this.CurrentAppli.SceneComponent.SetReferenceFromObject(article, false);
            switch (versionEGI)
            {
                case "V1.50": //DISCAC
                    article.PositionX += w1;
                    article.AngleOXY += 180;
                    break;
                case "V1.51": //FBD
                    article.PositionX += w1;
                    article.AngleOXY += 180;
                    break;
                default:
                    article.PositionX += w1;
                    article.AngleOXY += 180;
                    break;
            }
        }
        private void SetAngleArticleDimensions(Article article)
        {
            article.DimensionY = dB1;            

            Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
            if (childs != null && childs.Count > 0)
            {
                foreach (Article child in childs)
                {
                    if (child.Name.StartsWith("@F"))
                    {
                        child.DimensionX = angleFilerWidth;
                        child.DimensionY = angleFilerDepth;
                    }
                }
            }
        }
        private int GetLinearType(string type)
        {
            switch (type)
            {
                case "1": //Plinthe
                    return 0;
                case "2": //WorkTop
                    return 1;
                case "3": //Cache lumière
                    return 2;
                case "4": //Corniche
                    return 3;
                case "5": //Edge workTop
                    return 4;
                default:
                    return -1;                   
            }
        }
        private void PlaceLinearArticle(string section)
        {
            string polyPoints = String.Empty;
            string typePoint = "0;";
            polyType = this.GetPolyTypeFromEGI(section);
            int linearType = this.GetLinearType(polyType);
            polyCounter = this.GetPolyCounterFromEGI(section);
            int.TryParse(polyCounter, out int valueCounter);

            if (valueCounter != 0)
            {
                for (int count = 1; count <= valueCounter; count++)
                {
                    string polyPntX = EGIIniFile.GetStringValue(section, "PolyPntX_" + count.ToString("0000")) + KD.StringTools.Const.Comma;
                    string polyPntY = EGIIniFile.GetStringValue(section, "PolyPntY_" + count.ToString("0000")) + KD.StringTools.Const.Comma;
                    string polyPntZ = EGIIniFile.GetStringValue(section, "PolyPntZ_" + count.ToString("0000")) + KD.StringTools.Const.Comma;

                    polyPoints += polyPntX + polyPntY + polyPntZ + typePoint;
                }
                polyPoints = polyPoints.Substring(0, polyPoints.Length - 1);
                this.CurrentAppli.Scene.SceneAddShape(polyPoints);
                
                bool isPlace = false;
                string linearReferences = String.Empty;
                foreach (string catalog in catalogsList)
                {
                    string manufacturerCat = this.CurrentAppli.CatalogGetCustomInfo(catalog, "MANUFACTURER");
                    if (!String.IsNullOrEmpty(manufacturerCat) && manufacturer.Equals(manufacturerCat))
                    {                       
                        string chapterList = this.CurrentAppli.CatalogGetSectionsList(catalog, false, "@r" + KD.CharTools.Const.Comma);
                        int valuechapter = chapterList.Split(KD.CharTools.Const.Comma).Length;                       
                        string blockNb = this.CurrentAppli.CatalogGetBlocksList(catalog, valuechapter, false, "@r" + KD.CharTools.Const.Comma);                       

                        if (blockNb.EndsWith(KD.StringTools.Const.Comma))
                        {
                            blockNb = blockNb.Remove(blockNb.Length -1, 1);
                        }
                        string[] blockSplit = blockNb.Split(KD.CharTools.Const.Comma);
                        string blockList = blockSplit[blockSplit.Length - 1];
                        int.TryParse(blockList, out int valueBlockList);
                        for (int block = 0; block < valueBlockList; block++)
                        {
                            string referencesList = this.CurrentAppli.CatalogGetArticlesList(catalog, block, false, "@n" + KD.CharTools.Const.SemiColon);
                            linearReferences += referencesList;
                        }
                                              
                        string[] linearReferenceList = linearReferences.Split(KD.CharTools.Const.SemiColon);                       
                        if (linearReferenceList.Length > 0)
                        {
                            foreach (string linearReference in linearReferenceList)
                            {
                                if (linearReference.StartsWith(reference))
                                {                                   
                                    isPlace = this.CurrentAppli.Scene.EditPlaceLinearObject(catalog, linearReference, "0", true);
                                    break;
                                }
                            }
                        }
                        this.SetReference();
                        if (isPlace) { break; }
                    }
                }
                this.CurrentAppli.Scene.SceneDeleteAllShapes();
            }
        }
      
        private void PlaceArticlesInScene()
        {
            #region //INFO
            //placer les objets
            //Manufacturer = 35            
            //RefNo = 1844
            //RefPos = 1.0
            //Shape = 1             
            #endregion
            foreach (string articleSection in this.ArticleSectionsList())
            {               
                manufacturer = this.GetManufacturerFromEGI(articleSection);
                reference = this.GetReferenceFromEGI(articleSection);
                constructionType = this.GetConstructionFromEGI(articleSection);
                hingeType = this.GetHingeFromEGI(articleSection);
                this.SetPositionsFromEGI(articleSection);
                this.SetDimensionsFromEGI(articleSection);
                this.SetAngleFromEGI(articleSection);

                polyType = this.GetPolyTypeFromEGI(articleSection);

                this.SetReference();
                if (HasPolytype(articleSection))// && polyType != "2")
                {
                    this.PlaceLinearArticle(articleSection);
                }
                else
                {
                    this.PlaceArticle(articleSection);
                }
                
                this.ResetReference();
            }
        }

        private void ImportTerminate()
        {
            MessageBox.Show("Import EGI terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ImportNoValid()
        {
            MessageBox.Show("Import EGI non effectué.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class Translate
    {
        private string _lng;
        public string Language
        {
            get
            {
                return _lng;
            }
            set
            {
                _lng = value;
            }
        }

        public Translate(string lng)
        {
            this._lng = lng;
        }

        public string TextLanguageDir(string textDir)
        {
            if (System.IO.Directory.Exists(System.IO.Path.Combine(textDir, this.Language)))
            {
                return (Path.Combine(textDir, this.Language));
            }
            return string.Empty;
        }
        public bool IsMessagesFileExist(string textDir, string messagesFile)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(textDir, messagesFile)))
            {
                return true;
            }
            return false;
        }

        public string PluginFunctionLanguageTranslate()
        {
            string translate = string.Empty;
            switch (this.Language)
            {
                case "FRA":
                    translate = "Murs et Articles (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "ENG":
                    translate = "Walls and Articles (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "ESP":
                    translate = "Paredes y artículos (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "DEU":
                    translate = "Wände und Artikel (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                default:
                    translate = "Walls and Articles (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
            }
            return translate;
        }

        public string OpenCatalogLanguageTranslate()
        {
            string translate = string.Empty;
            switch (this.Language)
            {
                case "FRA":
                    translate = "Vous devez ouvrir un catalogue !";
                    break;
                case "ENG":
                    translate = "You must open a catalog!";
                    break;
                case "ESP":
                    translate = "¡Debes abrir un catálogo!";
                    break;
                default:
                    translate = "You must open a catalog!";
                    break;
            }
            return translate;
        }

        public string ArticleTableLanguageTranslate()
        {
            string translate = string.Empty;
            switch (this.Language)
            {
                case "FRA":
                    translate = "Vous devez être dans la table article.";
                    break;
                case "ENG":
                    translate = "You must be on article table.";
                    break;
                case "ESP":
                    translate = "Debe estar en la tabla de artículos.";
                    break;
                default:
                    translate = "You must be on article table.";
                    break;
            }
            return translate;
        }
    }
}
