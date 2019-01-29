using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace AmeisenBot.Map
{
    public class AmeisenMap
    {
        public List<Unit> ActiveUnits { get; private set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public AmeisenMap(int initialSizeX, int initialSizeY)
        {
            SizeX = initialSizeX;
            SizeY = initialSizeY;

            ActiveUnits = new List<Unit>();
        }

        public BitmapImage GenerateBitmap(Me me, bool drawBackground = true)
        {
            BitmapImage bitmapImageMap = new BitmapImage();

            Brush backgroundBrush = new SolidBrush(Color.FromArgb(255, 220, 220, 220));

            Brush unitBrush = new SolidBrush(Color.FromArgb(255, 255, 173, 65));
            Brush playerBrush = new SolidBrush(Color.FromArgb(255, 66, 138, 255));
            Brush meBrush = new SolidBrush(Color.FromArgb(255, 255, 65, 65));

            using (Bitmap bitmap = new Bitmap(SizeX, SizeY))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    if (drawBackground)
                    {
                        for (int x = 0; x < SizeX; x++)
                        {
                            for (int y = 0; y < SizeY; y++)
                            {
                                graphics.FillRectangle(
                                    backgroundBrush,
                                    new Rectangle(x, y, 1, 1));
                            }
                        }
                    }

                    if (ActiveUnits.Count > 0)
                    {
                        Brush activeBrush = unitBrush;
                        foreach (Unit u in ActiveUnits)
                        {
                            if (u.GetType() == typeof(Me))
                            {
                                activeBrush = meBrush;
                            }
                            else if (u.GetType() == typeof(Player))
                            {
                                activeBrush = playerBrush;
                            }
                            else if (u.GetType() == typeof(Unit))
                            {
                                activeBrush = unitBrush;
                            }

                            int mapX = (int)(me.pos.X - u.pos.X);
                            int mapY = (int)(me.pos.Y - u.pos.Y);

                            if (IsPositionInBoundaries(me, mapX, mapY))
                            {
                                graphics.FillRectangle(
                                    activeBrush,
                                    new Rectangle(mapX, mapY, 1, 1));
                            }
                        }
                    }
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImageMap.BeginInit();
                    bitmapImageMap.StreamSource = memory;
                    bitmapImageMap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImageMap.EndInit();
                }
            }

            return bitmapImageMap;
        }

        public bool IsPositionInBoundaries(Me me, int x, int y)
        {
            int xOuterBoudary = (int)me.pos.X + (SizeX / 2);
            int xInnerBoudary = (int)me.pos.X - (SizeX / 2);
            int yOuterBoudary = (int)me.pos.Y + (SizeY / 2);
            int yInnerBoudary = (int)me.pos.Y - (SizeY / 2);

            return x > xInnerBoudary
                && x < xOuterBoudary
                && y > yInnerBoudary
                && y < yOuterBoudary;
        }
    }
}
