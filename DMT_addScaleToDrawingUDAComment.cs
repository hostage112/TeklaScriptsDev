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
            NameToDrawingUDAComment.main();
            myModel.CommitChanges();
        }
    }

    public static class NameToDrawingUDAComment
    {
        public static void main()
        {
            DrawingEnumerator selectedDrawings = getSelectedDrawings();

            foreach (Drawing currentDrawing in selectedDrawings)
            {
                double highestScale = getHighestScale(currentDrawing);
                setScaleToComment(currentDrawing, highestScale);
            }

            MessageBox.Show("Valitud " + selectedDrawings.GetSize() + " joonist." + Environment.NewLine +
                    "Muudetud " + selectedDrawings.GetSize() + " joonise UDA 'comment'");

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

            if (disp.AxisX != viewp.AxisX || disp.AxisY != viewp.AxisY)
            {
                return false;
            }

            return true;
        }
		
        private static void setScaleToComment(Drawing currentDrawing, double highestScale)
        {
            string scale = "1:" + highestScale.ToString();
            currentDrawing.SetUserProperty("comment", scale);
            currentDrawing.Modify();
        }
    }
}