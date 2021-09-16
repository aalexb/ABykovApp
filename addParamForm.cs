using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace WorkApp
{
    public partial class addParamForm : System.Windows.Forms.Form
    {
        //BuiltInParameterGroup 
        public bool exempl;
        public string name;
        public BuiltInParameterGroup pGroup;
        public ParameterType pType;
        public int num;
        
        public addParamForm(Document doc)
        {
            InitializeComponent();


            comboBox1.DataSource = Enum.GetValues(typeof(BuiltInParameterGroup));
            comboBox2.DataSource = Enum.GetValues(typeof(ParameterType));

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            exempl = checkBox1.Checked;
            pGroup = (BuiltInParameterGroup)comboBox1.SelectedItem;
            pType = (ParameterType)comboBox2.SelectedItem;
            num = (int)numericUpDown1.Value;
            name = textBox1.Text;

            this.Close();
        }
    }
}
