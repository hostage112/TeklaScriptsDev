using System;
using System.Collections;
using System.Diagnostics;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new AssemblyToSubAssembly();
        }
    }

    public class AssemblyToSubAssembly
    {
        public AssemblyToSubAssembly()
        {
            //Get selected objects and put them in an enumerator/container
            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var selectedAssemblyEnum = selector.GetSelectedObjects();

            //Cycle through selected objects
            while (selectedAssemblyEnum.MoveNext())
            {
                if (selectedAssemblyEnum.Current is Tekla.Structures.Model.Assembly)
                {
                    var currentAssembly = selectedAssemblyEnum.Current as Assembly;
                    checkSecondaryParts(currentAssembly);
                }
            }

            new Model().CommitChanges();

        }

        private static void checkSecondaryParts(Assembly assembly)
        {
            var mainPart = assembly.GetMainPart() as Part;
            var secondaryParts = assembly.GetSecondaries();

            for (int i = secondaryParts.Count - 1; i >= 0; i--)
            {
                var currentPart = secondaryParts[i] as Part;
                if (currentPart.Name != mainPart.Name)
                {
                    changePartAssemblyToSubAssembly(currentPart, assembly);
                }
            }
        }

        private static void changePartAssemblyToSubAssembly(Part part, Assembly assembly)
        {
            assembly.Remove(part);
            assembly.Modify();
            var partNewAssembly = part.GetAssembly() as Assembly;
            assembly.Add(partNewAssembly);
            assembly.Modify();
        }
    }
}
