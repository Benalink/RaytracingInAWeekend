using System.Numerics;
using Raytracer.MiscUtils;

namespace Raytracer.Helpers
{
    public static class VectorHelper
    {
        public static Vector3 GetRandomInUnitSphere()
        {
            Vector3 point;
            do
            {
                point = 2f * new Vector3((float) StaticRandom.NextDouble(), (float) StaticRandom.NextDouble(),
                    (float) StaticRandom.NextDouble()) - Vector3.One;
            } while (point.LengthSquared() >= 1f);

            return point;
        }
    }
}