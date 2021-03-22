
namespace Ord_Eancom
{
    public class OrderDescription
    {
        public const string EDIGRAPH = "EDIGRAPH";
        public const string FLOOR_PLAN = "Plan de base";
        public const string WALL_FRONT_VIEW = "Vue de face";
        public const string TILE_PLAN = "Plan de carrelage";
        public const string INSTALLATION_PLAN = "Plan d'installation";
        public const string PERSPECTIVE = "Perspective";
        public const string PLINTH_SKETCH = "Croquis de socle";
        public const string WORKTOP_SKETCH = "Croquis plan de travail";
        public const string WALL_SEALING_PROFILE_SKETCH = "Croquis du profil d'étanchéité murale";
        public const string LIGHT_PELMET_SKETCH = "Croquis de cache-lumière";
        public const string CORNICE_SKETCH = "Croquis de corniche";
        public const string FLOORING_SKETCH = "Croquis du sol";
        public const string OTHER = "Autre";
    }

    public class OrderKey
    {
        public const string ChoiceRetaillerDelivery = "IsChoiceRetaillerDelivery";
        public const string ChoiceCustomerDelivery = "IsChoiceCustomerDelivery";
        public const string MandatoryDeliveryCustomerInformation = "MandatoryDeliveryCustomerInformation";
        public const string MandatoryDeliveryRetailerInformation = "MandatoryDeliveryRetailerInformation";
        public const string ChoiceExportEGI = "IsChoiceExportEGI";
        public const string ChoiceExportPlan = "IsChoiceExportPlan";
        public const string ChoiceExportElevation = "IsChoiceExportElevation";
        public const string ChoiceExportPerspective = "IsChoiceExportPerspective";
        public const string ChoiceExportOrder = "IsChoiceExportOrder";
        public const string GenerateOrder = "IsGenerateOrder";
        public const string SupplierEmail = "SupplierEmail";
    }

    public class OrderConstants
    {
        public const string Insitu = "INSITU";
        public const string HandleName = "_HDL";

        public const string FormatDate_4yMd = "yyyyMMdd";
        public const string FormatDate_2yMd = "yyMMdd";
        public const string FormatDate_dMy = "ddMMyy";
        public const string FormatDate_d_M_y = "dd/MM/yyyy";
        public const string FormatDate_yW = "yyyyww";
        public const string FormatDate_y = "yyyy";
        public const string FormatTime_Hm = "HHmm";
        public const string FormatTime_H_m_s = "HH:mm:ss";

        public const int CommentSceneLinesMax = 99;
        public const int CommentSceneCharactersPerLineMax = 70;

        public const string PlinthFinishType = "7";
        public const string LeftAssemblyFinishType = "19";
        public const string RightAssemblyFinishType = "20";

        public const double FrontDepth = 20.0;
      
    }

    public static class PairingTablePosition
    {
        public const int ArticleSupplierId = 0;
        public const int ArticleSerieNo = 1;
        public const int ArticleEDPNumber = 2;
        public const int ArticleEANNumber = 3;
        public const int ArticleHinge = 4;
        public const int ArticleConstructionId = 5;
        public const int ArticleShape = 6;
        public const int ArticleWidth_B = 7;
        public const int ArticleHeight_H = 8;
        public const int ArticleDepth_T = 9;

        public const int StartArticleMeasure = 10;
        public const int ArticleWidth_B1 = 10;
        public const int ArticleWidth_B2 = 11;
        public const int ArticleWidth_B3 = 12;
        public const int ArticleWidth_B4 = 13;
        public const int ArticleWidth_B5 = 14;
        public const int ArticleWidth_Be = 15;

        public const int ArticleHeight_H1 = 16;
        public const int ArticleHeight_H2 = 17;
        public const int ArticleHeight_H3 = 18;
        public const int ArticleHeight_H4 = 19;
        public const int ArticleHeight_H5 = 20;
        public const int ArticleHeight_He = 21;

        public const int ArticleDepth_T1 = 22;
        public const int ArticleDepth_T2 = 23;
        public const int ArticleDepth_T3 = 24;
        public const int ArticleDepth_T4 = 25;
        public const int ArticleDepth_T5 = 26;
        public const int ArticleDepth_Te = 27;
        public const int EndArticleMeasure = 27;
    }

    public class OrderTransmission
    {
        public const string OrderEDIFileName = "Order.edi";
        public const string OrderEGIFileName = "Order.egi";
        public const string OrderName = "Commande";
        public const string OrderZipFileName = "Order.zip";

        public const string PlanName = "Plan";
        public const string ElevName = "Elevation";
        public const string PerspectiveName = "Perspective";

        public const string ExtensionEDI = ".edi";
        public const string ExtensionEGI = ".egi";
        public const string ExtensionZIP = ".zip";
        public const string ExtensionTXT = KD.IO.File.Extension.Txt;
        public const string ExtensionJPG = KD.IO.File.Extension.Jpg;
        public const string ExtensionPDF = KD.IO.File.Extension.Pdf;

        public const string VersionEancomOrder = "EANCOM_ORDER_V2.03";
        public const string VersionEdigraph_1_50 = "EDIGRAPH_V1.50";
        public const string VersionEdigraph_1_51 = "EDIGRAPH_V1.51";
        public const string HeaderSubject = "EDI-ORDER";

        public const string HeaderTagMandatoryDeliveryInformation = "{}";
    }
}
