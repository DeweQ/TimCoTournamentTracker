using FluentValidation.Results;
using System.ComponentModel;
using System.Data;
using System.Text;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerLibrary.Validators;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private BindingList<PersonModel> availableTeamMembers = new(GlobalConfig.Connection.GetPerson_All());
        private BindingList<PersonModel> selectedTeamMembers = new();
        private ITeamRequester teamRequester;

        public CreateTeamForm(ITeamRequester teamRequester)
        {
            InitializeComponent();

            this.teamRequester = teamRequester;

            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel person = GeneratePerson();

            PersonValidator validator = new();
            ValidationResult result = validator.Validate(person);
            if (!result.IsValid)
            {
                ShowErrors(result);
                return;
            }

            GlobalConfig.Connection.CreatePerson(person);

            selectedTeamMembers.Add(person);

            ClearForm();
        }

        private PersonModel GeneratePerson()
        {
            PersonModel person = new();

            person.FirstName = firstNameValue.Text;
            person.LastName = lastNameValue.Text;
            person.EmailAddress = emailAdressValue.Text;
            person.CellphoneNumber = cellphoneValue.Text;
            return person;
        }

        private void ClearForm()
        {
            firstNameValue.Text = string.Empty;
            lastNameValue.Text = string.Empty;
            emailAdressValue.Text = string.Empty;
            cellphoneValue.Text = string.Empty;
        }

        private static void ShowErrors(ValidationResult result)
        {
            StringBuilder sb = new();
            result.Errors.Select(e => e.ErrorMessage).ToList().ForEach(e => sb.AppendLine(e));
            MessageBox.Show(sb.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void addTeamMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel Person = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (Person != null)
            {
                availableTeamMembers.Remove(Person);
                selectedTeamMembers.Add(Person);
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel person = (PersonModel)teamMembersListBox.SelectedItem;

            if (person != null)
            {
                selectedTeamMembers.Remove(person);
                availableTeamMembers.Add(person);
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel team = new();
            team.TeamName = TeamNameValue.Text;
            team.TeamMembers = selectedTeamMembers.ToList();

            TeamValidator validator = new();
            ValidationResult result = validator.Validate(team);
            if (!result.IsValid)
            {
                ShowErrors(result);
                return;
            }
            //Save team to database
            GlobalConfig.Connection.CreateTeam(team);

            //Return new team to parent form
            teamRequester.TeamComplete(team);

            this.Close();
        }
    }
}
