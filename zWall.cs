using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;

namespace WorkApp
{
    class zWall
    {
        public Element refWall;
        public string Num;
        public double Area;
        public string OtdType;

        public zWall(Wall w)
        {
            this.refWall = w;
            Num = w.getP("Помещение");
            Area = w.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            OtdType = "Main";
        }
    }
}
