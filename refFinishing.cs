using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Architecture;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class refFinishing:IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            ElementId idPhase = doc.Phases.get_Item(doc.Phases.Size - 1).Id;

            //Фильтр  по общему параметру "ADSK_Номер здания"
            List<SharedParameterElement> shParamElements = new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>()
                .ToList();
            SharedParameterElement shParam = shParamElements.Where(x => x.Name == "ADSK_Номер здания").First();
           
            //Фильтр помещений
            FilterableValueProvider providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterElementIdRule rRule = new FilterElementIdRule(providerRoom, evaluator, idPhase);
            ElementParameterFilter room_filter = new ElementParameterFilter(rRule);
            FilterableValueProvider provRoomSchool = new ParameterValueProvider(shParam.Id);
            FilterStringRuleEvaluator StrEvaluator = new FilterStringEquals();
            FilterRule rScRule = new FilterStringRule(provRoomSchool, StrEvaluator, "", false);
            ElementParameterFilter roomSc_filter = new ElementParameterFilter(rScRule);

            IList<Element> revit_rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(room_filter)
                .WherePasses(roomSc_filter)
                .ToElements();

            //Фильтр стен
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule fRule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter wall_filter = new ElementParameterFilter(fRule);

            IList<Element> revit_walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .WherePasses(wall_filter)
                .ToElements();

            //Перенос помещений и стен в объекты наших классов
            List<GhostRoom> rooms = revit_rooms.Select(x=>new GhostRoom(x)).ToList();
            List<GhostWall> walls = new List<GhostWall>();
                      
            foreach (Element w in revit_walls)
            {
                if (w.getP("Помещение")!=null &w.getP("Помещение")!="")
                {
                    walls.Add(new GhostWall(w));
                }
            }
            

            foreach (string i in rooms.Select(x => x.Floor).Distinct())
            {
                
            }
            
            




            
            
            
            
            
            
            
            
            using (Transaction tr = new Transaction(doc,"Otdelka"))
            {
                tr.Start();
                foreach (GhostRoom r in rooms)
                {
                    r.Commit();
                }
                tr.Commit();
            }
            return Result.Succeeded;
        }
    }
}
