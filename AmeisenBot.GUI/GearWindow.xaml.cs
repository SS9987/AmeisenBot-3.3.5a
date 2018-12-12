using AmeisenBot.Character.Objects;
using AmeisenBotManager;
using System.Windows;
using System.Windows.Input;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für GearWindow.xaml
    /// </summary>
    public partial class GearWindow : Window
    {
        private BotManager BotManager { get; set; }

        public GearWindow(BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;
        }

        private void Mainscreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void Mainscreen_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Item item in BotManager.Character.Equipment.AsList())
            {
                listboxEquipped.Items.Add(item);
            }

            foreach (Item item in BotManager.Character.InventoryItems)
            {
                listboxInventory.Items.Add(item);
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
