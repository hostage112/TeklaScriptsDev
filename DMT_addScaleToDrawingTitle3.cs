using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();
            NameToDrawingTitle3.main();
            myModel.CommitChanges();
        }
    }

    public static class NameToDrawingTitle3
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
            DrawingHandler myDrawingHandler = new DrawingHandler();

            myDrawingHandler.SetActiveDrawing(currentDrawing, false);
            DrawingObjectEnumerator ViewEnum = currentDrawing.GetSheet().GetViews();
            
            foreach (TSD.View currentView in ViewEnum)
            {
                double currentScale = currentView.Attributes.Scale;
                highestScale = Math.Max(currentScale, highestScale);
            }

            myDrawingHandler.CloseActiveDrawing(false);

            return highestScale;
        }

        private static void setScaleToTitle3(Drawing currentDrawing, double highestScale)
        {
            currentDrawing.Title3 = "1:" + highestScale.ToString();
            currentDrawing.Modify();
        }
    }
}