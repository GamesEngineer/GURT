using System;
using System.IO;

namespace GURT
{
    public class Image
    {
        public int width;
        public int height;
        public Color[,] pixels;
        public float aspectRatio;
        public float gamma = 2.2f;

        public Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.pixels = new Color[height, width];
            this.aspectRatio = (float)width / (float)height;
        }

        // Create a Red-Green gradient test image
        public void MakeTestImage()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float red = (float)x / (float)(width - 1);
                    float green = (float)y / (float)(height - 1);
                    Color color = new() { R = red, G = green, B = 0, A = 1 };
                    pixels[y, x] = color;
                }
            }
        }

        public void WriteToFile(string filename)
        {
            if (filename.ToLower().EndsWith(".bmp"))
            {
                byte[] fileHeader = new byte[14]
                {
                    (byte)'B', (byte)'M', 0, 0, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0
                };
                byte[] imageHeader = new byte[40]
                {
                    40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                };

                int filesize = fileHeader.Length + imageHeader.Length + 3 * width * height;

                fileHeader[2] = (byte)(filesize);
                fileHeader[3] = (byte)(filesize >> 8);
                fileHeader[4] = (byte)(filesize >> 16);
                fileHeader[5] = (byte)(filesize >> 24);

                imageHeader[4] = (byte)(width);
                imageHeader[5] = (byte)(width >> 8);
                imageHeader[6] = (byte)(width >> 16);
                imageHeader[7] = (byte)(width >> 24);
                imageHeader[8] = (byte)(height);
                imageHeader[9] = (byte)(height >> 8);
                imageHeader[10] = (byte)(height >> 16);
                imageHeader[11] = (byte)(height >> 24);

                float MAX_VALUE = MathF.BitDecrement(256f);
                float invGamma = 1f / gamma;
                using (var writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(fileHeader);
                    writer.Write(imageHeader);
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color c = pixels[y, x];
                            c.ApplyGammaCorrection(invGamma);
                            c = c.Clamp01;
                            writer.Write((byte)(c.B * MAX_VALUE));
                            writer.Write((byte)(c.G * MAX_VALUE));
                            writer.Write((byte)(c.R * MAX_VALUE));
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
