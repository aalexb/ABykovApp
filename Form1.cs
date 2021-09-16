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

namespace WorkApp
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public PaperSize ps=null;
        public Form1(PaperSizeSet pss)
        {
            InitializeComponent();
            listBox1.DataSource = pss as IList<PaperSize>;
            listBox1.DisplayMember = "Name";
            //listBox1.ValueMember = "Id";
            foreach (PaperSize item in pss)
            {
                listBox1.Items.Add(item);
            }


            
            //listBox1.d
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ps = listBox1.SelectedItem as PaperSize;
            this.Close();
        }
    }
}
