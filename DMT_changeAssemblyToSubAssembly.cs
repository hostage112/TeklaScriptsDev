using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();
            AssemblyToSubAssembly.main();
            myModel.CommitChanges();
        }
    }

    public class AssemblyToSubAssembly
    {
        public static void main()
        {
            int wrongPartsCount = 0;

            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedAssemblys = getSelectedAssemblys(selectedObjects);

            foreach (Assembly currentAssembly in selectedAssemblys)
            {
                ArrayList wrongParts = findWrongParts(currentAssembly);
                fixWrongParts(currentAssembly, wrongParts);
                wrongPartsCount += wrongParts.Count;
            }

            MessageBox.Show("Kontrollitud " + selectedAssemblys.Count.ToString() + " assembly." + Environment.NewLine +
                "Tõstetub ümber " + wrongPartsCount.ToString() + " vigast elementi.");
        }

        private static ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        private static ArrayList getSelectedAssemblys(ModelObjectEnumerator selectedObjects)
        {
            ArrayList selectedAssemblys = new ArrayList();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is Assembly)
                {
                    selectedAssemblys.Add(selectedObjects.Current);
                }
            }

            return selectedAssemblys;
        }

        private static ArrayList findWrongParts(Assembly assembly)
        {
            Part mainPart = assembly.GetMainPart() as Part;
            ArrayList secondaryParts = new ArrayList(assembly.GetSecondaries());
            ArrayList wrongParts = new ArrayList();

            foreach (Part currentPart in secondaryParts)
            {
                if (currentPart.Class != mainPart.Class)
                {
                    wrongParts.Add(currentPart);
                }
            }

            return wrongParts;
        }

        private static void fixWrongParts(Assembly assembly, ArrayList wrongParts)
        {
            foreach (Part part in wrongParts)
            {
                changePartToSubAssembly(part, assembly);
            }
        }

        private static void changePartToSubAssembly(Part part, Assembly assembly)
        {
            assembly.Remove(part);
            assembly.Modify();
            part.Modify();

            Assembly partNewAssembly = part.GetAssembly() as Assembly;

            assembly.Add(partNewAssembly);
            assembly.Modify();
        }
    }
}
