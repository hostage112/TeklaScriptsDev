using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

using System.IO;
using System.Text;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {

        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            string fileName = @"test.txt";

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch { }


            int drawingNr = 1;
            DrawingHandler myDrawingHandler = new DrawingHandler();
            DrawingEnumerator selectedDrawings = myDrawingHandler.GetDrawingSelector().GetSelected();

            var csv = new StringBuilder();

            foreach (Drawing currentDrawing in selectedDrawings)
            {
                string name = currentDrawing.Title1;
                string nr = currentDrawing.Name;

                string dmtDate = "";
                currentDrawing.GetUserProperty("DR_RESP_DSGNR_DATE", ref dmtDate);

                string newLine = string.Format("{0};{1};{2}", name, nr, dmtDate);
                csv.AppendLine(newLine);

                //string mark = "";
                //DateLastMark(currentDrawing, ref mark);


                drawingNr++;
            }


            //File.WriteAllText(fileName, csv.ToString());

            //if (File.Exists(fileName))
            //{
            //    Process.Start(fileName);
            //}
            
        }

        //public static string DateLastMark(Drawing croquis, out string mark)
        //{
        //    DrawingHandler drawingHandler = new DrawingHandler();
        //    Type drawingType = croquis.GetType();
        //    PropertyInfo propertyInfo = drawingType.GetProperty("Identifier", BindingFlags.Instance | BindingFlags.NonPublic);
        //    object value = propertyInfo.GetValue(croquis, null);

        //    Identifier identifier = (Identifier)value;
        //    Beam fakeBeam = new Beam { Identifier = identifier };
        //    string RevisionMark = "";
        //    fakeBeam.GetReportProperty("REVISION.LAST_MARK", ref RevisionMark);

        //}
    }
}
