// Generated by Tekla.Technology.Akit.ScriptBuilder
using System;
using System.Collections;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Drawing;
using System.Windows.Forms;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            DrawingHandler drawingHandler = new DrawingHandler();

            if (drawingHandler.GetConnectionStatus())
            {
                int i = 1;

                Drawing currentDrawing = drawingHandler.GetActiveDrawing();

                if (currentDrawing != null)
                {
                    DrawingObjectEnumerator Enum = currentDrawing.GetSheet().GetViews();


                    while (Enum.MoveNext())
                    {
                        MessageBox.Show(i.ToString());
                        i++;
                        //Tekla.Structures.Drawing.View View = Enum.Current as Tekla.Structures.Drawing.View;
                    }
                }

                //                ArrayList fileText = new ArrayList();
                //                foreach (DrawingObject drawingObject in currentDrawing.GetSheet().GetAllObjects())
                //                {
                //                    if (drawingObject is TextFile)
                //                    {
                //                        fileText.Add(drawingObject);
                //                    }
                //                }

                //                if (fileText.Count > 0)
                //                {
                //                    MessageBox.Show((fileText as TextFile).InsertionPoint.ToString());
                //                    Point pp = new Point(0, 0, 0);
                //                    (fileText as TextFile).InsertionPoint = pp;
                //                }

                foreach (DrawingObject drawingObject in currentDrawing.GetSheet().GetAllObjects())
                {
                    if (drawingObject is TextFile)
                    {
                        MessageBox.Show((drawingObject as TextFile).InsertionPoint.ToString());
                        Point pp = new Point(651, 210, 0);
                        (drawingObject as TextFile).InsertionPoint = pp;
                        (drawingObject as TextFile).Modify();
                    }
                }

            }

        }
    }
}