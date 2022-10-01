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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorkApp
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class PageNumerator : IExternalCommand
	{
		Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var doc = uidoc.Document;

			var selectedSheets = uidoc.Selection.GetElementIds();
			List<Element> sheets = new List<Element>();
			List<PageToObj> StartList = new List<PageToObj>();

            foreach (var item in selectedSheets)
            {
				StartList.Add(new PageToObj(doc.GetElement(item) as ViewSheet));
            }
			//StartList = StartList.OrderBy(s => s.Num, new CustomComparer()).ToList();
			//StartList = NaturalSort(StartList.Select(x=>x.Num)).ToList();                            // Extract the values
														   //StartList=StartList.OrderBy(x=>x.Num).ToList();
			var winform= new PagesForm(StartList);
			winform.ShowDialog();
            if (!winform.cont)
            {
				return Result.Cancelled;
            }
			//var setOfPrintingPages = new ViewSet();
			
			using (Transaction tr = new Transaction(doc, "Нумерация листов"))
			{
				tr.Start();
                foreach (var item in winform.EndList)
                {
					(item.refEl as ViewSheet).get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(item.Num);
                }
				//doc.Export(savePath.Substring(0, savePath.Length - 1), namePrefix, setOfPrintingPages, options);
				tr.Commit();
			}
			return Result.Succeeded;
		}

		/*
		 =====МЕТОДЫ=====
		 */
		public class CustomComparer : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				var regex = new Regex("^(d+)");

				// run the regex on both strings
				var xRegexResult = regex.Match(x);
				var yRegexResult = regex.Match(y);

				// check if they are both numbers
				if (xRegexResult.Success && yRegexResult.Success)
				{
					return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
				}

				// otherwise return as string comparison
				return x.CompareTo(y);
			}
		}
		

	}

	public class PageToObj
    {
		public Element refEl;
		public string Name { get; set; }
		public string Num { get; set; }
        public PageToObj(Autodesk.Revit.DB.ViewSheet s)
        {
			refEl = s;
			Name = s.Name;
			Num = s.SheetNumber;
        }
		public PageToObj(string name, string num, Element e)
		{
			refEl = e;
			Name = name;
			Num = num;
		}
	}
}
