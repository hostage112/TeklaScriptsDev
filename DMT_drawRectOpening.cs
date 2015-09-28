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
        private Point firstPoint;
        private Point secondPoint;
        private Point thirdPoint;
        private Point fourthPoint;

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
                    Point userPoint1 = null;
                    Point userPoint2 = null;
                    ViewBase view = null;

                    getPoints(out userPoint1, out userPoint2, out view);
                    calcPoints(userPoint1, userPoint2);
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

        private void calcPoints(Point userPoint1, Point userPoint2)
        {
            firstPoint = userPoint1;
            secondPoint = userPoint2;
            thirdPoint = new Point(firstPoint.X, secondPoint.Y);
            fourthPoint = new Point(secondPoint.X, firstPoint.Y);

        }

        private void drawLine(Point p1, Point p2, ViewBase view)
        {
            TSD.Line newLine = new TSD.Line(view, p1, p2);
            LineTypeAttributes lineParams = new LineTypeAttributes(LineTypes.SolidLine, DrawingColors.Black);
            newLine.Attributes.Line = lineParams;
            newLine.Insert();
        }


    }
}
