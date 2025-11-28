using System;
using System.Drawing;
using System.Windows.Forms;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Контекст приложения, работающего в системном трее.
    /// </summary>
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly HotkeyWindow _hotkeyWindow;
        private PhotographerForm? _photographerForm;

        public TrayApplicationContext()
        {
            // Создаём иконку в трее
            _trayIcon = new NotifyIcon
            {
                Icon = CreateDefaultIcon(),
                Text = "Photo Order Calculator\nCtrl+Shift+K - открыть",
                Visible = true,
                ContextMenuStrip = CreateContextMenu()
            };
            
            _trayIcon.DoubleClick += (s, e) => ShowPhotographerForm();

            // Регистрируем глобальную горячую клавишу Ctrl+Shift+K
            _hotkeyWindow = new HotkeyWindow();
            try
            {
                _hotkeyWindow.RegisterHotkey(
                    GlobalHotkey.MOD_CONTROL | GlobalHotkey.MOD_SHIFT, 
                    Keys.K
                );
                _hotkeyWindow.HotkeyPressed += (s, e) => ShowPhotographerForm();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message + "\n\nПопробуйте закрыть другие приложения или изменить горячую клавишу.", 
                    "Предупреждение", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                );
            }

            // Показываем уведомление о запуске
            _trayIcon.ShowBalloonTip(
                2000, 
                "Photo Order Calculator", 
                "Приложение запущено. Нажмите Ctrl+Shift+K для открытия.", 
                ToolTipIcon.Info
            );
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();
            
            menu.Items.Add("Открыть (Ctrl+Shift+K)", null, (s, e) => ShowPhotographerForm());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Открыть папку с данными", null, (s, e) => OpenDataFolder());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Выход", null, (s, e) => ExitApplication());
            
            return menu;
        }

        private void ShowPhotographerForm()
        {
            if (_photographerForm == null || _photographerForm.IsDisposed)
            {
                _photographerForm = new PhotographerForm();
            }
            
            if (!_photographerForm.Visible)
            {
                _photographerForm.Show();
            }
            
            if (_photographerForm.WindowState == FormWindowState.Minimized)
                _photographerForm.WindowState = FormWindowState.Normal;
            
            _photographerForm.Activate();
            _photographerForm.BringToFront();
        }

        private void OpenDataFolder()
        {
            string path = OrderStorage.GetDataFilePath();
            string? folder = System.IO.Path.GetDirectoryName(path);
            if (folder != null && System.IO.Directory.Exists(folder))
            {
                System.Diagnostics.Process.Start("explorer.exe", folder);
            }
        }

        private void ExitApplication()
        {
            _trayIcon.Visible = false;
            _hotkeyWindow.Dispose();
            _photographerForm?.Close();
            Application.Exit();
        }

        /// <summary>
        /// Создаёт простую иконку программно (камера).
        /// </summary>
        private Icon CreateDefaultIcon()
        {
            using var bitmap = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bitmap);
            
            g.Clear(Color.Transparent);
            
            // Рисуем простую иконку камеры
            using var brush = new SolidBrush(Color.FromArgb(70, 130, 180)); // Steel Blue
            using var pen = new Pen(Color.White, 2);
            
            // Корпус камеры
            g.FillRectangle(brush, 4, 10, 24, 16);
            
            // Объектив
            g.FillEllipse(Brushes.White, 10, 13, 12, 10);
            g.FillEllipse(brush, 12, 15, 8, 6);
            
            // Вспышка
            g.FillRectangle(Brushes.White, 6, 6, 6, 4);
            
            return Icon.FromHandle(bitmap.GetHicon());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _trayIcon.Dispose();
                _hotkeyWindow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
