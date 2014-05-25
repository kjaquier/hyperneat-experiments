using System.Drawing;
using System.Windows.Forms;

namespace SharpNeat.Experiments.Common
{
    public class BitmapView : PictureBox
    {
        private int pixelsX, pixelsY;

        private int TileWidth
        {
            get { return Width / pixelsY; }
        }

        private int TileHeight
        {
            get { return Height / pixelsX; }
        }

        public void SetResolution(int pixelsX, int pixelsY)
        {
            this.pixelsX = pixelsX;
            this.pixelsY = pixelsY;
        }

        public delegate Brush PixelSetter(int x, int y);

        public void Update(PixelSetter setColorFun)
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            using (var g = Graphics.FromImage(bitmap))
            {

                for (var i = 0; i < pixelsY; i++)
                {
                    for (var j = 0; j < pixelsX; j++)
                    {
                        var color = setColorFun(j, i);
                        g.FillRectangle(color, i * TileWidth, j * TileHeight, TileWidth, TileHeight);
                    }
                }
                Image = bitmap;
            }
            this.Update();
        }
    }
}
