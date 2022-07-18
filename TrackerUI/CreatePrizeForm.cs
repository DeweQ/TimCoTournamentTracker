using FluentValidation.Results;
using System.Text;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerLibrary.Validators;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel model = new PrizeModel(
                placeNameValue.Text,
                placeNumberValue.Text,
                prizeAmountValue.Text,
                prizePercentageValue.Text);

            PrizeValidator validator = new();
            ValidationResult result = validator.Validate(model);

            if (!result.IsValid)
            {
                StringBuilder sb = new();
                result.Errors.Select(e => e.ErrorMessage).ToList().ForEach(e => sb.AppendLine(e));
                MessageBox.Show(sb.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            GlobalConfig.Connection.CreatePrize(model);

            callingForm.PrizeComplete(model);
            this.Close();
        }

        private bool ValidateForm()
        {
            bool result = true;

            bool placeNumberIsNumber = int.TryParse(placeNumberValue.Text, out int placeNumber);
            bool placeNumberValidNumber = placeNumber > 0;

            if (!placeNumberIsNumber || !placeNumberValidNumber)
                result = false;

            if (placeNameValue.Text.Length == 0)
                result = false;


            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out decimal prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out double prizePercentage);

            if (!prizeAmountValid || !prizePercentageValid)
                result = false;

            if (prizeAmount <= 0 && prizePercentage <= 0)
                result = false;

            if (prizePercentage < 0 || prizePercentage > 100)
                result = false;

            return result;
        }
    }
}
