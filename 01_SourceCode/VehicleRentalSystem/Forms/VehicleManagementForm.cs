using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Araç Yönetimi modülü: Araç ekleme, listeleme, güncelleme ve silme işlemleri.
    /// Standart Türkiye plakası (38 DUM 101), yalnızca harf içeren Marka ve kurallı Model doğrulaması uygular.
    /// </summary>
    public class VehicleManagementForm : Form
    {
        private readonly RentalManager _manager;

        private DataGridView _dgvVehicles;
        private ComboBox _cmbCategory;
        private TextBox _txtPlate;
        private TextBox _txtBrand;
        private TextBox _txtModel;
        private ComboBox _cmbColor;
        private NumericUpDown _numYear;
        private NumericUpDown _numDailyRate;

        private int _selectedVehicleId = -1;

        public VehicleManagementForm(RentalManager manager)
        {
            _manager = manager;
            InitializeUI();
            LoadGrid();
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Araç Yönetimi";
            Size = new Size(1060, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.FromArgb(18, 34, 62) };
            Controls.Add(topBar);

            Label lblTitle = new Label
            {
                Text = "ARAÇ YÖNETİMİ  |  Duman Rent a Car",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 16)
            };
            topBar.Controls.Add(lblTitle);

            Button btnHome = new Button
            {
                Text = "<- Ana Sayfaya Dön",
                Location = new Point(860, 12),
                Size = new Size(165, 34),
                BackColor = Color.FromArgb(44, 78, 134),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.Click += (s, e) => Close();
            topBar.Controls.Add(btnHome);

            Panel formPanel = new Panel
            {
                Location = new Point(18, 72),
                Size = new Size(310, 520),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(formPanel);

            int y = 18;
            AddLabel(formPanel, "Kategori", 16, y);
            _cmbCategory = new ComboBox { Location = new Point(16, y + 22), Size = new Size(274, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbCategory.Items.AddRange(new object[] { "Sedan", "SUV" });
            _cmbCategory.SelectedIndex = 0;
            formPanel.Controls.Add(_cmbCategory);

            y += 60;
            AddLabel(formPanel, "Plaka (Örn: 38 DUM 137)", 16, y);
            Button btnAutoPlate = new Button
            {
                Text = "Oto. Plaka",
                Location = new Point(214, y - 3),
                Size = new Size(76, 23),
                BackColor = Color.FromArgb(232, 238, 248),
                ForeColor = Color.FromArgb(24, 52, 96),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAutoPlate.FlatAppearance.BorderSize = 1;
            btnAutoPlate.FlatAppearance.BorderColor = Color.FromArgb(190, 204, 224);
            btnAutoPlate.Click += (s, e) => { _txtPlate.Text = _manager.GenerateUniquePlate(); };
            formPanel.Controls.Add(btnAutoPlate);

            _txtPlate = new TextBox { Location = new Point(16, y + 22), Size = new Size(274, 28) };
            InputValidator.AttachPlateFilter(_txtPlate);
            formPanel.Controls.Add(_txtPlate);

            y += 58;
            AddLabel(formPanel, "Marka (Yalnızca Harf)", 16, y);
            _txtBrand = new TextBox { Location = new Point(16, y + 22), Size = new Size(274, 28) };
            InputValidator.AttachLettersOnlyFilter(_txtBrand, 20);
            formPanel.Controls.Add(_txtBrand);

            y += 58;
            AddLabel(formPanel, "Model (Örn: Fiat Egea / Peugeot 2008)", 16, y);
            _txtModel = new TextBox { Location = new Point(16, y + 22), Size = new Size(274, 28) };
            InputValidator.AttachModelFilter(_txtModel);
            formPanel.Controls.Add(_txtModel);

            y += 58;
            AddLabel(formPanel, "Renk", 16, y);
            _cmbColor = new ComboBox { Location = new Point(16, y + 22), Size = new Size(274, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbColor.Items.AddRange(new object[] { VehicleColor.Siyah, VehicleColor.Beyaz, VehicleColor.Gri });
            _cmbColor.SelectedIndex = 0;
            formPanel.Controls.Add(_cmbColor);

            y += 58;
            AddLabel(formPanel, "Üretim Yılı", 16, y);
            _numYear = new NumericUpDown { Location = new Point(16, y + 22), Size = new Size(126, 28), Minimum = 1995, Maximum = 2027, Value = 2025 };
            formPanel.Controls.Add(_numYear);

            AddLabel(formPanel, "Günlük Ücret (TL)", 158, y);
            _numDailyRate = new NumericUpDown { Location = new Point(158, y + 22), Size = new Size(132, 28), Minimum = 500, Maximum = 50000, Increment = 50, Value = 1800 };
            formPanel.Controls.Add(_numDailyRate);

            y += 72;
            Button btnAdd = CreateActionBtn("Araç Ekle", Color.FromArgb(20, 136, 78), 16, y, 130);
            btnAdd.Click += BtnAdd_Click;
            formPanel.Controls.Add(btnAdd);

            Button btnUpdate = CreateActionBtn("Güncelle", Color.FromArgb(18, 66, 134), 160, y, 130);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            y += 48;
            Button btnDelete = CreateActionBtn("Seçili Aracı Sil", Color.FromArgb(186, 40, 52), 16, y, 130);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            Button btnClear = CreateActionBtn("Temizle", Color.FromArgb(100, 112, 128), 160, y, 130);
            btnClear.Click += (s, e) => ClearInputs();
            formPanel.Controls.Add(btnClear);

            _dgvVehicles = new DataGridView
            {
                Location = new Point(344, 72),
                Size = new Size(684, 520),
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvVehicles.SelectionChanged += DgvVehicles_SelectionChanged;
            Controls.Add(_dgvVehicles);
        }

        private void AddLabel(Panel p, string text, int x, int y)
        {
            p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) });
        }

        private Button CreateActionBtn(string text, Color bg, int x, int y, int w)
        {
            Button b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 36),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void LoadGrid()
        {
            var rows = _manager.GetAllVehicles().Select(v => new
            {
                v.Id,
                Kategori = v.CategoryName,
                Plaka = v.PlateNumber,
                Marka = v.Brand,
                Model = v.Model,
                Renk = v.ColorText,
                Yil = v.Year,
                GunlukUcret = string.Format("{0:N0} TL", v.DailyRate),
                Durum = v.StatusText
            }).ToList();

            _dgvVehicles.DataSource = rows;

            if (_dgvVehicles.Columns["Id"] != null) { _dgvVehicles.Columns["Id"].HeaderText = "Araç ID"; _dgvVehicles.Columns["Id"].FillWeight = 45; }
            if (_dgvVehicles.Columns["Kategori"] != null) { _dgvVehicles.Columns["Kategori"].HeaderText = "Kategori"; _dgvVehicles.Columns["Kategori"].FillWeight = 65; }
            if (_dgvVehicles.Columns["Plaka"] != null) { _dgvVehicles.Columns["Plaka"].HeaderText = "Plaka"; _dgvVehicles.Columns["Plaka"].FillWeight = 110; }
            if (_dgvVehicles.Columns["Marka"] != null) { _dgvVehicles.Columns["Marka"].HeaderText = "Marka"; _dgvVehicles.Columns["Marka"].FillWeight = 95; }
            if (_dgvVehicles.Columns["Model"] != null) { _dgvVehicles.Columns["Model"].HeaderText = "Model"; _dgvVehicles.Columns["Model"].FillWeight = 135; }
            if (_dgvVehicles.Columns["Renk"] != null) { _dgvVehicles.Columns["Renk"].HeaderText = "Renk"; _dgvVehicles.Columns["Renk"].FillWeight = 65; }
            if (_dgvVehicles.Columns["Yil"] != null) { _dgvVehicles.Columns["Yil"].HeaderText = "Üretim Yılı"; _dgvVehicles.Columns["Yil"].FillWeight = 65; }
            if (_dgvVehicles.Columns["GunlukUcret"] != null) { _dgvVehicles.Columns["GunlukUcret"].HeaderText = "Günlük Ücret"; _dgvVehicles.Columns["GunlukUcret"].FillWeight = 85; }
            if (_dgvVehicles.Columns["Durum"] != null) { _dgvVehicles.Columns["Durum"].HeaderText = "Araç Durumu"; _dgvVehicles.Columns["Durum"].FillWeight = 70; }
        }

        private void DgvVehicles_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvVehicles.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(_dgvVehicles.SelectedRows[0].Cells["Id"].Value);
            Vehicle v = _manager.GetAllVehicles().FirstOrDefault(x => x.Id == id);
            if (v == null) return;

            _selectedVehicleId = v.Id;
            _cmbCategory.SelectedItem = v.CategoryName;
            _txtPlate.Text = v.PlateNumber;
            _txtBrand.Text = v.Brand;
            _txtModel.Text = v.Model;
            _cmbColor.SelectedItem = v.Color;
            _numYear.Value = Math.Max(_numYear.Minimum, Math.Min(_numYear.Maximum, v.Year));
            _numDailyRate.Value = Math.Max(_numDailyRate.Minimum, Math.Min(_numDailyRate.Maximum, v.DailyRate));
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _manager.AddVehicle(
                    _cmbCategory.SelectedItem.ToString(),
                    _txtPlate.Text,
                    _txtBrand.Text,
                    _txtModel.Text,
                    (VehicleColor)_cmbColor.SelectedItem,
                    (int)_numYear.Value,
                    _numDailyRate.Value);

                LoadGrid();
                ClearInputs();
                MessageBox.Show("Yeni araç başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Araç Ekleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedVehicleId < 0)
                {
                    MessageBox.Show("Lütfen listeden güncellenecek bir araç seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _manager.UpdateVehicle(
                    _selectedVehicleId,
                    _cmbCategory.SelectedItem != null ? _cmbCategory.SelectedItem.ToString() : "Sedan",
                    _txtPlate.Text,
                    _txtBrand.Text,
                    _txtModel.Text,
                    (VehicleColor)_cmbColor.SelectedItem,
                    (int)_numYear.Value,
                    _numDailyRate.Value);

                LoadGrid();
                MessageBox.Show("Araç bilgileri güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Güncelleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedVehicleId < 0)
                {
                    MessageBox.Show("Lütfen silinecek aracı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _manager.DeleteVehicle(_selectedVehicleId);
                LoadGrid();
                ClearInputs();
                MessageBox.Show("Araç kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            _selectedVehicleId = -1;
            if (_dgvVehicles != null)
            {
                _dgvVehicles.ClearSelection();
            }
            _txtPlate.Clear();
            _txtBrand.Clear();
            _txtModel.Clear();
            _cmbColor.SelectedIndex = 0;
        }
    }
}
