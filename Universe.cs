using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
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
            List<Element> armaFree = new FilteredElementCollector(doc).OfClass(typeof(Rebar))
                .WherePasses(stageFilter)
                .ToElements().ToList();
            List<Element> armaZ = new FilteredElementCollector(doc).OfClass(typeof(RebarInSystem))
                .WherePasses(stageFilter)
                .ToElements().ToList();
            List<Element> perila = new FilteredElementCollector(doc).OfClass(typeof(Railing))
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .ToElements()
                .ToList();
            List<Element> jelob = new FilteredElementCollector(doc).OfClass(typeof(HostedSweep))
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .ToElements()
                .ToList();
            List<Element> roof = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<FamilyInstance> genModel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<FamilyInstance>()
                .ToList();
            List<Element> floors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<Element> walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<Element> fundament = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<Element> lestnici = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Stairs)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<Element> plastini = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructConnectionPlates)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            List<Element> ankera = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructConnectionAnchors)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList();
            genModel = genModel.Where(x => x.Name != "cube").ToList();


            List<Cube> allCube = new List<Cube>();
            foreach (FamilyInstance i in karkas)
            {
                allCube.Add(new Cube(i));
            }
            foreach (FamilyInstance i in kolon)
            {
                allCube.Add(new Cube(i));
            }
            foreach (Rebar a in armaFree)
            {
                allCube.Add(new Cube(a));
            }
            foreach (RebarInSystem a in armaZ)
            {
                allCube.Add(new Cube(a));
            }
            foreach (Railing a in perila)
            {
                allCube.Add(new Cube(a));
            }
            foreach (FamilyInstance i in genModel)
            {
                Cube abc = new Cube(i);
                if (abc.Name!=null)
                {
                    allCube.Add(abc);
                }
                
            }
            foreach (Element e in jelob)
            {
                allCube.Add(new Cube(e));
            }
            foreach (Element e in roof)
            {
                allCube.Add(new Cube(e));
            }
            foreach (Element e in plastini)
            {
                allCube.Add(new Cube(e));
            }
            foreach (Element e in ankera)
            {
                allCube.Add(new Cube(e));
            }
            foreach (Element f in floors)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
                    Cube abc=new Cube(doc.GetElement(m), f);
                    if (abc.Name != null)
                    {
                        allCube.Add(abc);
                    }
                }

            }

            foreach (Element w in walls)
            {
                foreach (ElementId m in w.GetMaterialIds(false))
                {
                    Cube abc = new Cube(doc.GetElement(m), w);
                    if (abc.Name != null)
                    {
                        allCube.Add(abc);
                    }
                }

            }

            foreach (Element f in fundament)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
                    Cube abc = new Cube(doc.GetElement(m), f);
                    if (abc.Name != null)
                    {
                        allCube.Add(abc);
                    }
                }

            }
            foreach (Element f in lestnici)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
                    Cube abc = new Cube(doc.GetElement(m), f);
                    if (abc.Name != null)
                    {
                        allCube.Add(abc);
                    }
                }

            }
            List<List<Cube>> groupingCube = new List<List<Cube>>();

            //List<string> groupNum=allCube.Select(x => x.Group).Distinct().ToList();

            foreach (string eqGroup in allCube.Select(x => x.Group).Distinct())
            {
                List<Cube> similarGroup = new List<Cube>();
                foreach (Cube c in allCube)
                {
                    if (c.Group==eqGroup)
                    {
                        similarGroup.Add(c);
                    }
                }
                groupingCube.Add(similarGroup);
            }

            List<Cube> outCube = new List<Cube>();
            foreach (List<Cube> item in groupingCube)
            {
                int a = 0;
                foreach (string eqName in item.Select(x=>x.Name).Distinct())
                {
                    List<Cube> b = new List<Cube>();

                    foreach (Cube i in item)
                    {
                        if (i.Name ==eqName)
                        {
                            b.Add(i);
                        }
                    }
                    outCube.Add(Meta.forgeCube(b,++a));
                }
            }



            ElementParameterFilter cubeFilter = new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME)), new FilterStringEquals(), "cube", false));
            List<FamilyInstance> existCubes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().WherePasses(cubeFilter).Cast<FamilyInstance>().ToList();


            using (Transaction tr = new Transaction(doc,"yuhuu"))
            {
                tr.Start();
                foreach (FamilyInstance i in existCubes)
                {
                    doc.Delete(i.Id);
                }
                foreach (Cube i in outCube)
                {
                    i.Create(doc);
                }

                tr.Commit();
            }
            
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction = outCube.Count.ToString();
            msg.Show();
            return Result.Succeeded;
        }
    }
}
