using KD.Model;

namespace TT.Import.EGI
{
    public class WallSegment
    {
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

        private Wall _wall = null;
        private Segment _segment = null;

        private string shapeWallPoint = "0,0,0,0;1000,150,2500,0";

        public Wall Wall
        {
            get
            {
                return _wall;
            }
            set
            {
                _wall = value;
            }
        }
        public Segment Segment
        {
            get
            {
                return _segment;
            }
            set
            {
                _segment = value;
            }
        }

        public WallSegment(KD.SDKComponent.AppliComponent currentAppli, Segment segment)
        {
            _currentAppli = currentAppli;
            _segment = segment;
        }

        public void Add()
        {
            this.Place();
            this.SetDimensions();
            //this.SetReference();
            this.SetPositions();
            this.SetAngle();
            //this.ResetReference();
        }

        private void Place()
        {
            int wallId = this.CurrentAppli.Scene.EditPlaceWalls((int)this.Segment.DimensionY, (int)this.Segment.DimensionZ, shapeWallPoint);

            _wall = new Wall(this.CurrentAppli, wallId);
            //_wall = new Wall(this.CurrentAppli.ActiveArticle);
        }
        private void SetDimensions()
        {
            this.Wall.DimensionX = this.Segment.DimensionX; // w1;
            this.Wall.DimensionY = this.Segment.DimensionY;
            this.Wall.DimensionZ = this.Segment.DimensionZ;
        }
        private void SetPositions()
        {
            this.Wall.PositionX = this.Segment.PositionX;
            this.Wall.PositionY = this.Segment.PositionY;
            this.Wall.PositionZ = this.Segment.PositionZ;
        }
        private void SetAngle()
        {
            this.Wall.AngleOXY = this.Segment.AngleZ;
        }

        //private void SetReference()
        //{
        //    this.CurrentAppli.Scene.SceneSetReference((int)sceneDimX, (int)sceneDimY, (int)sceneDimZ, angleScene);
        //}
        //private void ResetReference()
        //{
        //    this.CurrentAppli.SceneComponent.ResetSceneReference();
        //}
    }
}
