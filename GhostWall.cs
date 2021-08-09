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
        public ElementId Level { get; set; }
        public string Room { get; set; }
        public double Area { get; set; }
        public int Secondary { get; set; }
        public bool isLocal = false;
        public string sostav { get; set; }
        

        public GhostWall (string room, ElementId level, double area, bool isLocal)
        {
            Room = room;
            Level = level;
            Area = area;
            this.isLocal = isLocal;
        }
        public GhostWall(Element wall) {
            refEl = wall;
            Room = wall.getP("Помещение");
            Level = wall.LevelId;
            Area = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            isLocal= (wall as Wall).WallType.LookupParameter("rykomoika").AsInteger() == 1 ? true : false;
            sostav = (wall as Wall).WallType.LookupParameter("СоставОтделкиСтен").AsString();
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
