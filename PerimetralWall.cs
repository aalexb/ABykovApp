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

            FinishForm MainForm = new FinishForm(doc);
            MainForm.disFElements("New");
            MainForm.ShowDialog();

            double FT = 0.3048;
            

            Phase lastPhase = MainForm.retPhase;
            ElementId idPhase = lastPhase.Id;
            //Выбираем элементы в Ревите
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            List<Room> selectedRooms = selectedElements.Select(x => doc.GetElement(x) as Room).ToList();

            //Находим граничные элементы помещения
            //SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();

            List<IList<IList<BoundarySegment>>> roomBounds = selectedRooms.Select(x => x.GetBoundarySegments(new SpatialElementBoundaryOptions())).ToList();
            
            //foreach (var r in selectedRooms)
            //{ 
            //    roomBounds.Add(r.GetBoundarySegments(options));
            //}

            //Получаем элементы границ и несоединенные кривые
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
                        tempCCCrvList.Add(elem.GetCurve());
                        if (doc.GetElement(elem.ElementId)==null)//Если элемент косячный
                        {
                            roomElems.Add(null);                            
                        }
                        else
                        {
                            roomElems.Add(doc.GetElement(elem.ElementId));                        
                        }
                    }
                    tempCrvList.Add(tempCCCrvList);
                }
                disjoinedCurves.Add(tempCrvList);
            }

            //Соединяем кривые в полилинии
            List<List<CurveLoop>> joinedCurvesUnfl = new List<List<CurveLoop>>();
            foreach (var d in disjoinedCurves)
            {
                List<CurveLoop> tempList = d.Select(x => CurveLoop.Create(x)).ToList();
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
            List<string> getRoomNumbers = new List<string>();

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

            //GlobalParameter OTD_Main;
            //using (Transaction tryGlobal = new Transaction(doc, "defineGlobal"))
            //{
            //    tryGlobal.Start();

            //    if (GlobalParametersManager.FindByName(doc, "ОТД_Основная") != ElementId.InvalidElementId)
            //    {
            //        OTD_Main = doc.GetElement(GlobalParametersManager.FindByName(doc, "ОТД_Основная")) as GlobalParameter;
            //    }
            //    else
            //    {
            //        OTD_Main = GlobalParameter.Create(doc, "ОТД_Основная", ParameterType.Text);
                    
            //    }

            //    tryGlobal.Commit();
            //}
            string s_OTD_Main = MainForm.wTypeBoxes[0];
            //string s_OTD_Main = ((StringParameterValue)OTD_Main.GetValue()).Value;
            //wTypes.Add(MainForm.wTypeBoxes)
            foreach (Element wt in allWallTypes)
            {
                if (wt.Name == s_OTD_Main)
                {
                    wTypes.Add(wt);
                }
            }
            //foreach (Room r in repeatedRoomsFl)
            //{
            //    //wHeights.Add(r.getP(s_OTD_Main));
            //    //allWallTypes.Where(x => x.Name == r.getP("setFFF")).ToList();
            //    foreach (Element wt in allWallTypes)
            //    {
            //        if (wt.Name==r.getP(s_OTD_Main))
            //        {
            //            wTypes.Add(wt);
            //        }
            //    }
            //}

            //Level of each room
            List<Level> levels = repeatedRoomsFl.Select(x => x.Level).ToList();

            //Create offset curve
            List<CurveLoop> offsetedCurves = new List<CurveLoop>();
            DisplayUnitType docLengthUnit = doc.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            count = 0;
            foreach (CurveLoop j in joinedCurvesFl)
            {
                double valueWith = wTypes[0].get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
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
                    try
                    {
                        offsetedCurves.Add(CurveLoop.CreateViaOffset(j, (valueWith * (-0.5)), j.GetPlane().Normal));
                    }
                    catch (Exception)
                    {

                        
                    }
                    
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
                        Wall w = Wall.Create(doc, crv, wTypes[0].Id, levels[count].Id, 2/FT, 0, false, false);
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
