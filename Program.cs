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
            var camera = new Camera(Vector3.UnitZ * -5f, Vector3.Zero);
            var tracer = new RayTracer();
            BuildTestScene(tracer);
            tracer.RenderImage(camera, image);
            image.WriteToFile(imageFilename);
            Console.WriteLine($"Image written to {Path.GetFullPath(imageFilename)}");
        }

        // Parsed arguments
        static int imageWidth = 1280;
        static int imageHeight = 720;
        static string imageFilename = "image.bmp";
        static string sceneFilename = "scene.obj";

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
                center = (Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ) * 5f,
                color = Color.White,
                intensity = 50f
            });

            tracer.sceneObjects.Add(new Elipsoid
            {
                center = Vector3.Zero,
                radii = Vector3.One,
                material = new Material
                {
                    baseColor = Color.White,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Elipsoid
            {
                center = Vector3.UnitY,
                radii = Vector3.One * 0.5f,
                material = new Material
                {
                    baseColor = Color.Magenta * 0.5f,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Elipsoid
            {
                center = Vector3.UnitX,
                radii = Vector3.One * 0.5f,
                material = new Material
                {
                    baseColor = Color.Yellow,
                    emissionColor = Color.Black,
                }
            });

            tracer.sceneObjects.Add(new Elipsoid
            {
                center = Vector3.UnitX * -2f,
                radii = Vector3.One * 0.5f,
                material = new Material
                {
                    baseColor = Color.Yellow,
                    emissionColor = Color.Red * 0.1f,
                }
            });
        }
    }
}
