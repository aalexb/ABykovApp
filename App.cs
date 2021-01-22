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

namespace WorkApp
{



    public class App : IExternalApplication
    {
        public BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }



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

            PushButton button5 = panel.AddItem(new PushButtonData("Аннотации", "Аннотации", thisAssemblyPath, "WorkApp.annot")) as PushButton;
            button5.ToolTip = "Обновить инвалидов";
            var globePath5 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "14463930391582863594-32.png");
            Uri uriImage5 = new Uri(globePath5);
            BitmapImage largeImage5 = new BitmapImage(uriImage5);
            button5.LargeImage = largeImage5;
            
            PushButton button6 = panel.AddItem(new PushButtonData("Металл", "Металл", thisAssemblyPath, "WorkApp.metall")) as PushButton;
            button6.ToolTip = "Обновить спецификации металла";
            var globePath6 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "14463930391582863594-32.png");
            //Uri uriImage6 = new Uri();

            //BitmapImage largeImage6 = WorkApp.Properties.Resources.gear as BitmapImage;
            BitmapImage pic6 =       ConvertBitmap( WorkApp.Properties.Resources.gear);

            button6.LargeImage = pic6;


            PushButton button7 = panel.AddItem(new PushButtonData("Спецификация", "Спецификация", thisAssemblyPath, "WorkApp.grouping")) as PushButton;
            button7.ToolTip = "Спецификация по группированию";

            PushButton testButton = panel.AddItem(new PushButtonData("Test", "Test", thisAssemblyPath, "WorkApp.test")) as PushButton;

            PushButton button8 = panel.AddItem(new PushButtonData("Периметр", "Периметр", thisAssemblyPath, "WorkApp.PerimetralWall")) as PushButton;
            button8.ToolTip = "Стены по периметру";
            var globePath8 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "14463930391582863594-32.png");
            //Uri uriImage6 = new Uri();

            //BitmapImage largeImage6 = WorkApp.Properties.Resources.gear as BitmapImage;
            BitmapImage pic8 = ConvertBitmap(WorkApp.Properties.Resources.gear);

            button8.LargeImage = pic8;

            PushButton button9 = panel.AddButton("СуперТест", "14463930391582863594-32.png", thisAssemblyPath, "SuperTest");
            


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
