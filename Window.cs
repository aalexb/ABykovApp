using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkApp
{
    public partial class Window : Form
    {
        public List<Cube> MyData { get; set; }
        public Window(List<Cube> data)
        {
            InitializeComponent();
            MyData = data;
            LoadData();
            
            //return "hello";
        }
        public string ReturnValue { get; set; }

        private void Window_Load(object sender, EventArgs e)
        {

        }
        private void LoadData()
        {



            //dataGridView1.Width = win;
            dataGridView1.DataSource = MyData;
            dataGridView1.AutoResizeColumns();
            
        }

        private void cubeBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.ReturnValue=dataGridView1.Rows.Count.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
            //return "Hi";
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
