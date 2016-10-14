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

using salesprint;
using System.Text;

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
            metroTabControl1.SelectedTab = tabhome;
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

            //disabling itemname
            txtitemname.Enabled = false;

       
            
           
            
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

                    //subtotal
                    txtsubtotal.Text = Convert.ToDecimal(stuff.unitPrice).ToString();

                    txtitemquantity.Enabled = true;

                }
            }
            
        }

    
        /// <summary>
        ///  actions in this method will execute if the txtitemquantity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtitemquantity_ValueChanged(object sender, EventArgs e)
        {

            if (Convert.ToDecimal(txtitemquantity.Value)>= 1)
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
            if (!string.IsNullOrEmpty(txtitemname.Text))
            {
                if (txtitemname.Text != "Item Name")
                {
                    // fill datagrid values with textbox values
                    datagridforItems.Rows.Add(txtitemname.Text, txtitemquantity.Value, txtunitprice.Text, txtsubtotal.Text);

                    try
                        {
                    // display and add total items bought amount
                    foreach (DataGridViewRow item in datagridforItems.Rows)
                    {
                        if (!string.IsNullOrEmpty(item.Cells[3].ToString()) & txtitemquantity.Value>=1)
                        {
                            
                                
                                var totalSum = (from DataGridViewRow row in datagridforItems.Rows
                                                where row.Cells[3].Value != null
                                                select Convert.ToDecimal(row.Cells[3].Value))
                                                .Sum()
                                                .ToString();
                                txtgross.Text = totalSum;
                           
                            
                        }



                    }
                }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);

                    }

                }
               
            }
            else
            {
                MetroMessageBox.Show(this, "Select an item from the item pane");
            }
           
        }




        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region phonenumbers
            // add a list of phone numbers from the company
            List<string> phone = new List<string>();
            phone.Add("08166307166");
            phone.Add("07035274153");

            List<string> comphone = new List<string>();
             
            
           foreach (var item in phone)
	        {
		        comphone.Add(item);
            }

            #endregion end of phone numbers

           foreach (DataGridViewRow item in datagridforItems.Rows)
           {
               if (!string.IsNullOrEmpty(item.Cells[3].ToString()))
               {
                   var totalSum = (from DataGridViewRow row in datagridforItems.Rows
                                   where row.Cells[3].Value != null
                                   select Convert.ToDecimal(row.Cells[3].FormattedValue)).Sum().ToString();
                   txtgross.Text = totalSum;
               }



           }
           int numberOfItemss = 0;
           if (datagridforItems.Rows[0].Cells[0].Value!=null)
           {
               for (int i = 0; i < datagridforItems.Rows.Count - 1; i++)
               {
                   numberOfItemss = i;
               }
           }

            //items

            List<string> myItemsList = new List<string>();
            if (datagridforItems.Rows[0].Cells[0].Value!=null)
           {
               for (int i = 0; i < datagridforItems.Rows.Count - 1; i++)
               {
                  var s= datagridforItems.Rows[i].Cells[0].Value;
                   myItemsList.Add(s.ToString());
                  
               }
           }

            //prices
            List<string> myprice = new List<string>();
            for (int i = 0; i < datagridforItems.Rows.Count-1; i++)
			{
			 var price = datagridforItems.Rows[i].Cells[2].Value;
                myprice.Add(price.ToString());
			}

            Random rand = new Random();

           Printing p = new Printing("","Company Name",txtsubtotal.Text,rand.Next(12049).ToString(),txtPaynow.Text,txtdiscount.Text,txtExtracharge.Text,myItemsList,comphone,myprice);
                p.print();
            
        }

       
        // save customer information
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
           
            
           
        }

        //report tab
        
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

        private void MainForm_Load(object sender, EventArgs e)
        {

           
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var query = from saleInfo in db.dailySales where saleInfo.date>= dateTimePicker1.Value
                        select saleInfo;
            foreach (var item in query.Take(10))
            {
                metroGrid1.Rows.Add(item.totalSaleToday,item.date);
            }
            



        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            checkout();
        }

        private void checkout()
        {
            Regex checkInput = new Regex("[0-9]");
            #region full payment code starts
            if (rbFullPay.Checked)
            {

                if (checkInput.IsMatch(txtgross.Text))
                {
                    ////check if the extra charge textbox contains ilegal characters
                     //check if the discount textbox contains ilegal characters
                    if (checkInput.IsMatch(txtdiscount.Text))
                    {
                        
                            txtNetTotal.Text = (Convert.ToDecimal(txtgross.Text) - Convert.ToDecimal(txtdiscount.Text)).ToString();
                            txtPaynow.Text = (Convert.ToDecimal(txtNetTotal.Text)).ToString();
                        
                    }
                   // check if the both have ilegal characters
                    else if (checkInput.IsMatch(txtExtracharge.Text))
                    {
                        
                            txtNetTotal.Text = (Convert.ToDecimal(txtgross.Text) + Convert.ToDecimal(txtExtracharge.Text)).ToString();
                            txtPaynow.Text = (Convert.ToDecimal(txtNetTotal.Text)).ToString();
                        
                    }
                    
                        //check if both extrachage and discount have invalid characters
                    else if (checkInput.IsMatch(txtExtracharge.Text) && checkInput.IsMatch(txtdiscount.Text))
                    {
                       
                            txtNetTotal.Text = (Convert.ToDecimal(txtgross.Text) + Convert.ToDecimal(txtExtracharge.Text) - Convert.ToDecimal(txtdiscount.Text)).ToString();
                            txtPaynow.Text = (Convert.ToDecimal(txtNetTotal.Text)).ToString();
                     
                    }
                    else
                    {
                        txtNetTotal.Text = (Convert.ToDecimal(txtgross.Text)).ToString();
                        txtPaynow.Text = txtNetTotal.Text;
                    }
                    
                }
                else
                {
                    MessageBox.Show("You entered invalid characters");

                }
            }
            #endregion fullpayment code ended 
            #region partpayment code starts
            else
            {
             
                if (checkInput.IsMatch(txtPartPayment.Text))
                {
                    MessageBox.Show("hello");
                }
                else
                {
                    MessageBox.Show("You entered invalid characters");
                }
            }
            #endregion part payment code ends


        }

           
}
}
     

       


