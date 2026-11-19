using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Müşteri Yönetimi modülü: Müşteri ekleme, listeleme, güncelleme ve silme işlemleri.
    /// Ad-Soyad (yalnızca harf), Telefon (05XX XXX XX XX) ve Ehliyet No (B-381045) doğrulaması içerir.
    /// </summary>
    public class CustomerManagementForm : Form
    {
        private readonly RentalManager _manager;
        private DataGridView _dgvCustomers;
        private TextBox _txtFirstName;
        private TextBox _txtLastName;
        private TextBox _txtPhone;
        private TextBox _txtLicense;
        private int _selectedCustomerId = -1;

        public CustomerManagementForm(RentalManager manager)
        {
            _manager = manager;
            InitializeUI();
            LoadGrid();
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Müşteri Yönetimi";
            Size = new Size(980, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.FromArgb(18, 34, 62) };
            Controls.Add(topBar);
            topBar.Controls.Add(new Label
            {
                Text = "MÜŞTERİ YÖNETİMİ  |  Duman Rent a Car",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 16)
            });

            Button btnHome = new Button
            {
                Text = "<- Ana Sayfaya Dön",
                Location = new Point(780, 12),
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
                Size = new Size(300, 470),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(formPanel);

            int y = 20;
            AddField(formPanel, "Ad (Yalnızca Harf)", out _txtFirstName, y);
            InputValidator.AttachLettersOnlyFilter(_txtFirstName, 25);

            y += 68;
            AddField(formPanel, "Soyad (Yalnızca Harf)", out _txtLastName, y);
            InputValidator.AttachLettersOnlyFilter(_txtLastName, 25);

            y += 68;
            AddField(formPanel, "Telefon (05XX XXX XX XX)", out _txtPhone, y);
            InputValidator.AttachPhoneFilter(_txtPhone);

            y += 68;
            AddField(formPanel, "Ehliyet No (Örn: B-381045)", out _txtLicense, y);
            InputValidator.AttachLicenseFilter(_txtLicense);

            y += 80;
            Button btnAdd = MakeBtn("Müşteri Ekle", Color.FromArgb(20, 136, 78), 16, y, 126);
            btnAdd.Click += BtnAdd_Click;
            formPanel.Controls.Add(btnAdd);

            Button btnUpdate = MakeBtn("Güncelle", Color.FromArgb(18, 66, 134), 156, y, 126);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            y += 50;
            Button btnDelete = MakeBtn("Müşteri Sil", Color.FromArgb(186, 40, 52), 16, y, 126);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            Button btnClear = MakeBtn("Temizle", Color.FromArgb(100, 112, 128), 156, y, 126);
            btnClear.Click += (s, e) => ClearInputs();
            formPanel.Controls.Add(btnClear);

            _dgvCustomers = new DataGridView
            {
                Location = new Point(334, 72),
                Size = new Size(614, 470),
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;
            Controls.Add(_dgvCustomers);
        }

        private void AddField(Panel p, string label, out TextBox tb, int y)
        {
            p.Controls.Add(new Label { Text = label, Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9.2f, FontStyle.Bold) });
            tb = new TextBox { Location = new Point(16, y + 24), Size = new Size(266, 28) };
            p.Controls.Add(tb);
        }

        private Button MakeBtn(string text, Color bg, int x, int y, int w)
        {
            Button b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 38),
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
            _dgvCustomers.DataSource = _manager.GetAllCustomers().Select(c => new
            {
                c.Id,
                Ad = c.FirstName,
                Soyad = c.LastName,
                Telefon = c.Phone,
                EhliyetNo = c.DriverLicenseNumber
            }).ToList();

            if (_dgvCustomers.Columns["Id"] != null) { _dgvCustomers.Columns["Id"].HeaderText = "Müşteri ID"; _dgvCustomers.Columns["Id"].FillWeight = 55; }
            if (_dgvCustomers.Columns["Ad"] != null) { _dgvCustomers.Columns["Ad"].HeaderText = "Ad"; _dgvCustomers.Columns["Ad"].FillWeight = 110; }
            if (_dgvCustomers.Columns["Soyad"] != null) { _dgvCustomers.Columns["Soyad"].HeaderText = "Soyad"; _dgvCustomers.Columns["Soyad"].FillWeight = 110; }
            if (_dgvCustomers.Columns["Telefon"] != null) { _dgvCustomers.Columns["Telefon"].HeaderText = "Telefon"; _dgvCustomers.Columns["Telefon"].FillWeight = 130; }
            if (_dgvCustomers.Columns["EhliyetNo"] != null) { _dgvCustomers.Columns["EhliyetNo"].HeaderText = "Ehliyet Numarası"; _dgvCustomers.Columns["EhliyetNo"].FillWeight = 110; }
        }

        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvCustomers.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(_dgvCustomers.SelectedRows[0].Cells["Id"].Value);
            Customer c = _manager.GetAllCustomers().FirstOrDefault(x => x.Id == id);
            if (c == null) return;

            _selectedCustomerId = c.Id;
            _txtFirstName.Text = c.FirstName;
            _txtLastName.Text = c.LastName;
            _txtPhone.Text = c.Phone;
            _txtLicense.Text = c.DriverLicenseNumber;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _manager.AddCustomer(_txtFirstName.Text, _txtLastName.Text, _txtPhone.Text, _txtLicense.Text);
                LoadGrid();
                ClearInputs();
                MessageBox.Show("Müşteri başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId < 0)
                {
                    MessageBox.Show("Güncellenecek müşteriyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _manager.UpdateCustomer(_selectedCustomerId, _txtFirstName.Text, _txtLastName.Text, _txtPhone.Text, _txtLicense.Text);
                LoadGrid();
                MessageBox.Show("Müşteri bilgileri güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (_selectedCustomerId < 0)
                {
                    MessageBox.Show("Silinecek müşteriyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _manager.DeleteCustomer(_selectedCustomerId);
                LoadGrid();
                ClearInputs();
                MessageBox.Show("Müşteri kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            _selectedCustomerId = -1;
            if (_dgvCustomers != null)
            {
                _dgvCustomers.ClearSelection();
            }
            _txtFirstName.Clear();
            _txtLastName.Clear();
            _txtPhone.Clear();
            _txtLicense.Clear();
        }
    }
}
