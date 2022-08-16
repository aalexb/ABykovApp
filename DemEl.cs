using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkApp
{
    public partial class DemEl : Form
    {
        public DemEl()
        {
            InitializeComponent();
            dataGridView1.DataSource= DemElData.db;

        }
    }
    public class DemElData
    {
        public static List<DemElData> db = new List<DemElData>();
        string Name;

    }
}
