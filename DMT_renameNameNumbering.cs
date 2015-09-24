using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

//KASUTAMINE:
//Macro asendab PART tyypi elemendi nime kasutaja poolt valitud nimega
//Elemendid valida 1 valikuga
//Võib valida mittu elementi korraga

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            //SETTINGS
            string userPrefix = "EB-1-1";
            int startNumber = 1;
            string stringStyle = "000";
            //(EB-1-1)(000+1) = EB-1-1001

            Model myModel = new Model();
            NameNumbering program = new NameNumbering(userPrefix, startNumber, stringStyle);
            program.main();
            myModel.CommitChanges();
        }
    }

    public class NameNumbering
    {
        public NameNumbering(string userPrefix, int startNumber, string stringStype)
        {
            _userPrefix = userPrefix;
            _currentNumber = startNumber;
            _stringStyle = stringStype;
        }

        private string _userPrefix;
        private int _currentNumber;
        private string _stringStyle;

        public void main()
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

        private ModelObjectEnumerator getSelectedObjects()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            ModelObjectEnumerator selectionEnum = selector.GetSelectedObjects();

            return selectionEnum;
        }

        private ArrayList getSelectedParts(ModelObjectEnumerator selectedObjects)
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

        private void changePartName(Part currentPart)
        {
            string userPos = _userPrefix + _currentNumber.ToString(_stringStyle);
            currentPart.Name = userPos;
            currentPart.Modify();

            _currentNumber++;
        }

    }
}
