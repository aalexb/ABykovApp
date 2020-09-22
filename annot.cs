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


            List<FamilyInstance> allAnnot = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_GenericAnnotation)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
            List<FamilyInstance> myAnnot = new List<FamilyInstance>();
            foreach (FamilyInstance item in allAnnot)
            {
                if (item.Symbol.FamilyName=="Значки1")
                {
                    myAnnot.Add(item);
                }
            }
            
            using (Transaction tr=new Transaction(doc,"annot"))
            {
                tr.Start();
                foreach (FamilyInstance i in myAnnot)
                {
                    i.LookupParameter("view").Set(doc.GetElement(i.OwnerViewId).Name);
                }
                tr.Commit();

            }
            return Result.Succeeded;
        }
    }
}
