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

namespace Week7FSM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VendState state = VendState.AWAITING;

        decimal input = .25m;
        string[] outputs = { "Gum", "Granola" };

        Dictionary<string, decimal> costLookup = new Dictionary<string, decimal>();

        decimal totalCollected = 0;
        string currentOutput = string.Empty;

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On grid loaded, set up the dictionary and update displays accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SetupDictionary();
            UpdateFunds();
            UpdateButtons();
        }

        /// <summary>
        /// if insert quarter is enabled and clicked, increment totalCollected by input and set state to accepting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            totalCollected += input;

            state = VendState.ACCEPTING;
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// if vend gum is enabled and clicked, set current output to gum and update vending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Gum_Click(object sender, RoutedEventArgs e)
        {
            currentOutput = outputs[0];

            UpdateVend();
        }

        /// <summary>
        /// if vend granola is enabled and clicked, set current output to granola and update vending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Granola_Click(object sender, RoutedEventArgs e)
        {
            currentOutput = outputs[1];

            UpdateVend();
        }

        /// <summary>
        /// if collect is enabled and clicked, inform user they have succesfully 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Collect_Click(object sender, RoutedEventArgs e)
        {
            tb_Output.Text = $"You have collected your {currentOutput}!";
            tb_Output.Text += totalCollected > 0 ? $"\nYour change is {totalCollected.ToString("C")}" : string.Empty;
            totalCollected = 0;
            state = VendState.AWAITING;
        }

        /// <summary>
        /// If cancel is enabled and clicked, set total collected to zero, inform user of cancellation and switch to awaiting state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            totalCollected = 0;
            tb_Output.Text = "Transaction cancelled";
            state = VendState.AWAITING;
        }

        #endregion

        #region Update Logic

        /// <summary>
        /// Update text to show current item, adjust funds based on current item and switch to vending state
        /// </summary>
        private void UpdateVend()
        {
            tb_Output.Text = $"Your {currentOutput} is ready to be collected!";
            AdjustFunds(currentOutput);

            state = VendState.VENDING;
        }

        /// <summary>
        /// Update funds textblock to display current total collected in money format
        /// </summary>
        private void UpdateFunds()
        {
            tb_Funds.Text = $"You have input {totalCollected.ToString("C")}";
        }

        /// <summary>
        /// Adjust total collected based on name of item vending using cost lookup dictionary
        /// </summary>
        /// <param name="itemName"></param>
        private void AdjustFunds(string itemName)
        {
            totalCollected -= costLookup[itemName];
        }

        /// <summary>
        /// Update button visibility based on state and total collected
        /// </summary>
        private void UpdateButtons()
        {
            btn_Granola.IsEnabled = state == VendState.ACCEPTING && totalCollected >= costLookup["Granola"] ? true : false;
            btn_Gum.IsEnabled = state == VendState.ACCEPTING && totalCollected >= costLookup["Gum"] ? true : false;

            btn_Insert.IsEnabled = state != VendState.VENDING && totalCollected < .75m ? true : false;

            btn_Collect.IsEnabled = state == VendState.VENDING ? true : false;

            btn_Cancel.IsEnabled = state == VendState.ACCEPTING ? true : false;
        }

        /// <summary>
        /// Setup costLookup dictionary with vending items and price
        /// </summary>
        private void SetupDictionary()
        {
            costLookup.Add(outputs[0], input * 2);
            costLookup.Add(outputs[1], input * 3);
        }

        #endregion

        #region Universal Update After Action

        /// <summary>
        /// Begin invokation of update actions to update fund display and button availability on any interaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// Dispatcher example found via ChatGPT, prompt: is there an event in c# wpf that triggers after the user makes any interaction?
        /// ChatGPT suggested previewmousedown, that did not work due to click event waiting for mouse up
        private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Control control = sender as Control ?? new Control();

            // Use the Dispatcher BeginInvoke to schedule action after control action has finished
            control.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateButtons();
                UpdateFunds();
            }));
        }

        #endregion
    }

    public enum VendState
    {
        AWAITING,
        ACCEPTING,
        VENDING
    }
}
