using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace WorkApp
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public partial class Params : Form, IExternalCommand
    {
        List<paramData> paramSet = new List<paramData>();
        private BindingSource bindingSource1 = new BindingSource();

        public Params()
        {
            InitializeComponent();
            dataGridView1.DataSource = bindingSource1;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var doc=commandData.Application.ActiveUIDocument.Document;


            
            var map = doc.ParameterBindings.ForwardIterator();
            map.Reset();
            while (map.MoveNext())
            {
                var pp=new paramData();

                pp.Definition = map.Key;
                pp.Name = map.Key.Name;
                pp.Binding=map.Current as ElementBinding;


                paramSet.Add(pp);
            }
            
            this.Show();


            return Result.Succeeded;
        }

        void GetData(string selectCommand)
        {

        }
    }

    public class paramData
    {
        public Definition Definition;
        public string Name;

        public ElementBinding Binding { get; internal set; }
    }
}

