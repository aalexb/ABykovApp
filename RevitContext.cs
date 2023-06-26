using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    public class RevitContext
    {
        public RevitContext(Document doc) {
            this.doc = doc;
        }
        public Document doc;
        public Phase selected_phase;
    }
}
