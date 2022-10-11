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
            if (select != null)
            {
                //doSomething
                
                
            }

            var one=(ix.ElementAt(0) as ViewSchedule).GetTableData().GetSectionData(0);



            /*
            =========Транзакция=======
            */
            using (Transaction tr = new Transaction(doc, this.GetType().Name))
            {

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
            shed.GetTableData().GetSectionData(0);
        }
    }
}
