using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Hit;
using Raytracer.Material;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Raytracer
{
    internal class Program
    {
        private static readonly Random Random = new Random();

        private static async Task Main(string[] args)
        {
            await using FileStream fileWriter = File.Open("render.png", FileMode.Create, FileAccess.Write);

            const int nx = 800;
            const int ny = 400;
            const int ns = 100;

            var interactableObjects = new List<IHitable>
            {
                new Sphere(new Vector3(0f, 0f, -1f), 0.5f, new Lambertian(new Vector3(0.8f, 0.3f, 0.3f))),
                new Sphere(new Vector3(0f, -100.5f, -1f), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0.8f))),
                new Sphere(new Vector3(1f, 0f, -1f), 0.5f, new Metal(new Vector3(0.8f, 0.6f, 0.2f))),
                new Sphere(new Vector3(-1f, 0f, -1f), 0.5f, new Metal(new Vector3(0.8f, 0.8f, 0.8f)))
            };
            var interactableObjectCollection = new HitableCollection(interactableObjects);

            var camera = new Camera();

#if DEBUG
            Console.WriteLine("Hardware acceleration is disabled in Debug mode");
#else
            Console.WriteLine($"Hardware acceleration: {Vector.IsHardwareAccelerated}");
#endif

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using var image = new Image<Rgba32>(nx, ny);
            for (int j = ny - 1; j >= 0; j--)
            for (int i = 0; i < nx; i++)
            {
                Vector3 colour = Vector3.Zero;
                for (int s = 0; s < ns; s++)
                {
                    float u = (float) (i + Random.NextDouble()) / nx;
                    float v = (float) (j + Random.NextDouble()) / ny;
                    Ray ray = camera.GetRay(u, v);
                    Vector3 point = ray.GetPoint(2f);
                    colour += Colour(ray, interactableObjectCollection, 0);
                }

                colour /= (float) ns;
                colour = new Vector3((float) Math.Sqrt(colour.X), (float) Math.Sqrt(colour.Y),
                    (float) Math.Sqrt(colour.Z));
                image[i, ny - j - 1] = new Rgba32(colour);
            }

            stopwatch.Stop();
            image.SaveAsPng(fileWriter);
            Console.WriteLine($"Render took {stopwatch.ElapsedMilliseconds}ms");
        }

        private static Vector3 NormalColour(Ray ray, IHitable world)
        {
            var record = new HitRecord();
            if (world.TestHit(ray, 0f, float.MaxValue, ref record))
                return 0.5f * new Vector3(record.Normal.X + 1f, record.Normal.Y + 1f, record.Normal.Z + 1f);

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 DiffuseColour(Ray ray, IHitable world)
        {
            var record = new HitRecord();
            if (world.TestHit(ray, 0.001f, float.MaxValue, ref record))
            {
                Vector3 target = record.Position + record.Normal + GetRandomInUnitSphere();
                return 0.5f * DiffuseColour(new Ray(record.Position, target - record.Position), world);
            }

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 Colour(Ray ray, IHitable world, int depth)
        {
            var record = new HitRecord();
            if (world.TestHit(ray, 0.001f, float.MaxValue, ref record))
            {
                if (depth < 50 && record.Material.Scatter(ray, ref record, out Vector3 attenuation, out Ray scattered))
                    return attenuation * Colour(scattered, world, ++depth);

                return Vector3.Zero;
                ;
            }

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 GetRandomInUnitSphere()
        {
            Vector3 point;
            do
            {
                point = 2f * new Vector3((float) Random.NextDouble(), (float) Random.NextDouble(),
                    (float) Random.NextDouble()) - Vector3.One;
            } while (point.LengthSquared() >= 1f);

            return point;
        }
    }
}