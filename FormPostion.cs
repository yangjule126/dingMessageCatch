using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dingMessageCatch
{
    public partial class FormPostion : Form
    {
        public FormPostion()
        {
            InitializeComponent();
        }

        MousePoint mousePoint = new MousePoint();
        private void FormPostion_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;//timer控件的执行频率
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxPos.Text = mousePoint.getMousePoint();
        }
    }
}
