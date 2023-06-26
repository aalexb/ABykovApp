using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class SetParamLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            var els = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements();
            var gro = els.GroupBy(el => el.LevelId);

            using (Transaction tr = new Transaction(doc,"Set level"))
            {
                tr.Start();
                

                
                foreach (var item in gro)
                {
                    var levEl = doc.GetElement(item.Key);
                    if (levEl == null) { continue; }
                    double lvl = levEl.LookupParameter("УР_№_Этажа").AsInteger();

                    if (lvl == 0) {
                        break;
                    }
                    foreach (var el in item)
                    {
                        try
                        {
                            el.LookupParameter(":Уровень Размещения").Set(lvl);
                        }
                        catch (Exception)
                        {

                            
                        }
                        
                        

                        
                    }
                }
                tr.Commit();
            }
            
            return Result.Succeeded;
        }
    }
}
