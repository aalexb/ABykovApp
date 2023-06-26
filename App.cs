using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using adWin = Autodesk.Windows;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using Autodesk.Revit.UI.Events;
using WorkApp.Properties;
using winform = System.Windows.Forms;

namespace WorkApp
{



    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            try
            {
                adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
                Color customColor = Color.FromRgb(255, 255, 255);
                SolidColorBrush brush = new SolidColorBrush(customColor);
                foreach (adWin.RibbonTab tab in ribbon.Tabs)
                {
                    
                    foreach (adWin.RibbonPanel pan in tab.Panels)
                    {
                        pan.CustomPanelTitleBarBackground = brush;
                        pan.CustomPanelBackground = brush;
                        
                        
                    }
                }
                adWin.ComponentManager.UIElementActivated += ComponentManager_UIElementActivated;
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.StackTrace + "\r\n" + ex.InnerException, "Error", System.Windows.Forms.MessageBoxButtons.OK);
            }

            RibbonPanel panelSheet = ribbonPanel(app,"Листы");
            RibbonPanel panelFinish = ribbonPanel(app, "Отделка");
            RibbonPanel panelOther = ribbonPanel(app, "Прочее");

            PushButton button110 = panelSheet.AddButton("Получить ПДФ", Resources.PDF, "ExportDWFX2");
            PushButton button120 = panelSheet.AddButton("Получить DWG", Resources.dwgpic, "ExportDWG");
            PushButton button130 = panelSheet.AddButton("Смена номера", Resources.tudaSyda, "SheetNum");
            PushButton button140 = panelSheet.AddButton("Нумерация", Resources.block, "PagesForm");




            PushButton button210 = panelFinish.AddButton("Отделочный слой", Resources.emptyHouse, "PerimetralWall");
            PushButton button220 = panelFinish.AddButton("Номер в отделку", Resources.fullHouse, "RNum2FWall");
            PushButton button230 = panelFinish.AddButton("Отделка", Resources.Brush, "NovaFinishing");



            PushButton button310 = panelOther.AddButton("Аннотации", Resources.MGN, "annot");
            PushButton button320 = panelOther.AddButton("Металл", Resources.gear,  "metall");
            PushButton button330 = panelOther.AddButton("Спецификация", Resources.block,  "grouping");
            PushButton button340 = panelOther.AddButton("СуперТест", Resources.block, "SuperTest");
            PushButton button350 = panelOther.AddButton("Всё", Resources.atom, "Universe");
            PushButton button360 = panelOther.AddButton("Стены 2021", Resources.plane, "WallLastStage");
            PushButton button370 = panelOther.AddButton("Общая спецификация", Resources.block, "SPECA");
            PushButton button380 = panelOther.AddButton("Спецификация - расчет", Resources.block, "SetCurrentSpec");
            PushButton button390 = panelOther.AddButton("Гидравлический расчет", Resources.block, "Hydra");
            PushButton button395 = panelOther.AddButton("Пространства", Resources.block, "macros");
            PushButton button399 = panelOther.AddButton("Сброс номеров помещений", Resources.block, "RoomRenumerate");


            PushButton button410 = panelOther.AddButton("Тесты", Resources.block, "TableDraw");
            PushButton button430 = panelOther.AddButton("Перемычки", Resources.block, "RemoveAfterGP2");
            PushButton button440 = panelOther.AddButton("Отделка", Resources.block, "Decore");
            PushButton button450 = panelOther.AddButton("Рабочие наборы", Resources.block, "Viewer");






            app.ApplicationClosing += a_ApplicationClosing;
            app.Idling += A_Idling;
            
            return Result.Succeeded;
        }

        private void ComponentManager_UIElementActivated(object sender, adWin.UIElementActivatedEventArgs e)
        {
            //winform.MessageBox.Show("Title","Error",winform.MessageBoxButtons.OK);
        }

        void A_Idling(object sender, IdlingEventArgs e)
        {

        }
        
        void a_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
            throw new NotImplementedException();
        }

        public RibbonPanel ribbonPanel(UIControlledApplication a,string name)
        {
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab("АММО");

            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel("АММО", name);
            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels("АММО");
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == name)
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
