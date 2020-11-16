using KD.SDKComponent;
using KD.Model;
using KD.Analysis;

namespace Ord_Eancom
{
    public class BuildElevation
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


        public  BuildElevation(AppliComponent currentAppli, BuildCommon buildCommon)
        {
            _currentAppli = currentAppli;
            _buildCommon = buildCommon;
        }

        public void Generate()
        {
            KD.SDK.SceneEnum.ViewMode currentViewMode = _buildCommon.GetView();

            Articles articles = this.CurrentAppli.GetArticleList(FilterArticle.FilterToGetWallByValid());
            if (articles != null && articles.Count > 0)
            {
                Walls walls = new Walls(articles);

                foreach (Wall wall in walls)
                {
                    Articles articlesAgainst = wall.AgainstMeASC;

                    if (articlesAgainst != null && articlesAgainst.Count > 0)
                    {
                        wall.IsActive = true;
                        _buildCommon.SetView(KD.SDK.SceneEnum.ViewMode.VECTELEVATION);
                        _buildCommon.ZoomAdjusted();
                        _buildCommon.ExportImageJPG(walls.IndexOf(wall) + 1, OrderTransmission.ElevName);
                    }

                }
                _buildCommon.SetView(currentViewMode);
            }
        }
    }
}
