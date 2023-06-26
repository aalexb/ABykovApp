using System;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public partial class Viewer : System.Windows.Forms.Form, IExternalCommand
    {
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            InitializeComponent();
            Init();
            this.Show();

            return Result.Succeeded;
        }

        void Init()
        {
            var ws = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToList();
            foreach (var item in ws)
            {
                comboBox1.Items.Add(item);
            }
            comboBox1.DisplayMember = "Name";
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var ss = new FilteredElementCollector(doc).WhereElementIsNotElementType().Where(x => x.WorksetId == (comboBox1.SelectedItem as Workset).Id).ToList();
            foreach(var item in ss)
            {
                listBox1.Items.Add(item);
            }
            listBox1.DisplayMember="Name";
        }
    }
}
