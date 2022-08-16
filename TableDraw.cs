using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;
using System.Linq;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class TableDraw : IExternalCommand
    {
        static double defColHeight = (-8)/Meta.FT;
        static double defRowWidth = (15) / Meta.FT;
        static List<ElementId> bigData = new List<ElementId>();
        static List<Line> lines = new List<Line>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Group group = null;
            View view=doc.ActiveView;
            var numC = 2;
            var numR = 10;

            drawBound(numC, numR);

            using (Transaction tr = new Transaction(doc,"Рисовашки"))
            {
                tr.Start();

                //draw(doc);
                tr.Commit();
            }
            return Result.Succeeded;
        }

        void drawBound(int M, int N)
        {
            var P1 = new XYZ(0, 0, 0);
            var P2 = new XYZ(M*defRowWidth, 0, 0);
            var P3 = new XYZ(0, N*defColHeight, 0);
            var P4 = new XYZ(M * defRowWidth, N * defColHeight, 0);
            lines.Add(Line.CreateBound(P1, P2));
            lines.Add(Line.CreateBound(P1, P3));
            lines.Add(Line.CreateBound(P2, P4));
            lines.Add(Line.CreateBound(P3, P4));
        }

        void draw(Document doc)
        {
            foreach (var item in lines)
            {
                bigData.Add(doc.Create.NewDetailCurve(doc.ActiveView, item).Id);
            }
            
            doc.Create.NewGroup(bigData);
        }
        void drawGrid()
        {
        }
    }
}
