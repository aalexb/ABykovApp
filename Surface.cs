using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorkApp
{
    public class Surface
    {
        public Element el {get;set;}
        public int room_id { get; set; }
        public string typeName { get; set; }
        public string sostav { get; set; }
        public double Area { get; set; }
        public string sur_func { get; set; }
        public Surface(Element el)
        { 
            this.el = el;
            typeName= el.GetType().Name;
            try
            {
                room_id = el.LookupParameter("Room_ID").AsInteger();
            }
            catch (Exception)
            {
                var msg = new Autodesk.Revit.UI.TaskDialog("Info");
                msg.MainInstruction = "Добавьте параметр Room_ID";
                msg.Show();
                throw;
            }
            Area = el.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
        }

        public void setParams()
        {

        }
    }
}
