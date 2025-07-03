using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AlonsMOTD
{
    /// <summary>The main entry point for Alon's MOTD mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>SMAPI calls this when the mod is first loaded.</summary>
        public override void Entry(IModHelper helper)
        {
            // Load or create the config
            Config = helper.ReadConfig<ModConfig>();

            // Register for day-started event to show the message
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            // Delay GMCM registration until all mods are initialized
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Raised after all mods are loaded and the game is ready.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return; // GMCM not installed, skip

            // Register this mod's config with GMCM
            gmcm.Register(
                ModManifest,
                () => Config = new ModConfig(),
                () => Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // Add Text option for MOTD
            gmcm.AddTextOption(
                ModManifest,
                () => Config.MessageOfTheDay,
                value => Config.MessageOfTheDay = value,
                () => "Message of the Day",
                () => "The message shown each morning.",
                allowedValues: null,
                formatAllowedValue: null,
                fieldId: null
            );

            // Add dropdown for selecting a named color
            string[] colors = new[] {
                "Dark Red", "Red", "Bright Red",
                "Dark Orange", "Orange", "Bright Orange",
                "Dark Yellow", "Yellow", "Bright Yellow",
                "Dark Green", "Green", "Bright Green",
                "Dark Blue", "Blue", "Bright Blue",
                "Dark Indigo", "Indigo", "Bright Indigo",
                "Dark Violet", "Violet", "Bright Violet",
                "White", "Light Gray", "Gray", "Dark Gray", "Black"
            };
            gmcm.AddTextOption(
                ModManifest,
                () => Config.ColorName,
                value => Config.ColorName = value,
                () => "Text Color",
                () => "Choose a color for the message.",
                allowedValues: colors,
                formatAllowedValue: null,
                fieldId: null
            );
        }

        /// <summary>Raised at the start of each new day.</summary>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Game1.chatBox.addMessage(
                Config.MessageOfTheDay,
                GetColorByName(Config.ColorName)
            );
        }

        /// <summary>Map a named color to a XNA Color using known hex codes.</summary>
        private Color GetColorByName(string name)
        {
            return name switch
            {
                "Dark Red"    => HexToColor("#8B0000"),
                "Red"         => HexToColor("#FF0000"),
                "Bright Red"  => HexToColor("#FF5555"),

                "Dark Orange"   => HexToColor("#FF8C00"),
                "Orange"        => HexToColor("#FFA500"),
                "Bright Orange" => HexToColor("#FFD580"),

                "Dark Yellow"    => HexToColor("#CCCC00"),
                "Yellow"         => HexToColor("#FFFF00"),
                "Bright Yellow"  => HexToColor("#FFFF66"),

                "Dark Green"   => HexToColor("#006400"),
                "Green"        => HexToColor("#008000"),
                "Bright Green" => HexToColor("#00FF00"),

                "Dark Blue"    => HexToColor("#00008B"),
                "Blue"         => HexToColor("#0000FF"),
                "Bright Blue"  => HexToColor("#6666FF"),

                "Dark Indigo"   => HexToColor("#4B0082"),
                "Indigo"        => HexToColor("#6A0DAD"),
                "Bright Indigo" => HexToColor("#9F00C5"),

                "Dark Violet"   => HexToColor("#9400D3"),
                "Violet"        => HexToColor("#EE82EE"),
                "Bright Violet" => HexToColor("#DA70D6"),

                "White"      => HexToColor("#FFFFFF"),
                "Light Gray" => HexToColor("#D3D3D3"),
                "Gray"       => HexToColor("#808080"),
                "Dark Gray"  => HexToColor("#A9A9A9"),
                "Black"      => HexToColor("#000000"),

                _ => Color.White
            };
        }

        /// <summary>Convert a hex code string (#RRGGBB) to a XNA Color.</summary>
        private Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex[1..];
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Color(r, g, b);
        }
    }

    /// <summary>Minimal GMCM v1.14+ API interface.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddTextOption(
            IManifest mod,
            Func<string> getValue,
            Action<string> setValue,
            Func<string> name,
            Func<string> tooltip = null,
            string[] allowedValues = null,
            Func<string, string> formatAllowedValue = null,
            string fieldId = null
        );
    }

    /// <summary>Configuration settings for Alon's MOTD mod.</summary>
    public class ModConfig
    {
        /// <summary>The message displayed each morning.</summary>
        public string MessageOfTheDay { get; set; } = "Welcome to the farm!";
        /// <summary>Color name used for the message.</summary>
        public string ColorName { get; set; } = "White";
    }
}
