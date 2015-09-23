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
                changeDrawingName(currentDrawing);
            }

            _myModel.CommitChanges();

            MessageBox.Show("Valitud " + selectedDrawings.GetSize() + " joonist." + Environment.NewLine +
                "Muudetud " + selectedCastUnitDrawings.Count.ToString() + " joonise nime");
        }

        private DrawingEnumerator getSelectedDrawings()
        {
            DrawingHandler MyDrawingHandler = new DrawingHandler();
            DrawingEnumerator SelectedDrawings = MyDrawingHandler.GetDrawingSelector().GetSelected();

            return SelectedDrawings;
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

        private void changeDrawingName(CastUnitDrawing currentDrawing)
        {
            var currentModelObject = _myModel.SelectModelObject(currentDrawing.CastUnitIdentifier);
            TSM.Assembly currentAssembly = currentModelObject as Assembly;
            TSM.Part currentMainPart = currentAssembly.GetMainPart() as TSM.Part;
            currentDrawing.Name = currentMainPart.Name;

            currentDrawing.Modify();
        }

    }
}
