using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

// ---------------------------------------
// for KD self registration via RegAsm
using System.Runtime.InteropServices;
// ---------------------------------------

using KD.Plugin;


namespace TT.Import.EGI
{
    public class Plugin : KD.Plugin.PluginBase
    {
        private string _language = string.Empty;

        private Translate translate = null;
        private KD.Plugin.MainAppMenuItem InSituMenuItem = null;
        private KD.Config.IniFile CurrentFileEGI = null;
        private ManageCatalog manageCatalog = null;
        private GlobalSegment globalSegment = null;
        private WallSegment wallSegment = null;
        private ArticleSegment articleSegment = null;

        public static double sceneDimX = 0.0;
        public static double sceneDimY = 0.0;
        public static string strSceneDimZ = String.Empty;
        public static double sceneDimZ = 0.0;
        public static double angleScene = 0.0;

        public static List<String> notPlacedArticleList = new List<string>(0);
        public static Dictionary<int, string> articleAlreadyPlacedDict = new Dictionary<int, string>();

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
        

        public void Main(int iCallParamsBlock)
        {
            ScnDimX = this.CurrentAppli.SceneDimX;
            ScnDimY = this.CurrentAppli.SceneDimY;
            ScnDimZ = 0;

            manageCatalog = new ManageCatalog(this.CurrentAppli);
            FileEGI fileEGI = new FileEGI(this.CurrentAppli);
           
            this.CurrentFileEGI = fileEGI.Initialize();            

            if (this.CurrentFileEGI != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                globalSegment = new GlobalSegment(this.CurrentFileEGI);
                string version = globalSegment.GetVersion();
                SetSceneReference(version);

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

        private List<string> WallSectionsList()
        {
            List<string> sectionsList = CurrentFileEGI.GetSectionsNames();
            List<string> wallList = new List<string>(0);

            foreach (string section in sectionsList)
            {
                if (section.StartsWith(SegmentName.Wall_))
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
                if (section.StartsWith(SegmentName.Article_))
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
                case GlobalSegment.V1_50: //Discac
                    sceneDimX = -ScnDimX  / 2;
                    sceneDimY = ScnDimY / 2;
                    //strSceneDimZ = this.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMZ);
                    sceneDimZ = 0; // Convert.ToDouble(strSceneDimZ);
                    angleScene = (0 * System.Math.PI) / 180;
                    break;
                case GlobalSegment.V1_51: //FBD , Bauformat
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
       
        public void SetReference()
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
                wallSegment = new WallSegment(this, this.CurrentFileEGI, wallSection);               
                wallSegment.Add();                              
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

            Plugin.notPlacedArticleList.Clear();
            Plugin.articleAlreadyPlacedDict.Clear();

            foreach (string articleSection in this.ArticleSectionsList())
            {
                //if (articleSection != "Article_0034")
                //{
                //    continue;
                //}
                articleSegment = new ArticleSegment(this, this.CurrentFileEGI, articleSection, manageCatalog);
                List<string> catalogsList = manageCatalog.CatalogsByManufacturerList(articleSegment.Manufacturer);
              
                if (articleSegment.HasPolytype())
                {
                   
                   
                        articleSegment.PlaceLinearArticle(articleSection, catalogsList);
                    
                }
                else
                {
                    articleSegment.PlaceArticle(articleSection, catalogsList);
                }

                this.ResetReference();
            }

            articleSegment.NoPlacedArticleMessage();
        }      
  
        private void TerminateMessage()
        {
            MessageBox.Show("Import EGI terminé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
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
