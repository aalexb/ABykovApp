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

            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter door_filter = new ElementParameterFilter(fRule);

            IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();


            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
                foreach (Element w in allWalls)
                {
                    w.LookupParameter("стадияСтены").Set("2021");
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