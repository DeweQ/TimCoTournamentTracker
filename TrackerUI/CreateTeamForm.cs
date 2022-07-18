using FluentValidation.Results;
using System;
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
using TrackerLibrary.Validators;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        //TODO: add validation for TeamName/teamMembers for createTeamButton_Click
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new();
        private ITeamRequester teamRequester;

        public CreateTeamForm(ITeamRequester teamRequester)
        {
            InitializeComponent();

            //CreateSampleData();
            this.teamRequester = teamRequester;

            WireUpLists();
        }

        private void CreateSampleData()
        {

        }

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
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

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = new();

            p.FirstName = firstNameValue.Text;
            p.LastName = lastNameValue.Text;
            p.EmailAddress = emailAdressValue.Text;
            p.CellphoneNumber = cellphoneValue.Text;

            PersonValidator validator = new();

            ValidationResult result = validator.Validate(p);

            if (!result.IsValid)
            {
                StringBuilder sb = new();
                result.Errors.Select(e => e.ErrorMessage).ToList().ForEach(e => sb.AppendLine(e));
                MessageBox.Show(sb.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GlobalConfig.Connection.CreatePerson(p);

            selectedTeamMembers.Add(p);

            WireUpLists();

            firstNameValue.Text = string.Empty;
            lastNameValue.Text = string.Empty;
            emailAdressValue.Text = string.Empty;
            cellphoneValue.Text = string.Empty;
        }

        private void addTeamMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);

                WireUpLists();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                WireUpLists();
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new();

            t.TeamName = TeamNameValue.Text;
            t.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(t);

            teamRequester.TeamComplete(t);

            this.Close();
        }
    }
}
