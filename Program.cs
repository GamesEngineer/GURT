using System;
using System.IO;
using System.Numerics;

namespace GURT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, I'm GURT.");
            Console.WriteLine("I trace rays to make pretty pictures.");
            ParseArgs(args);

            var image = new Image(imageWidth, imageHeight);
            var camera = new Camera(Vector3.UnitZ * 5f, Vector3.Zero);
            var tracer = new RayTracer();
            if (!string.IsNullOrEmpty(quality))
            {
                if (quality == "fast") tracer.quality = RayTracer.Quality.Fast;
                else if (quality == "good") tracer.quality = RayTracer.Quality.Good;
                else if (quality == "great") tracer.quality = RayTracer.Quality.Great;
            }

            BuildTestScene(tracer);

            if (string.IsNullOrEmpty(sceneFilename))
            {
                image.MakeTestImage();
                image.gamma = 1f;
            }
            else
            {
                tracer.RenderImage(camera, image);
                image.gamma = 2.2f;
            }

            image.WriteToFile(imageFilename);
            Console.WriteLine($"Image written to {Path.GetFullPath(imageFilename)}");
        }

        // Parsed arguments
        static int imageWidth = 1280;
        static int imageHeight = 720;
        static string imageFilename = "image.bmp";
        static string sceneFilename; // e.g., "scene.obj"
        static string quality;

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.Compare(arg, "--width") == 0)
                {
                    imageWidth = int.Parse(args[++i]);
                    Console.WriteLine($"Image width = {imageWidth}");
                }
                else if (string.Compare(arg, "--height") == 0)
                {
                    imageHeight = int.Parse(args[++i]);
                    Console.WriteLine($"Image height = {imageHeight}");
                }
                else if (string.Compare(arg, "--input") == 0)
                {
                    sceneFilename = args[++i];
                    Console.WriteLine($"Input scene filename = {sceneFilename}");
                }
                else if (string.Compare(arg, "--output") == 0)
                {
                    imageFilename = args[++i];
                    Console.WriteLine($"Ouput image filename = {imageFilename}");
                }
                else if (string.Compare(arg, "--quality") == 0)
                {
                    quality = args[++i];
                    Console.WriteLine($"Ouput image quality = {quality}");
                }
                else
                {
                    Console.WriteLine($"Ignoring unexpected argument: `{arg}`");
                }
            }
        }

        private static void BuildTestScene(RayTracer tracer)
        {
            tracer.lights.Add(new PointLight
            {
                center = Vector3.One * 15f,
                color = Color.White,
                intensity = 500f,
            });

            // "Ground"
            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.UnitY * -5002f,
                radius = 5000f,
                material = new Material
                {
                    baseColor = Color.Yellow * 0.1f,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.Zero,
                radius = 1f,
                material = new Material
                {
                    baseColor = Color.White,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.UnitY + Vector3.UnitZ,
                radius = 0.333f,
                material = new Material
                {
                    baseColor = Color.Magenta * 0.5f,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.UnitX * 1.6667f,
                radius = 0.5f,
                material = new Material
                {
                    baseColor = Color.Red,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.UnitX * -1.6667f,
                radius = 0.5f,
                material = new Material
                {
                    baseColor = Color.Yellow,
                    emissionColor = Color.Red * 0.1f,
                }
            });
            
            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.One,
                radius = 0.15f,
                material = new Material
                {
                    baseColor = Color.Blue,
                    emissionColor = Color.Black,
                }
            });
            
            tracer.sceneObjects.Add(new Sphere
            {
                center = Vector3.One - Vector3.UnitY * 0.25f,
                radius = 0.15f,
                material = new Material
                {
                    baseColor = Color.Green,
                    emissionColor = Color.Black,
                }
            });
        }
    }
}
