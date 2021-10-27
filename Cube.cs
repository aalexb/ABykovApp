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

	public class ElementExt
    {
		public Element refElement { get; set; }
		public string textDOWN { get; set; }
		public ElementExt(Element e)
        {
			refElement = e;
        }
    }


	public class Cube
    {
		public ElementExt refElement { get; set; }
		public static List<Cube> allCubes = new List<Cube>();
		public List<ElementExt> posElements = new List<ElementExt>();

		public string out_Pos { get; set; } //Позиция
		public string out_Snos { get; set; } //Стадия сноса
        public string out_Gost { get; set; } //ГОСТ
        public string out_Name { get; set; } //Имя в спецификации
        public string out_Kol_vo { get; set; } //Количество в штуках
		public string out_Mass { get; set; } //Масса
		public string out_Other { get; set; } //Примечание
		public string out_Group { get; set; } //Группирование
		//==================================
		public int Prior { get; set; } //Приоритет в спецификации
		public myTypes mType { get; set; } //Тип в спецификации
		public myUnits Units { get; set; } //Единицы измерения
		public double Length { get; set; } //Длина единицы
		public double TotalLength { get; set; } //Длина общая
		public double Area { get; set; } //Площадь
		public double TotalArea { get; set; } //Площадь
		public double Volume { get; set; } //Объем
		public double TotalVolume { get; set; } //Объем
		public double Massa { get; set; } //Масса единицы
		public double TotalMassa { get; set; } //Масса общая
		public int Quantity { get; set; }//Количество
		public string oldUnits { get; set; }
		public string typeName { get; set; }
		public string textUP { get; set; }
		public string textDOWN { get; set; }
		//==================================
		double FT = 0.3048;
		private const string GROUP = "ADSK_Группирование";
		private const string GOST = "ADSK_Обозначение";
		private const string MAT_GOST = "ADSK_Материал обозначение";
		private const string NAME = "ADSK_Наименование";
		private const string MAT_NAME = "ADSK_Материал наименование";
		private const string MASS = "ADSK_Масса";
		private const string STEEL_MARK = "КР_МаркаСтали";

		public Cube(string group, string name)
        {
            out_Group = group;
            out_Name = name;
			out_Pos = "pos";
			out_Gost = "Gost";
			out_Kol_vo = "Num";
			out_Mass = "Mass";
			out_Other = "Other";
        }

		public Cube (Element material, Element source)
		{
			out_Group = source.getP(GROUP);
            if (material.getP(MAT_NAME)=="")
            {
				out_Name = "Назв: " + material.Name;

			}
            else
            {
				out_Name = material.getP(MAT_NAME);
			}
			if (material.getP(MAT_GOST) == "")
			{
				out_Gost = "Обозн: " + material.Name;

			}
			else
			{
				out_Gost = material.getP(MAT_GOST);
			}

			
			out_Mass = material.Name;
			out_Snos = source.get_Parameter(BuiltInParameter.PHASE_DEMOLISHED).AsValueString();
			if (material.LookupParameter("Объем_Площадь").AsInteger()==0)
			{
				Area = source.GetMaterialArea(material.Id,false)*FT*FT;
				mType = myTypes.matArea;
			}
			else
			{
				Volume = source.GetMaterialVolume(material.Id) * FT * FT * FT;
				mType = myTypes.matVol;
			}
		}
		
		public Cube (Element e)
		{
			refElement = new ElementExt(e);
			out_Snos = e.get_Parameter(BuiltInParameter.PHASE_DEMOLISHED).AsValueString();
			typeName = e.Name;
			//GOST
			switch (e.Category.Name)
			{
				case "Пластины":
					out_Gost= "ГОСТ 19903-2015";
					break;
				case "Анкеры":
					string standardName = e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_STANDARD).AsString();
					ElementId gPar = GlobalParametersManager.FindByName(e.Document, standardName);
					out_Gost = gPar == ElementId.InvalidElementId ? "" :
						((StringParameterValue)
						((GlobalParameter)e.Document.GetElement(gPar))
						.GetValue()).Value.Split(':')[1];
					break;
				default:
					out_Gost = e.Document.GetElement(e.GetTypeId()).getP(GOST);
					break;
			}
			//GROUP
			switch (e.Category.Name)
			{
				case "Несущая арматура":
                    //ebar r = e as Rebar;
					Element osn = (e as Rebar) != null? e.Document.GetElement( (e as Rebar).GetHostId()): e.Document.GetElement((e as RebarInSystem).GetHostId());
					out_Group = osn.getP(GROUP);
					break;
				default:
					out_Group = e.getP(GROUP);
					break;
			}

			switch (e.Category.Name)
            {
				case "Пластины":
					mType = myTypes.plastini;
					double plLength = e.get_Parameter(BuiltInParameter.STEEL_ELEM_PLATE_LENGTH).AsDouble() * 1000*FT;
					double plWidth = e.get_Parameter (BuiltInParameter.STEEL_ELEM_PLATE_WIDTH).AsDouble() * 1000 * FT;
					double plThickness = e.get_Parameter(BuiltInParameter.STEEL_ELEM_PLATE_THICKNESS).AsDouble() * 1000 * FT;
					Massa = plLength * plWidth * plThickness * 7850 / 1000000000;
					Length = 1;
					out_Name =
						"Пластина "
						+ plLength.ToString("F0") 
						+ "x"
						+ plWidth.ToString("F0")
						+ "x" 
						+ plThickness.ToString("F0");
					
					break;

				case "Ограждение":
					mType = myTypes.commonLength;
					out_Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
					break;

				case "Анкеры":
					mType = myTypes.allNum;
					string standardName = e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_STANDARD).AsString();
					ElementId gPar = GlobalParametersManager.FindByName(e.Document, standardName);
					
					string ankerName = gPar==ElementId.InvalidElementId?standardName:
						((StringParameterValue)
						((GlobalParameter)e.Document.GetElement(gPar))
						.GetValue()).Value.Split(':')[0];
					out_Name =
						ankerName
						+ " Ø"
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_DIAMETER).AsString())
						+ "x"
						+ (e.get_Parameter(BuiltInParameter.STEEL_ELEM_ANCHOR_LENGTH).AsString()).Split(',')[0];
					Quantity = e.get_Parameter(BuiltInParameter.STEEL_ELEM_PATTERN_NUMBER_X).AsInteger() * e.get_Parameter(BuiltInParameter.STEEL_ELEM_PATTERN_NUMBER_Y).AsInteger();
					break;
				case "Несущая арматура":
                    
					
					string[] stringSeparator = new string[] { " : " };
					string[] subName = e.Name.Split(stringSeparator, StringSplitOptions.None);
					out_Name = "ø" + subName[0];
					if (e.LookupParameter("шт/м/м2/м3 экз").AsInteger()==1)
                    {
						mType = myTypes.armNum;
						Length = ((Rebar)e).get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble()*1000 * FT;
						refElement.textDOWN = (Math.Round(Length / 10)*10).ToString()+"мм";
						Quantity = ((Rebar)e).Quantity;
						Massa = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble()*Length/1000;
					}
                    else
                    {
						mType = myTypes.armLen;
						Massa = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble();
						try
						{
							Length = ((Rebar)e).TotalLength * FT;
						}
						catch (Exception)
						{

							Length = ((RebarInSystem)e).TotalLength * FT;
						}
					}
                    if (refElement.textDOWN!=null)
                    {
						refElement.textDOWN += ", ";
					}
					
					try
					{
                        
						refElement.textDOWN += "шаг " + ((e as Rebar).MaxSpacing * FT * 1000).ToString("F0");
					}
					catch (Exception)
					{


					}
					break;
				case "Желоба":
					mType = myTypes.commonLength;
					Length = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * FT;
					out_Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					break;
				case "Крыши":
					mType = myTypes.matArea;
					Area = e.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble() * FT*FT;
					out_Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					break;
				default:
					out_Name = e.Document.GetElement(e.GetTypeId()).getP(NAME);
					textUP = out_Name;
					if (e.Document.GetElement(e.GetTypeId()).getP(STEEL_MARK) != null)
					{
						out_Name += " " + e.Document.GetElement(e.GetTypeId()).getP(STEEL_MARK);

					}
					Length = 0;
					switch (e.Category.Name)
					{
						case "Каркас несущий":
							mType = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble()!=0?myTypes.armLen:myTypes.commonLength;
							Length = e.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble() * FT;
							Units = myUnits.mas;
							
							break;
						case "Несущие колонны":
							mType = myTypes.armLen;
							Length = e.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble() * FT;
							

							break;
						default:

							mType = e.Document.GetElement(e.GetTypeId()).LookupParameter("ЕслиЛинейный").AsInteger()==1? myTypes.commonLength:myTypes.allNum;
							if (e.LookupParameter("АММО_Длина_КМ") != null)
							{
								Length = e.LookupParameter("АММО_Длина_КМ").AsDouble() * FT;
							}
							break;
					}
					Massa = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble();
					break;
            }
		}
	

        public void Create(Document doc)
        {
            FamilySymbol neocube = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(q => q.Name == "cube").First() as FamilySymbol;
            if (!neocube.IsActive)
                neocube.Activate();
            FamilyInstance unit = doc.Create.NewFamilyInstance(new XYZ(), neocube, StructuralType.NonStructural);
            
            unit.setP("g_pos", out_Pos);
            unit.setP("g_gost", out_Gost);
            unit.setP("g_name", out_Name);
            unit.setP("g_num", out_Kol_vo);
            unit.setP("g_mass", out_Mass);
            unit.setP("g_other", out_Other);
            unit.setP("g_group", out_Group);
			unit.setP("g_snos", out_Snos);
			//unit.LookupParameter("g_sort").Set(this.Prior).ToString();
			unit.setP("g_sort", Prior.ToString());
        }


    }


	public class SpecForm
    {
		public static List<SpecForm> allSpecForm = new List<SpecForm>();
		ElementId eID { get; set; }
		Element refElement { get; set; }
		public string grouping;
		public List<Cube> FillSpec { get; set; }

		public SpecForm(ElementId eid, Document doc)
        {
			eID = eid;
			refElement = doc.GetElement(eid);
			grouping = refElement.getP("Группа");

			allSpecForm.Add(this);
			FillSpec = new List<Cube>();
        }
    }
     
}


