using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace WorkApp.wtf
{
    class Rebar : Product 
    {
        public Rebar(Element e)
        {
            init(e);
            string[] stringSeparator = new string[] { " : " };
            string[] subName = e.Name.Split(stringSeparator, StringSplitOptions.None);
            ovt.Name= "ø" + subName[0];
            if (e.LookupParameter("шт/м/м2/м3 экз").AsInteger() == 1)
            {
                mType = myTypes.armNum;
                Length = ((Autodesk.Revit.DB.Structure.Rebar)e).get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble() * 1000 * FT;
                refElt.textDOWN = (Math.Round(Length / 10) * 10).ToString() + "мм";
                Quantity = ((Autodesk.Revit.DB.Structure.Rebar)e).Quantity;
                Massa = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble() * Length / 1000;
            }
            else
            {
                mType = myTypes.armLen;
                Massa = e.Document.GetElement(e.GetTypeId()).LookupParameter(MASS).AsDouble();
                try
                {
                    Length = ((Autodesk.Revit.DB.Structure.Rebar)e).TotalLength * FT;
                }
                catch (Exception)
                {

                    Length = ((RebarInSystem)e).TotalLength * FT;
                }
            }
            if (refElt.textDOWN != null)
            {
                refElt.textDOWN += ", ";
            }

            try
            {

                refElt.textDOWN += "шаг " + ((e as Autodesk.Revit.DB.Structure.Rebar).MaxSpacing * FT * 1000).ToString("F0");
            }
            catch (Exception)
            {


            }
        }
    }
}
