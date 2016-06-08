using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;

using System.Windows.Forms;


namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();

            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedParts = getSelectedRebarGroups(selectedObjects);

            foreach (TSM.RebarGroup currentRebarGroup in selectedParts)
            {
                ungroupRebar(currentRebarGroup);
            }
            myModel.CommitChanges();
        }

        static private ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        static private ArrayList getSelectedRebarGroups(ModelObjectEnumerator selectedObjects)
        {
            ArrayList selectedParts = new ArrayList();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is TSM.RebarGroup)
                {
                    selectedParts.Add(selectedObjects.Current);
                }
            }

            return selectedParts;
        }

        static private void ungroupRebar(TSM.RebarGroup current)
        {
            ArrayList currentPolygons = current.GetRebarGeometries(true);
            foreach (TSM.RebarGeometry geo in currentPolygons)
            {
                TSM.Polygon poly = new TSM.Polygon();
                PolyLine line = geo.Shape;
                ArrayList points = line.Points;

                poly.Points = points;

                SingleRebar single = new SingleRebar();
                single.Polygon = poly;
                single.Father = current.Father;

                single.Name = current.Name;

                single.Class = current.Class;
                single.Size = current.Size;
                single.Grade = current.Grade;
                single.RadiusValues = current.RadiusValues;
				
				single.NumberingSeries.StartNumber = current.NumberingSeries.StartNumber;
				single.NumberingSeries.Prefix = current.NumberingSeries.Prefix;

                single.OnPlaneOffsets = new ArrayList();
                single.OnPlaneOffsets.Add(0.0);
                single.StartHook.Angle = -90;
                single.StartHook.Length = 10;
                single.StartHook.Radius = 10;
                single.StartHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
                single.EndHook.Angle = 90;
                single.EndHook.Length = 10;
                single.EndHook.Radius = 10;
                single.EndHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;

                single.Insert();
            }

	        current.Delete();
            MessageBox.Show(currentPolygons.Count.ToString());
        }
    }
}
