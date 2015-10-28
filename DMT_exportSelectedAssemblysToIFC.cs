using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            SelectedAssemblysToIFC.main();
        }

        public static class SelectedAssemblysToIFC
        {
            private static bool exportIFC(string outputFileName)
            {

                ComponentInput ComponentInput = new ComponentInput();
                ComponentInput.AddOneInputPosition(new Point(0, 0, 0));
                Component Comp = new Component(ComponentInput);
                Comp.Name = "ExportIFC";
                Comp.Number = BaseComponent.PLUGIN_OBJECT_NUMBER;

                // Parameters
                Comp.SetAttribute("OutputFile", outputFileName);
                Comp.SetAttribute("Format", 0);
                Comp.SetAttribute("ExportType", 1);
                Comp.SetAttribute("AdditionalPSets", "BMcDIFCConfig");
                Comp.SetAttribute("CreateAll", 0);  // 0 to export only selected objects

                // Advanced
                Comp.SetAttribute("Assemblies", 1);
                Comp.SetAttribute("Bolts", 1);
                Comp.SetAttribute("Welds", 0);
                Comp.SetAttribute("SurfaceTreatments", 1);

                Comp.SetAttribute("BaseQuantities", 1);
                Comp.SetAttribute("GridExport", 1);
                Comp.SetAttribute("ReinforcingBars", 1);
                Comp.SetAttribute("PourObjects", 1);

                Comp.SetAttribute("LayersNameAsPart", 1);
                Comp.SetAttribute("PLprofileToPlate", 0);
                Comp.SetAttribute("ExcludeSnglPrtAsmb", 0);

                //Comp.SetAttribute("LocsFromOrganizer", 0); // dafuq


                return Comp.Insert();
            }

            public static void main()
            {
                ModelObjectEnumerator selectedObjects = getSelectedObjects();
                List<Assembly> selectedAssemblys = getSelectedAssemblys(selectedObjects);

                foreach (Assembly currentAssembly in selectedAssemblys)
                {
                    exportCurrentAssembly(currentAssembly);
                }

                selectInitialObjects(selectedAssemblys);

                MessageBox.Show("done");

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

            private static void exportCurrentAssembly(Assembly assembly)
            {
                ArrayList currentAssemblySelect = new ArrayList();
                currentAssemblySelect.Add(assembly);

                var selector = new TSM.UI.ModelObjectSelector();
                selector.Select(currentAssemblySelect);

                string name = getFileName(assembly);
                exportIFC(name);
            }

            private static string getFileName(Assembly assembly)
            {
                assembly.GetReportProperty("ASSEMBLY_POS", ref position);
                //position = position.Replace("/", "_");
                //position = position.Replace("(?)", "_");

                if (position.Contains("/") || position.Contains("?") || position.Contains("(") || position.Contains(")"))
                {
                    MessageBox.Show("Element sisaldab keelatuid märke" + Environment.NewLine + position);
                }
            }

            private static void selectInitialObjects(List<Assembly> selectedAssemblys)
            {
                ArrayList initialSelectedAssemblys = new ArrayList(selectedAssemblys);

                var selector = new TSM.UI.ModelObjectSelector();
                selector.Select(initialSelectedAssemblys);
            }
        }
    }
}
