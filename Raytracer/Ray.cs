using System.Numerics;

namespace Raytracer
{
    public readonly struct Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = Vector3.Normalize(direction);
        }

        public Vector3 GetPoint(float t)
        {
            return this.Origin + t * this.Direction;
        }
    }
}