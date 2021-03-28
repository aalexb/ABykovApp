using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WorkApp
{
    
    
    public class Cube
    {
        public string Pos { get; set; }
        public string Gost { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Mass { get; set; }
		public double dMass { get; set; }
        public string Other { get; set; }
        public string Group { get; set; }
		public double Length { get; set; }
		public double Area { get; set; }
		public bool yesno { get; set; }
		public string Units { get; set; }
		double FT = 0.3048;
		const string GROUP = "ADSK_Группирование";
		const string GOST = "ADSK_Обозначение";
		const string NAME = "ADSK_Наименование";
		const string MAT_NAME = "ADSK_Материал наименование";
		const string MASS = "ADSK_Масса";

		public Cube(string group, string name)
        {
            Group = group;
            Name = name;
			Pos = "pos";
			Gost = "Gost";
			Num = "Num";
			Mass = "Mass";
			Other = "Other";
			yesno = false;

        }

		public Cube(Rebar a)
		{
			string[] stringSeparator =new string[] { " : " };
			string[] subName = a.Name.Split(stringSeparator,StringSplitOptions.None);
			Name = "ø"+ subName[0];
			Group = a.getP("Метка основы");
			Length = a.TotalLength*FT;
			

		}
		public Cube(RebarInSystem a)
		{
			string[] stringSeparator = new string[] { " : " };
			string[] subName = a.Name.Split(stringSeparator, StringSplitOptions.None);
			Name = "ø" + subName[0];
			Group = a.getP("Метка основы");
			Length = a.TotalLength*FT;

		}
		public Cube(Railing a)
		{
			//a.Document.
			Name=a.Document.GetElement(a.GetTypeId()).getP(NAME);
			//Name = a.getP(NAME);
			Group = a.getP(GROUP);
			Length = a.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;

		}
		public Cube (Element material, Element source)
		{
			Group = source.getP(GROUP);
			Name = material.getP(MAT_NAME);
			Mass = material.Name;
			if (material.LookupParameter("Объем_Площадь").AsInteger()==0)
			{
				Area = source.GetMaterialArea(material.Id,false);
			}
			else
			{
				Area = source.GetMaterialVolume(material.Id);
			}

		}
		public Cube (Element e)
		{
			Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
			Gost=e.Document.GetElement(e.GetTypeId()).getP(GOST);
			switch (e.Category.Name)
			{
				case "Желоба":
					Group = e.getP(GROUP);
					Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
					break;
				case "Крыши":
					Group = e.getP(GROUP);
					Area = e.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble() * FT;
					break;
				
				default:
					break;
			}
			
		}
		public Cube (FamilyInstance i)
		{
			Name = i.Symbol.getP(NAME);
			Group = i.getP(GROUP);
			Pos = "";
			Gost = i.Symbol.getP(GOST);
			Length = 0;
			switch (i.Category.Name)
			{
				case "Каркас несущий":
					Length = i.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble()*FT;
					Units = " кг";
					break;
				case "Несущие колонны":
					Length = i.get_Parameter(BuiltInParameter.STEEL_ELEM_CUT_LENGTH).AsDouble()*FT;
					Units = " кг";
					break;
				default:
					if (i.Symbol.LookupParameter("АММО_Длина_КМ")!=null)
					{
						Length = i.Symbol.LookupParameter("АММО_Длина_КМ").AsDouble();
					}
					break;
			}
			Num = Length.ToString();
			dMass = i.Symbol.LookupParameter(MASS).AsDouble();
			Mass = i.Name;
			//Mass = dMass.ToString();
			Other = (Length*dMass).ToString()+Units;

		}

        public void Create(Document doc)
        {
            FamilySymbol neocube = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(q => q.Name == "cube").First() as FamilySymbol;
            if (!neocube.IsActive)
                neocube.Activate();
            FamilyInstance unit = doc.Create.NewFamilyInstance(new XYZ(), neocube, StructuralType.NonStructural);
            
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

		public static PushButton AddButton(this RibbonPanel rp,string name,Bitmap pic,string className)
		{
			string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			PushButton butt = rp.AddItem(new PushButtonData(name, name, thisAssemblyPath, "WorkApp."+className)) as PushButton;
			butt.LargeImage = Tools.GetImage(pic.GetHbitmap());
			return butt;
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

		public static Cube forgeCube(List<Cube> IN,int position)
		{

			Cube a = new Cube(IN[0].Group,IN[0].Name);
			double l=0;
			foreach (Cube b in IN)
			{
				l += b.Length;
			}
			a.Num = l.ToString("F2");
			a.Gost = IN[0].Gost;
			a.Pos = position.ToString();
			a.Mass = "-";
			a.Other = "";
			return a;
		}
	}
}

public class GenericList<T> //Шаблон
{
	public static List<T> Flatten(List<List<T>> x) //Разворачивает список
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
