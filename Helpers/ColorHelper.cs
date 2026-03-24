using System.Drawing;

namespace PropertyManagementSystem.Helpers
{
    public static class ColorHelper
    {
        // Primary Colors
        public static Color PrimaryBlue = Color.FromArgb(0, 120, 212);
        public static Color PrimaryGreen = Color.FromArgb(16, 124, 16);

        // Secondary Colors
        public static Color SecondaryBlue = Color.FromArgb(50, 160, 232);
        public static Color SecondaryGreen = Color.FromArgb(46, 164, 46);

        // Background Colors
        public static Color BackgroundLight = Color.FromArgb(243, 242, 241);
        public static Color BackgroundWhite = Color.White;

        // Text Colors
        public static Color TextDark = Color.FromArgb(50, 49, 48);
        public static Color TextGray = Color.FromArgb(96, 94, 92);
        public static Color TextLight = Color.White;

        // Status Colors
        public static Color Success = Color.FromArgb(16, 124, 16);
        public static Color Warning = Color.FromArgb(255, 185, 0);
        public static Color Error = Color.FromArgb(196, 43, 28);
        public static Color Info = Color.FromArgb(0, 120, 212);
    }
}