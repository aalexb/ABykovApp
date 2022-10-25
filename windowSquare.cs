using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp
{
    public class windowSquare : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var AllWindow = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType();

            double area = 0;

            foreach (var item in AllWindow)
            {
                area =
                item.LookupParameter("Створка_Левая").AsDouble()
                *
                item.LookupParameter("Высота окна").AsDouble();
                if (area>1.5)
                {
                    item.LookupParameter("параметр").Set("Площадь больше чем надо!");
                }
            }


            return Result.Succeeded;
        }
    }
}
