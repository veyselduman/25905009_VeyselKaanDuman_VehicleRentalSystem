using System;
using System.Drawing;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Duman Rent a Car başlangıç ve müşteri giriş ekranı.
    /// Marka görselleri, araç kategorisi önizlemeleri, harf kısıtlı ad-soyad alanı ve danışman seçimi içerir.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly RentalManager _manager;
        private TextBox _txtFullName;
        private ComboBox _cmbConsultant;
        private Button _btnLogin;

        public LoginForm(RentalManager manager)
        {
            _manager = manager;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Sisteme Giriş";
            Size = new Size(980, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            // Üst Şerit: Öğrenci Bilgisi ve Marka Adı
            Panel topBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,
                BackColor = Color.FromArgb(18, 34, 62)
            };
            Controls.Add(topBanner);

            Label lblBrand = new Label
            {
                Text = "DUMAN RENT A CAR",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(26, 12)
            };
            topBanner.Controls.Add(lblBrand);

            Label lblTag = new Label
            {
                Text = "Araç Kiralama Yönetim Sistemi",
                ForeColor = Color.FromArgb(215, 175, 62),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(28, 45)
            };
            topBanner.Controls.Add(lblTag);

            Label lblStudent = new Label
            {
                Text = string.Format("Veysel Kaan Düman  |  Öğrenci No: {0}", RentalManager.StudentNumber),
                ForeColor = Color.FromArgb(228, 236, 248),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(620, 26)
            };
            topBanner.Controls.Add(lblStudent);

            // Sol Showcase Paneli: Araç Görselleri ve Marka Logoları
            Panel leftPanel = new Panel
            {
                Location = new Point(24, 96),
                Size = new Size(520, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(leftPanel);

            Label lblShowcaseTitle = new Label
            {
                Text = "Filomuzdaki Seçkin Marka ve Araç Kategorileri",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(18, 16)
            };
            leftPanel.Controls.Add(lblShowcaseTitle);

            // Sedan ve SUV tanıtım görselleri
            PictureBox picSedan = new PictureBox
            {
                Location = new Point(18, 52),
                Size = new Size(236, 148),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Image = CarGraphicsRenderer.RenderCarPreview("Sedan", "Fiat", "Fiat Egea", VehicleColor.Siyah, 340, 210)
            };
            leftPanel.Controls.Add(picSedan);

            PictureBox picSuv = new PictureBox
            {
                Location = new Point(264, 52),
                Size = new Size(236, 148),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Image = CarGraphicsRenderer.RenderCarPreview("SUV", "Peugeot", "Peugeot 2008", VehicleColor.Gri, 340, 210)
            };
            leftPanel.Controls.Add(picSuv);

            Label lblCatDesc = new Label
            {
                Text = "Sedan: Fiat Egea & Linea, VW Passat & Jetta, Ford Focus & Mondeo\r\nSUV: VW Tiguan & Touareg, Ford Kuga & Puma, Peugeot 2008 & 3008",
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(72, 84, 102),
                Location = new Point(18, 210),
                Size = new Size(482, 42)
            };
            leftPanel.Controls.Add(lblCatDesc);

            Label lblBrandsHeader = new Label
            {
                Text = "Anlaşmalı Araç Markalarımız",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(18, 266)
            };
            leftPanel.Controls.Add(lblBrandsHeader);

            string[] brands = { "Fiat", "Volkswagen", "Ford", "Peugeot" };
            for (int i = 0; i < brands.Length; i++)
            {
                int col = i % 2;
                int row = i / 2;
                PictureBox pbBrand = new PictureBox
                {
                    Location = new Point(18 + col * 246, 298 + row * 82),
                    Size = new Size(234, 70),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = CarGraphicsRenderer.RenderBrandBadge(brands[i], 234, 70)
                };
                leftPanel.Controls.Add(pbBrand);
            }

            Label lblColorsInfo = new Label
            {
                Text = "Tüm modellerde Siyah, Beyaz ve Gri renk seçenekleri mevcuttur.",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(95, 108, 128),
                AutoSize = true,
                Location = new Point(18, 466)
            };
            leftPanel.Controls.Add(lblColorsInfo);

            // Sağ Giriş Kartı
            Panel rightPanel = new Panel
            {
                Location = new Point(564, 96),
                Size = new Size(378, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(rightPanel);

            Label lblLoginTitle = new Label
            {
                Text = "Müşteri Girişi",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(28, 32)
            };
            rightPanel.Controls.Add(lblLoginTitle);

            Label lblSub = new Label
            {
                Text = "Kiralama işlemlerine başlamak için bilgilerinizi girip\r\nkiralama danışmanınızı seçiniz.",
                Font = new Font("Segoe UI", 9.2f),
                ForeColor = Color.FromArgb(96, 110, 128),
                Location = new Point(28, 68),
                Size = new Size(320, 42)
            };
            rightPanel.Controls.Add(lblSub);

            // Ad Soyad etiketi (yalnızca alan adı gösterilir)
            Label lblName = new Label
            {
                Text = "Ad Soyad",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 42, 62),
                AutoSize = true,
                Location = new Point(28, 140)
            };
            rightPanel.Controls.Add(lblName);

            _txtFullName = new TextBox
            {
                Location = new Point(28, 168),
                Size = new Size(320, 32),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle
            };
            InputValidator.AttachLettersOnlyFilter(_txtFullName);
            rightPanel.Controls.Add(_txtFullName);

            // Kiralama Danışmanı Seçimi (Veysel Duman / Kaan Duman)
            Label lblConsultant = new Label
            {
                Text = "Kiralama Danışmanı",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 42, 62),
                AutoSize = true,
                Location = new Point(28, 224)
            };
            rightPanel.Controls.Add(lblConsultant);

            _cmbConsultant = new ComboBox
            {
                Location = new Point(28, 252),
                Size = new Size(320, 32),
                Font = new Font("Segoe UI", 10.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (RentalConsultant c in _manager.GetConsultants())
            {
                _cmbConsultant.Items.Add(c);
            }
            if (_cmbConsultant.Items.Count > 0)
                _cmbConsultant.SelectedIndex = 0;
            rightPanel.Controls.Add(_cmbConsultant);

            // Bilgi kutusu
            Panel infoCard = new Panel
            {
                Location = new Point(28, 306),
                Size = new Size(320, 72),
                BackColor = Color.FromArgb(240, 245, 254),
                BorderStyle = BorderStyle.FixedSingle
            };
            rightPanel.Controls.Add(infoCard);

            Label lblInfoText = new Label
            {
                Text = "Duman Rent a Car Yetkili Danışmanları:\r\n• Veysel Duman\r\n• Kaan Duman",
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(36, 56, 88),
                Location = new Point(12, 10),
                Size = new Size(296, 54)
            };
            infoCard.Controls.Add(lblInfoText);

            _btnLogin = new Button
            {
                Text = "Sisteme Giriş Yap  ->",
                Location = new Point(28, 404),
                Size = new Size(320, 46),
                BackColor = Color.FromArgb(18, 52, 106),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnLogin.FlatAppearance.BorderSize = 0;
            _btnLogin.Click += BtnLogin_Click;
            rightPanel.Controls.Add(_btnLogin);

            AcceptButton = _btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string fullName = _txtFullName.Text.Trim();
                if (!InputValidator.IsValidFullName(fullName))
                {
                    MessageBox.Show(
                        "Lütfen geçerli bir Ad ve Soyad bilgisi giriniz.",
                        "Eksik Bilgi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    _txtFullName.Focus();
                    return;
                }

                _manager.RegisterOrGetLoginCustomer(fullName);
                _manager.ActiveConsultant = _cmbConsultant.SelectedItem as RentalConsultant;

                Hide();
                using (MainDashboardForm dashboard = new MainDashboardForm(_manager))
                {
                    dashboard.ShowDialog();
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
