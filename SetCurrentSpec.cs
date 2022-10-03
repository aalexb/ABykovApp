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
            List<ElementId> argh = uidoc.Selection.GetElementIds().ToList();
            List<wtf.Product> TabSel = new List<wtf.Product>();
            foreach (var item in argh)
            {
                TabSel.Add(new wtf.TABSCreator().Create(doc.GetElement(item)));
            }
            foreach (var item in TabSel)
            {
                foreach (Cube c in outCube)
                {
                    if (item.grouping==c.out_Group)
                    {
                        item.linkedElt.Add(c);
                    }
                }
            }
            using (Transaction tr = new Transaction(doc,"SetCurrentSpec"))
            {
                tr.Start();
                foreach (wtf.Product i in TabSel)
                {
                    if (i.linkedElt.Count<2)
                    {
                        continue;
                    }
                }
                int rowCount = 0;
                foreach (wtf.Product e in TabSel)
                {
                    e.refElt.refElement.setP("Лист", doc.GetElement(e.refElt.refElement.OwnerViewId).getP("Номер листа"));
                    e.refElt.refElement.setP("Строк", e.linkedElt.Count==1?2: e.linkedElt.Count);
                    e.linkedElt = e.linkedElt.OrderBy(x => x.Prior).ToList();
                    foreach (Cube cube in e.linkedElt)
                    {
                        if (cube.out_Name.Contains("Исключение"))
                        {
                            continue;
                        }
                        rowCount++;
                        e.refElt.refElement.setP($"Поз__{rowCount}", cube.out_Pos);
                        e.refElt.refElement.setP($"Обозначение__{rowCount}", cube.out_Gost);
                        e.refElt.refElement.setP($"Наименование__{rowCount}", cube.out_Name);
                        e.refElt.refElement.setP($"К__{rowCount}", cube.out_Kol_vo);
                        e.refElt.refElement.setP($"М__{rowCount}", cube.out_Mass);
                        e.refElt.refElement.setP($"Прим__{rowCount}", cube.out_Other);
                    }
                    rowCount = 0;
                }
                tr.Commit();
            }
            return Result.Succeeded;
        }
    }
}
