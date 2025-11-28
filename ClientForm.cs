using System;
using System.Drawing;
using System.Windows.Forms;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Экран для клиента - итоговая информация о заказе.
    /// </summary>
    public class ClientForm : Form
    {
        private readonly Order _order;

        public ClientForm(Order order)
        {
            _order = order;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Ваш заказ";
            Size = new Size(820, 900);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            KeyPreview = true;
            
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    Close();
                }
            };

            int y = 20;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Спасибо за выбор!",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblTitle);
            y += 70;

            // Инструкция
            var lblInstruction = new Label
            {
                Text = "Пожалуйста, запишите на листочке:",
                Font = new Font("Segoe UI", 18),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblInstruction);
            y += 40;

            string[] instructions = new[]
            {
                "• Вашу фамилию и имя",
                "• Контактный телефон И e-mail",
                "• Количество выбранных фотографий"
            };

            foreach (var instruction in instructions)
            {
                var lbl = new Label
                {
                    Text = instruction,
                    Font = new Font("Segoe UI", 14),
                    Location = new Point(40, y),
                    AutoSize = true
                };
                Controls.Add(lbl);
                y += 32;
            }

            y += 20;

            // Разделитель
            var separator1 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(20, y),
                Size = new Size(760, 2)
            };
            Controls.Add(separator1);
            y += 20;

            // Данные заказа
            var lblOrderTitle = new Label
            {
                Text = "Данные заказа:",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblOrderTitle);
            y += 48;

            // Количество фотографий
            var lblPhotos = new Label
            {
                Text = $"Количество фотографий: {_order.TotalPhotos}",
                Font = new Font("Segoe UI", 16),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblPhotos);
            y += 38;

            // Сумма
            var lblAmount = new Label
            {
                Text = $"Сумма к оплате: {_order.TotalAmount:N0} ₽",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 0),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblAmount);
            y += 55;

            // Разделитель
            var separator2 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(20, y),
                Size = new Size(760, 2)
            };
            Controls.Add(separator2);
            y += 20;

            // Детализация
            var lblDetails = new Label
            {
                Text = "Детализация:",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, y),
                AutoSize = true
            };
            Controls.Add(lblDetails);
            y += 36;

            // Таблица позиций
            var grid = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(760, Math.Min(220, _order.Positions.Count * 38 + 40)),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 250);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 34;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Position",
                HeaderText = "Позиция",
                Width = 160
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Count",
                HeaderText = "Количество фото",
                Width = 260
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Стоимость",
                Width = 260
            });

            foreach (var pos in _order.Positions)
            {
                var positionTotal = PriceCalculator.CalculatePositionTotal(pos.Count);
                grid.Rows.Add($"№ {pos.Position}", pos.Count.ToString(), $"{positionTotal:N0} ₽");
            }

            Controls.Add(grid);
            y += grid.Height + 30;

            // Кнопка закрытия
            var btnClose = new Button
            {
                Text = "Готово",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(240, 60),
                Location = new Point((ClientSize.Width - 240) / 2, y),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();
            Controls.Add(btnClose);

            // Подгоняем высоту формы
            ClientSize = new Size(ClientSize.Width, y + 80);
        }
    }
}
