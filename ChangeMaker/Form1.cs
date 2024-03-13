using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeMaker
{
    //=====================================================================================
    //
    //  Application Name    :   Change Maker
    //
    //  Developer           :   Charles Gionet
    //                          NBCC Miramichi
    //
    //  Synopsis            :   This Application keeps track of multiple denominations of currency for both a hypothetical
    //                          Customer and a Cashier. The application will calculate the change to be given to the customer
    //
    //          Date                    Developer               Comments
    //          ====                    =========               ========
    //          Mar 1 2024             C. Gionet                Assignment #1
    //
    //======================================================================================
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        // class-level variable to avoid hacky nonsense involving displaying the exact denominations given to customer
        string changeGiven = "";


        //=====================================================================================
        //
        //  Function Name    :   btnProceed_Click
        //
        //  Developer           :   Charles Gionet
        //                          NBCC Miramichi
        //
        //  Synopsis            :   Runs through the steps of the transaction when the user clicks the proceed button
        //                          
        //
        //          Date                    Developer               Comments
        //          ====                    =========               ========
        //          Mar 1 2024             C. Gionet                tied to a Button in form
        //
        //======================================================================================
        private void btnProceed_Click(object sender, EventArgs e)
        {
            // make sure the user entered a valid number
            bool isValid = ValidateSaleAmount();
            // sorta hacky crash prevention, in case the user enters an invalid data type
            if (!isValid)
            {
                return;
            }
            // Instantiate the currency class and declare the variables
            Currency currency = new Currency();
            // Variables
            decimal customerTender = 0m;
            decimal saleAmount = Convert.ToDecimal(txtSaleAmount.Text);
            decimal changeOwed = 0m; 
            // NumericUpDown array to store the customer's tender
            NumericUpDown[] customernumUpDowns = { cusTwenty, cusTen, cusFive, cusTwo, cusOne, cusQuarter, cusDime, cusNickel, };
            
            

            // iterate through the customer's tender and add the value of each denomination to the customer's tender
            if (isValid)
            {
               
                // iterate through the customer's tender and add the value of each denomination to the customer's tender
                for (int index = 0; index < currency.denominations.Length; index++)
                {
                    customerTender += currency.denominations[index] * customernumUpDowns[index].Value;
                }

                

                // compare the customer tender to the sale price
                // if the customer's tender is less than the sale price, deny the transaction
                if (customerTender < Convert.ToDecimal(txtSaleAmount.Text))
                {
                    MessageBox.Show("Customer has insufficient funds to complete Transaction", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // If the customer can afford the sale price, calculate the change owed if any
                else
                {
                    // call the CalculateChange  and pass the sale amount and the customer's tender to it
                    changeOwed = CalculateChange(saleAmount, customerTender);
                    // if the change owed is greater than zero, display the change owed
                    if (changeOwed > 0)
                    {
                        // display a message box listing the change owed and the denominations given to the customer
                        MessageBox.Show("Change owed: " + changeOwed.ToString("C") + "\n " + changeGiven
                         , "Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    // if the customer paid the flat amount, no change is owed
                    else if (changeOwed != -1)
                    {
                        MessageBox.Show("No change owed", "Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
       
            
        }


        //=====================================================================================
        //
        //  Function Name    :   ValidateSaleAmount
        //
        //  Developer           :   Charles Gionet
        //                          NBCC Miramichi
        //
        //  Synopsis            :   Runs through the steps of the transaction when the user clicks the proceed button
        //                          
        //
        //          Date                    Developer               Comments
        //          ====                    =========               ========
        //          Mar 1 2024             C. Gionet                
        //
        //======================================================================================
        private bool ValidateSaleAmount()
        {
            // declare a bool for the validity of the user input
            bool isValid = true;

            // error message if the user enters a negative number
            if (decimal.TryParse(txtSaleAmount.Text, out decimal saleTotal) && saleTotal <= 0 )
            {
                MessageBox.Show("Sale amount must be greater than zero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            // error message if the user enters an invalid datatype or leaves the input blank
            else
            if (!decimal.TryParse(txtSaleAmount.Text, out decimal saleAmount))
            {
                MessageBox.Show("Please enter a valid sale amount", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            // return the validity of the user input to the function caller
            return isValid;
        }


        //=====================================================================================
        //
        //  Function Name    :   CalculateChange
        //
        //  Developer           :   Charles Gionet
        //                          NBCC Miramichi
        //
        //  Synopsis            :   This function calculates the change owed to the customer
        //                          
        //
        //          Date                    Developer               Comments
        //          ====                    =========               ========
        //          Mar 1 2024             C. Gionet                
        //
        //======================================================================================
        private decimal CalculateChange(decimal saleAmount, decimal customerTender)
        {
             // reset the class level variable, to avoid issues with past values being passed twice.
             changeGiven = "";
            // variables and instantiation of the Currency class
            NumericUpDown[] cashierNumericUpdowns = { cashTwenty, cashTens, cashFives, cashTwos, cashOnes, cashQuarter, cashDimes, cashNickels, };
            Currency currency = new Currency();

            // Change calculation
            decimal change = customerTender - saleAmount;
            // this iteration aims to fix the issue of my denominations being calculated incorrectly, my order of operations was fudged
            // which caused the program to give the wrong amount of change, it did not iterate through the whole array.
            change = currency.RoundToNearest5Cents(change);
            decimal changeOwed = change;

            decimal cashierFloatSum = 0m;
            // iterate through every numupdown the cashier has to determine the sum of money in the float
            for (int index = 0; index < currency.denominations.Length; index++)
            {
                // add the count of denominations on hand for each member of the array and multiply it by the value of
                // the denomination
                cashierFloatSum += currency.denominations[index] * cashierNumericUpdowns[index].Value;

            }
            debugLabel2.Text = cashierFloatSum.ToString();

            
            // iterate through every numericUpDown of the cashier float groupbox to find 
            for (int index = 0; index < currency.denominations.Length; index++)
            {
                while (change >= currency.denominations[index] && cashierNumericUpdowns[index].Value > 0)
                {
                    // the idea of this statement is to calculate how many coins or bills of a certain denomination 
                    // we can give to the customer without going over the amount of change owed. Math.Floor rounds it down.
                    decimal denominationCount = Math.Min(Math.Floor(change / currency.denominations[index]), cashierNumericUpdowns[index].Value);
                    if (denominationCount > 0)
                    {
                        // add the denomination and the amount of change we're giving in that denomination to the string
                        changeGiven += denominationCount + " x " + currency.denominations[index].ToString("C") + "\n";
                    }
                    // indicate the amount of change we're giving in the current denomination
                    cashierNumericUpdowns[index].Value -= denominationCount;
                    // subtract the amount of change we're giving in the current demonination from the total change
                    change -= denominationCount * currency.denominations[index];
                }
                if (change >= currency.denominations[index])
                {
                    // display a message box and an error message and return nothing to the function caller
                    MessageBox.Show("Cashier has insufficient funds to complete transaction", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
            }
            // return the amount of change owed to the customer to function caller
              changeOwed = currency.RoundToNearest5Cents(changeOwed);
              return changeOwed;
            
          
        }



        //=====================================================================================
        //
        //  Function Name    :   btnReset_Click
        //
        //  Developer           :   Charles Gionet
        //                          NBCC Miramichi
        //
        //  Synopsis            :   Resets the form to its default state
        //                          
        //
        //          Date                    Developer               Comments
        //          ====                    =========               ========
        //          Mar 1 2024             C. Gionet                
        //
        //======================================================================================
        private void btnReset_Click(object sender, EventArgs e)
        {
            NumericUpDown[] customerupDowns = { cusTwenty, cusTen, cusFive, cusTwo, cusOne, cusQuarter, cusDime, cusNickel, };
            NumericUpDown[] cashierupDowns = { cashTwenty, cashTens, cashFives, cashTwos, cashOnes, cashQuarter, cashDimes, cashNickels, };
            foreach (NumericUpDown num in customerupDowns)
            {
                num.Value = 0;
            }
            foreach (NumericUpDown num in cashierupDowns)
            {
                num.Value = 0;
            }
        }
    }
}


// currency class
public class Currency
{
    // array of the exact valur of each denomination
    public decimal[] denominations = { 20.00m, 10.00m, 5.00m, 2.00m, 1.00m, 0.25m, 0.10m, 0.05m,};
    // currency rounding according the canadian penny rounding standards
    public decimal RoundToNearest5Cents(decimal amount)
    {
        return Math.Round(amount * 20) / 20;
    }
}
