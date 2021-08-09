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
    class WallLastStage : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;


            IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .ToElements();

            IList<Element> allDoors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .ToElements();


            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
                foreach (Element w in allWalls)
                {
                    if (w.get_Parameter(BuiltInParameter.PHASE_CREATED).AsValueString()=="обозначения")
                    {
                        continue;
                    }
                    LocationCurve loc = w.Location as LocationCurve;
                    
                    w.LookupParameter("СП_Стадия возведения").Set(w.get_Parameter(BuiltInParameter.PHASE_CREATED).AsValueString());
                    //BoundingBoxXYZ bbox=w.get_Geometry(new Options).
                    w.LookupParameter("СП_Стадия сноса").Set(w.get_Parameter(BuiltInParameter.PHASE_DEMOLISHED).AsValueString());
                    w.LookupParameter("СП_Высота").Set(w.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                    
                    
                    //w.LookupParameter("СП_Длина").Set(w.getP(BuiltInParameter.));

                }
                foreach (Element  w in allDoors)
                {
                    w.LookupParameter("СП_Стадия возведения").Set(w.get_Parameter(BuiltInParameter.PHASE_CREATED).AsValueString());
                }
                tr.Commit();
            }


                


                //TaskDialog msg = new TaskDialog("Info");
                //msg.MainInstruction = one[3];
                //msg.Show();



                return Result.Succeeded;
        }
    }
}