using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class template : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            var select = uidoc.Selection;
            if (select!=null)
            {
                //doSomething
            }




            /*
            =========Транзакция=======
            */
            using (Transaction tr=new Transaction(doc,this.GetType().Name))
            {

            }
            return Result.Succeeded;
        }
        /*
         * МЕТОДЫ *
         */
        void EmptyMethod1() { }
        void EmptyMethod2() { }
    }
}
