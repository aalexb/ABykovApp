using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkApp.Addons;

namespace WorkApp
{
    public class RoomFinishing
    {
        public static List<RoomFinishing> Rooms=new List<RoomFinishing>();
        public static List<List<RoomFinishing>> FinishTable = new List<List<RoomFinishing>>();
        public static List<List<RoomFinishing>> FloorTable = new List<List<RoomFinishing>>();
        public static IEnumerable<IGrouping<ElementId, RoomFinishing>> FloorTableGroup = null;


        public Element refElement { get; }
        public ElementId Id { get; }
        public string Name { get; }
        public string Num { get; set; }
        public ElementId Level { get; set; }
        public string FinishGroup = "";
        public string FloorGroup = "";
        string FloorTypeNumber { get; set; }
        //=============
        public FinishStructuralElement MainWall { get; set; }= new FinishStructuralElement();
        public List<FinishStructuralElement> LocalWallList { get; set; } = new List<FinishStructuralElement>();
        //public FinishStructuralElement LocalWall { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement NewWall { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement Kolon { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement Floor { get; set; } = new FinishStructuralElement();
        public List<FinishStructuralElement> FloorList { get; set; } = new List<FinishStructuralElement>();
        public FinishStructuralElement Ceil{ get; set; } = new FinishStructuralElement();
        public FinishStructuralElement Plintus { get; set; } = new FinishStructuralElement();

        public RoomFinishing(Element e)
        {
            refElement = e;
            Id = e.Id;
            Name=e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            Num = e.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();

            Level = e.LevelId;


            Ceil.setType( e.LookupParameter("ОТД_Потолок").AsValueString(),e);
            MainWall.Type = e.LookupParameter("ОТД_Стены").AsValueString();
            Floor.Type = e.LookupParameter("ОТД_Пол").AsValueString();
            FinishGroup= e.LookupParameter("ADSK_Группирование").AsString();
            FloorGroup = e.LookupParameter("AG_Групп_Пол").AsString();

            try
            {
                Floor.Text = e.Document.GetElement(e.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString();
            }
            catch (Exception)
            {
            }
            
            Floor.unitValue = e.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
            Kolon.Type = e.LookupParameter("ОТД_Колонны").AsValueString();
            Plintus.Type = e.LookupParameter("ОТД_Плинтус").AsValueString();
            Plintus.unitValue = e.get_Parameter(BuiltInParameter.ROOM_PERIMETER).AsDouble();

            Rooms.Add(this);
        }
        public static void SetWallToRoom(List<GhostWall> walls, FinishForm form)
        {
            foreach (var w in walls)
            {
                foreach (var r in RoomFinishing.Rooms)
                {

                    if (r.Id.IntegerValue == w.RoomID)
                    {
                        if (w.typeName != form.WallType.Name& w.typeName != form.ColType.Name)
                        {
                            r.LocalWallList.Add(new FinishStructuralElement() { Text = w.sostav, unitValue = w.Area });
                        }
                        else if (w.typeName == form.ColType.Name)
                        {
                            r.Kolon.unitValue += w.Area;
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
        }
        public static void SetFloorToRoom(List<GhostFloor> floors, FinishForm form)
        {
            foreach (var w in floors)
            {
                foreach (var r in RoomFinishing.Rooms)
                {
                    if (r.Id.IntegerValue == w.RoomID)
                    {
                        r.FloorList.Add(new FinishStructuralElement() { Text = w.sostav, unitValue = w.Area });
                    }
                }
            }
        }


        public static void makeFinish(FinishForm form)
        {

            var grfn = Rooms.GroupBy(key => (
            form.groupCheck? key.FinishGroup : null,
            form.splitLevel?key.Level:null,
            form.ColFromMat?key.Kolon.Text:key.Kolon.Type,
            key.Ceil.Type,
            key.MainWall.Type
            ));

            foreach (var f in grfn)
            {
                FinishTable.Add(f.Select(x => x).ToList());
                List<FinishStructuralElement> fseLocal = new List<FinishStructuralElement>();
                foreach (var r in f)
                {
                    foreach (var item in r.LocalWallList)
                    {
                        fseLocal.Add(item);
                    }
                }
                var SimilarElementList = GroupModelledTypes(fseLocal);
                foreach (RoomFinishing r in f)
                {
                    r.MainWall.Value = f.Sum(x => x.MainWall.unitValue);
                    r.LocalWallList = SimilarElementList;
                    //r.LocalWall.Value = f.Sum(x => x.LocalWall.unitValue);
                    r.Kolon.Value = f.Sum(x => x.Kolon.unitValue);
                    if (form.countNewW)
                    {
                        r.NewWall.Value = f.Sum(x => x.NewWall.unitValue);
                    }
                }
            }
        }

        private static List<FinishStructuralElement> GroupModelledTypes(List<FinishStructuralElement> eInRooms)
        {
            return eInRooms.GroupBy(x => x.Text)
                .Select(g => new FinishStructuralElement
                {
                    Value = g.Sum(x => x.unitValue),
                    Text = g.First().Text
                }).ToList();
        }

        public static void makeFloor(FinishForm form)
        {
            var grfl = Rooms.GroupBy(
                key =>(
                form.groupFloorCheck?key.FloorGroup:null,
                form.splitLevel? key.Level:null, 
                key.Floor.Type, 
                true?null:key.Plintus.Type));
            foreach (var f in grfl)
            {
                FloorTable.Add(f.Select(x => x).ToList());
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
        public static void FinishTableCommit(Document doc, FinishForm form) 
        {
            int i = 1;
            foreach (List<RoomFinishing> item in FinishTable)
            {
                if (item == null)
                {
                    continue;
                }
                String fillText = "";

                if (form.levels==1)
                {
                    foreach (ElementId lev in item.Select(x => x.Level).Distinct())
                    {
                        fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                        if (form.withnames == 1)
                        {
                            foreach (RoomFinishing gg in item.Where(x => x.Level == lev))
                            {
                                fillText += gg.Name + "-" + gg.Num + ", ";
                            }
                            fillText = fillText.Remove(fillText.Length - 2, 2) + "\n";
                            continue;
                        }
                        fillText += Meta.shortLists(item.Where(x => x.Level == lev).Select(y => y.Num).ToList()) + "\n";
                    }
                }
                else
                {
                    fillText += Meta.shortLists(item.Select(y => y.Num).ToList()) + "\n";
                }

                foreach (RoomFinishing r in item)
                {
                    try
                    {
                        r.refElement.LookupParameter("Н_Отделка").Set(i);
                    }
                    catch (Exception)
                    {
                    }



                    if (form.countNewW)
                    {

                        if (r.NewWall.Value > 0)
                        {
                            r.refElement.setP("countNewW", $"В т.ч. по вновь устраиваемым перегородкам - {r.NewWall.Value * Meta.FT * Meta.FT:F1} м²");
                        }
                        else
                        {
                            r.refElement.setP("countNewW", "");
                        }
                    }
                    r.refElement.LookupParameter("ОТД_Состав.Потолок").Set(r.Ceil.Type == "__Отделка : ---" ? "" : doc.GetElement(r.refElement.LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString());

                    try
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Стены").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                    }
                    catch (Exception)
                    {

                        r.refElement.LookupParameter("ОТД_Состав.Стены").Set("");
                    }

                    if (r.refElement.LookupParameter("ОТД_Кол.Колонны") != null)
                    {
                        if (form.groupCheck)
                        {
                            r.refElement.LookupParameter("ОТД_Кол.КолонныGROUP").Set(r.Kolon.Value > 0 ? (r.Kolon.Value * Meta.FT * Meta.FT).ToString("F1") : "");
                        }
                        else
                        {
                            r.refElement.LookupParameter("ОТД_Кол.Колонны").Set(r.Kolon.Value > 0 ? (r.Kolon.Value * Meta.FT * Meta.FT).ToString("F1") : "");
                        }

                        //r.refElement.LookupParameter("УДАЛИТЬ").Set(r.KolonWallVal > 0 ? r.KolonWallVal : 0);
                        if (form.ColFromMat)
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Колонны").Set(r.Kolon.Text);
                        }
                        else
                        {
                            try
                            {
                                r.refElement.LookupParameter("ОТД_Состав.Колонны").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Колонны").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                            }
                            catch (Exception)
                            {

                                r.refElement.LookupParameter("ОТД_Состав.Колонны").Set("");
                            }

                        }

                    }
                    if (r.refElement.LookupParameter("ОТД_Кол.Доп") == null)
                    {
                        Log.msg("Отсутствует параметр ОТД_Кол.Доп");
                        return;
                    }
                    var localMultiText = form.groupCheck ? "ОТД_Кол.ДопGROUP" : "ОТД_Кол.Доп";
                    if (r.refElement.LookupParameter(localMultiText) != null) //Проверяем что существует
                    {
                        
                        r.refElement.LookupParameter(localMultiText).Set(r.LocalWallList.Count > 0 ? FinishStructuralElement.getMultiString(r.LocalWallList,"withNum") : "");
                        

                        //r.refElement.LookupParameter("ОТД_Состав.Доп").Set(r.LocalWall.Text);
                    }

                    if (form.groupCheck)
                    {
                        r.refElement.LookupParameter("WMulAdd").Set(fillText);
                    }
                    else
                    {
                        r.refElement.LookupParameter("testW").Set(fillText);
                    }

                    //r.refElement.LookupParameter("ОТД_Кол.Стены").Set(0);
                    if (form.groupCheck)
                    {
                        r.refElement.LookupParameter("ОТД_Кол.СтеныGROUP").Set(r.MainWall.Value);
                    }
                    else
                    {
                        r.refElement.LookupParameter("ОТД_Кол.Стены").Set(r.MainWall.Value);
                    }

                    try
                    {
                        r.refElement.LookupParameter("ОТД_Пом.Стены").Set(r.MainWall.Value);
                    }
                    catch (Exception)
                    {

                    }


                    //r.refElement.LookupParameter("PlintusTotal").Set(r.Perimeter);
                    //item.Select(x => x.refElement.LookupParameter("testF").Set(fillText));
                    //item.Select(x => x.refElement.LookupParameter("PlintusTotal").Set(x.SimilarPlintusVal));

                }
                i++;
                
            }
        }


        public static void FinishSheduleCommit(FinishForm form, ViewSchedule vs)
        {
            var shed = new NovaShedule(vs, 3);
            var data = new List<List<SheduleCell>>();
            List<int[]> groups = new List<int[]>();
            var groupedRooms = Rooms
                .GroupBy(p => p.FinishGroup)
                .OrderBy(p => p.Key)
                .Select(g => new
                {
                    Name = g.Key,
                    Rooms = g
                    .GroupBy(key => (
                form.splitLevel ? key.Level : null,
                form.ColFromMat ? key.Kolon.Text : key.Kolon.Type,
                key.Ceil.Type,
                key.MainWall.Type))
                    .Select(key => new
                    {
                        Name = key.Key,
                        r = key.Select(p => p)
                    })
                });
            List<int> mergedCol = new List<int>();
            int colCounter = 0;
            foreach (var group in groupedRooms)
            {
                data.Add(SheduleCell.Subtitle(group.Name));
                mergedCol.Add(colCounter);
                colCounter++;
                foreach (var item in group.Rooms)
                {
                    string fillText = Meta.shortLists(item.r.Select(y => y.Num).ToList()) + "\n";
                    colCounter++;
                    data.Add(SheduleCell.FinishRow(
                            fillText,
                            item.r.ElementAt(0).Ceil.Text,
                            item.r.ElementAt(0).Ceil.Value,
                            item.r.ElementAt(0).MainWall.Text,
                            item.r.ElementAt(0).MainWall.Value,

                            floorNum.ToString(),
                            item.r.ElementAt(0).Floor.Text,
                            item.r.ElementAt(0).Floor.Value));
                    floorNum++;

                }

            }




            shed.CreateRow(data);
            foreach (var item in mergedCol)
            {
                shed.mergeCol(item);
            }
            shed.mergeRow(groups, 0);

            shed.setHeight();

            //shed.mergeRow(groups, 0);
            //shed.mergeCol();
            shed.setHeight();
        }
        public static void FloorSheduleCommit(FinishForm form, ViewSchedule vs)
        {
            var shed = new NovaShedule(vs, 2);
            var data = new List<List<SheduleCell>>();
            List<int[]> groups = new List<int[]>();
            /*
             Группируем помещения по группам отделки
            Затем группируем по типам отделки
             */
            var groupedRooms = Rooms
                .GroupBy(p => p.FloorGroup)
                .OrderBy(p=>p.Key)
                .Select(g => new
                {
                    Name = g.Key,
                    Rooms = g
                    .GroupBy(key => (
                form.groupFloorCheck ? key.FloorGroup : null,
                form.splitLevel ? key.Level : null,
                key.Floor.Type,
                true ? null : key.Plintus.Type))
                    .Select(key => new
                    {
                        Name = key.Key,
                        r = key.Select(p => p)
                    })
                });
            int floorNum = 1;
            List<int> mergedCol=new List<int>();
            int colCounter = 0;
            foreach (var group in groupedRooms)
            {
                data.Add(SheduleCell.Subtitle(group.Name));
                mergedCol.Add(colCounter);
                colCounter++;
                foreach (var item in group.Rooms)
                {
                    string fillText= Meta.shortLists(item.r.Select(y => y.Num).ToList()) + "\n";
                    if (item.r.ElementAt(0).FloorList.Count > 0)
                    {
                        
                        groups.Add(new int[] { data.Count, data.Count + item.r.ElementAt(0).FloorList.Count - 1 });
                        int suffix = 1;
                        foreach (var fl in item.r.ElementAt(0).FloorList)
                        {
                            colCounter++;
                            data.Add(SheduleCell.FloorRow(
                                fillText,
                                floorNum.ToString() + "." + suffix.ToString(),
                                fl.Text,
                                fl.Value));
                            suffix++;
                        }
                    }
                    else
                    {
                        colCounter++;
                        data.Add(SheduleCell.FloorRow(
                                fillText,
                                floorNum.ToString(),
                                item.r.ElementAt(0).Floor.Text,
                                item.r.ElementAt(0).Floor.Value));
                    }
                    floorNum++;
                    
                }
                
            }




            shed.CreateRow(data);
            foreach (var item in mergedCol)
            {
                shed.mergeCol(item);
            }
            shed.mergeRow(groups, 0);
            
            shed.setHeight();

            //shed.mergeRow(groups, 0);
            //shed.mergeCol();
            shed.setHeight();
        }

        public static void FloorTableCommit(int MoreThenOneLevel, int withNames, Document doc, FinishForm form, ViewSchedule vs)
        {
            var shed = new NovaShedule(vs, 2);
            List<int[]> groups=new List<int[]>();
            //int[][] groups = null;
            List<List<SheduleCell>> data = new List<List<SheduleCell>>();
            data.Add((new SheduleCell[] {  }).ToList());
            int floorNum = 1;
            foreach (var item in FloorTable)
            {
                if (item == null)
                {
                    continue;
                }
                String fillText = "";

                
                /*
                 * Если 1 уровень
                 */
                if (MoreThenOneLevel==1)
                {
                    foreach (var lev in item.Select(x => x.Level).Distinct())
                    {
                        fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                        if (withNames == 1)
                        {
                            foreach (RoomFinishing gg in item.Where(x => x.Level == lev))
                            {
                                fillText += gg.Name + "-" + gg.Num + ", ";
                            }
                            fillText = fillText.Remove(fillText.Length - 2, 2) + "\n";
                            continue;
                        }
                        fillText += Meta.shortLists(item.Where(x => x.Level == lev).Select(y => y.Num).ToList()) + "\n";
                    }
                }
                else
                {
                    fillText += Meta.shortLists(item.Select(y => y.Num).ToList()) + "\n";
                }

                if (item[0].FloorList.Count > 0)
                {
                    groups.Add(new int[] { data.Count, data.Count + item[0].FloorList.Count-1 });
                    int suffix = 1;
                    foreach (var fl in item[0].FloorList)
                    {
                        data.Add(SheduleCell.FloorRow(
                            fillText,
                            floorNum.ToString() + "." + suffix.ToString(),
                            fl.Text,
                            fl.Value));
                        suffix++;
                    }
                }
                else
                {
                    data.Add(SheduleCell.FloorRow(
                            fillText,
                            floorNum.ToString(),
                            item[0].Floor.Text,
                            item[0].Floor.Value));
                }
                floorNum++;
                //Транзакция

                foreach (var r in item)
                {
                    r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
                    try
                    {
                        if (r.FloorList.Count > 0)
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Пол").Set(FinishStructuralElement.getMultiString(r.FloorList));
                        }
                        else
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                        }

                    }
                    catch (Exception)
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
                    }
                    
                    string floorParam = form.groupFloorCheck ? "FMulAdd" : "testF";
                    
                    writePlintus(doc, r);


                    r.refElement.LookupParameter(floorParam).Set(fillText);
                    string floorNumParamName = form.groupFloorCheck ? "ОТД_Кол.ПолGROUP" : "ОТД_Кол.Пол";
                    string floorNumParamValue = "-";
                    if (r.FloorList.Count == 1)
                    {
                        floorNumParamValue = (r.FloorList.First().Value * Meta.FT * Meta.FT).ToString("F1");
                    }
                    if (r.FloorList.Count == 0)
                    {
                        floorNumParamValue = (r.Floor.Value * Meta.FT * Meta.FT).ToString("F1");
                    }
                    r.refElement.LookupParameter(floorNumParamName).Set(floorNumParamValue);
                }
            }

            shed.CreateRow(data);
            
            shed.mergeRow(groups, 0);
            //shed.mergeCol();
            shed.setHeight();
            
            
        }


        private static void writePlintus(Document doc, RoomFinishing r)
        {
            try
            {
                r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Плинтус").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
            }
            catch (Exception)
            {
                r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set("");
            }
            r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set(r.Plintus.Type == "__Отделка : ---" ? "" : (r.Plintus.Value * Meta.FT).ToString("F1"));
            r.refElement.LookupParameter("PlintusTotal").Set(r.Plintus.Value);
        }
    }
}
