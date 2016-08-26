using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using TSM = Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            TSM.Model myModel = new TSM.Model();
            AssemblyToSubAssembly.main(akit);
            myModel.CommitChanges();
        }
    }

    public static class AssemblyToSubAssembly
    {
        public static void main(Tekla.Technology.Akit.IScript akit)
        {
            int wrongPartsCount = 0;

            TSM.ModelObjectEnumerator selectedObjects = getSelectedObjects();
            List<TSM.Assembly> selectedAssemblys = getSelectedAssemblys(selectedObjects);

            foreach (TSM.Assembly currentAssembly in selectedAssemblys)
            {
                List<TSM.Part> wrongParts = findWrongParts(currentAssembly);

				if (wrongParts.Count > 0)
				{                
					fixWrongParts(currentAssembly, wrongParts, akit);
					
					TSM.Part mainpart = currentAssembly.GetMainPart() as TSM.Part;
					if (currentAssembly.Name == mainpart.Name)
					{
						removeAssemblyPropertys(currentAssembly, akit);
					}
				}          
				
				wrongPartsCount += wrongParts.Count;
				
            }

            MessageBox.Show("Kontrollitud " + selectedAssemblys.Count.ToString() + " assembly." + Environment.NewLine +
                "Tõstetub ümber " + wrongPartsCount.ToString() + " vigast elementi.");
        }

        private static TSM.ModelObjectEnumerator getSelectedObjects()
        {
            TSM.UI.ModelObjectSelector selector = new TSM.UI.ModelObjectSelector();
            TSM.ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        private static List<TSM.Assembly> getSelectedAssemblys(TSM.ModelObjectEnumerator selectedObjects)
        {
            List<TSM.Assembly> selectedAssemblys = new List<TSM.Assembly>();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is TSM.Assembly)
                {
                    selectedAssemblys.Add(selectedObjects.Current as TSM.Assembly);
                }
            }

            return selectedAssemblys;
        }

        private static List<TSM.Part> findWrongParts(TSM.Assembly assembly)
        {
            TSM.Part mainPart = assembly.GetMainPart() as TSM.Part;
            ArrayList secondaryParts = new ArrayList(assembly.GetSecondaries());
            List<TSM.Part> wrongParts = new List<TSM.Part>();

            foreach (TSM.Part currentPart in secondaryParts)
            {
                if (currentPart.Class != mainPart.Class)
                {
                    wrongParts.Add(currentPart);
                }
            }

            return wrongParts;
        }

        private static void fixWrongParts(TSM.Assembly assembly, List<TSM.Part> wrongParts, Tekla.Technology.Akit.IScript akit)
        {
            foreach (TSM.Part part in wrongParts)
            {
                fixPart(part, assembly);
                removeAssemblyPropertys(part.GetAssembly() as TSM.Assembly, akit);
            }
        }

        private static ArrayList fixPart(TSM.Part part, TSM.Assembly assembly)
        {
            ArrayList removedAssemblys = new ArrayList();

            assembly.Remove(part);
            assembly.Modify();

            TSM.Assembly removed = part.GetAssembly() as TSM.Assembly;

            assembly.Add(removed);
            assembly.Modify();

            return removedAssemblys;
        }

        private static void removeAssemblyPropertys(TSM.Assembly currentAssembly, Tekla.Technology.Akit.IScript akit)
        {
            ArrayList currentSelection = new ArrayList();
            currentSelection.Add(currentAssembly);

            TSM.UI.ModelObjectSelector selector = new TSM.UI.ModelObjectSelector();
            selector.Select(currentSelection);

            akit.Callback("acmd_display_selected_object_dialog", "", "View_01 window_1");
            TSM.Part currentMainPart = currentAssembly.GetMainPart() as TSM.Part;

            if (!currentMainPart.Material.MaterialString.StartsWith("C"))
            {
                akit.ValueChange("steelassembly_1", "AssemblyPrefix", "");
                akit.ValueChange("steelassembly_1", "AssemblyStartNumber", "");
                akit.ValueChange("steelassembly_1", "AssemblyName", "");
                akit.PushButton("modify_button", "steelassembly_1");
                akit.PushButton("OK_button", "steelassembly_1");
            }
            else
            {
                if (currentMainPart.CastUnitType == 0)
                {
                    akit.ValueChange("precastassembly_1", "AssemblyPrefix", "");
                    akit.ValueChange("precastassembly_1", "AssemblyStartNumber", "");
                    akit.ValueChange("precastassembly_1", "AssemblyName", "");
                    akit.PushButton("modify_button", "precastassembly_1");
                    akit.PushButton("OK_button", "precastassembly_1");
                }
                else
                {
                    akit.ValueChange("insituassembly_1", "AssemblyPrefix", "");
                    akit.ValueChange("insituassembly_1", "AssemblyStartNumber", "");
                    akit.ValueChange("insituassembly_1", "AssemblyName", "");
                    akit.PushButton("modify_button", "insituassembly_1");
                    akit.PushButton("OK_button", "insituassembly_1");
                }
            }
        }
    }
}
