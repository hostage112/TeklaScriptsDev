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
            Model myModel = new Model();
            ScaleToDrawingTitle3.main();
            myModel.CommitChanges();
        }
    }

    public static class ScaleToDrawingTitle3
    {
        public static void main()
        {
            DrawingEnumerator selectedDrawings = getSelectedDrawings();

            foreach (Drawing currentDrawing in selectedDrawings)
            {
                double highestScale = getHighestScale(currentDrawing);
                setScaleToTitle3(currentDrawing, highestScale);
            }

            MessageBox.Show("Valitud " + selectedDrawings.GetSize() + " joonist." + Environment.NewLine +
                    "Muudetud " + selectedDrawings.GetSize() + " joonise title3");

        }

        private static DrawingEnumerator getSelectedDrawings()
        {
            DrawingHandler myDrawingHandler = new DrawingHandler();
            DrawingEnumerator selectedDrawings = myDrawingHandler.GetDrawingSelector().GetSelected();

            return selectedDrawings;
        }

        private static double getHighestScale(Drawing currentDrawing)
        {
            double highestScale = 0;

            DrawingObjectEnumerator ViewEnum = currentDrawing.GetSheet().GetViews();

            foreach (TSD.View currentView in ViewEnum)
            {
                if (isView2D(currentView))
                {
                    double currentScale = currentView.Attributes.Scale;
                    highestScale = Math.Max(currentScale, highestScale);
                }
            }

            return highestScale;
        }

        private static bool isView2D(TSD.View currentView)
        {
            CoordinateSystem disp = currentView.DisplayCoordinateSystem as CoordinateSystem;
            CoordinateSystem viewp = currentView.ViewCoordinateSystem as CoordinateSystem;

            if (disp.AxisX.Z != viewp.AxisX.Z || disp.AxisY.Z != viewp.AxisY.Z)
            {
                return false;
            }

            return true;
        }

        private static void setScaleToTitle3(Drawing currentDrawing, double highestScale)
        {
            currentDrawing.Title3 = "1:" + highestScale.ToString();
            currentDrawing.Modify();
        }
    }
}