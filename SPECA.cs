using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;


namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SPECA : IExternalCommand
    { 
    Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            IList<Element> ReadPipes = filterOfCat(doc, BuiltInCategory.OST_PipeCurves);
            List<Element> GetFromModel = new List<Element>();
            foreach (var item in ReadPipes)
            {
                GetFromModel.Add(item);
            }

            int a = 0;
            foreach (Element e in GetFromModel)
            {
                SpecObj.AllObj.Add(new SpecObj(e.Name,a++,e));
            }

            FullSpec main = new FullSpec();
            main.Show();

            return Result.Succeeded;
        }
        List<Element> filterOfCat(Document doc, BuiltInCategory cat)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(cat)
                .ToList();

        }

    }
    class MainSorter
    {
        static List<MainSorter> AllElements = new List<MainSorter>();

        public MainSorter(Element e)
        {


            AllElements.Add(this);
        }
        
    }

    class SPEC_object
    {
        static List<SPEC_object> Output;
        IDENTY id;
        int pos;
        string Name;
        string Gost;
        string Code;
        string Fab;
        string Ed;
        string Kol;
        string Mas;
        string Other;

        public SPEC_object(string Name)
        {
            id = new IDENTY();
            this.Name = Name;
            Output.Add(this);
        }

    }

    class IDENTY
    {
        public string stage { get; set; }
        public string title { get; set; }
        public string tag { get; set; }
        public int pos { get; set; }
        public int type { get; set; }
    }

}
