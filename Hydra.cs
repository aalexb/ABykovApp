using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;


namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class Hydra : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //Application app = uiapp.Application;
            Document doc = uidoc.Document;

            List<PipingSystem> pipeSystems = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipingSystem)
                .WhereElementIsNotElementType()
                .Cast<PipingSystem>()
                .ToList();
            List<List<Autodesk.Revit.DB.Mechanical.MEPSection>> sect = new List<List<Autodesk.Revit.DB.Mechanical.MEPSection>>();
            foreach (PipingSystem item in pipeSystems)
            {
                List<Autodesk.Revit.DB.Mechanical.MEPSection> sc = new List<Autodesk.Revit.DB.Mechanical.MEPSection>();
                for (int i = 0; i < item.SectionsCount; i++)
                {
                    sc.Add(item.GetSectionByIndex(i));
                }
                sect.Add(sc);
               
            }



            return Result.Succeeded;
        }
    }
}
