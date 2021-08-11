using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
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
    class test : IExternalCommand
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

            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter door_filter = new ElementParameterFilter(fRule);

            IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();

            List<Element> otdWalls=allWalls.Where(x => x.Name.StartsWith("!!отделка")).ToList();
            List<Room> roomofWall = new List<Room>();
            using (Transaction tr = new Transaction(doc, "creating"))
            {
                tr.Start();
                foreach (Element i in otdWalls)
                {

                    BoundingBoxXYZ bBox = i.get_BoundingBox(null);

                    if (bBox!=null)
                    {
                        XYZ origin = new XYZ((bBox.Max.X + bBox.Min.X) / 2, (bBox.Max.Y + bBox.Min.Y) / 2, (bBox.Max.Z + bBox.Min.Z) / 2);
                        try
                        {
                            i.setP("Помещение", doc.GetRoomAtPoint(origin).Number);
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

    public class FinishType
    {
        string Info { get; set; }
        string Name { get; set; }
        public FinishClasses finType { get; set; }
        public FinishType(FamilySymbol sym)
        {
            
        }
    }
}