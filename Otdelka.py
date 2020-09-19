# Загрузить стандартную библиотеку Python и библиотеку DesignScript
import sys
if IN[3]:
	import clr

clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import *

clr.AddReference('DSCoreNodes')
from DSCore import Math

getWalls=IN[0]
getRooms=IN[1]
phase=IN[2]

walls=[]
wallNum=[]
wallLevel=[]
rooms=[]
AllCeiling=[]
AllFinish=[]
AllFloor=[]
RoomNumber=[]
roomArea=[]
roomAreaByLevel=[]
Levels=[]
SecFin=[]
SecFinByLevel=[]
isNewW=[]
isNewWByLevel=[]
isNewC=[]
isNewCByLevel=[]
isNewF=[]
isNewFByLevel=[]
Plintus=[]
PlintusByLevel=[]
WallS1=[]
WallS1Text=[]
WallS2=[]
WallS2Text=[]
Levels=[]
roomByLevel=[]
roomNumByLevel=[]
wallByLevel=[]
wallNumByLevel=[]
wallSecFin=[]
wallSecFinByLevel=[]
wallArea=[]
wallAreaByLevel=[]
wallText=[]
wallTextByLevel=[]
CeilText=[]
CeilS=[]
CeilTextByLevel=[]
FloorText=[]
FloorS=[]
FloorTextByLevel=[]
MainText=[]
MainTextByLevel=[]
MainText2=[]
MainText2ByLevel=[]
notEdit=[]
notEditNum=[]
notEditF=[]
notEditFNum=[]
FinishTable=[]
FinishTableNum=[]							
SecTable=[]
FinishTableW1=[]
FinishTableW2=[]
FloorTable=[]
FloorTableNum=[]
FloorTablePlintus=[]
PlintusSum=[]
W1Sum=[]
W2Sum=[]

#Получаем помещения с указанной стадии
for i in getRooms:
	if i.GetParameterValueByName("Стадия")==phase and i.GetParameterValueByName("ADSK_Номер здания")!="СК":
		rooms.append(i)
		
#Сортируем помещения по номеру		
for i in rooms:
	RoomNumber.append(i.GetParameterValueByName("Номер"))
	
xo = zip(RoomNumber,rooms)
xs=sorted(xo, key=lambda tup: tup[0])
RoomNumber=[xo[0] for xo in xs]
rooms=[xo[1] for xo in xs]

#Сортируем по уровню
for i in rooms:
	Levels.append(i.GetParameterValueByName("Уровень"))

yo = zip(Levels,rooms)
ys=sorted(yo, key=lambda tupy: tupy[0])
Levels=[yo[0] for yo in ys]
rooms=[yo[1] for yo in ys]
RoomNumber=[]
		

#Получаем стены отделки с номерами помещений
for i in getWalls:
	if  i.GetParameterValueByName("Помещение")!="" and i.GetParameterValueByName("Стадия возведения")==phase:
		walls.append(i)
		wallNum.append(i.GetParameterValueByName("Помещение"))
		wallLevel.append(i.GetParameterValueByName("Зависимость снизу").Name)
		wallSecFin.append(i.ElementType.GetParameterValueByName("SecondFinishes"))
		wallArea.append(i.GetParameterValueByName("Площадь"))
		#wallText.append(i.GetParameterValueByName("СоставОтделкиСтен"))	
#Получаем параметры помещений
for i in rooms:
	RoomNumber.append(i.GetParameterValueByName("Номер"))
	roomArea.append(i.GetParameterValueByName("Площадь"))
	CeilText.append(i.GetParameterValueByName("ПотолокОписание"))
	FloorText.append(i.GetParameterValueByName("ПолОписание"))
	MainText.append(i.GetParameterValueByName("СтеныОписание"))
	MainText2.append(i.GetParameterValueByName("MainText2"))
	SecFin.append(i.GetParameterValueByName("SecondaryFinishes"))
	isNewW.append(i.GetParameterValueByName("НоваяОтделка"))
	isNewC.append(i.GetParameterValueByName("РемонтПотолка"))
	isNewF.append(i.GetParameterValueByName("ЗаменаПокрытияПола"))
	try:
		Plintus.append((i.GetParameterValueByName("Периметр") - i.GetParameterValueByName("ДлинаПроемов"))/1000)
	except:
		Plintus.append(0)
UniqLev=list(set(Levels))
UniqLev.sort()
#Делим помещения и стены по уровням
for lev in UniqLev:
	s=[]
	n=[]
	nw=[]
	nc=[]
	ws=[]
	#wt=[]
	w=[]
	wn=[]
	wsf=[]
	wa=[]
	wi=[]
	ct=[]
	ra=[]
	ft=[]
	sf=[]
	mt=[]
	mt2=[]
	pl=[]
	nf=[]
	
	for i,obj in enumerate(Levels):
		if obj==lev:
			s.append(rooms[i])
			n.append(RoomNumber[i])
			nw.append(isNewW[i])
			nc.append(isNewC[i])
			nf.append(isNewF[i])
			ws.append(0)
			wi.append("")
			ct.append(CeilText[i])
			ra.append(roomArea[i])
			ft.append(FloorText[i])
			sf.append(SecFin[i])
			mt.append(MainText[i])
			mt2.append(MainText2[i])
			pl.append(Plintus[i])
	roomByLevel.append(s)
	roomNumByLevel.append(n)
	roomAreaByLevel.append(ra)
	isNewWByLevel.append(nw)
	isNewCByLevel.append(nc)
	isNewFByLevel.append(nf)
	WallS1.append(ws)
	WallS2.append(ws)
	CeilS.append(ws)
	FloorS.append(ws)
	WallS1Text.append(wi)
	WallS2Text.append(wi)
	AllCeiling.append(wi)
	AllFloor.append(wi)
	CeilTextByLevel.append(ct)
	FloorTextByLevel.append(ft)
	SecFinByLevel.append(sf)
	MainTextByLevel.append(mt)
	MainText2ByLevel.append(mt2)
	PlintusByLevel.append(pl)
	
	for i,obj in enumerate(wallLevel):
		if obj==lev:
			w.append(walls[i])
			wn.append(wallNum[i])
			wsf.append(wallSecFin[i])
			wa.append(wallArea[i])
			#wt.append(wallText[i])
	wallByLevel.append(w)
	wallNumByLevel.append(wn)
	wallSecFinByLevel.append(wsf)
	wallAreaByLevel.append(wa)
	#wallTextByLevel.append(wt)	

#Задаём площади отделки помещений и указываем неизменные помещения
for lev in range(len(UniqLev)):
	for i,obj in enumerate(roomNumByLevel[lev]):
		if isNewCByLevel[lev][i]==0:
			AllCeiling[lev][i]="Без изменений"
		else:
			AllCeiling[lev][i]=CeilTextByLevel[lev][i]
			CeilS[lev][i]=roomAreaByLevel[lev][i]
		if isNewFByLevel[lev][i]==0:
			AllFloor[lev][i]="Без изменений"
		else:
			AllFloor[lev][i]=FloorTextByLevel[lev][i]
			FloorS[lev][i]=roomAreaByLevel[lev][i]
					
		if isNewWByLevel[lev][i]==0:
			WallS1Text[lev][i]="Без изменений"
			continue
		for indw,w in enumerate(wallNumByLevel[lev]):		
			if obj==w:
				if wallSecFinByLevel[lev][indw]:
					WallS2[lev][i]+=wallAreaByLevel[lev][indw]
					#if WallS2Text[lev][i]=="":
					WallS2Text[lev][i]=MainText2ByLevel[lev][i]
				else:
					#try:
					WallS1[lev][i]+=wallAreaByLevel[lev][indw]
					#except:
					#	pass
					#if WallS1Text[lev][i]=="":
					WallS1Text[lev][i]=MainTextByLevel[lev][i]							
	
#Выписываем помещения с неизменной отделкой
for r,ro in enumerate(rooms):
	if isNewW[r]==0 and isNewC[r]==0:
		notEdit.append(ro)
		notEditNum.append(RoomNumber[r])
	if isNewF[r]==0:
		notEditF.append(ro)
		notEditFNum.append(RoomNumber[r])
		

#Сортируем помещения по типу отделки стен и потолка
for isW in list(set(isNewW)):
	for isC in list(set(isNewC)):
		for k in list(set(SecFin)):
			for i in list(set(CeilText)):
				for j in list(set(MainText)):
					SimilarFinish=[]
					SimilarFinishNum=[]
					SimSecFin=[]
					SimW1=[]
					SimW2=[]
					for lev in range(len(UniqLev)):
						for r,ro in enumerate(roomByLevel[lev]):
							if (isNewWByLevel[lev][r]==0 and isNewCByLevel[lev][r]==0):
								continue
							if CeilTextByLevel[lev][r]==i and MainTextByLevel[lev][r]==j and SecFinByLevel[lev][r]==k and isNewWByLevel[lev][r]==isW and isNewCByLevel[lev][r]==isC:
								SimilarFinish.append(ro)
								SimilarFinishNum.append(roomNumByLevel[lev][r])
								SimSecFin.append(SecFinByLevel[lev][r])
								SimW1.append(WallS1[lev][r])
								SimW2.append(WallS2[lev][r])
					FinishTable.append(SimilarFinish)
					FinishTableNum.append(SimilarFinishNum)
					SecTable.append(SimSecFin)
					FinishTableW1.append(SimW1)
					FinishTableW2.append(SimW2)
FinishTable.append(notEdit)
FinishTableNum.append(notEditNum)	

#Сортируем помещения по типу отделки пола
for i in list(set(FloorText)):
	SimilarFloor=[]
	SimilarFloorNum=[]
	SimilarFloorPlint=[]
	for lev in range(len(UniqLev)):
		for r,ro in enumerate(roomByLevel[lev]):
			if FloorTextByLevel[lev][r]==i and isNewFByLevel[lev][r]!=0:
				SimilarFloor.append(ro)
				SimilarFloorNum.append(roomNumByLevel[lev][r])
				SimilarFloorPlint.append(PlintusByLevel[lev][r])
	FloorTable.append(SimilarFloor)
	FloorTableNum.append(SimilarFloorNum)
	FloorTablePlintus.append(SimilarFloorPlint)
FloorTable.append(notEditF)
FloorTableNum.append(notEditFNum)

#Общее количество плинтуса по типу отделки
for i in FloorTablePlintus:
	s=0
	for p in i:
		s+=p
	PlintusSum.append(s)
#Общая площадь по типу отделки
for indi,i in enumerate(SecTable):
	sw1=0
	sw2=0
	for ind,k in enumerate(i):
		try:
			if k:
				sw1+=FinishTableW1[indi][ind]
				sw2+=FinishTableW2[indi][ind]
		except:
			pass
	#W1Sum.append(sw1)
	try:
		W1Sum.append(sum(FinishTableW1(indi)))
	except:
		pass
	W2Sum.append(sw2)

#Передаем номера помещений с одинаковым типом отделки пола
for ind,i in enumerate(FloorTable):
	for r in i:
		r.SetParameterByName("testF",", ".join(FloorTableNum[ind]))
		try:
			r.SetParameterByName("PlintusTotal",sum(FloorTablePlintus[ind]))
		except:
			pass

#Передаем номера помещений с одинаковым типом отделки стен и потолка
for ind,i in enumerate(FinishTable):
	for indr,r in enumerate(i):
		r.SetParameterByName("test",", ".join(FinishTableNum[ind]))
		try:
			if SecTable[ind][indr]:
				
				r.SetParameterByName("SecTextTotal"," ("+Math.Round(W1Sum[ind]-W2Sum[ind],1).ToString()+"м²)\nДо высоты 1,8м("+Math.Round(W2Sum[ind],1).ToString()+"):")
				#r.SetParameterByName("WallS2Total",W2Sum[ind])
		except:
			pass
for lev in range(len(roomByLevel)):
	for ind,i in enumerate(roomByLevel[lev]):
		i.SetParameterByName("WallText",WallS1Text[lev][ind])
		i.SetParameterByName("CeilingText",AllCeiling[lev][ind])
		#try:
		#if WallS1[lev][ind]!=None:
		#	i.SetParameterByName("WallS",WallS1[lev][ind])
		#except:
		#	pass
		
for lev in range(len(roomByLevel)):
	for ind,i in enumerate(roomByLevel[lev]):
		try:
			i.SetParameterByName("CeilingSNum",CeilS[lev][ind])
		except:
			pass
		
		
"""	
for i in rooms:
	s=i.GetParameterValueByName("Номер").ToString()+ ": " +i.GetParameterValueByName("Имя")
	i.SetParameterByName("Заголовок",s)
	ListOfConcat.append(s)
"""
OUT = roomNumByLevel