using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Universe : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //Application app = uiapp.Application;
            Document doc = uidoc.Document;

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;


            FilterableValueProvider valueProvider = new ParameterValueProvider(new ElementId(BuiltInParameter.PHASE_CREATED));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            ElementId ruleValue = idPhase;
            ElementParameterFilter stageFilter = new ElementParameterFilter(new FilterElementIdRule(valueProvider, evaluator, ruleValue));

          
            List<FamilyInstance> karkas = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Where(x=>((FamilyInstance)x).StructuralType.ToString()!="NonStructural")
                .Cast<FamilyInstance>()
                .ToList();

            List<FamilyInstance> kolon = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<FamilyInstance>()
                .ToList();

            List<Cube> allCube = new List<Cube>();
            foreach (FamilyInstance i in karkas)
            {
                allCube.Add(new Cube(i));
            }
            foreach (FamilyInstance i in kolon)
            {
                allCube.Add(new Cube(i));
            }
            using (Transaction tr = new Transaction(doc,"yuhuu"))
            {
                tr.Start();
                foreach (Cube i in allCube)
                {
                    i.Create(doc);
                }

                tr.Commit();
            }
            
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction = allCube.Count.ToString();
            msg.Show();
            return Result.Succeeded;
        }
    }
}
