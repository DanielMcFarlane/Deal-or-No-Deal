using System;
using System.Collections.Generic;
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

namespace Deal_or_No_Deal
{
    public partial class Game : Page
    {
        private int boxSelected;
        private int firstBoxValue;
        private int finalBoxValue;
        private double bankersOffer;
        private int remainingTurns = 3;
        private Dictionary<int, Label> labelValue;

        public Game()
        {
            InitializeComponent();
            SideLabelValue();
            ShuffleBoxes();
        }

        // ==================
        // Gameplay Functions
        // ==================

        /// <summary>
        /// Side prize label values assigned
        /// </summary>
        private void SideLabelValue()
        {
            // This adds the key and value of prize labels to the dictionary.

            labelValue = new Dictionary<int, Label>
            {
                {1, lblPrize1}, {5, lblPrize2}, {10, lblPrize3}, {25, lblPrize4},
                {50, lblPrize5}, {75, lblPrize6}, {100, lblPrize7}, {250, lblPrize8},
                {500, lblPrize9}, {750, lblPrize10}, {1000, lblPrize11}, {5000, lblPrize12},
                {10000, lblPrize13}, {15000, lblPrize14}, {20000, lblPrize15}, {25000, lblPrize16},
                {50000, lblPrize17}, {75000, lblPrize18}, {100000, lblPrize19}, {250000, lblPrize20}
            };
        }


        /// <summary>
        /// Fisher-Yates shuffle algorithm
        /// </summary>
        private static List<int> FisherYates(List<int> list)
        {
            // This is the Fisher-Yates shuffle algorithm that will be used to shuffle values.

            Random r = new();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int index = r.Next(i + 1);
                (list[i], list[index]) = (list[index], list[i]); // A tuple is used to swap the values.
            }
            return list;
        }


        /// <summary>
        /// Shuffle box values
        /// </summary>
        private void ShuffleBoxes()
        {
            /*
                This will retrieve the list of prize values from the labelValue dictionary keys.
                The list of prize values is then shuffled using the Fisher-Yates algorithm and asign it to prizeValues.
                It will then iterate through each button in the boxPanel and assign a shuffled prize value to its Tag property.
            */

            List<int> prizeValues = new(labelValue.Keys);
            prizeValues = FisherYates(prizeValues);

            for (int i = 0; i < boxPanel.Children.Count; i++)
            {
                Button button = (Button)boxPanel.Children[i];
                button.Tag = prizeValues[i];
            }
        }


        /// <summary>
        /// Handler for when a box is clicked
        /// </summary>
        private void Box_Click(object sender, RoutedEventArgs e)

        {
            /*
                This disables the first box and then hides the boxes as they are clicked.
                A switch statement is used for the 1st and 20th boxes which have unique criteria.
                The default removes the pize label value for the box selected, removes 1 from remainingTurns and updates the label.
                If the turn is divisible by 3 indicating that it is the third turn, an offer is shown and remainingTurns is reset to 3 along with the label.
            */

            Button clickedBox = (Button)sender;
            int value = int.Parse(clickedBox.Tag.ToString()); // Get the value of a clicked box, convert it to a string and parse it to an integer.

            boxSelected++;

            switch (boxSelected)
            {
                case 1:
                    firstBoxValue = value;
                    clickedBox.IsEnabled = false;
                    //MessageBox.Show($"First box value : {value:C}", "Initial box value"); // Debug statement to show initial box value.
                    lblBoxSelected.Visibility = Visibility.Visible;
                    lblBoxSelected.Content = clickedBox.Content;
                    lblTurnsRemaining.Content = remainingTurns;
                    break;

                case 20:
                    finalBoxValue = value;
                    DisplayFinal();
                    break;

                default:
                    RemovePrize(value);

                    if ((boxSelected - 1) % 3 == 0)
                    {
                        DisplayOffer();
                        remainingTurns = (boxSelected == 19) ? 1 : 3; // Using ternary to assign either 1 or 3 to remainingTurns.

                    }
                    else
                    {
                        remainingTurns--;
                    }

                    clickedBox.Visibility = Visibility.Hidden;
                    lblTurnsRemaining.Content = remainingTurns;
                    break;
            }
        }


        /// <summary>
        /// Remove the prize value when the appropriate box is selected
        /// </summary>
        private void RemovePrize(int boxValue)
        {
            // This will remove the value of the box selected from the dictionary and remove the corresponding label.

            if (labelValue.TryGetValue(boxValue, out Label? value))
            {
                value.Visibility = Visibility.Hidden;
                labelValue.Remove(boxValue);
            }
        }


        /// <summary>
        /// Restarts the game
        /// </summary>
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.RestartGame();
        }


        /// <summary>
        /// Exit the game
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        // ====================
        // Offer page Functions
        // ====================

        /// <summary>
        /// Calculate an offer based on the remaining values
        /// </summary>
        private double CalculateOffer()
        {
            /*
                Since a dictionary is used you can get the average of remaining keys with Keys.Average().
                It is then multiplied by 0.5 to return an offer.
            */

            double averageValue = labelValue.Keys.Average();
            double bankersOffer = averageValue * 0.5;

            return bankersOffer;
        }


        /// <summary>
        /// Toggle deal or no deal panel
        /// </summary>
        private void OfferPageVisible(bool isVisible)
        {
            // A ternary toggle function that will flip between an offer layout and gameplay if flagged true or false.

            offerPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            offerButtons.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            boxPanel.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
            gameInfo.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
        }


        /// <summary>
        /// Display the offer panel
        /// </summary>
        private void DisplayOffer()
        {
            /*
                Offer state is set to true triggering the offer panel and buttons to show.
                The offer is then calculated and shown to the user.
            */

            OfferPageVisible(true);

            bankersOffer = CalculateOffer();
            lblOffer.Content = $"The banker has offered \nyou {bankersOffer:C}";
        }


        /// <summary>
        /// Offer accept handler
        /// </summary>
        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            // EndGame is called and congratulations or commiserations are shown

            EndGame();

            lblOffer.Content = firstBoxValue < bankersOffer
                ? $"You won {bankersOffer:C} \n\nYour box contained \n{firstBoxValue:C} \n\nYou beat the banker!"
                : $"Better luck next time. \n\nYour box contained \n{firstBoxValue:C} \n\nYou lost to the banker.";
        }


        /// <summary>
        /// Offer decline handler
        /// </summary>
        private void BtnDecline_Click(object sender, RoutedEventArgs e)
        {
            // OfferPageVisible is set back to false signalling that the game is back in motion.

            OfferPageVisible(false);
        }


        // ===============================
        // Final round/ end game Functions
        // ===============================

        /// <summary>
        /// Display the final panel
        /// </summary>
        private void DisplayFinal()
        {
            // The page is set up to display the final prompt.

            OfferPageVisible(true);
            finalButtons.Visibility = Visibility.Visible;

            lblOffer.Content = $"It's the final round. \n\nYou can keep your box \nor swap your box.";
        }


        /// <summary>
        /// Prepare the window for the final message and exit button
        /// </summary>
        private void EndGame()
        {
            // Function can be called to set the page up for the final winning or losing message. 

            leftPrizes.Visibility = Visibility.Hidden;
            rightPrizes.Visibility = Visibility.Hidden;
            endGameButton.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Final keep handler
        /// </summary>
        private void BtnKeep_Click(object sender, RoutedEventArgs e)
        {
            // EndGame is called and congratulations or commiserations are shown

            EndGame();

            lblOffer.Content = firstBoxValue > finalBoxValue
                ? $"Great choice! \n\nYou've won {firstBoxValue:C}"
                : $"You should have \nswapped. \n\nYou won {firstBoxValue:C}";
        }


        /// <summary>
        /// Final swap handler
        /// </summary>
        private void BtnSwap_Click(object sender, RoutedEventArgs e)
        {
            // EndGame is called and congratulations or commiserations are shown

            EndGame();

            lblOffer.Content = finalBoxValue > firstBoxValue
                ? $"Great choice! \n\nYou've won {finalBoxValue:C}"
                : $"You shouldn't have \nswapped. \n\nYou won {finalBoxValue:C}";
        }
    }
}