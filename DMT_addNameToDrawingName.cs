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
            Model MyModel = new Model();

            DrawingHandler MyDrawingHandler = new DrawingHandler();
            DrawingEnumerator SelectedDrawings = MyDrawingHandler.GetDrawingSelector().GetSelected();

            while (SelectedDrawings.MoveNext())
            {
                if (SelectedDrawings.Current is Tekla.Structures.Drawing.CastUnitDrawing)
                {
                    var currentCastUnitDrawing = SelectedDrawings.Current as Tekla.Structures.Drawing.CastUnitDrawing;
                    var currentModelObject = MyModel.SelectModelObject(currentCastUnitDrawing.CastUnitIdentifier);
                    var currentAssembly = currentModelObject as Tekla.Structures.Model.Assembly;
                    var currentMainPart = currentAssembly.GetMainPart() as Tekla.Structures.Model.Part;
                    currentCastUnitDrawing.Name = currentMainPart.Name;
                    currentCastUnitDrawing.Modify();
                }
                else
                {
                    MessageBox.Show("Vale joonise tyyp");
                }

            }

            MyModel.CommitChanges();

        }
    }
}
