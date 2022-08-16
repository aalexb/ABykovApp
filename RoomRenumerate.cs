using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class RoomRenumerate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;

            List<Room> allRooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>().ToList();

            

            using (Transaction tr = new Transaction(doc,"RoomRenum"))
            {
                tr.Start();
                allRooms = allRooms.Where(r => r.get_BoundingBox(null) != null).ToList();
                allRooms = allRooms.OrderBy(x =>x.get_BoundingBox(null).Max.Y).ToList();
                allRooms = allRooms.OrderBy(x => x.get_BoundingBox(null).Max.X).ToList();
                //allRooms = allRooms.OrderBy(x => x.get_BoundingBox(null).Min.Y).ToList();
                //allRooms = allRooms.OrderBy(x => x.get_BoundingBox(null).Min.Z).ToList();
                foreach (string lev in allRooms.Select(x=>x.LookupParameter("Уровень(текст)").AsString()).Distinct())
                {
                    int index = 1;
                    foreach (Room r in allRooms.Where(x=>x.LookupParameter("Уровень(текст)").AsString()==lev))
                    {
                        r.Number = (int.Parse(lev) * 100 + index).ToString();
                        index++;
                    }
                }

                tr.Commit();
            }




            return Result.Succeeded;
        }
    }
}
