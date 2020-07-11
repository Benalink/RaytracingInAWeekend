using System;
using System.Numerics;
using Raytracer.Material;

namespace Raytracer.Hit
{
    public class Sphere : IHitable
    {
        public Vector3 Position { get; }
        public float Radius { get; }
        public IMaterial Material { get; }

        public Sphere(Vector3 position, float radius, IMaterial material)
        {
            this.Position = position;
            this.Radius = radius;
            this.Material = material;
        }
        
        public bool TestHit(in Ray ray, float tMin, float tMax, ref HitRecord record)
        {
            Vector3 oc = ray.Origin - this.Position;
            float a = ray.Direction.LengthSquared();
            float b = Vector3.Dot(oc, ray.Direction);
            float c = oc.LengthSquared() - this.Radius * this.Radius;
            float discriminant = b*b * a*c;

            if (discriminant > 0)
            {
                float t = (-b - (float) Math.Sqrt(b * b - a * c)) / a;
                if (t < tMax && t > tMin)
                {
                    record.T = t;
                    record.Position = ray.GetPoint(t);
                    record.Normal = (record.Position - this.Position) / this.Radius;
                    record.Material = this.Material;
                    
                    return true;
                }
            }

            return false;
        }
    }
}