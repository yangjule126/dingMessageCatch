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
    public partial class FormSet : Form
    {
        public FormSet()
        {
            InitializeComponent();
        }

        private void FormSet_Load(object sender, EventArgs e)
        {
           textBoxGroupName.Text= Config.GetAppConfig("groupName");
           textBoxGroupCount.Text = Config.GetAppConfig("groupCount");
           textBoxPosFirstGroupx.Text = Config.GetAppConfig("groupPosFirstGroupx");
           textBoxPosFirstGroupy.Text = Config.GetAppConfig("groupPosFirstGroupy");
            textBoxPosGetGroupNamex.Text = Config.GetAppConfig("groupPosGetGroupNamex");
            textBoxPosGetGroupNamey.Text = Config.GetAppConfig("groupPosGetGroupNamey");
            textBoxPosGetMegx.Text = Config.GetAppConfig("groupPosGetGroupMsgx");
            textBoxPosGetMegy.Text = Config.GetAppConfig("groupPosGetGroupMsgy");
            textBoxGroupLableGap.Text = Config.GetAppConfig("groupLableGap");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Config.UpdateAppConfig("groupName", textBoxGroupName.Text);
            Config.UpdateAppConfig("groupCount", textBoxGroupCount.Text);
            Config.UpdateAppConfig("groupPosFirstGroupx", textBoxPosFirstGroupx.Text);
            Config.UpdateAppConfig("groupPosFirstGroupy", textBoxPosFirstGroupy.Text);
            Config.UpdateAppConfig("groupPosGetGroupNamex", textBoxPosGetGroupNamex.Text);
            Config.UpdateAppConfig("groupPosGetGroupNamey", textBoxPosGetGroupNamey.Text);
            Config.UpdateAppConfig("groupPosGetGroupMsgx", textBoxPosGetMegx.Text);
            Config.UpdateAppConfig("groupPosGetGroupMsgy", textBoxPosGetMegy.Text);
            Config.UpdateAppConfig("groupLableGap", textBoxGroupLableGap.Text);
        }
    }
}
