using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace dingMessageCatch
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        catchMessage catchMessage = new catchMessage();
        
        Dictionary<string, string> dingMessage = new Dictionary<string, string>();
        public HOOK hook = new HOOK();
        private int hotKey_Ctrl_F2;
        
        int groupCount = Convert.ToInt32(Config.GetAppConfig("groupCount"));
        int groupLableGap =Convert.ToInt32(Config.GetAppConfig("groupLableGap"));
        private void FormMain_Load(object sender, EventArgs e)
        {

            hook.Hotkey(this.Handle);
            hotKey_Ctrl_F2 = hook.RegisterHotkey(System.Windows.Forms.Keys.F2, HOOK.KeyFlags.MOD_CONTROL);
            hook.OnHotkey += new HOOK.HotkeyEventHandler(OnHotkey);
        }


        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) 
            {
                this.ShowInTaskbar = false; 
                notifyIcon1.Visible = true;
            }
        }


        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                    Thread thread = new Thread(runThread);
                    thread.IsBackground = true;
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();       
        }

        private void runThread()
        {
            while (true)
            {
                int posStart = Convert.ToInt32(Config.GetAppConfig("groupPosFirstGroupy"));

                for (int i = 0; i < groupCount; i++)
                {                  
                    dingMessage = catchMessage.getDingMessage(posStart);
                    if (dingMessage!=null)
                    {
                        System.Console.Write(dingMessage["GroupName"]+":");
                        System.Console.Write(dingMessage["Message"]);
                        if (textBoxGroupName.InvokeRequired)
                        {
                            textBoxGroupName.Invoke(new MethodInvoker(delegate
                            {
                                textBoxGroupName.Text = (dingMessage["GroupName"].ToString());
                            }));
                        }
                        if (textBoxMessage.InvokeRequired)
                        {
                            textBoxMessage.Invoke(new MethodInvoker(delegate
                            {
                                textBoxMessage.Text = (dingMessage["Message"].ToString());
                            }));
                        }
                        posStart = posStart + groupLableGap;
                        if (catchMessage.runFlag == 1&& dingMessage["Message"].ToString()!=null && dingMessage["Message"].ToString() != "")
                        {
                          Kafka.Produce(dingMessage["GroupName"].ToString() + ":" + dingMessage["Message"].ToString());
                        }
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        posStart = posStart + groupLableGap;
                    }
                    
                }
                catchMessage.runFlag = 1;

            }

        }

        private void OnHotkey(int HotkeyID)
        {
            if (HotkeyID == hotKey_Ctrl_F2)
            {
                notifyIcon1.Dispose();
                this.Close();
                
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            hook.UnregisterHotkeys();
            notifyIcon1.Dispose();
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            FormSet formSet = new FormSet();
            formSet.Show();
        }

        private void buttonPos_Click(object sender, EventArgs e)
        {
            FormPostion formPostion = new FormPostion();
            formPostion.Show();
        }
    }
}

