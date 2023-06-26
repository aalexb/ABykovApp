using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using WorkApp.Отделка;

namespace WorkApp
{
    
    public partial class FinishForm : System.Windows.Forms.Form
    {
        public StyleByFamily style_by;
        public CheckBoxes check;
        public ParamFields parnames;


        public int levels = 0;
        public int withnames = 0;
        public int poetagno = 0;
        public bool countNewW=false;
        public bool groupCheck = false;
        public bool groupFloorCheck = false;
        public bool splitLevel;
        public string locWallString;
        public Phase retPhase;
        public Element WallType;
        public Element ColType;
        public bool ColFromMat;
        public string groupField;
        public string groupFloorField;
        public Element LocType;
        List<ElementId> defSet = new List<ElementId>();
        public List<string> wTypeBoxes = new List<string>();
        Document doc;
        RevitContext rC;
        Context C;

        void readFromGlobal()
        {
            wTypeBoxes = ((doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter).GetValue() as StringParameterValue).Value.Split('|').ToList();

        }
        void writeToGlobal()
        {
            
        }

        public FinishForm(RevitContext rc, Context context)
        {
            rC= rc;
            this.doc = rC.doc;
            C=context;
            if (GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId)
            {
                GlobalParameter GlobePar2 = doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter;

                StringParameterValue strPar =GlobePar2.GetValue() as StringParameterValue;
                String GPar = strPar.Value;
                wTypeBoxes = GPar.Split('|').ToList();
            }
            
            PhaseArray doc_phases = doc.Phases;
            List<Element> walltypes1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToList();
            walltypes1 = walltypes1.OrderBy(x => x.Name).ToList();
            List<Element> walltypes2 = walltypes1.Select(x=>x).ToList();
            List<Element> walltypes3 = walltypes1.Select(x => x).ToList();

            InitializeComponent();
            checkGroup(null,null);
            MainFinSelector.DataSource = walltypes1;
            MainFinSelector.DisplayMember = "Name";
            MainFinSelector.ValueMember = "Id";

            try
            {
                MainFinSelector.SelectedIndex = walltypes1.IndexOf(walltypes1.First(x => x.Name == wTypeBoxes[0]));
            }
            catch (Exception) { }
            LocFinSelector.DataSource = walltypes2;
            LocFinSelector.DisplayMember = "Name";
            LocFinSelector.ValueMember = "Id";
            try
            {
                LocFinSelector.SelectedIndex = walltypes2.IndexOf(walltypes2.First(x => x.Name == wTypeBoxes[1]));
            }
            catch (Exception) { }
            ColumnFinSelector.DataSource = walltypes3;
            ColumnFinSelector.DisplayMember = "Name";
            ColumnFinSelector.ValueMember = "Id";
            try
            {
                ColumnFinSelector.SelectedIndex = walltypes3.IndexOf(walltypes3.First(x => x.Name == wTypeBoxes[2]));
            }
            catch (Exception) { }
            PhaseSelector.DataSource = doc_phases as IList<Phase>;
            PhaseSelector.DisplayMember = "Name";
            PhaseSelector.ValueMember = "Id";
            foreach (Phase item in doc_phases)
            {
                PhaseSelector.Items.Add(item);
            }
            PhaseSelector.SelectedIndex = doc_phases.Size-1;
            var roomGroup = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                 .WhereElementIsNotElementType()
                 .ToElements();            
        }
        public void disableSomeElements(string who)
        {
            SomeLevels.Enabled = false;
            RoomNames.Enabled = false;
            chkSplitLevel.Enabled = false;
            checkBox1.Enabled = false;
            if (who=="New")
            {
                LocFinSelector.Enabled = false;
                ColumnFinSelector.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkCol.Enabled = false;
            }
        }

        public void selElem(string a)
        {
            out_shed_name.Text = a;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            style_by = new StyleByFamily{
                Name = (comboBox1.SelectedItem as Family).Name,
                ceil=textBox9.Text,
                wall=textBox8.Text,
                floor=textBox10.Text,
                kolon=textBox11.Text,
                plintus=textBox12.Text,
                other=textBox13.Text,
            };
            check = new CheckBoxes{
                room_names=RoomNames.Checked,
                simple_names=checkBox4.Checked,
                recon=checkBox1.Checked,
                some_levels=SomeLevels.Checked,
                diff_levels=chkSplitLevel.Checked,
                grouped=checkBox5.Checked,
                main_type=checkBox2.Checked,
                local_type=checkBox3.Checked,
                kolon_type=checkCol.Checked,
                finish_mode=FinishMode.Checked,
            };
            parnames = new ParamFields
            {
                group_finish = finish_group_field.Text,
                group_floor = floor_group_field.Text,
                prefix_finish = textBox1.Text,
                prefix_floor = textBox2.Text,
                sostav = textBox5.Text,
                sostav_ceil = ceil_sostav_field.Text,
                wall_func = wall_func_field.Text,
                note_finish = textBox6.Text,
                note_floor = textBox7.Text,
                endnote_finish = textBox3.Text,
                endnote_floor = textBox4.Text,
                floor_mark = textBox14.Text,
            };
            */

            levels = SomeLevels.Checked ? 1 : 0;
            withnames = RoomNames.Checked ? 1 : 0;
            ColFromMat = checkCol.Checked;
            splitLevel = chkSplitLevel.Checked;
            countNewW = checkBox1.Checked;
            rC.selected_phase= (Phase)PhaseSelector.SelectedItem;
            retPhase = (Phase)PhaseSelector.SelectedItem;
            WallType = (Element)MainFinSelector.SelectedItem;
            ColType = (Element)ColumnFinSelector.SelectedItem;
            LocType = (Element)LocFinSelector.SelectedItem;
            wTypeBoxes = new List<string>
            {
                (MainFinSelector.SelectedItem as Element).Name,
                (LocFinSelector.SelectedItem as Element).Name,
                (ColumnFinSelector.SelectedItem as Element).Name
            };
            groupCheck = FinishMode.Checked;
            groupFloorCheck = FloorMode.Checked;
            this.Close();
        }
        private void checkGroup(object sender, EventArgs e)
        {
            finish_group_field.Enabled = checkBox5.Checked && FinishMode.Checked;
            textBox1.Enabled = FinishMode.Checked;
            floor_group_field.Enabled = checkBox5.Checked && FloorMode.Checked;
            textBox2.Enabled = FloorMode.Checked;
        }

        private void checkFinComboBox(object sender, EventArgs e)
        {
            MainFinSelector.Enabled = !checkBox2.Checked;
            LocFinSelector.Enabled = !checkBox3.Checked;
            ColumnFinSelector.Enabled = !checkCol.Checked;
        }

        private void SomeLevels_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FinishForm_Load(object sender, EventArgs e)
        {

        }

        private void wall_func_field_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}
