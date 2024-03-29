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

namespace TrackerUI;

public partial class TournamentDashboardForm : Form
{
    List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournament_All();

    public TournamentDashboardForm()
    {
        InitializeComponent();

        WireUpLists();
    }

    private void WireUpLists()
    {
        loadExistingTournamentDropDown.DataSource = null;
        loadExistingTournamentDropDown.DataSource = tournaments;
        loadExistingTournamentDropDown.DisplayMember = "TournamentName";
    }

    private void createTournamentButton_Click(object sender, EventArgs e)
    {
        CreateTournamentForm frm = new();
        frm.Show();
    }

    private void loadTournamentButton_Click(object sender, EventArgs e)
    {
        //Task: Add check on selected model.
        TournamentModel tm = loadExistingTournamentDropDown.SelectedItem as TournamentModel;
        TournamentViewerForm frm = new(tm);
        frm.Show();
    }
}
