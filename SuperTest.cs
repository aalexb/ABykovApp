using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SuperTest: IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //UIApplication.DoDragDrop()
            //Cube one = new Cube("A", "B");


            //string[] amm=Directory.GetFiles(newPath+"\\");
            Cube one = new Cube("A", "B");

            var data = new List<Cube>();
            data.Add(one);
            using (Window SheetControl = new Window(data))
            {
                var result = SheetControl.ShowDialog();
                if (result==DialogResult.OK)
                {
                    string val = SheetControl.ReturnValue;
                    TaskDialog msg = new TaskDialog("Info");
                    msg.MainInstruction = one.Name; 
                    msg.Show();
                }
            }

            return Result.Succeeded;
        }

    }
}
