using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

using System.Windows.Forms;


namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();

            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedParts = getSelectedBooleans(selectedObjects);

            foreach (TSM.BooleanPart currentBoolean in selectedParts)
            {
                changeCut(currentBoolean);
            }

            myModel.CommitChanges();
        }

        static private ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        static private ArrayList getSelectedBooleans(ModelObjectEnumerator selectedObjects)
        {
            ArrayList selectedParts = new ArrayList();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is TSM.BooleanPart)
                {
                    selectedParts.Add(selectedObjects.Current);
                }
            }

            return selectedParts;
        }

        static private void changeCut(TSM.BooleanPart currentBoolean)
        {
            TSM.Part cutPart = currentBoolean.OperativePart as TSM.Part;
            double minCutZ = findMinimum(cutPart);

            TSM.Part realPart = currentBoolean.Father as TSM.Part;
            double minPartZ = findMinimum(realPart);

            double delta =  minPartZ - minCutZ;

            setCutPoints(cutPart, delta);
        }

        static private double findMinimum(TSM.Part current)
        {
            double min = Double.MaxValue;

            if (current is TSM.ContourPlate)
            {
                TSM.ContourPlate currentCP = current as TSM.ContourPlate;

                foreach (Point cur in currentCP.Contour.ContourPoints)
                {
                    if (min > cur.Z)
                    {
                        min = cur.Z;
                    }
                }
            }
            else if (current is TSM.Beam)
            {
                TSM.Beam currentBeam = current as TSM.Beam;
                min = Math.Min(currentBeam.StartPoint.Z, currentBeam.EndPoint.Z);
            }

            return min;
        }

        static private void setCutPoints(TSM.Part current, double delta)
        {
            if (current is TSM.ContourPlate)
            {
                TSM.ContourPlate currentCP = current as TSM.ContourPlate;

                foreach (Point cur in currentCP.Contour.ContourPoints)
                {
                    cur.Z += delta;
                }
                if (delta <= 0)
                {
                    if (currentCP.Position.DepthOffset >= 0)
                    {
                        currentCP.Position.Depth = Position.DepthEnum.FRONT;
                    }
                    else
                    {
                        currentCP.Position.Depth = Position.DepthEnum.BEHIND;
                    }
                }
                else
                {
                    if (currentCP.Position.DepthOffset >= 0)
                    {
                        currentCP.Position.Depth = Position.DepthEnum.BEHIND;
                    }
                    else
                    {
                        currentCP.Position.Depth = Position.DepthEnum.FRONT;
                    }
                }

                currentCP.Position.DepthOffset = -2.5;


            }

            current.Modify();
        }
    }
}
