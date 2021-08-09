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
    class metall : IExternalCommand
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

            List<FamilyInstance> karkas = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
            
            List<FamilyInstance> kolon = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList(); 
            
            //List
            List<FamilyInstance> myKar = new List<FamilyInstance>();
            foreach (FamilyInstance i in karkas)
            {
                if (i.StructuralType.ToString()!="NonStructural")
                {
                    myKar.Add(i);
                }
            }
            
            using (Transaction tr = new Transaction(doc, "metall"))
            {
                tr.Start();

                foreach (FamilyInstance i in myKar)
                {
                    i.LookupParameter("АММО_Длина_КМ").Set(i.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble());



                }
                foreach (FamilyInstance i in kolon)
                {
                    i.LookupParameter("АММО_Длина_КМ").Set(i.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble());
                }
                tr.Commit();
            }
            return Result.Succeeded;
        }
    }
}
