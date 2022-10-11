using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkApp
{
    public class FinishStructuralElement
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public double unitValue { get; set; }
        public double Value { get; set; }
        public FinishStructuralElement()
        {
            unitValue = 0;
            Value = 0;
            Type = "";
            Text = "";
        }
        public void setType(string t, Element r)
        {
            Type = t;
            Text = t == "__Отделка : ---" ? "" : r.Document.GetElement(r.LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString();

        }
    }
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
        //=============
        public FinishStructuralElement MainWall { get; set; }= new FinishStructuralElement();
        public FinishStructuralElement LocalWall { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement NewWall { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement Kolon { get; set; } = new FinishStructuralElement();
        public FinishStructuralElement Floor { get; set; } = new FinishStructuralElement();
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
            Kolon.Type = e.LookupParameter("ОТД_Колонны").AsValueString();
            Plintus.Type = e.LookupParameter("ОТД_Плинтус").AsValueString();
            Plintus.unitValue = e.get_Parameter(BuiltInParameter.ROOM_PERIMETER).AsDouble();

            Rooms.Add(this);
        }

        public static void makeFinish(FinishForm form)
        {
            bool combineLocalWall = true;

            var grfn = Rooms.GroupBy(key => (
            form.splitLevel?key.Level:null,
            form.ColFromMat?key.Kolon.Text:key.Kolon.Type,
            key.Ceil.Type,
            key.MainWall.Type,
            combineLocalWall?null: key.LocalWall.Type
            ));

            foreach (var f in grfn)
            {
                FinishTable.Add(f.Select(x => x).ToList());
                foreach (RoomFinishing r in f)
                {
                    r.MainWall.Value = f.Sum(x => x.MainWall.unitValue);
                    r.LocalWall.Value = f.Sum(x => x.LocalWall.unitValue);
                    r.Kolon.Value = f.Sum(x => x.Kolon.unitValue);
                    if (form.countNewW)
                    {
                        r.NewWall.Value = f.Sum(x => x.NewWall.unitValue);
                    }
                }
            }   
        }

        public static void makeFloor(FinishForm form)
        {
            var grfl = Rooms.GroupBy(
                key =>(
                form.splitLevel? key.Level:null, 
                key.Floor.Type, 
                key.Plintus.Type));
            foreach (var f in grfl)
            {
                FloorTable.Add(f.Select(x => x).ToList());
                foreach (RoomFinishing r in f)
                {
                    r.Plintus.Value = f.Sum(x => x.Plintus.unitValue);
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
                    if (r.refElement.LookupParameter("ОТД_Кол.Доп") != null) //Проверяем что существует
                    {
                        if (form.groupCheck)
                        {
                            r.refElement.LookupParameter("ОТД_Кол.ДопGROUP").Set(r.LocalWall.Value > 0 ? (r.LocalWall.Value * Meta.FT * Meta.FT).ToString("F1") : "");
                        }
                        else
                        {
                            r.refElement.LookupParameter("ОТД_Кол.Доп").Set(r.LocalWall.Value > 0 ? (r.LocalWall.Value * Meta.FT * Meta.FT).ToString("F1") : "");
                        }

                        r.refElement.LookupParameter("ОТД_Состав.Доп").Set(r.LocalWall.Text);
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

        public static void FloorTableCommit(int MoreThenOneLevel, int withNames, Document doc, FinishForm form)
        {
            foreach (List<RoomFinishing> item in FloorTable)
            {
                if (item == null)
                {
                    continue;
                }
                String fillText = "";

                /*
                 * Если 1 уровень
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 */
                if (MoreThenOneLevel==1)
                {
                    foreach (ElementId lev in item.Select(x => x.Level).Distinct())
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

                //Транзакция

                foreach (var r in item)
                {
                    r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
                    try
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                    }
                    catch (Exception)
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
                    }
                    try
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Плинтус").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                    }
                    catch (Exception)
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set("");
                    }
                    if (form.groupCheck)
                    {
                        r.refElement.LookupParameter("FMulAdd").Set(fillText);
                    }
                    else
                    {
                        r.refElement.LookupParameter("testF").Set(fillText);
                    }
                    r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set(r.Plintus.Type == "__Отделка : ---" ? "" : (r.Plintus.Value * Meta.FT).ToString("F1"));
                    r.refElement.LookupParameter("PlintusTotal").Set(r.Plintus.Value);
                }

            }
        }
    }
}
