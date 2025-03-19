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
        Main f1;
        public ListadeIECS(Main form1)
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
            foreach(var IECS in f1.container.sCEI_Entries)
            {
                if (IECS.Pack_Offset.ToString("X2") != "FFFFFFFF")
                {
                    ListViewItem item = new ListViewItem(k.ToString());

                    string sceiname = "iecs" + k.ToString() + ".bhd";

                    if (f1.FileList != null)
                        sceiname = f1.FileList.ElementAt(k).Key;

                    item.SubItems.Add(sceiname);

                    item.SubItems.Add(IECS.Pack_Size.ToString());
                    listView1.Items.Add(item);
                    k++;
                }
                else
                {
                    ListViewItem item = new ListViewItem("");
                    item.ForeColor = Color.Red;
                    string sceiname = "ENTRADA VAZIA";
                    item.SubItems.Add(sceiname);
                    listView1.Items.Add(item);
                }
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
                if (listView1.SelectedItems[0].SubItems[1].Text != "ENTRADA VAZIA")
                {
                    f1.extrairuniccon.Enabled = true;
                    //if (f1.sceifile != null)
                    //{
                    //    f1.sceifile = null;
                    //    f1.ShowHide();
                    //}
                    f1.treeView1.Nodes.Clear();
                    f1.sceifile = f1.container.sCEI_Entries[listView1.SelectedIndices[0]].scei_File;
                    #region Acionar Labels
                    f1.linkLabel1.Text = "Nome: " + Path.GetFileName(listView1.SelectedItems[0].SubItems[1].Text);
                    #endregion
                    f1.TreePopulate();
                    f1.addToolStripMenuItem.Enabled = true;
                }
            }
            else
            {
                f1.extrairuniccon.Enabled = false;
            }
            
        }
    }
}
