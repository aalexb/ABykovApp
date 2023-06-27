using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System.Linq;
using WorkApp.Отделка;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class NovaFinishing : IExternalCommand
    {
        
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;
            
            var selected_element_id = uidoc.Selection.GetElementIds();
            var selected_element = selected_element_id.Select(x => doc.GetElement(x));
            ViewSchedule selected_view_schedule = null;
            try
            {
                selected_view_schedule = doc.GetElement((selected_element.ElementAt(0) as ScheduleSheetInstance).ScheduleId) as ViewSchedule;
            }
            catch (System.Exception)
            {
            }
            var name = "";
            try
            {
                name = selected_element.First().Name;
            }
            catch (System.Exception)
            {

            }
            var C = new Context();
            var RC = new RevitContext(doc);

            FinishForm MainForm = new FinishForm(RC,C);
            MainForm.selElem(name);
            MainForm.Show();


            FinishAbs root;
            if (MainForm.check.finish_mode)
                root = new FinishCalc(MainForm,doc);
            else
                root = new FloorCalc(MainForm, doc);

            var globe = new GlobSerde(doc, "Findata");

            using (Transaction tr = new Transaction(doc, "setGP"))
            {
                tr.Start();
                globe.write(string.Join("|", MainForm.wTypeBoxes));
                tr.Commit();
            }

            root.GetRooms();
            root.Get();
            root.Translate();
            root.Make();

            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
                root.Commit(selected_view_schedule);
                tr.Commit();
            }
            var msg = new Autodesk.Revit.UI.TaskDialog("Info");
            msg.MainInstruction = $"Выполнен расчет отделки для стадии \"{MainForm.retPhase.Name}\"";
            msg.Show();
            return Result.Succeeded;
        }
    }
}
