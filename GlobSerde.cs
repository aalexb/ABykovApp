using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    internal class GlobSerde
    {
        bool shouldCreate = false;
        public GlobalParameter par;
        Document doc;
        string Name;
        public GlobSerde(Document doc, string name)
        {
            this.doc = doc;
            this.Name = name;
            par = null;
            shouldCreate= !exist();
        }
        public void read()
        {

        }
        public void write(string val) 
        {
            if (shouldCreate)
                par = GlobalParameter.Create(doc, Name, ParameterType.Text);
            par.SetValue(new StringParameterValue(val));
        }
        private bool exist()
        {
            var s = GlobalParametersManager.FindByName(doc, Name);
            if (s!=ElementId.InvalidElementId)
            {
                
                par = doc.GetElement(GlobalParametersManager.FindByName(doc, Name)) as GlobalParameter;
                return true;
            }
            return false;
            
        }
    }
}
