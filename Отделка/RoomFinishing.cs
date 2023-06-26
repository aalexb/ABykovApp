using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkApp.Addons;

namespace WorkApp
{
    public class RoomFinishing
    {
        public static List<RoomFinishing> Rooms;
        public static List<List<RoomFinishing>> FinishTable;
        public static List<List<RoomFinishing>> FloorTable;
        public static IEnumerable<IGrouping<ElementId, RoomFinishing>> FloorTableGroup = null;
        public static List<List<SheduleCell>> FinishData;
        public int id;


        public Element refElement { get; }
        public ElementId Id { get; }
        public string Name { get; }
        public string Num { get; set; }
        public string FinishNote = "";
        public string FloorNote = "";
        public string FinishEndNote = "";
        public ElementId Level { get; set; }
        public string FinishGroup = "";
        public string FloorGroup = "";
        public bool FakeRoom = false;
        public string ControlSum = "";


        //=============
        public List<Surface> unsort { get; set; } = new List<Surface>();

        public List<FinishStructuralElement> MainWallList { get; set; } = new List<FinishStructuralElement>();
        public List<FinishStructuralElement> UpWallList { get; set; } = new List<FinishStructuralElement>();
        public List<FinishStructuralElement> DownWallList { get; set; } = new List<FinishStructuralElement>();
        public FinishStructuralElement MainWall { get; set; }= new FinishStructuralElement();
        public List<FinishStructuralElement> LocalWallList { get; set; } = new List<FinishStructuralElement>();
        public FinishStructuralElement LocalWall { get; set; } = new FinishStructuralElement();
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
            id = Id.IntegerValue;
            Name=e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            Num = e.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
            FinishNote = e.LookupParameter("ПримечаниеТамбуры")==null?"": e.LookupParameter("ПримечаниеТамбуры").AsString();
            FloorNote = e.LookupParameter(":Примечание")==null?"": e.LookupParameter(":Примечание").AsString();


            FinishEndNote = e.LookupParameter("ADSK_Примечание").AsString();

            Level = e.LevelId;


            Ceil.setType( e.LookupParameter("ОТД_Потолок")==null? "__Отделка : ---" : e.LookupParameter("ОТД_Потолок").AsValueString(),e);
            Ceil.unitValue = e.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
            MainWall.Type = e.LookupParameter("ОТД_Стены").AsValueString();
            try
            {
                MainWall.Text = e.Document.GetElement(e.LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString();
            }
            catch (Exception)
            {
            }
            Floor.Type = e.LookupParameter("ОТД_Пол").AsValueString();
            FinishGroup= e.LookupParameter("ADSK_Группирование").AsString();
            FloorGroup = e.LookupParameter("AG_Групп_Пол")==null?"":e.LookupParameter("AG_Групп_Пол").AsString();

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
        public static void FormTableLine(IEnumerable<RoomFinishing> r, FinishForm form)
        {
            List<FinishStructuralElement> LocWallForLocWall = new List<FinishStructuralElement>();
            List<FinishStructuralElement> LocWallForMainWall = new List<FinishStructuralElement>();
            List<FinishStructuralElement> LocWallForUp = new List<FinishStructuralElement>();
            List<FinishStructuralElement> LocWallForDown = new List<FinishStructuralElement>();
            
            foreach (var item in r)
            {
                foreach (var loc in item.LocalWallList)
                {
                    LocWallForLocWall.Add(loc);
                }
                foreach (var loc in item.MainWallList)
                {

                    LocWallForMainWall.Add(loc);
                }
                foreach (var loc in item.UpWallList)
                {

                    LocWallForUp.Add(loc);
                }
                foreach (var loc in item.DownWallList)
                {

                    LocWallForDown.Add(loc);
                }
            }
            bool haveLocal = true;
            string fillText = "";
            if (form.withnames== 1)
            {
                foreach (RoomFinishing gg in r.GroupBy(x=>x.Num).Select(g=>g.First()))
                {
                    fillText += gg.Name + "-" + gg.Num + ", ";
                }
                fillText = fillText.Remove(fillText.Length - 2, 2) + "\n";
            }
            else
            {
                fillText = Meta.shortLists(r.Select(y => y.Num).ToList()) + "\n";
            }
            
            var CountOfLocWallTypes=LocWallForLocWall.Select(x=>x.Text).Distinct().Count();
            var CountOfMainWallTypes=LocWallForMainWall.Select(x => x.Text).Distinct().Count();
            if (CountOfLocWallTypes==0)
            {
                haveLocal = false;
                CountOfLocWallTypes = 1;
            }
            CountOfMainWallTypes = CountOfMainWallTypes == 0 ? 1 : CountOfMainWallTypes;
            int dddddd = 0;
            int MWNum = 0;
            
            
            if (LocWallForUp.Count != 0)
            {
                var upgr = LocWallForUp.Select(p => p.Text).Distinct().ElementAt(0);
                var BotWallText =LocWallForDown.Count()!=0? LocWallForDown.Select(p => p.Text).Distinct().ElementAt(0):null;
                FinishData.Add(SheduleCell.FinishRow(
                                String.Concat(fillText, " ", String.Concat(r.Select(x => x.FinishNote).Distinct())),
                                r.ElementAt(0).Ceil.Text,
                                r.Sum(x => x.Ceil.unitValue),
                                upgr,
                                LocWallForUp.Sum(x => x.unitValue),
                                BotWallText: BotWallText,
                                BotWallValue:LocWallForDown.Sum(x => x.unitValue)
                                ));
            }
            for (int i = 0; i < CountOfLocWallTypes; i++)
            {
                
                for (int j = 0; j < CountOfMainWallTypes; j++)
                {
                    if (dddddd - CountOfLocWallTypes == 0)
                    {
                        dddddd = 0;
                        MWNum++;
                    }
                    dddddd++;
                    var BotWallText = "";
                    if (haveLocal)
                    {
                        BotWallText = LocWallForLocWall.Select(p => p.Text).Distinct().ElementAt(i);
                    }

                    var MainWallText = LocWallForMainWall.Select(p => p.Text).Distinct().ElementAt(MWNum);
                    /*
                                     .Where(g => g.WallFunc != "Основная"))
                    .Select(p=>p)
                                    .Distinct().ElementAt(i);
                    */
                    FinishData.Add(SheduleCell.FinishRow(
                                String.Concat(fillText, " ", String.Concat(r.Select(x => x.FinishNote).Distinct())),
                                r.ElementAt(0).Ceil.Text,
                                r.Sum(x => x.Ceil.unitValue),
                                MainWallText,
                                LocWallForMainWall.Where(x=>x.Text==MainWallText).Sum(x=>x.unitValue),
                                BotWallText: BotWallText,
                                BotWallValue:LocWallForLocWall.Where(x => x.Text == BotWallText).Sum(x=>x.unitValue),
                                Note: r.ElementAt(0).FinishEndNote
                                ));
                }
            }
            

        }
        

        public void CalculateControlSum()
        {
            this.ControlSum= String.Concat(MainWallList.Select(x => x.Text.ToString()).Distinct()) 
                + "__"+String.Concat(LocalWallList.Select(x=>x.Text.ToString()).Distinct())
                + "__" + String.Concat(UpWallList.Select(x => x.Text.ToString()).Distinct())
                + "__" + String.Concat(DownWallList.Select(x => x.Text.ToString()).Distinct());
        }

        public static void FinishSheduleCommit(FinishForm form, ViewSchedule vs)
        {
            var shed = new NovaShedule(vs, 3);
            var data = new List<List<SheduleCell>>();
            List<int[]> groups = new List<int[]>();


            var groupedRooms = Rooms
                //1111111111111111111111
                .GroupBy(p => p.FinishGroup)
                .OrderBy(p => p.Key)
                .Select(groupedRoomslambda => new
                //111_Objects
                {
                    Name = groupedRoomslambda.Key,
                    Rooms = groupedRoomslambda
                    //22222222222222222222222
                    .GroupBy(finishLambda => (
                        form.splitLevel ? finishLambda.Level : null,
                        form.ColFromMat ? finishLambda.Kolon.Text : finishLambda.Kolon.Type,
                        finishLambda.Ceil.Type,
                        finishLambda.FinishEndNote))
                    .Select(xxx => new
                    //222_Objects
                    {
                        EndNote = xxx.Key.FinishEndNote,
                        r = xxx.Select(p => p),
                        SEL=xxx.Select(p=>p.LocalWallList),
                        lvtz=xxx
                        //333333333333333333333333
                        .GroupBy(MainWallLambda=>MainWallLambda.MainWall.Type)
                        .Select(zzz => new
                        //333_Objects
                        {
                            lvt=zzz
                            //44444444444444444444
                            .GroupBy(localLambda => (
                            localLambda.LocalWall.Type))
                            .Select(x => new
                            //444_Objects
                            {
                                lvte = x.Select(p => p.LocalWall)
                            })
                        })
                    })
                });


            List<int> mergedCol = new List<int>();
            int colCounter = 0;
            foreach (var group in groupedRooms)//11111111111111111111
            {
                //Заголовок группы
                data.Add(SheduleCell.Subtitle(group.Name));
                mergedCol.Add(colCounter);
                colCounter++;
                
                foreach (var item in group.Rooms)//2222222222222222222222222
                {
                    //Объединяем названия помещений
                    string fillText = Meta.shortLists(item.r.Select(y => y.Num).ToList()) + "\n";
                    foreach (var mwll in item.lvtz)//3333333333333333333333
                    {
                        if (mwll.lvt.Count() > 1)
                        {
                            foreach (var lvtex in mwll.lvt)//44444444444444444
                            {
                                colCounter++;
                                data.Add(SheduleCell.FinishRow(
                                String.Concat(fillText, " ", String.Concat(item.r.Select(x => x.FinishNote).Distinct())),
                                item.r.ElementAt(0).Ceil.Text,
                                item.r.ElementAt(0).Ceil.Value,
                                item.r.ElementAt(0).MainWall.Text,
                                item.r.ElementAt(0).MainWall.Value,
                                BotWallText: lvtex.lvte.First().Text,
                                BotWallValue: lvtex.lvte.First().Value,
                                Note: item.EndNote

                                ));
                            }
                        }
                        else
                        {
                            colCounter++;
                            data.Add(SheduleCell.FinishRow(
                                String.Concat(fillText, " ", String.Concat(item.r.Select(x => x.FinishNote).Distinct())),
                                item.r.ElementAt(0).Ceil.Text,
                                item.r.ElementAt(0).Ceil.Value,
                                item.r.ElementAt(0).MainWall.Text,
                                item.r.ElementAt(0).MainWall.Value,
                                BotWallText: item.r.ElementAt(0).LocalWall.Text,
                                BotWallValue: item.r.ElementAt(0).LocalWall.Value,
                                Note: item.EndNote
                                ));
                        }
                    }
                    
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
                .Select(groupLambda => new
                {
                    Name = groupLambda.Key,
                    Rooms = groupLambda
                    .GroupBy(floorLambda => (
                        floorLambda.Floor.Type,
                        true ? null : floorLambda.Plintus.Type))
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
                    string fillText = "";
                    if (form.withnames == 1)
                    {
                        foreach (RoomFinishing gg in item.r.GroupBy(x => x.Num).Select(g => g.First()))
                        {
                            fillText += gg.Name + "-" + gg.Num + ", ";
                        }
                        fillText = fillText.Remove(fillText.Length - 2, 2) + "\n";
                    }
                    else
                    {
                        fillText = Meta.shortLists(item.r.Select(y => y.Num).ToList()) + "\n";
                    }
                    if (item.r.ElementAt(0).FloorList.Select(x=>x.Text).Distinct().Count() > 1)
                    {
                        
                        groups.Add(new int[] { data.Count, data.Count + item.r.ElementAt(0).FloorList.Count - 1 });
                        int suffix = 1;
                        foreach (var r in item.r)
                        {
                            r.refElement.setP("Тип пола", "");
                        }
                        foreach (var fl in item.r.ElementAt(0).FloorList)
                        {
                            colCounter++;
                            try
                            {
                                fl.refEl.Document.GetElement(fl.refEl.GetTypeId()).setP("Маркировка типоразмера",String.Concat(floorNum.ToString() + "." + suffix.ToString()));
                            }
                            catch (Exception)
                            {
                            }
                            data.Add(SheduleCell.FloorRow(
                                String.Concat(fillText, " ",String.Concat( item.r.Select(x=>x.FloorNote).Distinct())),
                                floorNum.ToString() + "." + suffix.ToString(),
                                fl.Text,
                                fl.Value));
                            suffix++;
                        }
                    }
                    else
                    {
                        colCounter++;
                        foreach (var r in item.r)
                        {
                            if (r.FakeRoom)
                            {
                                continue;
                            }
                            r.refElement.setP("Тип пола", floorNum.ToString());
                        }
                        data.Add(SheduleCell.FloorRow(
                                String.Concat(fillText," ", String.Concat(item.r.Select(x => x.FloorNote).Distinct())),
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
            //shed.setHeight();
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
                    
                    //writePlintus(doc, r);


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

    }
}
