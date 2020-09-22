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
	class Finishing : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;

			//double FT = 0.3048;
			PhaseArray xcom = doc.Phases;
			Phase lastPhase = xcom.get_Item(xcom.Size - 1);
			ElementId idPhase = lastPhase.Id;
			FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();

			List<SharedParameterElement> shParamElements = new FilteredElementCollector(doc)
				.OfClass(typeof(SharedParameterElement))
				.Cast<SharedParameterElement>()
				.ToList();
			SharedParameterElement shParam = shParamElements.Where(x => x.Name == "ADSK_Номер здания").First();

			//rooms
			FilterableValueProvider providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
			FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
			ElementParameterFilter room_filter = new ElementParameterFilter(rRule);		
			
			FilterableValueProvider provRoomSchool = new ParameterValueProvider(shParam.Id);
			FilterStringRuleEvaluator StrEvaluator = new FilterStringEquals();
			FilterRule rScRule = new FilterStringRule(provRoomSchool, StrEvaluator, "",false);
			ElementParameterFilter roomSc_filter = new ElementParameterFilter(rScRule);

			//walls
			FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
			FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
			ElementParameterFilter door_filter = new ElementParameterFilter(fRule);
			


			IList<Element> rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
				.WhereElementIsNotElementType()
				.WherePasses(room_filter)
				.WherePasses(roomSc_filter)
				.ToElements();

			IList<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
				.WhereElementIsNotElementType()
				.WherePasses(door_filter)
				.ToElements();
			List<Element> walls = new List<Element>();
            foreach (Element item in allWalls)
            {
                if (item.LookupParameter("Помещение").AsString()!=null & item.LookupParameter("Помещение").AsString() != "")
                {
					walls.Add(item);
                }
            }
			List<ElementId> Levels = new List<ElementId>();
			rooms = rooms.OrderBy(x => x.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString()).ToList();
			List<ElementId> wallLevels = new List<ElementId>();

            foreach (Element i in rooms)
            {
				Levels.Add(i.LevelId);				
            }
            foreach (Element i in walls)
            {
				wallLevels.Add(i.LevelId);
            }
			IEnumerable<String> LevelsName = new List<String>();
			//Levels=Levels.Distinct().ToList();
			//Levels = Levels.OrderBy(x=>doc.GetElement(x).Name).ToList();
			
            foreach (ElementId i in Levels.Distinct().OrderBy(x => doc.GetElement(x).Name))
            {
				LevelsName=LevelsName.Append( doc.GetElement(i).Name);
			}
			String str = String.Join(",", LevelsName);
			
			IEnumerable<bool> isNewC = new List<bool>();
			IEnumerable<bool> isNewW = new List<bool>();
			IEnumerable<bool> SecFin = new List<bool>();
			List<String> CeilText = new List<String>();
			List<String> MainText = new List<String>();
			List<String> FloorText = new List<String>();
			List<string> WT2 = new List<string>();
			List<List<Element>> roomByLevel = new List<List<Element>>();
			List<List<String>> roomNumByLevel = new List<List<String>>();
			List<List<String>> CeilTextByLevel = new List<List<string>>();
			List<List<String>> MainTextByLevel = new List<List<string>>();
			List<List<String>> FloorTextByLevel = new List<List<string>>();
			List<List<List<Element>>> FinishTable = new List<List<List<Element>>>();
			List<List<List<String>>> FinishTableNum = new List<List<List<String>>>();
			List<List<Element>> wallByLevel = new List<List<Element>>();
			List<List<String>> wallNumByLevel = new List<List<String>>();
			List<List<double>> wallAreaByLevel = new List<List<double>>();
			List<List<double>> WallS1 = new List<List<double>>();
			List<List<double>> WallS2 = new List<List<double>>();
			List<List<string>> WallT2 = new List<List<string>>();
			List< List < List < Element >>> floorTable = new List<List<List<Element>>>();
			List<List<List<string>>> floorTableNum = new List<List<List<string>>>();


			foreach (ElementId lev in Levels.Distinct().OrderBy(x => doc.GetElement(x).Name))
            {
				List<Element> s = new List<Element>();
				List<String> n = new List<String>();
				List<String> ct = new List<String>();
				List<String> mt = new List<String>();
				List<String> ft = new List<String>();
				List<double> ws = new List<double>();
				List<string> wt = new List<string>();

				for (int i = 0; i < Levels.Count(); i++)
				{
                    if (Levels[i]==lev)
                    {
						s.Add(rooms.ElementAt(i));
						n.Add(rooms.ElementAt(i).get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
						ct.Add(rooms.ElementAt(i).LookupParameter("Отделка потолка").AsString());
						mt.Add(rooms.ElementAt(i).LookupParameter("Отделка стен").AsString());
						ft.Add(rooms.ElementAt(i).LookupParameter("Отделка пола").AsString());
						ws.Add(0);
						wt.Add("");

						CeilText.Add(rooms.ElementAt(i).LookupParameter("Отделка потолка").AsString());
						MainText.Add(rooms.ElementAt(i).LookupParameter("Отделка стен").AsString());
						FloorText.Add(rooms.ElementAt(i).LookupParameter("Отделка пола").AsString());
						



					}
				}
				roomByLevel.Add(s);
				roomNumByLevel.Add(n);
				CeilTextByLevel.Add(ct);
				MainTextByLevel.Add(mt);
				FloorTextByLevel.Add(ft);
				WallS1.Add(ws);
				WallS2.Add(ws);
				WallT2.Add(wt);


				List<Element> w = new List<Element>();
				List<String> wn = new List<String>();
				List<double> wa = new List<double>();
				//List<Wall> est = walls as List<Wall>;
                for (int i = 0; i < wallLevels.Count(); i++)
                {
					if (wallLevels[i] == lev)
                    {
						w.Add(walls[i]);
						wn.Add(walls[i].LookupParameter("Помещение").AsString());
						wa.Add(walls[i].get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    }
                }
				wallAreaByLevel.Add(wa);
				wallByLevel.Add(w);
				wallNumByLevel.Add(wn);
			}

            //Задаём площади отделки помещений и указываем неизменные помещения

            for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
            {
                for (int r = 0; r < roomNumByLevel[lev].Count(); r++)
                {
                    for (int w = 0; w < wallNumByLevel[lev].Count(); w++)
                    {
						FamilyInstance checkWall = wallByLevel[lev][w] as FamilyInstance;
						if (roomNumByLevel[lev][r]==wallNumByLevel[lev][w])
                        {
							
                            if (checkWall.Symbol.LookupParameter("rykomoika").AsInteger()==1)
                            {
								WallS2[lev][r]+= wallAreaByLevel[lev][w];
								WallT2[lev][r] = checkWall.Symbol.LookupParameter("СоставОтделкиСтен").AsString();
								WT2.Add(checkWall.Symbol.LookupParameter("СоставОтделкиСтен").AsString());
								continue;
							}
							WallS1[lev][r] += wallAreaByLevel[lev][w];
                        }
                    }
                }
            }
			int finishTypes = 0;
			//Сортируем помещения по типу отделки потолка и стен
			foreach (string wt2 in WT2.Distinct())
			{
				foreach (String i in CeilText.Distinct())
				{
					foreach (String j in MainText.Distinct())
					{
						FinishTable.Add(new List<List<Element>>());
						FinishTableNum.Add(new List<List<string>>());
						for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
						{
							List<Element> SimilarFinish = new List<Element>();
							List<String> SimilarFinishNum = new List<String>();
							for (int r = 0; r < roomByLevel[lev].Count(); r++)
							{

								if (CeilTextByLevel[lev][r] == i & MainTextByLevel[lev][r] == j & WallT2[lev][r]==wt2)
								{
									SimilarFinish.Add(roomByLevel[lev][r]);
									SimilarFinishNum.Add(roomNumByLevel[lev][r]);
								}
							}
							FinishTable[finishTypes].Add(SimilarFinish);
							FinishTableNum[finishTypes].Add(SimilarFinishNum);
						}
						finishTypes++;
					}
				}
			}
          
			int floorTypes = 0;
            foreach (string i in FloorText.Distinct())
            {
				floorTable.Add(new List<List<Element>>());
				floorTableNum.Add(new List<List<string>>());				
                for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
                {
					List<Element> simFloor = new List<Element>();
					List<string> simFloorNum = new List<string>();
					for (int r = 0; r < roomByLevel[lev].Count(); r++)
                    {
						if (FloorTextByLevel[lev][r] == i)
                        {
							simFloor.Add(roomByLevel[lev][r]);
							simFloorNum.Add(roomNumByLevel[lev][r]);
                        }
                    }
					floorTable[floorTypes].Add(simFloor);
					floorTableNum[floorTypes].Add(simFloorNum);
				}
				
				floorTypes++;

            }
			
			

            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
				tr.Start();
                //Передаем номера помещений с одинаковым типом отделки стен и потолка
                for (int lev = 0; lev < roomByLevel.Count(); lev++)
                {
                    for (int r = 0; r < roomByLevel[lev].Count(); r++)
                    {
						roomByLevel[lev][r].LookupParameter("SanT").Set(WallT2[lev][r]);
                    }
                }
				for (int i = 0; i < FinishTable.Count(); i++)
				{
					String fillText = "";
					for (int lev = 0; lev < FinishTable[i].Count(); lev++)
					{
						if (FinishTable[i][lev].Count() == 0)
						{
							continue;
						}
						else
						{
							fillText += (lev + 1).ToString() + " этаж:\n";
							fillText += String.Join(", ", FinishTableNum[i][lev]);
							fillText += "\n";
						}
					}
					for (int lev = 0; lev < FinishTable[i].Count(); lev++)
					{
						for (int r = 0; r < FinishTable[i][lev].Count(); r++)
						{
							FinishTable[i][lev][r].LookupParameter("testW").Set(fillText);
						}
					}					
				}
				
                for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
                {
                    for (int r = 0; r < roomByLevel[lev].Count(); r++)
                    {
						roomByLevel[lev][r].LookupParameter("WallS1n").Set(WallS1[lev][r]);
                    }
                }
				//Передаем номера помещений с одинаковым типом отделки пола
				for (int i = 0; i < floorTable.Count(); i++)
				{
					String fillText = "";
                    for (int lev = 0; lev < floorTable[i].Count(); lev++)
                    {
                        if (floorTable[i][lev].Count()==0)
                        {
							continue;
                        }
                        else
                        {
							fillText += (lev+1).ToString() + " этаж:\n";
							fillText += String.Join(", ", floorTableNum[i][lev]);
							fillText += "\n";
                        }
                    }
                    for (int lev = 0; lev < floorTable[i].Count(); lev++)
                    {
                        for (int r = 0; r < floorTable[i][lev].Count(); r++)
                        {
							floorTable[i][lev][r].LookupParameter("testF").Set(fillText);
                        }						
                    }		
				}
				tr.Commit();
				
				
			}
			String output = String.Join(", ", roomNumByLevel[0]);
			TaskDialog msg = new TaskDialog("Info");
			msg.MainInstruction = "Ok"; //output;// FinishTable.Count().ToString();
			msg.Show(); 

			return Result.Succeeded;
		}
	}

}