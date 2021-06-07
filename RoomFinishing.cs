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
        public static List<RoomFinishing> AllRooms = new List<RoomFinishing>();
        public static List<List<RoomFinishing>> FloorTable = new List<List<RoomFinishing>>();
        public static List<List<RoomFinishing>> FinishTable = new List<List<RoomFinishing>>();

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


        public string SimilarFloorVal { get; set; }
        public RoomFinishing(Element e)
        {
            this.refElement = e;
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
        }

        public static void organizeFloor()
        {
            foreach (string i in AllRooms.Select(x=>x.FloorType).Distinct())
            {
                foreach (string pl in AllRooms.Select(x=>x.PlintusType).Distinct())
                {
                    List<RoomFinishing> flpl = AllRooms
                        .Where(x => x.FloorType == i)
                        .Where(y => y.PlintusType == pl)
                        .ToList();
                    FloorTable.Add(flpl);
                    foreach (RoomFinishing room in flpl)
                    {
                        room.SimilarPlintusVal = flpl.Sum(x => x.Perimeter);
                    }
                } 
            }
        }

        public static void organizeFinish()
        {
            foreach (string c in AllRooms.Select(x=>x.CeilType).Distinct())
            {
                foreach (string w in AllRooms.Select(x => x.WallType).Distinct())
                {
                    List<RoomFinishing> cw = AllRooms
                        .Where(x => x.CeilType == c)
                        .Where(y => y.WallType == w)
                        .ToList();
                    FinishTable.Add(cw);
                    foreach (RoomFinishing room in cw)
                    {
                        room.SimilarWallVal = cw.Sum(x => x.MainWallVal);
                    }
                }
            }
        }
    }
}
