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
            string fileName = @"Reports/test.csv";

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

                DateTime dmtDate = new DateTime(1970, 1, 1);
                int dmtDateSeconds = 0;
                currentDrawing.GetUserProperty("DR_RESP_DSGNR_DATE", ref dmtDateSeconds);
                dmtDate = dmtDate.AddSeconds(dmtDateSeconds);

                DateTime revisionDate = new DateTime(1970, 1, 1);
                int revisionDateSeconds = 0;
                string revisionMark = "";
                DateLastMark(currentDrawing, out revisionMark, out revisionDateSeconds);
                revisionDate = revisionDate.AddSeconds(revisionDateSeconds);

                string newLine = string.Format("{0} ;{1} ;{2} ;{3} ;{4} ;", name, nr, revisionMark, dmtDate.ToShortDateString(), revisionDate.ToShortDateString());
                csv.AppendLine(newLine);

                //MessageBox.Show(newLine);

                drawingNr++;
            }

            try
            {
                File.WriteAllText(fileName, csv.ToString());
            }
            catch
            {
                MessageBox.Show("write failed");
            }
            
        }

        public static void DateLastMark(Drawing croquis, out string revisionMark, out int revisionDateSeconds)
        {
            DrawingHandler drawingHandler = new DrawingHandler();
            Type drawingType = croquis.GetType();
            PropertyInfo propertyInfo = drawingType.GetProperty("Identifier", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = propertyInfo.GetValue(croquis, null);
            
            Identifier identifier = (Identifier)value;
            Beam fakeBeam = new Beam { Identifier = identifier };

            revisionMark = "";
            fakeBeam.GetReportProperty("REVISION.LAST_MARK", ref revisionMark);

            revisionDateSeconds = 0;
            fakeBeam.GetReportProperty("REVISION.LAST_DATE_CREATE", ref revisionDateSeconds);
        }
    }
}
