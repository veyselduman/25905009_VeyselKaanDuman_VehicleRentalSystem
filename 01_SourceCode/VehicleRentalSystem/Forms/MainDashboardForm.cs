using System;
using System.Drawing;
using System.Windows.Forms;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Ana Sayfa (Dashboard) ekranı.
    /// Dinamik filo/müşteri/kiralama sayaçları, kategori görselleri ve tüm modüllere erişim sağlar.
    /// </summary>
    public class MainDashboardForm : Form
    {
        private readonly RentalManager _manager;

        private Label _lblTotalVehicles;
        private Label _lblAvailableVehicles;
        private Label _lblRentedVehicles;
        private Label _lblTotalCustomers;
        private Label _lblActiveRentals;
        private Label _lblLastActivity;
        private Panel _chartPanel;

        public MainDashboardForm(RentalManager manager)
        {
            _manager = manager;
            _manager.DataChanged += Manager_DataChanged;
            InitializeUI();
            RefreshDashboardStats();
        }

        private void Manager_DataChanged(object sender, RentalEventArgs e)
        {
            RefreshDashboardStats();
            if (_lblLastActivity != null)
            {
                _lblLastActivity.Text = string.Format("Son İşlem ({0:HH:mm:ss}): {1}", e.Timestamp, e.Message);
            }
        }

        private void InitializeUI()
        {
            Text = "Duman Rent a Car - Ana Sayfa (Dashboard)";
            Size = new Size(1160, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 9.5f);

            // Üst Header
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(18, 34, 62)
            };
            Controls.Add(header);

            Label lblTitle = new Label
            {
                Text = "DUMAN RENT A CAR  |  Yönetim Paneli",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };
            header.Controls.Add(lblTitle);

            string custName = _manager.ActiveCustomer != null ? _manager.ActiveCustomer.FullName : "-";
            string consName = _manager.ActiveConsultant != null ? _manager.ActiveConsultant.FullName : "-";
            Label lblUserMeta = new Label
            {
                Text = string.Format("Veysel Kaan Düman (25905009)  •  Müşteri: {0}  •  Danışman: {1}", custName, consName),
                ForeColor = Color.FromArgb(224, 232, 246),
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(515, 24)
            };
            header.Controls.Add(lblUserMeta);

            // Sol Navigasyon Menüsü
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 225,
                BackColor = Color.FromArgb(26, 44, 74)
            };
            Controls.Add(sidebar);

            Label lblMenu = new Label
            {
                Text = "MODÜLLER",
                ForeColor = Color.FromArgb(215, 175, 62),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 22)
            };
            sidebar.Controls.Add(lblMenu);

            AddMenuButton(sidebar, "Ana Sayfa (Dashboard)", 54, true, (s, e) => RefreshDashboardStats());
            AddMenuButton(sidebar, "Araç Kiralama Sihirbazı", 108, false, (s, e) => OpenRentalWizard());
            AddMenuButton(sidebar, "Araç Yönetimi", 162, false, (s, e) => OpenVehicleManagement());
            AddMenuButton(sidebar, "Müşteri Yönetimi", 216, false, (s, e) => OpenCustomerManagement());
            AddMenuButton(sidebar, "Araç İade İşlemleri", 270, false, (s, e) => OpenReturnVehicle());

            Button btnQuickRent = new Button
            {
                Text = "Hızlı Kiralama Başlat",
                Location = new Point(16, 355),
                Size = new Size(192, 46),
                BackColor = Color.FromArgb(215, 165, 42),
                ForeColor = Color.FromArgb(18, 28, 44),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnQuickRent.FlatAppearance.BorderSize = 0;
            btnQuickRent.Click += (s, e) => OpenRentalWizard();
            sidebar.Controls.Add(btnQuickRent);

            // Ana İçerik Paneli
            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(22)
            };
            Controls.Add(content);
            content.BringToFront();

            // 5 Dinamik KPI Kartı
            _lblTotalVehicles = CreateKpiCard(content, "Toplam Araç", "0", Color.FromArgb(24, 66, 134), 20, 18);
            _lblAvailableVehicles = CreateKpiCard(content, "Müsait Araç", "0", Color.FromArgb(22, 138, 86), 198, 18);
            _lblRentedVehicles = CreateKpiCard(content, "Kiradaki Araç", "0", Color.FromArgb(204, 114, 24), 376, 18);
            _lblTotalCustomers = CreateKpiCard(content, "Toplam Müşteri", "0", Color.FromArgb(88, 66, 152), 554, 18);
            _lblActiveRentals = CreateKpiCard(content, "Aktif Kiralama", "0", Color.FromArgb(182, 42, 60), 732, 18);

            // Filo Görsel Kartları (Sedan ve SUV)
            Panel fleetShowcase = new Panel
            {
                Location = new Point(20, 136),
                Size = new Size(876, 232),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            content.Controls.Add(fleetShowcase);

            Label lblFleetTitle = new Label
            {
                Text = "Araç Kategorilerimiz (Seçim için tıklayarak kiralama başlatabilirsiniz)",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            fleetShowcase.Controls.Add(lblFleetTitle);

            PictureBox pbSedan = new PictureBox
            {
                Location = new Point(16, 44),
                Size = new Size(416, 174),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Image = CarGraphicsRenderer.RenderCarPreview("Sedan", "Volkswagen", "Volkswagen Passat", VehicleColor.Beyaz, 416, 174)
            };
            pbSedan.Click += (s, e) => OpenRentalWizard("Sedan");
            fleetShowcase.Controls.Add(pbSedan);

            PictureBox pbSuv = new PictureBox
            {
                Location = new Point(444, 44),
                Size = new Size(416, 174),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Image = CarGraphicsRenderer.RenderCarPreview("SUV", "Peugeot", "Peugeot 3008", VehicleColor.Siyah, 416, 174)
            };
            pbSuv.Click += (s, e) => OpenRentalWizard("SUV");
            fleetShowcase.Controls.Add(pbSuv);

            // Alt Bölüm: Marka Amblemleri + Canlı Filo Doluluk Grafiği
            Panel brandPanel = new Panel
            {
                Location = new Point(20, 384),
                Size = new Size(512, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            content.Controls.Add(brandPanel);

            Label lblBrands = new Label
            {
                Text = "Filo Markaları",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 34, 62),
                AutoSize = true,
                Location = new Point(14, 10)
            };
            brandPanel.Controls.Add(lblBrands);

            string[] brands = { "Fiat", "Volkswagen", "Ford", "Peugeot" };
            for (int i = 0; i < brands.Length; i++)
            {
                int col = i % 2;
                int row = i / 2;
                PictureBox pb = new PictureBox
                {
                    Location = new Point(14 + col * 246, 40 + row * 68),
                    Size = new Size(236, 60),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = CarGraphicsRenderer.RenderBrandBadge(brands[i], 236, 60)
                };
                brandPanel.Controls.Add(pb);
            }

            // Canlı Filo Grafiği
            _chartPanel = new Panel
            {
                Location = new Point(546, 384),
                Size = new Size(350, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _chartPanel.Paint += ChartPanel_Paint;
            content.Controls.Add(_chartPanel);

            // Alt Durum Çubuğu
            _lblLastActivity = new Label
            {
                Text = "Sistem hazır. Tüm veriler bellek koleksiyonlarından anlık olarak hesaplanmaktadır.",
                Location = new Point(20, 578),
                Size = new Size(876, 26),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(72, 86, 108)
            };
            content.Controls.Add(_lblLastActivity);
        }

        private void AddMenuButton(Panel sidebar, string text, int top, bool active, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = "  " + text,
                Location = new Point(12, top),
                Size = new Size(200, 44),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = active ? Color.FromArgb(40, 74, 128) : Color.FromArgb(32, 54, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            sidebar.Controls.Add(btn);
        }

        private Label CreateKpiCard(Panel parent, string title, string initialVal, Color accent, int x, int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(164, 102),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(card);

            Panel stripe = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = accent
            };
            card.Controls.Add(stripe);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(92, 104, 122),
                Location = new Point(12, 18),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            Label lblVal = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(12, 42),
                AutoSize = true
            };
            card.Controls.Add(lblVal);

            return lblVal;
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (Font tf = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (SolidBrush tb = new SolidBrush(Color.FromArgb(18, 34, 62)))
            {
                g.DrawString("Canlı Filo Durum Grafiği", tf, tb, 14, 10);
            }

            int total = Math.Max(1, _manager.TotalVehicleCount);
            int avail = _manager.AvailableVehicleCount;
            int rented = _manager.RentedVehicleCount;

            int barMaxW = 220;
            int availW = (int)(barMaxW * (avail / (double)total));
            int rentedW = (int)(barMaxW * (rented / (double)total));

            using (Font lf = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                g.DrawString(string.Format("Müsait ({0})", avail), lf, Brushes.DarkSlateGray, 16, 52);
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 236, 244)), 105, 50, barMaxW, 24);
                g.FillRectangle(new SolidBrush(Color.FromArgb(22, 138, 86)), 105, 50, Math.Max(6, availW), 24);

                g.DrawString(string.Format("Kirada ({0})", rented), lf, Brushes.DarkSlateGray, 16, 98);
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 236, 244)), 105, 96, barMaxW, 24);
                g.FillRectangle(new SolidBrush(Color.FromArgb(204, 114, 24)), 105, 96, rented > 0 ? Math.Max(10, rentedW) : 0, 24);

                int pct = (int)Math.Round(avail * 100.0 / total);
                g.DrawString(string.Format("Filo Müsaitlik Oranı: %{0}", pct), lf, new SolidBrush(Color.FromArgb(24, 66, 134)), 16, 142);
            }
        }

        public void RefreshDashboardStats()
        {
            _lblTotalVehicles.Text = _manager.TotalVehicleCount.ToString();
            _lblAvailableVehicles.Text = _manager.AvailableVehicleCount.ToString();
            _lblRentedVehicles.Text = _manager.RentedVehicleCount.ToString();
            _lblTotalCustomers.Text = _manager.TotalCustomerCount.ToString();
            _lblActiveRentals.Text = _manager.ActiveRentalCount.ToString();
            if (_chartPanel != null)
            {
                _chartPanel.Invalidate();
            }
        }

        private void OpenRentalWizard(string preselectedCategory = null)
        {
            using (RentalWizardForm frm = new RentalWizardForm(_manager, preselectedCategory))
            {
                frm.ShowDialog(this);
            }
            RefreshDashboardStats();
        }

        private void OpenVehicleManagement()
        {
            using (VehicleManagementForm frm = new VehicleManagementForm(_manager))
            {
                frm.ShowDialog(this);
            }
            RefreshDashboardStats();
        }

        private void OpenCustomerManagement()
        {
            using (CustomerManagementForm frm = new CustomerManagementForm(_manager))
            {
                frm.ShowDialog(this);
            }
            RefreshDashboardStats();
        }

        private void OpenReturnVehicle()
        {
            using (ReturnVehicleForm frm = new ReturnVehicleForm(_manager))
            {
                frm.ShowDialog(this);
            }
            RefreshDashboardStats();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _manager.DataChanged -= Manager_DataChanged;
            base.OnFormClosed(e);
        }
    }
}
