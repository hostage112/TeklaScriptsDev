using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;
using TSM = Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {

        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            new Magic();
        }
    }

    public class Magic
    {
        public Magic()
        {
            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var myEnum = selector.GetSelectedObjects();

            while (myEnum.MoveNext())
            {
                if (myEnum.Current is Tekla.Structures.Model.Beam)
                {
                    var myBeam = myEnum.Current as Beam;
                    double dX = Math.Abs(myBeam.StartPoint.X - myBeam.EndPoint.X);
                    double dY = Math.Abs(myBeam.StartPoint.Y - myBeam.EndPoint.Y);
                    double dZ = Math.Abs(myBeam.StartPoint.Z - myBeam.EndPoint.Z);
                    //MessageBox.Show(dX.ToString() + ", " + dY.ToString() + ", " + dZ.ToString());

                    if (dX > 0.006 && dX < 0.70)
                    {
                        MessageBox.Show(dX.ToString());
                        myBeam.EndPoint.X = myBeam.StartPoint.X;
                        myBeam.Modify();
                        //MessageBox.Show("fixed X");
                    }

                    if (dY > 0.006 && dY < 0.70)
                    {
                        MessageBox.Show(dY.ToString());
                        myBeam.EndPoint.Y = myBeam.StartPoint.Y;
                        myBeam.Modify();
                        //MessageBox.Show("fixed Y");
                    }

                    if (dZ > 0.006 && dZ < 0.70)
                    {
                        MessageBox.Show(dZ.ToString());
                        myBeam.EndPoint.Z = myBeam.StartPoint.Z;
                        myBeam.Modify();
                        //MessageBox.Show("fixed Z");
                    }
                }
            }

            //Update model with changes
            new Model().CommitChanges();
        }
    }
}
