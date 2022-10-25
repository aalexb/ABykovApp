//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace WorkApp
{
    class doorObj
    {
        public static List<doorObj> AllDoorObj=new List<doorObj>();
        public ElementId fromRoom { get; set; } = null;
        public ElementId toRoom { get; set; } = null;
        public double width { get; set; }
        Element refEl { get; set; }
        public doorObj(Element e,Phase phase)
        {
            refEl = e;

            width= e.Document.GetElement(e.GetTypeId()).get_Parameter(BuiltInParameter.CASEWORK_WIDTH).AsDouble();
            try
            {
                fromRoom = (e as FamilyInstance).get_FromRoom(phase).Id;
            }
            catch (Exception)
            {
            }
            
            try
            {
                toRoom = (e as FamilyInstance).get_ToRoom(phase).Id;
            }
            catch (Exception)
            {
            }
            
            AllDoorObj.Add(this);
        }
    }
}
