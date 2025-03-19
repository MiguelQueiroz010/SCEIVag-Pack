using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SCEIVag_Pack.Forms
{
    public partial class SCEINode : TreeNode
    {
        public IECS.VAGInfo.Information Information;
        public IECS.Prog.Entry Entry;
        public IECS.VagSamples.Sample Sample;
        public IECS.Sset.SampleSet SampleSet;
        public SCEINode()
        {
            InitializeComponent();
        }
    }
}
