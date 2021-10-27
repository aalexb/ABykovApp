using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;

namespace WorkApp.wtf
{
    abstract class Creator
    {
        public abstract Product Create(Element e);
    }
    class ConcreteCreatorA : Creator
    {
        public override Product Create(Element e)
        {
            return new ConcreteProductA();
        }
    }
    class TABSCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new TABS(e);
        }
    }
    abstract class Product
    {
        public Element refElt;
        public string grouping { get; set; }
        public List<Cube> linkedElt { get; set; }
    }
    class ConcreteProductA : Product
    {
    }
    class TABS : Product
    {
        public TABS(Element e)
        {
            refElt = e;
            grouping = refElt.getP("Группа");
            linkedElt = new List<Cube>();
        }
    }
}
