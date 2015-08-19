using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new AssemblyPartsForPrefixErrors();
        }
    }

    public class AssemblyPartsForPrefixErrors
    {
        public AssemblyPartsForPrefixErrors()
        {
            var allWrongParts = new ArrayList();
            
            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var selectedAssemblyEnum = selector.GetSelectedObjects();
            
            while (selectedAssemblyEnum.MoveNext())
            {
                if (selectedAssemblyEnum.Current is Tekla.Structures.Model.Assembly)
                {
                    var currentWrongParts = checkCurrentAssembly(selectedAssemblyEnum.Current as Assembly);
                    allWrongParts.AddRange(currentWrongParts);
                }
            }

            generateReportFromWrongParts(allWrongParts);

            new Model().CommitChanges();
        }

        private static ArrayList checkCurrentAssembly(Assembly assembly)
        {
            var currentWrongParts = new ArrayList();

            var subAssemblyWrongParts = checkSubAssemblys(assembly);
            currentWrongParts.AddRange(subAssemblyWrongParts);
            var currentAssemblyWrongParts = checkPartsOfAssembly(assembly);
            currentWrongParts.AddRange(currentAssemblyWrongParts);

            return currentWrongParts;
        }

        private static ArrayList checkSubAssemblys(Assembly assembly)
        {
            var currentWrongParts = new ArrayList();

            var allSubAssemblys = assembly.GetSubAssemblies();
            if (allSubAssemblys.Count > 0)
            {
                for (int i = 0; i < allSubAssemblys.Count; i++)
                {
                    var subWrongParts = checkCurrentAssembly(allSubAssemblys[i] as Assembly);
                    currentWrongParts.AddRange(subWrongParts);
                }
            }

            return currentWrongParts;
        }

        private static ArrayList checkPartsOfAssembly(Assembly assembly)
        {
            var currentWrongParts = new ArrayList();

            var assemblyPrefix = string.Empty;
            assembly.GetReportProperty("ASSEMBLY_PREFIX", ref assemblyPrefix);

            var allParts = assembly.GetSecondaries();
            allParts.Add(assembly.GetMainPart());

            for (int i = 0; i < allParts.Count; i++)
            {
                var currentPart = allParts[i] as Part;

                var partPrefix = string.Empty;
                currentPart.GetReportProperty("PART_PREFIX", ref partPrefix);

                if (!(partPrefix == assemblyPrefix + getAssumedPartSuffix(currentPart)))
                {
                    currentWrongParts.Add(currentPart);
                }
            }

            return currentWrongParts;
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
            MessageBox.Show("Number of wrong parts: " + (allWrongParts.Count).ToString());

            if (allWrongParts.Count > 0)
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
}
