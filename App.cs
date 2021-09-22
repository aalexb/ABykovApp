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

            PushButton button1 = panel.AddButton("Получить ПДФ", Resources.PDF, "ExportDWFX");
            PushButton buttonPDFspec = panel.AddButton("ПДФ спец.", Resources.PDF, "ExportPDFspec");
            PushButton buttonDWG = panel.AddButton("Получить DWG", Resources.dwgpic, "ExportDWG");
            PushButton button2 = panel.AddButton("Смена номера", Resources.tudaSyda, "SheetNum");
            PushButton button3 = panel.AddButton("Эл-ты помещ.", Resources.Peace, "RoomElements");
            PushButton button4 = panel.AddButton("Отделка", Resources.Brush, "NovaFinishing");
            PushButton button5 = panel.AddButton("Аннотации", Resources.MGN, "annot");
            PushButton button6 = panel.AddButton("Металл", Resources.gear,  "metall");
            PushButton button7 = panel.AddButton("Спецификация", Resources.block,  "grouping");
            PushButton button8 = panel.AddButton("Периметр", Resources.emptyHouse,  "PerimetralWall");
            PushButton button9 = panel.AddButton("СуперТест", Resources.block, "SuperTest");
            PushButton buttonTest = panel.AddButton("Номер в отделку", Resources.fullHouse, "RNum2FWall");
            PushButton buttonUniv = panel.AddButton("Всё", Resources.atom, "Universe");
            PushButton buttonWLS= panel.AddButton("Стены 2021", Resources.plane, "WallLastStage");
            PushButton buttonSPEC = panel.AddButton("Общая спецификация", Resources.block, "SPECA");
            PushButton SetCurrentSpec = panel.AddButton("Спецификация - расчет", Resources.block, "SetCurrentSpec");
            PushButton Hydpra = panel.AddButton("Гидравлический расчет", Resources.block, "Hydra");







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
