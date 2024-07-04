using PhysicsBenchmark.DOTS.Classic.Spawner;
using PhysicsBenchmark.Settings;

namespace PhysicsBenchmark.Helpers
{
    public static class CubeCountHelper
    {
        public static int GetPerimeterNumber(int x, int y)
        {
            if (x == 1)
                return y;
            
            return y * 4 * (x - 1);
        }
        
        public static int GetNumber()
        {
            var result = GetPerimeterNumber(ClassicSettings.length, ClassicSettings.height);

            return ClassicSettings.formationIdentifier == FormationIdentifier.Perimeter
                ? result
                : result / 2;
        }
    }
}