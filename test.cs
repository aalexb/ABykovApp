using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class test : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            List<FamilyInstance> karkas = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
              .WhereElementIsNotElementType()
              .Cast<FamilyInstance>()
              .ToList();

            List<FamilyInstance> kolon = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            //List
            List<FamilyInstance> myKar = new List<FamilyInstance>();
            foreach (FamilyInstance i in karkas)
            {
                if (i.StructuralType.ToString() != "NonStructural")
                {
                    myKar.Add(i);
                }
            }

            List<Cube> cubes = new List<Cube>();

            foreach (FamilyInstance e in myKar)
            {
                if (e.LookupParameter("ADSK_Группирование").AsString().Length>2)
                {
                    cubes.Add(new Cube(e.LookupParameter("ADSK_Группирование").AsString(), e.Name));
                }
                
            }






           

            ElementParameterFilter cubeFilter = new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId((int)BuiltInParameter.ALL_MODEL_TYPE_NAME)), new FilterStringEquals(), "cube", false));
            List<FamilyInstance> existCubes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().WherePasses(cubeFilter).Cast<FamilyInstance>().ToList();

            using (Transaction tr = new Transaction(doc, "creating"))
            {
                tr.Start();
                foreach (FamilyInstance i in existCubes)
                {
                    doc.Delete(i.Id);
                }

                foreach (Cube i in cubes)
                {
                    i.Create(doc);
                }
                
                tr.Commit();
            }


            return Result.Succeeded;
        }
    }
}
