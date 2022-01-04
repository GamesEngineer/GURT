using System;
using System.Numerics;

namespace GURT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! I'm GURT. I trace rays to make pretty pictures.");
            ParseArgs(args);
            var image = new Image(imageWidth, imageHeight);
            var camera = new Camera(Vector3.UnitZ * -5f, Vector3.Zero);
            var tracer = new RayTracer();
            // TODO - remove test object
            tracer.sceneObjects.Add(new Elipsoid { center = Vector3.Zero, radii = Vector3.One, material = new Material() });
            tracer.RenderImage(camera, image);
            image.WriteToFile(imageFilename);
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
    }
}
