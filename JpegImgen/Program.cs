using System;
using System.Drawing;

namespace JpegImgen {
    class Program {
        static void fail() {
            Console.WriteLine("stupid");
            Environment.Exit(0);
        }

        static void Main(string[] args) {
            if (args.Length < 3) fail();
            string fn = args[0];
            bool s = true;
            int w, h;
            s &= int.TryParse(args[1], out w);
            s &= int.TryParse(args[2], out h);
            if (!s) fail();

            Bitmap b = new Bitmap(w*8, h*8);

            for (int x = 0; x < w; x++) for (int y = 0; y < h; y++) for (int ix = 0; ix < 8; ix++) for (int iy = 0; iy < 8; iy++)
                            b.SetPixel(x * 8 + ix, y * 8 + iy, Color.FromArgb(255, (255 * (ix + iy)) / 16, ((255 * x) / w), ((255 * y) / h)));

            b.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                                
                
            

        }
    }
}
