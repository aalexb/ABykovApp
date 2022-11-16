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
    public partial class PagesForm : System.Windows.Forms.Form
    {
        public bool cont=false;
        public PaperSize ps=null;
        public List<PageToObj> StartList = new List<PageToObj>();
        public List<PageToObj> EndList = new List<PageToObj>();
        public PagesForm(List<PageToObj> pto)
        {
            StartList = pto;
            InitializeComponent();

            //listBox1.DataSource = pss as IList<PaperSize>;
            //listBox1.DisplayMember = "Name";
            //listBox1.ValueMember = "Id";
            foreach (var item in pto)
            {
                lbBegin.Items.Add(item.order+" "+item.Name);
            }


            
            //listBox1.d
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //UpdateList();
            cont = true;
            this.Close();
        }
        public void UpdateList()
        {
            lbEnd.Items.Clear();
            EndList.Clear();
            //ps = listBox1.SelectedItem as PaperSize;
            string prefix = textPrefix.Text;
            int counter = 1;
            try
            {
                counter = int.Parse(textCounter.Text);
            }
            catch (Exception)
            {

            }
            
            string suffix = textSuffix.Text;
            foreach (var item in StartList)
            {
                var a = new PageToObj(item.Name, prefix + counter + suffix, item.refEl);
                EndList.Add(a);
                counter++;
            }
            foreach (var item in EndList)
            {
                lbEnd.Items.Add(item.Num + " " + item.Name);
            }
            this.Update();
        }

        private void textCounter_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void textPrefix_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void textSuffix_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
    }
}
