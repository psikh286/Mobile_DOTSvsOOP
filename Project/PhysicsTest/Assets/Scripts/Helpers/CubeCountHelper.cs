using PhysicsBenchmark.DOTS.Classic.Spawner;
using PhysicsBenchmark.Settings;

namespace PhysicsBenchmark.Helpers
{
    public static class CubeCountHelper
    {
        public static int GetPerimeterNumber(int x, int y)
        {
            return y * 4 * (x - 1);
        }
        
        public static int GetNumber(int lenght)
        {
            var result = GetPerimeterNumber(lenght, ClassicSettings.height);

            return ClassicSettings.formationIdentifier == FormationIdentifier.Perimeter
                ? result
                : result / 2;
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