using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using TSM = Tekla.Structures.Model;

using Tekla.Structures.Geometry3d;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {


            TSM.UI.ModelViewEnumerator views = TSM.UI.ViewHandler.GetSelectedViews();

            if (views.Count > 0)
            {
                while (views.MoveNext())
                {
                    TSM.UI.View activeView = views.Current;
                    TSM.UI.ModelObjectSelector selector = new Tekla.Structures.Model.UI.ModelObjectSelector();

                    ArrayList visibleParts = ItemSelector.getParts(activeView);
                    selector.Select(visibleParts);

                    akit.Callback("acmdViewTopInFormFace", "", "View_01 window_1");

                    ArrayList empty = new ArrayList();
                    selector.Select(empty);
                }
            }
            else
            {
                MessageBox.Show("Vaate aken peab olema aktiivne");
            }

        }
    }

    public static class ItemSelector
    {
        public static ArrayList getParts(Tekla.Structures.Model.UI.View view)
        {
            ArrayList visibleParts = new ArrayList();

            Point min = new Point(-999999, -999999, -999999);
            Point max = new Point(999999, 999999, 999999);

            TSM.UI.ModelObjectSelector selector = new TSM.UI.ModelObjectSelector();
            TSM.ModelObjectEnumerator allObjects = selector.GetObjectsByBoundingBox(min, max, view);

            while (allObjects.MoveNext())
            {
                if (allObjects.Current is TSM.Part)
                {
                    if ((allObjects.Current as TSM.Part).PartNumber.Prefix.StartsWith("Concrete_"))
                    {
                        visibleParts.Add(allObjects.Current);
                    }
                        
                }
            }

            return visibleParts;
        }
    }
}
