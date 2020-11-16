using KD.SDKComponent;


namespace Ord_Eancom
{
    public class BuildPerspective
    {
        BuildCommon _buildCommon = null;

        AppliComponent _currentAppli = null;
        AppliComponent CurrentAppli
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

        public BuildPerspective(AppliComponent currentAppli, BuildCommon buildCommon)
        {
            _currentAppli = currentAppli;
            _buildCommon = buildCommon;
        }

        public void Generate()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = _buildCommon.GetView();

            _buildCommon.SetView(KD.SDK.SceneEnum.ViewMode.OGLREAL);
            _buildCommon.ZoomAdjusted();
            _buildCommon.ExportImageJPG(1, OrderTransmission.PerspectiveName);
            _buildCommon.SetView(currentViewMode);
        }
    }
}
