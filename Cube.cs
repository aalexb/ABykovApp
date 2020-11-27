using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    
    
    public class Cube
    {
        public string Pos { get; set; }
        public string Gost { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Mass { get; set; }
        public string Other { get; set; }
        public string Group { get; set; }

        public Cube(string group, string name)
        {
            Group = group;
            Name = name;
        }

        public void Create(Document doc)
        {
            FamilySymbol neocube = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(q => q.Name == "cube").First() as FamilySymbol;
            if (!neocube.IsActive)
                neocube.Activate();
            FamilyInstance unit = doc.Create.NewFamilyInstance(new XYZ(), neocube, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            
            unit.setP("g_pos", Pos);
            unit.setP("g_gost", Gost);
            unit.setP("g_name", Name);
            unit.setP("g_num", Num);
            unit.setP("g_mass", Mass);
            unit.setP("g_other", Other);
            unit.setP("g_group", Group);
        }
    }

     public static class FinishWall
    {
        public static string getP(this Element e, string name)
        {
            return e.LookupParameter(name).AsString();
        }
        public static void setP(this Element e, string name, string value)
        {
            e.LookupParameter(name).Set(value);
        }
       
    }
}
