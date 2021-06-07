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




            foreach (Element e in rooms)
            {
				RoomFinishing.AllRooms.Add(new RoomFinishing(e));
			}
			RoomFinishing.AllRooms=RoomFinishing.AllRooms.OrderBy(x => x.Num).ToList();


			//Плинтус
			foreach (FamilyInstance d in doors)
			{
				foreach (RoomFinishing r in RoomFinishing.AllRooms)
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

			//Задаём площади отделки помещений и указываем неизменные помещения
			foreach (ElementId lev in RoomFinishing.AllRooms.Select(x => x.Level).Distinct())
			{
				foreach (RoomFinishing r in RoomFinishing.AllRooms.Where(x => x.Level == lev))
				{
					//Стены
					for (int i = 0; i < RoomFinishing.AllRooms.Select(x => x.Level).Distinct().Count(); i++)
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

			//Сортируем помещения по типу отделки потолка и стен
			RoomFinishing.organizeFinish();

			//Сортируем помещения по типу пола  
			RoomFinishing.organizeFloor();


            using (Transaction tr = new Transaction(doc, "otdelka"))
            {
                tr.Start();
				GlobalParameter ohohoh = GlobalParametersManager.FindByName(doc, "НесколькоЭтажей") != ElementId.InvalidElementId ?
				doc.GetElement(GlobalParametersManager.FindByName(doc, "НесколькоЭтажей")) as GlobalParameter :
				GlobalParameter.Create(doc, "НесколькоЭтажей", ParameterType.YesNo);
				


				int MoreThenOneLevel = ((IntegerParameterValue)ohohoh.GetValue()).Value;

				//Передаем номера помещений с одинаковым типом отделки стен и потолка
				

				int withNames = 1;//Если нужны имена помещений
								  //Передаем номера помещений с одинаковым типом стен потолка
				foreach (List<RoomFinishing> item in RoomFinishing.FinishTable)
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
								fillText += gg.Name + "(" + gg.Num + "), ";
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
				foreach (List<RoomFinishing> floortype in RoomFinishing.FloorTable)
                {
                    if (floortype==null)
                    {
						continue;
                    }
					String fillText = "";
                    foreach (ElementId lev in floortype.Select(x=>x.Level).Distinct())
                    {
                        if (MoreThenOneLevel==1)
                        {
							fillText += doc.GetElement(lev).LookupParameter("Название уровня").AsString() + ":\n";
                        }
                        if (withNames==1)
                        {
                            foreach (RoomFinishing gg in floortype.Where(x => x.Level == lev))
                            {
								fillText += gg.Name + "-" + gg.Num + ", ";
							}
							fillText = fillText.Remove(fillText.Length-2,2)+"\n";
							continue;
                        }
						fillText += Meta.shortLists(floortype.Where(x => x.Level == lev).Select(y => y.Num).ToList())+"\n";
                    }
					foreach (ElementId lev in floortype.Select(x => x.Level).Distinct())
					{
						foreach (RoomFinishing r in floortype.Where(x => x.Level == lev))
						{
							r.refElement.LookupParameter("ОТД_Состав.Пол").Set("");
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Пол").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Пол").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch
							{

								r.refElement.LookupParameter("ОТД_Состав.Пол").Set("НЕТ ОТДЕЛКИ");
							}
							try
							{
								r.refElement.LookupParameter("ОТД_Состав.Плинтус").Set(doc.GetElement(r.refElement.LookupParameter("ОТД_Плинтус").AsElementId()).LookupParameter("АР_Состав отделки").AsString());
							}
							catch
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
						}
					}

				}

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

