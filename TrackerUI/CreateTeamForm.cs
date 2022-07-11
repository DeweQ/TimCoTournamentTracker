﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        public CreateTeamForm()
        {
            InitializeComponent();
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new();

                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAdress = emailAdressValue.Text;
                p.CellphoneNumber = cellphoneValue.Text;

                GlobalConfig.Connection.CreatePerson(p);

                firstNameValue.Text = string.Empty;
                lastNameValue.Text = string.Empty;
                emailAdressValue.Text = string.Empty;
                cellphoneValue.Text = string.Empty;
            }
            else MessageBox.Show("You need to fill in all of the fields.");
        }

        private bool ValidateForm()
        {
            //TODO: amp validation
            bool result = true;

            if (firstNameValue.Text.Length == 0) result = false;

            if (lastNameValue.Text.Length == 0) result = false; 

            if (emailAdressLabel.Text.Length == 0) result = false;

            if (cellphoneLabel.Text.Length == 0) result = false; 

            return result;
        }
    }
}
