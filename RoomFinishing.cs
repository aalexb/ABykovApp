using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WorkApp
{
    public class RoomFinishing
    {
        public static List<RoomFinishing> Rooms=new List<RoomFinishing>();
        public static List<List<RoomFinishing>> FinishTable = new List<List<RoomFinishing>>();
        public static List<List<RoomFinishing>> FloorTable = new List<List<RoomFinishing>>();

        public Element refElement { get; }
        public ElementId Id { get; }
        public string Name { get; }
        public string Num { get; set; }
        public ElementId Level { get; set; }
        public string CeilType { get; set; }
        public string WallType { get; set; }
        public string FloorType { get; set; }
        public double Perimeter { get; set; }
        public string PlintusType { get; set; }
        public double PlintusVal { get; set; }
        public double SimilarPlintusVal { get; set; }
        public double MainWallVal { get; set; }//Значение основной отделки стен
        public double SimilarWallVal { get; set; }
        public double LocalWallVal { get; set; }//Значение местной отделки стен
        public string LocalWallText { get; set; }//Текст местной отделки стен
        public double unitMainWallVal { get; set; }
        public double unitLocalWallVal { get; set; }


        public string SimilarFloorVal { get; set; }
        public RoomFinishing(Element e)
        {
            refElement = e;
            Id = e.Id;
            Name=e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            Num = e.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
            Level = e.LevelId;
            CeilType = e.LookupParameter("ОТД_Потолок").AsValueString();
            WallType = e.LookupParameter("ОТД_Стены").AsValueString();
            FloorType = e.LookupParameter("ОТД_Пол").AsValueString();
            PlintusType = e.LookupParameter("ОТД_Плинтус").AsValueString();
            Perimeter = e.get_Parameter(BuiltInParameter.ROOM_PERIMETER).AsDouble();
            MainWallVal = 0;
            LocalWallVal = 0;
            PlintusVal = 0;
            unitLocalWallVal = 0;
            unitMainWallVal = 0;
        }
        public static void makeFinish()
        {
            foreach (string l in Rooms.Select(x=>x.LocalWallText).Distinct())
            {
                foreach (string c in Rooms.Select(x => x.CeilType).Distinct())
                {
                    foreach (string w in Rooms.Select(x => x.WallType).Distinct())
                    {
                        List<RoomFinishing> cw = Rooms
                            .Where(x => x.CeilType == c)
                            .Where(y => y.WallType == w)
                            .Where(z => z.LocalWallText ==l)
                            .ToList();
                        FinishTable.Add(cw);
                        foreach (RoomFinishing r in cw)
                        {
                            r.SimilarWallVal = cw.Sum(x => x.unitMainWallVal);
                            r.LocalWallVal = cw.Sum(x => x.unitLocalWallVal);
                        }

                    }
                }
            }
        }

        public static void makeFloor()
        {
            foreach(string i in Rooms.Select(x => x.FloorType).Distinct())
            {
                foreach (string pl in Rooms.Select(x => x.PlintusType).Distinct())
                {
                    List<RoomFinishing> flpl = Rooms
                        .Where(x => x.FloorType == i)
                        .Where(y => y.PlintusType == pl)
                        .ToList();
                    FloorTable.Add(flpl);
                    foreach (RoomFinishing r in flpl)
                    {
                        r.SimilarPlintusVal = flpl.Sum(x => x.Perimeter);

                    }
                }
            }
        }



        public static void FinishTableCommit(int MoreThenOneLevel, int withNames,Document doc) 
        {
            foreach (List<RoomFinishing> item in FinishTable)
            {
                if (item == null)
                {
                    continue;
                }
                String fillText = "";
                foreach (ElementId lev in item.Select(x => x.Level).Distinct())
                {
                    if (MoreThenOneLevel == 1)
                    {
                        fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                    }
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
                foreach (ElementId lev in item.Select(x => x.Level).Distinct())
                {
                    foreach (RoomFinishing r in item.Where(x => x.Level == lev))
                    {
                        try
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Потолок").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                        }
                        catch (Exception)
                        {

                            r.refElement.LookupParameter("ОТД_Состав.Потолок").Set("НЕТ ОТДЕЛКИ");
                        }
                        try
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Стены").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                        }
                        catch (Exception)
                        {

                            r.refElement.LookupParameter("ОТД_Состав.Стены").Set("НЕТ ОТДЕЛКИ");
                        }
                        r.refElement.LookupParameter("testW").Set(fillText);
                        //r.refElement.LookupParameter("ОТД_Кол.Стены").Set(0);
                        r.refElement.LookupParameter("ОТД_Кол.Стены").Set(r.SimilarWallVal);


                        //r.refElement.LookupParameter("PlintusTotal").Set(r.Perimeter);
                        //item.Select(x => x.refElement.LookupParameter("testF").Set(fillText));
                        //item.Select(x => x.refElement.LookupParameter("PlintusTotal").Set(x.SimilarPlintusVal));

                    }
                }
            }
        }

        public static void FloorTableCommit(int MoreThenOneLevel, int withNames, Document doc)
        {
            foreach (List<RoomFinishing> item in FloorTable)
            {
                if (item == null)
                {
                    continue;
                }
                String fillText = "";
                foreach (ElementId lev in item.Select(x => x.Level).Distinct())
                {
                    if (MoreThenOneLevel == 1)
                    {
                        fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                    }
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
                foreach (ElementId lev in item.Select(x => x.Level).Distinct())
                {
                    foreach (RoomFinishing r in item.Where(x => x.Level == lev))
                    {
                        r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
                        try
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                        }
                        catch (Exception)
                        {

                            r.refElement.LookupParameter("ОТД_Состав.Пол").Set("НЕТ ОТДЕЛКИ");
                        }
                        try
                        {
                            r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Плинтус").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
                        }
                        catch (Exception)
                        {

                            r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set("НЕТ ОТДЕЛКИ");
                        }
                        r.refElement.LookupParameter("testF").Set(fillText);
                        r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set("");

                        if (r.PlintusType != "__Отделка : ---")
                        {
                            r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set((r.SimilarPlintusVal * Meta.FT).ToString("F1"));
                        }

                        r.refElement.LookupParameter("PlintusTotal").Set(r.Perimeter);
                        //item.Select(x => x.refElement.LookupParameter("testF").Set(fillText));
                        //item.Select(x => x.refElement.LookupParameter("PlintusTotal").Set(x.SimilarPlintusVal));

                    }
                }

            }
        }
    }
}
