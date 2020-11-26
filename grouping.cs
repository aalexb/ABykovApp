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
    class grouping : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;
			double FT = 0.3048;

			FamilySymbol neocube = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(q => q.Name == "cube").First() as FamilySymbol;

			//FilterStringRuleEvaluator cubeEval = new FilterStringEquals();
			//FilterableValueProvider cubeProv = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME));
			//FilterStringRule cubeRule = new FilterStringRule(cubeProv, cubeEval, "cube", false);
			ElementParameterFilter cubeFilter = new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME)), new FilterStringEquals(), "cube", false));
			List<FamilyInstance> existCubes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().WherePasses(cubeFilter).Cast<FamilyInstance>().ToList();

			List<FamilyInstance> karkas = new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_StructuralFraming)
				.WhereElementIsNotElementType()
				.Cast<FamilyInstance>()
				.ToList();
			List<FamilyInstance> myKar = new List<FamilyInstance>();
			foreach (FamilyInstance i in karkas)
			{
				if (i.StructuralType.ToString() != "NonStructural")
				{
					myKar.Add(i);
				}
			}

			List<FamilyInstance> kolon = new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_StructuralColumns)
				.WhereElementIsNotElementType()
				.Cast<FamilyInstance>()
				.ToList();
		
			

			using (Transaction tr = new Transaction(doc, "creating"))
			{
				tr.Start();

				foreach (FamilyInstance i in existCubes)
				{
					doc.Delete(i.Id);
				}

				/*
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
											 * (dr.LookupParameter("VIDNAL_Высота проема").AsDouble() * 2 + dr.LookupParameter("VIDNAL_Ширина проема").AsDouble()));
								winOtkos.setType("area");

								FamilyInstance winPodok = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								winPodok.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								String output = String.Format("Подоконник {0:f2}x{1:f2}", dr.LookupParameter("VIDNAL_Ширина проема").AsDouble() * FT, dr.LookupParameter("ADSK_Откосы_Глубина").AsDouble() * FT);
								winPodok.LookupParameter("MainText").Set(output);
								winPodok.setType();

								FamilyInstance winUgol = doc.Create.NewFamilyInstance(center, neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
								winUgol.LookupParameter("RoomNum").Set(i.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
								winUgol.LookupParameter("MainText").Set("ПВХ уголок 60х60");
								winUgol.LookupParameter("dlina").Set(dr.LookupParameter("VIDNAL_Высота проема").AsDouble() * 2 + dr.LookupParameter("VIDNAL_Ширина проема").AsDouble());
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
					if (i.LookupParameter("ЗаменаПокрытияПола").AsInteger() == 1)
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
						if (cubeIns.LookupParameter("MainText").AsString() == "")
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
				*/
				tr.Commit();
			}

			return Result.Succeeded;
        }
    }
}
