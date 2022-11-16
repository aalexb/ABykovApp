using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp.Addons
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class EditSchedule : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            var select = uidoc.Selection.GetElementIds();
            var ix = select.Select(x => doc.GetElement(x));

            ViewSchedule vs=ix.ElementAt(0) as ViewSchedule;
            int offset = 2;

            /*
            =========Транзакция=======
            */
            using (Transaction tr = new Transaction(doc, this.GetType().Name))
            {
                tr.Start();

                CreateRow(clearTable(vs,offset), offset, 5);
                
                tr.Commit();
            }
            return Result.Succeeded;
        }
        /*
         ==========МЕТОДЫ==========
        */
        TableSectionData clearTable(ViewSchedule vs,int offset)
        {
            var tsd = vs.GetTableData().GetSectionData(0);
            int lenght = tsd.LastRowNumber + 1;
            for (int i = offset; i<lenght; i++)
            {
                tsd.RemoveRow(offset);
            }
            return tsd;
        }
        void CreateRow(TableSectionData tsd,int offset,int data)
        {
            for (int i = 0; i < data; i++)
            {
                tsd.InsertRow(offset);
                tsd.SetRowHeight(offset, 8 / 304.8);
                for (int k = 0; k < data; k++)
                {
                   
                }
            }
            
        }

        void EmptyMethod1() { }
        void EmptyMethod2() { }
    }


    /*
    ==========КЛАССЫ==========
    */
    class SimpleSheduleData
    {

        SimpleSheduleData() { }
    }
    internal class NovaShedule
    {
        int Col;
        int Row;
        int offset;
        ViewSchedule shed;
        List<List<string>> data= new List<List<string>>();

        public NovaShedule(int col, int row)
        {
            Col = col;
            Row = row;
        }

        TableSectionData clearTable(ViewSchedule vs, int offset)
        {
            var tsd = vs.GetTableData().GetSectionData(0);
            int lenght = tsd.LastRowNumber + 1;
            for (int i = offset; i < lenght; i++)
            {
                tsd.RemoveRow(offset);
            }
            return tsd;
        }
        void CreateRow(TableSectionData tsd, int offset, List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                tsd.InsertRow(offset);
                tsd.SetRowHeight(offset, 8 / 304.8);
            }

        }
        public void DrawToRevit()
        {
            shed.GetTableData().GetSectionData(0);
            //CreateRow(clearTable(shed,offset), offset, data);
        }
    }
}
