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
            return Result.Succeeded;
        }
    }
}
