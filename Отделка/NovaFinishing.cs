using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkApp.Addons;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class NovaFinishing : IExternalCommand
    {
        
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc;
            Document doc;
            IEnumerable<Element> ix;
            RoomFinishing.Rooms = new List<RoomFinishing>();
            RoomFinishing.FinishTable = new List<List<RoomFinishing>>();
            RoomFinishing.FloorTable = new List<List<RoomFinishing>>();
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            var selx = uidoc.Selection.GetElementIds();
            ix = selx.Select(x => doc.GetElement(x));

            ViewSchedule vs = doc.GetElement((ix.ElementAt(0) as ScheduleSheetInstance).ScheduleId) as ViewSchedule;

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            var presel = false;

            GlobalParameter GlobePar2 = GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter : null;
            FinishForm MainForm = new FinishForm(doc);
            List<Element> selEl = new List<Element>();
            if (uidoc.Selection != null & uidoc.Selection.GetElementIds().Count != 0)
            {
                presel = true;
                foreach (var item in uidoc.Selection.GetElementIds())
                {
                    selEl.Add(doc.GetElement(item));
                }
                MainForm.selElem(selEl.Count.ToString());
            }

            MainForm.ShowDialog();
            using (Transaction tr = new Transaction(doc, "setGP"))
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
            if (MainForm.groupCheck & MainForm.groupFloorCheck)
            {
                TaskDialog msger = new TaskDialog("Info");
                msger.MainInstruction = "Не надо так делать";
                msger.Show();
                return Result.Failed;
            }

            //if (MainForm.groupCheck)
            //{
            //    rooms = rooms.Where(x => x.LookupParameter("ADSK_Группирование").AsString() == MainForm.groupField).ToList();
            //}
            //if (MainForm.groupFloorCheck)
            //{
            //    rooms = rooms.Where(x => x.LookupParameter("AG_Групп_Пол").AsString() == MainForm.groupFloorField).ToList();
            //}

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

            RoomFinishing.Rooms = RoomFinishing.Rooms.OrderBy(x => x.Num).ToList();

            var cWalls = new List<GhostWall>();
            foreach (Element wall in allWalls)
            {
                if (wall.LookupParameter("Помещение_Имя").AsString() != null & wall.LookupParameter("Помещение_Имя").AsString() != "")
                {
                    cWalls.Add(new GhostWall(wall, MainForm.LocType));
                }
            }
            var cFloors = new List<GhostFloor>();
            foreach (Element e in allFloors)
            {
                //var par=e.LookupParameter("").AsString();
                if ((e as Floor).FloorType.Name.Contains("Отделка А_"))
                {
                    cFloors.Add(new GhostFloor(e));
                }
            }

            //Соотнести элементы отделки с помещениями
            RoomFinishing.SetFloorToRoom(cFloors, MainForm);
            RoomFinishing.SetWallToRoom(cWalls, MainForm);


            //Плинтус
            foreach (var d in doors)
            {
                new doorObj(d, lastPhase);

            }

            foreach (var item in doorObj.AllDoorObj)
            {
                foreach (var r in RoomFinishing.Rooms)
                {
                    var doorInRoom = item.fromRoom == r.Id | item.toRoom == r.Id;

                    if (doorInRoom)
                    {
                        r.Plintus.unitValue -= item.width;
                    }

                }
            }

            RoomFinishing.fakeRoomForMainWall();
            //RoomFinishing.fakeRoomForLocalWall();
            if (MainForm.groupCheck)
            {
                RoomFinishing.makeFinish(MainForm);
            }
            if (MainForm.groupFloorCheck)
            {
                RoomFinishing.makeFloor(MainForm);
            }



            using (Transaction tr = new Transaction(doc, "otdelka"))
            {

                tr.Start();
                foreach (var item in rooms)
                {
                    item.LookupParameter("Room_ID").Set(item.Id);
                }
                if (!MainForm.groupFloorCheck)
                {
                    RoomFinishing.FinishMEGACommit(vs,MainForm);
                    //RoomFinishing.FinishSheduleCommit(MainForm, vs);
                    //RoomFinishing.FinishTableCommit(doc, MainForm);
                    //RoomFinishing.FloorSheduleCommit();
                }
                if (!MainForm.groupCheck)
                {
                    //RoomFinishing.FloorTableCommit(MainForm.levels, MainForm.withnames, doc, MainForm,vs);
                    RoomFinishing.FloorSheduleCommit(MainForm, vs);
                }
                tr.Commit();
            }
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction = $"Выполнен расчет отделки для стадии \"{MainForm.retPhase.Name}\"";
            msg.Show();
            return Result.Succeeded;
        }
    }
}
