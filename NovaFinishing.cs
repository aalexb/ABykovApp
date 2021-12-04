﻿using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
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
    class NovaFinishing : IExternalCommand
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
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();

            GlobalParameter GlobePar2 = GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter :null;
            FinishForm MainForm = new FinishForm(doc);
            MainForm.ShowDialog();
            using (Transaction tr=new Transaction(doc,"setGP"))
            {
                tr.Start();
                GlobalParameter GlobePar = GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter :
                GlobalParameter.Create(doc, "FinData", ParameterType.Text);
                GlobePar.SetValue(new StringParameterValue(string.Join("|", MainForm.wTypeBoxes)));
                //int MoreThenOneLevel = ((IntegerParameterValue)GlobePar.GetValue()).Value;
                tr.Commit();
            }

            lastPhase = MainForm.retPhase;
            idPhase = lastPhase.Id;

            List<SharedParameterElement> shParamElements = new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>()
                .ToList();
            //Фильтр: Помещения на последней стадии
            FilterableValueProvider providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
            ElementParameterFilter room_filter = new ElementParameterFilter(rRule);
            //FilterableValueProvider provRoomSchool = new ParameterValueProvider(shParam.Id);
            FilterStringRuleEvaluator StrEvaluator = new FilterStringEquals();
            IList<Element> rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(room_filter)
                //.WherePasses(roomSc_filter)
                .ToElements();

            //Фильтр: Стены созданные на последней стадии
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter door_filter = new ElementParameterFilter(fRule);

            IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(door_filter)
                .ToElements();

            //Фильтр: экземпляры дверей
            List<FamilyInstance> doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();


            foreach (Element e in rooms)
            {
                RoomFinishing.Rooms.Add(new RoomFinishing(e));
            }
            RoomFinishing.Rooms = RoomFinishing.Rooms.OrderBy(x => x.Num).ToList();

            List<GhostWall> cWalls = new List<GhostWall>();
            foreach (Element wall in allWalls)
            {
                if (wall.LookupParameter("Помещение").AsString() != null & wall.LookupParameter("Помещение").AsString() != "")
                {
                    cWalls.Add(new GhostWall(wall,MainForm.LocType));
                }
            }

            foreach (GhostWall w in cWalls)
            {
                foreach (RoomFinishing r in RoomFinishing.Rooms)
                {
                    if (r.Num==w.Room)
                    {
                        if (w.typeName == MainForm.LocType.Name)
                        {
                            r.unitLocalWallVal += w.Area;
                            r.LocalWallText = w.sostav;
                        }
                        else if (w.typeName== MainForm.ColType.Name)
                        {
                            r.unitKolonWallVal += w.Area;
                            r.KolonWallText = w.sostav;
                        }
                        else
                        {
                            r.unitMainWallVal += w.Area;
                        }
                        if (w.countNewW)
                        {
                            r.unitNewWallVal += w.Area;
                        }
                        
                    }
                }
            }


            //Плинтус
            foreach (FamilyInstance d in doors)
            {
                foreach (RoomFinishing r in RoomFinishing.Rooms)
                {
                    try
                    {
                        if (d.get_FromRoom(lastPhase).Id == r.Id | d.get_ToRoom(lastPhase).Id == r.Id)
                        {
                            r.Perimeter -= d.LookupParameter("сп_Ширина проёма").AsDouble();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            RoomFinishing.makeFinish(MainForm);
            RoomFinishing.makeFloor(MainForm.splitLevel);
            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
                GlobalParameter GlobePar = GlobalParametersManager.FindByName(doc, "НесколькоЭтажей") != ElementId.InvalidElementId ?
                doc.GetElement(GlobalParametersManager.FindByName(doc, "НесколькоЭтажей")) as GlobalParameter :
                GlobalParameter.Create(doc, "НесколькоЭтажей", ParameterType.YesNo);
                int MoreThenOneLevel = ((IntegerParameterValue)GlobePar.GetValue()).Value;

                int withNames = MainForm.withnames;
                MoreThenOneLevel = MainForm.levels;

                RoomFinishing.FinishTableCommit(doc, MainForm);
                RoomFinishing.FloorTableCommit(MoreThenOneLevel, withNames, doc);

                tr.Commit();
            }
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction =  $"Выполнен расчет отделки для стадии \"{MainForm.retPhase.Name}\"";
            msg.Show();
            return Result.Succeeded;
        }
    }
}
