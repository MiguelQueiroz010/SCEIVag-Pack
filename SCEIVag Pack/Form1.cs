using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static SCEIVag_Pack.Bin;


namespace SCEIVag_Pack
{
    public partial class Form1 : Form
    {
        public IECS sceifile;
        public BINContainer container;
        public ListadeIECS listadeIECS;
        public Graphics paneSave;
        public string filename;
        public Form1()
        {
            InitializeComponent();
        }
        public void ShowHide()
        {
            listView1.Visible = !listView1.Visible;
            salvarToolStripMenuItem.Enabled = !salvarToolStripMenuItem.Enabled;
            fecharToolStripMenuItem.Enabled = !fecharToolStripMenuItem.Enabled;
            groupBox1.Visible = !groupBox1.Visible;
            extrairTodosToolStripMenuItem.Enabled = !extrairTodosToolStripMenuItem.Enabled;
        }
        public void ListInsert()
        {
            int i = 1;
            foreach (var vgs in sceifile.vagi.Vdata)
            {
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add("stream" + i.ToString() + ".vag");
                item.SubItems.Add(Convert.ToString(vgs.Frequency) + "Hz");
                listView1.Items.Add(item);
                i++;
            }
            ShowHide();
        }
        public void Fechar()
        {
            sceifile = null;
            filename = null;
            container = null;
            listadeIECS = null;
            extrairToolStripMenuItem.Enabled = false;
            importarVAGToolStripMenuItem.Enabled = false;
            extrairTudoToolStripMenuItem.Enabled = false;
            fecharToolStripMenuItem1.Enabled = false;
            extrairuniccon.Enabled = false;
            listView1.Items.Clear();
            try
            {
                foreach (var form in Application.OpenForms)
                {
                    try
                    {
                        var opt = form as ListadeIECS;
                        opt.Close();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
            if(listView1.Visible)
                ShowHide();
        }
        public void Extrair()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Ps2 Vag Audio(*.vag)|*.vag";
            save.Title = "Escolha onde salvar o áudio VAG selecionado";
            save.FileName = listView1.SelectedItems[0].SubItems[1].Text;
            if(save.ShowDialog()==DialogResult.OK)
            {
                byte[] VAG = ReadBlock(sceifile.StreamData, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamOffset, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamSize);
                File.WriteAllBytes(save.FileName, VAG);
                MessageBox.Show("Concluído!\nExtraído para: " + Path.GetDirectoryName(save.FileName) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void Importar()
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Title = "Selecione o áudio VAG para importar e sobrescrever";
            opn.Filter = "PS2 Vag Audio(*.vag)|*.vag";
            if (opn.ShowDialog() == DialogResult.OK)
            {
                if (container != null)
                {
                    #region Verificar VAG por header
                    byte[] replace = File.ReadAllBytes(opn.FileName);
                    if(Encoding.Default.GetString(ReadBlock(replace, 0, 3))=="VAG")
                    {
                        replace = ReadBlock(replace, 0x30, (uint)replace.Length - 0x30);
                    }
                    #endregion
                    int offset = container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].vagi.Vdata[listView1.SelectedIndices[0]].StreamOffset;
                    int size = container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].vagi.Vdata[listView1.SelectedIndices[0]].StreamSize;
                    if (replace.Length == size)
                    {
                        Array.Copy(replace, 0, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].StreamData, offset, size);
                    }
                    else if (replace.Length < size)
                    {
                        byte[] nreplace = new byte[size];
                        Array.Copy(replace, nreplace, replace.Length);
                        Array.Copy(nreplace, 0, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].StreamData, offset, size);
                    }
                    Array.Copy(container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].StreamData, 0, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].Input, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].VAGS_Offset, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].VAGS_Size);
                    int COffset = container.IECSoffsets[listadeIECS.listView1.SelectedIndices[0]];
                    int CSize = container.IECSsizes[listadeIECS.listView1.SelectedIndices[0]];
                    byte[] sceiSDATA = container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].Input;
                    byte[] imp = new byte[CSize];
                    Array.Copy(sceiSDATA, imp, CSize);
                    Array.Copy(imp, 0, container.Container, COffset, CSize);

                    listView1.SelectedItems[0].ForeColor = Color.Red;
                    listadeIECS.listView1.SelectedItems[0].ForeColor = Color.Red;
                    MessageBox.Show("Importado no container!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    #region Verificar VAG por header
                    byte[] replace = File.ReadAllBytes(opn.FileName);
                    if (Encoding.Default.GetString(ReadBlock(replace, 0, 3)) == "VAG")
                    {
                        replace = ReadBlock(replace, 0x30, (uint)replace.Length - 0x30);
                    }
                    #endregion
                    int offset = sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamOffset;
                    int size = sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamSize;
                    if (replace.Length == size)
                    {
                        Array.Copy(replace, 0, sceifile.StreamData, offset, size);
                    }
                    else if (replace.Length < size)
                    {
                        byte[] nreplace = new byte[size];
                        Array.Copy(replace, nreplace, replace.Length);
                        Array.Copy(nreplace, 0, sceifile.StreamData, offset, size);
                    }
                    Array.Copy(sceifile.StreamData, 0, sceifile.Input, sceifile.VAGS_Offset, sceifile.VAGS_Size);
                    listView1.SelectedItems[0].ForeColor = Color.Red;
                    MessageBox.Show("Importado!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public void Salvar()
        {
            try
            {
                if (container != null)
                {
                    File.WriteAllBytes(filename, container.Container);
                }
                else
                {
                    File.WriteAllBytes(filename, sceifile.Input);
                }
                paneSave = panel1.CreateGraphics();
                panel1.Visible = true;
                panel1.BringToFront();
                paneSave.Clear(Color.White);
                paneSave.DrawString("Arquivo salvo!", new Font(FontFamily.GenericSerif, 24f,FontStyle.Bold), Brushes.DarkGreen, 0, 0);
                timer1.Enabled = true;
                timer1.Start();
                RefreshFile();
            }
            catch (Exception) {
                MessageBox.Show("Erro ao salvar o arquivo!\nVerifique se outro processo não o está usando!", "Salvar",
MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void RefreshFile()
        {
            listView1.Items.Clear();
            int i = 1;
            foreach (var vgs in sceifile.vagi.Vdata)
            {
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add("stream" + i.ToString() + ".vag");
                item.SubItems.Add(Convert.ToString(vgs.Frequency) + "Hz");
                listView1.Items.Add(item);
                i++;
            }
        }
        public void ExtrairUN()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Ps2 SCEI Audio Pack(*.bhd)|*.bhd";
            save.Title = "Escolha onde salvar o Container BHD Scei selecionado";
            save.FileName = listadeIECS.listView1.SelectedItems[0].SubItems[1].Text;
            if (save.ShowDialog() == DialogResult.OK)
            {
                int index = listadeIECS.listView1.SelectedIndices[0];
                byte[] BHD = ReadBlock(container.Container, (uint)container.IECSoffsets[index], (uint)container.IECSsizes[index]);
                File.WriteAllBytes(save.FileName, BHD);
                MessageBox.Show("Concluído!\nExtraído para: " + Path.GetDirectoryName(save.FileName) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void ExtrairTudo()
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            string folder;
            fold.Description = "Escolha a pasta onde extrair todos os áudios VAG";
            if (fold.ShowDialog() == DialogResult.OK)
            {
                if (container != null)
                {
                     folder = fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(listadeIECS.listView1.SelectedItems[0].SubItems[1].Text);
                }
                else
                {
                     folder = fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename);

                }
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                int i = 0;
                foreach (var vag in sceifile.vagi.Vdata)
                {
                    byte[] VAG = ReadBlock(sceifile.StreamData, (uint)vag.StreamOffset, (uint)vag.StreamSize);
                    File.WriteAllBytes(folder + @"\"+listView1.Items[i].SubItems[1].Text, VAG);
                    i++;
                }
                MessageBox.Show("Concluído!\nExtraído para: " + folder + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void ExtrairContainer()
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            fold.Description = "Escolha a pasta onde extrair o conteúdo do Container";
            if (fold.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename)))
                    Directory.CreateDirectory(fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename));
                for(int i =0; i< container.fileCount;i++)
                {
                    byte[] IEC = ReadBlock(container.Container, (uint)container.IECSoffsets[i], (uint)container.IECSsizes[i]);
                    File.WriteAllBytes(fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename) + @"\" + listadeIECS.listView1.Items[i].SubItems[1].Text, IEC);
                }
                MessageBox.Show("Concluído!\nExtraído para: " + fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void RecriarContainer()
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            fold.ShowNewFolderButton = false;
            fold.Description = "Escolha a pasta onde estão os arquivos BHD do container(somente iecs[n].bhd)";
            if (fold.ShowDialog() == DialogResult.OK)
            {
                if (Directory.EnumerateFiles(fold.SelectedPath).Count() > 1)
                {
                    var tosave = new List<byte>();
                    int i = 0;
                    foreach(var file in Directory.EnumerateFiles(fold.SelectedPath))
                    {
                        foreach (var f in Directory.EnumerateFiles(fold.SelectedPath))
                        {
                            if (Path.GetFileName(f) == "iecs" + i.ToString() + ".bhd")
                            {
                                byte[] filebin = File.ReadAllBytes(f);
                                tosave.AddRange(filebin);
                            }
                        }
                        i++;
                    }
                    SaveFileDialog saveCON = new SaveFileDialog();
                    saveCON.Filter = "Arquivo BIN(*.bin)|*.bin";
                    saveCON.FileName = fold.SelectedPath.Split(new string[] { Path.GetDirectoryName(fold.SelectedPath),@"\","/" },StringSplitOptions.RemoveEmptyEntries)[0];
                    saveCON.Title = "Escolha onde salvar o novo Container";
                    if (saveCON.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveCON.FileName, tosave.ToArray());
                        MessageBox.Show("Concluído!\nRecriado para: " + Path.GetDirectoryName(fold.SelectedPath)+@"\"+Path.GetFileName(saveCON.FileName), "Extração", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Essa pasta tá vazia meu filho!", "Container", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        void Abrir(bool isDrag)
        {
            switch(isDrag)
            {
                case true:
                    #region Verificar abertos
                    if (sceifile != null | container != null)
                    {
                        Fechar();
                    }
                    #endregion
                    #region Abrir arquivo
                    sceifile = new IECS(File.ReadAllBytes(filename));
                    #endregion
                    #region Acionar Labels
                    filenamelabel.Text = "Nome: " + Path.GetFileName(filename);
                    verslabel.Text = "Pack Versão: " + sceifile.version.Versão.ToString();
                    vagnlabel.Text = "Número de Áudios: " + sceifile.vagi.VAGcount.ToString();
                    #endregion
                    #region Adicionar na lista
                    ListInsert();
                    #endregion
                    break;

                case false:
                    var open = new OpenFileDialog();
                    open.Filter = "Ps2 SCEI Audio Pack(*.bhd;*.iec;*.psf2)|*.bhd;*.iec;*.psf2|Todos os arquivos(*.*)|*.*";
                    open.Title = "Escolha um arquivo de áudio SCEI(IECS) BHD";
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        #region Verificar abertos
                        if (sceifile != null|container != null)
                        {
                            Fechar();
                        }
                        #endregion
                        filename = open.FileName;
                        #region Abrir arquivo
                        sceifile = new IECS(File.ReadAllBytes(filename));
                        #endregion
                        #region Acionar Labels
                        filenamelabel.Text = "Nome: " + Path.GetFileName(filename);
                        verslabel.Text = "Pack Versão: " + sceifile.version.Versão.ToString();
                        vagnlabel.Text = "Número de Áudios: " + sceifile.vagi.VAGcount.ToString();
                        #endregion
                        #region Adicionar na lista
                        ListInsert();
                        #endregion
                    }
                    break;
            }
        }
        void AbrirContainer(bool isDrag)
        {
            switch (isDrag)
            {
                case true:
                    #region Verificar abertos
                    if (sceifile != null | container != null)
                    {
                        Fechar();
                    }
                    #endregion
                    fecharToolStripMenuItem1.Enabled = true;
                    #region Abrir arquivo
                    container = new BINContainer(File.ReadAllBytes(filename));
                    #endregion
                    #region Adicionar na lista de IECS
                    listadeIECS = new ListadeIECS(this);
                    listadeIECS.Show();
                    #endregion
                    break;
                case false:
                    var open = new OpenFileDialog();
                    open.Filter = "Container Binário(*.bin)|*.bin|Todos os arquivos(*.*)|*.*";
                    open.Title = "Escolha um Container BIN para abrir";
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        #region Verificar abertos
                        if(sceifile!=null|container!=null)
                        {
                            Fechar();
                        }
                        #endregion
                        filename = open.FileName;
                        fecharToolStripMenuItem1.Enabled = true;
                        #region Abrir arquivo
                        container = new BINContainer(File.ReadAllBytes(filename));
                        #endregion
                        #region Adicionar na lista de IECS
                        listadeIECS = new ListadeIECS(this);
                        listadeIECS.Show();
                        #endregion
                    }
                    break;
            }
        }
        private void abrirBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirContainer(false);
        }
        private void abrirIECToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Abrir(false);
        }
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (dropped.Length > 1)
            {
                MessageBox.Show("Apenas 1 por vez!!", "Nope", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                filename = dropped[0];
                if (Path.GetExtension(filename).ToLower() == ".bhd")
                {
                    Abrir(true);
                }
                else if(Path.GetExtension(filename).ToLower()==".bin")
                {
                    AbrirContainer(true);
                }

            }
        }
        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Fechar();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count>0)
            {
                extrairToolStripMenuItem.Enabled = true;
                importarVAGToolStripMenuItem.Enabled = true;
            }
        }

        private void extrairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Extrair();
        }

        private void extrairTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtrairTudo();
        }

        private void importarVAGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Importar();
        }

        private void extrairTudoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtrairContainer();
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salvar();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Visible = false;
            timer1.Stop();
            timer1.Enabled = false;
        }

        private void recriarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecriarContainer();
        }

        private void extrairuniccon_Click(object sender, EventArgs e)
        {
            ExtrairUN();
        }

        private void fecharToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listadeIECS.Close();
            container = null;
            extrairuniccon.Enabled = false;
            extrairTudoToolStripMenuItem.Enabled = false;
            fecharToolStripMenuItem1.Enabled = false;
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1(this);
            about.ShowDialog();
        }
    }
}
