using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Tekla.Structures.Plugins;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace RebarSample3
{
    public class StructuresData 
    {
        [Tekla.Structures.Plugins.StructuresField("bpl4")]
        public double textboxValue;
    }

    [Plugin("myPluginTestingThing")] 
    [PluginUserInterface(myPluginTestingThing.UserInterfaceDefinitions.dialog)]
    public class myPluginTestingThing : PluginBase 
    {
        private StructuresData data;

        public myPluginTestingThing(StructuresData data) 
        {
            this.data = data;
        }

        public override List<InputDefinition> DefineInput()   // this is called by TS when the command is started
        {
            List<InputDefinition> inputList = new List<InputDefinition>();

            Picker myPicker = new Picker();
            ModelObject myPart1 = myPicker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            ModelObject myPart2 = myPicker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            InputDefinition input1 = new InputDefinition(myPart1.Identifier);
            InputDefinition input2 = new InputDefinition(myPart2.Identifier);
            inputList.Add(input1);
            inputList.Add(input2);

            return inputList;
        }

        public override bool Run(List<InputDefinition> Input)  // this is called by TS when the component is created or modified
        {
            try
            {

            }
            catch { }

            return true;
        }

        public class UserInterfaceDefinitions   
            // this is a nested class conatining the UI definition for plug-in component
            // the format for UI definitionm is same as for custom components
        {
            public const string dialog =
                @"page(""TeklaStructures"","""")
                {
                    plugin(1, ""myPluginTestingThing"")
                    {
                        tab_page("""", ""Parameters 1"", 1)
                        {
                            parameter(""Number of bottom bars"", ""bottom_number"", integer, number, 1)
                            parameter(""Number of top bars"", ""top_number"", integer, number, 4)
                        }
                        depend(2)
                        modify(1)
                        draw(1, 100.0, 100.0, 0.0, 0.0)
                   }
                }";
        }
    }
}

