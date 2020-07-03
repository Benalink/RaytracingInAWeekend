using System;
using System.Numerics;
using Raytracer.Hit;

namespace Raytracer.Material
{
    public class Dielectric: IMaterial
    {
        private static readonly Random Random = new Random();
        private readonly float refractiveIndex;

        public Dielectric(float refractiveIndex)
        {
            this.refractiveIndex = refractiveIndex;
        }
        public bool Scatter(Ray rayIn, ref HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 outwardNormal;
            Vector3 reflected = Vector3.Reflect(rayIn.Direction, hitRecord.Normal);
            float niOverNt;
            attenuation = new Vector3(1f, 1f, 1f);
            Vector3 refracted;
            float reflectProbability;
            float cosine;
            
            if (Vector3.Dot(rayIn.Direction, hitRecord.Normal) > 0)
            {
                outwardNormal = -hitRecord.Normal;
                niOverNt = this.refractiveIndex;
                cosine = this.refractiveIndex * Vector3.Dot(rayIn.Direction, hitRecord.Normal) /
                         rayIn.Direction.Length();
            }
            else
            {
                outwardNormal = hitRecord.Normal;
                niOverNt = 1f / this.refractiveIndex;
                cosine = -Vector3.Dot(rayIn.Direction, hitRecord.Normal) / rayIn.Direction.Length();
            }

            if (Refract(rayIn.Direction, outwardNormal, niOverNt, out refracted))
            {
                reflectProbability = schlick(cosine, this.refractiveIndex);
            }
            else
            {
                reflectProbability = 1f;
            }

            if ((float) Random.NextDouble() < reflectProbability)
            {
                scattered = new Ray(hitRecord.Position, reflected);
            }
            else
            {
                scattered = new Ray(hitRecord.Position, refracted);
            }
            
            return true;
        }

        private bool Refract(Vector3 direction, Vector3 normal, float niOverNt, out Vector3 refracted)
        {
            float dt = Vector3.Dot(direction, normal);
            float discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);
            if (discriminant > 0)
            {
                refracted = niOverNt * (direction - normal * dt) - normal * (float)Math.Sqrt(discriminant);
                return true;
            }
            else
            {
                refracted = Vector3.Zero;
                return false;
            }
        }

        private float schlick(float cosine, float refractiveIndex)
        {
            float r0 = (1 - refractiveIndex) / (1 + refractiveIndex);
            r0 = r0 * r0;
            return r0 + (1 - r0) * (float)Math.Pow(1 - cosine, 5);
        }
    }
}