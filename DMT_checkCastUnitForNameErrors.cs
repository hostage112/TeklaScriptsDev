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

            var allParts = assembly.GetSecondaries();
            allParts.Add(assembly.GetMainPart());
            var allSubAssemblys = assembly.GetSubAssemblies();

            for (int i = 0; i < allParts.Count; i++)
            {
                var currentPart = allParts[i] as Part;

                if (currentPart.Name != assembly.Name)
                {
                    currentWrongParts.Add(currentPart);
                }
            }

            for (int i = 0; i < allSubAssemblys.Count; i++)
            {
                Assembly currentSubAssembly = allSubAssemblys[i] as Assembly;
                Part currentSubMainPart = currentSubAssembly.GetMainPart() as Part;

                if (currentSubMainPart.Name != currentSubAssembly.Name)
                {
                    currentWrongParts.Add(currentSubMainPart);
                }
            }

            return currentWrongParts;
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
