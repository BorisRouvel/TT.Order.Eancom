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
        private string _version = String.Empty;
      

        private Translate translate = null;
        private KD.Plugin.MainAppMenuItem InSituMenuItem = null;
        private KD.Config.IniFile CurrentFileEGI = null;
        private ManageCatalog manageCatalog = null;

        private const string Dir = "orders";
        //private const string ManufacturerCustomFromCatalog = "MANUFACTURER";
        //private const string FilterEGIFile = "Fichiers egi (*.egi)|*.egi";
        //private const string V1_50 = "V1.50"; // Discac
        //private const string V1_51 = "V1.51"; // FBD , Bauformat
        //private const string RefPointFormat = "0000";

        //public static string orderDir = String.Empty;

    

        public static double sceneDimX = 0.0;
        public static double sceneDimY = 0.0;
        public static string strSceneDimZ = String.Empty;
        public static double sceneDimZ = 0.0;
        public static double angleScene = 0.0;

        //private string xb = String.Empty;
        //private string yb = String.Empty;
        //private string zb = String.Empty;
        //private string x = String.Empty;
        //private string y = String.Empty;
        //private string z = String.Empty;
        //private string wb = String.Empty;
        //private string db = String.Empty;
        //private string hb = String.Empty;
        //private string w = String.Empty;
        //private string d = String.Empty;
        //private string h = String.Empty;
        //private string ab = String.Empty;
        //private double a = 0.0;

        //private string wb1 = String.Empty;
        //private string wb2 = String.Empty;
        //private string wb3 = String.Empty;
        //private string db1 = String.Empty;
        //private double wB1 = 0.0;
        //private double wB2 = 0.0;
        //private double wB3 = 0.0;
        //private double dB1 = 0.0;

        //private double x1 = 0.0;
        //private double y1 = 0.0;
        //private double z1 = 0.0;
        //private double w1 = 0.0;
        //private double d1 = 0.0; 
        //private double h1 = 0.0;

        //private double angleFilerWidth = 0.0;
        //private double angleFilerDepth = 0.0;

        //private string manufacturer = String.Empty;        
        //private string reference = String.Empty;
        //private string constructionType = String.Empty;
        //private string refNo = String.Empty;
        //private string refPos = String.Empty;
        //private string polyType = String.Empty;
        //private string polyCounter = String.Empty;
        //private string polyPntX = String.Empty;
        //private string polyPntY = String.Empty;
        //private string polyPntZ = String.Empty;
        //private string manageCatalog.CatalogManufacturer = String.Empty;
        

        //private int hingeType = 0;

        List<String> notPlacedArticleList = new List<string>(0);
        Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>();

        int CallParamsBlock = KD.Const.UnknownId;

        static double ScnDimX;
        static double ScnDimY;
        static double ScnDimZ;

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


        public Plugin()
           : base(true)
        {            
            this._language = this.CurrentAppli.GetLanguage();
            this.translate = new Translate(this.Language);

            this.InSituMenuItem = new MainAppMenuItem((int)KD.SDK.AppliEnum.FileMenuItemsId.IMPORT_WMF, translate.PluginFunctionLanguageTranslate(),
                this.AssemblyFileName, KD.Plugin.Const.PluginClassName, "Main");
                   
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
                this.CallParamsBlock = iCallParamsBlock;
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
            //orderDir = Path.Combine(this.CurrentAppli.ExeDir, Dir);
            return true;
        }


        public void Main(int iCallParamsBlock)
        {
            ScnDimX = this.CurrentAppli.SceneDimX;
            ScnDimY = this.CurrentAppli.SceneDimY;
            ScnDimZ = 0;

            manageCatalog = new ManageCatalog(this.CurrentAppli);
            FileEGI fileEGI = new FileEGI(this.CurrentAppli);
           
            this.CurrentFileEGI = fileEGI.Initialize();
            Cursor.Current = Cursors.WaitCursor;

            if (CurrentFileEGI != null)
            {
                Segment segment = new Segment(this.CurrentFileEGI, String.Empty);
                _version = segment.GetVersion();
                SetSceneReference(this.Version);

                this.PlaceWallsInScene();
                this.PlaceArticlesInScene();

                this.ResetReference();
                this.TerminateMessage();
            }
            else
            {
                this.NoValidMessage();
            }
           
            
            Cursor.Current = Cursors.Arrow;
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
        //private string GetVersion()
        //{
        //    string version = CurrentFileEGI.GetStringValue(Segment.Global, ItemKey.Version);

        //    if (!String.IsNullOrEmpty(version))
        //    {
        //        if (version.Split(KD.CharTools.Const.Underscore).Length > 1)
        //        {
        //            return version.Split(KD.CharTools.Const.Underscore)[1];
        //        }
        //    }

        //    return String.Empty;
        //}
        private List<string> WallSectionsList()
        {
            List<string> sectionsList = CurrentFileEGI.GetSectionsNames();
            List<string> wallList = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith(Segment.Wall_))
                {
                    wallList.Add(section);
                }
            }
            return wallList;
        }
        private List<string> ArticleSectionsList()
        {
            List<string> sectionsList = CurrentFileEGI.GetSectionsNames();
            List<string> articleList = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith(Segment.Article_))
                {
                    articleList.Add(section);
                }
            }
            return articleList;
        }

        public static void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case Segment.V1_50: //Discac
                    sceneDimX = -ScnDimX  / 2;
                    sceneDimY = ScnDimY / 2;
                    //strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case Segment.V1_51: //FBD , Bauformat
                    sceneDimX = -ScnDimX / 2;
                    sceneDimY = -ScnDimY / 2;
                    //strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180; 
                    break;
                default:
                    sceneDimX = -ScnDimX / 2;
                    sceneDimY = ScnDimY / 2;
                    //strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
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

  

        private void PlaceWallsInScene()
        {
            foreach (string wallSection in this.WallSectionsList())
            {
                Segment segment = new Segment(this.CurrentFileEGI, wallSection);
                segment.SetWallItems();

                WallSegment wallSegment = new WallSegment(this.CurrentAppli, segment);
                this.SetReference();
                wallSegment.Add();
                this.ResetReference();                
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

            notPlacedArticleList.Clear();
            articleAlreadyPlacedDict.Clear();

            foreach (string articleSection in this.ArticleSectionsList())
            {
                Segment segment = new Segment(this.CurrentFileEGI, articleSection);
                segment.SetArticleItems();
                              
                List<string> catalogsList = manageCatalog.CatalogsByManufacturerList(segment.Manufacturer);

                this.SetReference();
                if (segment.HasPolytype())// && polyType != "2")
                {
                    this.PlaceLinearArticle(segment, articleSection, catalogsList);
                }
                else
                {
                    this.PlaceArticle(segment, articleSection, catalogsList);
                }

                this.ResetReference();
            }

            this.NoPlacedArticleMessage();
        }


        //private IEnumerable<string> CatalogsBaseList()
        //{                           
        //    return Directory.EnumerateFiles(this.CurrentAppli.CatalogDir, KD.StringTools.Const.Wildcard + KD.IO.File.Extension.Cat);
        //}
        //private string GetCatalogCustomInfo(string catalog, string info)
        //{
        //    return this.CurrentAppli.CatalogGetCustomInfo(catalog, info);
        //}
        //private List<string> CatalogsByManufacturerList()
        //{           
        //    List<string> list = new List<string>();
        //    list.Clear();

        //    foreach (string catalogPath in this.CatalogsBaseList())
        //    {
        //        string manufacturerCat = this.GetCatalogCustomInfo(catalogPath, ManufacturerCustomFromCatalog);

        //        if (!String.IsNullOrEmpty(manufacturerCat) && manufacturer.Equals(manufacturerCat))
        //        {
        //            list.Add(catalogPath);
        //        }
        //    }
        //    return list;
        //}
        //private List<string> CatalogsByFirst4LettersList(List<string> catalogPathList)
        //{
        //    string first4LettersBase = String.Empty;
        //    List<string> list = new List<string>();
        //    list.Clear();

        //    string catalogBase = Path.GetFileName(catalogPathList[0]);
        //    if (catalogBase.Length > 3)
        //    {
        //        first4LettersBase = catalogBase.Substring(0, 4);
        //    }

        //    foreach (string catalogPath in catalogPathList)
        //    {
        //        string catalog = Path.GetFileName(catalogPath);
        //        if (!String.IsNullOrEmpty(first4LettersBase) && catalog.Length > 3)
        //        {
        //            string first4Letters = catalog.Substring(0, 4);
        //            if (first4LettersBase.Equals(first4Letters))
        //            {
        //                list.Add(catalogPath);
        //            }                  
        //        }
        //    }
        //    return list;
        //}
        //private string CatalogsByDateList(List<string> catalogsPathList)
        //{
        //    string lastCatalog = String.Empty;
        //    int lastDate = 0;

        //    foreach (string catalogPath in catalogsPathList)
        //    {              
        //        int date = this.CurrentAppli.CatalogGetModificationTime(catalogPath);

        //        if (date > lastDate)
        //        {
        //            lastDate = date;
        //            lastCatalog = catalogPath;
        //        }               
        //    }
        //    return lastCatalog;
        //}

        //private string GetLastManufacturerFromCatalogs(List<string> catalogsPathList)
        //{
        //    List<string> catalogsFirst4LettersList = new List<string>();
        //    List<string> catalogsManufacturerList = catalogsPathList;

        //    if (catalogsManufacturerList.Count > 0)
        //    {
        //        catalogsFirst4LettersList.Clear();
        //        catalogsFirst4LettersList = this.CatalogsByFirst4LettersList(catalogsManufacturerList);
        //    }

        //    string lastManufacturerDateCatalog = String.Empty;           

        //    if (catalogsFirst4LettersList.Count > 0)
        //    {                
        //        lastManufacturerDateCatalog = this.CatalogsByDateList(catalogsFirst4LettersList);
        //    }

        //    return lastManufacturerDateCatalog;
        //}
        //private void SetLastManufacturerCatalog(List<string> catalogsPathList)
        //{            
        //     manageCatalog.CatalogManufacturer = this.GetLastManufacturerFromCatalogs(catalogsPathList);          
        //}

        //private bool HasPolytype(string section)
        //{
        //   if (!String.IsNullOrEmpty(CurrentFileEGI.GetStringValue(section, ItemKey.PolyType)))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //private bool HasMeasureT1(string section)
        //{
        //    if (!String.IsNullOrEmpty(CurrentFileEGI.GetStringValue(section, ItemKey.Measure_T1)))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //private bool IsAngle(string section)
        //{
        //    string value = CurrentFileEGI.GetStringValue(section, ItemKey.Shape);
        //    if (!String.IsNullOrEmpty(value))
        //    {
        //        if (value == "27")
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsCorner(string section)
        //{
        //    string value = CurrentFileEGI.GetStringValue(section, ItemKey.Shape);
        //    if (!String.IsNullOrEmpty(value))
        //    {
        //        if (value == "20")
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private string GetPolyTypeFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.PolyType);
        //}
        //private string GetPolyCounterFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.PolyCounter);
        //}
        //private string GetManufacturerFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.Manufacturer);
        //}
        //private string GetReferenceFromEGI(string section)
        //{
        //    //string name = EGIIniFile.GetStringValue(section, ItemKey.Name_);
        //    //if (!String.IsNullOrEmpty(name))
        //    //{
        //    //    return name;
        //    //}
        //    //else
        //    //{ 
        //        return CurrentFileEGI.GetStringValue(section, ItemKey.Name);
        //    //}
        //}
        //private string GetConstructionFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.ConstructionType);
        //}
        //private int GetHingeFromEGI(string section)
        //{
        //    hingeType = 0;
        //    string hinge = CurrentFileEGI.GetStringValue(section, ItemKey.Hinge);

        //    if (String.IsNullOrEmpty(hinge))
        //    {
        //        hinge = constructionType;
        //    }
        //    switch (hinge)
        //    {
        //        case ItemValue.Left_Hinge:
        //            return 1;
        //        case ItemValue.Right_Hinge:
        //            return 2;
        //        default:
        //            return 0;
        //    }
        //}
        //private string GetRefNoFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.RefNo);
        //}
        //private string GetRefPosFromEGI(string section)
        //{
        //    return CurrentFileEGI.GetStringValue(section, ItemKey.RefPos);
        //}
        //private Article PlaceObject()
        //{
        //    this.SetReference();

        //    int objectID = this.CurrentAppli.Scene.EditPlaceObject(manageCatalog.CatalogManufacturer, reference, hingeType, (int)w1, (int)d1, (int)h1,
        //           (int)x1, (int)y1, (int)z1, 0, a, false, false, false);

        //    if (objectID.Equals(KD.Const.UnknownId))
        //    {
        //        this.SetReference();

        //        objectID = this.CurrentAppli.Scene.EditPlaceObject(manageCatalog.CatalogManufacturer, reference, 0, (int)w1, (int)d1, (int)h1,
        //           (int)x1, (int)y1, (int)z1, 0, a, false, false, false);
        //    }

        //    if (!objectID.Equals(KD.Const.UnknownId))
        //    {
        //        return new Article(this.CurrentAppli, objectID);
        //    }
        //    return null;
        //}
        //private Article PlaceComponentObject()
        //{
        //    if (refPos.Contains(KD.StringTools.Const.Dot))
        //    {
        //        string refBase = refPos.Split(KD.CharTools.Const.Dot)[0];

        //        foreach (KeyValuePair<int, string> kvp in articleAlreadyPlacedDict)
        //        {
        //            if (kvp.Value.Equals(refBase))
        //            {
        //                Article parent = new Article(this.CurrentAppli, kvp.Key);
        //                Articles childs = parent.GetChildren(FilterArticle.strFilterToGetValidNotPlacedHostedAndChildren());
        //                foreach (Article child in childs)
        //                {
        //                    if (child.IsValid && child.Ref.Equals(reference))
        //                    {
        //                        bool find = false;  //Test with Bauformat Side panel maybe don't work for another
        //                        if (x1 == 0.0 && child.Handing == this.CurrentAppli.GetTranslatedText("G"))
        //                        {
        //                            find = true;
        //                        }
        //                        else if (x1 > 0.0 && child.Handing == this.CurrentAppli.GetTranslatedText("D"))
        //                        {
        //                            find = true;
        //                        }
        //                        else
        //                        {
        //                            find = true;
        //                        }

        //                        if (find)
        //                        {
        //                            child.IsPlaced = true;
        //                            //child.DimensionX = w1;
        //                            //child.DimensionY = d1;
        //                            child.DimensionZ = h1;
        //                            //child.PositionX = x1;
        //                            //child.PositionY = y1;
        //                            child.PositionZ = z1;
        //                            return child;
        //                        }
        //                    }
        //                }
        //            }
        //        }               
        //    }
        //    return null;
        //}
        private void PlaceArticle(Segment segment, string section, List<string> catalogsList)
        {           
            manageCatalog.SetLastManufacturerCatalog(catalogsList);          

            if (!String.IsNullOrEmpty(manageCatalog.CatalogManufacturer))
            {
                ArticleSegment articleSegment = new ArticleSegment(this.CurrentAppli, segment, manageCatalog.CatalogManufacturer);
                this.SetReference();

                Article component = articleSegment.PlaceComponentObject();
                if (component != null && component.IsValid)
                {
                    articleSegment.AddPlacedArticleDict(component, segment.RefPos);
                    return;
                }
                else
                {
                    articleSegment.PlaceObject();
                    if (articleSegment.Article != null && articleSegment.Article.IsValid)
                    {
                        articleSegment.MoveArticlePerRepere();
                        if (segment.HasMeasureT1())
                        {
                            if (segment.IsAngle())
                            {
                                segment.SetAngleDimensionsFromEGI();
                                articleSegment.SetAngleChildDimensions();
                                articleSegment.SetAnglePositionAngle();
                            }
                            if (segment.IsCorner())
                            {
                                articleSegment.SetCornerDimensions();
                            }
                            
                        }

                        if (!String.IsNullOrEmpty(segment.RefPos) && !segment.RefPos.Contains(KD.StringTools.Const.Dot))
                        {
                            articleSegment.Article.Number = Convert.ToInt16(segment.RefPos);
                        }
                        articleSegment.AddPlacedArticleDict(articleSegment.Article, segment.RefPos);
                    }                    
                    else
                    {
                        if (catalogsList.Count > 1)
                        {
                            catalogsList.Remove(manageCatalog.CatalogManufacturer);
                            this.PlaceArticle(segment, section, catalogsList);
                        }
                        else
                        {
                            notPlacedArticleList.Add(segment.Reference);
                        }
                    }
                }
            }
        }
        //private void MoveArticlePerRepere(Article article)
        //{
        //    this.CurrentAppli.SceneComponent.SetReferenceFromObject(article, false);
        //    switch (this.Version)
        //    {
        //        case Plugin.V1_50: //DISCAC
        //            article.PositionX += w1;
        //            article.AngleOXY += 180;
        //            break;
        //        case Plugin.V1_51: //FBD et Bauformat
        //            article.PositionX += w1;
        //            article.AngleOXY += 180;
        //            break;
        //        default:
        //            article.PositionX += w1;
        //            article.AngleOXY += 180;
        //            break;
        //    }
        //}
        //private void SetAngleChildDimensions(Article article)
        //{           
        //    article.DimensionY = dB1;            

        //    Articles childs = article.GetChildren(FilterArticle.strFilterToGetValidPlacedHostedAndChildren());
        //    if (childs != null && childs.Count > 0)
        //    {
        //        foreach (Article child in childs)
        //        {
        //            if (child.Name.ToUpper().Contains(this.CurrentAppli.GetTranslatedText("Fileur".ToUpper()))) //Name.StartsWith("@F")) 
        //            {
        //                child.DimensionX = d1 - dB1; //angleFilerWidth;
        //                child.DimensionY = wB1; // angleFilerDepth;                     
        //            }
        //        }               
        //    }
        //}
        //private void SetCornerDimensions(Article article)
        //{           
        //    article.DimensionX = w1 - wB1; //angleFilerWidth;
        //    article.DimensionY = d1 - dB1; // angleFilerDepth;            
        //}
        //private void SetAnglePositionAngle(Article article)
        //{
        //    List<string> firstBraceParameters = KD.StringTools.Helper.ExtractParameters(article.Script, String.Empty, KD.StringTools.Const.BraceOpen, KD.StringTools.Const.BraceClose);

        //    if (article.Handing == this.CurrentAppli.GetTranslatedText("G") && firstBraceParameters.Contains("SI".ToUpper()))
        //    {
        //        article.AngleOXY += 90.0;
        //        this.SetReference();
        //        article.PositionY += w1;
        //    }
        //}

        private int GetLinearType(string type)
        {
            PolytypeValue polytypeValue = new PolytypeValue(Convert.ToInt16(type));            
            return polytypeValue.Number();       
            
            #region // First edit
            //switch (type)
            //{
            //    case "1": //Plinthe
            //        return 0;
            //    case "2": //WorkTop
            //        return 1;
            //    case "3": //Cache lumière
            //        return 2;
            //    case "4": //Corniche
            //        return 3;
            //    case "5": //Edge workTop
            //        return 4;
            //    default:
            //        return -1;
            //}
            #endregion
        }
        private void PlaceLinearArticle(Segment segment, string section, List<string> catalogsList)
        {
            string polyPoints = String.Empty;
            string typePoint = "0;";
            //polyType = this.GetPolyTypeFromEGI(section);
            //int linearType = this.GetLinearType(polyType); // not used actually
            //polyCounter = segment.SetPolyCounterFromEGI();
            int.TryParse(segment.PolyCounter, out int valueCounter);

            if (valueCounter != 0)
            {
                for (int count = 1; count <= valueCounter; count++)
                {
                    string polyPntX = CurrentFileEGI.GetStringValue(section, ItemKey.PolyPntX + KD.StringTools.Const.Underscore + count.ToString(Segment.RefPointFormat)) + KD.StringTools.Const.Comma;
                    string polyPntY = CurrentFileEGI.GetStringValue(section, ItemKey.PolyPntY + KD.StringTools.Const.Underscore + count.ToString(Segment.RefPointFormat)) + KD.StringTools.Const.Comma;
                    string polyPntZ = CurrentFileEGI.GetStringValue(section, ItemKey.PolyPntZ + KD.StringTools.Const.Underscore + count.ToString(Segment.RefPointFormat)) + KD.StringTools.Const.Comma;

                    polyPoints += polyPntX + polyPntY + polyPntZ + typePoint;
                }
                polyPoints = polyPoints.Substring(0, polyPoints.Length - 1);
                this.CurrentAppli.Scene.SceneAddShape(polyPoints);
                
                bool isPlace = false;
                string linearReferences = String.Empty;

                manageCatalog.SetLastManufacturerCatalog(catalogsList);

                if (!String.IsNullOrEmpty(manageCatalog.CatalogManufacturer))
                {                       
                    string chapterList = this.CurrentAppli.CatalogGetSectionsList(manageCatalog.CatalogManufacturer, false, "@r" + KD.CharTools.Const.Comma);
                    int valuechapter = chapterList.Split(KD.CharTools.Const.Comma).Length;                       
                    string blockNb = this.CurrentAppli.CatalogGetBlocksList(manageCatalog.CatalogManufacturer, valuechapter, false, "@r" + KD.CharTools.Const.Comma);                       

                    if (blockNb.EndsWith(KD.StringTools.Const.Comma))
                    {
                        blockNb = blockNb.Remove(blockNb.Length -1, 1);
                    }
                    string[] blockSplit = blockNb.Split(KD.CharTools.Const.Comma);
                    string blockList = blockSplit[blockSplit.Length - 1];
                    int.TryParse(blockList, out int valueBlockList);
                    for (int block = 0; block < valueBlockList; block++)
                    {
                        string referencesList = this.CurrentAppli.CatalogGetArticlesList(manageCatalog.CatalogManufacturer, block, false, "@n" + KD.CharTools.Const.SemiColon);
                        linearReferences += referencesList;
                    }
                                              
                    string[] linearReferenceList = linearReferences.Split(KD.CharTools.Const.SemiColon);                       
                    if (linearReferenceList.Length > 0)
                    {
                        string linearRef = String.Empty;
                        foreach (string linearReference in linearReferenceList)
                        {
                            if (linearReference.StartsWith(segment.Reference))
                            {                                   
                                isPlace = this.CurrentAppli.Scene.EditPlaceLinearObject(manageCatalog.CatalogManufacturer, linearReference, KD.StringTools.Const.Zero, true);
                                linearRef = linearReference;
                                break;
                            }                            
                        }
                        if (!isPlace)
                        {
                            if (catalogsList.Count > 1)
                            {
                                catalogsList.Remove(manageCatalog.CatalogManufacturer);
                                this.PlaceLinearArticle(segment, section, catalogsList);
                            }
                            else
                            {
                                notPlacedArticleList.Add(segment.Reference);
                            }                           
                        }
                        else
                        {

                            //KD.FilterBuilder.FilterClauseDict filterBuilder = new KD.FilterBuilder.FilterClauseDict();
                            //filterBuilder.Clear();
                            //filterBuilder.Add(KD.SDK.SceneEnum.ObjectInfo.REF, linearRef);

                            //Articles linearArticles = this.CurrentAppli.GetArticleList(filterBuilder);
                            //if (linearArticles[0].ObjectId != KD.Const.UnknownId)
                            //{
                            //    this.AddPlacedArticleDict(linearArticles[0]);
                            //}
                        }
                    }
                    this.SetReference();                       
                }
                
                this.CurrentAppli.Scene.SceneDeleteAllShapes();
            }
        }
      
   

        private void NoPlacedArticleMessage()
        {
            if (notPlacedArticleList.Count > 0)
            {
                string listString = String.Empty;
                foreach (string notPlacedArticle in notPlacedArticleList)
                {
                    listString += notPlacedArticle + Environment.NewLine;
                }
                //MessageBox.Show(listString, "Import EGI : Réf. non trouvé");
            }

            if (articleAlreadyPlacedDict.Count > 0)
            {
                string listString = String.Empty;
                foreach (KeyValuePair<int,string> articleAlreadyPlaced in articleAlreadyPlacedDict)
                {
                    listString += articleAlreadyPlaced.Key.ToString() + " : " + articleAlreadyPlaced.Value.ToString() + Environment.NewLine;
                }
                //MessageBox.Show(listString, "Import EGI : Réf. posé");
            }
            
        }
        private void TerminateMessage()
        {
            MessageBox.Show("Import EGI terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void NoValidMessage()
        {
            MessageBox.Show("Import EGI non effectué.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //private void AddPlacedArticleDict(Article article, string refPos)
        //{
        //    if (article != null && article.IsValid && article.Number != KD.Const.UnknownId)
        //    {
        //        articleAlreadyPlacedDict.Add(article.ObjectId, refPos);
        //    }
        //    else if (article == null)
        //    {
        //        articleAlreadyPlacedDict.Add(Convert.ToInt32(refPos), refPos);
        //    }
        //}
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
     
    }

  

   
}
