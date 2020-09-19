# Загрузить стандартную библиотеку Python и библиотеку DesignScript
import sys
if IN[3]:
	import clr

clr.AddReference('ProtoGeometry')
import Autodesk
from Autodesk.DesignScript.Geometry import *

clr.AddReference("RevitServices")
import RevitServices
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

clr.AddReference("RevitAPI")

from Autodesk.Revit.DB import *

clr.AddReference("RevitNodes")
from Revit.Elements import*
#clr.ImportExtensions(Revit.Elements)



clr.AddReference('DSCoreNodes')
from DSCore import Math

# Введенные в этом узле данные сохраняется в виде списка в переменных IN.
dataEnteringNode = IN

# Разместите код под этой строкой

getRooms=IN[1]
phase=IN[2]
rooms=[]	
# Назначьте вывод переменной OUT.
for i in getRooms:
	if i.GetParameterValueByName("Стадия")==phase:
		rooms.append(i)
		


Levels=[]
for i in rooms:
	if Levels.Contains(i.GetParameterValueByName("Уровень"))==False:
		Levels.append(i.GetParameterValueByName("Уровень"))
Levels.sort()
roomByLevel=[]
for lev in Levels:
	s=[]
	for i in rooms:
		if i.GetParameterValueByName("Уровень")==lev:
			s.append(i)
	s.sort()
	roomByLevel.append(s)
roomsEl=[]
for i in rooms:
	roomsEl.append(UnwrapElement(i))
AllDoor=0
doc=DocumentManager.Instance.CurrentDBDocument
idPhase=roomsEl[0].GetParameters("Стадия")[0].AsElementId()
collector=FilteredElementCollector(doc)
provider=ParameterValueProvider(ElementId(int(BuiltInParameter.PHASE_CREATED)))
evaluator=FilterNumericEquals()
rule=FilterElementIdRule(provider,evaluator,idPhase)
parafilter=ElementParameterFilter(rule)
Doors = collector.OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().WherePasses(parafilter).ToElements()




for i in roomsEl:
	DoorNum=0
	DoorLen=0
	Phase=doc.GetElement(i.GetParameters("Стадия")[0].AsElementId())
	
	for dr in Doors:
		#AllDoor=dr.GetParameters("Ширина")
		try:
			if dr.FromRoom[Phase].Id==i.Id or dr.ToRoom[Phase].Id==i.Id:
				bb=i.get_BoundingBox(None)
				center=(bb.Max-bb.Min)/2
		
				newCube=FamilyInstance.ByPoint(FamilyType.ByName("cube"),Autodesk.DesignScript.Geometry.Point.ByCoordinates(center.X,center.Y,center.Z))
				#try:
				if newCube.SetParameterByName("MainText",dr.LookupParameter("ADSK_Марка").AsValueString())!=type(String):
					newCube.SetParameterByName("MainText","error")
				#except:
				#	pass
			#DoorNum+=1
				#try:
				#DoorLen=DoorLen+dr.GetParameters("Ширина")[0].AsDouble()
				
				#except:
				#	pass
				
		except:
			pass
		
	#i.LookupParameter("КолДверей").Set(DoorNum)
	#i.LookupParameter("ДлинаПроемов").Set(DoorLen)
	

#a=Point.Create(XYZ.Zero)
#point1=Autodesk.DesignScript.Geometry.Point.ByCoordinates(0,0,0)
#point2=roomByLevel[0].GetLocation()
#FamilyInstance.ByPoint(FamilyType.ByName("cube"),point2)
OUT = Doors