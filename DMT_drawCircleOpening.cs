using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;


namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            DrawingHandler drawingHandler = new DrawingHandler();
            if (drawingHandler.GetConnectionStatus())
            {
                CircleOpening program = new CircleOpening(drawingHandler);
                program.main();
            }
            else
            {
                MessageBox.Show("Ei leia joonist");
            }
        }
    }

    public class CircleOpening
    {
        private DrawingHandler _drawingHandler;

        public CircleOpening(DrawingHandler drawingHandler)
        {
            _drawingHandler = drawingHandler;
        }

        public void main()
        {
            try
            {
                while (true)
                {
                    Point userCenter = null;
                    Point userRadius = null;
                    ViewBase view = null;

                    Point firstPoint = null;
                    Point secondPoint = null;
                    Point thirdPoint = null;
                    Point fourthPoint = null;
                    
                    getPoints(out userCenter, out userRadius, out view);
                    calcPoints(userCenter, userRadius, out firstPoint, out secondPoint, out thirdPoint, out fourthPoint);
                    drawLine(firstPoint, secondPoint, view);
                    drawLine(thirdPoint, fourthPoint, view);
                }
            }
            catch (PickerInterruptedException)
            {
                // No pick happened
            }
        }

        private void getPoints(out Point firstPoint, out Point secondPoint, out ViewBase view)
        {
            Picker picker = _drawingHandler.GetPicker();
            picker.PickTwoPoints("Vali ringi keskpunkt", "Vali punkt ringi kontuuril", out firstPoint, out secondPoint, out view);
        }

        private void calcPoints(Point userCenter, Point userRadius, out Point firstPoint, out Point secondPoint, out Point thirdPoint, out Point fourthPoint)
        {
            double Rx = userCenter.X - userRadius.X;
            double Ry = userCenter.Y - userRadius.Y;
            double R = Math.Sqrt(Rx * Rx + Ry * Ry);
            double COS45 = 0.7071068;

            Point a1 = new Point(userCenter.X - R * COS45, userCenter.Y - R * COS45);
            firstPoint = a1;
            Point a2 = new Point(userCenter.X + R * COS45, userCenter.Y + R * COS45);
            secondPoint = a2;
            Point a3 = new Point(userCenter.X + R * COS45, userCenter.Y - R * COS45);
            thirdPoint = a3;
            Point a4 = new Point(userCenter.X - R * COS45, userCenter.Y + R * COS45);
            fourthPoint = a4;

        }

        private void drawLine(Point p1, Point p2, ViewBase view)
        {
            TSD.Line newLine = new TSD.Line(view, p1, p2);
            LineTypeAttributes lineParams = new LineTypeAttributes(LineTypes.SolidLine, DrawingColors.Red);
            newLine.Attributes.Line = lineParams;
            newLine.Insert();
        }


    }
}
