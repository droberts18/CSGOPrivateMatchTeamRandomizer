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

namespace PrivateMatchTeamRandomizer
{
    /// <summary>
    /// Interaction logic for PlayerSelection.xaml
    /// </summary>
    public partial class PlayerSelection : Page
    {
        MainWindow m;
        public PlayerSelection(MainWindow m)
        {
            InitializeComponent();
            this.m = m;
            DataObject.AddPastingHandler(newPlayerName, OnCancelCommand);
            DataObject.AddCopyingHandler(newPlayerName, OnCancelCommand);
            loadPlayersFromTxt();
        }

        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }

        private void loadPlayersFromTxt()
        {
            try
            {
                if (!File.Exists("players.txt"))
                    File.Create("players.txt");

                using(StreamReader sr = new StreamReader("players.txt", true))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line[0] == '-') break;
                        storedPlayers.Items.Add(line);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void savePlayerToTxt(string p) {
            var txtLines = File.ReadAllLines("players.txt").ToList();
            txtLines.Insert(txtLines.Count, p);
            File.WriteAllLines("players.txt", txtLines);
        }

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] playersList = new string[activePlayers.Items.Count];
            for (int i = 0; i < playersList.Length; i++)
                playersList[i] = (string)activePlayers.Items.GetItemAt(i);     
            m.Content = new RandomizedResults(m, playersList);
        }

        private void NameInputOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(newPlayerName.Text))
                    addStoredPlayer();

            if (((e.Key < Key.D0) || (e.Key > Key.D9)) && ((e.Key < Key.NumPad0) || (e.Key > Key.NumPad9)) && ((e.Key < Key.A) || (e.Key > Key.Z)))
            {
                if((e.Key == Key.Back || e.Key == Key.Delete) & string.IsNullOrWhiteSpace(newPlayerName.Text))
                    addNewPlayerBtn.IsEnabled = false;
                e.Handled = true;
            }
            else
                addNewPlayerBtn.IsEnabled = true;
        }

        private void addStoredPlayer()
        {
            Player p = new Player(newPlayerName.Text);
            foreach(string s in storedPlayers.Items)
            {
                if (s == newPlayerName.Text)
                {
                    MessageBox.Show("Failed to add player: There is already a stored player with the same name.");
                    newPlayerName.Clear();
                    return;
                }
            }
            savePlayerToTxt(p.name);
            storedPlayers.Items.Add(p.name);
            newPlayerName.Clear();
            addNewPlayerBtn.IsEnabled = false;
        }

        private void addPlayerBtnClick(object sender, RoutedEventArgs e)
        {
            addStoredPlayer();
        }

        private void movePlayerFromListBox(ListBox source, ListBox destination, string s)
        {
            destination.Items.Add(s);
            source.Items.Remove(s);
        }

        private void movePlayersBtnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < storedPlayers.SelectedItems.Count;)
            {
                movePlayerFromListBox(storedPlayers, activePlayers, storedPlayers.SelectedItems[i].ToString());
            } while (storedPlayers.SelectedItems.Count > 0) ;
            storedPlayers.SelectedIndex = -1;

            if (activePlayers.Items.Count > 1)
                doneBtn.IsEnabled = true;
        }

        private void storedPlayersSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (storedPlayers.SelectedItem != null)
                moveStoredPlayersBtn.IsEnabled = true;
            else
                moveStoredPlayersBtn.IsEnabled = false;
        }

        private void removeActivePlayersBtnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < activePlayers.SelectedItems.Count;)
            {
                movePlayerFromListBox(activePlayers, storedPlayers, activePlayers.SelectedItems[i].ToString());
            } while (activePlayers.SelectedItems.Count > 0) ;
            activePlayers.SelectedIndex = -1;

            if (activePlayers.Items.Count < 1)
                doneBtn.IsEnabled = false;
        }

        private void activePlayersOnSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (activePlayers.SelectedItem != null)
                removeActivePlayersBtn.IsEnabled = true;
            else
                removeActivePlayersBtn.IsEnabled = false;
        }
    }
}
