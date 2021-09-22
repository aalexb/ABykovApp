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
    
    public partial class FullSpec : System.Windows.Forms.Form
    {
        public FullSpec()
        {
            InitializeComponent();
            dataGridView1.DataSource =SpecObj.AllObj;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
    class SpecObj
    {
        public static List<SpecObj> AllObj = new List<SpecObj>();
        public int pos { get; set; }
        public string name { get; set; }
        Element refEl;
        
        public SpecObj(string name, int pos,Element e)
        {
            refEl = e;
            this.name = name;
            this.pos = pos;
        }
    }
}
