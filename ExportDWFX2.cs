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
	class ExportDWFX2 : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var doc = uidoc.Document;

			var selectedSheets = uidoc.Selection.GetElementIds();
			var setOfPrintingPages = new ViewSet();

			foreach (var sheet in selectedSheets)
            {
				setOfPrintingPages.Insert(doc.GetElement(sheet) as ViewSheet);
            }
			

            var options = new DWFXExportOptions
            {
                MergedViews = true
            };

            string savePath = Path.GetTempPath();
			string namePrefix = "Export";
			string fileName = savePath+ namePrefix+".dwfx";
			
			using (Transaction tr = new Transaction(doc, "MyExort"))
			{
				tr.Start();
				doc.Export(savePath.Substring(0, savePath.Length - 1), namePrefix, setOfPrintingPages, options);
				tr.Commit();
			}
			
			try
            {
                using (FileStream fs = File.Create(savePath+"printing.bpj"))
                {
					byte[] info = new UTF8Encoding(true).GetBytes(batchPrint(fileName));
					fs.Write(info,0,info.Length);
                }
            }
            catch (Exception){}

			Process.Start("DesignReview.exe", savePath + "printing.bpj");
			return Result.Succeeded;
		}

		/*
		 =====МЕТОДЫ=====
		 */
		string batchPrint(string name)
		{
			string batch = "<configuration_file>";
			batch += string.Format("<DWF_File FileName=\"{0}\" PageSize=\"Прочее\" " +
				"NoOfSections=\"2\" Print_to_scale=\"100\" Print_Style=\"1\" " +
				"Print_What=\"0\" Fit_To_Paper=\"0\" Paper_Size=\"9\" " +
				"Paper_Size_Width=\"2100\" Paper_Size_Height=\"2970\" Orientation=\"1\"" +
				" Number_of_copies=\"1\" PrinterName=\"Adobe PDF\" Page_Range=\"0\" " +
				"Print_Range_Str=\"\" Reverse_Order=\"0\" Collate=\"0\" printColor=\"0\" " +
				"MarkupColor=\"0\" printAlignment=\"0\" Use_DWF_Paper_Size=\"-1\" " +
				"PrintasImage=\"0\" PaperName=\"A4\"/>", name);
			batch += "</configuration_file>";
			return batch;
		}
	}

	
}
