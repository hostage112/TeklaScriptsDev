using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();
            AssemblyPartsForPrefixErrors.main();
            myModel.CommitChanges();
        }
    }

    public class AssemblyPartsForPrefixErrors
    {
        public static void main()
        {
            ArrayList allWrongParts = new ArrayList();

            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedAssemblys = getSelectedAssemblys(selectedObjects);


            foreach (Assembly currentAssembly in selectedAssemblys)
            {
                var currentWrongParts = checkCurrentAssembly(currentAssembly);
                allWrongParts.AddRange(currentWrongParts);
            }

            if (allWrongParts.Count > 0)
            {
                generateReportFromWrongParts(allWrongParts);
            }

            MessageBox.Show("Kontrollitud " + selectedAssemblys.Count.ToString() + " assembly." + Environment.NewLine +
                "Leitud " + allWrongParts.Count + " vigast elementi.");
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

        private static ArrayList checkCurrentAssembly(Assembly assembly)
        {
            ArrayList currentWrongParts = new ArrayList();

            ArrayList subAssemblyWrongParts = checkSubAssemblys(assembly);
            currentWrongParts.AddRange(subAssemblyWrongParts);
            ArrayList currentAssemblyWrongParts = checkPartsOfAssembly(assembly);
            currentWrongParts.AddRange(currentAssemblyWrongParts);

            return currentWrongParts;
        }

        private static ArrayList checkSubAssemblys(Assembly assembly)
        {
            ArrayList currentWrongParts = new ArrayList();
            ArrayList allSubAssemblys = assembly.GetSubAssemblies();

            foreach (Assembly subAssembly in allSubAssemblys)
            {
                ArrayList subWrongParts = checkCurrentAssembly(subAssembly);
                currentWrongParts.AddRange(subWrongParts);
            }

            return currentWrongParts;
        }

        private static ArrayList checkPartsOfAssembly(Assembly assembly)
        {
            ArrayList currentWrongParts = new ArrayList();

            string assemblyPrefix = string.Empty;
            assembly.GetReportProperty("ASSEMBLY_PREFIX", ref assemblyPrefix);

            ArrayList allParts = assembly.GetSecondaries();
            allParts.Add(assembly.GetMainPart());

            foreach (Part currentPart in allParts)
            {
                string partPrefix = string.Empty;
                currentPart.GetReportProperty("PART_PREFIX", ref partPrefix);
                string assumedPrefix = assemblyPrefix + getAssumedPartSuffix(currentPart);

                if (isWrong(partPrefix, assumedPrefix))
                {
                    currentWrongParts.Add(currentPart);
                }
            }

            return currentWrongParts;
        }

        private static bool isWrong(string partPrefix, string assumedPrefix)
        {
            return !(partPrefix == assumedPrefix);
        }

        private static String getAssumedPartSuffix(Part part)
        {
            if (part is Tekla.Structures.Model.Beam)
            {
                var partProfile = (part.Profile as Profile).ProfileString;

                if (partProfile.Contains("BL"))
                {
                    return "S";
                }
                if (partProfile.Contains("PL"))
                {
                    return "S";
                }

                return "P";
                
            }
            if (part is Tekla.Structures.Model.ContourPlate)
            {
                return "S";
            }

            return "KONTROLLIDA";

        }

        private static void generateReportFromWrongParts(ArrayList allWrongParts)
        {
            var ModelSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelSelector.Select(allWrongParts);
                
            try
            {
                string path = @"Reports";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                Operation.CreateReportFromSelected("P_Select_Part_Position_with_ID", "error_report.xsr", "Report for Assembly prefix errors", "", "");
                Operation.DisplayReport("error_report.xsr");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
