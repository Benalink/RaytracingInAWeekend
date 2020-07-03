using System;
using System.Numerics;

namespace Raytracer
{
    public class Camera
    {
        private static readonly Random Random = new Random();

        private Vector3 lowerLeftCorner;
        private Vector3 horizontal;
        private Vector3 vertical;
        private float lensRadius;
        
        private Vector3 u;
        private Vector3 v;
        private Vector3 w;

        public Vector3 Position { get; }

        public Camera(Vector3 position, Vector3 target, Vector3 vup, float verticalFov, float aspectRatio, float aperture, float focusDistance)
        {
            this.lensRadius = aperture / 2;
            float theta = verticalFov * (float)Math.PI / 180;
            float halfHeight = (float)Math.Tan(theta / 2);
            float halfWidth = aspectRatio * halfHeight;
            this.Position = position;
            this.w = Vector3.Normalize(position - target);
            this.u = Vector3.Normalize(Vector3.Cross(vup, this.w));
            this.v = Vector3.Cross(this.w, this.u);
            
            this.lowerLeftCorner = new Vector3(-halfWidth, -halfHeight, -1f);
            this.lowerLeftCorner = this.Position - halfWidth * focusDistance * this.u - halfHeight * focusDistance * this.v - focusDistance * this.w;
            this.horizontal = 2 * halfWidth * focusDistance * this.u;
            this.vertical = 2 * halfHeight * focusDistance * this.v;
        }

        public Ray GetRay(float s, float t)
        {
            Vector3 rd = this.lensRadius * GetRandomInUnitDisk();
            Vector3 offset = this.u * rd.X + this.v * rd.Y;
           return new Ray(this.Position + offset, this.lowerLeftCorner + s * this.horizontal + t * this.vertical - this.Position - offset);
        }

        private Vector3 GetRandomInUnitDisk()
        {
            Vector3 point;
            do
            {
                point = 2f * new Vector3((float) Random.NextDouble(), (float) Random.NextDouble(), 0) -
                        new Vector3(1f, 1f, 0f);
            } while (Vector3.Dot(point, point) >= 1f);

            return point;
        }
    }
}