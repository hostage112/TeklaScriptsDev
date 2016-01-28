using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new NameToDrawingTitle();
            akit.Callback("acmdViewTopInFormFace", "", "View_01 window_1");
        }
    }

    public class NameToDrawingTitle
    {
        public NameToDrawingTitle()
        {
            Model myModel = new Model();
            if (myModel.GetConnectionStatus())
            {
                ArrayList selectedParts = new ArrayList();
                ModelObjectEnumerator allObjects = myModel.GetModelObjectSelector().GetAllObjects();

                while (allObjects.MoveNext())
                {
                    if (allObjects.Current is Part)
                    {
                        selectedParts.Add(allObjects.Current);
                    }
                }

                Tekla.Structures.Model.UI.ModelObjectSelector MS = new Tekla.Structures.Model.UI.ModelObjectSelector();
                MS.Select(selectedParts);
            }
            else
            {
                MessageBox.Show("Viga yhenduses");
            }
        }
    }
}
