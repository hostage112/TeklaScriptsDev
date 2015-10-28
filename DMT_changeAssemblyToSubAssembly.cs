using System;
using System.Collections;
using System.Collections.Generic;
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

    public static class AssemblyToSubAssembly
    {
        public static void main()
        {
            int wrongPartsCount = 0;

            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            List<Assembly> selectedAssemblys = getSelectedAssemblys(selectedObjects);

            foreach (Assembly currentAssembly in selectedAssemblys)
            {
                List<Part> wrongParts = findWrongParts(currentAssembly);
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

        private static List<Assembly> getSelectedAssemblys(ModelObjectEnumerator selectedObjects)
        {
            List<Assembly> selectedAssemblys = new List<Assembly>();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is Assembly)
                {
                    selectedAssemblys.Add(selectedObjects.Current as Assembly);
                }
            }

            return selectedAssemblys;
        }

        private static List<Part> findWrongParts(Assembly assembly)
        {
            Part mainPart = assembly.GetMainPart() as Part;
            ArrayList secondaryParts = new ArrayList(assembly.GetSecondaries());
            List<Part> wrongParts = new List<Part>();

            foreach (Part currentPart in secondaryParts)
            {
                if (currentPart.Class != mainPart.Class)
                {
                    wrongParts.Add(currentPart);
                }
            }

            return wrongParts;
        }

        private static void fixWrongParts(Assembly assembly, List<Part> wrongParts)
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
