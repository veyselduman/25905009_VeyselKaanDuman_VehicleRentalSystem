using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Services
{
    /// <summary>
    /// Duman Rent a Car arayüzü için gerçek araç fotoğraflarını (Assets/Vehicles veya gömülü kaynak),
    /// marka rozetlerini ve renk/kategori bilgilerini showroom kartında görüntüler.
    /// </summary>
    public static class CarGraphicsRenderer
    {
        private static readonly Dictionary<string, string> ModelImageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Fiat Egea", "fiat_egea.png" },
            { "Fiat Linea", "fiat_linea.png" },
            { "Volkswagen Passat", "volkswagen_passat.png" },
            { "Volkswagen Jetta", "volkswagen_jetta.png" },
            { "Ford Focus", "ford_focus.png" },
            { "Ford Mondeo", "ford_mondeo.png" },
            { "Volkswagen Tiguan", "volkswagen_tiguan.png" },
            { "Volkswagen Touareg", "volkswagen_touareg.png" },
            { "Ford Kuga", "ford_kuga.png" },
            { "Ford Puma", "ford_puma.png" },
            { "Peugeot 2008", "peugeot_2008.png" },
            { "Peugeot 3008", "peugeot_3008.png" }
        };

        private static string ResolveVehicleFileName(string category, string model)
        {
            string fileName = null;
            if (!string.IsNullOrWhiteSpace(model))
            {
                string exact;
                if (ModelImageMap.TryGetValue(model.Trim(), out exact))
                {
                    fileName = exact;
                }
                else
                {
                    foreach (KeyValuePair<string, string> kv in ModelImageMap)
                    {
                        if (model.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            fileName = kv.Value;
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(fileName))
            {
                bool isSuv = category != null && category.Equals("SUV", StringComparison.OrdinalIgnoreCase);
                fileName = isSuv ? "volkswagen_tiguan.png" : "volkswagen_passat.png";
            }

            return fileName;
        }

        private static Image TryLoadVehicleImage(string category, string model)
        {
            string fileName = ResolveVehicleFileName(category, model);

            string[] probePaths =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Vehicles", fileName),
                Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Vehicles", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Assets", "Vehicles", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "Vehicles", fileName)
            };

            foreach (string candidate in probePaths)
            {
                if (File.Exists(candidate))
                {
                    byte[] bytes = File.ReadAllBytes(candidate);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    using (Image tmp = Image.FromStream(ms))
                    {
                        return new Bitmap(tmp);
                    }
                }
            }

            // Gömülü kaynak (Embedded Resource) yedeği: .exe tek başına veya ZIP içinden çalıştırılsa bile görseller yüklenir
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string resName in asm.GetManifestResourceNames())
            {
                if (resName.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream s = asm.GetManifestResourceStream(resName))
                    {
                        if (s != null)
                        {
                            using (Image tmp = Image.FromStream(s))
                            {
                                return new Bitmap(tmp);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Bitmap RenderCarPreview(string category, string brand, string model, VehicleColor color, int width = 460, int height = 220)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                g.Clear(Color.White);
                using (LinearGradientBrush bottomGrad = new LinearGradientBrush(
                    new Rectangle(0, height - 38, width, 38),
                    Color.FromArgb(244, 247, 252),
                    Color.FromArgb(230, 237, 247),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bottomGrad, 0, height - 38, width, 38);
                }

                bool isSuv = category != null && category.Equals("SUV", StringComparison.OrdinalIgnoreCase);
                using (Image carImg = TryLoadVehicleImage(category, model))
                {
                    if (carImg != null)
                    {
                        int topMargin = 36;
                        int bottomMargin = 38;
                        int sideMargin = 14;

                        int availW = Math.Max(40, width - sideMargin * 2);
                        int availH = Math.Max(40, height - topMargin - bottomMargin);

                        float scale = Math.Min((float)availW / carImg.Width, (float)availH / carImg.Height);
                        int drawW = Math.Max(1, (int)(carImg.Width * scale));
                        int drawH = Math.Max(1, (int)(carImg.Height * scale));
                        int drawX = (width - drawW) / 2;
                        int drawY = topMargin + (availH - drawH) / 2;

                        g.DrawImage(carImg, new Rectangle(drawX, drawY, drawW, drawH));
                    }
                    else
                    {
                        using (SolidBrush cardBg = new SolidBrush(Color.FromArgb(245, 248, 253)))
                        using (Pen cardPen = new Pen(Color.FromArgb(210, 220, 234), 1.5f))
                        {
                            Rectangle infoRect = new Rectangle(24, 48, width - 48, height - 96);
                            g.FillRectangle(cardBg, infoRect);
                            g.DrawRectangle(cardPen, infoRect);

                            using (Font hf = new Font("Segoe UI", 12f, FontStyle.Bold))
                            using (Font sf = new Font("Segoe UI", 9f, FontStyle.Regular))
                            using (SolidBrush nb = new SolidBrush(Color.FromArgb(18, 34, 62)))
                            using (SolidBrush mb = new SolidBrush(Color.FromArgb(95, 108, 128)))
                            {
                                string title = string.IsNullOrWhiteSpace(model) ? brand : model;
                                SizeF tsz = g.MeasureString(title, hf);
                                g.DrawString(title, hf, nb, (width - tsz.Width) / 2f, infoRect.Y + infoRect.Height / 2f - 22);

                                string sub = "Duman Rent a Car Filo Aracı";
                                SizeF ssz = g.MeasureString(sub, sf);
                                g.DrawString(sub, sf, mb, (width - ssz.Width) / 2f, infoRect.Y + infoRect.Height / 2f + 4);
                            }
                        }
                    }
                }

                DrawBadge(g, isSuv ? "SUV KATEGORİSİ" : "SEDAN KATEGORİSİ", 10, 8, Color.FromArgb(18, 34, 62), Color.White);
                DrawColorBadge(g, color, width - 10, 8);

                string labelText = string.IsNullOrWhiteSpace(model)
                    ? (isSuv ? "SUV Filosu" : "Sedan Filosu")
                    : model;

                using (Font f = new Font("Segoe UI", 10.5f, FontStyle.Bold))
                using (SolidBrush tb = new SolidBrush(Color.FromArgb(18, 34, 62)))
                {
                    SizeF sz = g.MeasureString(labelText, f);
                    g.DrawString(labelText, f, tb, (width - sz.Width) / 2f, height - 28);
                }

                using (Pen border = new Pen(Color.FromArgb(212, 220, 232), 1.5f))
                {
                    g.DrawRectangle(border, 0, 0, width - 1, height - 1);
                }
            }
            return bmp;
        }

        private static void DrawBadge(Graphics g, string text, float x, float y, Color bgColor, Color fgColor)
        {
            using (Font f = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(text, f);
                RectangleF rect = new RectangleF(x, y, sz.Width + 14, sz.Height + 5);
                using (SolidBrush br = new SolidBrush(bgColor))
                    g.FillRectangle(br, rect);
                using (SolidBrush fg = new SolidBrush(fgColor))
                    g.DrawString(text, f, fg, x + 7, y + 2);
            }
        }

        private static void DrawColorBadge(Graphics g, VehicleColor color, float rightX, float y)
        {
            string text = string.Format("Renk: {0}", color);
            Color swatch = color == VehicleColor.Siyah
                ? Color.FromArgb(26, 30, 38)
                : (color == VehicleColor.Beyaz ? Color.White : Color.FromArgb(132, 142, 156));

            using (Font f = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(text, f);
                float totalW = sz.Width + 30;
                float x = rightX - totalW;
                RectangleF rect = new RectangleF(x, y, totalW, sz.Height + 5);
                using (SolidBrush br = new SolidBrush(Color.FromArgb(215, 165, 42)))
                    g.FillRectangle(br, rect);

                float dotY = y + (rect.Height - 10) / 2f;
                using (SolidBrush sb = new SolidBrush(swatch))
                    g.FillEllipse(sb, x + 7, dotY, 10, 10);
                using (Pen sp = new Pen(Color.FromArgb(40, 48, 62), 1f))
                    g.DrawEllipse(sp, x + 7, dotY, 10, 10);

                using (SolidBrush fg = new SolidBrush(Color.FromArgb(18, 28, 44)))
                    g.DrawString(text, f, fg, x + 22, y + 2);
            }
        }

        public static Bitmap RenderBrandBadge(string brandName, int width = 148, int height = 62)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Color primary;
                string shortMark;
                switch (brandName.ToLowerInvariant())
                {
                    case "fiat":
                        primary = Color.FromArgb(168, 22, 34);
                        shortMark = "FIAT";
                        break;
                    case "volkswagen":
                        primary = Color.FromArgb(18, 48, 96);
                        shortMark = "VW";
                        break;
                    case "ford":
                        primary = Color.FromArgb(20, 74, 152);
                        shortMark = "FORD";
                        break;
                    default:
                        primary = Color.FromArgb(30, 36, 48);
                        shortMark = "PEUGEOT";
                        break;
                }

                g.Clear(Color.White);
                using (Pen border = new Pen(Color.FromArgb(216, 224, 236), 1.5f))
                {
                    g.DrawRectangle(border, 1, 1, width - 3, height - 3);
                }

                g.FillEllipse(new SolidBrush(primary), 10, 11, 40, 40);
                using (Pen ring = new Pen(Color.FromArgb(228, 234, 242), 2f))
                    g.DrawEllipse(ring, 12, 13, 36, 36);

                string mono = shortMark.Length <= 4 ? shortMark : shortMark.Substring(0, 1);
                using (Font mf = new Font("Segoe UI", mono.Length > 2 ? 7.5f : 11f, FontStyle.Bold))
                using (SolidBrush wb = new SolidBrush(Color.White))
                {
                    SizeF msz = g.MeasureString(mono, mf);
                    g.DrawString(mono, mf, wb, 30 - msz.Width / 2f, 31 - msz.Height / 2f);
                }

                using (Font bf = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (SolidBrush tb = new SolidBrush(Color.FromArgb(26, 36, 52)))
                {
                    g.DrawString(brandName, bf, tb, 56, 15);
                }

                using (Font sf = new Font("Segoe UI", 7.2f, FontStyle.Regular))
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(108, 120, 138)))
                {
                    string sub = brandName == "Fiat" ? "Sedan Filosu"
                        : brandName == "Peugeot" ? "SUV Filosu"
                        : "Sedan & SUV";
                    g.DrawString(sub, sf, sb, 56, 34);
                }
            }
            return bmp;
        }
    }
}
