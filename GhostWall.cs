using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    public class Ghosty
    {
        public Element refEl { get; set; }
        public string typeName { get; set; }
        public ElementId Level { get; set; }
        public int RoomID { get; set; }
        public double Area { get; set; }
        public string sostav { get; set; }
        public void init()
        {
            RoomID= refEl.LookupParameter("Room_ID").AsInteger();
            Level = refEl.LevelId;
            Area = refEl.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            
        }
    }


    public class GhostFloor:Ghosty
    {
        public GhostFloor(Element e)
        {
            refEl = e;
            init();
            typeName = (e as Floor).FloorType.Name;
            sostav = (e as Floor).FloorType.LookupParameter("Состав").AsString();
        }
        
    }

    public class GhostWall:Ghosty
    {
        
        public string Room { get; set; }
        public int Secondary { get; set; }
        public bool isLocal = false;
        public bool countNewW = false;
        public string WallChanger { get; set; }
        

        public GhostWall (string room, ElementId level, double area, bool isLocal)
        {
            Room = room;
            Level = level;
            Area = area;
            this.isLocal = isLocal;
        }
        public GhostWall(Element wall, Element LocWall) {
            refEl = wall;
            init();
            typeName = (wall as Wall).WallType.Name;
            try
            {
                WallChanger = refEl.Document.GetElement(refEl.GetTypeId()).getP("ОтделкаФункция");
            }
            catch (Exception)
            {
            }

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
