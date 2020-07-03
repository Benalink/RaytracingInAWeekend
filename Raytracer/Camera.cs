using System.Numerics;

namespace Raytracer
{
    public class Camera
    {
        private Vector3 lowerLeftCorner;
        private Vector3 horizontal;
        private Vector3 vertical;
        public Vector3 Position { get; }

        public Camera()
        {
            this.lowerLeftCorner = new Vector3(-2f, -1f, -1f);
            this.horizontal = new Vector3(4f, 0f, 0f);
            this.vertical = new Vector3(0f, 2f, 0f);
            this.Position = Vector3.Zero;
        }

        public Ray GetRay(float u, float v)
        {
           return new Ray(this.Position, this.lowerLeftCorner + u * this.horizontal + v * this.vertical - this.Position);
        }
    }
}