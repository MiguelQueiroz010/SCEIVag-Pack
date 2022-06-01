using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SCEIVag_Pack
{
    public partial class ListadeIECS : Form
    {
        Form1 f1;
        public ListadeIECS(Form1 form1)
        {
            InitializeComponent();
            f1 = form1;
        }
        public void Listar()
        {
            filenamlab.Text = Path.GetFileName(f1.filename);
            quantlab.Text = "Quantia: " + f1.container.fileCount;
            f1.extrairTudoToolStripMenuItem.Enabled = true;
            
            int k = 0;
            foreach(var IECS in f1.container.IECSsizes)
            {
                ListViewItem item = new ListViewItem(k.ToString());

                string sceiname = "iecs" + k.ToString() + ".bhd";

                if(f1.FileList!=null)
                    sceiname = f1.FileList.ElementAt(k).Key;

                item.SubItems.Add(sceiname);

                item.SubItems.Add(IECS.ToString());
                listView1.Items.Add(item);
                k++;
            }
        }
        private void ListadeIECS_Load(object sender, EventArgs e)
        {
            Listar();
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
            {
                f1.extrairuniccon.Enabled = true;
                if (f1.sceifile != null)
                {
                    f1.sceifile = null;
                    f1.ShowHide();
                }
                f1.listView1.Items.Clear();
                f1.sceifile = f1.container.sceiFiles[listView1.SelectedIndices[0]];
                #region Acionar Labels
                f1.filenamelabel.Text = "Nome: " + Path.GetFileName(listView1.SelectedItems[0].SubItems[1].Text);
                f1.verslabel.Text = "Pack Versão: " + f1.sceifile.version.Versão.ToString();
                f1.vagnlabel.Text = "Número de Áudios: " + f1.sceifile.vagi.VAGcount.ToString();
                #endregion
                f1.ListInsert();
            }
            else
            {
                f1.extrairuniccon.Enabled = false;
            }
            
        }
    }
}
