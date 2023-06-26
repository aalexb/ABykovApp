using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace WorkApp
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class ExportDWG : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIDocument uidoc = commandData.Application.ActiveUIDocument;
			Document doc = uidoc.Document;

			var exportingViews = uidoc.Selection.GetElementIds();

			//DWG options
            DWGExportOptions dwgOptions = new DWGExportOptions
			{
                MergedViews = true,
                Colors = ExportColorMode.TrueColorPerView,
                FileVersion = ACADVersion.R2013
            };

			//Select path
            FolderBrowserDialog pathDialog = new FolderBrowserDialog();
			pathDialog.ShowDialog();
			string outputPath = pathDialog.SelectedPath + "\\DWG";
			System.IO.Directory.CreateDirectory(outputPath);
			
			//Transaction
			using (Transaction tr = new Transaction(doc, "Вывод DWG"))
			{
				tr.Start();
				doc.Export(outputPath, "", exportingViews, dwgOptions);
				tr.Commit();
			}

			return Result.Succeeded;
		}
	}
}
