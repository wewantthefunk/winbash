using System.Runtime.InteropServices;
using System.Text;

namespace bash.dotnet
{
    class Clipboard {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        // The CF_TEXT format represents ASCII text.
        private const uint CF_TEXT = 1;

        public string GetText() {
            string result = string.Empty;
            if (!OpenClipboard(IntPtr.Zero))
            {
                return String.Empty;
            }

            if (!IsClipboardFormatAvailable(CF_TEXT))
            {
                CloseClipboard();
                return String.Empty;
            }

            IntPtr handle = GetClipboardData(CF_TEXT);
            if (handle == IntPtr.Zero)
            {
                CloseClipboard();
                return String.Empty;
            }

            IntPtr pointer = IntPtr.Zero;

            try {
                pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }

                int size = 0;
                while (Marshal.ReadByte(pointer, size) != 0)
                {
                    size++;
                }

                byte[] buffer = new byte[size];
                Marshal.Copy(pointer, buffer, 0, buffer.Length);

                result = Encoding.ASCII.GetString(buffer);
            }
            catch {
                result = String.Empty;
            }
            finally {
                if (pointer != IntPtr.Zero)
                {
                    GlobalUnlock(handle);
                }

                CloseClipboard();
            }

            return result;
        }
    }
}
