namespace PhysicsBenchmark.Helpers
{
    public static class NumberOfCubesHelper
    {
        public static int GetPerimeterNumberNumber(int x, int y)
        {
            return y * 4 * (x - 1);
        }
    }
}