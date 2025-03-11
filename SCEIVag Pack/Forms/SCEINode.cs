using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEIVag_Pack.Forms
{
    public partial class SCEINode : TreeNode
    {
        public IECS.VAGInfo.Information Information;
        public SCEINode()
        {
            InitializeComponent();
        }
    }
}
