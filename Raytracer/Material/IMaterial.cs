using System.Numerics;
using Raytracer.Hit;

namespace Raytracer.Material
{
    public interface IMaterial
    {
        bool Scatter(in Ray rayIn, ref HitRecord hitRecord, out Vector3 attenuation, out Ray scattered);
    }
}