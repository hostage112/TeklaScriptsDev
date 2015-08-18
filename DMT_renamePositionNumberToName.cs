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
            new PositionNumberToName();
        }
    }

    public class PositionNumberToName
    {
        public PositionNumberToName()
        {
            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var myEnum = selector.GetSelectedObjects();

            while (myEnum.MoveNext())
            {
                if (myEnum.Current is Tekla.Structures.Model.Part)
                {
                    var myPart = myEnum.Current as Part;

                    var castUnitPos = string.Empty;
                    myPart.GetReportProperty("CAST_UNIT_POS", ref castUnitPos);
                    myPart.Name = castUnitPos;
                    myPart.Modify();
                }
            }

            new Model().CommitChanges();

        }
    }
}
