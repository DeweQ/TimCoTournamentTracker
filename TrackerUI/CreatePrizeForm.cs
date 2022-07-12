using TrackerLibrary;
using TrackerLibrary.Models;

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
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNameValue.Text,
                    placeNumberValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);

                callingForm.PrizeComplete(model);
                this.Close();

                //placeNameValue.Text = string.Empty;
                //placeNumberValue.Text = string.Empty;
                //prizeAmountValue.Text = "0";
                //prizePercentageValue.Text = "0";
            }
            else
                MessageBox.Show("This form has invalid information. Please check it and try again.");
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
