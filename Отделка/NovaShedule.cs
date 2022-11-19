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
            var sx=ix.ToArray();
            ViewSchedule vs=doc.GetElement((sx[0] as ScheduleSheetInstance).ScheduleId) as ViewSchedule;
            int offset = 2;
            List<List<String>> data = new List<List<string>>();
            string[] a = { "1","A","B","X","Y" };
            string[] b = { "2", "C", "D" };
            string[] c = { "3", "K", "L","M" };
            string[] d = { "4", "I","" ,"J" };
            int[][] group ={ new int[]{0,1}, new int[] { 2, 3 } };
            data.Add(a.ToList());
            data.Add(b.ToList());
            data.Add(c.ToList());
            data.Add(d.ToList());

            var shed = new NovaShedule(vs, offset);


            /*
            =========Транзакция=======
            */
            using (Transaction tr = new Transaction(doc, this.GetType().Name))
            {
                tr.Start();
                
                shed.CreateRow(data);
                //shed.mergeRow(group, 0);
                
                tr.Commit();
            }
            return Result.Succeeded;
        }
        /*
         ==========МЕТОДЫ==========
        */
        

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
    public class NovaShedule
    {
        int Col;
        int Row;
        int offset;
        ViewSchedule shed;
        List<List<string>> data= new List<List<string>>();
        TableSectionData tsd;

        public NovaShedule(ViewSchedule vs,int OFFSET)
        {
            tsd=vs.GetTableData().GetSectionData(0);
            offset = OFFSET;
        }
        void clearTable()
        {
            int lenght = tsd.LastRowNumber + 1;
            for (int i = offset; i < lenght; i++)
            {
                tsd.RemoveRow(offset);
            }
        }
        public void mergeCol()
        {
            TableMergedCell mergedCell = new TableMergedCell( offset, 0, offset, tsd.NumberOfColumns-1);
            tsd.MergeCells(mergedCell);

        }
        public void mergeRow(List<int[]> groups, int col)
        {
            foreach (var item in groups)
            {
                TableMergedCell mergedCell = new TableMergedCell(item[0] + offset, col, item[1] + offset, col);
                tsd.MergeCells(mergedCell);
            }

        }
        public void setHeight()
        {
            //string[] stringSeparators = new string[] { "\r-" };
            for (int i = 0; i < tsd.LastRowNumber+1; i++)
            {
                for (int k = 0; k < tsd.LastColumnNumber+1; k++)
                {
                    var h = tsd.GetRowHeight(i);
                    var t = tsd.GetCellText(i,k);
                    var m =tsd.GetMergedCell(i,k);
                    int n = 0;
                    foreach (var item in t.Split('\r'))
                    {
                        n++;
                        if (item.Length>50)
                        {
                            n++;
                        }
                        
                        
                    }
                    //var n=t.Split('-').Count();
                    var nh = (n * 5 + 3) / 304.8;
                    if (nh>h&m.Bottom==m.Top)
                    {
                        tsd.SetRowHeight(i, nh);
                    }
                }
            }
        }

        public void CreateRow(List<List<string>> data)
        {
            var style=new TableCellStyle() { FontHorizontalAlignment=HorizontalAlignmentStyle.Left};
            clearTable();
            data.Reverse();
            foreach (var row in data)
            {
                tsd.InsertRow(offset);
                tsd.SetRowHeight(offset, 8 / 304.8);
                int k = 0;
                foreach (var cell in row)
                {
                    if (k < tsd.NumberOfColumns)
                    {
                        if (cell != null)
                        {
                            tsd.SetCellText(offset, k, cell);
                            tsd.ResetCellOverride(offset, k);
                            tsd.SetCellStyle(offset, k, style);
                        }
                    }
                    k++;
                }
            }
        }
    }
}
