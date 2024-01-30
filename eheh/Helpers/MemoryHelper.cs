using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace eheh.Helpers
{
    public class MemoryHelper
    {
        private IntPtr pHandel;

        [DllImport("kernel32.dll")]
        private static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] [Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] [Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        public bool Process_Handle(string ProcessName)
        {
            try
            {
                Process[] processesByName = Process.GetProcessesByName(ProcessName);
                if (processesByName.Length == 0)
                {
                    return false;
                }
                pHandel = processesByName[0].Handle;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process_Handle - " + ex.Message);
                return false;
            }
        }

        private byte[] Read(int Address, int Length)
        {
            byte[] array = new byte[Length];
            IntPtr lpNumberOfBytesWritten = IntPtr.Zero;
            ReadProcessMemory(pHandel, (IntPtr)Address, array, (uint)array.Length, out lpNumberOfBytesWritten);
            return array;
        }

        private void Write(int Address, int Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);
            IntPtr lpNumberOfBytesWritten = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, bytes, (uint)bytes.Length, out lpNumberOfBytesWritten);
        }

        public void WriteInteger(int Address, int Value)
        {
            Write(Address, Value);
        }

        public void WriteString(int Address, string Text)
        {
            byte[] bytes = new ASCIIEncoding().GetBytes(Text);
            IntPtr lpNumberOfBytesWritten = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, bytes, (uint)bytes.Length, out lpNumberOfBytesWritten);
        }

        public void WriteBytes(int Address, byte[] Bytes)
        {
            IntPtr lpNumberOfBytesWritten = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, Bytes, (uint)Bytes.Length, out lpNumberOfBytesWritten);
        }

        public void WriteNOP(int Address)
        {
            byte[] array = new byte[5]
            {
                144,
                144,
                144,
                144,
                144
            };
            IntPtr lpNumberOfBytesWritten = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, array, (uint)array.Length, out lpNumberOfBytesWritten);
        }

        public int ReadInteger(int Address, int Length = 4)
        {
            return BitConverter.ToInt32(Read(Address, Length), 0);
        }

        public string ReadString(int Address, int Length = 4)
        {
            return new ASCIIEncoding().GetString(Read(Address, Length));
        }

        public byte[] ReadBytes(int Address, int Length)
        {
            return Read(Address, Length);
        }
    }
}
