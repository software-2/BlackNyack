using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackNyackCS
{
    public struct UICard
    {
        public Button button;
        public int value;
        public UICard(Button button, int value)
        {
            this.button = button;
            this.value = value;
        }
    }

    public enum Owner : ushort
    {
        None = 0,
        Player,
        Nyau,
        ThirdParty
    }



    public partial class Form1 : Form
    {
        static readonly Version currentVersion = new Version(1, 0, 1);

        private static Color PlayerColor = Color.DarkGreen;
        private Color NyauColor = Color.DarkOrange;
        private Color ThirdPartyColor = Color.DarkGray;

        private List<UICard> deck;

        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
            deck = new List<UICard>() { 
                new UICard(heart_ace, 11),
                new UICard(heart_2, 2),
                new UICard(heart_3, 3),
                new UICard(heart_4, 4),
                new UICard(heart_5, 5),
                new UICard(heart_6, 6),
                new UICard(heart_7, 7),
                new UICard(heart_8, 8),
                new UICard(heart_9, 9),
                new UICard(heart_10, 10),
                new UICard(heart_jack, 10),
                new UICard(heart_queen, 10),
                new UICard(heart_king, 10),

                new UICard(club_ace, 11),
                new UICard(club_2, 2),
                new UICard(club_3, 3),
                new UICard(club_4, 4),
                new UICard(club_5, 5),
                new UICard(club_6, 6),
                new UICard(club_7, 7),
                new UICard(club_8, 8),
                new UICard(club_9, 9),
                new UICard(club_10, 10),
                new UICard(club_jack, 10),
                new UICard(club_queen, 10),
                new UICard(club_king, 10),

                new UICard(diamond_ace, 11),
                new UICard(diamond_2, 2),
                new UICard(diamond_3, 3),
                new UICard(diamond_4, 4),
                new UICard(diamond_5, 5),
                new UICard(diamond_6, 6),
                new UICard(diamond_7, 7),
                new UICard(diamond_8, 8),
                new UICard(diamond_9, 9),
                new UICard(diamond_10, 10),
                new UICard(diamond_jack, 10),
                new UICard(diamond_queen, 10),
                new UICard(diamond_king, 10),

                new UICard(spade_ace, 11),
                new UICard(spade_2, 2),
                new UICard(spade_3, 3),
                new UICard(spade_4, 4),
                new UICard(spade_5, 5),
                new UICard(spade_6, 6),
                new UICard(spade_7, 7),
                new UICard(spade_8, 8),
                new UICard(spade_9, 9),
                new UICard(spade_10, 10),
                new UICard(spade_jack, 10),
                new UICard(spade_queen, 10),
                new UICard(spade_king, 10)
            };
        }


        private void GenerateOdds()
        {
            int simulationCount = 10000;

            double lossFromBustFromNextDrawCount = 0;

            double bustIfStayCount = 0;
            double winIfStayCount = 0;
            double loseIfStayCount = 0;
            double drawIfStayCount = 0;

            double stayWinsFrom5CardCount = 0;
            double stayWinsFrom21CardCount = 0;
            double stayWinsFroNormalCount = 0;


            double winsFrom5CardCount = 0;
            double winsFrom21CardCount = 0;
            double winsFromNyauBustCount = 0;
            double winsFromHigherTotalCount = 0;

            double drawCount = 0;

            double lossFrom5CardCount = 0;
            double lossFrom21Count = 0;
            double lossFromBustCount = 0;
            double lossFromHigherTotalCount = 0;

            for (int i = 0; i < simulationCount; ++i)
            {
                List<SimCard> gameDeck = new List<SimCard>();
                List<SimCard> stayDeck = new List<SimCard>();

                foreach (var card in deck)
                {
                    Owner owner = BlackNyackCS.Owner.None;
                    var color = card.button.BackColor;
                    if (color == PlayerColor)
                    {
                        owner = BlackNyackCS.Owner.Player;
                    }
                    else if (color == NyauColor)
                    {
                        owner = BlackNyackCS.Owner.Nyau;
                    }
                    else if (color == ThirdPartyColor)
                    {
                        owner = BlackNyackCS.Owner.ThirdParty;
                    }

                    gameDeck.Add(new SimCard(owner, card.value));
                    stayDeck.Add(new SimCard(owner, card.value));
                }



                //Run the scenario if the player stays, how likely is Nyau to win?
                //Nyau draws until 17. I have no data to model when he goes higher, so consider that an extra bonyus.
                //Stats here are simpler because I don't need to generate stats to that high fidelity
                while (TotalForOwner(stayDeck, BlackNyackCS.Owner.Nyau) < 17 && NumberOfCardsForOwner(stayDeck, BlackNyackCS.Owner.Nyau) < 5)
                {
                    stayDeck[GetDrawableCardIndex(stayDeck)].owner = BlackNyackCS.Owner.Nyau;
                }
                int playerStayTotal = TotalForOwner(stayDeck, BlackNyackCS.Owner.Player);
                int playerStayCount = NumberOfCardsForOwner(stayDeck, BlackNyackCS.Owner.Player);
                int NyauStayTotal = TotalForOwner(stayDeck, BlackNyackCS.Owner.Nyau);
                int NyauStayCount = NumberOfCardsForOwner(stayDeck, BlackNyackCS.Owner.Nyau);
                if (playerStayTotal <=21 && (playerStayTotal > NyauStayTotal || (playerStayCount == 5 && NyauStayCount < 5)))
                {
                    //Simpler stats because I'm lazy
                    winIfStayCount++;
                    if (playerStayCount == 5)
                    {
                        stayWinsFrom5CardCount++;
                    }
                    if (playerStayTotal == 21)
                    {
                        stayWinsFrom21CardCount++;
                    }
                    if (playerStayCount != 5 && playerStayTotal != 21)
                    {
                        stayWinsFroNormalCount++;
                    }

                }
                else if (playerStayTotal <= 21 && (playerStayTotal == NyauStayTotal || (playerStayCount == 5 && NyauStayCount == 5)))
                {
                    drawIfStayCount++;
                }
                else
                {
                    loseIfStayCount++;
                }







                if (TotalForOwner(gameDeck, BlackNyackCS.Owner.Player) > 21)
                {
                    bustIfStayCount++;
                    lossFromBustCount++;
                    continue;
                }

                //Player keeps drawing until failure
                //TODO: Might want to make this a textbox variable. But considering the bonus for 21/5 card, it's likely this is still optimal.
                int playerCardsDrawn = 0;
                while (TotalForOwner(gameDeck, BlackNyackCS.Owner.Player) < 21 && NumberOfCardsForOwner(gameDeck, BlackNyackCS.Owner.Player) < 5)
                {
                    gameDeck[GetDrawableCardIndex(gameDeck)].owner = BlackNyackCS.Owner.Player;
                    playerCardsDrawn++;
                    if (playerCardsDrawn == 1)
                    {
                        //As an assessment of if it's safe to draw one more card, this is a special recording
                        if (TotalForOwner(gameDeck, BlackNyackCS.Owner.Player) > 21)
                        {
                            lossFromBustFromNextDrawCount++;
                            continue;
                        }
                    }
                }

                //Nyau draws until 17. I have no data to model when he goes higher, so consider that an extra bonyus.
                while (TotalForOwner(gameDeck, BlackNyackCS.Owner.Nyau) < 17 && NumberOfCardsForOwner(gameDeck, BlackNyackCS.Owner.Nyau) < 5)
                {
                    gameDeck[GetDrawableCardIndex(gameDeck)].owner = BlackNyackCS.Owner.Nyau;
                }

                int playerTotal = TotalForOwner(gameDeck, BlackNyackCS.Owner.Player);
                int playerCount = NumberOfCardsForOwner(gameDeck, BlackNyackCS.Owner.Player);
                int NyauTotal = TotalForOwner(gameDeck, BlackNyackCS.Owner.Nyau);
                int NyauCount = NumberOfCardsForOwner(gameDeck, BlackNyackCS.Owner.Nyau);

                if (playerTotal > 21)
                {
                    lossFromBustCount++;
                    continue;
                }

                if (NyauCount == 5 && NyauTotal <= 21)
                {
                    if (playerCount == 5)
                    {
                        drawCount++;
                        continue;
                    }
                    else
                    {
                        lossFrom5CardCount++;
                        continue;
                    }
                }
                
                if (NyauTotal == 21)
                {
                    if (playerTotal == 21)
                    {
                        drawCount++;
                        continue;
                    }
                    else
                    {
                        lossFrom21Count++;
                        continue;
                    }
                }

                if (NyauTotal > playerTotal && NyauTotal <= 21)
                {
                    lossFromHigherTotalCount++;
                    continue;
                }

                if (NyauTotal == playerTotal)
                {
                    drawCount++;
                    continue;
                }


                
                if (playerCount == 5)
                {
                    winsFrom5CardCount++;
                }

                if (playerTotal == 21)
                {
                    winsFrom21CardCount++;
                }

                if (NyauTotal > 21)
                {
                    winsFromNyauBustCount++;
                }
                else if (playerTotal > NyauTotal)
                { 
                    winsFromHigherTotalCount++;
                }
            }

            double bustIfHitPercent = (bustIfStayCount + lossFromBustFromNextDrawCount) / simulationCount;
            label_BustIfHit.Text = bustIfHitPercent.ToString();


            double winFrom5CardPercent = winsFrom5CardCount / simulationCount;
            label_5CardWinChance.Text = winFrom5CardPercent.ToString();


            double winFrom21Percent = winsFrom21CardCount / simulationCount;
            label_21WinChance.Text = winFrom21Percent.ToString();


            double hitIncome = 
                (winsFrom5CardCount * 300) + 
                (winsFrom21CardCount * 200) + 
                ((winsFrom5CardCount + winsFrom21CardCount + winsFromNyauBustCount + winsFromHigherTotalCount) * 100) +
                (drawCount * 50);
            double stayIncome =
                (stayWinsFrom5CardCount * 300) +
                (stayWinsFrom21CardCount * 200) +
                ((stayWinsFroNormalCount + stayWinsFrom5CardCount + stayWinsFrom21CardCount) * 100) +
                (drawIfStayCount * 50);
            double costToPlay = simulationCount * 50;

            label_StayIncome.Text = ((stayIncome - costToPlay) / simulationCount).ToString();
            label_HitIncome.Text = ((hitIncome - costToPlay) / simulationCount).ToString();
        }


        private int TotalForOwner(List<SimCard> deck, Owner owner)
        {
            int total = 0;
            int aceCount = 0;
            foreach (var card in deck)
            {
                if (card.owner == owner)
                {
                    total += card.value;
                    if (card.value == 11)
                    {
                        aceCount++;
                    }
                }
            }

            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }

            return total;
        }

        private int NumberOfCardsForOwner(List<SimCard> deck, Owner owner)
        {
            int total = 0;
            foreach (var card in deck)
            {
                if (card.owner == owner)
                {
                    total++;
                }
            }
            return total;
        }

        private int GetDrawableCardIndex(List<SimCard> deck)
        {
            //I'm too lazy to manage a second list of drawable cards. Just keep retrying.
            while (true)
            {
                int drawIndex = rand.Next(0, deck.Count - 1);
                if (deck[drawIndex].owner != BlackNyackCS.Owner.None)
                {
                    continue;
                }

                return drawIndex;
            }
        }



        private void ToggleButtonForColor(Button button, Color color)
        {
            if (button.BackColor == color)
            {
                button.BackColor = SystemColors.Control;
            }
            else
            {
                button.BackColor = color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Card_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (e.Button == MouseButtons.Left)
            {
                ToggleButtonForColor(button, PlayerColor);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ToggleButtonForColor(button, NyauColor);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                ToggleButtonForColor(button, ThirdPartyColor);
            }

            GenerateOdds();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            foreach(var card in deck)
            {
                card.button.BackColor = SystemColors.Control;
            }
            label_21WinChance.Text = "0";
            label_5CardWinChance.Text = "0";
            label_BustIfHit.Text = "0";
            label_HitIncome.Text = "0";
            label_StayIncome.Text = "0";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void projectGitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/software-2/BlackNyack");
        }

        private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/software-2/BlackNyack/issues");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var version = (new WebClient()).DownloadString("https://github.com/software-2/BlackNyack/raw/master/latestversion.txt");
                var parsed = Version.Parse(version);

                if (parsed.CompareTo(currentVersion) != 0)
                {
                    var dialog = "There is a new version! Want to go get it?\n\nYour Version: " + parsed.ToString() + "\n" + "New Version: " + currentVersion.ToString();
                    var result = MessageBox.Show(dialog, "New Version!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://github.com/software-2/BlackNyack/releases");
                    }
                }
                else
                {
                    MessageBox.Show("You're up to date!", "Up To Date!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch
            {
                var result = MessageBox.Show("Sorry, I couldn't find the version number. Do you want to go to the website to check?", "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/software-2/BlackNyack/releases");
                }
            }
        }
    }

    public class SimCard
    {
        public Owner owner;
        public int value;

        public SimCard(Owner owner, int value)
        {
            this.owner = owner;
            this.value = value;
        }
    }

}
