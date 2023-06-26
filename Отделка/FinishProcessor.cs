using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkApp.Addons;

namespace WorkApp.Отделка
{
    public abstract class FinishAbs
    {
        public static List<RoomFinishing> Rooms;
        public static List<List<RoomFinishing>> Table;
        protected static List<Surface> surfaces;
        protected static List<List<SheduleCell>> FinishData;
        protected FinishForm form;
        protected Document doc;
        ParameterValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
        FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();


        public FinishAbs(FinishForm form, Document doc)
        {
            this.form = form;
            this.doc = doc;
            Rooms = new List<RoomFinishing>();
            Table = new List<List<RoomFinishing>>();
        }
        public abstract void Get();
        public abstract void Make();
        public abstract void Translate();
        public abstract void Commit(ViewSchedule vs);


        protected static List<FinishStructuralElement> GroupModelledTypes(List<FinishStructuralElement> eInRooms)
        {
            return eInRooms.GroupBy(x => x.Text)
                .Select(g => new FinishStructuralElement
                {
                    Value = g.Sum(x => x.unitValue),
                    Text = g.First().Text
                }).ToList();
        }
        protected void GetSurfaces(BuiltInCategory cat, string prefix )
        {
            var rule = new FilterElementIdRule(provider, evaluator, form.retPhase.Id);
            var filter = new ElementParameterFilter(rule);

            var source = new FilteredElementCollector(doc).OfCategory(cat)
                .WhereElementIsNotElementType()
                .WherePasses(filter)
                .ToElements()
                .Where(x => x.Name.StartsWith(prefix)).ToList();

            foreach (Element element in source)
                surfaces.Add(new Surface(element));
        }
        public void GetRooms()
        {
            var providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            var rule = new FilterElementIdRule(providerRoom, evaluator, form.retPhase.Id);
            var filter = new ElementParameterFilter(rule);
            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(filter)
                .ToElements();
            foreach (var item in rooms)
                Rooms.Add(new RoomFinishing(item));
            Rooms = RoomFinishing.Rooms.OrderBy(x => x.Num).ToList();
        }
    }

    public class FinishCalc : FinishAbs
    {
        public FinishCalc(FinishForm form, Document doc) : base(form, doc) { }


        public override void Commit(ViewSchedule vs)
        {
            
        }

        public override void Get()
        {
            GetSurfaces(BuiltInCategory.OST_Walls, form.parnames.prefix_finish);
        }

        public override void Make()
        {
            fakeRoomForMainWall();
            var newgro = Rooms
                .GroupBy(key => (
                form.check.grouped ? key.FinishGroup : null,
                key.FinishNote,
                key.LocalWall.Type,
                form.check.diff_levels? key.Level : null,
                form.check.kolon_type ? key.Kolon.Text : key.Kolon.Type,
                key.Ceil.Type,
                key.MainWall.Type
                ))
                .Select(key => new
                {
                    room = key.Select(p => p),
                    sum = key.Sum(p => p.MainWall.unitValue),
                    ceilSum = key.Sum(p => p.Ceil.unitValue),
                    SEL = key.Select(p => p.LocalWallList),
                    kolonSum = key.Sum(p => p.Kolon.unitValue),
                    newwallSum = key.Sum(p => p.NewWall.unitValue),
                    locSum = key.Sum(p => p.LocalWall.unitValue)
                }
                    );

            foreach (var group in newgro)
            {
                foreach (var r in group.room)
                {
                    r.MainWall.Value = group.sum;
                    r.Ceil.Value = group.ceilSum;
                    r.Kolon.Value = group.kolonSum;
                    r.NewWall.Value = form.countNewW ? group.newwallSum : 0;
                    r.LocalWall.Value = group.locSum;
                }
            }
        }

        public static void fakeRoomForMainWall()
        {
            List<RoomFinishing> fakerooms = new List<RoomFinishing>();
            foreach (var r in Rooms)
            {
                r.MainWallList.Add(r.MainWall);
                if (r.LocalWallList.Where(x => x.WallFunc == "Основная").Count() != 0)
                {
                    foreach (var item in r.LocalWallList.Where(x => x.WallFunc == "Основная"))
                    {
                        r.MainWallList.Add(item);
                    }

                    r.LocalWallList = r.LocalWallList.Where(x => x.WallFunc != "Основная").ToList();
                }
                if (r.LocalWallList.Where(x => x.WallFunc == "Верх").Count() != 0)
                {
                    foreach (var item in r.LocalWallList.Where(x => x.WallFunc == "Верх"))
                    {
                        r.UpWallList.Add(item);
                    }

                    r.LocalWallList = r.LocalWallList.Where(x => x.WallFunc != "Верх").ToList();
                    foreach (var item in r.LocalWallList.Where(x => x.WallFunc == "Низ"))
                    {
                        r.DownWallList.Add(item);
                    }

                    r.LocalWallList = r.LocalWallList.Where(x => x.WallFunc != "Низ").ToList();
                }
            }
        }

        public override void Translate()
        {
            
            foreach (var wall in surfaces)
            {
                foreach (var room in Rooms)
                {
                    if (room.id == wall.room_id)
                    {
                        room.unsort.Add(wall);


                        if (wall.typeName != form.WallType.Name & wall.typeName != form.ColType.Name & wall.sostav != null)
                        {
                            room.LocalWallList.Add(new FinishStructuralElement(wall));
                        }
                        else if (wall.typeName == form.ColType.Name)
                        {
                            room.Kolon.unitValue += wall.Area;
                            room.Kolon.Text = wall.sostav;
                        }
                        else
                        {
                            room.MainWall.unitValue += wall.Area;
                        }
                    }
                }
            }



        }
    }


    public class FloorCalc : FinishAbs
    {
        

        public FloorCalc(FinishForm form, Document doc) : base(form, doc)
        {
        }

        public override void Commit(ViewSchedule vs)
        {
            var shed = new NovaShedule(vs, 3);

            foreach (var r in Rooms)
            {
                r.LocalWallList = r.LocalWallList.OrderBy(x => x.Text).ToList();
                r.CalculateControlSum();
            }
            var groups = Rooms
                .GroupBy(p => p.FinishGroup)
                .OrderBy(p => p.Key)
                .Select(groupedRoomslambdda => new
                {
                    Name = groupedRoomslambdda.Key,
                    Rooms = groupedRoomslambdda
                    .GroupBy(x => (
                        x.Ceil.Type,
                        x.ControlSum,
                        x.FinishEndNote))
                    .Select(x => new
                    {
                        r = x.Select(p => p)
                    })
                });


            List<int> mergedCol = new List<int>();
            int colCounter = 0;
            FinishData = new List<List<SheduleCell>>();
            foreach (var group in groups)
            {
                FinishData.Add(SheduleCell.Subtitle(group.Name));
                mergedCol.Add(colCounter);
                colCounter++;
                foreach (var item in group.Rooms)
                {
                    //FormTableLine(item.r, form);
                }

            }
            shed.CreateRow(FinishData);
            shed.setHeight();
        }

        public override void Get()
        {
            GetSurfaces(BuiltInCategory.OST_Floors, form.parnames.prefix_floor);
        }

        public override void Make()
        {
            var grfl = Rooms.GroupBy(
                key => (
                form.groupFloorCheck ? key.FloorGroup : null,
                form.splitLevel ? key.Level : null,
                key.Floor.Type,
                true ? null : key.Plintus.Type));
            foreach (var f in grfl)
            {
                Table.Add(f.Select(x => x).ToList());
                List<FinishStructuralElement> fseLocal = new List<FinishStructuralElement>();
                foreach (RoomFinishing r in f)
                {
                    r.Plintus.Value = f.Sum(x => x.Plintus.unitValue);
                    foreach (var item in r.FloorList)
                    {
                        fseLocal.Add(item);
                    }
                }
                var SimilarElementList = GroupModelledTypes(fseLocal);
                foreach (RoomFinishing r in f)
                {
                    r.Floor.Value = f.Sum(x => x.Floor.unitValue);
                    r.FloorList = SimilarElementList;
                }
            }

        }

        public override void Translate()
        {
            throw new NotImplementedException();
        }
    }



}
