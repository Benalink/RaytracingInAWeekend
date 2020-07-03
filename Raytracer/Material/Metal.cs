using System.Numerics;
using Raytracer.Hit;

namespace Raytracer.Material
{
    public class Metal : IMaterial
    {
        private readonly Vector3 albedo;

        public Metal(Vector3 albedo)
        {
            this.albedo = albedo;
        }
        
        public bool Scatter(Ray rayIn, ref HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 reflected = Reflect(rayIn.Direction, hitRecord.Normal);
            scattered = new Ray(hitRecord.Position, reflected);
            attenuation = this.albedo;

            return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0f;
        }

        private Vector3 Reflect(Vector3 hitDirection, Vector3 normal)
        {
            return hitDirection - 2 * Vector3.Dot(hitDirection, normal) * normal;
        }
    }
}