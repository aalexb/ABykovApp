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
    public class outVal
    {
        public string Name { get; set; }
        public string Pos { get; set; }
        public string Gost { get; set; }
        public string Kol { get; set; }
        public string Mass { get; set; }
        public string Other { get; set; }
    }
    public abstract class Creator
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
    class RebarCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Rebar();
        }
    }
    public abstract class Product
    {
        public Element refElt;
        public myTypes mType;
        public outVal ovt{get;set;}
        public string grouping { get; set; }
        public string Name { get; set; }
        public List<Cube> linkedElt { get; set; }
        public List<Product> linkedEltP { get; set; }
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
    class Rebar : Product
    {

    }
}
