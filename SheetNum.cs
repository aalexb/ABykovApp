using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SheetNum : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //Reference r;

            //ElementInLinkSelectionFilter<View> filter = new ElementInLinkSelectionFilter<View>(doc);

            //r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.PointOnElement);

            //Element e = doc.GetElement(r);

            ICollection<ElementId> elt = uidoc.Selection.GetElementIds();
            ViewSheet a, b;

            //ViewSet b=new ViewSet();
            /*
            foreach (ElementId i in elt)
            {
                b.Insert(doc.GetElement(i) as ViewSheet);
            }
            DWFXExportOptions options = new DWFXExportOptions();
            options.MergedViews = true;
            String dir = doc.PathName.Substring(0, doc.PathName.Length - doc.Title.Length - 4);
            */
            using (Transaction tr = new Transaction(doc, "SheetNumberChanging"))
            {
                tr.Start();

                StringBuilder info = new StringBuilder();
                ViewSet sheets = new ViewSet();
                foreach (ElementId t in elt)
                {
                    ViewSheet sheet = doc.GetElement(t) as ViewSheet;
                    sheets.Insert(sheet);
                }
                List<ViewSheet> mySheets = elt.Select(x => doc.GetElement(x) as ViewSheet).ToList();
                List<ViewSheet> sortedElements = mySheets.OrderBy(x => x.SheetNumber).ToList();
                a = sortedElements.ElementAt(0);
                b = sortedElements.ElementAt(1);

                string firstSheet = a.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                string secondSheet = b.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                a.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set("-99");
                b.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(firstSheet);
                a.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(secondSheet);
                //doc.Export(dir, "MyExportBitch"+b.Size.ToString(), b, options);

                tr.Commit();

            }





            return Result.Succeeded;
        }
    }
}
