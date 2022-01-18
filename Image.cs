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
            if (!filename.ToLower().EndsWith(".bmp"))
            {
                throw new ArgumentException($"unsupported file type: {filename}", nameof(filename));
            }

            // https://en.wikipedia.org/wiki/BMP_file_format
            // http://www.ece.ualberta.ca/~elliott/ee552/studentAppNotes/2003_w/misc/bmp_file_format/bmp_file_format.htm

            int rowSize = AlignToFour(BYTES_PER_PIXEL * width); // each row of pixels must be a multiple of 4 bytes
            int imageDataSize = rowSize * height;

            // Open and write the BMP file
            float MAX_VALUE = MathF.BitDecrement(256f);
            float invGamma = 1f / gamma;
            using (var writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                WriteBmFileHeader(writer, imageDataSize);
                WriteDibHeader(writer, width, height);
                // write the image data, row by row
                for (int y = 0; y < height; y++)
                {
                    // write a row of pixels
                    for (int x = 0; x < width; x++)
                    {
                        Color c = pixels[y, x];
                        c.ApplyGammaCorrection(invGamma);
                        c = c.Clamp01;
                        writer.Write((byte)(c.B * MAX_VALUE));
                        writer.Write((byte)(c.G * MAX_VALUE));
                        writer.Write((byte)(c.R * MAX_VALUE));
                    }
                    // add padding to ensure that each row ends on a word-aligned boundary
                    int padding = rowSize - width * BYTES_PER_PIXEL;
                    while (padding-- > 0)
                    {
                        writer.Write((byte)0);
                    }
                }
            }
        }

        const int SIZEOF_BM_FILE_HEADER = 14; // bytes
        const int SIZEOF_DIB_HEADER = 40; // bytes
        const int BYTES_PER_PIXEL = 3; // blue, green, red

        static int AlignToFour(int x) => ((x + 3) / 4) * 4;

        static void WriteBmFileHeader(BinaryWriter writer, int imageDataSize)
        {
            int imageDataOffset = SIZEOF_BM_FILE_HEADER + SIZEOF_DIB_HEADER;
            int fileSize = imageDataOffset + imageDataSize;
            writer.Write((byte)'B'); writer.Write((byte)'M'); // file type signature
            writer.Write(fileSize);
            writer.Write(0); // application codes
            writer.Write(imageDataOffset);
        }

        static void WriteDibHeader(BinaryWriter writer, int width, int height)
        {
            writer.Write(SIZEOF_DIB_HEADER); // the size also implicitly specifies which type of DIB header this is
            writer.Write(width); // image width (# pixels)
            writer.Write(height); // image height (# pixels) [Note: positive = "bottom up", negative = "top down"]
            writer.Write((short)1); // # of color planes
            writer.Write((short)(BYTES_PER_PIXEL * 8)); // bits per pixel
            writer.Write(0); // type of compression (0 = BI_RGB no compression)
            writer.Write(0); // size of image after compression (can be 0 when compression is BI_RGB)
            writer.Write(0); // horizontal resolution (pixels per meter)
            writer.Write(0); // vertical resolution (pixels per meter)
            writer.Write(0); // # colors actually used from palette (set to 0 when using 24-bit RGB)
            writer.Write(0); // # of important colors (0 = all)
        }
    }
}
