
namespace Ord_Eancom
{
    public class OrderEnum
    {
        public enum Type
        {
            EDIGRAPH = 1,
            FLOOR_PLAN = 11,
            WALL_FRONT_VIEW = 21,
            TILE_PLAN = 22,
            INSTALLATION_PLAN = 23,
            PERSPECTIVE = 31,
            PLINTH_SKETCH = 41,
            WORKTOP_SKETCH = 42,
            WALL_SEALING_PROFILE_SKETCH = 43,
            LIGHT_PELMET_SKETCH = 44,
            CORNICE_SKETCH = 45,
            FLOORING_SKETCH = 46,
            OTHER = 99
        }

        public enum Format
        {
            EDIGRAPH = 1,
            JPEG = 11,
            PDF = 50,
            Others = 99
        }
    }
}
