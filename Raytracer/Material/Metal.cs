using System.Numerics;
using Raytracer.Helpers;
using Raytracer.Hit;

namespace Raytracer.Material
{
    public class Metal : IMaterial
    {
        private readonly Vector3 albedo;
        private readonly float fuzziness;

        public Metal(Vector3 albedo, float fuzziness)
        {
            this.albedo = albedo;
            this.fuzziness = fuzziness < 1f ? fuzziness : 1f;
        }
        
        public bool Scatter(in Ray rayIn, ref HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 reflected = Vector3.Reflect(rayIn.Direction, hitRecord.Normal);
            scattered = new Ray(hitRecord.Position, reflected + this.fuzziness * VectorHelper.GetRandomInUnitSphere());
            attenuation = this.albedo;

            return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0f;
        }
    }
}