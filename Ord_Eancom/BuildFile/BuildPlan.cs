using KD.SDKComponent;


namespace Ord_Eancom
{
    public class BuildPlan
    {
        BuildCommon _buildCommon = null;

        AppliComponent _currentAppli = null;
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

        public BuildPlan(AppliComponent currentAppli, BuildCommon buildCommon)
        {
            _currentAppli = currentAppli;
            _buildCommon = buildCommon;           
        }

        public void Generate()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = _buildCommon.GetView();
            _buildCommon.SetView(KD.SDK.SceneEnum.ViewMode.TOP);
            _buildCommon.ZoomAdjusted();
            _buildCommon.ExportImageJPG(1, OrderTransmission.PlanName);
            _buildCommon.SetView(currentViewMode);
        }
       
    }
}
