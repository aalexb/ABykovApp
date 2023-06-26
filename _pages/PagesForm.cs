using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Internal.InfoCenter;

namespace WorkApp
{
    
    enum fixSymbol
    {
        None=0,
        LRE,
        RLE
    }
    public enum OutType
    {
        pageNum,
        pageName,
        param
    }
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public partial class PagesForm : System.Windows.Forms.Form, IExternalCommand
    {
        public OutType resultParam = OutType.pageNum;
        bool numerateButton = true;
        public Guid par { get; set; }
        fixSymbol prefixSymbol { get; set; } = fixSymbol.None;
        fixSymbol suffixSymbol { get; set; } = fixSymbol.None;

        public bool cont=false;
        public List<PageToObj> pages = new List<PageToObj>();

        public PagesForm(List<PageToObj> pto)
        {
            //par = BuiltInParameter.SHEET_NUMBER;
            pages = pto;
            
            InitializeComponent();
            var pars = pages[0].refEl.Parameters.Cast<Parameter>().Select(x => new
            {
                Name = x.Definition.Name,
                p = x
            }).ToList();
            pars = pars.OrderBy(x => x.Name).ToList();
            
            comboBox1.DataSource = pars;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "p";


            if (pages[0].Num.First() == '\u202A') { prefixSymbol = fixSymbol.None;button2_Click(null, null); }
            if (pages[0].Num.First() == '\u202B') { prefixSymbol = fixSymbol.LRE; button2_Click(null, null); }
            if (pages[0].Num.Last() == '\u202A') { suffixSymbol = fixSymbol.None; button3_Click(null, null); }
            if (pages[0].Num.Last() == '\u202B') { suffixSymbol = fixSymbol.LRE; button3_Click(null, null); }
            textCounter.Text = pages[0].order.ToString();
            foreach (var item in pages)
                inListBox.Items.Add(item.order + " " + item.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cont = true;
            this.Close();
        }
        public void UpdateList()
        {
            outListBox.Items.Clear();
            string prefix="";
            if (prefixSymbol==fixSymbol.None)
            {
                prefix = textPrefix.Text;
            }
            else if(prefixSymbol== fixSymbol.LRE)
            {
                prefix = "\u202A";
            }
            else if (prefixSymbol == fixSymbol.RLE)
            {
                prefix = "\u202B";
            }
            string suffix="";
            if (suffixSymbol==fixSymbol.None)
            {
                suffix = textSuffix.Text;
            }
            else if (suffixSymbol==fixSymbol.LRE)
            {
                suffix = "\u202A";
            }
            else if (suffixSymbol == fixSymbol.RLE)
            {
                suffix = "\u202B";
            }

            int counter = 1;
            try
            {
                counter = int.Parse(textCounter.Text);
            }
            catch (Exception){}

            foreach (var item in pages)
            {
                item.newValue = prefix + (numerateButton? counter.ToString() : "") + suffix;
                outListBox.Items.Add(item.Name+" = "+ item.newValue);
                counter++;
            }
            this.Update();
        }

        private void textCounter_TextChanged(object sender, EventArgs e){UpdateList();}
        private void textPrefix_TextChanged(object sender, EventArgs e){UpdateList();}
        private void textSuffix_TextChanged(object sender, EventArgs e){UpdateList();}

        private void button2_Click(object sender, EventArgs e)
        {
            switch (prefixSymbol)
            {
                case fixSymbol.None:
                    textPrefix.Enabled = false;
                    textPrefix.Text = "LRE";
                    prefixSymbol = fixSymbol.LRE;
                    button2.Text = "LRE";
                    break;
                case fixSymbol.LRE:
                    textPrefix.Enabled = false;
                    textPrefix.Text = "RLE";
                    prefixSymbol = fixSymbol.RLE;
                    button2.Text = "RLE";
                    break;
                case fixSymbol.RLE:
                    textPrefix.Enabled = true;
                    textPrefix.Text = "";
                    prefixSymbol = fixSymbol.None;
                    button2.Text = "---";
                    break;
                default:
                    break;
            }
            UpdateList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch (suffixSymbol)
            {
                case fixSymbol.None:
                    textSuffix.Enabled = false;
                    textSuffix.Text = "LRE";
                    suffixSymbol = fixSymbol.LRE;
                    button3.Text = "LRE";
                    break;
                case fixSymbol.LRE:
                    textSuffix.Enabled = false;
                    textSuffix.Text = "RLE";
                    suffixSymbol = fixSymbol.RLE;
                    button3.Text = "RLE";
                    break;
                case fixSymbol.RLE:
                    textSuffix.Enabled = true;
                    textSuffix.Text = "";
                    suffixSymbol = fixSymbol.None;
                    button3.Text = "---";
                    break;
                default:
                    break;
            }
            UpdateList();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            paramBefore.Items.Clear();

            par = (comboBox1.SelectedValue as Parameter).GUID;
            foreach (var item in pages)
            {
                string s = (comboBox1.SelectedValue as Parameter).Definition.Name+"="+ item.refEl.LookupParameter((comboBox1.SelectedValue as Parameter).Definition.Name).AsString();
                //if (s == null) { s = "-"; }
                paramBefore.Items.Add(s);
            }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            resultParam = OutType.pageNum;
            comboBox1.Enabled = radioButton2.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            resultParam = OutType.param;
            comboBox1.Enabled = radioButton2.Checked;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Show();
            throw new NotImplementedException();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (numerateButton)
            {
                numerateButton = false;
                textCounter.Enabled= false;
            }
            else 
            {
                numerateButton = true;
                textCounter.Enabled = true;
            }
            UpdateList();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            resultParam = OutType.pageName;
            comboBox1.Enabled = radioButton2.Checked;
        }
    }
    public class MyParam
    {
        Parameter par;
        string hisName { get; set; }
        public MyParam(Parameter p)
        {
            par = p;
            hisName = p.Definition.Name;
        }
    }
}
