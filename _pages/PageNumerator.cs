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

			List<PageToObj> StartList = new List<PageToObj>();

            foreach (var item in selectedSheets)
            {
				StartList.Add(new PageToObj(doc.GetElement(item) as ViewSheet));
            }
			StartList = StartList.OrderBy(x => x.order).ToList();
            
			
														  
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
				if (winform.resultParam==OutType.pageNum)
				{
                    foreach (var item in winform.pages)
                    {

                        (item.refEl as ViewSheet).get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(item.newValue);
                    }
                }

				if (winform.resultParam==OutType.param)
				{
                    foreach (var item in winform.pages)
                    {

                        (item.refEl as ViewSheet).get_Parameter(winform.par).Set(item.newValue);
                    }
                }

				if (winform.resultParam==OutType.pageName)
				{
                    foreach (var item in winform.pages)
                    {
                        (item.refEl as ViewSheet).get_Parameter(BuiltInParameter.SHEET_NAME).Set(item.newValue);
                    }
                }
                
				//doc.Export(savePath.Substring(0, savePath.Length - 1), namePrefix, setOfPrintingPages, options);
				tr.Commit();
			}
			return Result.Succeeded;
		}

		/*
		 =====МЕТОДЫ=====
		 */

		

	}


	public class PageToObj
    {
		public Element refEl;
		public string Name { get; set; }
		
		public string newValue { get; set; }
		public string Num { get; set; }
		public int order { get; set; }

		void getOrder()
        {
			order = 0;
			string numericString=string.Empty;
			foreach (var c in this.Num)
            {
                if (c >= '0' && c <= '9')
                {
					numericString=string.Concat(numericString, c.ToString());
                }
			}
			order=int.Parse(numericString);
        }
        public PageToObj(Autodesk.Revit.DB.ViewSheet s)
        {
			refEl = s;
			Name = s.Name;
			Num = s.SheetNumber;
            try
            {
				order = s.LookupParameter("_pageNum").AsInteger();
            }
            catch (Exception)
            {
				order = 0;
            }
			getOrder();
        }
		public PageToObj(string name, string num, Element e)
		{
			refEl = e;
			Name = name;
			Num = num;
			try
			{
				order = e.LookupParameter("_pageNum").AsInteger();
			}
			catch (Exception)
			{
				order = 0;
			}
			getOrder();
		}
	}
}
