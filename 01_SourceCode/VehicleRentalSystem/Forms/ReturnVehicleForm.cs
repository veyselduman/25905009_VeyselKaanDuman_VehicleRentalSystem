using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Araç İade modülü: Aktif kiralamaları listeler, seçilen kaydı iade alarak aracı tekrar Müsait yapar.
    /// </summary>
    public class ReturnVehicleForm : Form
    {
        private readonly RentalManager _manager;
        private DataGridView _dgvActiveRentals;
        private Label _lblCount;
        private RadioButton _rbActiveOnly;
        private RadioButton _rbAllRentals;
        private Label _lblReturnSummary;

        public ReturnVehicleForm(RentalManager manager)
        {
            _manager = manager;
            InitializeUI();
            LoadActiveRentals();
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Araç İade İşlemleri";
            Size = new Size(1020, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.FromArgb(18, 34, 62) };
            Controls.Add(topBar);
            topBar.Controls.Add(new Label
            {
                Text = "ARAÇ İADE İŞLEMLERİ  |  Aktif Kiralamalar",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 16)
            });

            Button btnHome = new Button
            {
                Text = "<- Ana Sayfaya Dön",
                Location = new Point(820, 12),
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

            _lblCount = new Label
            {
                Location = new Point(20, 74),
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 52, 92)
            };
            Controls.Add(_lblCount);

            _rbActiveOnly = new RadioButton
            {
                Text = "Aktif Kiralamalar",
                Location = new Point(560, 72),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 52, 92)
            };
            _rbActiveOnly.CheckedChanged += (s, e) => { if (_rbActiveOnly.Checked) LoadActiveRentals(); };
            Controls.Add(_rbActiveOnly);

            _rbAllRentals = new RadioButton
            {
                Text = "Tüm Kiralamalar (Aktif + Tamamlanan)",
                Location = new Point(710, 72),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 52, 92)
            };
            _rbAllRentals.CheckedChanged += (s, e) => { if (_rbAllRentals.Checked) LoadActiveRentals(); };
            Controls.Add(_rbAllRentals);

            _dgvActiveRentals = new DataGridView
            {
                Location = new Point(20, 106),
                Size = new Size(964, 340),
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(_dgvActiveRentals);

            Panel summaryPanel = new Panel
            {
                Location = new Point(20, 458),
                Size = new Size(660, 84),
                BackColor = Color.FromArgb(238, 250, 244),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(summaryPanel);

            _lblReturnSummary = new Label
            {
                Text = "İade İşlemi Durumu:\r\n• Listeden aktif bir kiralama kaydı seçip 'Seçili Aracı İade Al (Tamamla)' butonuna tıklayınız.\r\n• İade tamamlandığında kiralama durumu 'Tamamlandı', araç durumu ise yeniden 'Müsait' olur.",
                Location = new Point(12, 8),
                Size = new Size(636, 68),
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(20, 92, 54)
            };
            summaryPanel.Controls.Add(_lblReturnSummary);

            Button btnReturn = new Button
            {
                Text = "Seçili Aracı İade Al (Tamamla)",
                Location = new Point(704, 476),
                Size = new Size(280, 48),
                BackColor = Color.FromArgb(20, 136, 78),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReturn.FlatAppearance.BorderSize = 0;
            btnReturn.Click += BtnReturn_Click;
            Controls.Add(btnReturn);
        }

        private void LoadActiveRentals()
        {
            var actives = _manager.GetActiveRentals();
            _lblCount.Text = string.Format("Aktif Kiralama Sayısı: {0}  (Toplam Kayıt: {1})", actives.Count, _manager.GetAllRentals().Count);

            List<Rental> listToShow = (_rbAllRentals != null && _rbAllRentals.Checked)
                ? _manager.GetAllRentals()
                : actives;

            _dgvActiveRentals.DataSource = listToShow.Select(r => new
            {
                KiralamaNo = r.Id,
                Musteri = r.Customer.FullName,
                Danisman = r.Consultant.FullName,
                Kategori = r.Vehicle.CategoryName,
                Arac = r.Vehicle.Model,
                Renk = r.Vehicle.ColorText,
                Plaka = r.Vehicle.PlateNumber,
                Baslangic = r.StartDate.ToString("dd.MM.yyyy"),
                Bitis = r.EndDate.ToString("dd.MM.yyyy"),
                ToplamGun = r.TotalDays,
                ToplamTutar = string.Format("{0:N0} TL", r.TotalPrice),
                Durum = r.StatusText
            }).ToList();

            if (_dgvActiveRentals.Columns["KiralamaNo"] != null) { _dgvActiveRentals.Columns["KiralamaNo"].HeaderText = "Kiralama ID"; _dgvActiveRentals.Columns["KiralamaNo"].FillWeight = 65; }
            if (_dgvActiveRentals.Columns["Musteri"] != null) { _dgvActiveRentals.Columns["Musteri"].HeaderText = "Müşteri"; _dgvActiveRentals.Columns["Musteri"].FillWeight = 110; }
            if (_dgvActiveRentals.Columns["Danisman"] != null) { _dgvActiveRentals.Columns["Danisman"].HeaderText = "Danışman"; _dgvActiveRentals.Columns["Danisman"].FillWeight = 95; }
            if (_dgvActiveRentals.Columns["Kategori"] != null) { _dgvActiveRentals.Columns["Kategori"].HeaderText = "Kategori"; _dgvActiveRentals.Columns["Kategori"].FillWeight = 60; }
            if (_dgvActiveRentals.Columns["Arac"] != null) { _dgvActiveRentals.Columns["Arac"].HeaderText = "Araç"; _dgvActiveRentals.Columns["Arac"].FillWeight = 115; }
            if (_dgvActiveRentals.Columns["Renk"] != null) { _dgvActiveRentals.Columns["Renk"].HeaderText = "Renk"; _dgvActiveRentals.Columns["Renk"].FillWeight = 55; }
            if (_dgvActiveRentals.Columns["Plaka"] != null) { _dgvActiveRentals.Columns["Plaka"].HeaderText = "Plaka"; _dgvActiveRentals.Columns["Plaka"].FillWeight = 85; }
            if (_dgvActiveRentals.Columns["Baslangic"] != null) { _dgvActiveRentals.Columns["Baslangic"].HeaderText = "Başlangıç"; _dgvActiveRentals.Columns["Baslangic"].FillWeight = 75; }
            if (_dgvActiveRentals.Columns["Bitis"] != null) { _dgvActiveRentals.Columns["Bitis"].HeaderText = "Bitiş"; _dgvActiveRentals.Columns["Bitis"].FillWeight = 75; }
            if (_dgvActiveRentals.Columns["ToplamGun"] != null) { _dgvActiveRentals.Columns["ToplamGun"].HeaderText = "Toplam Gün"; _dgvActiveRentals.Columns["ToplamGun"].FillWeight = 65; }
            if (_dgvActiveRentals.Columns["ToplamTutar"] != null) { _dgvActiveRentals.Columns["ToplamTutar"].HeaderText = "Toplam Ücret"; _dgvActiveRentals.Columns["ToplamTutar"].FillWeight = 80; }
            if (_dgvActiveRentals.Columns["Durum"] != null) { _dgvActiveRentals.Columns["Durum"].HeaderText = "Kiralama Durumu"; _dgvActiveRentals.Columns["Durum"].FillWeight = 85; }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dgvActiveRentals.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen iade edilecek aktif kiralama kaydını seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int rentalId = Convert.ToInt32(_dgvActiveRentals.SelectedRows[0].Cells["KiralamaNo"].Value);
                Rental target = _manager.GetAllRentals().FirstOrDefault(r => r.Id == rentalId);
                string vehicleInfo = target != null ? string.Format("{0} - {1}", target.Vehicle.Model, target.Vehicle.PlateNumber) : string.Empty;

                _manager.ReturnVehicle(rentalId);
                LoadActiveRentals();

                if (_lblReturnSummary != null)
                {
                    _lblReturnSummary.Text = string.Format(
                        "İade İşlemi Sonucu (Otomatik Durum Güncellemesi):\r\n" +
                        "• Kiralama #{0} durumu: 'Aktif' --> 'Tamamlandı'\r\n" +
                        "• Araç ({1}) durumu: 'Kirada' --> 'Müsait' (Dashboard sayaçları güncellendi)",
                        rentalId, vehicleInfo);
                }

                MessageBox.Show(
                    string.Format("Kiralama #{0} başarıyla iade alındı.\r\nKiralama durumu 'Tamamlandı', araç durumu ise 'Müsait' olarak güncellendi.", rentalId),
                    "İade Tamamlandı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "İade Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
