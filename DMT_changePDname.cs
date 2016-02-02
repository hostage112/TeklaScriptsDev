using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{

    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new PDLength();
        }
    }

    public class PDLength
    {
        public PDLength()
        {
            Model myModel = new Model();
            if (myModel.GetConnectionStatus())
            {
                ModelObjectEnumerator allObjects = myModel.GetModelObjectSelector().GetAllObjects();

                while (allObjects.MoveNext())
                {
                    if (allObjects.Current is TSM.CustomPart)
                    {
                        TSM.CustomPart currentComponent = allObjects.Current as TSM.CustomPart;
                        if (currentComponent.Name == "EB_PD")
                        {
                            double curWidth = 0.0;
                            double curLength = 0.0;

                            currentComponent.GetUserProperty("w", ref curWidth);
                            currentComponent.GetUserProperty("L", ref curLength);

                            string newName = "PD" + curWidth.ToString() + " L=" + curLength.ToString();
                            currentComponent.SetUserProperty("P1a", newName);
                            currentComponent.Modify();
                        }
                    }
                }
                myModel.CommitChanges();
            }
            else
            {
                MessageBox.Show("Viga yhenduses");
            }
        }
    }
}
