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
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.Text.Json;

namespace WorkApp
{
    public partial class FinishEnterprise : System.Windows.Forms.Form
    {
        List<FinishStyle> a = new List<FinishStyle>();
        public FinishEnterprise(Document doc)
        {
            InitializeComponent();
            
            a.Add(new FinishStyle("ABC", "Wall_1"));
            a.Add(new FinishStyle("XYZ", "Wall_2"));
            listBox1.DataSource = a;
            //var b = new List<string>() { "ABC", "XYZ" };
            
            listBox1.DisplayMember = "Name";
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string JSONstring = JsonSerializer.Serialize(a[0]);
            SaveNClose();
        }
        private void SaveNClose()
        {
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Text = (listBox1.SelectedItem as FinishStyle).Wall;
        }
    }
    class FinishStyle
    {
        public string Name { get; set; }
        public string Wall { get; set; }
        public FinishStyle(string name, string wall)
        {
            Name = name;
            Wall = wall;
        }
        
    }

    class FinishUnit
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public string Category { get; set; }
    }
}
