using AmeisenBotManager;
using AmeisenBotUtilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private BotManager BotManager { get; }
        private Settings Settings { get; set; }

        public SettingsWindow(BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;
            Settings = Settings;
            Topmost = Settings.topMost;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void ButtonSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                RestoreDirectory = true,
                Filter = "Images|*.png;*.jpg;*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                LoadBotPicture(openFileDialog.FileName);
            }
        }

        private void ColorMe_Click(object sender, RoutedEventArgs e)
            => SelectColor("MeNodeColor");

        private void ColorWalkable_Click(object sender, RoutedEventArgs e)
            => SelectColor("WalkableNodeColorLow");

        private void ColorWalkableNodeHigh_Click(object sender, RoutedEventArgs e)
            => SelectColor("WalkableNodeColorHigh");

        private void LoadAmeisenServerSettings()
        {
            textboxIP.Text = Settings.ameisenServerIP;
            textboxPort.Text = Settings.ameisenServerPort.ToString();
            checkboxAutoConnect.IsChecked = Settings.serverAutoConnect;
        }

        private void LoadBotPicture(string fileName)
        {
            string configDir = AppDomain.CurrentDomain.BaseDirectory + "config/";
            string imageDir = AppDomain.CurrentDomain.BaseDirectory + "config/img/";

            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            if (!Directory.Exists(imageDir))
            {
                Directory.CreateDirectory(imageDir);
            }

            string imagePath = $"{AppDomain.CurrentDomain.BaseDirectory}config\\\\img\\\\{Path.GetFileName(fileName)}";

            if (imagePath != fileName)
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                File.Copy(fileName, imagePath);
            }

            Settings.picturePath = imagePath;
            labelSelectedPicture.Content = Path.GetFileName(fileName);
        }

        private void LoadDatabaseSettings()
        {
            textboxDBIP.Text = Settings.databaseIP;
            textboxDBPort.Text = Settings.databasePort.ToString();
            textboxDBDatabase.Text = Settings.databaseName;
            textboxDBUsername.Text = Settings.databaseUsername;
            textboxDBPassword.Password = Settings.databasePasswort;
            checkboxDBAutoConnect.IsChecked = Settings.databaseAutoConnect;
        }

        /// <summary>
        /// Load settings to UI
        /// </summary>
        private void LoadSettings()
        {
            labelSelectedPicture.Content = Path.GetFileName(Settings.picturePath);

            LoadAmeisenServerSettings();
            LoadDatabaseSettings();

            checkboxSaveBotPosition.IsChecked = Settings.saveBotWindowPosition;
            checkboxSaveWoWPosition.IsChecked = Settings.saveWoWWindowPosition;

            // Colors are already loaded
        }

        private void SaveAmeisenServerSettings()
        {
            Settings.ameisenServerIP = textboxIP.Text;
            Settings.ameisenServerPort = Convert.ToInt32(textboxPort.Text);
            Settings.serverAutoConnect = (bool)checkboxAutoConnect.IsChecked;
        }

        private void SaveDatabaseSettings()
        {
            Settings.databaseIP = textboxDBIP.Text;
            Settings.databasePort = Convert.ToInt32(textboxDBPort.Text);
            Settings.databaseName = textboxDBDatabase.Text;
            Settings.databaseUsername = textboxDBUsername.Text;
            Settings.databasePasswort = textboxDBPassword.Password;
            Settings.databaseAutoConnect = (bool)checkboxDBAutoConnect.IsChecked;
        }

        private void SaveMainUISettings()
        {
            Settings.accentColor = GetColorFromResources("AccentColor").ToString();
            Settings.backgroundColor = GetColorFromResources("BackgroundColor").ToString();
            Settings.textColor = GetColorFromResources("TextColor").ToString();
            Settings.healthColor = GetColorFromResources("HealthColor").ToString();
            Settings.energyColor = GetColorFromResources("EnergyColor").ToString();
            Settings.expColor = GetColorFromResources("ExpColor").ToString();
            Settings.targetHealthColor = GetColorFromResources("TargetHealthColor").ToString();
            Settings.targetEnergyColor = GetColorFromResources("TargetEnergyColor").ToString();
            Settings.holoLogoColor = GetColorFromResources("holoLogoColor").ToString();
        }

        private Color GetColorFromResources(string resString)
            => (Color)Application.Current.Resources[resString];

        private void SaveMapUISettings()
        {
            Settings.walkableNodeColorLow = GetColorFromResources("WalkableNodeColorLow").ToString();
            Settings.walkableNodeColorHigh = GetColorFromResources("WalkableNodeColorHigh").ToString();
            Settings.meNodeColor = GetColorFromResources("MeNodeColor").ToString();
        }

        /// <summary>
        /// Save settings from UI
        /// </summary>
        private void SaveSettings()
        {
            SaveAmeisenServerSettings();
            SaveDatabaseSettings();
            SaveMapUISettings();
            SaveMainUISettings();

            Settings.saveBotWindowPosition = (bool)checkboxSaveBotPosition.IsChecked;
            Settings.saveWoWWindowPosition = (bool)checkboxSaveWoWPosition.IsChecked;

            BotManager.SaveSettingsToFile(BotManager.LoadedConfigName);
        }

        private void SelectColor(string resourceColor)
        {
            ColorPickWindow colorpicker = new ColorPickWindow(GetColorFromResources(resourceColor))
            {
                Topmost = Settings.topMost
            };
            colorpicker.ShowDialog();
            if (colorpicker.ApplyColor)
            {
                Application.Current.Resources[resourceColor] = colorpicker.ActiveColor;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((bool)radiobuttonRefreshSpeedLowest.IsChecked)
            {
                Settings.dataRefreshRate = 1000;
            }
            else if ((bool)radiobuttonRefreshSpeedLow.IsChecked)
            {
                Settings.dataRefreshRate = 500;
            }
            else if ((bool)radiobuttonRefreshSpeedMedium.IsChecked)
            {
                Settings.dataRefreshRate = 250;
            }
            else if ((bool)radiobuttonRefreshSpeedHigh.IsChecked)
            {
                Settings.dataRefreshRate = 100;
            }
            else if ((bool)radiobuttonRefreshSpeedHighest.IsChecked)
            {
                Settings.dataRefreshRate = 0;
            }
            else
            {
                //something is wrong...
            }

            SaveSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Settings.dataRefreshRate)
            {
                case 1000:
                    radiobuttonRefreshSpeedLowest.IsChecked = true;
                    break;

                case 500:
                    radiobuttonRefreshSpeedLow.IsChecked = true;
                    break;

                case 250:
                    radiobuttonRefreshSpeedMedium.IsChecked = true;
                    break;

                case 100:
                    radiobuttonRefreshSpeedHigh.IsChecked = true;
                    break;

                case 0:
                    radiobuttonRefreshSpeedHighest.IsChecked = true;
                    break;

                default:
                    break;
            }

            LoadSettings();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }

        private void ColorBackground_Click(object sender, RoutedEventArgs e)
            => SelectColor("BackgroundColor");

        private void ColorOutline_Click(object sender, RoutedEventArgs e)
            => SelectColor("AccentColor");

        private void ColorText_Click(object sender, RoutedEventArgs e)
            => SelectColor("TextColor");

        private void ColorHealth_Click(object sender, RoutedEventArgs e)
            => SelectColor("HealthColor");

        private void ColorEnergy_Click(object sender, RoutedEventArgs e)
            => SelectColor("EnergyColor");

        private void ColorExp_Click(object sender, RoutedEventArgs e)
            => SelectColor("EXPColor");

        private void ColorTargetHealth_Click(object sender, RoutedEventArgs e)
            => SelectColor("TargetHealthColor");

        private void ColorTargetEnergy_Click(object sender, RoutedEventArgs e)
            => SelectColor("TargetEnergyColor");

        private void ColorThreads_Click(object sender, RoutedEventArgs e)
            => SelectColor("holoLogoColor");
    }
}