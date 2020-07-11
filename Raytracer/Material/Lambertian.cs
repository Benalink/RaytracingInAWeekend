using System.Numerics;
using Raytracer.Helpers;
using Raytracer.Hit;

namespace Raytracer.Material
{
    public class Lambertian : IMaterial
    {
        private readonly Vector3 albedo;

        public Lambertian(Vector3 albedo)
        {
            this.albedo = albedo;
        }
        
        public bool Scatter(in Ray rayIn, ref HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 target = hitRecord.Position + hitRecord.Normal + VectorHelper.GetRandomInUnitSphere();
            scattered = new Ray(hitRecord.Position, target - hitRecord.Position);
            attenuation = this.albedo;
            
            return true;
        }
    }
}