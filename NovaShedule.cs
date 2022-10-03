using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace WorkApp.Addons
{
    internal class NovaShedule
    {
        int Col;
        int Row;
        ViewSchedule shed;

        public NovaShedule(int col, int row)
        {
            Col = col;
            Row = row;
        }
        public void DrawToRevit()
        {
            shed.GetTableData().GetSectionData();
        }

}
