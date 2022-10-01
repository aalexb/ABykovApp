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
    public static class Log
    {
        public static void msg(string s)
        {
            TaskDialog msg = new TaskDialog("Info");
            msg.MainInstruction = s;
            msg.Show();
        }
    }
}
