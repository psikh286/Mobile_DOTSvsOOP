namespace PhysicsBenchmark.Helpers
{
    public static class CubeCountHelper
    {
        public static int GetPerimeterNumber(int x, int y)
        {
            return y * 4 * (x - 1);
        }
    }
}