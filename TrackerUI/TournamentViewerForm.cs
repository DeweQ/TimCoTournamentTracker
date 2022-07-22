using System.ComponentModel;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        BindingList<int> rounds = new();
        BindingList<MatchupModel> selectedMatchups = new();

        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();
            tournament = tournamentModel;

            tournament.OnTournamentComplete += (s, e) => this.Close();

            selectedMatchups.ListChanged += matchupListBox_SelectedIndexChanged;
            LoadFromData();

            LoadRounds();
            WireUpRounds();
            WireUpMatchups();

        }

        private void LoadFromData()
        {
            tournamentName.Text = tournament.TournamentName;
        }

        private void LoadRounds()
        {
            rounds.Clear();
            Enumerable.Range(1, tournament.Rounds.Count).ToList().ForEach(x => rounds.Add(x));
            //WireUpRounds();
        }

        private void WireUpRounds()
        {
            roundDropDown.DataSource = rounds;
        }

        private void WireUpMatchups()
        {
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;
            selectedMatchups.Clear();
            tournament.Rounds.First(e => e.First().MatchupRound == round).ToList().ForEach(x =>
            {
                if (x.Winner == null || !unplayedOnlyCheckBox.Checked)
                {
                    selectedMatchups.Add(x);
                }
            });
            WireUpMatchups();
            LoadMatchup();
            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);

            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;

            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;

            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup();
        }

        private void LoadMatchup()
        {
            teamOneScoreValue.Text = String.Empty;
            teamTwoScoreValue.Text = String.Empty;
            teamOneName.Text = "Not Yet Set";
            teamTwoName.Text = "Not Yet Set";
            MatchupModel matchup = (MatchupModel)matchupListBox.SelectedItem;
            if (matchup?.Entries.Count > 0 && matchup.Entries[0].TeamCompeting is not null)
            {
                teamOneName.Text = matchup.Entries[0].TeamCompeting.TeamName;
                teamOneScoreValue.Text = matchup.Entries[0].Score.ToString();
                teamTwoName.Text = "<bye>";
            }
            if (matchup?.Entries.Count > 1)
            {
                teamTwoName.Text = "Net Yet Set";
                if (matchup.Entries[1].TeamCompeting is not null)
                {
                    teamTwoName.Text = matchup.Entries[1].TeamCompeting?.TeamName;
                    teamTwoScoreValue.Text = matchup.Entries[1].Score.ToString();
                }
            }
        }

        private bool ProcessScores()
        {
            double teamOneScore = 0;
            double teamTwoScore = 0;
            MatchupModel matchup = (MatchupModel)matchupListBox.SelectedItem;
            if (matchup?.Entries.Count > 0 && matchup.Entries[0].TeamCompeting is not null)
            {
                if (ValidateScore(teamOneScoreValue.Text))
                {
                    teamOneScore = double.Parse(teamOneScoreValue.Text);
                    matchup.Entries[0].Score = double.Parse(teamOneScoreValue.Text);
                }
                else
                {
                    MessageBox.Show($"Enter a valid score for team {teamOneName.Text}");
                    return false;
                }
            }
            if (matchup?.Entries.Count > 1 && matchup.Entries[1].TeamCompeting is not null)
            {
                if (ValidateScore(teamTwoScoreValue.Text))
                {
                    teamTwoScore = double.Parse(teamTwoScoreValue.Text);
                    matchup.Entries[1].Score = double.Parse(teamTwoScoreValue.Text); 
                }
                else
                {
                    MessageBox.Show($"Enter a valid score for team {teamOneName.Text}");
                    return false;
                }
            }
            if (teamOneScore == teamTwoScore && matchup?.Entries.Count > 0)
            {
                MessageBox.Show("Matchup can't end with a tie");
                return false;
            }
            return true;
        }


        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel matchup = (MatchupModel)matchupListBox.SelectedItem;

            if (matchup.Entries.Any(e => e.TeamCompeting == null))
            {
                MessageBox.Show("Cant score this match yet.");
                return;
            }
            //Validate inputs?

            //Add inputs to the model (matchupEntries)
            bool scoresAreValid = ProcessScores();

            if (!scoresAreValid)
                return;

            try
            {
                TournamentLogic.UpdateTournamentResults(tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application had the folllowing error: {ex.Message}");
                return;
            }

            LoadMatchups();
        }

        private bool ValidateScore(string score)
        {
            bool result = true;
            result = double.TryParse(score, out double parsed);
            return result;
        }

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }
    }
}