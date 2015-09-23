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

            AssemblyToSubAssembly program = new AssemblyToSubAssembly();
            program.main();

            myModel.CommitChanges();
        }
    }

    public class AssemblyToSubAssembly
    {
        private int _wrongPartsCount = 0;

        public void main()
        {
            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedAssemblys = getSelectedAssemblys(selectedObjects);

            foreach (Assembly currentAssembly in selectedAssemblys)
            {
                checkCurrentAssembly(currentAssembly);
            }

            MessageBox.Show("Kontrollitud " + selectedAssemblys.Count.ToString() + " assembly." + Environment.NewLine +
                "Tõstetub ümber " + _wrongPartsCount.ToString() + " vigast elementi.");
        }

        private ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        private ArrayList getSelectedAssemblys(ModelObjectEnumerator selectedObjects)
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

        private void checkCurrentAssembly(Assembly assembly)
        {
            ArrayList wrongParts = findWrongParts(assembly);

            foreach (Part part in wrongParts)
            {
                changePartToSubAssembly(part, assembly);
            }
        }

        private ArrayList findWrongParts(Assembly assembly)
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

        private void changePartToSubAssembly(Part part, Assembly assembly)
        {
            assembly.Remove(part);
            assembly.Modify();
            part.Modify();

            Assembly partNewAssembly = part.GetAssembly() as Assembly;

            assembly.Add(partNewAssembly);
            assembly.Modify();
            _wrongPartsCount += 1;
        }
    }
}
