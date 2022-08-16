using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
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
    class annot : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            /*-
            var sel = uidoc.Selection.GetElementIds().First();
            var selel =doc.GetElement( sel) as Wall;
            var box = selel.get_BoundingBox(doc.ActiveView);
            */

            var allAnnot = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_GenericAnnotation)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>();
            
            var allUzl = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .ToElements();
            var allSpec = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .ToElements();
            
            using (Transaction tr=new Transaction(doc,"annot"))
            {
                tr.Start();
                
                foreach (var i in allSpec)
                {
                    try
                    {
                        i.LookupParameter("СП_Лист").Set(doc.GetElement(i.OwnerViewId).Name);
                    }
                    catch (Exception) { }
                }
                foreach (FamilyInstance i in allAnnot)
                {
                    try
                    {
                        i.LookupParameter("СП_Лист").Set(doc.GetElement(i.OwnerViewId).Name);
                    }
                    catch (Exception) { }
                    
                }
                foreach (FamilyInstance i in allUzl)
                {
                    try
                    {
                        i.LookupParameter("СП_Лист").Set(doc.GetElement(i.OwnerViewId).Name);
                    }
                    catch (Exception) { }
                }

                
                
                tr.Commit();

            }
            return Result.Succeeded;
        }

        
    }
}
