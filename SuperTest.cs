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
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            addParamForm MainForm = new addParamForm(doc);
            MainForm.ShowDialog();

            FamilyManager fm = doc.FamilyManager;


            string messa;
            using (Transaction tr = new Transaction(doc, "pampam"))
            {
                tr.Start();
                for (int i = 1; i <= MainForm.num; i++)
                {
                    fm.AddParameter(MainForm.name + $"__{i}", MainForm.pGroup, MainForm.pType, MainForm.exempl);
                }

                Book t = new Book();
                messa =t.ReadXML();
                tr.Commit();
            }
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction = messa; //output;// FinishTable.Count().ToString();
            msg.Show();
            

            //List<FinishElements> elt = new List<FinishElements>();
            //FinishingLib FLib = new FinishingLib(elt);
            //FLib.Show();



            //Cube one = new Cube("A", "B");

            //var data = new List<Cube>();
            //data.Add(one);
            //using (Window SheetControl = new Window(data))
            //{
            //    var result = SheetControl.ShowDialog();
            //    if (result==DialogResult.OK)
            //    {
            //        string val = SheetControl.ReturnValue;
            //        TaskDialog msg = new TaskDialog("Info");
            //        msg.MainInstruction = one.Name; 
            //        msg.Show();
            //    }
            //}

            return Result.Succeeded;
        }

    }
    public class FinishElements
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
