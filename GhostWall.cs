using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    public class GhostWall
    {
        public Element refEl { get; set; }
        public string typeName { get; set; }
        public ElementId Level { get; set; }
        public int RoomID { get; set; }
        public string Room { get; set; }
        public double Area { get; set; }
        public int Secondary { get; set; }
        public bool isLocal = false;
        public string sostav { get; set; }
        public bool countNewW = false;
        

        public GhostWall (string room, ElementId level, double area, bool isLocal)
        {
            Room = room;
            Level = level;
            Area = area;
            this.isLocal = isLocal;
        }
        public GhostWall(Element wall, Element LocWall) {
            refEl = wall;
            typeName = (wall as Wall).WallType.Name;
            RoomID = wall.LookupParameter("Room_ID").AsInteger();
            //Room = wall.getP("Помещение");
            Level = wall.LevelId;
            Area = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            if (LocWall!=null)
            {
                isLocal = wall.Id == LocWall.Id;
            }
            sostav = (wall as Wall).WallType.LookupParameter("Состав").AsString();
            if (wall.LookupParameter("ПоНовымСтенам")!=null)
            {
                countNewW = wall.LookupParameter("ПоНовымСтенам").AsInteger() == 1 ? true : false;
            }        

        }
    }




    public class GhostRoom
    {
        public Element Room { get; set; }
        public ElementId Level { get; set; }
        public string Floor { get; set; }
        public static List<List<List<Element>>> FloorTable = new List<List<List<Element>>>();


        public GhostRoom(Element room)
        {
            Room = room;
        }


        public void Commit()
        {
            Room.setP("Ля-ля-ля","бла-бла-бла");
        }
    }

}
