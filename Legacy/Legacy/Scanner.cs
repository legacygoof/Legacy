using Emgu.CV;
using Emgu.CV.Structure;
using OverwatchHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Legacy
{
    public class Scanner
    {
        public Point FindClosestEnemy(Image<Bgr, byte> image, Point center, Point offset)
        {
            
            Point target = Point.Empty;
            //finding players
            if (Input.KeyDown(Input.Keys.DOWN))
            {
                MessageBox.Show(image.Bitmap.GetPixel(150, 150).ToString());
            }
            
            var gray = image.InRange(new Bgr(15, 0, 168), new Bgr(22, 0, 255));

            gray = gray.SmoothBlur(5, 1);

            gray *= 10;

            gray = gray.Dilate(2);
            var points = new List<Point>();
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };
            Parallel.For(0, gray.Height, options, y =>
            {
                Parallel.For(0, gray.Width, options, x =>
                {
                    try
                    {
                        if (gray.Data[y, x, 0] != 0 && gray.Data[y, x - 1, 0] == 0 && gray.Data[y + 1, x - 1, 0] == 0 && gray.Data[y + 1, x, 0] == 0 && gray.Data[y - 1, x - 3, 0] == 0)
                        {
                            
                            image.Data[y, x, 0] = 255;
                            image.Data[y, x, 1] = 255;
                            image.Data[y, x, 2] = 255;

                            var point = new Point(x + offset.X, y + offset.Y);
                            points.Add(point);
                        }
                    }
                    catch
                    {

                    }
                });
            });

            if (points.Count > 0)
            {
                
                int centerX = gray.Width / 2;
                int centerY = gray.Height / 2;
                var centerPoint = new Point(centerX, centerY);
                var closestPoint = points[0];
                foreach (var point in points)
                {
                    if (GetAngleDiff(centerPoint, point) < GetAngleDiff(centerPoint, point))
                    {
                        closestPoint = point;
                    }
                }

                target = closestPoint;
            }

            gray.Dispose();
            return target;
        }

        double GetAngleDiff(Point crosshair, Point finalTarget)
        {
            var c = new System.Windows.Vector(crosshair.X, crosshair.Y);
            var t = new System.Windows.Vector(finalTarget.X, finalTarget.Y);
            var d = new System.Windows.Vector(Aimbot.mouseDelta.X, Aimbot.mouseDelta.Y);
            var a1 = System.Windows.Vector.AngleBetween(c, d);
            var a2 = System.Windows.Vector.AngleBetween(c, t);
            return Math.Abs(a1 - a2);
        }
    }
}
