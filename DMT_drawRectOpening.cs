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
                RectOpening program = new RectOpening(drawingHandler);
                program.main();
            }
            else
            {
                MessageBox.Show("Ei leia joonist");
            }
        }
    }

    public class RectOpening
    {
        private DrawingHandler _drawingHandler;

        public RectOpening(DrawingHandler drawingHandler)
        {
            _drawingHandler = drawingHandler;
        }

        public void main()
        {
            try
            {
                while (true)
                {
                    Point firstPoint = null;
                    Point secondPoint = null;
                    Point thirdPoint = null;
                    Point fourthPoint = null;
                    ViewBase view = null;

                    getPoints(out firstPoint, out secondPoint, out view);
                    calcPoints(firstPoint, secondPoint, out thirdPoint, out fourthPoint);
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
            picker.PickTwoPoints("Vali esimene punkt", "Vali teine punkt", out firstPoint, out secondPoint, out view);
        }

        private void calcPoints(Point firstPoint, Point secondPoint, out Point thirdPoint, out Point fourthPoint)
        {
            Point calc1 = new Point(firstPoint.X, secondPoint.Y);
            thirdPoint = calc1;

            Point calc2 = new Point(secondPoint.X, firstPoint.Y);
            fourthPoint = calc2;

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
