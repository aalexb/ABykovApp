using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Multiverse : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<wtf.Product> allObj = new List<wtf.Product>();

            allObj.AddRange(doc.addFilterResult<wtf.StructuralCreator>(BuiltInCategory.OST_StructuralFraming, "NonStructural"));
            allObj.AddRange(doc.addFilterResult<wtf.StructuralCreator>(BuiltInCategory.OST_StructuralColumns, "NonStructural"));
            allObj.AddRange(doc.addFilterResult<wtf.RoofCreator>(BuiltInCategory.OST_Roofs));
            allObj.AddRange(doc.addFilterResult<wtf.PlatesCreator>(BuiltInCategory.OST_StructConnectionPlates));
            allObj.AddRange(doc.addFilterResult<wtf.AnchorsCreator>(BuiltInCategory.OST_StructConnectionAnchors));
            allObj.AddRange(doc.addFilterResult<wtf.GenericCreator>(BuiltInCategory.OST_GenericModel));
            allObj.AddRange(doc.addFilterResult<RebarInSystem, wtf.RebarCreator>());
            allObj.AddRange(doc.addFilterResult<Rebar, wtf.RebarCreator>());
            allObj.AddRange(doc.addFilterResult<Railing, wtf.RailingCreator>());
            allObj.AddRange(doc.addFilterResult<HostedSweep, wtf.HostedSweepCreator>());
            allObj.AddRange(doc.addFilterResult(BuiltInCategory.OST_Floors));
            allObj.AddRange(doc.addFilterResult(BuiltInCategory.OST_Walls));
            allObj.AddRange(doc.addFilterResult(BuiltInCategory.OST_StructuralFoundation));
            allObj.AddRange(doc.addFilterResult(BuiltInCategory.OST_Stairs));
            ///===========================
            List<wtf.Product> outObj = new List<wtf.Product>();

            var allObjGrouped = allObj.GroupBy<wtf.Product, string>(x => x.grouping);
            foreach (var result in allObjGrouped)
            {
                int a = 1;
                foreach (myTypes mt in result.Select(x=>x.mType).Distinct())
                {
                    foreach (string eqName in result.Select(x => x.Name).Distinct())
                    {
                        List<wtf.Product> b = result
                            .Where(x => x.mType == mt)
                            .Where(x => x.Name == eqName)
                            .ToList();
                        if (b.Count==0)
                        {
                            continue;
                        }
                        (wtf.Product addProd, int addpos) = Meta.forgeProduct(b, a);
                        a += addpos;
                        outObj.Add(addProd);
                    }
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
                foreach (wtf.Product c in outObj)
                {
                    if (item.grouping == c.grouping)
                    {
                        item.linkedEltP.Add(c);
                    }
                }
            }
            using (Transaction tr = new Transaction(doc, "SetCurrentSpec"))
            {
                tr.Start();
                foreach (wtf.Product i in TabSel)
                {
                    if (i.linkedElt.Count < 2)
                    {
                        continue;
                    }
                }
                int rowCount = 0;
                foreach (wtf.Product e in TabSel)
                {
                    e.refElt.refElement.setP("Лист", doc.GetElement(e.refElt.refElement.OwnerViewId).getP("Номер листа"));
                    e.refElt.refElement.setP("Строк", e.linkedElt.Count);
                    e.linkedElt = e.linkedElt.OrderBy(x => x.Prior).ToList();
                    foreach (wtf.Product cube in e.linkedEltP)
                    {
                        if (cube.ovt.Name.Contains("Исключение"))
                        {
                            continue;
                        }
                        rowCount++;
                        e.refElt.refElement.setP($"Поз__{rowCount}", cube.ovt.Pos);
                        e.refElt.refElement.setP($"Обозначение__{rowCount}", cube.ovt.Gost);
                        e.refElt.refElement.setP($"Наименование__{rowCount}", cube.ovt.Name);
                        e.refElt.refElement.setP($"К__{rowCount}", cube.ovt.Kol);
                        e.refElt.refElement.setP($"М__{rowCount}", cube.ovt.Mass);
                        e.refElt.refElement.setP($"Прим__{rowCount}", cube.ovt.Other);
                    }
                    rowCount = 0;
                }
                tr.Commit();
            }

            return Result.Succeeded;
        }

        
    }
}
