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

     public static class Meta
    {
        public static string getP(this Element e, string name)
        {
            return e.LookupParameter(name).AsString();
        }
		public static double getP(this Element e, BuiltInParameter name)
		{
			return e.get_Parameter(name).AsDouble();
		}
		public static void setP(this Element e, string name, string value)
        {
            e.LookupParameter(name).Set(value);
        }

		public static string shortLists(List<string> IN)
		{
			string Out = "";
			int first = -1;
			int current;
			int previous = -2;
			bool inQueue = false;
			int value;
			string str = "";
			string sym = " - ";
			string sep = ", ";

			for (int i = 0; i < IN.Count(); i++)
			{
				if (int.TryParse(IN[i], out value))
				{
					if (inQueue)
					{
						if (value == previous + 1 & previous == first)
						{

							previous = value;
							str = first.ToString("D") + sep + IN[i] + sep;
						}
						else if (previous + 1 == value & previous != first)
						{

							previous = value;
							str = first.ToString("D") + sym + IN[i] + sep;
						}
						else
						{
							if (previous != first)
							{
								Out += str;
								str = "";
							}
							else
							{
								Out += first.ToString("D") + sep;
								str = "";
							}

							inQueue = false;
							first = -1;
							previous = -2;
							i--;
						}
					}
					else
					{
						first = value;
						previous = value;
						inQueue = true;
						str = IN[i] + ", ";
					}

				}
				else
				{
					Out += str + IN[i] + sep;
					first = -1;
					previous = -2;
					str = "";
				}
			}
			if (first > 0)
			{
				Out += str;
			}




			return Out.Remove(Out.Length - 2);
		}

		
	}
}

public class GenericList<T>
{
	public static List<T> Flatten(List<List<T>> x)
	{
		List<T> result = new List<T>();
		foreach (List<T> el in x)
		{
            foreach (T em in el)
            {
				result.Add(em);
			}
			
			
		}
		return result;
	}
}
