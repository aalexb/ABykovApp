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

            ICollection<ElementId> selectedElementIds = uidoc.Selection.GetElementIds();
            List<Element> selElem = selectedElementIds.Select(x => doc.GetElement(x)).ToList();
            Element foundtype = doc.GetElement(selElem[0].GetTypeId());
            //List<string> parlist = new List<string>();
            //foreach (Parameter item in foundtype.Parameters)
            //{
            //    parlist.Add(item.Definition.Name);
            //}
            //int parvalue = foundtype.LookupParameter("шт/м/м2/м3").AsInteger();
            FilterableValueProvider valueProvider = new ParameterValueProvider(new ElementId(BuiltInParameter.PHASE_CREATED));
            
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            ElementId ruleValue = idPhase;
            ElementParameterFilter stageFilter = new ElementParameterFilter(new FilterElementIdRule(valueProvider, evaluator, ruleValue));

            List<List<Element>> collectFromModel = new List<List<Element>>();

            collectFromModel.Add(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .WherePasses(stageFilter)
                .Where(x=>((FamilyInstance)x).StructuralType.ToString()!="NonStructural")
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
            collectFromModel.Add( new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel)
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
            List<string> ororo = fundament.Select(x => x.Name).ToList();

            //genModel = genModel.Where(x => x.Name != "cube").ToList();
            List<Cube> allCube = new List<Cube>();
            //List<Cube> someCube = new List<Cube>();

            foreach (Element f in floors)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
                    Cube abc=new Cube(doc.GetElement(m), f);
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
                if (doc.GetElement(f.GetTypeId()).LookupParameter("шт/м/м2/м3") !=null && doc.GetElement(f.GetTypeId()).LookupParameter("шт/м/м2/м3").AsInteger()==0)
                {
                    Cube abc = new Cube(f);
                    if (abc.out_Name != null)
                    {
                        allCube.Add(abc);
                    }
                }
                else
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
            foreach (List<Element> a in collectFromModel)
            {
                foreach (Element e in a)
                {
                    Cube abc = new Cube(e);
                    if (abc.out_Name == null)
                    {
                        abc.out_Name = "Исключение: " + abc.typeName;
                        abc.DontSetPos = true;
                    }
                    allCube.Add(abc);
                }
            }
            List<List<Cube>> groupingCube = new List<List<Cube>>();

            //List<string> groupNum=allCube.Select(x => x.Group).Distinct().ToList();

            foreach (string eqGroup in allCube.Select(x => x.out_Group).Distinct())
            {
                if (eqGroup==""|eqGroup==null)
                {
                    continue;
                }
                List<Cube> similarGroup = new List<Cube>();
                foreach (Cube c in allCube)
                {
                    if (c.out_Group==eqGroup)
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
                foreach (myTypes mt in item.Select(x => x.mType).Distinct())
                {
                    foreach (string eqName in item.Select(x => x.out_Name).Distinct())
                    {
                        List<Cube> b = item
                            .Where(x => x.mType == mt)
                            .Where(y => y.out_Name == eqName)
                            .ToList();
                        if (b.Count==0)
                        {
                            continue;
                        }
                        (Cube addCube, int addpos) = Meta.forgeCube(b, a);
                        if (!addCube.DontSetPos)
                        {
                            a += addpos;
                        }

                        
                        outCube.Add(addCube);
                    }
                }
                
            }

            List<List<Cube>> secondOutCube = new List<List<Cube>>();
            foreach (string eqGr in outCube.Select(x=>x.out_Name).Distinct())
            {
                foreach (Cube cube in outCube)
                {

                }
            }
            //foreach (string eqGroup in outCube.Select(x => x.out_Group).Distinct())
            //{
            //    List<Cube> similarGroup = new List<Cube>();
            //    foreach (Cube c in allCube)
            //    {
            //        if (c.out_Group == eqGroup)
            //        {
            //            similarGroup.Add(c);
            //        }
            //    }
            //    int i = 1;
            //    similarGroup.OrderBy(x => x.Prior);
            //    foreach (var item in similarGroup)
            //    {
            //        item.out_Pos = i.ToString();
            //        i++;
            //        secondOutCube.Add(item);
            //    }

            //    //similarGroup.short
            //    //secondOutCube.Add(similarGroup.Select());
            //}
            //outCube = secondOutCube;



            ElementParameterFilter cubeFilter = new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME)), new FilterStringEquals(), "cube", false));
            List<FamilyInstance> existCubes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().WherePasses(cubeFilter).Cast<FamilyInstance>().ToList();


            using (Transaction tr = new Transaction(doc,"ModelledSpec"))
            {
                tr.Start();
                foreach (FamilyInstance i in existCubes)
                {
                    doc.Delete(i.Id);
                }
                foreach (Cube i in outCube)
                {
                    if (i.posElements.Count!=0)
                    {
                        foreach (ElementExt e in i.posElements)
                        {
                            try
                            {
                                e.refElement.LookupParameter("СП_Позиция").Set(i.out_Pos);
                                e.refElement.LookupParameter("АН__Верх").Set(i.textUP);
                                e.refElement.LookupParameter("АН__Низ").Set(e.textDOWN);

                            }
                            catch
                            {

                            }
                            
                        }
                    }
                    
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
