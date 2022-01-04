using System;
using System.IO;
using System.Drawing;

namespace GURT
{
    public class Image
    {
        public int width;
        public int height;
        public Color[,] pixels; 

        public Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.pixels = new Color[height, width];
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

                using (var writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(fileHeader);
                    writer.Write(imageHeader);
                    for (int y = height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color c = pixels[y, x];
                            writer.Write(c.R);
                            writer.Write(c.G);
                            writer.Write(c.B);
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
