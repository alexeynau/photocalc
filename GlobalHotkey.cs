using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Регистрация глобальных горячих клавиш Windows.
    /// </summary>
    public class GlobalHotkey : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;
        
        // Модификаторы
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;
        public const uint MOD_NOREPEAT = 0x4000;

        private readonly int _id;
        private readonly IntPtr _hwnd;
        private bool _disposed;

        public GlobalHotkey(IntPtr hwnd, int id, uint modifiers, Keys key)
        {
            _hwnd = hwnd;
            _id = id;
            
            if (!RegisterHotKey(hwnd, id, modifiers | MOD_NOREPEAT, (uint)key))
            {
                throw new InvalidOperationException(
                    $"Не удалось зарегистрировать горячую клавишу. " +
                    $"Возможно, она уже используется другим приложением.");
            }
        }

        public int Id => _id;

        public void Dispose()
        {
            if (!_disposed)
            {
                UnregisterHotKey(_hwnd, _id);
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Невидимое окно для получения сообщений о горячих клавишах.
    /// </summary>
    public class HotkeyWindow : NativeWindow, IDisposable
    {
        private const int HOTKEY_ID = 1;
        private GlobalHotkey? _hotkey;
        
        public event EventHandler? HotkeyPressed;

        public HotkeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        public void RegisterHotkey(uint modifiers, Keys key)
        {
            _hotkey?.Dispose();
            _hotkey = new GlobalHotkey(Handle, HOTKEY_ID, modifiers, key);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == GlobalHotkey.WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            _hotkey?.Dispose();
            DestroyHandle();
        }
    }
}
