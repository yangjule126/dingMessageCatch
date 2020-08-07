using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace dingMessageCatch
{
    public class HOOK : IMessageFilter
    {
        public delegate void HotkeyEventHandler(int HotKeyID);
        public event HotkeyEventHandler OnHotkey;
        private Hashtable keyIDs = new Hashtable();
        private IntPtr hWnd;

        /// <summary>
        /// 辅助按键
        /// </summary>
        public enum KeyFlags
        {
            MOD_NULL = 0x0,
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        /// <summary>
        /// 注册热键API
        /// </summary>
        [DllImport("user32.dll")]
        public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

        /// <summary>
        /// 注销热键API
        /// </summary>
        [DllImport("user32.dll")]
        public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

        /// <summary>
        /// 全局原子表添加原子
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern UInt32 GlobalAddAtom(String lpString);

        /// <summary>
        /// 全局原子表删除原子
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hWnd">当前句柄</param>
        public void Hotkey(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// 注册热键
        /// </summary>
        public int RegisterHotkey(Keys Key, KeyFlags keyflags)
        {
            UInt32 hotkeyid = GlobalAddAtom(System.Guid.NewGuid().ToString());
            RegisterHotKey((IntPtr)hWnd, hotkeyid, (UInt32)keyflags, (UInt32)Key);
            keyIDs.Add(hotkeyid, hotkeyid);
            return (int)hotkeyid;
        }

        /// <summary>
        /// 注销所有热键
        /// </summary>
        public void UnregisterHotkeys()
        {
            Application.RemoveMessageFilter(this);
            foreach (UInt32 key in keyIDs.Values)
            {
                UnregisterHotKey(hWnd, key);
                GlobalDeleteAtom(key);
            }
        }

        /// <summary>
        /// 消息筛选
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x312)
            {
                if (OnHotkey != null)
                {
                    foreach (UInt32 key in keyIDs.Values)
                    {
                        if ((UInt32)m.WParam == key)
                        {
                            OnHotkey((int)m.WParam);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
