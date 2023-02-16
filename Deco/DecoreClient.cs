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

namespace WorkApp.Deco
{
    public partial class DecoreClient : System.Windows.Forms.Form
    {
        Element SelectedElement;
        Config config;
        IList<Element> MainWall=new List<Element>();
        public DecoreClient(Document doc, Filter filter)
        {

            InitializeComponent();
            init(doc, filter);
            
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void init(Document doc,Filter filter)
        {
            phaseBox.DataSource = filter.stages as IList<Phase>;
            phaseBox.DisplayMember = "Name";
            phaseBox.ValueMember = "Id";
            foreach (var item in filter.stages)
            {
                phaseBox.Items.Add(item);
            }
            phaseBox.SelectedIndex = phaseBox.Items.Count-1;

        }
    }

    struct Config
    {
        bool flag1;
        bool flag2;
        bool flag3;


    }
    

}
