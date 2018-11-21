using AmeisenBotLogger;
using AmeisenBotManager;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Objects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für mainscreenForm.xaml 💕
    /// </summary>
    public partial class BotWindow : Window
    {
        public static Dictionary<UnitTrait, string> UnitTraitSymbols =
            new Dictionary<UnitTrait, string> {
                { UnitTrait.SELL, "💰" },
                { UnitTrait.REPAIR, "🔧" },
                { UnitTrait.FOOD, "🍖" },
                { UnitTrait.DRINK, "🍹" },
                { UnitTrait.FLIGHTMASTER, "🛫" },
                { UnitTrait.AUCTIONMASTER, "💸" },
            };

        private string lastImgPath;
        private DispatcherTimer uiUpdateTimer;
        private BotManager BotManager { get; }
        private Settings Settings => BotManager.Settings;
        private ulong LastGuid { get; set; }

        public BotWindow(WowExe wowExe, BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;

            // Load Settings
            BotManager.LoadSettingsFromFile(wowExe.characterName);
            ApplyConfigColors();
            BotManager.StartBot(wowExe);

            if (Settings.saveBotWindowPosition)
            {
                if (Settings.oldXindowPosX != 0)
                {
                    Left = Settings.oldXindowPosX;
                }

                if (Settings.oldXindowPosY != 0)
                {
                    Top = Settings.oldXindowPosY;
                }
            }
        }

        private void ApplyConfigColors()
        {
            ResourceDictionary resources = Application.Current.Resources;
            resources["AccentColor"] = ParseColor(Settings.accentColor);
            resources["BackgroundColor"] = ParseColor(Settings.backgroundColor);
            resources["TextColor"] = ParseColor(Settings.textColor);

            resources["MeNodeColor"] = ParseColor(Settings.meNodeColor);
            resources["WalkableNodeColorLow"] = ParseColor(Settings.walkableNodeColorLow);
            resources["WalkableNodeColorHigh"] = ParseColor(Settings.walkableNodeColorHigh);

            resources["HealthColor"] = ParseColor(Settings.healthColor);
            resources["EnergyColor"] = ParseColor(Settings.energyColor);
            resources["ExpColor"] = ParseColor(Settings.expColor);
            resources["TargetHealthColor"] = ParseColor(Settings.targetHealthColor);
            resources["TargetEnergyColor"] = ParseColor(Settings.targetEnergyColor);
            resources["holoLogoColor"] = ParseColor(Settings.holoLogoColor);
        }

        private Color ParseColor(string colorString) => (Color)ColorConverter.ConvertFromString(colorString);

        private void ButtonCobatClassEditor_Click(object sender, RoutedEventArgs e)
        {
            // Going to be reworked
            //new CombatClassWindow(BotManager).Show();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ButtonExtendedDebugUI_Click(object sender, RoutedEventArgs e)
            => new DebugWindow(BotManager).Show();

        private void ButtonGroup_Click(object sender, RoutedEventArgs e)
            => new GroupWindow(BotManager).Show();

        private void ButtonMap_Click(object sender, RoutedEventArgs e)
            => new MapWindow(BotManager, BotManager.AmeisenDBManager).Show();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                RestoreDirectory = true,
                Filter = "CombatClass *.cs|*.cs"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BotManager.LoadCombatClassFromFile(openFileDialog.FileName);
            }
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
            => new SettingsWindow(BotManager).ShowDialog();

        private void CheckBoxAssistGroup_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToAssistParty = (bool)checkBoxAssistGroup.IsChecked;

        private void CheckBoxAssistPartyAttack_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToAttack = (bool)checkBoxAssistPartyAttack.IsChecked;

        private void CheckBoxAssistPartyBuff_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToBuff = (bool)checkBoxAssistPartyBuff.IsChecked;

        private void CheckBoxAssistPartyHeal_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToHeal = (bool)checkBoxAssistPartyAttack.IsChecked;

        private void CheckBoxAssistPartyTank_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToTank = (bool)checkBoxAssistPartyTank.IsChecked;

        private void CheckBoxFollowMaster_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToFollowParty = (bool)checkBoxFollowParty.IsChecked;

        private void CheckBoxTopMost_Click(object sender, RoutedEventArgs e)
            => SetTopMost();

        private void LoadViewSettings()
        {
            checkBoxAssistPartyAttack.IsChecked = Settings.behaviourAttack;
            BotManager.IsAllowedToAttack = Settings.behaviourAttack;

            checkBoxAssistPartyTank.IsChecked = Settings.behaviourTank;
            BotManager.IsAllowedToTank = Settings.behaviourTank;

            checkBoxAssistPartyHeal.IsChecked = Settings.behaviourHeal;
            BotManager.IsAllowedToHeal = Settings.behaviourHeal;

            checkBoxAssistPartyBuff.IsChecked = Settings.behaviourBuff;
            BotManager.IsAllowedToBuff = Settings.behaviourBuff;

            checkBoxAssistGroup.IsChecked = Settings.assistParty;
            BotManager.IsAllowedToAssistParty = Settings.assistParty;

            checkBoxFollowParty.IsChecked = Settings.followMaster;
            BotManager.IsAllowedToFollowParty = Settings.followMaster;

            checkBoxReleaseSpirit.IsChecked = Settings.releaseSpirit;
            BotManager.IsAllowedToReleaseSpirit = Settings.releaseSpirit;

            checkBoxRandomEmotes.IsChecked = Settings.randomEmotes;
            BotManager.IsAllowedToDoRandomEmotes = Settings.randomEmotes;

            checkBoxDoBotStuff.IsChecked = Settings.doOwnStuff;
            BotManager.IsAllowedToDoOwnStuff = Settings.doOwnStuff;

            checkBoxRevive.IsChecked = Settings.revive;
            BotManager.IsAllowedToRevive = Settings.revive;

            sliderDistance.Value = Settings.followDistance;

            checkBoxTopMost.IsChecked = Settings.topMost;
            Topmost = Settings.topMost;
        }

        private void Mainscreen_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveViewSettings();
            Settings.oldXindowPosX = Left;
            Settings.oldXindowPosY = Top;
            BotManager.StopBot();
        }

        private void Mainscreen_Loaded(object sender, RoutedEventArgs e)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Loaded MainScreen", this);

            Title = $"AmeisenBot - {BotManager.WowExe.characterName} [{BotManager.WowExe.process.Id}]";

            UpdateUI();
            StartUIUpdateTime();

            LoadViewSettings();
        }

        private void Mainscreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }

        private void SaveViewSettings()
        {
            Settings.behaviourAttack = (bool)checkBoxAssistPartyAttack.IsChecked;
            Settings.behaviourTank = (bool)checkBoxAssistPartyTank.IsChecked;
            Settings.behaviourHeal = (bool)checkBoxAssistPartyHeal.IsChecked;
            Settings.behaviourBuff = (bool)checkBoxAssistPartyBuff.IsChecked;
            Settings.followMaster = (bool)checkBoxFollowParty.IsChecked;
            Settings.releaseSpirit = (bool)checkBoxReleaseSpirit.IsChecked;
            Settings.revive = (bool)checkBoxRevive.IsChecked;
            BotManager.SaveSettingsToFile(BotManager.LoadedConfigName);
        }

        private void SetTopMost()
        {
            Topmost = (bool)checkBoxTopMost.IsChecked;
            Settings.topMost = (bool)checkBoxTopMost.IsChecked;
        }

        private void SliderDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                labelDistance.Content = $"Follow Distance: {Math.Round(sliderDistance.Value, 2)}m";
                Settings.followDistance = Math.Round(sliderDistance.Value, 2);
            }
            catch { }
        }

        private void StartUIUpdateTime()
        {
            uiUpdateTimer = new DispatcherTimer();
            uiUpdateTimer.Tick += new EventHandler(UIUpdateTimer_Tick);
            uiUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            uiUpdateTimer.Start();
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Started UI-Update-Timer", this);
        }

        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (BotManager.IsIngame)
            {
                UpdateUI();
            }
        }

        private void UpdateMyViews()
        {
            try
            {
                if (Settings.picturePath != lastImgPath)
                {
                    if (Settings.picturePath.Length > 0)
                    {
                        botPicture.Source = new BitmapImage(new Uri(Settings.picturePath));
                        lastImgPath = Settings.picturePath;
                    }
                }
            }
            catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, "Failed to load picture...", this); }

            labelName.Content = BotManager.Me.Name + " lvl." + BotManager.Me.Level;

            labelHP.Content = $"Health {BotManager.Me.Health} / {BotManager.Me.MaxHealth}";
            progressBarHP.Maximum = BotManager.Me.MaxHealth;
            progressBarHP.Value = BotManager.Me.Health;

            labelEnergy.Content = $"Energy {BotManager.Me.Energy} / {BotManager.Me.MaxEnergy}";
            progressBarEnergy.Maximum = BotManager.Me.MaxEnergy;
            progressBarEnergy.Value = BotManager.Me.Energy;

            labelExp.Content = $"Exp {BotManager.Me.Exp} / {BotManager.Me.MaxExp}";
            progressBarXP.Maximum = BotManager.Me.MaxExp;
            progressBarXP.Value = BotManager.Me.Exp;
        }

        private void UpdateTargetViews()
        {
            labelNameTarget.Content = $"{BotManager.Target.Name} lvl.{BotManager.Target.Level}";

            labelTargetHP.Content = $"Health {BotManager.Target.Health} / {BotManager.Target.MaxHealth}";
            progressBarHPTarget.Maximum = BotManager.Target.MaxHealth;
            progressBarHPTarget.Value = BotManager.Target.Health;

            labelTargetEnergy.Content = $"Energy {BotManager.Target.Energy} / {BotManager.Target.MaxEnergy}";
            progressBarEnergyTarget.Maximum = BotManager.Target.MaxEnergy;
            progressBarEnergyTarget.Value = BotManager.Target.Energy;

            labelTargetDistance.Content = $"Distance: {Math.Round(BotManager.Target.Distance, 2)}m";

            Unit target = BotManager.Target;

            if (target != null && target.Guid != 0)
            {
                if (target.Guid != LastGuid)
                {
                    target.Update();
                    RememberedUnit rememberedUnit = BotManager.CheckForRememberedUnit(target.Name, target.ZoneID, target.MapID);

                    if (rememberedUnit != null)
                    {
                        labelRemember.Content = "I know this Unit";

                        StringBuilder sb = new StringBuilder();
                        foreach (UnitTrait u in rememberedUnit.UnitTraits)
                        {
                            sb.Append($"{UnitTraitSymbols[u]} ");
                        }

                        labelUnitTraits.Content = sb.ToString();
                    }
                    else
                    {
                        labelRemember.Content = "I don't know this Unit";
                        labelUnitTraits.Content = "-";
                    }
                    LastGuid = target.Guid;
                }
            }
        }

        private void UpdateFSMViews() => labelFSMState.Content = $"{BotManager.CurrentFSMState}";

        /// <summary>
        /// This thing updates the UI... Note to myself: "may need to improve this thing in the future..."
        /// </summary>
        private void UpdateUI()
        {
            // TODO: find a better way to update this
            //AmeisenManager.Instance.GetObjects();

            Process currentProcess = Process.GetCurrentProcess();
            long memoryUsageMB = currentProcess.WorkingSet64 / 1000000;

            labelLoadedCombatClass.Content = $"{Path.GetFileName(Settings.combatClassPath)}.cs";
            labelLoadedCombatClassC.Content = $"{BotManager.CurrentCombatClass}";
            labelClass.Content = $"{BotManager.Me.Class.ToString()}";
            labelRace.Content = $"{BotManager.Me.Race.ToString()}";

            if (BotManager.Me != null)
            {
                try
                {
                    UpdateFSMViews();
                    UpdateMyViews();

                    if (BotManager.Target != null)
                    {
                        UpdateTargetViews();
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.Instance.Log(LogLevel.ERROR, e.ToString(), this);
                }
            }
        }

        private void CheckBoxReleaseSpirit_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToReleaseSpirit = (bool)checkBoxReleaseSpirit.IsChecked;

        private void CheckBoxReleaseSpirit_Copy_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToRevive = (bool)checkBoxRevive.IsChecked;

        private void ButtonRememberUnit_Click(object sender, RoutedEventArgs e)
        {
            if (BotManager.Target != null && BotManager.Target.Guid != 0)
            {
                RememberUnitWindow rememberUnitWindow = new RememberUnitWindow(BotManager.Target)
                {
                    Topmost = Settings.topMost
                };
                rememberUnitWindow.ShowDialog();

                if (rememberUnitWindow.ShouldRemember)
                {
                    BotManager.RememberUnit(rememberUnitWindow.UnitToRemmeber);
                }
            }
        }
    }
}