using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;


namespace WorkApp
{
    public partial class FinishForm : System.Windows.Forms.Form
    {
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

        void readFromGlobal()
        {
            wTypeBoxes = ((doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter).GetValue() as StringParameterValue).Value.Split('|').ToList();

        }
        void writeToGlobal()
        {
            
        }

        public FinishForm(Document Doc)
        {
            this.doc = Doc;
            if (GlobalParametersManager.FindByName(doc, "FinData") != ElementId.InvalidElementId)
            {
                GlobalParameter GlobePar2 = doc.GetElement(GlobalParametersManager.FindByName(doc, "FinData")) as GlobalParameter;

                StringParameterValue strPar =GlobePar2.GetValue() as StringParameterValue;
                String GPar = strPar.Value;
                wTypeBoxes = GPar.Split('|').ToList();
            }
            
            PhaseArray xcom = doc.Phases;
            List<Element> walltypes1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToList();
            walltypes1 = walltypes1.OrderBy(x => x.Name).ToList();
            List<Element> walltypes2 = walltypes1.Select(x=>x).ToList();
            List<Element> walltypes3 = walltypes1.Select(x => x).ToList();

            InitializeComponent();
            GroupSelector.Enabled = false;
            GroupFloorSelector.Enabled = false;
            comboBox1.DataSource = walltypes1;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Id";
            
            try
            {
                comboBox1.SelectedIndex = walltypes1.IndexOf(walltypes1.First(x => x.Name == wTypeBoxes[0]));
            }
            catch (Exception)
            {

                
            }
            
            
            LocFinSelector.DataSource = walltypes2;
            LocFinSelector.DisplayMember = "Name";
            LocFinSelector.ValueMember = "Id";
            try
            {
                LocFinSelector.SelectedIndex = walltypes2.IndexOf(walltypes2.First(x => x.Name == wTypeBoxes[1]));
            }
            catch (Exception)
            {


            }
            ColumnFinSelector.DataSource = walltypes3;
            ColumnFinSelector.DisplayMember = "Name";
            ColumnFinSelector.ValueMember = "Id";
            try
            {
                ColumnFinSelector.SelectedIndex = walltypes3.IndexOf(walltypes3.First(x => x.Name == wTypeBoxes[2]));
            }
            catch (Exception)
            {


            }

            PhaseSelector.DataSource = xcom as IList<Phase>;
            PhaseSelector.DisplayMember = "Name";
            PhaseSelector.ValueMember = "Id";
            foreach (Phase item in xcom)
            {
                PhaseSelector.Items.Add(item);
            }
            PhaseSelector.SelectedIndex = xcom.Size-1;
            var roomGroup = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                 .WhereElementIsNotElementType()
                 .ToElements();
            try
            {
                var lll = roomGroup.Select(x => x.LookupParameter("ADSK_Группирование").AsString()).Distinct().ToList();
                foreach (var item in lll)
                {
                    if (item != null)
                    {
                        GroupSelector.Items.Add(item);
                    }

                }
                //GroupSelector.DataSource = lll;
            }
            catch (Exception)
            {

                
            }
            try
            {
                var lll = roomGroup.Select(x => x.LookupParameter("AG_Групп_Пол").AsString()).Distinct().ToList();
                foreach (var item in lll)
                {
                    if (item != null)
                    {
                        GroupFloorSelector.Items.Add(item);
                    }

                }
                //GroupSelector.DataSource = lll;
            }
            catch (Exception)
            {

                
            }
            
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
            SelNum.Text = a;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            levels = SomeLevels.Checked ? 1 : 0;
            withnames = RoomNames.Checked ? 1 : 0;
            ColFromMat = checkCol.Checked;
            splitLevel = chkSplitLevel.Checked;
            countNewW = checkBox1.Checked;
            retPhase = (Phase)PhaseSelector.SelectedItem;
            WallType = (Element)comboBox1.SelectedItem;
            ColType = (Element)ColumnFinSelector.SelectedItem;
            LocType = (Element)LocFinSelector.SelectedItem;
            wTypeBoxes = new List<string>();
            wTypeBoxes.Add((comboBox1.SelectedItem as Element).Name);
            wTypeBoxes.Add((LocFinSelector.SelectedItem as Element).Name);
            wTypeBoxes.Add((ColumnFinSelector.SelectedItem as Element).Name);
            groupCheck = checkGroup.Checked;
            groupFloorCheck = checkFloorGroup.Checked;
            locWallString = locWallParam.Text;
            if (groupCheck)
            {
                groupField = GroupSelector.Text;
            }
            if (groupFloorCheck)
            {
                groupFloorField = GroupFloorSelector.Text;
            }
            this.Close();
        }

        private void PhaseSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void checkGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkGroup.Checked == false)
            {
                GroupSelector.Enabled = false;
            }
            else
            {
                GroupSelector.Enabled = true;
            }
        }

        private void checkFloorGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkFloorGroup.Checked == false)
            {
                GroupFloorSelector.Enabled = false;
            }
            else
            {
                GroupFloorSelector.Enabled = true;
            }
        }

        private void GroupSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SelNum.Text=roo GroupSelector.Text;
        }
    }
}
