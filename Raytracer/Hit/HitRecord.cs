using System.Numerics;
using Raytracer.Material;

namespace Raytracer.Hit
{
    public struct HitRecord
    {
        public float T { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public IMaterial Material { get; set; }
    }
}