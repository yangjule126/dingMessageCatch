using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace dingMessageCatch
{
    class catchMessage
    {
        int posStart = Convert.ToInt32(Config.GetAppConfig("groupPosFirstGroupy"));
        int groupPosFirstGroupx = Convert.ToInt32(Config.GetAppConfig("groupPosFirstGroupx"));
        int groupLableGap = Convert.ToInt32(Config.GetAppConfig("groupLableGap"));
        int groupPosGetGroupNamex = Convert.ToInt32(Config.GetAppConfig("groupPosGetGroupNamex"));
        int groupPosGetGroupNamey = Convert.ToInt32(Config.GetAppConfig("groupPosGetGroupNamey"));
        int groupPosGetGroupMsgx = Convert.ToInt32(Config.GetAppConfig("groupPosGetGroupMsgx"));
        int groupPosGetGroupMsgy = Convert.ToInt32(Config.GetAppConfig("groupPosGetGroupMsgy"));

        public Dictionary<string, string> LastEndMeg = new Dictionary<string, string>();
        public int runFlag = 0;
        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);
        public void MoveMouseToPoint(System.Drawing.Point p)
        {
            SetCursorPos(p.X, p.Y);
        }
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool
        SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strclassName, string strWindowName);
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MAXIMIZE = 0xF030;
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //move mouse
        const int MOUSEEVENTF_MOVE = 0x0001;
        //simulation mouse right button click
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //simulation mouse left button click 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //absolute coordinate
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        /// <summary>
        /// captrue the dingding message
        /// </summary>
        /// <param name="posFirstGroup"></param>
        public Dictionary<string, string> getDingMessage(int posFirstGroup)
        {
             Dictionary<string, string> dingMessage = new Dictionary<string, string>();
            IDataObject idata = Clipboard.GetDataObject();
            //max the ding windows
            Process[] processes = System.Diagnostics.Process.GetProcessesByName("DingTalk");
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    IntPtr hWnd = processes[i].MainWindowHandle;
                    SendMessage(hWnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                    SetForegroundWindow(hWnd);
                }
            }
            //locate first group point and click
            Point point = new Point(groupPosFirstGroupx, posFirstGroup);
            MoveMouseToPoint(point);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //get group name
            Thread.Sleep(2000);
            point = new Point(groupPosGetGroupNamex, groupPosGetGroupNamey);
            MoveMouseToPoint(point);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(1000);
            String GroupName = (String)idata.GetData(DataFormats.Text);
            String captrueGroupName = Config.GetAppConfig("groupName");
            if (captrueGroupName.Contains(GroupName))
            {
                //locate first group point and click
                point = new Point(groupPosFirstGroupx, posFirstGroup);
                MoveMouseToPoint(point);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                Thread.Sleep(2000);
                //move mouse to ding message session tail row
                Thread.Sleep(2000);
                point = new Point(groupPosGetGroupMsgx, groupPosGetGroupMsgy);
                MoveMouseToPoint(point);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                //press ctrl+A
                Thread.Sleep(20);
                keybd_event(Keys.ControlKey, 0, 0, 0);
                keybd_event(Keys.A, 0, 0, 0);
                keybd_event(Keys.ControlKey, 0, 2, 0);
                //press ctrl+c 
                Thread.Sleep(2000);
                keybd_event(Keys.ControlKey, 0, 0, 0);
                keybd_event(Keys.C, 0, 0, 0);
                keybd_event(Keys.ControlKey, 0, 2, 0);
                Thread.Sleep(1000);

                //cancel the select message
                point = new Point(groupPosGetGroupMsgx, groupPosGetGroupMsgy);
                MoveMouseToPoint(point);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                String messageOriginal = (String)idata.GetData(DataFormats.Text);

                //drop not chinesses character
                messageOriginal = Regex.Replace(messageOriginal, @"[^\u4e00-\u9fa5]", "");

                //drop space and \n\t\r
                messageOriginal = messageOriginal.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("刚刚", "").Replace("分钟前", "").Replace("今天", "").Replace("前天", "");

                string msg = messageOriginal;
                System.Console.WriteLine("message_original:" + msg);
                if (runFlag != 0)
                {
                    String lastReadFlag = LastEndMeg[GroupName];
                    int indexNumber = msg.LastIndexOf(lastReadFlag);
                    msg = msg.Substring(indexNumber + lastReadFlag.Length);
                    System.Console.WriteLine("message_exclude:" + msg);
                }

                //if this's first times run it then >>
                String cur_msg_end_sign = null;
                if (msg != null && msg != "")
                {
                    cur_msg_end_sign = messageOriginal.Substring(messageOriginal.Length - 20);
                }
                else
                {
                    cur_msg_end_sign = LastEndMeg[GroupName];
                }

                Boolean flag_exsit = false;
                foreach (String key in LastEndMeg.Keys)
                {
                    if (key == GroupName)
                    { flag_exsit = true; }
                }
                if (flag_exsit)
                {
                    LastEndMeg[GroupName] = cur_msg_end_sign;
                }
                else
                {
                    LastEndMeg.Add(GroupName, cur_msg_end_sign);
                }

                dingMessage.Add("GroupName", GroupName);
                dingMessage.Add("Message", msg);
                return dingMessage;
            }
            else
            {
                return null;
            }
           
            
        }
    }
}
