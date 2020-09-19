using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
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
				formatA.Add(titleOnSheet[i].LookupParameter("Формат А").AsInteger());
				kratn.Add(titleOnSheet[i].LookupParameter("Кратность").AsInteger());
				knign.Add(titleOnSheet[i].LookupParameter("Книжная ориентация").AsInteger());

			}

			foreach (ElementId i in elt)
			{
				b.Insert(doc.GetElement(i) as ViewSheet);
			}
			DWFXExportOptions options = new DWFXExportOptions();
			options.MergedViews = false;
			
			string dir = doc.PathName.Substring(0, doc.PathName.Length - doc.Title.Length - 4);
			ModelPath mp=doc.GetWorksharingCentralModelPath();
			string vmp=ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
			int arr = 0;
            for (int i = vmp.Length-1; i !=0; i--)
            {
                if (vmp.ElementAt(i)=='\\')
                {
					arr = i;
					break;
                }
            }
			string newPath = vmp.Substring(0, arr+1)+"PDF";
			
            System.IO.DirectoryInfo printDir = System.IO.Directory.CreateDirectory(newPath);
            string nameOfExportDWFX = "AutoExport " + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
			
			using (Transaction tr = new Transaction(doc, "MyExort"))
				
			{
				tr.Start();


				doc.Export(newPath, nameOfExportDWFX, b, options);
				tr.Commit();
			}
			//Form1 SheetControl = new Form1();
			//SheetControl.ShowDialog();
			TaskDialog msg = new TaskDialog("Info");
			msg.MainContent = "Ok";
			msg.Show();

			

			return Result.Succeeded;
		}
		void batchPrint(List<string> name, List<int> fA, List<int> kr, List<int> kniga)
		{
            for (int i = 0; i < name.Count(); i++)
            {

            }
		}
	}

	
}
