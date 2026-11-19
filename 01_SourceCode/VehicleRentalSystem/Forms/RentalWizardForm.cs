using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Adım adım Araç Kiralama ve Ödeme Onay Sihirbazı:
    /// 1. Kategori Seçimi (Sedan / SUV)
    /// 2. Marka, Model ve Renk (Siyah / Beyaz / Gri) Seçimi + Canlı Araç Önizleme
    /// 3. Kiralama Süresi (1 Gün / 1 Hafta / 1 Ay + Tarih Seçimi) ve Fiyat Hesaplama
    /// 4. Ödeme Onayı ve Kiralama Tamamlama Makbuzu
    /// </summary>
    public class RentalWizardForm : Form
    {
        private readonly RentalManager _manager;
        private int _currentStep = 1;

        private string _selectedCategory = "Sedan";
        private VehicleColor _selectedColor = VehicleColor.Siyah;
        private string _selectedPackage = "1 Gün";
        private Vehicle _selectedVehicle;

        private Panel _stepIndicatorPanel;
        private Panel _pnlStep1;
        private Panel _pnlStep2;
        private Panel _pnlStep3;
        private Panel _pnlStep4;

        private Button _btnBack;
        private Button _btnNext;

        // Adım 1 kontrolleri
        private Panel _cardSedan;
        private Panel _cardSuv;

        // Adım 2 kontrolleri
        private ComboBox _cmbBrand;
        private ComboBox _cmbModel;
        private RadioButton _rbSiyah;
        private RadioButton _rbBeyaz;
        private RadioButton _rbGri;
        private PictureBox _picVehiclePreview;
        private Label _lblVehicleInfo;
        private Label _lblAvailability;

        // Adım 3 kontrolleri
        private ComboBox _cmbCustomer;
        private ComboBox _cmbConsultant;
        private RadioButton _rbOneDay;
        private RadioButton _rbOneWeek;
        private RadioButton _rbOneMonth;
        private DateTimePicker _dtpStart;
        private DateTimePicker _dtpEnd;
        private Label _lblPriceBreakdown;
        private Label _lblTotalPrice;

        // Adım 4 (Ödeme) kontrolleri
        private Label _lblPaymentSummary;
        private ComboBox _cmbPaymentMethod;
        private TextBox _txtCardHolder;
        private TextBox _txtCardNumber;

        public RentalWizardForm(RentalManager manager, string preselectedCategory = null)
        {
            _manager = manager;
            if (!string.IsNullOrWhiteSpace(preselectedCategory))
                _selectedCategory = preselectedCategory;

            InitializeUI();
            ShowStep(1);
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Araç Kiralama ve Ödeme";
            Size = new Size(980, 680);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            // Üst Başlık
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 62,
                BackColor = Color.FromArgb(18, 34, 62)
            };
            Controls.Add(topBar);

            Label lblHead = new Label
            {
                Text = "DUMAN RENT A CAR  |  Araç Kiralama Sihirbazı",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };
            topBar.Controls.Add(lblHead);

            Label lblStudent = new Label
            {
                Text = "Veysel Kaan Düman | 25905009",
                ForeColor = Color.FromArgb(215, 175, 62),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(730, 22)
            };
            topBar.Controls.Add(lblStudent);

            // Adım Göstergesi
            _stepIndicatorPanel = new Panel
            {
                Location = new Point(22, 74),
                Size = new Size(920, 44),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _stepIndicatorPanel.Paint += StepIndicator_Paint;
            Controls.Add(_stepIndicatorPanel);

            // İçerik Panelleri
            BuildStep1Panel();
            BuildStep2Panel();
            BuildStep3Panel();
            BuildStep4Panel();

            // Alt Gezinme Butonları
            _btnBack = new Button
            {
                Text = "<- Önceki Adım",
                Location = new Point(22, 586),
                Size = new Size(160, 42),
                BackColor = Color.FromArgb(214, 222, 234),
                ForeColor = Color.FromArgb(24, 36, 56),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnBack.FlatAppearance.BorderSize = 0;
            _btnBack.Click += (s, e) =>
            {
                if (_currentStep > 1) ShowStep(_currentStep - 1);
                else Close();
            };
            Controls.Add(_btnBack);

            _btnNext = new Button
            {
                Text = "Sonraki Adım ->",
                Location = new Point(722, 586),
                Size = new Size(220, 42),
                BackColor = Color.FromArgb(18, 66, 134),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnNext.FlatAppearance.BorderSize = 0;
            _btnNext.Click += BtnNext_Click;
            Controls.Add(_btnNext);
        }

        private void StepIndicator_Paint(object sender, PaintEventArgs e)
        {
            string[] steps =
            {
                "1. Kategori (Sedan/SUV)",
                "2. Marka, Model & Renk",
                "3. Süre & Fiyat Hesabı",
                "4. Ödeme Onayı"
            };
            int segW = _stepIndicatorPanel.Width / steps.Length;
            for (int i = 0; i < steps.Length; i++)
            {
                int stepNum = i + 1;
                Rectangle rect = new Rectangle(i * segW, 0, segW, _stepIndicatorPanel.Height);
                Color bg = stepNum == _currentStep
                    ? Color.FromArgb(18, 66, 134)
                    : (stepNum < _currentStep ? Color.FromArgb(22, 138, 86) : Color.White);
                Color fg = stepNum <= _currentStep ? Color.White : Color.FromArgb(100, 114, 132);

                using (SolidBrush br = new SolidBrush(bg))
                    e.Graphics.FillRectangle(br, rect);

                TextRenderer.DrawText(
                    e.Graphics,
                    steps[i],
                    new Font("Segoe UI", 9.2f, FontStyle.Bold),
                    rect,
                    fg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        // --- ADIM 1: SEDAN VE SUV KATEGORİ SEÇİMİ ---
        private void BuildStep1Panel()
        {
            _pnlStep1 = new Panel
            {
                Location = new Point(22, 128),
                Size = new Size(920, 444),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(_pnlStep1);

            Label lblTitle = new Label
            {
                Text = "Kiralamak İstediğiniz Araç Kategorisini Seçiniz",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(26, 20)
            };
            _pnlStep1.Controls.Add(lblTitle);

            _cardSedan = CreateCategoryCard(
                "Sedan",
                "SEDAN KATEGORİSİ",
                "Markalar: Fiat (Egea, Linea), Volkswagen (Passat, Jetta), Ford (Focus, Mondeo)\r\nKonforlu şehir içi ve uzun yol sürüşü • Günlük 1.150 TL'den başlayan fiyatlar",
                26, 70);
            _pnlStep1.Controls.Add(_cardSedan);

            _cardSuv = CreateCategoryCard(
                "SUV",
                "SUV KATEGORİSİ",
                "Markalar: Volkswagen (Tiguan, Touareg), Ford (Kuga, Puma), Peugeot (2008, 3008)\r\nYüksek sürüş pozisyonu ve geniş hacim • Günlük 2.150 TL'den başlayan fiyatlar",
                474, 70);
            _pnlStep1.Controls.Add(_cardSuv);

            HighlightCategoryCard();
        }

        private Panel CreateCategoryCard(string category, string title, string desc, int x, int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(420, 346),
                BackColor = Color.FromArgb(248, 250, 254),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };

            PictureBox pic = new PictureBox
            {
                Location = new Point(14, 14),
                Size = new Size(390, 200),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = CarGraphicsRenderer.RenderCarPreview(
                    category,
                    category == "Sedan" ? "Volkswagen" : "Peugeot",
                    category == "Sedan" ? "Ford Focus" : "Ford Kuga",
                    category == "Sedan" ? VehicleColor.Beyaz : VehicleColor.Siyah,
                    390, 200),
                Cursor = Cursors.Hand
            };
            card.Controls.Add(pic);

            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                Location = new Point(14, 224),
                AutoSize = true
            };
            card.Controls.Add(lblT);

            Label lblD = new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(72, 86, 106),
                Location = new Point(14, 256),
                Size = new Size(390, 74)
            };
            card.Controls.Add(lblD);

            EventHandler selectHandler = (s, e) =>
            {
                _selectedCategory = category;
                HighlightCategoryCard();
            };

            card.Click += selectHandler;
            pic.Click += selectHandler;
            lblT.Click += selectHandler;
            lblD.Click += selectHandler;

            return card;
        }

        private void HighlightCategoryCard()
        {
            _cardSedan.BackColor = _selectedCategory == "Sedan" ? Color.FromArgb(224, 238, 255) : Color.FromArgb(248, 250, 254);
            _cardSuv.BackColor = _selectedCategory == "SUV" ? Color.FromArgb(224, 238, 255) : Color.FromArgb(248, 250, 254);
        }

        // --- ADIM 2: MARKA, MODEL VE RENK SEÇİMİ ---
        private void BuildStep2Panel()
        {
            _pnlStep2 = new Panel
            {
                Location = new Point(22, 128),
                Size = new Size(920, 444),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            Controls.Add(_pnlStep2);

            Label lblTitle = new Label
            {
                Text = "Marka, Model ve Renk Seçimi",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(26, 18)
            };
            _pnlStep2.Controls.Add(lblTitle);

            Label lblBrand = new Label { Text = "Araç Markası", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 66), AutoSize = true };
            _pnlStep2.Controls.Add(lblBrand);

            _cmbBrand = new ComboBox
            {
                Location = new Point(26, 92),
                Size = new Size(330, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5f)
            };
            _cmbBrand.SelectedIndexChanged += (s, e) => PopulateModels();
            _pnlStep2.Controls.Add(_cmbBrand);

            Label lblModel = new Label { Text = "Araç Modeli", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 144), AutoSize = true };
            _pnlStep2.Controls.Add(lblModel);

            _cmbModel = new ComboBox
            {
                Location = new Point(26, 170),
                Size = new Size(330, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5f)
            };
            _cmbModel.SelectedIndexChanged += (s, e) => OnModelSelectionChanged();
            _pnlStep2.Controls.Add(_cmbModel);

            Label lblColor = new Label { Text = "Araç Rengi (3 Seçenek)", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 224), AutoSize = true };
            _pnlStep2.Controls.Add(lblColor);

            _rbSiyah = new RadioButton { Text = "Siyah", Location = new Point(28, 256), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            _rbBeyaz = new RadioButton { Text = "Beyaz", Location = new Point(136, 256), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            _rbGri = new RadioButton { Text = "Gri", Location = new Point(246, 256), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };

            _rbSiyah.CheckedChanged += (s, e) => { if (_rbSiyah.Checked) { _selectedColor = VehicleColor.Siyah; UpdateVehiclePreview(); } };
            _rbBeyaz.CheckedChanged += (s, e) => { if (_rbBeyaz.Checked) { _selectedColor = VehicleColor.Beyaz; UpdateVehiclePreview(); } };
            _rbGri.CheckedChanged += (s, e) => { if (_rbGri.Checked) { _selectedColor = VehicleColor.Gri; UpdateVehiclePreview(); } };

            _pnlStep2.Controls.Add(_rbSiyah);
            _pnlStep2.Controls.Add(_rbBeyaz);
            _pnlStep2.Controls.Add(_rbGri);

            _lblVehicleInfo = new Label
            {
                Location = new Point(26, 304),
                Size = new Size(330, 74),
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(44, 58, 78)
            };
            _pnlStep2.Controls.Add(_lblVehicleInfo);

            _lblAvailability = new Label
            {
                Location = new Point(26, 386),
                Size = new Size(330, 32),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            _pnlStep2.Controls.Add(_lblAvailability);

            _picVehiclePreview = new PictureBox
            {
                Location = new Point(386, 66),
                Size = new Size(504, 340),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };
            _pnlStep2.Controls.Add(_picVehiclePreview);
        }

        private void PopulateBrands()
        {
            _cmbBrand.Items.Clear();
            foreach (string brand in _manager.GetBrandsByCategory(_selectedCategory))
                _cmbBrand.Items.Add(brand);

            if (_cmbBrand.Items.Count > 0)
                _cmbBrand.SelectedIndex = 0;
        }

        private void PopulateModels()
        {
            _cmbModel.Items.Clear();
            if (_cmbBrand.SelectedItem == null) return;

            string brand = _cmbBrand.SelectedItem.ToString();
            foreach (string model in _manager.GetModelsByCategoryAndBrand(_selectedCategory, brand))
                _cmbModel.Items.Add(model);

            if (_cmbModel.Items.Count > 0)
                _cmbModel.SelectedIndex = 0;
        }

        private void OnModelSelectionChanged()
        {
            if (_cmbBrand.SelectedItem == null || _cmbModel.SelectedItem == null) return;
            string brand = _cmbBrand.SelectedItem.ToString();
            string model = _cmbModel.SelectedItem.ToString();

            // Seçili renkte müsait araç yoksa, bu modelin müsait olan ilk rengini otomatik seç
            Vehicle currentColorAvail = _manager.FindAvailableVehicle(_selectedCategory, brand, model, _selectedColor);
            if (currentColorAvail == null)
            {
                var specVehicles = _manager.GetVehiclesBySpec(_selectedCategory, brand, model);
                Vehicle firstAvail = specVehicles.FirstOrDefault(v => v.IsAvailable) ?? specVehicles.FirstOrDefault();
                if (firstAvail != null && firstAvail.Color != _selectedColor)
                {
                    _selectedColor = firstAvail.Color;
                    _rbSiyah.Checked = (_selectedColor == VehicleColor.Siyah);
                    _rbBeyaz.Checked = (_selectedColor == VehicleColor.Beyaz);
                    _rbGri.Checked = (_selectedColor == VehicleColor.Gri);
                }
            }

            UpdateVehiclePreview();
        }

        private void UpdateVehiclePreview()
        {
            if (_cmbBrand.SelectedItem == null || _cmbModel.SelectedItem == null) return;
            string brand = _cmbBrand.SelectedItem.ToString();
            string model = _cmbModel.SelectedItem.ToString();

            if (_picVehiclePreview.Image != null)
            {
                _picVehiclePreview.Image.Dispose();
            }
            _picVehiclePreview.Image = CarGraphicsRenderer.RenderCarPreview(_selectedCategory, brand, model, _selectedColor, 504, 340);

            _selectedVehicle = _manager.FindAvailableVehicle(_selectedCategory, brand, model, _selectedColor);
            if (_selectedVehicle != null)
            {
                _lblVehicleInfo.Text = string.Format("Plaka: {0}\r\nModel Yılı: {1}\r\nGünlük Kira Ücreti: {2:N0} TL",
                    _selectedVehicle.PlateNumber, _selectedVehicle.Year, _selectedVehicle.DailyRate);
                _lblAvailability.Text = "Durum: MÜSAİT (Kiralanabilir)";
                _lblAvailability.ForeColor = Color.FromArgb(20, 134, 78);
            }
            else
            {
                Vehicle rentedMatch = _manager.FindAnyVehicleBySpec(_selectedCategory, brand, model, _selectedColor);
                if (rentedMatch != null)
                {
                    _lblVehicleInfo.Text = string.Format("Plaka: {0} | Yıl: {1}\r\nBu renk/model şu anda başka bir müşteridedir.", rentedMatch.PlateNumber, rentedMatch.Year);
                    _lblAvailability.Text = "Durum: KİRADA (Müsait Değil)";
                }
                else
                {
                    _lblVehicleInfo.Text = "Seçilen modelin bu renk seçeneği filoda kayıtlı değildir.\r\nLütfen diğer renk seçeneklerini kontrol ediniz.";
                    _lblAvailability.Text = "Durum: BU RENK FİLODA YOK";
                }
                _lblAvailability.ForeColor = Color.FromArgb(196, 36, 48);
            }
        }

        // --- ADIM 3: KİRALAMA SÜRESİ (1 GÜN / 1 HAFTA / 1 AY) VE ÜCRET HESABI ---
        private void BuildStep3Panel()
        {
            _pnlStep3 = new Panel
            {
                Location = new Point(22, 128),
                Size = new Size(920, 444),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            Controls.Add(_pnlStep3);

            Label lblTitle = new Label
            {
                Text = "Kiralama Süresi, Müşteri ve Ücret Hesaplama",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(26, 18)
            };
            _pnlStep3.Controls.Add(lblTitle);

            Label lblCust = new Label { Text = "Müşteri", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 66), AutoSize = true };
            _pnlStep3.Controls.Add(lblCust);
            _cmbCustomer = new ComboBox { Location = new Point(26, 92), Size = new Size(380, 32), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            _pnlStep3.Controls.Add(_cmbCustomer);

            Label lblCons = new Label { Text = "Kiralama Danışmanı", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(454, 66), AutoSize = true };
            _pnlStep3.Controls.Add(lblCons);
            _cmbConsultant = new ComboBox { Location = new Point(454, 92), Size = new Size(430, 32), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            _pnlStep3.Controls.Add(_cmbConsultant);

            Label lblPkg = new Label { Text = "Kiralama Süresi Paketi", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 148), AutoSize = true };
            _pnlStep3.Controls.Add(lblPkg);

            _rbOneDay = new RadioButton { Text = "1 Gün", Location = new Point(28, 180), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };
            _rbOneWeek = new RadioButton { Text = "1 Hafta (7 Gün)", Location = new Point(150, 180), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };
            _rbOneMonth = new RadioButton { Text = "1 Ay (30 Gün)", Location = new Point(320, 180), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };

            _rbOneDay.CheckedChanged += (s, e) => { if (_rbOneDay.Checked) ApplyDurationPackage("1 Gün", 1); };
            _rbOneWeek.CheckedChanged += (s, e) => { if (_rbOneWeek.Checked) ApplyDurationPackage("1 Hafta", 7); };
            _rbOneMonth.CheckedChanged += (s, e) => { if (_rbOneMonth.Checked) ApplyDurationPackage("1 Ay", 30); };

            _pnlStep3.Controls.Add(_rbOneDay);
            _pnlStep3.Controls.Add(_rbOneWeek);
            _pnlStep3.Controls.Add(_rbOneMonth);

            Label lblStart = new Label { Text = "Başlangıç Tarihi", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(26, 232), AutoSize = true };
            _pnlStep3.Controls.Add(lblStart);
            _dtpStart = new DateTimePicker { Location = new Point(26, 258), Size = new Size(240, 30), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            _dtpStart.ValueChanged += (s, e) => RecalculatePrice();
            _pnlStep3.Controls.Add(_dtpStart);

            Label lblEnd = new Label { Text = "Bitiş Tarihi", Font = new Font("Segoe UI", 10f, FontStyle.Bold), Location = new Point(304, 232), AutoSize = true };
            _pnlStep3.Controls.Add(lblEnd);
            _dtpEnd = new DateTimePicker { Location = new Point(304, 258), Size = new Size(240, 30), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(1) };
            _dtpEnd.ValueChanged += (s, e) => RecalculatePrice();
            _pnlStep3.Controls.Add(_dtpEnd);

            Panel priceBox = new Panel
            {
                Location = new Point(26, 314),
                Size = new Size(860, 108),
                BackColor = Color.FromArgb(240, 246, 255),
                BorderStyle = BorderStyle.FixedSingle
            };
            _pnlStep3.Controls.Add(priceBox);

            _lblPriceBreakdown = new Label
            {
                Location = new Point(16, 14),
                Size = new Size(824, 34),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(38, 56, 86)
            };
            priceBox.Controls.Add(_lblPriceBreakdown);

            _lblTotalPrice = new Label
            {
                Location = new Point(16, 52),
                Size = new Size(824, 42),
                Font = new Font("Segoe UI", 17f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 66, 134)
            };
            priceBox.Controls.Add(_lblTotalPrice);
        }

        private void LoadStep3Data()
        {
            _cmbCustomer.Items.Clear();
            foreach (Customer c in _manager.GetAllCustomers())
                _cmbCustomer.Items.Add(c);
            if (_manager.ActiveCustomer != null)
                _cmbCustomer.SelectedItem = _manager.ActiveCustomer;
            else if (_cmbCustomer.Items.Count > 0)
                _cmbCustomer.SelectedIndex = 0;

            _cmbConsultant.Items.Clear();
            foreach (RentalConsultant rc in _manager.GetConsultants())
                _cmbConsultant.Items.Add(rc);
            if (_manager.ActiveConsultant != null)
                _cmbConsultant.SelectedItem = _manager.ActiveConsultant;
            else if (_cmbConsultant.Items.Count > 0)
                _cmbConsultant.SelectedIndex = 0;

            ApplyDurationPackage(_rbOneMonth.Checked ? "1 Ay" : (_rbOneWeek.Checked ? "1 Hafta" : "1 Gün"),
                _rbOneMonth.Checked ? 30 : (_rbOneWeek.Checked ? 7 : 1));
        }

        private void ApplyDurationPackage(string label, int days)
        {
            _selectedPackage = label;
            _dtpEnd.Value = _dtpStart.Value.Date.AddDays(days);
            RecalculatePrice();
        }

        private void RecalculatePrice()
        {
            if (_selectedVehicle == null) return;
            if (_dtpEnd.Value.Date < _dtpStart.Value.Date)
            {
                _lblPriceBreakdown.Text = "HATA: Bitiş tarihi başlangıç tarihinden önce olamaz!";
                _lblTotalPrice.Text = "Geçersiz Tarih Aralığı";
                _lblTotalPrice.ForeColor = Color.FromArgb(196, 36, 48);
                return;
            }

            int days = Math.Max(1, (int)(_dtpEnd.Value.Date - _dtpStart.Value.Date).TotalDays);
            if (days == 1) _selectedPackage = "1 Gün";
            else if (days == 7) _selectedPackage = "1 Hafta";
            else if (days == 30) _selectedPackage = "1 Ay";
            else _selectedPackage = "Özel Süre";

            decimal total = _selectedVehicle.CalculateRentalPrice(days);
            _lblPriceBreakdown.Text = _selectedVehicle.GetPriceBreakdown(days);
            _lblTotalPrice.Text = string.Format("Ödenecek Toplam Tutar: {0:N0} TL", total);
            _lblTotalPrice.ForeColor = Color.FromArgb(18, 66, 134);
        }

        // --- ADIM 4: ÖDEME ONAYI ---
        private void BuildStep4Panel()
        {
            _pnlStep4 = new Panel
            {
                Location = new Point(22, 128),
                Size = new Size(920, 444),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            Controls.Add(_pnlStep4);

            Label lblTitle = new Label
            {
                Text = "Ödeme Onayı ve Kiralama Özeti",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(26, 18)
            };
            _pnlStep4.Controls.Add(lblTitle);

            Panel summaryCard = new Panel
            {
                Location = new Point(26, 62),
                Size = new Size(440, 356),
                BackColor = Color.FromArgb(246, 249, 254),
                BorderStyle = BorderStyle.FixedSingle
            };
            _pnlStep4.Controls.Add(summaryCard);

            _lblPaymentSummary = new Label
            {
                Location = new Point(16, 16),
                Size = new Size(408, 324),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(24, 38, 60)
            };
            summaryCard.Controls.Add(_lblPaymentSummary);

            Panel payForm = new Panel
            {
                Location = new Point(486, 62),
                Size = new Size(404, 356),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _pnlStep4.Controls.Add(payForm);

            Label lblPayHead = new Label
            {
                Text = "Ödeme Bilgileri",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                Location = new Point(18, 16),
                AutoSize = true
            };
            payForm.Controls.Add(lblPayHead);

            Label lblMethod = new Label { Text = "Ödeme Yöntemi", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(18, 58), AutoSize = true };
            payForm.Controls.Add(lblMethod);
            _cmbPaymentMethod = new ComboBox
            {
                Location = new Point(18, 84),
                Size = new Size(364, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbPaymentMethod.Items.AddRange(new object[] { "Kredi Kartı", "Banka Kartı", "Havale / EFT", "Nakit (Şubede)" });
            _cmbPaymentMethod.SelectedIndex = 0;
            _cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            payForm.Controls.Add(_cmbPaymentMethod);

            Label lblHolder = new Label { Text = "Kart / Ödeme Sahibi Ad Soyad (Yalnızca Harf)", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(18, 134), AutoSize = true };
            payForm.Controls.Add(lblHolder);
            _txtCardHolder = new TextBox { Location = new Point(18, 160), Size = new Size(364, 30) };
            InputValidator.AttachLettersOnlyFilter(_txtCardHolder, 40);
            payForm.Controls.Add(_txtCardHolder);

            Label lblCardNo = new Label { Text = "Kart / Referans Numarası (16 Hane Rakam)", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(18, 210), AutoSize = true };
            payForm.Controls.Add(lblCardNo);
            _txtCardNumber = new TextBox { Location = new Point(18, 236), Size = new Size(364, 30) };
            InputValidator.AttachCardNumberFilter(_txtCardNumber);
            _txtCardNumber.Text = "4543 6012 8944 5009";
            payForm.Controls.Add(_txtCardNumber);

            Label lblNote = new Label
            {
                Text = "Ödemeyi onayladığınızda kiralama sözleşmesi aktifleşir\r\nve araç durumu otomatik olarak 'Kirada' olarak güncellenir.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(88, 102, 122),
                Location = new Point(18, 286),
                Size = new Size(364, 48)
            };
            payForm.Controls.Add(lblNote);
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_txtCardNumber == null) return;
            string method = _cmbPaymentMethod.SelectedItem != null ? _cmbPaymentMethod.SelectedItem.ToString() : "Kredi Kartı";
            if (method.IndexOf("Nakit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _txtCardNumber.ReadOnly = true;
                _txtCardNumber.Text = "NAKİT - ŞUBEDE ÖDEME";
            }
            else
            {
                _txtCardNumber.ReadOnly = false;
                if (!_txtCardNumber.Text.Any(char.IsDigit))
                {
                    _txtCardNumber.Text = method.IndexOf("Havale", StringComparison.OrdinalIgnoreCase) >= 0
                        ? "3820 2610 0150"
                        : "4543 6012 8944 5009";
                }
            }
        }

        private void PopulatePaymentSummary()
        {
            Customer cust = _cmbCustomer.SelectedItem as Customer;
            RentalConsultant cons = _cmbConsultant.SelectedItem as RentalConsultant;
            int days = Math.Max(1, (int)(_dtpEnd.Value.Date - _dtpStart.Value.Date).TotalDays);
            decimal total = _selectedVehicle.CalculateRentalPrice(days);

            _txtCardHolder.Text = cust != null ? cust.FullName : string.Empty;

            _lblPaymentSummary.Text = string.Format(
                "FİRMA: Duman Rent a Car\r\n" +
                "----------------------------------------\r\n" +
                "Müşteri: {0}\r\n" +
                "Kiralama Danışmanı: {1}\r\n" +
                "Kategori: {2}\r\n" +
                "Marka / Model: {3}\r\n" +
                "Renk: {4}\r\n" +
                "Plaka: {5}\r\n" +
                "Kiralama Paketi: {6} ({7} Gün)\r\n" +
                "Başlangıç: {8:dd.MM.yyyy}\r\n" +
                "Bitiş: {9:dd.MM.yyyy}\r\n" +
                "----------------------------------------\r\n" +
                "TOPLAM TUTAR: {10:N0} TL",
                cust != null ? cust.FullName : "-",
                cons != null ? cons.FullName : "-",
                _selectedVehicle.CategoryName,
                _selectedVehicle.Model,
                _selectedVehicle.ColorText,
                _selectedVehicle.PlateNumber,
                _selectedPackage,
                days,
                _dtpStart.Value,
                _dtpEnd.Value,
                total);
        }

        private void ShowStep(int step)
        {
            _currentStep = step;
            _pnlStep1.Visible = (step == 1);
            _pnlStep2.Visible = (step == 2);
            _pnlStep3.Visible = (step == 3);
            _pnlStep4.Visible = (step == 4);

            _btnBack.Text = step == 1 ? "İptal / Ana Sayfa" : "<- Önceki Adım";
            _btnNext.Text = step == 4 ? "Ödemeyi Onayla ve Tamamla" : "Sonraki Adım ->";
            _btnNext.BackColor = step == 4 ? Color.FromArgb(20, 136, 78) : Color.FromArgb(18, 66, 134);

            if (step == 2) PopulateBrands();
            if (step == 3) LoadStep3Data();
            if (step == 4) PopulatePaymentSummary();

            _stepIndicatorPanel.Invalidate();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentStep == 1)
                {
                    ShowStep(2);
                }
                else if (_currentStep == 2)
                {
                    if (_selectedVehicle == null)
                    {
                        throw new InvalidOperationException("Seçilen marka, model ve renkte müsait araç bulunmamaktadır. Lütfen farklı bir renk veya model seçiniz.");
                    }
                    ShowStep(3);
                }
                else if (_currentStep == 3)
                {
                    if (_dtpEnd.Value.Date < _dtpStart.Value.Date)
                    {
                        throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz.");
                    }
                    if (_cmbCustomer.SelectedItem == null || _cmbConsultant.SelectedItem == null)
                    {
                        throw new ArgumentException("Müşteri ve danışman seçimi zorunludur.");
                    }
                    ShowStep(4);
                }
                else if (_currentStep == 4)
                {
                    Customer cust = _cmbCustomer.SelectedItem as Customer;
                    RentalConsultant cons = _cmbConsultant.SelectedItem as RentalConsultant;
                    string paymentMethod = _cmbPaymentMethod.SelectedItem != null ? _cmbPaymentMethod.SelectedItem.ToString() : "Kredi Kartı";

                    if (!InputValidator.IsValidFullName(_txtCardHolder.Text))
                    {
                        _txtCardHolder.Focus();
                        throw new ArgumentException("Lütfen 'Kart / Ödeme Sahibi Ad Soyad' alanına geçerli bir Ad ve Soyad giriniz.");
                    }

                    string paymentError;
                    if (!InputValidator.IsValidPaymentReference(paymentMethod, _txtCardNumber.Text, out paymentError))
                    {
                        _txtCardNumber.Focus();
                        throw new ArgumentException(paymentError);
                    }

                    Rental rental = _manager.CreateRental(
                        cust,
                        _selectedVehicle,
                        cons,
                        _dtpStart.Value.Date,
                        _dtpEnd.Value.Date,
                        _selectedPackage,
                        paymentMethod);

                    MessageBox.Show(
                        string.Format(
                            "ÖDEME ONAYLANDI VE KİRALAMA TAMAMLANDI!\r\n\r\n" +
                            "Kiralama No: #{0}\r\n" +
                            "Müşteri: {1}\r\n" +
                            "Danışman: {2}\r\n" +
                            "Araç: {3} ({4}) - {5}\r\n" +
                            "Süre: {6} ({7} Gün)\r\n" +
                            "Ödeme Yöntemi: {8}\r\n" +
                            "Ödenen Tutar: {9:N0} TL\r\n\r\n" +
                            "Araç durumu 'Kirada' olarak güncellenmiştir.",
                            rental.Id,
                            rental.Customer.FullName,
                            rental.Consultant.FullName,
                            rental.Vehicle.Model,
                            rental.Vehicle.ColorText,
                            rental.Vehicle.PlateNumber,
                            rental.DurationPackage,
                            rental.TotalDays,
                            rental.PaymentMethod,
                            rental.TotalPrice),
                        "Duman Rent a Car - Ödeme Onayı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Uyarı / İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
