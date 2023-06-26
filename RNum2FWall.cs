using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class RNum2FWall : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;
            FinishForm MainForm = new FinishForm(null,null);
            MainForm.disableSomeElements("Number");
            MainForm.ShowDialog();
            lastPhase = MainForm.retPhase;
            idPhase = lastPhase.Id;



            
            




            var providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
            ElementParameterFilter room_filter = new ElementParameterFilter(rRule);
            ElementParameterFilter door_filter = new ElementParameterFilter(fRule);

            IList<Element> rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(room_filter)
                .ToElements();

            IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();
            IList<Element> allFloors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();

            List<Element> otdWalls=allWalls.Where(x => x.Name.StartsWith("(Вн_О)")).ToList();


            //List<Element> otdWalls=allWalls.Where(x => x.Name.StartsWith("!!отделка")).ToList();
            List<Room> roomofWall = new List<Room>();






            using (Transaction tr = new Transaction(doc, "creating"))
            {
                tr.Start();
                foreach (Room item in rooms)
                {
                    PickWall(item, doc);
                }
                foreach (var i in allFloors)
                {
                    if (i.LookupParameter("Room_ID").AsInteger()==0)
                    {
                        continue;
                    }
                    BoundingBoxXYZ bBox = i.get_BoundingBox(null);

                    if (bBox != null)
                    {
                        XYZ origin = new XYZ((bBox.Max.X + bBox.Min.X) / 2, (bBox.Max.Y + bBox.Min.Y) / 2, (bBox.Max.Z + bBox.Min.Z) / 2);
                        

                        try
                        {
                            i.setP("Room_ID", doc.GetRoomAtPoint(origin, lastPhase).Id.IntegerValue);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            i.setP("Room_ID", doc.GetRoomAtPoint(origin, lastPhase).Id.IntegerValue);
                        }
                        catch (Exception)
                        {
                        }
                    }




                }
                /*
                foreach (Element i in otdWalls)
                {

                    BoundingBoxXYZ bBox = i.get_BoundingBox(null);

                    if (bBox!=null)
                    {
                        XYZ origin = new XYZ((bBox.Max.X + bBox.Min.X) / 2, (bBox.Max.Y + bBox.Min.Y) / 2, (bBox.Max.Z + bBox.Min.Z) / 2);
                        try
                        {
                            i.setP("Помещение", doc.GetRoomAtPoint(origin,lastPhase).Number);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            i.setP("Имя помещения", doc.GetRoomAtPoint(origin, lastPhase).Number);
                        }
                        catch (Exception)
                        {
                        }
                    }


                   
                       
            }
                */





                tr.Commit();
            }

                return Result.Succeeded;
        }

        void PickWall(Room room,Document doc)
        {
            var boundary = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            foreach (var b in boundary)
            {
                foreach (var s in b)
                {
                    var neighbour = doc.GetElement(s.ElementId);
                    if (neighbour is Wall)
                    {
                        var wall = neighbour as Wall;
                        if (wall.Name.StartsWith("(Вн_О)"))
                        {
                            wall.setP("Room_ID", room.Id.IntegerValue);
                        }
                        
                    }
                    if (neighbour is Floor)
                    {
                        var floor = neighbour as Floor;
                        try
                        {
                            floor.setP("Room_ID", room.Id.IntegerValue);
                        }
                        catch (Exception)
                        {

                           
                        }
                        

                    }
                }
            }
        }
    }

    public class FinishType
    {
        string Info { get; set; }
        string Name { get; set; }
        public FinishClasses finType { get; set; }
        public FinishType(FamilySymbol sym)
        {
            
        }
    }
}