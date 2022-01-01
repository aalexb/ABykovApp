using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Empty : IExternalCommand
    {

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            //Получаем объекты
            getAllElements();
            //Создаём внутренние объекты
            List<Ghost> lg = createInnerObjects(null);
            //Делим по группам
            splitByGroup(lg);
            //Объединяем по группам
            sumByGroup();

            using (Transaction tr = new Transaction(doc, "Крутая спецификация"))
            {
                tr.Start();

                //Передаем результат в Ревит
                commitToRevit();

                tr.Commit();
            }
            return Result.Succeeded;
        }

        void splitByGroup(List<Ghost> lg)
        {
            List<List<Ghost>> LLghost = new List<List<Ghost>>();
            List<Ghost> adder = new List<Ghost>();
            
            foreach (var item in lg.Select(x=>x.outField.out_Group).Distinct())
            {

            }
        }

        void getAllElements() 
        { 

        }
        List<Ghost> createInnerObjects(List<Element> raw) 
        {
            List<Ghost> ghost=new List<Ghost>();
            foreach (Element e in raw)
            {
                ghost.Add(new BaseFactory().Create(e));
            }
            return ghost;
        }
        void sumByGroup() 
        { 

        }
        void commitToRevit() 
        { 

        }
    }


    public abstract class Factory //Абстрактный класс создающий обект
    {
        public abstract Ghost Create(Element e);
    }
    class BaseFactory : Factory //Базовый класс создания объекта
    {
        public override Ghost Create(Element e)
        {
            return new BaseGhost(e);
        }
    }

    public abstract class Ghost //Абстрактный класс описывающий объект
    {
        public Element refElement { get; set; }//Элемент на который ссылается объект
        public OutText outField{get;set;} //Поля для вывода в Ревит
        public string typeName { get; set; }//Имя объекта
        protected void init(Element e)
        {
            refElement = e;
            typeName = e.Name;
        }

        

    }
    class BaseGhost : Ghost //Базовый класс создающий объект
    {
        public BaseGhost(Element e)
        {
            this.init(e);
        }
    }

    public class OutText
    {
        public string out_Pos { get; set; } //Позиция
        public string out_Gost { get; set; } //ГОСТ
        public string out_Name { get; set; } //Имя в спецификации
        public string out_Kol_vo { get; set; } //Количество в штуках
        public string out_Mass { get; set; } //Масса
        public string out_Other { get; set; } //Примечание
        public string out_Group { get; set; } //Группирование
    }
}
