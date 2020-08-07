using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dingMessageCatch
{
    class MousePoint
    {
        public String getMousePoint()
        {
            Point ms = Control.MousePosition;
            MouseButtons mb = Control.MouseButtons;
            return string.Format("当前鼠标位置：{0},{1}", ms.X, ms.Y);
        }
    }
}
