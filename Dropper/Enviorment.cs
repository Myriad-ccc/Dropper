namespace Dropper
{
    public static class Environment
    {
        public const float g = 9.81f;
        private static readonly float Pressure = 101325; //Pa
        private static readonly float GasConstant = 287; //J
        private static readonly float Temperature = 288; //K=~15C (constant)  
        public static float AirDensity =
            Pressure / (GasConstant * Temperature);      //https://en.wikipedia.org/wiki/Ideal_gas_law                        
    }
}
