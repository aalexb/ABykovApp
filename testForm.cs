using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace WorkApp
{
    public partial class testForm : System.Windows.Forms.Form
    {
        public testForm()
        {
            InitializeComponent();
            //testGrid.formIt(IN);
            //dataGrid.AutoGenerateColumns = true;

            dataGrid.DataSource = testGrid.db;
            
            dataGrid.Columns[0].DataPropertyName = "Pos";
            dataGrid.Columns[0].HeaderText = "Поз.";
            dataGrid.Columns[0].Width = 50;
            dataGrid.Columns[1].DataPropertyName = "Name";
            dataGrid.Columns[1].HeaderText = "Имя типа";
            dataGrid.Columns[1].Width = 200;
            dataGrid.Columns[2].DataPropertyName = "MARK";
            dataGrid.Columns[2].HeaderText = "Марка";
            dataGrid.Columns[2].Width = 50;
            dataGrid.Columns[3].DataPropertyName = "NumInDoc";
            dataGrid.Columns[3].HeaderText = "Кол-во";
            dataGrid.Columns[3].Width = 50;
            
            //dataGrid.Columns[0]
            //dataGrid.AutoSize = true;
            this.Update();

        }
    }
    public class testGrid
    {
        public static List<testGrid> db=new List<testGrid>();
        public string Name { get; set; }
        public int Pos { get; set; }
        public int NumInDoc { get; set; }
        public int MARK { get; set; }
        public Element refEl { get; set; }
        private testGrid(Element e,int pos)
        {
            this.refEl = e;
            this.Name = e.Name;
            this.MARK = e.LookupParameter("ОД_Марка").AsInteger();
            this.Pos = pos;
        }
        public static void formIt(IEnumerable<Element> s)
        {
            int pos = 1;
            foreach (var item in s)
            {
                db.Add(new testGrid(item, pos));
                pos++;
            }
        }
    }

        
}
