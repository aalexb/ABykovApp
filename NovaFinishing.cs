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
    class NovaFinishing : IExternalCommand
    {
        
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
        


            RoomFinishing.Rooms = new List<RoomFinishing>();
            RoomFinishing.FinishTable = new List<List<RoomFinishing>>();
            RoomFinishing.FloorTable = new List<List<RoomFinishing>>();
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            var presel = false;

            GlobalParameter GlobePar2 = GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter :null;
            FinishForm MainForm = new FinishForm(doc);
            List<Element> selEl=new List<Element>();
            if (uidoc.Selection!=null& uidoc.Selection.GetElementIds().Count!=0)
            {
                presel = true;
                foreach (var item in uidoc.Selection.GetElementIds())
                {
                    selEl.Add(doc.GetElement(item));
                }
                MainForm.selElem(selEl.Count.ToString());
            }
            
            MainForm.ShowDialog();
            using (Transaction tr=new Transaction(doc,"setGP"))
            {
                tr.Start();
                GlobalParameter GlobePar = GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter :
                GlobalParameter.Create(doc, "FinData", ParameterType.Text);
                GlobePar.SetValue(new StringParameterValue(string.Join("|", MainForm.wTypeBoxes)));
                //int MoreThenOneLevel = ((IntegerParameterValue)GlobePar.GetValue()).Value;
                tr.Commit();
            }

            lastPhase = MainForm.retPhase;
            idPhase = lastPhase.Id;

            var shParamElements = new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>()
                .ToList();
            //Фильтр: Помещения на последней стадии
            var providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            var rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
            var room_filter = new ElementParameterFilter(rRule);
            //FilterableValueProvider provRoomSchool = new ParameterValueProvider(shParam.Id);
            var StrEvaluator = new FilterStringEquals();
            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(room_filter)
                //.WherePasses(roomSc_filter)
                .ToElements();
            if (MainForm.groupCheck)
            {
                rooms = rooms.Where(x => x.LookupParameter("ADSK_Группирование").AsString() == MainForm.groupField).ToList();
            }

            //Фильтр: Стены созданные на последней стадии
            var provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            var fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            var door_filter = new ElementParameterFilter(fRule);

            var allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();
            var allFloors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();

            //Фильтр: экземпляры дверей
            var doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();


            //Create objects
            foreach (var item in rooms)
            {
                new RoomFinishing(item);
            }
            //var _=rooms.Select(x => new RoomFinishing(x));

            
            RoomFinishing.Rooms = RoomFinishing.Rooms.OrderBy(x => x.Num).ToList();

            
            var cWalls = new List<GhostWall>();
            foreach (Element wall in allWalls)
            {
                if (wall.LookupParameter("Помещение_Имя").AsString() != null & wall.LookupParameter("Помещение_Имя").AsString() != "")
                {
                    cWalls.Add(new GhostWall(wall,MainForm.LocType));
                }
            }

            
            foreach (var w in cWalls)
            {
                foreach (var r in RoomFinishing.Rooms)
                {
                    
                    if (r.Id.IntegerValue==w.RoomID)
                    {
                        if (w.typeName == MainForm.LocType.Name)
                        {
                            r.LocalWall.unitValue += w.Area;
                            r.LocalWall.Text = w.sostav;
                        }
                        else if (w.typeName== MainForm.ColType.Name)
                        {
                            r.Kolon.unitValue+= w.Area;
                            r.Kolon.Text = w.sostav;
                        }
                        else
                        {
                            r.MainWall.unitValue += w.Area;
                        }
                        if (w.countNewW)
                        {
                            r.NewWall.unitValue += w.Area;
                        }
                    }
                }
            }


            //Плинтус
            foreach (var d in doors)
            {
                foreach (var r in RoomFinishing.Rooms)
                {
                    try
                    {
                        if (d.get_FromRoom(lastPhase).Id == r.Id | d.get_ToRoom(lastPhase).Id == r.Id)
                        {
                            r.Plintus.unitValue-= d.LookupParameter("сп_Ширина проёма").AsDouble();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            
            RoomFinishing.makeFloor(MainForm);
            RoomFinishing.makeFinish(MainForm);

            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
                RoomFinishing.FinishTableCommit(doc, MainForm);
                RoomFinishing.FloorTableCommit(MainForm.levels, MainForm.withnames, doc,MainForm);
                tr.Commit();
            }
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction =  $"Выполнен расчет отделки для стадии \"{MainForm.retPhase.Name}\"";
            msg.Show();
            return Result.Succeeded;
        }
    }
}
