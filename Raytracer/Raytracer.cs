using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Raytracer.Helpers;
using Raytracer.Hit;
using Raytracer.MiscUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Raytracer
{
    public class Raytracer
    {
        private readonly int imageWidth;
        private readonly int imageHeight;
        private readonly int samplesPerPixel;
        static object myLock = new object();


        public Raytracer(int imageWidth, int imageHeight, int samplesPerPixel)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.samplesPerPixel = samplesPerPixel;
        }
        
        public Image<Rgba32> Render(Camera camera, IHitable world)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var image = new Image<Rgba32>(this.imageWidth, this.imageHeight);
            Console.WriteLine("Rendering scene... ");
            using var progress = new ProgressBar();
            {
                int completeLines = 0;
                Parallel.For(0, this.imageHeight, j =>
                {
                    Span<Rgba32> rowSpan = image.GetPixelRowSpan(this.imageHeight - j - 1);
                    for (int i = 0; i < this.imageWidth; i++)
                    {
                        Vector3 colour = Vector3.Zero;
                        for (int s = 0; s < this.samplesPerPixel; s++)
                        {
                            float u = (float) (i + StaticRandom.NextDouble()) / this.imageWidth;
                            float v = (float) (j + StaticRandom.NextDouble()) / this.imageHeight;
                            Ray ray = camera.GetRay(u, v);
                            colour += Colour(in ray, world, 0);
                        }
                
                        colour /= (float) this.samplesPerPixel;
                        colour = new Vector3((float) Math.Sqrt(colour.X), (float) Math.Sqrt(colour.Y),
                            (float) Math.Sqrt(colour.Z));
                        rowSpan[i] = new Rgba32(colour);
                    }

                    Interlocked.Increment(ref completeLines);
                    progress.Report(completeLines / (float) this.imageHeight);
                    
                });
                // for (int j = 0; j < this.imageHeight; j++)
                // {
                //     progress.Report(j / (float) this.imageHeight);
                //     for (int i = 0; i < this.imageWidth; i++)
                //     {
                //         Vector3 colour = Vector3.Zero;
                //         for (int s = 0; s < this.samplesPerPixel; s++)
                //         {
                //             float u = (float) (i + StaticRandom.NextDouble()) / this.imageWidth;
                //             float v = (float) (j + StaticRandom.NextDouble()) / this.imageHeight;
                //             Ray ray = camera.GetRay(u, v);
                //             colour += Colour(in ray, world, 0);
                //         }
                //
                //         colour /= (float) this.samplesPerPixel;
                //         colour = new Vector3((float) Math.Sqrt(colour.X), (float) Math.Sqrt(colour.Y),
                //             (float) Math.Sqrt(colour.Z));
                //         image[i, this.imageHeight - j - 1] = new Rgba32(colour);
                //     }
                // }
            }

            stopwatch.Stop();
            Console.WriteLine($"\nRender took {stopwatch.ElapsedMilliseconds}ms");
            return image;
        }
        
        private Vector3 Colour(in Ray ray, IHitable world, int depth)
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
    }
}