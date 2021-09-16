using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class ExportDWFX : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;
			List < Element > titleBlocks = new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_TitleBlocks)
				.WhereElementIsNotElementType()
				.ToElements()
				.ToList();

			List<ElementId> elt = uidoc.Selection.GetElementIds().ToList();

			List<Element> beta = new List<Element>();
            foreach (ElementId i in elt)
            {
				beta.Add(doc.GetElement(i));
            }
			ViewSheet a;
			a = doc.GetElement(elt.First()) as ViewSheet;
			ViewSet b = new ViewSet();
			List<List<Element>> elementOnSheet = new List<List<Element>>();


            for (int i = 0; i < elt.Count(); i++)
            {
				List<Element> sheetEl = new List<Element>();
                foreach (Element e in new FilteredElementCollector(doc).OwnedByView(elt[i]))
                {
					sheetEl.Add(e);
                }
				elementOnSheet.Add(sheetEl);
            }
			List<Element> titleOnSheet = new List<Element>();


            for (int i = 0; i < elt.Count(); i++)
            {
				titleOnSheet.Add(null);
                foreach (Element e in elementOnSheet[i])
                {
                    foreach (Element tb in titleBlocks)
                    {
                        if (e.Id==tb.Id)
                        {
							titleOnSheet[i] = tb;
                        }
                    }
                }
            }

			List<int> formatA = new List<int>();
			List<int> kratn = new List<int>();
			List<int> knign = new List<int>();
            for (int i = 0; i < titleOnSheet.Count(); i++)
            {
                try
                {
					formatA.Add(titleOnSheet[i].LookupParameter("Формат А").AsInteger());
					kratn.Add(titleOnSheet[i].LookupParameter("Кратность").AsInteger());
					knign.Add(titleOnSheet[i].LookupParameter("Книжная ориентация").AsInteger());
				}
                catch (Exception)
                {
                    
                }
             
				

			}
			string namePrefix = "Export";
			//string nameOfExportDWFX = "AutoExport " + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
			List<string> viewName = new List<string>();
			List<ViewSheet> listNum = new List<ViewSheet>();
			foreach (ElementId i in elt)
			{

				b.Insert(doc.GetElement(i) as ViewSheet);
				listNum.Add(doc.GetElement(i) as ViewSheet);
			}
            
            
			DWFXExportOptions options = new DWFXExportOptions();
			options.MergedViews = true;
			
			
            
			string dir = doc.PathName.Substring(0, doc.PathName.Length - doc.Title.Length - 4);


			//ModelPath mp=doc.GetWorksharingCentralModelPath();
			//string vmp=ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
			//int arr = 0;
   //         for (int i = vmp.Length-1; i !=0; i--)
   //         {
   //             if (vmp.ElementAt(i)=='\\')
   //             {
			//		arr = i;
			//		break;
   //             }
   //         }


			string newPath = Path.GetTempPath();
			//string newPath = vmp.Substring(0, arr+1)+"PDF";
			for (int i = 0; i < listNum.Count(); i++)
			{
				viewName.Add(newPath + listNum[i].SheetNumber+listNum[i].Name+".dwfx");

			}
			string fileName = newPath+ namePrefix+".dwfx";
			System.IO.DirectoryInfo printDir = System.IO.Directory.CreateDirectory(newPath);
			
			
			using (Transaction tr = new Transaction(doc, "MyExort"))
				
			{
				tr.Start();
				/*
				foreach (ViewSheet i in listNum)
				{
					ViewSet vSet = new ViewSet();
					vSet.Insert(i);
				
					doc.Export(newPath, i.SheetNumber + i.Name,vSet, options);
                }
				*/
				doc.Export(newPath.Substring(0, newPath.Length - 1), namePrefix, b, options);
				tr.Commit();
			}
			
			//Form1 SheetControl = new Form1();
			//SheetControl.ShowDialog();
			//string[] amm=Directory.GetFiles(newPath+"\\");
			
			try
            {
                using (FileStream fs = File.Create(newPath+"printing.bpj"))
                {
					//byte[] info = new UTF8Encoding(true).GetBytes(batchPrint(viewName, formatA, kratn, knign));
					byte[] info = new UTF8Encoding(true).GetBytes(batchPrint(fileName));
					fs.Write(info,0,info.Length);
                }
            }
            catch (Exception)
            {

                
            }
			Environment.SetEnvironmentVariable("MYAPP", newPath.Substring(0, newPath.Length-1),EnvironmentVariableTarget.User);
			Process.Start("DesignReview.exe", newPath + "printing.bpj");
			

			return Result.Succeeded;
		}
		string batchPrint(List<string> name, List<int> fA, List<int> kr, List<int> kniga)
		{
			string batch = "<configuration_file>";
            for (int i = 0; i < name.Count(); i++)
            {
				batch += string.Format("<DWF_File FileName=\"{0}\" PageSize=\"Прочее\" NoOfSections=\"2\" Print_to_scale=\"100\" Print_Style=\"1\" Print_What=\"0\" Fit_To_Paper=\"0\" Paper_Size=\"9\" Paper_Size_Width=\"2100\" Paper_Size_Height=\"2970\" Orientation=\"{2}\" Number_of_copies=\"1\" PrinterName=\"Bullzip PDF Printer\" Page_Range=\"0\" Print_Range_Str=\"\" Reverse_Order=\"0\" Collate=\"0\" printColor=\"0\" MarkupColor=\"0\" printAlignment=\"0\" Use_DWF_Paper_Size=\"-1\" PrintasImage=\"0\" PaperName=\"A{1}{3}\"/>", name[i], fA[i], kniga[i]==1?1:2, kr[i]!=1?"x"+kr[i].ToString():"");
            }
			batch += "</configuration_file>";
			return batch;
		}
		string batchPrint(string name)
		{
			string batch = "<configuration_file>";
			batch += string.Format("<DWF_File FileName=\"{0}\" PageSize=\"Прочее\" NoOfSections=\"2\" Print_to_scale=\"100\" Print_Style=\"1\" Print_What=\"0\" Fit_To_Paper=\"0\" Paper_Size=\"9\" Paper_Size_Width=\"2100\" Paper_Size_Height=\"2970\" Orientation=\"1\" Number_of_copies=\"1\" PrinterName=\"Adobe PDF\" Page_Range=\"0\" Print_Range_Str=\"\" Reverse_Order=\"0\" Collate=\"0\" printColor=\"0\" MarkupColor=\"0\" printAlignment=\"0\" Use_DWF_Paper_Size=\"-1\" PrintasImage=\"0\" PaperName=\"A4\"/>", name);
			batch += "</configuration_file>";
			return batch;
		}
	}

	
}
