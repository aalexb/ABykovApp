using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp
{
	public enum FinishClasses
    {
		wall,
		floor,
		ceiling,
    }
	public enum myTypes
	{
		matVol,
		matArea,
		kmLen,//Общая длина
		kmNum,//Длина штуки
		armLen,//Общая длина
		armNum,//Длина штуки
		allNum,//количество
		plastini,//для пластин пример
		totalArea,//общая площадь
		commonLength,//чтото в м.п.
	}
	public enum myUnits
	{
		num,
		len,
		are,
		vol,
		mas
	}
	public static class Tools
    {
        public static Workset GetActiveWorkset(Document doc)
        {
            WorksetTable table = doc.GetWorksetTable();
            WorksetId activeId = table.GetActiveWorksetId();
            Workset workset = table.GetWorkset(activeId);
            return workset;
        }

        private static FilteredElementCollector WorksetElements(Document doc, Workset workset)
        {
            FilteredElementCollector elementCollector = new FilteredElementCollector(doc).OfClass(typeof(Family));
            ElementWorksetFilter elementWorksetFilter = new ElementWorksetFilter(workset.Id, false);
            return elementCollector.WherePasses(elementWorksetFilter);
        }
        private static IList<Workset> GetAllWorksets(Document doc)
        {
            string message = string.Empty;
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.FamilyWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            //if (worksets.Count == 0)
            //    TaskDialog.Show("Worksets", " No Worksets in project");
            foreach (Workset workset in worksets)
            {
                message += "Workset : " + workset.Name;
                message += "\nUnique Id : " + workset.UniqueId;
                message += "\nOwner : " + workset.Owner;
                message += "\nKind : " + workset.Kind;
                message += "\nIs default : " + workset.IsDefaultWorkset;
                message += "\nIs editable : " + workset.IsEditable;
                message += "\nIs open : " + workset.IsOpen;
                message += "\nIs visible by default : " + workset.IsVisibleByDefault + "\n";
                message += "\n\n";
                //TaskDialog.Show("GetWorksetsInfo", message);
            }
            return worksets;
        }

        private static string GetFamiliesElements(FilteredElementCollector elementCollector)
        {
            string temp = string.Empty;

            foreach (Element element in elementCollector)
            {
                if (!(element.Name.Contains("Standart") ||
                      element.Name.Contains("Mullion") ||
                      element.Name.Contains("Tag")))
                {
                    Family family = element as Family;
                    temp += element.Name;

                    ISet<ElementId> familySymbolId = family.GetFamilySymbolIds();
                    foreach (ElementId id in familySymbolId)
                    {
                        var symbol = family.Document.GetElement(id) as FamilySymbol;
                        if (symbol != null) temp += "#" + symbol.Name;
                    }
                    temp += "\n";
                }
            }
            return temp;
        }

        //public static void CollectFamilyData(Autodesk.Revit.DB.Document doc)
        //{
        //    Properties.
        //    Properties.Settings.Default.CollectedData = string.Empty;

        //    if (GetAllWorksets(doc).Count == 0)
        //    {
        //        FilteredElementCollector elementCollector = new FilteredElementCollector(doc);
        //        elementCollector = elementCollector.OfClass(typeof(Family));
        //        Properties.Settings.Default.CollectedData = GetFamiliesElements(elementCollector);
        //    }
        //    else
        //    {
        //        string temp = string.Empty;
        //        foreach (Workset workset in GetAllWorksets(doc))
        //        {
        //            if (workset.IsEditable)
        //                temp += GetFamiliesElements(WorksetElements(doc, workset));
        //        }
        //        Properties.Settings.Default.CollectedData = temp;
        //    }
        //}

        public static void CreateImages(Autodesk.Revit.DB.Document doc)
        {
            var collector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance));
            foreach (FamilyInstance fi in collector)
            {
                ElementId typeId = fi.GetTypeId();
                ElementType type = doc.GetElement(typeId) as ElementType;

                string TempImgFolder = Path.GetTempPath() + "FamilyBrowser\\";
                if (!Directory.Exists(TempImgFolder))
                {
                    Directory.CreateDirectory(TempImgFolder);
                }

                string filename = Path.Combine(TempImgFolder + type.Name + ".bmp");

                if (!File.Exists(filename))
                {
                    System.Drawing.Size imgSize = new System.Drawing.Size(200, 200);
                    Bitmap image = type.GetPreviewImage(imgSize);
                    //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ConvertBitmapToBitmapSource(image)));
                    // encoder.QualityLevel = 25;
                    FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);

                    encoder.Save(file);
                    file.Close();
                }
            }
        }

        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bmp)
        {
            return System.Windows.Interop.Imaging
                .CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        public static BitmapSource GetImage(IntPtr bm)
        {
            BitmapSource bmSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bm,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return bmSource;
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

	public static class Meta
	{
		public static double FT = 0.3048;
		public static List<Element> getFilter<T>(this Document doc)
		{
			//new FilteredElementCollector(doc)
			return new FilteredElementCollector(doc)
				.OfClass(typeof(T))
				.ToElements()
				.ToList();
		}
		public static void setParamInForeach(this Document doc, IEnumerable<Element> mas)
		{
			foreach (var item in mas)
			{
				try
				{
					item.LookupParameter("СП_Лист").Set(doc.GetElement(item.OwnerViewId).Name);
				}
				catch (Exception) { }

			}
		}
		public static List<Element> getFilter(this Document doc,BuiltInCategory cat, string s) 
		{
			//new FilteredElementCollector(doc)
			return new FilteredElementCollector(doc)
				.OfCategory(cat)
				.WhereElementIsNotElementType()
				.Where(x=>((FamilyInstance)x).StructuralType.ToString()!=s)
				.Cast<Element>()
				.ToList();
		}
		public static List<Element> getFilter(this Document doc, BuiltInCategory cat)
		{
			//new FilteredElementCollector(doc)
			return new FilteredElementCollector(doc)
				.OfCategory(cat)
				.WhereElementIsNotElementType()
				.Cast<Element>()
				.ToList();
		}
		public static List<wtf.Product> addFilterResult<T, V>(this Document doc) where V : wtf.Creator, new()
		{
			//var ook = new V();
			return doc.getFilter<T>().Select(x => new V().Create(x)).ToList();
		}
		public static List<wtf.Product> addFilterResult<V>(this Document doc,BuiltInCategory b,string s)where V:wtf.Creator, new()
		{
			return doc.getFilter(b,s).Select(x => new V().Create(x)).ToList();
		}
		public static List<wtf.Product> addFilterResult<V>(this Document doc, BuiltInCategory b) where V : wtf.Creator, new()
		{
			return doc.getFilter(b).Select(x => new V().Create(x)).ToList();
		}
		public static List<wtf.Product> addFilterResult(this Document doc, BuiltInCategory b)
		{
			List<Element> az = doc.getFilter(b);
			List<wtf.Product> mat = null;
            foreach (Element f in az)
            {
                foreach (ElementId m in f.GetMaterialIds(false))
                {
					mat.Add(new wtf.MaterialCreator().Create(doc.GetElement(m)));
                }
            }

			return mat;
		}

		public static void ororor(this List<Cube> a)
		{

		}
		public static string getP(this Element e, string name)
		{
            if (e.LookupParameter(name)==null)
            {
				return null;
            }
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
		public static void setP(this Element e, string name, int value)
		{
			e.LookupParameter(name).Set(value);
		}
		public static PushButton AddButton2(this RibbonPanel rp, string name, Bitmap pic, string className)
		{
			string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			PushButton butt = rp.AddItem(new PushButtonData(name, name, thisAssemblyPath, className)) as PushButton;
			butt.LargeImage = Tools.GetImage(pic.GetHbitmap());
			return butt;
		}
		public static PushButton AddButton(this RibbonPanel rp, string name, Bitmap pic, string className)
		{
			string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			PushButton butt = rp.AddItem(new PushButtonData(name, name, thisAssemblyPath, "WorkApp." + className)) as PushButton;
			butt.LargeImage = Tools.GetImage(pic.GetHbitmap());
			return butt;
		}
		public static PushButton AddButton(this RibbonPanel rp, string name,string Comment, Bitmap pic, string className)
		{
			string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			PushButton butt = rp.AddItem(new PushButtonData(name, Comment, thisAssemblyPath, "WorkApp." + className)) as PushButton;
			butt.LargeImage = Tools.GetImage(pic.GetHbitmap());
			return butt;
		}
		//public static string 
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
		public static (wtf.Product, int) forgeProduct(List<wtf.Product> IN, int position)
        {
			int addpos = 1;
			string grName = IN[0].grouping == null ? "Без группы" : IN[0].grouping;
			wtf.Product nova = new wtf.RebarCreator().Create(null);
            foreach (var c in IN)
            {
                try
                {
					nova.posElements.Add(c.refElt);
                }
                catch{}
            }
            try
            {
				nova.textUP = IN[0].textUP;
			}
            catch (Exception)
            {
            }
			
			nova.mType = IN[0].mType;
			nova.Length = IN[0].Length;
			return (null,0);
        }
		public static (Cube,int) forgeCube(List<Cube> IN, int position)
		{
			int addpos = 1;
            if (IN[0].DontSetPos)
            {
				addpos = 0;
            }
			string grName = IN[0].out_Group == null ?"Без группы": IN[0].out_Group;
			Cube nova = new Cube(grName, IN[0].out_Name);
			foreach (Cube c in IN)
			{
                try
                {
					//nova.
					nova.posElements.Add(c.refElement);
				}
                catch 
                {
                }
				
			}
			nova.textUP = IN[0].textUP;
			nova.mType = IN[0].mType;
			nova.Length = IN[0].Length;
			//nova
			//nova.Quantity=IN.Where(x=>x.Quantity)
            foreach (Cube i in IN)
            {
                if (i.Quantity<=1)
                {
					nova.Quantity += 1;
                }
                else
                {
					nova.Quantity += i.Quantity;
                }
            }
			if (IN[0].mType == myTypes.matArea | IN[0].mType == myTypes.matVol)
			{
				addpos = 0;
				nova.out_Pos = "";
				nova.Prior = 1000;
			}
            else
            {
				nova.out_Pos = position.ToString();
				nova.Prior = 10;
			}
			if (IN[0].mType == myTypes.totalArea)
			{
				addpos = 0;
				nova.out_Pos = IN[0].typeName;
				nova.Prior = 100;
			}
			
			//nova.Quantity = IN.Count;
			nova.Massa = IN[0].Massa;
			
			
			foreach (Cube b in IN)
			{
				nova.TotalLength += b.Length;
				nova.TotalMassa +=b.Length>0?b.Length*b.Massa: b.Massa;
				nova.TotalArea += b.Area;
				nova.TotalVolume += b.Volume;
			}
            switch (nova.mType)
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
					nova.out_Name = nova.out_Name + " l=" + nova.TotalLength.ToString("F1") + " м.п.";
					break;
                case myTypes.armNum:
					nova.TotalMassa = nova.Massa * nova.Quantity;
					nova.out_Name = nova.out_Name + " L=" + (Math.Round(nova.Length / 10) * 10).ToString("F0") + " мм";
					break;
                case myTypes.allNum:
                    break;
                case myTypes.plastini:
                    break;
                case myTypes.totalArea:
					nova.out_Name = nova.out_Name + " S=" + (nova.TotalArea*FT*FT).ToString("F2") + " м²";
					nova.TotalMassa = nova.Massa * nova.TotalArea*FT*FT;
					break;
                case myTypes.commonLength:
                    break;
                default:
                    break;
            }

			nova.out_Gost = IN[0].out_Gost;
			//nova.Prior = IN[0].Prior;
			(nova.out_Kol_vo, nova.out_Mass, nova.out_Other) = Meta.chtoto(nova);
			return (nova,addpos);
		}
		public static (string, string, string) chtoto(Cube cube)
		{
			string a="";
			string b="-";
			string c="";
			switch (cube.mType)
			{
				case myTypes.matVol:
					a = "-";
					c = cube.TotalVolume.ToString("F2") + " м³";
					break;
				case myTypes.matArea:
					a = "-";
					c = cube.TotalArea.ToString("F1") + " м²";
					break;
				case myTypes.kmLen:
					a = cube.TotalLength.ToString("F1") + " м.п.";
					b = "-";
					c= cube.TotalMassa.ToString("F2") + " кг";
					break;
				case myTypes.kmNum:
					break;
				case myTypes.armLen:
					a = "-";
					b = cube.Massa.ToString("F3");
					c = cube.TotalMassa.ToString("F2") + " кг";
					break;
				case myTypes.armNum:
					a = cube.Quantity.ToString();
					b = cube.Massa.ToString("F3");
					c = cube.TotalMassa.ToString("F2") + " кг";

					break;
				case myTypes.allNum:
					a = cube.Quantity.ToString();
					b = "-";
					c = "шт";
					break;
				case myTypes.commonLength:
					a = cube.TotalLength.ToString("F1");
					c = "м.п.";
					break;
				case myTypes.plastini:
					a = cube.Quantity.ToString();
					b = cube.Massa.ToString("F2");
					c = cube.TotalMassa.ToString("F2") + " кг";
					break;
				case myTypes.totalArea:
					a = "-";
					b= cube.Massa.ToString("F2");
					c= cube.TotalMassa.ToString("F2") + " кг";
					break;
				default:
					a = cube.Quantity.ToString();
					b = "-";
					c = "шт";
					break;
			}
			return (a,b,c);
		}
	}
}