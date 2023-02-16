using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace WorkApp.Deco
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class Decore : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var filter = new Filter(doc);

            

            var decoreClient = new DecoreClient(doc,filter);
            decoreClient.Show();


            var decorator = new Decorator();

            return decorator.Run();

        }

        private void Filter(Document doc)
        {
            var collect = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToElements();

        }




    }



    public class Filter
    {
        public IList<Element> wallTypes { get; private set; }
        public PhaseArray stages { get;private set; }

        public Filter(Document doc)
        {
            wallTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToElements();
            stages = doc.Phases;
        }

        
    }

    class Decorator
    {
        public Decorator()
        {

        }


        public Result Run()
        {
            /*
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * */



            return Result.Succeeded;
        }
    }

    
}
