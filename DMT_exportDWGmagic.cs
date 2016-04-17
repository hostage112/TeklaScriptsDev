using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

using System.IO;

namespace Tekla.Technology.Akit.UserScript
{
    public static class PrintToSize
    {
        public static string paperSize(Drawing currentDrawing)
        {
            string paper = "";
            int height = Convert.ToInt32(currentDrawing.Layout.SheetSize.Height);
            int width = Convert.ToInt32(currentDrawing.Layout.SheetSize.Width);

            if (width == 210 && height == 297)
                paper = "a4";
            else if ((width % 420 == 0) && height == 297)
                paper = "a3";
            else if ((width % 594 == 0) && height == 420)
                paper = "a2";
            else if ((width % 841 == 0) && height == 594)
                paper = "a1";
            else if ((width % 1189 == 0) && height == 841)
                paper = "a0";

            return paper;
        }

        public static string selectPrinter(string size)
        {
            if (size == "a4")
                return "PDFactoryA4";
            else if (size == "a3")
                return "PDFactoryA3";
            else if (size == "a2")
                return "PDFactoryA2";
            else if (size == "a1")
                return "PDFactoryA1";

            return "PDFactoryA0";
        }
    }

    public class Script
    {

        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            string fileName = "_macroPrintReport.txt";

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch { }


            int drawingNr = 1;
            DrawingEnumerator selectedDrawings = ExportToScale.getSelectedDrawings();

            foreach (Drawing currentDrawing in selectedDrawings)
            {
                string report = "";
                akit.TableSelect("Drawing_selection", "dia_draw_select_list", drawingNr);

                string elementMark = ExportToScale.getMark(currentDrawing);
                double scaleFactor = ExportToScale.getHighestScale(currentDrawing);
                double lineTypeScale = scaleFactor / 4;

                akit.PopupCallback("acmdDisplayExportDrawingsDialog", "", "Drawing_selection", "dia_draw_select_list");
                akit.ValueChange("diaExportDrawings", "textScaleFactor", scaleFactor.ToString());
                akit.ValueChange("diaExportDrawings", "txtLineTypeScale", lineTypeScale.ToString());
                akit.PushButton("butExport", "diaExportDrawings");

                string paper = PrintToSize.paperSize(currentDrawing);
                string printerName = "PRINTER_NOT_FOUND";
                if (!String.IsNullOrEmpty(paper))
                {
                    printerName = PrintToSize.selectPrinter(paper);

                    akit.PopupCallback("acmd_display_plot_dialog", "", "Drawing_selection", "dia_draw_select_list");
                    akit.ListSelect("Plot", "component_list", printerName);
                    akit.PushButton("butPrint", "Plot");
                }
                else
                {
                    printerName = "PRINTER_NOT_FOUND";
                }

                report += elementMark + " ";
                report += "SCALE: " + scaleFactor.ToString() + " ";
                report += lineTypeScale.ToString() + " ";
                report += "PRINTER: " + printerName;

                System.IO.File.AppendAllLines(fileName, new[] { report });

                drawingNr++;
            }

            akit.PushButton("cancel_pb", "Plot");
            akit.PushButton("butCancel", "diaExportDrawings");

            if (File.Exists(fileName))
            {
                Process.Start(fileName);
            }
            
        }
    }

    public static class ExportToScale
    {
        public static DrawingEnumerator getSelectedDrawings()
        {
            DrawingHandler myDrawingHandler = new DrawingHandler();
            DrawingEnumerator selectedDrawings = myDrawingHandler.GetDrawingSelector().GetSelected();

            return selectedDrawings;
        }
        public static string getMark(Drawing currentDrawing)
        {
            return currentDrawing.Mark;
        }

        public static double getHighestScale(Drawing currentDrawing)
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
    }
}