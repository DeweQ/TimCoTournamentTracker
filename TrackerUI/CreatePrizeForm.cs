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
            if(!NumberFieldsAreActualNumbers())
            {
                MessageBox.Show("'Prize Amount', 'Prize Percentage': must be numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrizeModel model = new PrizeModel(
                placeNameValue.Text,
                placeNumberValue.Text,
                prizeAmountValue.Text,
                prizePercentageValue.Text);

            PrizeValidator validator = new();
            ValidationResult result = validator.Validate(model);

            if (!result.IsValid)
            {
                ShowErrors(result);
                return;
            }

            GlobalConfig.Connection.CreatePrize(model);

            callingForm.PrizeComplete(model);
            this.Close();
        }

        private static void ShowErrors(ValidationResult result)
        {
            StringBuilder sb = new();
            result.Errors.Select(e => e.ErrorMessage).ToList().ForEach(e => sb.AppendLine(e));
            MessageBox.Show(sb.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool NumberFieldsAreActualNumbers()
        {
            bool result = true;

            if (!double.TryParse(prizePercentageValue.Text, out double p)) result = false;

            if (!decimal.TryParse(prizeAmountValue.Text, out decimal a)) result = false;

            return result;
        }
    }
}
