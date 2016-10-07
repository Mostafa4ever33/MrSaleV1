using MaterialSkin.Controls;
using MaterialSkin;
using MetroFramework.Forms;
using MetroFramework;
//basic namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MrSales_Manager
{
    public partial class Home : MaterialForm
    {
        public Home()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan800, Primary.Pink600, Primary.Pink600, Accent.Pink100, TextShade.BLACK);

            // textbox
            txtPartAmount.Hint = "Amount";

            //radio buttons
            metroToggle1.Checked = true;

            //panel

            //time
            
            timer1.Start();
            

        }

        private void paytype(object sender, EventArgs e)
        {
           
           if(rbHalfPayment.Checked)
            {
                txtPartAmount.Enabled = true;
            }
           else
           {
               txtPartAmount.Enabled = false;
           }
        }

        private void datasalesInfomation_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString();
        }

       

       
    }
}
