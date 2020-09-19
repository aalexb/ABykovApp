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
	class RoomElements : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;
			
			PhaseArray xcom = doc.Phases;
			Phase lastPhase = xcom.get_Item(xcom.Size-1);
			FilterableValueProvider providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
			FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
			FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();

			double FT = 0.3048;

			ElementId idPhase = lastPhase.Id;
			FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
			FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
			ElementParameterFilter room_filter = new ElementParameterFilter(rRule);
			ElementParameterFilter door_filter = new ElementParameterFilter(fRule);
			
			
			IList<Element> rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
				.WhereElementIsNotElementType()
				.WherePasses(room_filter)
				.ToElements();

			IList<FamilyInstance> doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
				.WhereElementIsNotElementType()
				.WherePasses(door_filter).Cast<FamilyInstance>().ToList();
			IList<FamilyInstance> windows = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows)
				.WhereElementIsNotElementType()
				.WherePasses(door_filter).Cast<FamilyInstance>().ToList();

			FamilySymbol neocube = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(q => q.Name == "cube").First() as FamilySymbol;
			IList<XYZ> a=new List<XYZ>();
			IList<String> b = new List<String>();
			//Room two = rooms[0] as Room;
			int g = 0;
			FilterStringRuleEvaluator cubeEval = new FilterStringEquals();
			FilterableValueProvider cubeProv = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME));
			FilterStringRule cubeRule = new FilterStringRule(cubeProv, cubeEval, "cube", false);
			ElementParameterFilter cubeFilter = new ElementParameterFilter(cubeRule);

			List<FamilyInstance> existCubes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().WherePasses(cubeFilter).Cast<FamilyInstance>().ToList();


			using (Transaction tr = new Transaction(doc,"creating"))
			{
				tr.Start();

                foreach (FamilyInstance i in existCubes)
                {
					doc.Delete(i.Id);
                }
				foreach (Element i in rooms)
				{

                    for (int ind = 0; ind < doors.Count; ind++)
                    {
						FamilyInstance dr = doors[ind];
						try
						{
							if (dr.get_FromRoom(lastPhase).Id == i.Id | dr.get_ToRoom(lastPhase).Id == i.Id)
							{

								BoundingBoxXYZ bBox = i.get_BoundingBox(null);
								LocationPoint origin = (LocationPoint)i.Location;

								XYZ center = origin.Point;
								if (!neocube.IsActive)
									neocube.Activate();
								FamilyInstance cubeIns = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								cubeIns.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								cubeIns.LookupParameter("MainText").Set(dr.Symbol.LookupParameter("ADSK_Марка").AsString());
								cubeIns.setType();
								
								b.Append("ok");//dr.LookupParameter("ADSK_Марка").AsValueString());
								doors.Remove(dr);
								ind--;

							}
						}
						catch (Exception)
						{

						}

					}
					/*
					foreach (FamilyInstance dr in doors)
					{
						try
						{
							if (dr.get_FromRoom(lastPhase).Id == i.Id | dr.get_ToRoom(lastPhase).Id == i.Id)
							{
								
								BoundingBoxXYZ bBox = i.get_BoundingBox(null);
								LocationPoint origin = (LocationPoint)i.Location;

								XYZ center = origin.Point;
								if (!neocube.IsActive)
									neocube.Activate();
								FamilyInstance cubeIns = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								cubeIns.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								cubeIns.LookupParameter("MainText").Set(dr.Symbol.LookupParameter("ADSK_Марка").AsString());
								cubeIns.LookupParameter("isAreo").Set(0);

								b.Append("ok");//dr.LookupParameter("ADSK_Марка").AsValueString());
								
							}
						}
						catch (Exception)
						{

						}
					}
					*/
					foreach (FamilyInstance dr in windows)
					{
						try
						{
							if (dr.get_FromRoom(lastPhase).Id == i.Id)
							{

								
								LocationPoint origin = (LocationPoint)i.Location;

								XYZ center = origin.Point;
								if (!neocube.IsActive)
									neocube.Activate();
								FamilyInstance cubeIns = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								cubeIns.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								cubeIns.LookupParameter("MainText").Set(dr.Symbol.LookupParameter("ADSK_Марка").AsString());
								cubeIns.setType();
								

								FamilyInstance winOtkos = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								winOtkos.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								winOtkos.LookupParameter("MainText").Set("Площадь откосов: ");
								winOtkos.LookupParameter("Area").Set(dr.LookupParameter("ADSK_Откосы_Глубина").AsDouble()
                                             * (dr.LookupParameter("VIDNAL_Высота проема").AsDouble()*2 + dr.LookupParameter("VIDNAL_Ширина проема").AsDouble()));
								winOtkos.setType("area");

								FamilyInstance winPodok = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								winPodok.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								String output = String.Format("Подоконник {0:f2}x{1:f2}", dr.LookupParameter("VIDNAL_Ширина проема").AsDouble()*FT, dr.LookupParameter("ADSK_Откосы_Глубина").AsDouble()*FT);
								winPodok.LookupParameter("MainText").Set(output);
								winPodok.setType();

								FamilyInstance winUgol = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								winUgol.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								winUgol.LookupParameter("MainText").Set("ПВХ уголок 60х60");
								winUgol.LookupParameter("dlina").Set(dr.LookupParameter("VIDNAL_Высота проема").AsDouble()*2 + dr.LookupParameter("VIDNAL_Ширина проема").AsDouble());
								winUgol.setType("len");

							}
						}
						catch (Exception)
						{

						}
					}
				}
                foreach (Element i in rooms)
                {
                    if (i.LookupParameter("ЗаменаПокрытияПола").AsInteger()==1)
                    {
						
						LocationPoint origin = (LocationPoint)i.Location;
						XYZ center = origin.Point;
						if (!neocube.IsActive)
							neocube.Activate();
						FamilyInstance cubeIns = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
						cubeIns.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
						cubeIns.LookupParameter("MainText").Set(i.LookupParameter("snosF").AsString());
						cubeIns.LookupParameter("Area").Set(i.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble());
						cubeIns.setType("area");
						if (cubeIns.LookupParameter("MainText").AsString()=="")
                        {
							doc.Delete(cubeIns.Id);

                        }
						
					}
					if (i.LookupParameter("НоваяОтделка").AsInteger() == 1)
					{

						LocationPoint origin = (LocationPoint)i.Location;
						XYZ center = origin.Point;
						if (!neocube.IsActive)
							neocube.Activate();
						FamilyInstance cubeIns = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
						cubeIns.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
						cubeIns.LookupParameter("MainText").Set("Демонтаж штукатурки");
						cubeIns.LookupParameter("Area").Set(i.LookupParameter("WallS").AsDouble());
						cubeIns.setType("area");
					}

				}
				tr.Commit();
			}
			
			
			//XYZ origin = new XYZ(0,0,0);

			/*
			using (Transaction tr = new Transaction(doc,"creating"))
			{
				tr.Start();
				
				for (int i = 0; i < a.Count; i++)
				{
					if (!neocube.IsActive)
						neocube.Activate();
					FamilyInstance cubeIns = doc.Create.NewFamilyInstance(a[i], neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
					cubeIns.LookupParameter("MainText").Set(b[i]);
				}
				
				
				tr.Commit();

			}
			*/

			//FamilySymbol elt = doors[0].Symbol;
			//FamilySymbol one=elt as FamilySymbol;
			TaskDialog msg = new TaskDialog("Info");
			msg.MainInstruction = g.ToString();
			msg.Show();
			



			return Result.Succeeded;
		}
	}
	public static class myCube 
    {
		public static void setType(this FamilyInstance famIns, String str)
        {
			if (str == "len")
			{
				famIns.LookupParameter("isLen").Set(1);
				famIns.LookupParameter("isAreo").Set(0);

			}
			else if (str == "area"|str=="Area")
			{
				famIns.LookupParameter("isLen").Set(0);
				famIns.LookupParameter("isAreo").Set(1);
			}
			else
			{
				famIns.LookupParameter("isLen").Set(0);
				famIns.LookupParameter("isAreo").Set(0);
			}
			return;
		}
		public static void setType(this FamilyInstance famIns)
		{
			famIns.LookupParameter("isLen").Set(0);
			famIns.LookupParameter("isAreo").Set(0);
			return;
		}



	}
	

}
