using PhysicsBenchmark.DOTS.Classic.Spawner;

namespace PhysicsBenchmark.Settings
{
    public static class ClassicSettings
    {
        public static int height = 5;
        public static float angle = 15f;
        public static float heightOffset = 5f;
        public static bool enableSphere = false;
        public static FormationIdentifier formationIdentifier = FormationIdentifier.Perimeter;
        
        
        public static int length
        {
            get => formationIdentifier == FormationIdentifier.Checkerboard ? (2 * _internalLenght) - 1 : _internalLenght;
            set => _internalLenght = value;
        }
        private static int _internalLenght = 5;
    }
}