using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI.Events;
using System.IO;
using System.Drawing;
using System.Windows.Media;
using WorkApp.Properties;

namespace WorkApp
{

    

    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            RibbonPanel panel = ribbonPanel(a);

            PushButton button1 = panel.AddButton("Получить ПДФ", Resources.pdf, "ExportDWFX");
            PushButton button2 = panel.AddButton("Смена номера", Resources.tudaSyda, "SheetNum");
            PushButton button3 = panel.AddButton("Эл-ты помещ.", Resources.pirog, "RoomElements");
            PushButton button4 = panel.AddButton("Отделка", Resources.kista, "Finishing");
            PushButton button5 = panel.AddButton("Аннотации", Resources.inval, "annot");
            PushButton button6 = panel.AddButton("Металл", Resources.gear,  "metall");
            PushButton button7 = panel.AddButton("Спецификация", Resources.block,  "grouping");
            PushButton button8 = panel.AddButton("Периметр", Resources.block,  "PerimetralWall");
            PushButton button9 = panel.AddButton("СуперТест", Resources.block, "SuperTest");
            PushButton buttonTest = panel.AddButton("Тест", Resources.block, "test");
            PushButton buttonUniv = panel.AddButton("Всё", Resources.block, "Universe");





            a.ApplicationClosing += a_ApplicationClosing;
            a.Idling += A_Idling;
            
            return Result.Succeeded;
        }

        void A_Idling(object sender, IdlingEventArgs e)
        {

        }
        
        void a_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
            throw new NotImplementedException();
        }

        public RibbonPanel ribbonPanel(UIControlledApplication a)
        {
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab("АММО");

            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel("АММО", "Настройка листов");
                //RibbonPanel secondPanel = a.CreateRibbonPanel("АММО", "Общее");

            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels("АММО");
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == "Настройка листов")
                {
                    ribbonPanel = p;
                }
            }
            return ribbonPanel;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
