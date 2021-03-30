using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WorkApp
{
	public enum myTypes
	{
		matVol,
		matArea,
		kmLen,//Общая длина
		kmNum,//Длина штуки
		armLen,//Общая длина
		armNum,//Длина штуки
		allNum,//количество
	}
	public enum myUnits
	{
		num,
		len,
		are,
		vol,
		mas
	}

	public class Cube
    {
		public int Prior { get; set; } //Приоритет в спецификации
		public myTypes mType { get; set; }
		public myUnits Units { get; set; } //Единицы измерения
		public string Pos { get; set; } //Позиция
        public string Gost { get; set; } //ГОСТ
        public string Name { get; set; } //Имя в спецификации
        public string Num { get; set; } //Количество в штуках
		public double Length { get; set; } //Длина
		public double Area { get; set; } //Площадь
		public double Volume { get; set; } //Объем
		public string Mass { get; set; } //Масса
		public double dMass { get; set; } //Масса единицы
        public string Other { get; set; } //Примечание
        public string Group { get; set; } //Группирование
		
		
		//public bool yesno { get; set; }
		public string oldUnits { get; set; }
		double FT = 0.3048;
		private const string GROUP = "ADSK_Группирование";
		private const string GOST = "ADSK_Обозначение";
		private const string NAME = "ADSK_Наименование";
		private const string MAT_NAME = "ADSK_Материал наименование";
		private const string MASS = "ADSK_Масса";

		public Cube(string group, string name)
        {
            Group = group;
            Name = name;
			Pos = "pos";
			Gost = "Gost";
			Num = "Num";
			Mass = "Mass";
			Other = "Other";
			//yesno = false;

        }

        //public Cube(Rebar e)
        //{
        //	string[] stringSeparator = new string[] { " : " };
        //	string[] subName = e.Name.Split(stringSeparator, StringSplitOptions.None);
        //	Name = "ø" + subName[0];
        //	Group = e.getP("Метка основы");
        //	Length = ((Rebar)e).TotalLength * FT;
        //}
        //public Cube(RebarInSystem a)
        //{
        //    string[] stringSeparator = new string[] { " : " };
        //    string[] subName = a.Name.Split(stringSeparator, StringSplitOptions.None);
        //    Name = "ø" + subName[0];
        //    Group = a.getP("Метка основы");
        //    Length = a.TotalLength * FT;

        //}
  //      public Cube(Railing e)
		//{
		//	Name=e.Document.GetElement(e.GetTypeId()).getP(NAME);
		//	Group = e.getP(GROUP);
		//	Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
		//}
		public Cube (Element material, Element source)
		{
			Group = source.getP(GROUP);
			Name = material.getP(MAT_NAME);
			Mass = material.Name;
			if (material.LookupParameter("Объем_Площадь").AsInteger()==0)
			{
				Area = source.GetMaterialArea(material.Id,false);
				mType = myTypes.matArea;
				//Units=myUnits.are;
				
			}
			else
			{
				Area = source.GetMaterialVolume(material.Id);
				mType = myTypes.matVol;
				//Units = myUnits.vol;
				
			}
			//Other = Meta.setEdizm(Units);

		}
		
		public Cube (Element e)
		{
            switch (e.Category.Name)
            {
				case "Пластины":
					//Units=myUnits
					Name =
						"Пластина "
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_PLATE_LENGTH).AsDouble() * 1000 * FT).ToString("F0") 
						+ "x"
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_PLATE_WIDTH).AsDouble()*1000*FT).ToString("F0") 
						+ "x" 
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_PLATE_THICKNESS).AsDouble() * 1000 * FT).ToString("F0");
					Group = e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
					break;
				case "Ограждение":
					Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					Group = e.getP(GROUP);
					Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
					break;
				case "Анкеры":
					string standardName = e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_STANDARD).AsString();
					ElementId gPar = GlobalParametersManager.FindByName(e.Document, standardName);
					string ankerName = gPar==ElementId.InvalidElementId?standardName:
						((StringParameterValue)
						((GlobalParameter)e.Document.GetElement(gPar))
						.GetValue()).Value;
					Name =
						ankerName
						+ " Ø"
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_DIAMETER).AsString())
						+ "x"
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_LENGTH).AsString()).Split(',')[0];
					Group = e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
					break;
				case "Несущая арматура":
					string[] stringSeparator = new string[] { " : " };
					string[] subName = e.Name.Split(stringSeparator, StringSplitOptions.None);
					Name = "ø" + subName[0];
					Group = e.getP("Метка основы");
                    try
                    {
						Length = ((Rebar)e).TotalLength * FT;
					}
                    catch (Exception)
                    {

						Length = ((RebarInSystem)e).TotalLength * FT;
					}
					
					break;
				case "Желоба":
					Group = e.getP(GROUP);
					Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
					Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					Gost = e.Document.GetElement(e.GetTypeId()).getP(GOST);
					break;
				case "Крыши":
					Group = e.getP(GROUP);
					Area = e.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble() * FT;
					Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					Gost = e.Document.GetElement(e.GetTypeId()).getP(GOST);
					break;
				default:
					Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					Group = e.getP(GROUP);
					Pos = "";
					Gost = e.Document.GetElement(e.GetTypeId()).getP(GOST);
					Length = 0;
					switch (e.Category.Name)
					{
						case "Каркас несущий":
							Length = e.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble() * FT;
							Units = myUnits.mas;
							oldUnits = " кг";
							break;
						case "Несущие колонны":
							Length = e.get_Parameter(BuiltInParameter.STEEL_ELEM_CUT_LENGTH).AsDouble() * FT;
							oldUnits = " кг";
							break;
						default:
							if (e.Document.GetElement(e.GetTypeId()).LookupParameter("АММО_Длина_КМ") != null)
							{
								Length = e.Document.GetElement(e.GetTypeId()).LookupParameter("АММО_Длина_КМ").AsDouble() * FT;
							}
							break;
					}
					Num = Length.ToString();
					dMass = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble();
					Mass = e.Name;
					//Mass = dMass.ToString();
					Other = (Length * dMass).ToString() + oldUnits;
					//Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					//Gost = e.Document.GetElement(e.GetTypeId()).getP(GOST);
					break;
            }

			
		}
		//public Cube (FamilyInstance e)
		//{
		//	Name = e.Symbol.getP(NAME);
		//	Group = e.getP(GROUP);
		//	Pos = "";
		//	Gost = e.Symbol.getP(GOST);
		//	Length = 0;
		//	switch (e.Category.Name)
		//	{
		//		case "Каркас несущий":
		//			Length = e.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble()*FT;
		//			Units = " кг";
		//			break;
		//		case "Несущие колонны":
		//			Length = e.get_Parameter(BuiltInParameter.STEEL_ELEM_CUT_LENGTH).AsDouble()*FT;
		//			Units = " кг";
		//			break;
		//		default:
		//			if (e.Symbol.LookupParameter("АММО_Длина_КМ")!=null)
		//			{
		//				Length = e.Symbol.LookupParameter("АММО_Длина_КМ").AsDouble()*FT;
		//			}
		//			break;
		//	}
		//	Num = Length.ToString();
		//	dMass = e.Symbol.LookupParameter(MASS).AsDouble();
		//	Mass = e.Name;
		//	//Mass = dMass.ToString();
		//	Other = (Length*dMass).ToString()+Units;

		//}

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
		public static string setEdizm(myTypes wow) //Устанавливает единицы измерения
		{

			//switch (wow)
			//{
			//	case myUnits.num:
			//		s = "шт";
			//		break;
			//	case myUnits.len:
			//		s = "м.п.";
			//		break;
			//	case myUnits.are:
			//		s = "м²";
			//		break;
			//	case myUnits.vol:
			//		s = "м³";
			//		break;
			//	case myUnits.mas:
			//		s = "кг";
			//		break;
			//	default:
			//		s = "";
			//		break;
			//}
			string s = "";
			switch (wow)
			{
				case myTypes.matVol:
					s = "м³";
					break;
				case myTypes.matArea:
					s = "м²";
					break;
				case myTypes.kmLen:
					s = "кг";
					break;
				case myTypes.kmNum:
					s = "кг";
					break;
				case myTypes.armLen:
					s = "кг";
					break;
				case myTypes.armNum:
					s = "кг";
					break;
				case myTypes.allNum:
					break;
				default:
					break;
			}
			return s;
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
			
			switch (IN[0].mType)
			{
				case myTypes.matVol:
					break;
				case myTypes.matArea:
					break;
				case myTypes.kmLen:
					break;
				case myTypes.kmNum:
					break;
				case myTypes.armLen:
					break;
				case myTypes.armNum:
					break;
				case myTypes.allNum:
					break;
				default:
					break;
			}
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
			(a.Pos, a.Mass, a.Other) = Meta.chtoto(a);
			return a;
		}
		public static (string,string,string) chtoto(Cube uno)
		{
			return ("", "", "");
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
