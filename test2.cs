using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Modules;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Test2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string s = null;
            FinishDB main = new FinishDB("Hello");
            main.Show();
            return Result.Succeeded;
        }
    }
}
