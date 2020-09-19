using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI.Events;

namespace WorkApp
{



    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            RibbonPanel panel = ribbonPanel(a);

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButton button = panel.AddItem(new PushButtonData("DWFX", "DWFX", thisAssemblyPath, "WorkApp.ExportDWFX")) as PushButton;

            button.ToolTip = "Экспорт DWFX";
            var globePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dwfx.png");
            Uri uriImage = new Uri(globePath);
            BitmapImage largeImage = new BitmapImage(uriImage);
            button.LargeImage = largeImage;


            PushButton button2 = panel.AddItem(new PushButtonData("Смена номера", "Смена номера", thisAssemblyPath, "WorkApp.SheetNum")) as PushButton;
            button2.ToolTip = "Меняет нумерацию у двух листов";
            var globePath2 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "18070417311582004489-32.png");
            Uri uriImage2 = new Uri(globePath2);
            BitmapImage largeImage2 = new BitmapImage(uriImage2);
            button2.LargeImage = largeImage2;


            PushButton button3 = panel.AddItem(new PushButtonData("Эл-ты Помещ.", "Эл-ты Помещ.", thisAssemblyPath, "WorkApp.RoomElements")) as PushButton;
            button3.ToolTip = "Добавляет описание элементов по каждому помещению";
            var globePath3 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "19695904531582800514-32.png");
            Uri uriImage3 = new Uri(globePath3);
            BitmapImage largeImage3 = new BitmapImage(uriImage3);
            button3.LargeImage = largeImage3;

            PushButton button4 = panel.AddItem(new PushButtonData("Отделка", "Отделка", thisAssemblyPath, "WorkApp.Finishing")) as PushButton;
            button4.ToolTip = "Отделка помещений";
            var globePath4 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "6187224341600002476-32.png");
            Uri uriImage4 = new Uri(globePath4);
            BitmapImage largeImage4 = new BitmapImage(uriImage4);
            button4.LargeImage = largeImage4;

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
