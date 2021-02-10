using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------

using KD.Plugin;
using KD.Model;


namespace TT.Import.EGI
{
    public class Plugin : PluginBase
    {
        private string _language = string.Empty;

        private MainForm mainForm = null;

        private Translate translate = null;
        private readonly MainAppMenuItem InSituMenuItem = null;
        private KD.Config.IniFile CurrentFileEGI = null;
        private ManageCatalog manageCatalog = null;
        private GlobalSegment globalSegment = null;
        private WallSegment wallSegment = null;
        private DoorSegment doorSegment = null;
        public WindowSegment windowSegment = null;
        private RecessSegment recessSegment = null;
        //private HindranceSegment hindranceSegment = null;
        private ArticleSegment articleSegment = null;
   
        public static double sceneDimX = 0.0;
        public static double sceneDimY = 0.0;
        public static string strSceneDimZ = String.Empty;
        public static double sceneDimZ = 0.0;
        public static double angleScene = 0.0;

        public static List<String> notPlacedArticleList = new List<string>(0);
        public static Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>();

        List<string> wallSectionList = new List<string>();
        List<string> doorSectionList = new List<string>();
        List<string> windowSectionList = new List<string>();
        List<string> recessSectionList = new List<string>();
        List<string> articleSectionList = new List<string>();
        private int allSectionsCount = 0;

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
      

        public Plugin() : base(true)
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
        
        private void Initialize()
        {
            FileEGI fileEGI = new FileEGI(this.CurrentAppli);
            this.CurrentFileEGI = fileEGI.Initialize();

            if (this.CurrentFileEGI != null)
            {
                this.SetSceneDimensions();

                if (manageCatalog == null)
                {
                    manageCatalog = new ManageCatalog(this.CurrentAppli);
                }

                if (globalSegment == null)
                {
                    globalSegment = new GlobalSegment(this.CurrentFileEGI);
                }

                string version = globalSegment.GetVersion();
                SetSceneReference(version);

                this.ClearAllSectionsList();
                this.SetAllSectionsList();
            }
        }

        public void Main(int iCallParamsBlock)
        {
            this.Initialize();

            if (this.CurrentFileEGI != null)
            {
                if (mainForm == null)
                {
                    mainForm = new MainForm(this);
                }
                mainForm.ShowDialog(this.CurrentAppli.GetNativeIWin32Window());
            }

        }
        public long Execute(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (this.CurrentFileEGI != null)
            {
                Cursor.Current = Cursors.WaitCursor;                

                this.PlaceWallsInScene();
                this.PlaceDoorsInScene();
                this.PlaceWindowsInScene();
                this.PlaceRecessInScene();
                //this.PlaceHindrancesInScene();
                this.PlaceArticlesInScene();
                this.EnableUnitFloorDetails();
                this.ResetReference();
               
                //this.TerminateMessage();
            }
            else
            {
                this.NoValidMessage();
            }

            Cursor.Current = Cursors.Arrow;

            return 100;
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
        private void ClearAllSectionsList()
        {
            wallSectionList.Clear();
            doorSectionList.Clear();
            windowSectionList.Clear();
            recessSectionList.Clear();
            articleSectionList.Clear();
        }
        private void SetAllSectionsList()
        {
            wallSectionList.AddRange(this.TypeSectionsList(SegmentName.Wall_));
            doorSectionList.AddRange(this.TypeSectionsList(SegmentName.Door_));
            windowSectionList.AddRange(this.TypeSectionsList(SegmentName.Window_));
            recessSectionList.AddRange(this.TypeSectionsList(SegmentName.Recess_));
            articleSectionList.AddRange(this.TypeSectionsList(SegmentName.Article_));

            allSectionsCount = wallSectionList.Count + doorSectionList.Count + windowSectionList.Count + recessSectionList.Count + articleSectionList.Count;
        }

        private List<string> TypeSectionsList(string type)
        {
            List<string> sectionsList = CurrentFileEGI.GetSectionsNames();
            List<string> list = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith(type))
                {
                    list.Add(section);
                }
            }
            return list;
        }

        private void SetSceneDimensions()
        {
            ScnDimX = this.CurrentAppli.SceneDimX;
            ScnDimY = this.CurrentAppli.SceneDimY;
            ScnDimZ = 0;
        }
        public static void SetSceneReference(string version)
        {
            switch (version.ToUpper())
            {
                case ItemValue.V1_50: //DISCAC
                    sceneDimX = -ScnDimX  / 2;
                    sceneDimY = ScnDimY / 2;                   
                    sceneDimZ = 0; 
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case ItemValue.V1_51: //FBD , BAUFORMAT
                    sceneDimX = -ScnDimX / 2;
                    sceneDimY = -ScnDimY / 2;
                    sceneDimZ = 0; 
                    angleScene = (0 * System.Math.PI) / 180; 
                    break;
                default: // V1_51
                    sceneDimX = -ScnDimX / 2;
                    sceneDimY = -ScnDimY / 2;
                    sceneDimZ = 0;
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
            }
            
        }
       
        public void SetReference()
        {
            this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);
        }
        private void ResetReference()
        {
            this.CurrentAppli.SceneComponent.ResetSceneReference();
        }     

        public void PlaceWallsInScene()
        { 
            foreach (string wallSection in wallSectionList)
            {              

                wallSegment = new WallSegment(this, this.CurrentFileEGI, wallSection);
                wallSegment.Add();

                mainForm.SetProgressBar(wallSection,  allSectionsCount);
               
            }
        }
        private void PlaceDoorsInScene()
        {
            foreach (string doorSection in doorSectionList)
            {
                doorSegment = new DoorSegment(this, this.CurrentFileEGI, doorSection);
                doorSegment.Add();

                mainForm.SetProgressBar(doorSection, allSectionsCount);
            }
        }
        private void PlaceWindowsInScene()
        {
            foreach (string windowSection in windowSectionList)
            {
                windowSegment = new WindowSegment(this, this.CurrentFileEGI, windowSection);
                windowSegment.Add();

                mainForm.SetProgressBar(windowSection, allSectionsCount);
            }
        }
        private void PlaceRecessInScene()
        {
            foreach (string recessSection in recessSectionList)
            {
                recessSegment = new RecessSegment(this, this.CurrentFileEGI, recessSection);
                recessSegment.Add();

                mainForm.SetProgressBar(recessSection, allSectionsCount);
            }
        }
        //private void PlaceHindranceInScene()
        //{
        //    foreach (string hindranceSection in this.TypeSectionsList(SegmentName.Hindrance_))
        //    {
        //        hindranceSegment = new DoorSegment(this, this.CurrentFileEGI, hindranceSection);
        //        hindranceSegment.Add();
        //    }
        //}
        private void PlaceArticlesInScene()
        {
            #region //INFO
            //placer les objets
            //Manufacturer = 35            
            //RefNo = 1844
            //RefPos = 1.0
            //Shape = 1             
            #endregion

            Plugin.notPlacedArticleList.Clear();
            Plugin.articleAlreadyPlacedDict.Clear();

            foreach (string articleSection in articleSectionList)
            {
                mainForm.SetProgressBar(articleSection, allSectionsCount);

                articleSegment = new ArticleSegment(this, this.CurrentFileEGI, articleSection, manageCatalog);
                SegmentClassification segmentClassification = new SegmentClassification(articleSection, this.CurrentFileEGI);
                List<string> catalogsList = manageCatalog.CatalogsByManufacturerList(articleSegment.Manufacturer);

                if (segmentClassification.HasSectionPolytype())
                {
                    articleSegment.AddLinear(articleSection, catalogsList);
                }
                else
                {
                    articleSegment.Add(articleSection, catalogsList);
                }

                this.ResetReference();
                //mainForm.SetProgressBar(articleSection, allSectionsCount);
            }

            if (articleSegment != null)
            {
                articleSegment.NoPlacedArticleMessage();
            }
        }

        //Method to enable feet of unit whose on floor
        private void EnableUnitFloorDetails()
        {
            Articles articles = this.CurrentAppli.GetArticleList(KD.Analysis.FilterArticle.filterToGetArticleByCodeValidPlaced(manageCatalog.catalogCode));
            foreach (Article article in articles)
            {
                SegmentClassification segmentClassification = new SegmentClassification(article);
                if ((article.Type == (int)KD.SDK.SceneEnum.ObjectType.LINEAR) && segmentClassification.IsArticlePlinth())
                {
                    string articleOverlapping = this.CurrentAppli.SceneComponent.ObjectGetOverlappingObjectsList(article.ObjectId, false);
                    string[] articleOverlappingIDs = articleOverlapping.Split(KD.CharTools.Const.Comma);
                    foreach (string id in articleOverlappingIDs)
                    {
                        Article articleToDelDetails = new Article(this.CurrentAppli, id);
                        segmentClassification = new SegmentClassification(articleToDelDetails);
                        if (segmentClassification.IsArticleUnitFloor())
                        {
                            this.CurrentAppli.Scene.ObjectSetInfo(Convert.ToInt32(id), KD.StringTools.Const.Zero, KD.SDK.SceneEnum.ObjectInfo.HASVISIBLEDETAILS);
                        }
                    }
                }
            }
        }

        //private void TerminateMessage()
        //{
        //    MessageBox.Show("Import EGI terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        private void NoValidMessage()
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
                    translate = "Plan (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "ENG":
                    translate = "Plan (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "ESP":
                    translate = "Plan (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                case "DEU":
                    translate = "Planen (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
                    break;
                default:
                    translate = "Plan (.EGI)...";// + KD.CharTools.Const.Tab + "Ctrl+...";
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
