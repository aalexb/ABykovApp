using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
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
            unit.LookupParameter("g_pos").Set(Pos);
            unit.LookupParameter("g_gost").Set(Gost);
            unit.LookupParameter("g_name").Set(Name);
            unit.LookupParameter("g_num").Set(Num);
            unit.LookupParameter("g_mass").Set(Mass);
            unit.LookupParameter("g_other").Set(Other);
            unit.LookupParameter("g_group").Set(Group);



        }
    }
}
