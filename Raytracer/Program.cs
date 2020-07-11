using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Hit;
using Raytracer.Material;
using Raytracer.MiscUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Raytracer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await using FileStream fileWriter = File.Open("render.png", FileMode.Create, FileAccess.Write);

            const float aspectRatio = 16f / 9f;
            const int imageWidth = 1200;
            const int imageHeight = (int)(imageWidth / aspectRatio);
            const int samplesPerPixel = 100;

            // var interactableObjects = new List<IHitable>
            // {
            //     new Sphere(new Vector3(0f, 0f, -1f), 0.5f, new Lambertian(new Vector3(0.1f, 0.2f, 0.5f))),
            //     new Sphere(new Vector3(0f, -100.5f, -1f), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0.0f))),
            //     new Sphere(new Vector3(1f, 0f, -1f), 0.5f, new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0f)),
            //     new Sphere(new Vector3(-1f, 0f, -1f), 0.5f, new Dielectric(1.5f))
            // };
            // var interactableObjectCollection = new HitableCollection(interactableObjects);

            IHitable world = CreateRandomScene();

            var camPos = new Vector3(13f, 2f, 3f);
            var camTarget = new Vector3(0f, 0f, 0f);
            float focusDistance = 10f;
            float aperture = 0f;
            
            var camera = new Camera(camPos, camTarget, new Vector3(0f, 1f, 0f), 20,
                aspectRatio, aperture, focusDistance);

#if DEBUG
            Console.WriteLine("Hardware acceleration is disabled in Debug mode");
#else
            Console.WriteLine($"Hardware acceleration: {Vector.IsHardwareAccelerated}");
#endif
            var raytracer = new Raytracer(imageWidth, imageHeight, samplesPerPixel);
            using (Image<Rgba32> render = raytracer.Render(camera, world))
            {
                render.SaveAsPng(fileWriter);
            }
        }

        private static IHitable CreateRandomScene()
        {
            var hitables = new List<IHitable>(500);
            hitables.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, new Lambertian(new Vector3(0.5f, 0.5f, 0.5f))));
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    float randomMat = (float)StaticRandom.NextDouble();
                    var center = new Vector3(a + 0.9f * (float)StaticRandom.NextDouble(), 0.2f, b + 0.9f * (float)StaticRandom.NextDouble());
                    if((center - new Vector3(4f, 0.2f, 0f)).Length() > 0.9f)
                    {
                        if (randomMat < 0.8f) //diffuse
                        {
                            hitables.Add(new Sphere(center, 0.2f, new Lambertian(new Vector3((float)StaticRandom.NextDouble() * (float)StaticRandom.NextDouble(), (float)StaticRandom.NextDouble() * (float)StaticRandom.NextDouble(), (float)StaticRandom.NextDouble() * (float)StaticRandom.NextDouble()))));
                        }
                        else if (randomMat < 0.95f) //metal
                        {
                            hitables.Add(new Sphere(center, 0.2f, new Metal(new Vector3(0.5f * (1f + (float)StaticRandom.NextDouble()), 0.5f * (1 + (float)StaticRandom.NextDouble()), 0.5f * (float)StaticRandom.NextDouble()), 0.2f)));
                        }
                        else //glass
                        {
                            hitables.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
                        }
                    }
                }
            }
            
            hitables.Add(new Sphere(new Vector3(0f, 1f, 0f),1.0f, new Dielectric(1.5f)));
            hitables.Add(new Sphere(new Vector3(-4f, 1f, 0f),1.0f, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
            hitables.Add(new Sphere(new Vector3(4f, 1f, 0f),1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)));
            
            return new HitableCollection(hitables);
        }

        private static Vector3 NormalColour(in Ray ray, IHitable world)
        {
            var record = new HitRecord();
            if (world.TestHit(in ray, 0f, float.MaxValue, ref record))
                return 0.5f * new Vector3(record.Normal.X + 1f, record.Normal.Y + 1f, record.Normal.Z + 1f);

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 DiffuseColour(in Ray ray, IHitable world)
        {
            var record = new HitRecord();
            if (world.TestHit(in ray, 0.001f, float.MaxValue, ref record))
            {
                Vector3 target = record.Position + record.Normal + GetRandomInUnitSphere();
                return 0.5f * DiffuseColour(new Ray(record.Position, target - record.Position), world);
            }

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 Colour(in Ray ray, IHitable world, int depth)
        {
            var record = new HitRecord();
            if (world.TestHit(in ray, 0.001f, float.MaxValue, ref record))
            {
                if (depth < 50 && record.Material.Scatter(in ray, ref record, out Vector3 attenuation, out Ray scattered))
                    return attenuation * Colour(in scattered, world, ++depth);

                return Vector3.Zero;
            }

            float t = 0.5f * ray.Direction.Y + 1f;
            return (1f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static Vector3 GetRandomInUnitSphere()
        {
            Vector3 point;
            do
            {
                point = 2f * new Vector3((float) StaticRandom.NextDouble(), (float) StaticRandom.NextDouble(),
                    (float) StaticRandom.NextDouble()) - Vector3.One;
            } while (point.LengthSquared() >= 1f);

            return point;
        }
    }
}