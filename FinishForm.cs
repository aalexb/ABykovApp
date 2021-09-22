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
    public partial class FinishForm : System.Windows.Forms.Form
    {
        public int levels = 0;
        public int withnames=0;
        public int poetagno = 0;
        public bool countNewW;
        public bool splitLevel;
        public Phase retPhase;

        public FinishForm(Document doc)
        {
            PhaseArray xcom = doc.Phases;
            //xcom.
            
            
            //PhaseSelector.SelectedItem = xcom;

            InitializeComponent();

            PhaseSelector.DataSource = xcom as IList<Phase>;
            PhaseSelector.DisplayMember = "Name";
            PhaseSelector.ValueMember = "Id";
            foreach (Phase item in xcom)
            {
                PhaseSelector.Items.Add(item);
            }
            PhaseSelector.SelectedIndex = xcom.Size-1;
        }
        public void disFElements()
        {
            SomeLevels.Enabled = false;
            RoomNames.Enabled = false;
            chkSplitLevel.Enabled = false;
            checkBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            levels = SomeLevels.Checked ? 1 : 0;
            withnames = RoomNames.Checked ? 1 : 0;
            splitLevel = chkSplitLevel.Checked;
            countNewW = checkBox1.Checked;
            retPhase = (Phase)PhaseSelector.SelectedItem;
            this.Close();
        }

        private void PhaseSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
