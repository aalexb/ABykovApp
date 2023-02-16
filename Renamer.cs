using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class Renamer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            /*
            var CollectElements = CollectElements();
            var ListOfParameters = GetListOfParameters(CollectElements);
            var parameter = null;
            */
            using(Transaction tr = new Transaction(null, "Renamer"))
            {
                tr.Start();

                //Rename(CollectElements,parameter,prefix,Number,suffix)

                tr.Commit();
            }



            return Result.Succeeded;
        }
    }

}
