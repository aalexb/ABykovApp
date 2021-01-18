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

            double FT = 0.3048;
            PhaseArray xcom = doc.Phases;
            Phase lastPhase = xcom.get_Item(xcom.Size - 1);
            ElementId idPhase = lastPhase.Id;

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
            List<List<CurveLoop>> joinedCurvesUnfl = new List<List<CurveLoop>>();
            foreach (var d in disjoinedCurves)
            {
                List<CurveLoop> tempList = new List<CurveLoop>();
                foreach (var item in d)
                {
                    tempList.Add(CurveLoop.Create(item));
                }
                joinedCurvesUnfl.Add(tempList);
            }

            //Check the sense of polycurve
            foreach (var j in joinedCurvesUnfl)
            {
                foreach (CurveLoop crv in j)
                {
                    
                    if (crv.GetPlane().Normal.Z<0)
                    {
                        crv.Flip();
                    }
                }
            }

            List<List<Room>> repeatedRoomsUnfl = new List<List<Room>>();
            int count = 0;

            foreach (var j in joinedCurvesUnfl)
            {
                List<Room> tempList = new List<Room>();
                foreach (CurveLoop crv in j)
                {

                    tempList.Add(selectedRooms.ElementAt(count));
                    

                }
                repeatedRoomsUnfl.Add(tempList);
                count+=1;
            }

            List<Room> repeatedRoomsFl = GenericList<Room>.Flatten(repeatedRoomsUnfl);
            List<CurveLoop> joinedCurvesFl = GenericList<CurveLoop>.Flatten(joinedCurvesUnfl);

            List<string> wHeights = new List<string>();
            List<Element> wTypes = new List<Element>();
            List<Element> allWallTypes = new FilteredElementCollector(doc).OfClass(typeof(WallType)).ToList();
            foreach (Room r in repeatedRoomsFl)
            {
                wHeights.Add(r.getP("setFFF"));
                foreach (Element wt in allWallTypes)
                {
                    if (wt.Name==r.getP("setFFF"))
                    {
                        wTypes.Add(wt);
                    }
                }
            }

            //Level of each room
            List<Level> levels = new List<Level>();
            foreach (Room r in repeatedRoomsFl)
            {
                levels.Add(r.Level);
               
            }



            //Create offset curve
            List<CurveLoop> offsetedCurves = new List<CurveLoop>();
            DisplayUnitType docLengthUnit = doc.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            count = 0;
            foreach (CurveLoop j in joinedCurvesFl)
            {
                double valueWith = wTypes[count].get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
                //double value = UnitUtils.Convert(valueWith, DisplayUnitType.DUT_DECIMAL_FEET, docLengthUnit);
                double value = valueWith / FT;
                CurveLoop gg=CurveLoop.CreateViaOffset(j, (valueWith*(0.5)),j.GetPlane().Normal);

                if (repeatedRoomsFl[count].IsPointInRoom(gg.GetCurveLoopIterator().Current.GetEndPoint(0))==true)
                {
                    
                    
                    offsetedCurves.Add(gg);
                }
                else
                {
                    //offsetedCurves.Add(gg);
                    offsetedCurves.Add(CurveLoop.CreateViaOffset(j, (valueWith * (-0.5)), j.GetPlane().Normal));
                }
                count += 1;
            }

            List<List<Curve>> explodedCurves = new List<List<Curve>>();
            foreach (CurveLoop oc in offsetedCurves)
            {
                List<Curve> tempList = new List<Curve>();
                foreach (Curve i in oc)
                {
                    tempList.Add(i);
                }
                
                explodedCurves.Add(tempList);
                
            }
            List<Wall> walls = new List<Wall>();
            using (Transaction tr = new Transaction(doc, "PerimetralWall"))
            {
                tr.Start();
                count = 0;
                foreach (List<Curve> group in explodedCurves)
                {
                    
                    foreach (Curve crv in group)
                    {
                        Wall w = Wall.Create(doc, crv, wTypes[count].Id, levels[count].Id, 2/FT, 0, false, false);
                        walls.Add(w);
                    }
                    
                    count += 1;
                    
                }
                count = 0;
                foreach (Wall w in walls)
                {
                    
                    w.get_Parameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).Set(0);
                    w.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                    w.DemolishedPhaseId= lastPhase.Id;
                    //w.setP("Помещение", repeatedRoomsFl[count].Number);
                    count += 1;
                }
                count = 0;
                /*
                foreach (Element r in roomElems)
                {
                    try
                    {
                        JoinGeometryUtils.JoinGeometry(doc, walls[count], r);
                    }
                    catch (Exception)
                    {

                       
                    }
                    count += 1;
                }
                */

                tr.Commit();
            }
                return Result.Succeeded;
        }
    }
}
