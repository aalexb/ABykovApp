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
using System.Windows;
using System.Windows.Forms;

namespace WorkApp
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class ExportDWG : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
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


			DWGExportOptions options = new DWGExportOptions();
			options.MergedViews = true;
			options.Colors = ExportColorMode.TrueColorPerView;
			options.FileVersion = ACADVersion.R2013;
			//options.
			
            
			string dir = doc.PathName.Substring(0, doc.PathName.Length - doc.Title.Length - 4);
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.ShowDialog();
            //ialog.SelectedPath
            

			//ModelPath mp=doc.GetWorksharingCentralModelPath();
			string vmp= dialog.SelectedPath;
			int arr = 0;
            for (int i = vmp.Length - 1; i != 0; i--)
            {
                if (vmp.ElementAt(i) == '\\')
                {
                    arr = i;
                    break;
                }
            }


            //string newPath = Path.GetTempPath();
			string newPath = vmp+"\\DWG";
			for (int i = 0; i < listNum.Count(); i++)
			{
				viewName.Add(newPath + listNum[i].SheetNumber+listNum[i].Name+".dwfx");

			}
			string fileName = newPath+ namePrefix+".dwg";
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
				doc.Export(newPath, "", elt, options);
				//doc.Export(newPath.Substring(0, newPath.Length - 1), namePrefix, b, options);
				tr.Commit();
			}
			
			//Form1 SheetControl = new Form1();
			//SheetControl.ShowDialog();
			//string[] amm=Directory.GetFiles(newPath+"\\");

			return Result.Succeeded;
		}
	}
}
