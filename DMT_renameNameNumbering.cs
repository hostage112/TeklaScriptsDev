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
            new NameNumbering();
        }
    }

    public class NameNumbering
    {
        public NameNumbering()
        {
            Model MyModel = new Model();

            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var myEnum = selector.GetSelectedObjects();

            string userPrefix = "EB-1-1";
            int userNumber = 1;

            while (myEnum.MoveNext())
            {
                if (myEnum.Current is Tekla.Structures.Model.Part)
                {
                    var currentPart = myEnum.Current as Part;
                    string userPos = userPrefix + userNumber.ToString("000");
                    currentPart.Name = userPos;
                    currentPart.Modify();

                    userNumber++;
                }
            }

            MyModel.CommitChanges();
        }
    }
}
