using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

//KASUTAMINE:
//Macro asendab PART tyypi elemendi nime tema enda Position Numbriga
//Elemendid valida 1 valikuga
//Võib valida mittu elementi korraga

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            Model myModel = new Model();
            PositionNumberToName.main();
            myModel.CommitChanges();
        }
    }

    public static class PositionNumberToName
    {
        public static void main()
        {
            ModelObjectEnumerator selectedObjects = getSelectedObjects();
            ArrayList selectedParts = getSelectedParts(selectedObjects);

            foreach (Part currentPart in selectedParts)
            {
                changePartName(currentPart);
            }

            MessageBox.Show("Valitud " + selectedObjects.GetSize() + " objekti." + Environment.NewLine +
                "Muudetud " + selectedParts.Count.ToString() + " elemendi nime.");
        }

        private static ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        private static ArrayList getSelectedParts(ModelObjectEnumerator selectedObjects)
        {
            ArrayList selectedParts = new ArrayList();

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is Part)
                {
                    selectedParts.Add(selectedObjects.Current);
                }
            }

            return selectedParts;
        }

        private static void changePartName(Part currentPart)
        {
            var castUnitPos = string.Empty;
            currentPart.GetReportProperty("CAST_UNIT_POS", ref castUnitPos);

            castUnitPos = castUnitPos.Replace("(?)", "");

            currentPart.Name = castUnitPos;
            currentPart.Modify();
        }

    }
}
