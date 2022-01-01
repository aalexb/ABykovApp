using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Reborn
{
    public class App : IExternalApplication
    {


        public Result OnStartup(UIControlledApplication a)
        {
            RibbonPanel panel = makePanel(a);

            //panel.AddButton();


            return Result.Succeeded;
        }
        public RibbonPanel makePanel(UIControlledApplication a)
        {
            return null;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        
    }
    public static class AppExt
    {
        public static PushButton AddButton(this RibbonPanel rp, string name, Bitmap pic, string className)
        {
            return null;
        }
    }
}
