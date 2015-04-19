using System;
using System.Windows.Forms;

namespace kERP
{
    class Language
    {
        private static InputLanguage En;
        private static InputLanguage Kh;

        public enum Keyboard
        {
            English,
            Khmer
        }

        public void SetKeyboardLayout(InputLanguage layout)
        {
            InputLanguage.CurrentInputLanguage = layout;
        }

        public static void LoadLanguage()
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                string s = lang.Culture.EnglishName.ToLower();
                if (s.StartsWith("english"))
                    En = lang;
                else if (s.StartsWith("catalan") || s.StartsWith("khmer"))
                    Kh = lang;
            }            
        }

        public static void SwitchKeyboard(Keyboard k)
        {
            try
            {
                switch (k)
                {
                    case Keyboard.English:
                        InputLanguage.CurrentInputLanguage = En;
                        break;
                    case Keyboard.Khmer:
                        InputLanguage.CurrentInputLanguage = Kh;
                        break;
                }
            }
            catch { }
        }       

        public static void SwitchToEN()
        {
            SwitchKeyboard(Keyboard.English);
        }

        public static void SwitchToKH()
        {
            SwitchKeyboard(Keyboard.Khmer);
        }
    }
}
