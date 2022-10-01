using Autodesk.Revit.ApplicationServices;
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

			//GlobalParameter.
			//GlobalParameter one = ;
			FinishForm questions = new FinishForm(doc);
			questions.Show();
			questions.Activate();
			

			double FT = 0.3048;
			PhaseArray xcom = doc.Phases;
			Phase lastPhase = xcom.get_Item(xcom.Size - 1);
			ElementId idPhase = lastPhase.Id;
			FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();

			List<SharedParameterElement> shParamElements = new FilteredElementCollector(doc)
				.OfClass(typeof(SharedParameterElement))
				.Cast<SharedParameterElement>()
				.ToList();
			//SharedParameterElement shParam = shParamElements.Where(x => x.Name == "ADSK_Номер здания").First();

			//Фильтр: Помещения на последней стадии
			FilterableValueProvider providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
			FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
			ElementParameterFilter room_filter = new ElementParameterFilter(rRule);		
			//FilterableValueProvider provRoomSchool = new ParameterValueProvider(shParam.Id);
			FilterStringRuleEvaluator StrEvaluator = new FilterStringEquals();
			//FilterRule rScRule = new FilterStringRule(provRoomSchool, StrEvaluator, "",false);
			//ElementParameterFilter roomSc_filter = new ElementParameterFilter(rScRule);

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

            List<FamilySymbol> ento = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Entourage)
              .WhereElementIsElementType()
              .Cast<FamilySymbol>()
              .ToList();

            List<String> entoName = new List<string>();
            foreach (FamilySymbol i in ento)
            {
                entoName.Add(i.Name);
            }

            List<String> entoFamily = new List<string>();
			//List<otdelka> otd = new List<otdelka>();
            foreach (FamilySymbol f in ento)
            {
				//otd.Add(new otdelka(f.FamilyName+':'+f.Name,f.getP("АР_Состав отделки")));
                entoFamily.Add(f.FamilyName);
            }
            //List<String> one = new List<string>();
            
            //foreach (FamilySymbol f in ento)
            //{

            //    one.Add(f.getP("АР_Состав отделки"));
            //}
			//string two = doc.GetElement(rooms[0].LookupParameter("ОТД_Пол").AsElementId()).Name;

            List<Element> walls = new List<Element>();
			List<GhostWall> cWalls = new List<GhostWall>();
            foreach (Element item in allWalls)
            {
                if (item.LookupParameter("Помещение").AsString()!=null & item.LookupParameter("Помещение").AsString() != "")
                {
					bool isLocal = (item as Wall).WallType.LookupParameter("rykomoika").AsInteger() == 1 ? true : false;
					walls.Add(item);

					cWalls.Add(new GhostWall(
						item.getP("Помещение"),
						item.LevelId,
						item.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble(),
                        isLocal
						));
                }
            }
			List<ElementId> Levels = new List<ElementId>();
			rooms = rooms.OrderBy(x => x.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString()).ToList();
			List<RoomFinishing> novaRooms = new List<RoomFinishing>();
            foreach (Element e in rooms)
            {
				novaRooms.Add(new RoomFinishing(e));
            }

			novaRooms = novaRooms.OrderBy(x => x.Num).ToList();

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
			List<string> WallsLocal = new List<string>();
			List<List<Element>> roomByLevel = new List<List<Element>>();
			List<List<String>> roomNumByLevel = new List<List<String>>();
			List<List<String>> CeilTextByLevel = new List<List<string>>();
			List<List<String>> MainTextByLevel = new List<List<string>>();
			List<List<String>> FloorTextByLevel = new List<List<string>>();
			List<List<List<Element>>> FinishTable = new List<List<List<Element>>>();
			List<List<List<String>>> FinishTableNum = new List<List<List<String>>>();
			List<List<List<double>>> FinishTableW3S = new List<List<List<double>>>();
			List<List<Element>> wallByLevel = new List<List<Element>>();
			List<List<String>> wallNumByLevel = new List<List<String>>();
			List<List<double>> wallAreaByLevel = new List<List<double>>();
			List<List<double>> WallS1 = new List<List<double>>();
			List<List<double>> WallS2 = new List<List<double>>();
			List<List<string>> WallsLocalText = new List<List<string>>();
			List< List < List < Element >>> floorTable = new List<List<List<Element>>>();
			List<List<List<string>>> floorTableNum = new List<List<List<string>>>();
			List<List<List<double>>> plintTable = new List<List<List<double>>>();
			List<List<double>> plintByLevel = new List<List<double>>();
			List<List<double>> perimByLevel = new List<List<double>>();
			
			
			foreach (ElementId lev in Levels.Distinct().OrderBy(x => doc.GetElement(x).Name))
            {
				List<Element> s = new List<Element>();
				List<String> n = new List<String>();
				List<String> ct = new List<String>();
				List<String> mt = new List<String>();
				List<String> ft = new List<String>();
				List<double> ws = new List<double>();
				List<double> ws2 = new List<double>();
				List<string> wt = new List<string>();
				List<double> pl = new List<double>();
				List<double> pr = new List<double>();

				for (int i = 0; i < Levels.Count(); i++)
				{
                    if (Levels[i]==lev)
                    {
						s.Add(rooms.ElementAt(i));
						n.Add(rooms.ElementAt(i).get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
						ct.Add(rooms.ElementAt(i).LookupParameter("ОТД_Потолок").AsValueString());
						mt.Add(rooms.ElementAt(i).LookupParameter("ОТД_Стены").AsValueString());
						ft.Add(rooms.ElementAt(i).LookupParameter("ОТД_Пол").AsValueString());
						pr.Add(rooms.ElementAt(i).get_Parameter(BuiltInParameter.ROOM_PERIMETER).AsDouble());
						ws.Add(0);
						ws2.Add(0);
						pl.Add(0);
						wt.Add("");

						CeilText.Add(rooms.ElementAt(i).LookupParameter("ОТД_Потолок").AsValueString());
						MainText.Add(rooms.ElementAt(i).LookupParameter("ОТД_Стены").AsValueString());
						FloorText.Add(rooms.ElementAt(i).LookupParameter("ОТД_Пол").AsValueString());
						
					}
				}
				roomByLevel.Add(s);
				roomNumByLevel.Add(n);
				CeilTextByLevel.Add(ct);
				MainTextByLevel.Add(mt);
				FloorTextByLevel.Add(ft);
				WallS1.Add(ws);
				WallS2.Add(ws2);
				WallsLocalText.Add(wt);
				plintByLevel.Add(pl);
				perimByLevel.Add(pr);


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

			//Плинтус
			foreach (FamilyInstance d in doors)
			{
                foreach (RoomFinishing r in novaRooms)
                {
                    try
                    {
                        if (d.get_FromRoom(lastPhase).Id==r.Id | d.get_ToRoom(lastPhase).Id == r.Id)
                        {
							r.Perimeter -= d.LookupParameter("сп_Ширина проёма").AsDouble();
                        }
                    }
                    catch (Exception)
                    { 
                    }
                }
			}

			//Задаём площади отделки помещений и указываем неизменные помещения
			foreach (ElementId lev in novaRooms.Select(x=>x.Level).Distinct())
            {
                foreach (RoomFinishing r in novaRooms.Where(x=>x.Level==lev))
                {
                    //Стены
                    for (int i = 0; i < novaRooms.Select(x => x.Level).Distinct().Count(); i++)
                    {
                        for (int w = 0; w < wallNumByLevel[i].Count(); w++)
                        {
                            if (wallByLevel[i][w].LevelId != lev)
                            {
                                continue;
                            }
                            Wall checkWall = (Wall)wallByLevel[i][w];
                            if (r.Num == wallNumByLevel[i][w])
                            {

                                if (checkWall.WallType.LookupParameter("rykomoika").AsInteger() == 1)
                                {
                                    r.LocalWallVal += wallAreaByLevel[i][w];
                                    r.LocalWallText = checkWall.WallType.LookupParameter("СоставОтделкиСтен").AsString();
                                    WallsLocal.Add(checkWall.WallType.LookupParameter("СоставОтделкиСтен").AsString());
                                    continue;
                                }
                                r.MainWallVal += wallAreaByLevel[i][w];
                                WallsLocal.Add("");
                            }
                        }
                    }
                }
            }


            for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
            {
                for (int r = 0; r < roomNumByLevel[lev].Count(); r++)
                {

                    //Плинтус
                    for (int i = 0; i < doors.Count(); i++)
                    {
                        try
                        {
                            if (doors[i].get_FromRoom(lastPhase).Id == roomByLevel[lev][r].Id | doors[i].get_ToRoom(lastPhase).Id == roomByLevel[lev][r].Id)
                            {
                                plintByLevel[lev][r] += doors[i].LookupParameter("Ширина").AsDouble();
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }

                    //Стены
                    for (int w = 0; w < wallNumByLevel[lev].Count(); w++)
                    {
						
						Wall checkWall = (Wall)wallByLevel[lev][w];
						if (roomNumByLevel[lev][r]==wallNumByLevel[lev][w])
                        {
							
                            if (checkWall.WallType.LookupParameter("rykomoika").AsInteger()==1)
                            {
								WallS2[lev][r]+= wallAreaByLevel[lev][w];
								WallsLocalText[lev][r] = checkWall.WallType.LookupParameter("СоставОтделкиСтен").AsString();
								WallsLocal.Add(checkWall.WallType.LookupParameter("СоставОтделкиСтен").AsString());
								continue;
							}
							WallS1[lev][r] += wallAreaByLevel[lev][w];
							WallsLocal.Add("");
                        }
                    }
                }
            }
			WallsLocal = WallsLocal.OrderBy(x=>x).ToList();
            

			//Сортируем помещения по типу отделки потолка и стен
			int finishTypes = 0;
			List<List<RoomFinishing>> novaFinishTable = new List<List<RoomFinishing>>();


			if (WallsLocal.Count == 0)
			{
				foreach (string c in novaRooms.Select(x => x.CeilType).Distinct())
				{
					foreach (string w in novaRooms.Select(x => x.WallType).Distinct())
					{
						List<RoomFinishing> cw = novaRooms
							.Where(x => x.CeilType == c)
							.Where(y => y.WallType == w)
							.ToList();
						novaFinishTable.Add(cw);
						foreach (RoomFinishing r in cw)
						{
							r.SimilarWallVal = cw.Sum(x => x.MainWallVal);

						}

					}
				}
			}
			else
			{
				foreach (string wt3 in WallsLocal.Distinct())
				{
					foreach (string c in novaRooms.Select(x => x.CeilType).Distinct())
					{
						foreach (string w in novaRooms.Select(x => x.WallType).Distinct())
						{
							List<RoomFinishing> cw = novaRooms
								.Where(x => x.CeilType == c)
								.Where(y => y.WallType == w)
								.ToList();
							novaFinishTable.Add(cw);
							foreach (RoomFinishing r in cw)
							{
								r.SimilarWallVal = cw.Sum(x => x.MainWallVal);

							}

						}
					}
				}
			}


				if (WallsLocal.Count==0)
            {
				foreach (String i in CeilText.Distinct())
				{
					foreach (String j in MainText.Distinct())
					{
						FinishTable.Add(new List<List<Element>>());
						FinishTableNum.Add(new List<List<string>>());
						FinishTableW3S.Add(new List<List<double>>());

						for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
						{
							List<Element> SimilarFinish = new List<Element>();
							List<String> SimilarFinishNum = new List<String>();
							List<double> SimW3S = new List<double>();
							for (int r = 0; r < roomByLevel[lev].Count(); r++)
							{

								if (CeilTextByLevel[lev][r] == i & MainTextByLevel[lev][r] == j)
								{
									SimilarFinish.Add(roomByLevel[lev][r]);
									SimilarFinishNum.Add(roomNumByLevel[lev][r]);
									SimW3S.Add(WallS2[lev][r]);

								}
							}
							FinishTable[finishTypes].Add(SimilarFinish);
							FinishTableNum[finishTypes].Add(SimilarFinishNum);
							FinishTableW3S[finishTypes].Add(SimW3S);
						}
						finishTypes++;
					}
				}

		}
            else
            {
				foreach (string wt3 in WallsLocal.Distinct())
				{
					foreach (String i in CeilText.Distinct())
					{
						foreach (String j in MainText.Distinct())
						{
							FinishTable.Add(new List<List<Element>>());
							FinishTableNum.Add(new List<List<string>>());
							FinishTableW3S.Add(new List<List<double>>());

							for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
							{
								List<Element> SimilarFinish = new List<Element>();
								List<String> SimilarFinishNum = new List<String>();
								List<double> SimW3S = new List<double>();
								for (int r = 0; r < roomByLevel[lev].Count(); r++)
								{

									if (CeilTextByLevel[lev][r] == i & MainTextByLevel[lev][r] == j & WallsLocalText[lev][r] == wt3)
									{
										SimilarFinish.Add(roomByLevel[lev][r]);
										SimilarFinishNum.Add(roomNumByLevel[lev][r]);
										SimW3S.Add(WallS2[lev][r]);

									}
								}
								FinishTable[finishTypes].Add(SimilarFinish);
								FinishTableNum[finishTypes].Add(SimilarFinishNum);
								FinishTableW3S[finishTypes].Add(SimW3S);
							}
							finishTypes++;
						}
					}
				}
			}

			List<List<RoomFinishing>> novaFloorTable = new List<List<RoomFinishing>>();
            foreach (string i in novaRooms.Select(x=>x.FloorType).Distinct())
            {
                foreach (string pl in novaRooms.Select(x=>x.PlintusType).Distinct())
                {
					List < RoomFinishing > flpl= novaRooms
						.Where(x => x.FloorType == i)
						.Where(y => y.PlintusType == pl)
						.ToList();
					novaFloorTable.Add(flpl);
                    foreach (RoomFinishing r in flpl)
                    {
						r.SimilarPlintusVal = flpl.Sum(x => x.Perimeter);

                    }
                }   
            }

			//Сортируем помещения по типу пола  
			int floorTypes = 0;
            foreach (string i in FloorText.Distinct())
            {
				floorTable.Add(new List<List<Element>>());
				floorTableNum.Add(new List<List<string>>());
				plintTable.Add(new List<List<double>>());
                for (int lev = 0; lev < Levels.Distinct().Count(); lev++)
                {
					List<Element> simFloor = new List<Element>();
					List<string> simFloorNum = new List<string>();
					List<double> simPlint = new List<double>();
					for (int r = 0; r < roomByLevel[lev].Count(); r++)
                    {
						if (FloorTextByLevel[lev][r] == i)
                        {
							simFloor.Add(roomByLevel[lev][r]);
							simFloorNum.Add(roomNumByLevel[lev][r]);
							simPlint.Add(perimByLevel[lev][r]-plintByLevel[lev][r]);
                        }
                    }
					floorTable[floorTypes].Add(simFloor);
					floorTableNum[floorTypes].Add(simFloorNum);
					plintTable[floorTypes].Add(simPlint);
				}
				
				floorTypes++;

            }










            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
				GlobalParameter ohohoh = GlobalParametersManager.FindByName(doc, "НесколькоЭтажей") != ElementId.InvalidElementId ?
				doc.GetElement(GlobalParametersManager.FindByName(doc, "НесколькоЭтажей")) as GlobalParameter :
				GlobalParameter.Create(doc, "НесколькоЭтажей", ParameterType.YesNo);
				


				int MoreThenOneLevel = ((IntegerParameterValue)ohohoh.GetValue()).Value;

				//Передаем номера помещений с одинаковым типом отделки стен и потолка
				for (int lev = 0; lev < roomByLevel.Count(); lev++)
                {
                    for (int r = 0; r < roomByLevel[lev].Count(); r++)
                    {
                        roomByLevel[lev][r].LookupParameter("SanT").Set(WallsLocalText[lev][r]);
                        roomByLevel[lev][r].LookupParameter("ДлинаПроемов").Set(plintByLevel[lev][r]);
                    }
                }

                for (int i = 0; i < FinishTable.Count(); i++)
                {
                    String fillText = "";
                    //String fillText2 = "";
                    double sumW3S = 0;
                    for (int lev = 0; lev < FinishTable[i].Count(); lev++)
                    {
                        sumW3S += FinishTableW3S[i][lev].Sum() * (FT * FT);
                        if (FinishTable[i][lev].Count() == 0)
                        {
                            continue;
                        }
                        else
                        {
							if (MoreThenOneLevel==1)
							{
								fillText += (lev + 1).ToString() + " этаж:\n";
							}                            
                            fillText += Meta.shortLists(FinishTableNum[i][lev]);
                            fillText += "\n";
                        }
                    }
                    for (int lev = 0; lev < FinishTable[i].Count(); lev++)
                    {
                        for (int r = 0; r < FinishTable[i][lev].Count(); r++)
                        {
							try
							{
								FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Стены").Set(doc.GetElement(FinishTable[i][lev][r].LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
								//FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Потолок").Set(doc.GetElement(FinishTable[i][lev][r].LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{
								FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Стены").Set("НЕТ ОТДЕЛКИ");
								//FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Потолок").Set("НЕТ ОТДЕЛКИ");
							}
							try
							{
								//FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Стены").Set(doc.GetElement(FinishTable[i][lev][r].LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
								FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Потолок").Set(doc.GetElement(FinishTable[i][lev][r].LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{
								//FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Стены").Set("НЕТ ОТДЕЛКИ");
								FinishTable[i][lev][r].LookupParameter("ОТД_Состав.Потолок").Set("НЕТ ОТДЕЛКИ");
							}
							FinishTable[i][lev][r].LookupParameter("testW").Set(fillText);
                            //FinishTable[i][lev][r].LookupParameter("unitTest").Set(fillText2);
                            FinishTable[i][lev][r].LookupParameter("SanS").Set(sumW3S > 0 ? sumW3S.ToString("F1") : "");
                            FinishTable[i][lev][r].LookupParameter("snS").Set(FinishTableW3S[i][lev][r]);
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
				


				int withNames = questions.withnames;//Если нужны имена помещений
								  //Передаем номера помещений с одинаковым типом стен потолка
				foreach (List<RoomFinishing> item in novaFinishTable)
				{
					if (item == null)
					{
						continue;
					}
					String fillText = "";
					foreach (ElementId lev in item.Select(x => x.Level).Distinct())
					{
						if (MoreThenOneLevel == 1)
						{
							fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
						}
						if (withNames == 1)
						{
							foreach (RoomFinishing gg in item.Where(x => x.Level == lev))
							{
								fillText += gg.Name + "-" + gg.Num + ", ";
							}
							fillText = fillText.Remove(fillText.Length - 2, 2) + "\n";
							continue;
						}
						fillText += Meta.shortLists(item.Where(x => x.Level == lev).Select(y => y.Num).ToList()) + "\n";
					}
					foreach (ElementId lev in item.Select(x => x.Level).Distinct())
					{
						foreach (RoomFinishing r in item.Where(x => x.Level == lev))
						{
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Потолок").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{

								r.refElement.LookupParameter("ОТД_Состав.Потолок").Set("НЕТ ОТДЕЛКИ");
							}
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Стены").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Стены").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{

								r.refElement.LookupParameter("ОТД_Состав.Стены").Set("НЕТ ОТДЕЛКИ");
							}
							r.refElement.LookupParameter("testW").Set(fillText);
							//r.refElement.LookupParameter("ОТД_Кол.Стены").Set(0);
							r.refElement.LookupParameter("ОТД_Кол.Стены").Set(r.SimilarWallVal);
							

							//r.refElement.LookupParameter("PlintusTotal").Set(r.Perimeter);
							//item.Select(x => x.refElement.LookupParameter("testF").Set(fillText));
							//item.Select(x => x.refElement.LookupParameter("PlintusTotal").Set(x.SimilarPlintusVal));

						}
					}
				}

				//Передаем номера помещений с одинаковым типом отделки пола
				foreach (List<RoomFinishing> item in novaFloorTable)
                {
                    if (item==null)
                    {
						continue;
                    }
					String fillText = "";
                    foreach (ElementId lev in item.Select(x=>x.Level).Distinct())
                    {
                        if (MoreThenOneLevel==1)
                        {
							fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                        }
                        if (withNames==1)
                        {
                            foreach (RoomFinishing gg in item.Where(x => x.Level == lev))
                            {
								fillText += gg.Name + "-" + gg.Num + ", ";
							}
							fillText = fillText.Remove(fillText.Length-2,2)+"\n";
							continue;
                        }
						fillText += Meta.shortLists(item.Where(x => x.Level == lev).Select(y => y.Num).ToList())+"\n";
                    }
					foreach (ElementId lev in item.Select(x => x.Level).Distinct())
					{
						foreach (RoomFinishing r in item.Where(x => x.Level == lev))
						{
							r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{

								r.refElement.LookupParameter("ОТД_Состав.Пол").Set("НЕТ ОТДЕЛКИ");
							}
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Плинтус").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch (Exception)
							{

								r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set("НЕТ ОТДЕЛКИ");
							}
							r.refElement.LookupParameter("testF").Set(fillText);
							r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set("");

							if (r.PlintusType!="__Отделка : ---")
                            {
								r.refElement.LookupParameter("ОТД_Кол.Плинтус").Set((r.SimilarPlintusVal * FT).ToString("F1"));
							}
							
							r.refElement.LookupParameter("PlintusTotal").Set(r.Perimeter);
							//item.Select(x => x.refElement.LookupParameter("testF").Set(fillText));
							//item.Select(x => x.refElement.LookupParameter("PlintusTotal").Set(x.SimilarPlintusVal));

						}
					}

				}
      //          for (int i = 0; i < floorTable.Count(); i++)
      //          {
      //              double sumPlint = 0;
      //              String fillText = "";
      //              for (int lev = 0; lev < floorTable[i].Count(); lev++)
      //              {
      //                  sumPlint += plintTable[i][lev].Sum() * FT;
      //                  if (floorTable[i][lev].Count() == 0)
      //                  {
      //                      continue;
      //                  }
      //                  else
      //                  {
						//	if (MoreThenOneLevel==1)
						//	{
						//		fillText += (lev + 1).ToString() + " этаж:\n";
						//	}                            
      //                      fillText += Meta.shortLists(floorTableNum[i][lev]);
      //                      fillText += "\n";
      //                  }
      //              }

                    

      //              for (int lev = 0; lev < floorTable[i].Count(); lev++)
      //              {
						//for (int r = 0; r < floorTable[i][lev].Count(); r++)
						//{
      //                      try
      //                      {
						//		floorTable[i][lev][r].LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(floorTable[i][lev][r].LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
								
						//	}
      //                      catch (Exception)
      //                      {
						//		floorTable[i][lev][r].LookupParameter("ОТД_Состав.Пол").Set("НЕТ ОТДЕЛКИ");

						//	}							
      //                      floorTable[i][lev][r].LookupParameter("testF").Set(fillText);
      //                      floorTable[i][lev][r].LookupParameter("PlintusTotal").Set(sumPlint);
      //                      if (floorTable[i][lev][r].LookupParameter("плинтус").AsInteger() == 1)
      //                      {
      //                          floorTable[i][lev][r].setP("PlintusTotalT", (sumPlint * FT).ToString("F1"));
      //                      }


      //                  }
      //              }
      //          }
                tr.Commit();


            }
            //String output = String.Join(", ", roomNumByLevel[0]);
            TaskDialog msg = new TaskDialog("Info");
			msg.MainInstruction = "Ok"; //output;// FinishTable.Count().ToString();
			msg.Show(); 

			return Result.Succeeded;
		}

		
	}

}

