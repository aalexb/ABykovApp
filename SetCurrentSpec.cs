using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SetCurrentSpec : IExternalCommand
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


            FilterableValueProvider valueProvider = new ParameterValueProvider(new ElementId(BuiltInParameter.PHASE_CREATED));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            ElementId ruleValue = idPhase;
            ElementParameterFilter stageFilter = new ElementParameterFilter(new FilterElementIdRule(valueProvider, evaluator, ruleValue));

            List<List<Element>> collectFromModel = new List<List<Element>>();

            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Where(x => ((FamilyInstance)x).StructuralType.ToString() != "NonStructural")
                .Cast<Element>()
                .ToList());
            //collectFromModel.Add(karkas);

            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList());

            collectFromModel.Add(new FilteredElementCollector(doc).OfClass(typeof(Rebar))
                .WherePasses(stageFilter)
                .ToElements().ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfClass(typeof(RebarInSystem))
                .WherePasses(stageFilter)
                .ToElements().ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfClass(typeof(Railing))
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .ToElements()
                .ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfClass(typeof(HostedSweep))
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .ToElements()
                .ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructConnectionPlates)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructConnectionAnchors)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .ToList());
            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Cast<Element>()
                .Where(x => x.Name != "cube")
                .ToList());


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




            //genModel = genModel.Where(x => x.Name != "cube").ToList();
            List<Cube> allCube = new List<Cube>();
            foreach (List<Element> a in collectFromModel)
            {
                foreach (Element e in a)
                {
                    Cube abc = new Cube(e);
                    if (abc.out_Name == null| abc.out_Name =="")
                    {
                        abc.out_Name = "Исключение: " + abc.typeName;
                    }
                    allCube.Add(abc);
                }
            }

            foreach (Element f in floors)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
                    Cube abc = new Cube(doc.GetElement(m), f);
                    if (abc.out_Name != null)
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
                    if (abc.out_Name != null)
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
                    if (abc.out_Name != null)
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
                    if (abc.out_Name != null)
                    {
                        allCube.Add(abc);
                    }
                }

            }

            
            
            List<List<Cube>> groupingCube = new List<List<Cube>>();

            //List<string> groupNum=allCube.Select(x => x.Group).Distinct().ToList();

            foreach (string eqGroup in allCube.Select(x => x.out_Group).Distinct())
            {
                List<Cube> similarGroup = new List<Cube>();
                foreach (Cube c in allCube)
                {
                    if (c.out_Group == eqGroup)
                    {
                        similarGroup.Add(c);
                    }
                }
                groupingCube.Add(similarGroup);
            }

            List<Cube> outCube = new List<Cube>();

            foreach (List<Cube> item in groupingCube)
            {
                int a = 1;
                //int addpos = 1;

                foreach (string eqName in item.Select(x => x.out_Name).Distinct())
                {
                    List<Cube> b = new List<Cube>();

                    foreach (Cube i in item)
                    {
                        if (i.out_Name == eqName)
                        {
                            b.Add(i);
                        }
                    }
                    (Cube addCube, int addpos) = Meta.forgeCube(b, a);
                    if (addCube.out_Name.Contains("Исключение"))
                    {
                        a--;
                    }
                    a += addpos;
                    outCube.Add(addCube);
                }
            }




            Element elt = doc.GetElement(uidoc.Selection.GetElementIds().ToList().ElementAt(0));
            string grouping = elt.getP("Группа");


            List<Cube> FillSpec = new List<Cube>();
            foreach (Cube c in outCube)
            {
                if (c.out_Group==grouping)
                {
                    FillSpec.Add(c);
                }
            }


            
            int rowCount = 0;

            //TaskDialog msg = new TaskDialog("Вывод");
            //msg.MainInstruction = elt.Name;
            //msg.Show();
            

            using (Transaction tr = new Transaction(doc,"SetCurrentSpec"))
            {
                tr.Start();
                if (FillSpec.Count<2)
                {
                    return Result.Failed;
                }
                elt.setP("Лист", doc.GetElement(elt.OwnerViewId).getP("Номер листа"));
                elt.setP("Строк", FillSpec.Count);
                FillSpec = FillSpec.OrderBy(x => x.Prior).ToList();
                foreach (Cube cube in FillSpec)
                {
                    if (cube.out_Name.Contains("Исключение"))
                    {
                        continue;
                    }
                    rowCount++;
                    elt.setP($"Поз__{rowCount}", cube.out_Pos);
                    elt.setP($"Обозначение__{rowCount}", cube.out_Gost);
                    elt.setP($"Наименование__{rowCount}", cube.out_Name);
                    elt.setP($"К__{rowCount}", cube.out_Kol_vo);
                    elt.setP($"М__{rowCount}", cube.out_Mass);
                    elt.setP($"Прим__{rowCount}", cube.out_Other);
                }
                
                tr.Commit();
            }
            

            return Result.Succeeded;
        }
    }
}
