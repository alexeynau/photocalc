using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Форма для фотографа - ввод позиций заказа.
    /// </summary>
    public class PhotographerForm : Form
    {
        private DataGridView _grid = null!;
        private Label _lblTotalPhotos = null!;
        private Label _lblTotalAmount = null!;
        private Button _btnAddPosition = null!;
        private Button _btnOk = null!;
        private Button _btnCancel = null!;

        public PhotographerForm()
        {
            InitializeComponent();
            ResetOrder();
        }

        private void InitializeComponent()
        {
            Text = "Заказ фотографий";
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);

            var workingArea = Screen.FromPoint(Cursor.Position).WorkingArea;
            int formWidth = Math.Max(520, Math.Min(640, workingArea.Width - 40));
            int formHeight = Math.Max(600, Math.Min(700, workingArea.Height - 40));

            Size = new Size(formWidth, formHeight);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            KeyPreview = true;
            
            // Таблица позиций
            int gridWidth = ClientSize.Width - 40;
            int availableHeight = ClientSize.Height - 200; // оставляем место под панель и кнопки
            int gridHeight = Math.Max(220, Math.Min(360, availableHeight - 160));

            _grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(gridWidth, gridHeight),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                EditMode = DataGridViewEditMode.EditOnEnter
            };
            _grid.RowTemplate.Height = 34;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            _grid.ColumnHeadersHeight = 30;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Колонка номера позиции
            var colPosition = new DataGridViewTextBoxColumn
            {
                Name = "Position",
                HeaderText = "№",
                Width = Math.Max(70, gridWidth / 6),
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colPosition.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPosition.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Колонка количества фото
            var colCount = new DataGridViewTextBoxColumn
            {
                Name = "Count",
                HeaderText = "Количество фото",
                Width = Math.Max(180, gridWidth - colPosition.Width - 20),
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _grid.Columns.AddRange(colPosition, colCount);
            
            _grid.CellValueChanged += Grid_CellValueChanged;
            _grid.CellValidating += Grid_CellValidating;
            _grid.KeyDown += Grid_KeyDown;
            _grid.EditingControlShowing += Grid_EditingControlShowing;

            // Панель итогов
            var panelTotals = new Panel
            {
                Location = new Point(20, 20 + gridHeight + 20),
                Size = new Size(gridWidth, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            _lblTotalPhotos = new Label
            {
                Location = new Point(15, 15),
                Size = new Size(220, 26),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Text = "Всего фото: 0"
            };

            _lblTotalAmount = new Label
            {
                Location = new Point(15, 55),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 0),
                Text = "Сумма: 0 ₽"
            };

            panelTotals.Controls.AddRange(new Control[] { _lblTotalPhotos, _lblTotalAmount });

            // Кнопки
            _btnAddPosition = new Button
            {
                Location = new Point(20, panelTotals.Bottom + 20),
                Size = new Size(160, 44),
                Text = "Добавить позицию",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                UseVisualStyleBackColor = true
            };
            _btnAddPosition.Click += BtnAddPosition_Click;

            _btnOk = new Button
            {
                Location = new Point(190, panelTotals.Bottom + 20),
                Size = new Size(170, 44),
                Text = "Показать клиенту",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnOk.Click += BtnOk_Click;

            _btnCancel = new Button
            {
                Location = new Point(370, panelTotals.Bottom + 20),
                Size = new Size(150, 44),
                Text = "Очистить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                UseVisualStyleBackColor = true
            };
            _btnCancel.Click += BtnCancel_Click;

            Controls.AddRange(new Control[] { _grid, panelTotals, _btnAddPosition, _btnOk, _btnCancel });

            // При закрытии формы - просто скрываем
            FormClosing += (s, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                    ResetOrder();
                }
            };

            Activated += (s, e) =>
            {
                BeginInvoke(() =>
                {
                    if (!_grid.IsCurrentCellInEditMode)
                    {
                        _grid.Focus();
                        _grid.BeginEdit(true);
                    }
                });
            };
        }

        private void Grid_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.KeyPress -= TextBox_KeyPress;
                textBox.KeyPress += TextBox_KeyPress;
            }
        }

        private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры и управляющие символы
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Grid_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_grid.CurrentCell == null) return;
            
            int currentRow = _grid.CurrentCell.RowIndex;
            
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                
                // Завершаем редактирование текущей ячейки
                _grid.EndEdit();
                
                // Переход к следующей строке или создание новой
                if (currentRow >= _grid.Rows.Count - 1)
                {
                    AddPosition();
                }
                
                if (currentRow + 1 < _grid.Rows.Count)
                {
                    _grid.CurrentCell = _grid.Rows[currentRow + 1].Cells["Count"];
                    _grid.BeginEdit(true);
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == Keys.Enter && (_grid.Focused || _grid.IsCurrentCellInEditMode))
            {
                _grid.EndEdit();
                
                int currentRow = _grid.CurrentCell?.RowIndex ?? 0;
                
                // Если последняя строка — создаём новую
                if (currentRow >= _grid.Rows.Count - 1)
                {
                    AddPosition();
                }
                else
                {
                    // Переходим к следующей строке
                    _grid.CurrentCell = _grid.Rows[currentRow + 1].Cells["Count"];
                }
                
                _grid.BeginEdit(true);
                return true; // Подавляем стандартную обработку Tab
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Grid_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_grid.Columns[e.ColumnIndex].Name != "Count") return;
            
            string value = e.FormattedValue?.ToString() ?? "";
            
            if (string.IsNullOrWhiteSpace(value))
                return; // Пустое значение разрешено (= 0)
                
            if (!int.TryParse(value, out int num) || num < 0)
            {
                e.Cancel = true;
                MessageBox.Show("Введите целое неотрицательное число", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                UpdateTotals();
            }
        }

        private void BtnAddPosition_Click(object? sender, EventArgs e)
        {
            AddPosition();
        }

        private void AddPosition()
        {
            int newPosition = _grid.Rows.Count + 1;
            _grid.Rows.Add(newPosition.ToString(), "");
            
            // Фокус на новую строку
            _grid.CurrentCell = _grid.Rows[_grid.Rows.Count - 1].Cells["Count"];
            _grid.BeginEdit(true);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            _grid.EndEdit();
            
            var positions = GetPositions();
            int totalPhotos = positions.Sum(p => p.Count);
            
            if (totalPhotos == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну фотографию!", "Внимание", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Создаём заказ
            var order = new Order
            {
                Id = Order.GenerateId(),
                DateTime = DateTime.Now,
                TotalPhotos = totalPhotos,
                TotalAmount = PriceCalculator.CalculateTotal(positions.Select(p => p.Count)),
                Positions = positions
            };

            // Сохраняем
            try
            {
                OrderStorage.SaveOrder(order);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Скрываем форму фотографа
            Hide();

            // Показываем клиентский экран
            using (var clientForm = new ClientForm(order))
            {
                clientForm.ShowDialog();
            }

            // После закрытия клиентской формы - сброс
            ResetOrder();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ResetOrder();
        }

        private void ResetOrder()
        {
            _grid.Rows.Clear();
            _grid.Rows.Add("1", "");
            UpdateTotals();
            
            _grid.CurrentCell = _grid.Rows[0].Cells["Count"];
        }

        private List<OrderPosition> GetPositions()
        {
            var positions = new List<OrderPosition>();
            
            for (int i = 0; i < _grid.Rows.Count; i++)
            {
                string? countStr = _grid.Rows[i].Cells["Count"].Value?.ToString();
                int count = 0;
                
                if (!string.IsNullOrWhiteSpace(countStr))
                    int.TryParse(countStr, out count);
                
                if (count > 0)
                {
                    positions.Add(new OrderPosition
                    {
                        Position = i + 1,
                        Count = count
                    });
                }
            }
            
            return positions;
        }

        private void UpdateTotals()
        {
            var positions = GetPositions();
            int totalPhotos = positions.Sum(p => p.Count);
            decimal totalAmount = PriceCalculator.CalculateTotal(positions.Select(p => p.Count));
            
            _lblTotalPhotos.Text = $"Всего фото: {totalPhotos}";
            _lblTotalAmount.Text = $"Сумма: {totalAmount:N0} ₽";
        }

        // protected override void OnShown(EventArgs e)
        // {
        //     base.OnShown(e);
            
        //     // При показе формы - фокус на поле ввода
        //     _grid.Focus();
        //     if (_grid.Rows.Count > 0)
        //     {
        //         _grid.CurrentCell = _grid.Rows[0].Cells["Count"];
        //         _grid.BeginEdit(true);
        //     }
        // }
    }
}
