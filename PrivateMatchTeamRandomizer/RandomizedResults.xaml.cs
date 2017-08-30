using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PrivateMatchTeamRandomizer
{
    /// <summary>
    /// Interaction logic for RandomizedResults.xaml
    /// </summary>
    public partial class RandomizedResults : Page
    {
        private MainWindow m;
        private string[] players;
        private static Random rng;
        private string[] tPistols;
        private string[] ctPistols;

        public RandomizedResults(MainWindow m, string[] players)
        {
            InitializeComponent();
            this.m = m;
            this.players = players;
            rng = new Random();
            tPistols = new string[] { "Glock-18", "P250", "Desert Eagle", "R8 Revolver", "Dual Berettas", "CZ75-Auto", "Tec-9" };
            ctPistols = new string[] { "USP-S", "P2000", "CZ75-Auto", "P250", "Desert Eagle", "R8 Revolver", "Dual Berettas", "Five-SeveN" };

            randomize();
        }

        private void randomizeAgainBtn_Click(object sender, RoutedEventArgs e)
        {
            teamOne.Items.Clear();
            teamTwo.Items.Clear();
            randomize();
        }

        private void shuffleList(string[] s)
        {
            int n = s.Length;
            while(n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string temp = s[k];
                s[k] = s[n];
                s[n] = temp;
            }
        }

        private void randomize()
        {
            randomizePlayers();
            randomizeMap();
            randomizeTeamNames();
            randomizePistols();
        }

        private void randomizePlayers()
        {
            // random players
            shuffleList(players);

            // bool to decide which team a player goes to
            bool team = rng.NextDouble() >= 0.5;
            foreach (string p in players)
            {
                if (team)
                {
                    teamOne.Items.Add(p);
                    team = false;
                }
                else
                {
                    teamTwo.Items.Add(p);
                    team = true;
                }
            }
        }

        private void randomizeMap()
        {
            var maps = new List<string>[4];

            try {
                int currentMapIndex = -1;

                using(StreamReader sr = new StreamReader("maps.txt"))
                {
                    string line;
                    while((line = sr.ReadLine()).Substring(0, 2) != "--")
                    {
                        if (line[0] == '-')
                        {
                            currentMapIndex++;
                            maps[currentMapIndex] = new List<string>();
                        }
                        else
                            maps[currentMapIndex].Add(line);
                    }
                }

                List<string> selectedMaps = new List<string>();
                if (activeDutyCheckbox.IsChecked ?? true)
                    selectedMaps.AddRange(maps[0]);
                if (reserveDutyCheckbox.IsChecked ?? false)
                    selectedMaps.AddRange(maps[1]);
                if (operationCheckbox.IsChecked ?? false)
                    selectedMaps.AddRange(maps[2]);
                if (workshopCheckbox.IsChecked ?? false)
                    selectedMaps.AddRange(maps[3]);

                int m = rng.Next(0, selectedMaps.Count);
                mapLbl.Content = selectedMaps[m].ToUpper();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void randomizeTeamNames()
        {
            List<string> teamNames = File.ReadAllLines("teamNames.txt").ToList();
            int t1 = rng.Next(0, teamNames.Count);
            int t2;
            do
            {
                t2 = rng.Next(0, teamNames.Count);
            } while (t2 == t1);

            teamOneName.Content = "Team " + teamNames[t1];
            teamTwoName.Content = "Team " + teamNames[t2];
        }

        private void randomizePistols()
        {
            
        }

        private void changePlayers_Click(object sender, RoutedEventArgs e)
        {
            m.Content = new PlayerSelection(m);
        }
    }
}