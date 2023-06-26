using Autodesk.Revit.DB;
using DecoRe;

namespace WorkApp
{
    public class FillContext
    {
        Context C;
        RevitContext RC;
        FilterNumericRuleEvaluator evaluator;
        public FillContext(Context context,RevitContext revitContext) {
            C = context;
            RC = revitContext;
            evaluator = new FilterNumericEquals();
        }
        public void collectRooms()
        {
            var providerRoom = new ParameterValueProvider(new ElementId((int)BuiltInParameter.ROOM_PHASE_ID));
            var rule = new FilterElementIdRule(providerRoom, evaluator, RC.selected_phase.Id);
            var filter = new ElementParameterFilter(rule);
            var rooms = new FilteredElementCollector(RC.doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .WherePasses(filter)
                .ToElements();

            foreach (var room in rooms)
            {
                var r = GetRoom(room);
                C.rooms.Add(r);
            }
               
        }
        Room GetRoom(Element e)
        {
            return new Room
            {
                id = e.UniqueId,
                group = e.LookupParameter(C.par_dict[par_name.GROUP]).AsString(),
                note = e.LookupParameter(C.par_dict[par_name.NOTE]).AsString(),
                level = e.LevelId.IntegerValue
            };
        }
        DecoRe.Surface GetSurface(Element e)
        {
            var s = new DecoRe.Surface();



            return s;
        }

    }
}
