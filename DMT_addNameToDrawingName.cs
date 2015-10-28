using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            NameToDrawingTitle program = new NameToDrawingTitle();
            program.main();

        }
    }

    public class NameToDrawingTitle
    {
        private Model _myModel = new TSM.Model();

        public void main()
        {
            DrawingEnumerator selectedDrawings = getSelectedDrawings();
            ArrayList selectedCastUnitDrawings = getSelectedCastUnitDrawing(selectedDrawings);

            foreach (CastUnitDrawing currentDrawing in selectedCastUnitDrawings)
            {
                string objectName = getObjectName(currentDrawing);
                changeDrawingName(currentDrawing, objectName);
            }

            _myModel.CommitChanges();

            MessageBox.Show("Valitud " + selectedDrawings.GetSize() + " joonist." + Environment.NewLine +
                "Muudetud " + selectedCastUnitDrawings.Count.ToString() + " joonise nime");
        }

        private DrawingEnumerator getSelectedDrawings()
        {
            DrawingHandler myDrawingHandler = new DrawingHandler();
            DrawingEnumerator selectedDrawings = myDrawingHandler.GetDrawingSelector().GetSelected();

            return selectedDrawings;
        }

        private ArrayList getSelectedCastUnitDrawing(DrawingEnumerator selectedDrawings)
        {
            ArrayList selectedCastUnitDrawings = new ArrayList();

            while (selectedDrawings.MoveNext())
            {
                if (selectedDrawings.Current is CastUnitDrawing)
                {
                    selectedCastUnitDrawings.Add(selectedDrawings.Current);
                }
            }

            return selectedCastUnitDrawings;
        }

        private string getObjectName(CastUnitDrawing currentDrawing)
        {
            var currentModelObject = _myModel.SelectModelObject(currentDrawing.CastUnitIdentifier);
            TSM.Assembly currentAssembly = currentModelObject as TSM.Assembly;
            TSM.Part currentMainPart = currentAssembly.GetMainPart() as TSM.Part;

            return currentMainPart.Name;
        }

        private void changeDrawingName(CastUnitDrawing currentDrawing, string objectName)
        {
            currentDrawing.Name = objectName;
            currentDrawing.Modify();
        }
    }
}
