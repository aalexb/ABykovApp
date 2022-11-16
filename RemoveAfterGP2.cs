using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class RemoveAfterGP2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var doorFil = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
            using (Transaction tr= new Transaction(doc, "_RemoveAfterGP2"))
            {
                tr.Start();
                foreach (var item in doorFil.Select(x=>doc.GetElement(x)))
                {
                    var shir = item.LookupParameter("Перемычка_Ширина").AsDouble();
                    var opslev = item.LookupParameter("Опирание_Слева");
                    if (item.LookupParameter("Перемычка_Ширина").AsValueString() == "200" & item.LookupParameter("Опирание_Слева").AsValueString() == "250" & item.LookupParameter("Опирание_Справа").AsValueString() == "250")
                    {
                        item.LookupParameter("Опирание_Справа").Set(0.200/Meta.FT);
                        item.LookupParameter("Опирание_Слева").Set(0.200 / Meta.FT);
                    }
                }
                tr.Commit();
            }
            

            return Result.Succeeded;
        }
    }
}
