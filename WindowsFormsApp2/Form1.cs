using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        LSTABINT_SERV.LSTABINTSERVICE service = new LSTABINT_SERV.LSTABINTSERVICE();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            button1.Enabled = false;    
            service.Inicio();
            label1.Text = "El servicio esta funcionando";
        }

    }
}
