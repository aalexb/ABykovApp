using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace WorkApp
{
    public class FinishStructuralElement
    {
        public string Type { get; set; }
        public Element refEl{get;set;}
        public string Text { get; set; }
        public double unitValue { get; set; }
        public double Value { get; set; }
        public double Height { get; set; }
        public FinishStructuralElement()
        {
            unitValue = 0;
            Value = 0;
            Type = "";
            Text = "";
        }
        public  void setType(string type, Element room)
        {
            Type = type;
            Text = type == "__Отделка : ---" ? "" : room.Document.GetElement(room.LookupParameter("ОТД_Потолок").AsElementId()).LookupParameter("АР_Состав отделки").AsString();
        }
        public static string getMultiString(List<FinishStructuralElement> l, string withNum="")
        {
            if (l.Count==1&withNum=="")
            {
                return l.First().Text;
            }
            string a = "";
            foreach (var item in l)
            {
                a += item.Text+" - ";
                a += (item.Value*Meta.FT*Meta.FT).ToString("F1");
                a += " м²\n";
            }
            return a;
        }
    }
}
