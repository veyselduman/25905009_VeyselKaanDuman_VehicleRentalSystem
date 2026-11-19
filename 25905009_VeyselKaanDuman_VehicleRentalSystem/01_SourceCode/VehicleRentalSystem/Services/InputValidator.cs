using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VehicleRentalSystem.Services
{
    /// <summary>
    /// Kullanıcı girişlerini hem klavye/yapıştırma anında hem de kayıt öncesinde doğrulayan yardımcı sınıf.
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Metin kutusuna yalnızca harf (Türkçe harfler dahil) ve boşluk girilmesine izin verir.
        /// </summary>
        public static void AttachLettersOnlyFilter(TextBox textBox, int maxLength = 35)
        {
            if (textBox == null) return;
            textBox.MaxLength = maxLength;

            textBox.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                }
            };

            textBox.TextChanged += (sender, e) =>
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    string filtered = new string(tb.Text.Where(c => char.IsLetter(c) || c == ' ').ToArray());
                    if (filtered != tb.Text)
                    {
                        int cursor = Math.Max(0, tb.SelectionStart - (tb.Text.Length - filtered.Length));
                        tb.Text = filtered;
                        tb.SelectionStart = Math.Min(cursor, tb.Text.Length);
                    }
                }
            };
        }

        /// <summary>
        /// Araç modeli kutusuna yalnızca harf, boşluk ve en fazla 4 adet rakam (örn. Peugeot 2008, 3008, A4) girilmesine izin verir.
        /// </summary>
        public static void AttachModelFilter(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.MaxLength = 25;

            textBox.KeyPress += (sender, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;

                if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '-')
                {
                    e.Handled = true;
                    return;
                }

                TextBox tb = sender as TextBox;
                if (tb != null && char.IsDigit(e.KeyChar))
                {
                    // Model adı doğrudan rakamla başlayamaz ve toplamda en fazla 4 rakam içerebilir
                    if (tb.Text.Trim().Length == 0 && tb.SelectionStart == 0)
                    {
                        e.Handled = true;
                        return;
                    }
                    int currentDigits = tb.Text.Count(char.IsDigit);
                    int selectedDigits = tb.SelectedText.Count(char.IsDigit);
                    if (currentDigits - selectedDigits >= 4)
                    {
                        e.Handled = true;
                    }
                }
            };

            textBox.TextChanged += (sender, e) =>
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                int digitCount = 0;
                var chars = new System.Collections.Generic.List<char>();
                foreach (char c in tb.Text)
                {
                    if (char.IsLetter(c) || c == ' ' || c == '-')
                    {
                        chars.Add(c);
                    }
                    else if (char.IsDigit(c))
                    {
                        // İlk karakter rakam olamaz ve en fazla 4 rakam bulunabilir
                        if (chars.Any(char.IsLetter) && digitCount < 4)
                        {
                            chars.Add(c);
                            digitCount++;
                        }
                    }
                }

                string filtered = new string(chars.ToArray());
                if (filtered != tb.Text)
                {
                    int cursor = Math.Max(0, tb.SelectionStart - (tb.Text.Length - filtered.Length));
                    tb.Text = filtered;
                    tb.SelectionStart = Math.Min(cursor, tb.Text.Length);
                }
            };
        }

        private static char NormalizePlateChar(char c)
        {
            if (c == 'İ' || c == 'ı' || c == 'i')
                return 'I';
            return char.ToUpperInvariant(c);
        }

        private static bool IsValidTurkishPlateLetter(char c)
        {
            char upper = NormalizePlateChar(c);
            return upper >= 'A' && upper <= 'Z' && upper != 'Q' && upper != 'W' && upper != 'X';
        }

        /// <summary>
        /// Plaka kutusuna yalnızca rakam, standart plaka harfi (A-Z) ve boşluk girilmesine izin verir.
        /// Kayıt anında IsValidPlate ile [01-81 İl Kodu] [1-3 Harf] [2-4 Rakam] doğrulaması ve biçimlendirmesi yapılır.
        /// </summary>
        public static void AttachPlateFilter(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.MaxLength = 12;
            textBox.CharacterCasing = CharacterCasing.Upper;

            textBox.KeyPress += (sender, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;

                if (e.KeyChar == ' ')
                    return;

                if (char.IsDigit(e.KeyChar))
                    return;

                if (IsValidTurkishPlateLetter(e.KeyChar))
                {
                    if (e.KeyChar == 'İ' || e.KeyChar == 'ı' || e.KeyChar == 'i')
                    {
                        e.KeyChar = 'I';
                    }
                    return;
                }

                e.Handled = true;
            };

            bool isCleaning = false;
            textBox.TextChanged += (sender, e) =>
            {
                if (isCleaning) return;
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                isCleaning = true;
                try
                {
                    var chars = new System.Collections.Generic.List<char>();
                    foreach (char raw in tb.Text)
                    {
                        char c = NormalizePlateChar(raw);
                        if (char.IsDigit(c) || IsValidTurkishPlateLetter(c))
                        {
                            chars.Add(c);
                        }
                        else if (c == ' ' && chars.Count > 0 && chars[chars.Count - 1] != ' ')
                        {
                            chars.Add(' ');
                        }
                    }

                    string cleaned = new string(chars.ToArray());
                    if (tb.Text != cleaned)
                    {
                        int cursor = Math.Max(0, tb.SelectionStart - (tb.Text.Length - cleaned.Length));
                        tb.Text = cleaned;
                        tb.SelectionStart = Math.Min(cursor, tb.Text.Length);
                    }
                }
                finally
                {
                    isCleaning = false;
                }
            };
        }

        /// <summary>
        /// Telefon kutusuna yalnızca rakam ve boşluk girilmesine izin verir (en fazla 11 rakam).
        /// </summary>
        public static void AttachPhoneFilter(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.MaxLength = 14;

            textBox.KeyPress += (sender, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                    return;
                }

                TextBox tb = sender as TextBox;
                if (tb != null && char.IsDigit(e.KeyChar))
                {
                    int digitCount = tb.Text.Count(char.IsDigit) - tb.SelectedText.Count(char.IsDigit);
                    if (digitCount >= 11)
                    {
                        e.Handled = true;
                    }
                }
            };

            textBox.TextChanged += (sender, e) =>
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                int digits = 0;
                var chars = new System.Collections.Generic.List<char>();
                foreach (char c in tb.Text)
                {
                    if (char.IsDigit(c) && digits < 11)
                    {
                        chars.Add(c);
                        digits++;
                    }
                    else if (c == ' ' && chars.Count > 0 && chars[chars.Count - 1] != ' ')
                    {
                        chars.Add(c);
                    }
                }

                string filtered = new string(chars.ToArray());
                if (filtered != tb.Text)
                {
                    int cursor = Math.Max(0, tb.SelectionStart - (tb.Text.Length - filtered.Length));
                    tb.Text = filtered;
                    tb.SelectionStart = Math.Min(cursor, tb.Text.Length);
                }
            };
        }

        /// <summary>
        /// Ehliyet numarası kutusuna yalnızca büyük harf, rakam ve tire (-) girilmesine izin verir (en fazla 15 karakter).
        /// </summary>
        public static void AttachLicenseFilter(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.MaxLength = 15;
            textBox.CharacterCasing = CharacterCasing.Upper;

            textBox.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-')
                {
                    e.Handled = true;
                }
            };

            textBox.TextChanged += (sender, e) =>
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    string filtered = new string(tb.Text.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray()).ToUpperInvariant();
                    if (filtered != tb.Text)
                    {
                        int cursor = Math.Max(0, tb.SelectionStart - (tb.Text.Length - filtered.Length));
                        tb.Text = filtered;
                        tb.SelectionStart = Math.Min(cursor, tb.Text.Length);
                    }
                }
            };
        }

        /// <summary>
        /// Kart / Referans numarası kutusuna yalnızca rakam girilmesine izin verir ve 4'lü gruplar halinde biçimlendirir (16 hane).
        /// </summary>
        public static void AttachCardNumberFilter(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.MaxLength = 19; // "XXXX XXXX XXXX XXXX"

            textBox.KeyPress += (sender, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                    return;
                }

                TextBox tb = sender as TextBox;
                if (tb != null && char.IsDigit(e.KeyChar))
                {
                    int digitCount = tb.Text.Count(char.IsDigit) - tb.SelectedText.Count(char.IsDigit);
                    if (digitCount >= 16)
                    {
                        e.Handled = true;
                    }
                }
            };

            bool isFormatting = false;
            textBox.TextChanged += (sender, e) =>
            {
                if (isFormatting) return;
                TextBox tb = sender as TextBox;
                if (tb == null || tb.ReadOnly) return;

                isFormatting = true;
                try
                {
                    string digitsOnly = new string(tb.Text.Where(char.IsDigit).Take(16).ToArray());
                    var groups = new System.Collections.Generic.List<string>();
                    for (int i = 0; i < digitsOnly.Length; i += 4)
                    {
                        int len = Math.Min(4, digitsOnly.Length - i);
                        groups.Add(digitsOnly.Substring(i, len));
                    }
                    string formatted = string.Join(" ", groups.ToArray());
                    if (tb.Text != formatted)
                    {
                        tb.Text = formatted;
                        tb.SelectionStart = tb.Text.Length;
                    }
                }
                finally
                {
                    isFormatting = false;
                }
            };
        }

        /// <summary>
        /// Metin içindeki ardışık fazla boşlukları tek boşluğa indirger.
        /// </summary>
        public static string NormalizeSpaces(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string result = input.Trim();
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }
            return result;
        }

        public static bool IsValidNamePart(string namePart)
        {
            string normalized = NormalizeSpaces(namePart);
            if (string.IsNullOrWhiteSpace(normalized) || normalized.Length < 2 || normalized.Length > 35)
                return false;

            if (!normalized.All(c => char.IsLetter(c) || c == ' '))
                return false;

            string[] words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.All(w => w.Length >= 2);
        }

        public static bool IsValidFullName(string fullName)
        {
            string normalized = NormalizeSpaces(fullName);
            if (string.IsNullOrWhiteSpace(normalized))
                return false;

            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return false;

            return parts.All(p => p.Length >= 2 && p.All(char.IsLetter));
        }

        public static Tuple<string, string> SplitFullName(string fullName)
        {
            string normalized = NormalizeSpaces(fullName);
            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return Tuple.Create(string.Empty, string.Empty);
            if (parts.Length == 1)
                return Tuple.Create(parts[0], string.Empty);

            string lastName = parts[parts.Length - 1];
            string firstName = string.Join(" ", parts.Take(parts.Length - 1));
            return Tuple.Create(firstName, lastName);
        }

        /// <summary>
        /// Türkiye cep telefonu doğrulaması: 05XX XXX XX XX (11 rakam veya başında 0 olmadan 10 rakam 5XXXXXXXXX).
        /// </summary>
        public static bool IsValidPhone(string phone, out string formattedPhone)
        {
            formattedPhone = string.Empty;
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Harf içeriyorsa geçersiz
            if (phone.Any(char.IsLetter))
                return false;

            string digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length == 10 && digits.StartsWith("5"))
            {
                digits = "0" + digits;
            }

            if (digits.Length != 11 || !digits.StartsWith("05"))
                return false;

            formattedPhone = string.Format("{0} {1} {2} {3}",
                digits.Substring(0, 4),
                digits.Substring(4, 3),
                digits.Substring(7, 2),
                digits.Substring(9, 2));
            return true;
        }

        /// <summary>
        /// Ehliyet numarası doğrulaması: Örn. B-381045 veya seri/belge no (en az 4 rakam, 5-15 karakter).
        /// </summary>
        public static bool IsValidLicenseNumber(string license, out string formattedLicense)
        {
            formattedLicense = string.Empty;
            if (string.IsNullOrWhiteSpace(license))
                return false;

            string cleaned = license.Trim().ToUpperInvariant().Replace(" ", "");
            if (cleaned.Length < 5 || cleaned.Length > 15)
                return false;

            if (!cleaned.All(c => char.IsLetterOrDigit(c) || c == '-'))
                return false;

            int digitCount = cleaned.Count(char.IsDigit);
            int letterCount = cleaned.Count(char.IsLetter);
            int dashCount = cleaned.Count(c => c == '-');

            // En az 4 rakam, en fazla 3 harf ve en fazla 1 tire içermelidir (rastgele harf dizilerini engeller)
            if (digitCount < 4 || letterCount > 3 || dashCount > 1)
                return false;

            formattedLicense = cleaned;
            return true;
        }

        /// <summary>
        /// Türkiye standart plaka doğrulaması: [01-81 İl Kodu] [1-3 Harf] [2-4 Rakam] (Örn: 38 DUM 101, 34 ABC 123).
        /// </summary>
        public static bool IsValidPlate(string plate, out string formattedPlate)
        {
            formattedPlate = string.Empty;
            if (string.IsNullOrWhiteSpace(plate))
                return false;

            string cleaned = NormalizeSpaces(plate)
                .Replace('İ', 'I')
                .Replace('ı', 'I')
                .Replace('i', 'I')
                .ToUpperInvariant();
            Match m = Regex.Match(cleaned, @"^(0[1-9]|[1-7][0-9]|8[01])\s*([A-HJ-PR-VZ]{1,3})\s*(\d{2,4})$");
            if (!m.Success)
                return false;

            string city = m.Groups[1].Value;
            string letters = m.Groups[2].Value;
            string digits = m.Groups[3].Value;

            // 3 harfli plakalarda en fazla 3 rakam olur; rakam grubu tamamen sıfır (00, 000) olamaz
            if (letters.Length == 3 && digits.Length > 3)
                return false;
            if (digits.All(c => c == '0'))
                return false;

            formattedPlate = string.Format("{0} {1} {2}", city, letters, digits);
            return true;
        }

        /// <summary>
        /// Araç markası doğrulaması: Yalnızca harf (ve boşluk), en az 2 karakter (rakam içeremez).
        /// </summary>
        public static bool IsValidBrand(string brand)
        {
            string normalized = NormalizeSpaces(brand);
            if (string.IsNullOrWhiteSpace(normalized) || normalized.Length < 2 || normalized.Length > 25)
                return false;

            return normalized.All(c => char.IsLetter(c) || c == ' ');
        }

        /// <summary>
        /// Araç modeli doğrulaması: Harf ile başlamalı, en az 2 harf içermeli ve en fazla 4 rakam barındırmalıdır (Örn: Fiat Egea, Peugeot 2008, Focus).
        /// </summary>
        public static bool IsValidModel(string model)
        {
            string normalized = NormalizeSpaces(model);
            if (string.IsNullOrWhiteSpace(normalized) || normalized.Length < 2 || normalized.Length > 25)
                return false;

            if (!char.IsLetter(normalized[0]))
                return false;

            if (!normalized.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-'))
                return false;

            int letterCount = normalized.Count(char.IsLetter);
            int digitCount = normalized.Count(char.IsDigit);

            return letterCount >= 2 && digitCount <= 4;
        }

        /// <summary>
        /// Ödeme kart / referans numarası doğrulaması:
        /// Kredi/Banka kartında 16 haneli rakam, Havale/EFT'de 8-16 haneli rakam zorunludur (harf kabul edilmez).
        /// </summary>
        public static bool IsValidPaymentReference(string paymentMethod, string input, out string errorMessage)
        {
            errorMessage = string.Empty;
            string method = paymentMethod ?? "Kredi Kartı";

            if (method.IndexOf("Nakit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Kart / Referans numarası alanı boş bırakılamaz.";
                return false;
            }

            if (input.Any(char.IsLetter))
            {
                errorMessage = "Kart / Referans numarası alanında harf kullanılamaz. Lütfen yalnızca rakam giriniz.";
                return false;
            }

            string digits = new string(input.Where(char.IsDigit).ToArray());
            bool isCard = method.IndexOf("Kart", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isCard)
            {
                if (digits.Length != 16)
                {
                    errorMessage = "Kredi / Banka kartı numarası tam 16 haneli rakamdan oluşmalıdır (Örn: 4543 6012 8944 5009).";
                    return false;
                }
                if (digits.Distinct().Count() <= 2)
                {
                    errorMessage = "Girilen kart numarası geçersizdir (tekrarlayan rakam dizisi). Lütfen geçerli bir 16 haneli kart numarası giriniz.";
                    return false;
                }
            }
            else
            {
                if (digits.Length < 8 || digits.Length > 16)
                {
                    errorMessage = "Havale / EFT referans numarası 8 ile 16 haneli rakamdan oluşmalıdır.";
                    return false;
                }
                if (digits.Distinct().Count() == 1)
                {
                    errorMessage = "Referans numarası tamamen aynı rakamlardan oluşamaz.";
                    return false;
                }
            }

            return true;
        }
    }
}
