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
        }
    }
}
