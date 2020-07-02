using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace Raytracer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await using TextWriter writer = File.CreateText("render.ppm");
            
            const int nx = 200;
            const int ny = 100;
            
            await writer.WriteLineAsync("P3");
            await writer.WriteLineAsync($"{nx} {ny}");
            await writer.WriteLineAsync("255");
            
            var lowerLeftCorner = new Vector3(-2f, -1f, -1f);
            var horizontal = new Vector3(4f, 0f, 0f);
            var vertical = new Vector3(0f, 2f, 0f);
            Vector3 origin = Vector3.Zero;
            
            for (int j = ny - 1; j >= 0; j--)
            {
                for (int i = 0; i < nx; i++)
                {
                    float u = i / (float) nx;
                    float v = j / (float) ny;
                    
                    var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical);
                    Vector3 colour = Colour(ray);

                    int ir = (int) (255.99 * colour.X);
                    int ig = (int) (255.99 * colour.Y);
                    int ib = (int) (255.99 * colour.Z);

                    await writer.WriteLineAsync($"{ir} {ig} {ib}");
                }
            }
        }

        private static float hitSphere(Vector3 center, float radius, Ray ray)
        {
            Vector3 oc = ray.Origin - center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return -1f;
            
            return (-b - (float)Math.Sqrt(discriminant)) / (2f * a);
        }

        private static Vector3 Colour(Ray ray)
        {
            float t = hitSphere(new Vector3(0, 0, -1), 0.5f, ray);
            if (t > 0f)
            {
                Vector3 normal = Vector3.Normalize(ray.GetPoint(t) - new Vector3(0f, 0f, -1f));
                return 0.5f * new Vector3(normal.X + 1f, normal.Y + 1f, normal.Z + 1f);
            }
            
            t = 0.5f * (ray.Direction.Y + 1f);
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }
    }
}