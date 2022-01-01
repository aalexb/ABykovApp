using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

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
            return new Rebar(e);
        }
    }
    class StructuralCreator : Creator 
    {
        public override Product Create(Element e)
        {
            return new Structural(e);
        }
    }
    class RailingCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Railing(e);
        }
    }
    class HostedSweepCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new HostedSweep(e);
        }
    }
    class PlatesCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Plates(e);
        }
    }
    class RoofCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Roof(e);
        }
    }
    class GenericCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Generic(e);
        }
    }
    class outProductCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new outProduct(e);
        }
    }
    class AnchorsCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Anchors(e);
        }
    }
    class MaterialCreator : Creator
    {
        public override Product Create(Element e)
        {
            return new Material(e);
        }
    }

    //======================================================

    public abstract class Product
    {
        
        public double FT = 0.3048;
        public const string GROUP = "ADSK_Группирование";
        public const string GOST = "ADSK_Обозначение";
        public const string MAT_GOST = "ADSK_Материал обозначение";
        public const string NAME = "ADSK_Наименование";
        public const string MAT_NAME = "ADSK_Материал наименование";
        public const string MASS = "ADSK_Масса";
        public const string STEEL_MARK = "КР_МаркаСтали";
        //==================================

        public ElementExt refElt { get; set; }
        public List<ElementExt> posElements;
        public myTypes mType;
        public string textUP { get; set; }
        public string textDOWN { get; set; }
        public double Length { get; set; } //Длина единицы
        public double Massa { get; set; } //Масса единицы
        public double TotalMassa { get; set; } //Масса общая
        public int Quantity { get; set; }//Количество
        public outVal ovt{get;set;}
        public string grouping { get; set; }
        public string Name { get; set; }
        public List<Cube> linkedElt { get; set; }
        public List<Product> linkedEltP { get; set; }
        public string typeName { get; set; }
        public void init(Element e)
        {
            Name = e.Name;
            refElt = new ElementExt(e);
        }


    }
    class outProduct : Product { 
        public outProduct(Element e)
        {
            init(e);
        }
    }
    class TABS : Product
    {
        public TABS(Element e)
        {
            init(e);
            refElt = new ElementExt(e);
            grouping = refElt.refElement.getP("Группа");
            linkedElt = new List<Cube>();
        }
    }
    class Structural : Product
    {
        public Structural(Element e)
        {

        }
    }
    class Railing : Product 
    {
        public Railing(Element e)
        {

        }
        
    }
    class HostedSweep : Product 
    { 
        public HostedSweep(Element e)
        {

        }
    }
    class Plates : Product 
    { 
        public Plates(Element e)
        {

        }
    }
    class Roof : Product 
    {
        public Roof(Element e)
        {

        }
    }
    class Generic : Product 
    {
        public Generic(Element e)
        {

        }
    }
    class Anchors : Product 
    {
        public Anchors(Element e)
        {

        }
    }
    class Material : Product 
    {
        public Material(Element e)
        {

        }
    }
}
