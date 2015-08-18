using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new NameToDrawingTitle();
        }
    }

    public class NameToDrawingTitle
    {
        public NameToDrawingTitle()
        {
            DrawingHandler MyDrawingHandler = new DrawingHandler();
            DrawingEnumerator SelectedDrawings = MyDrawingHandler.GetDrawingSelector().GetSelected();

            while (SelectedDrawings.MoveNext())
            {
                var currentDrawing = SelectedDrawings.Current as Drawing;

                MyDrawingHandler.SetActiveDrawing(currentDrawing, false);
                DrawingObjectEnumerator ViewEnum = currentDrawing.GetSheet().GetViews();

                double highestScale = 0;

                while (ViewEnum.MoveNext())
                {
                    var currentView = ViewEnum.Current as Tekla.Structures.Drawing.View;

                    double currentScale = currentView.Attributes.Scale;
                    if (currentScale > highestScale)
                    {
                        highestScale = currentScale;
                    }
                }

                MyDrawingHandler.CloseActiveDrawing(false); 

                currentDrawing.Title3 = "1:" + highestScale.ToString();
                currentDrawing.Modify();
                
            }

            new Model().CommitChanges();

        }
    }
}
