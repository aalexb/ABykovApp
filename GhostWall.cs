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

        public ElementId Level { get; set; }
        public string Room { get; set; }
        public double Area { get; set; }
        public int Secondary { get; set; }
        

        public GhostWall (string room, ElementId level, double area)
        {
            Room = room;
            Level = level;
            Area = area;

        }
        public GhostWall(Element wall) { 
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
