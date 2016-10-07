using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

// other libraries
using MaterialSkin.Controls;
using MaterialSkin;
using MetroFramework;
using Eze.Printing;

namespace MrSales_Manager
{
    public partial class MainForm : MaterialForm
    {

        DataClasses1DataContext db = new DataClasses1DataContext();
        SqlConnection sql = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename="+@"C:\Users\ezesunday\Documents\Visual Studio 2012\Projects\MrSales Manager\MrSales Manager\mrsalesdbv1.mdf"+";Integrated Security=True;Connect Timeout=30");

        public MainForm()
        {
            InitializeComponent();
            //
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan800, Primary.Pink600, Primary.Pink600, Accent.Pink100, TextShade.BLACK);

            DateTime date = DateTime.Now;
            lblDate.Text = date.Day.ToString() +"-" + date.Month.ToString() + "-" + date.Year.ToString();
            // autosuggest
            autosugestCustomerName();
            autosugest();
            //buttons
           
            //tab

            //toggle
            toggleAvailable.Enabled = false;

            // disabling the id field
            txtItemId.Enabled = false;
            // disabling  the quantity textbox
            txtitemquantity.Enabled = false;
            
            
           
            
        }

        private void metroLabel2_Click(object sender, EventArgs e)
        {

        }

        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPartPay.Checked)
            {
                txtPartPayment.Enabled = true;
            }
            else
            {
                txtPartPayment.Enabled = false;
            }
        }

        public void autosugestCustomerName()
        {
            txtcustomername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtcustomername.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection myautocompleteSource = new AutoCompleteStringCollection();

            var source = from cus in db.customers
                         select cus;
            foreach (var item in source)
            {
                myautocompleteSource.Add(item.customerName);
            }
            txtcustomername.AutoCompleteCustomSource = myautocompleteSource;

        }
        public  void autosugest()
        {
            txtMainItemName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtMainItemName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection data = new AutoCompleteStringCollection();

            // Linq Query version
            var query = from myitems in db.items
                        select myitems;

            foreach (var stuff in query)
            {
                
                data.Add(stuff.name);
            }

            txtMainItemName.AutoCompleteCustomSource = data;
          
            // Sql Version

            //try
            //{
            //    sql.Open();
            //    string search = "select name from items";
            //    SqlCommand command = new SqlCommand(search, sql);
            //    SqlDataReader readdata = command.ExecuteReader();
            //    while (readdata.Read())
            //    {
            //        var rex = readdata.GetString(0).ToString();
            //        data.Add(rex);
            //    }
            //    txtMainItemName.AutoCompleteCustomSource = data;
            //    sql.Close();
            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show(ee.Message.ToString());
            //}
            //finally
            //{
            //    sql.Close();
            //}
           

        }

        private  void txtMainItemName_TextChanged(object sender, EventArgs e)
        {

            var query = from myitems in db.items
                        where myitems.name == txtMainItemName.Text
                        select myitems;
            foreach (var stuff in query)
            {
                if (stuff.qty>0)
                {
                    toggleAvailable.Checked = true;

                    //products instock
                    txtqtyinstock.Text = stuff.qty.ToString();

                    // item id
                    txtItemId.Text = stuff.Id.ToString();

                    //item name
                    txtitemname.Text = stuff.name;

                    //unit  price
                    txtunitprice.Text = stuff.unitPrice;

                    txtitemquantity.Enabled = true;

                }
            }
            
        }

    

        private void txtitemquantity_ValueChanged(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txtitemquantity.Value) > 1)
            {

                decimal sub = Convert.ToDecimal(txtunitprice.Text) * Convert.ToDecimal(txtitemquantity.Value);
                txtsubtotal.Text = sub.ToString();
                if (txtdiscount.Text != null || txtdiscount.Text == "")
                {

                }
                //decimal  gross =  sub +  
                //txtgross.Text  = 


            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (txtitemname.Text!="0" || txtitemname.Text!="")
            {
                datagridforItems.Rows.Add(txtitemname.Text,txtitemquantity.Value,txtunitprice.Text,txtsubtotal.Text);

               
            }

            // display and add total items bought amount
            foreach (DataGridViewRow item in datagridforItems.Rows)
            {
                if (!string.IsNullOrEmpty(item.Cells[3].ToString()))
                {
                    var totalSum = (from DataGridViewRow row in datagridforItems.Rows
                                    where row.Cells[3].Value != null
                                    select Convert.ToDecimal(row.Cells[3].FormattedValue)).Sum().ToString();
                    txtgross.Text=totalSum;
                }
               

                
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // using the printing library
            Printing p = new Printing();
            p.print();
            
        }

       

        private void SaveCustomerEvent(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtcustomername.Text) || txtcustomername.Text=="Name")
            {
                try
                {
                    sql.Open();
                    Random rand = new Random();
                    int id = rand.Next(150, 300);
                    string insert = "insert into customers(customerName,customerId,customerPhone) values('" + txtcustomername.Text + "','" + id + "','" + txtcustomerphonenumber.Text + "')";
                    SqlCommand command = new SqlCommand(insert, sql);
                    command.ExecuteNonQueryAsync();
                    sql.Close();
                    MetroMessageBox.Show(this, "Customer's Information has been Saved Successfully ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, 200);


                }
                catch (Exception ex)
                {

                    MetroMessageBox.Show(this, ex.Message + "We could not save your customers name successfully, call the technical department or activate the database", "database error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 400);
                }

            }
            else
            {
                MetroMessageBox.Show(this,"Customers name can not be empty");
            }
           
            //try
            //{
            //    customer mmcustomer = new customer()
            //    {

            //        customerName = txtcustomername.Text,
            //        customerPhone = txtcustomerphonenumber.Text,
            //        customerId = id
            //    };

            //    db.customers.InsertOnSubmit(mmcustomer);

            //    try
            //    {
            //        db.SubmitChanges();
            //        MessageBox.Show("Saved");
            //    }
            //    catch (Exception ex)
            //    {

            //        MessageBox.Show(ex.Message);
            //    }
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);
            //}
            
            
           
        }

        private void txtcustomername_TextChanged(object sender, EventArgs e)
        {
            var details = from detail in db.customers
                          where detail.customerName==txtcustomername.Text
                          select detail;

            foreach (var item in details)
            {
                txtcustomerphonenumber.Text = item.customerPhone;
                txtdebt.Text = item.customersDebt;
            }
        }

        

     

       
    }
}
