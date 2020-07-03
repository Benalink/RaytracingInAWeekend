using System;
using System.Numerics;

namespace Raytracer.Helpers
{
    public static class VectorHelper
    {
        private static readonly Random Random = new Random();
        
        public static Vector3 GetRandomInUnitSphere()
        {
            Vector3 point;
            do
            {
                point = 2f * new Vector3((float) Random.NextDouble(), (float) Random.NextDouble(),
                    (float) Random.NextDouble()) - Vector3.One;
            } while (point.LengthSquared() >= 1f);

            return point;
        }
    }
}