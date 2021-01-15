using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PerimetralWall : IExternalCommand
    {
        

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            List<Room> selectedRooms = selectedElements.Select(x => doc.GetElement(x) as Room).ToList();
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            List<IList<IList<BoundarySegment>>> roomBounds = new List<IList<IList<BoundarySegment>>>();
            foreach (var r in selectedRooms)
            {
                //IList<IList<BoundarySegment>> i = r.GetBoundarySegments(options);
                roomBounds.Add(r.GetBoundarySegments(options));
                //roomBounds.Append<IList<IList<BoundarySegment>>>(i);
            }

            //Get room boundaries, elements and disjoined curves
            List<Element> roomElems = new List<Element>();
            List<List<List<Curve>>> disjoinedCurves = new List<List<List<Curve>>>();
            foreach (IList<IList<BoundarySegment>> rb in roomBounds)
            {
                List<List<Curve>> tempCrvList = new List<List<Curve>>();
                foreach (var closedCrv in rb)
                {
                    List<Curve> tempCCCrvList = new List<Curve>();
                    foreach (var elem in closedCrv)
                    {
                        
                        if (doc.GetElement(elem.ElementId)==null)
                        {
                            roomElems.Add(null);
                            tempCCCrvList.Add(elem.GetCurve());
                        }
                        else
                        {
                            
                            roomElems.Add(doc.GetElement(elem.ElementId));

                            tempCCCrvList.Add(elem.GetCurve());
                        }
                    }
                    tempCrvList.Add(tempCCCrvList);
                }
                disjoinedCurves.Add(tempCrvList);
            }

            //Join curves in polycurves
            List<List<CurveLoop>> joinedCurves = new List<List<CurveLoop>>();
            foreach (var d in disjoinedCurves)
            {
                List<CurveLoop> tempList = new List<CurveLoop>();
                foreach (var item in d)
                {
                    tempList.Add(CurveLoop.Create(item));
                }
                joinedCurves.Add(tempList);
            }

            //Check the sense of polycurve
            foreach (var j in joinedCurves)
            {
                foreach (CurveLoop crv in j)
                {
                    
                    if (crv.GetPlane().Normal.Z<0)
                    {
                        crv.Flip();
                    }
                }
            }

            List<List<Room>> repeatedRooms = new List<List<Room>>();
            int count = 0;

            foreach (var j in joinedCurves)
            {
                List<Room> tempList = new List<Room>();
                foreach (CurveLoop crv in j)
                {

                    tempList.Add(selectedRooms.ElementAt(count));
                    

                }
                repeatedRooms.Add(tempList);
                count+=1;
            }

            List<Element> allWallTypes = new FilteredElementCollector(doc).OfClass(typeof(WallType)).ToList();

            return Result.Succeeded;
        }
    }
}
