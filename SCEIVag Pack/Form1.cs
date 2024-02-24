﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static SCEIVag_Pack.Bin;
using System.Xml;


namespace SCEIVag_Pack
{
    public partial class Form1 : Form
    {
        public IECS sceifile;
        public BINContainer container;
        public ListadeIECS listadeIECS;
        public Graphics paneSave;
        public string caminhoELF;

        public SoundPlayer player;

        public string filename;
        public bool reprodz = true;
        public Dictionary<string, string[]> FileList;
        public Form1()
        {
            InitializeComponent();
            //ReadXML(@"C:\Users\Raiden\Desktop\out\filelisttest.xml");
        }
        #region Funções
        public Dictionary<string, string[]> GetEntries(bool wav, bool ext = true, string altext = "SYSTEM")
        {
            var entries = new Dictionary<string, string[]>();
            int ind = 0;
            foreach (var scei in container.sceiFiles)
            {
                string key = "IECS" + ind.ToString();
                var nameslist = new List<string>();

                int vagc = 0;
                foreach (var vag in scei.vagi.Vdata)
                {
                    string vagname = "stream" + vagc.ToString();
                    if (ext)
                    {
                        if (altext == "SYSTEM")
                        {
                            if (wav)
                                vagname += ".wav";
                            else
                                vagname += ".vag";
                        }
                        else
                        {
                            vagname += altext;
                        }
                    }
                    nameslist.Add(vagname);
                    vagc++;
                }

                entries.Add(key, nameslist.ToArray());

                ind++;
            }

            return entries;
        }
        public Dictionary<string, string[]> ReadXML(string filelistpath)
        {
            var dict = new Dictionary<string, string[]>();
            #region Leitor XML
            XmlDocument reader = new XmlDocument();
            reader.Load(filelistpath); //Carregando o arquivo

            XmlNodeList xnList = reader.GetElementsByTagName("Pasta");

            foreach (XmlNode xn in xnList)
            {
                string[] files = xn.InnerText.Split(new string[] { "\r\n", "" }, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(xn.Attributes["name"].Value, files);
            }
            #endregion  
            return dict;
        }
        public Stream GetWav()
        {
            byte[] vag = ReadBlock(sceifile.StreamData, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamOffset, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamSize);
            byte[] PCM = ADPCM.ToPCMMono(vag, vag.Length);
            byte[] wav = ADPCM.PCMtoWAV(PCM, sceifile.vagi.Vdata[listView1.SelectedIndices[0]].Frequency, 1);
            var mem = new MemoryStream(wav);
            return mem;
        }
        public void WriteToXML(Dictionary<string, string[]> entries, string savexml)
        {
            XmlDocument creator = new XmlDocument();

            XmlElement element1 = creator.CreateElement("FILELIST");

            var list = new List<XmlNode>();
            foreach (var entry in entries)
            {
                XmlElement pasta = creator.CreateElement("Pasta");
                pasta.SetAttribute("name", entry.Key);
                string innertext = ArrayToString(entry.Value);
                XmlText filename = creator.CreateTextNode(innertext);
                pasta.AppendChild(filename);

                element1.AppendChild(pasta);
            }
            creator.AppendChild(element1);
            creator.Save(savexml);
        }
        public void ShowHide()
        {
            listView1.Visible = !listView1.Visible;
            salvarToolStripMenuItem.Enabled = !salvarToolStripMenuItem.Enabled;
            salvarComoToolStripMenuItem.Enabled = !salvarComoToolStripMenuItem.Enabled;
            fecharToolStripMenuItem.Enabled = !fecharToolStripMenuItem.Enabled;
            groupBox1.Visible = !groupBox1.Visible;
            extrairTodosToolStripMenuItem.Enabled = !extrairTodosToolStripMenuItem.Enabled;
            button1.Visible = !button1.Visible;
            button2.Visible = !button2.Visible;
        }
        public void ListInsert()
        {
            int i = 0;
            foreach (var vgs in sceifile.vagi.Vdata)
            {
                var item = new ListViewItem(i.ToString());
                string vagname = "stream" + i.ToString() + ".vag";

                if (FileList != null)
                    vagname = FileList.ElementAt(listadeIECS.listView1.SelectedItems[0].Index).Value[i];

                item.SubItems.Add(vagname);
                item.SubItems.Add(Convert.ToString(vgs.Frequency) + "Hz");
                listView1.Items.Add(item);
                i++;
            }
            if(!listView1.Visible)
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
            if (listView1.Visible)
                ShowHide();
        }
        public void Extrair()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Wave Audio(*.wav)|*.wav|PS2 ADPCM VAG(*.vag)|*.vag";
            save.Title = "Escolha onde salvar o áudio VAG selecionado";
            save.FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].SubItems[1].Text);
            if (save.ShowDialog() == DialogResult.OK)
            {
                byte[] VAG = ReadBlock(sceifile.StreamData, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamOffset, (uint)sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamSize);
                if (save.FilterIndex == 1)
                {
                    File.WriteAllBytes(save.FileName, ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), sceifile.vagi.Vdata[listView1.SelectedIndices[0]].Frequency, 1));
                }
                else
                {
                    File.WriteAllBytes(save.FileName, VAG);
                }
                MessageBox.Show("Concluído!\nExtraído para: " + Path.GetDirectoryName(save.FileName) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void Importar()
        {
            bool freqdif = false;
            int freq = 0;
            OpenFileDialog opn = new OpenFileDialog();
            opn.Title = "Selecione o áudio VAG para importar e sobrescrever";
            opn.Filter = "Wave Audio(*.wav)|*.wav|PS2 Vag Audio(*.vag)|*.vag";
            opn.FilterIndex = 2;
            if (opn.ShowDialog() == DialogResult.OK)
            {
                if (container != null)
                {
                    #region Verificar VAG por header
                    byte[] replace = File.ReadAllBytes(opn.FileName);
                    if (opn.FilterIndex == 2)
                        if (Encoding.Default.GetString(ReadBlock(replace, 0, 3)) == "VAG")
                        {
                            replace = ReadBlock(replace, 0x30, (uint)replace.Length - 0x30);
                        }
                    #region Wave Format Parse
                    if (opn.FilterIndex == 1)
                    {
                        replace = ADPCM.FromPCMMono(ADPCM.WAVtoPCM(replace, container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].vagi.Vdata[listView1.SelectedIndices[0]].Frequency, out freqdif, out freq));
                    }
                    #endregion
                    #endregion
                    container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].vagi.Vdata[listView1.SelectedIndices[0]].StreamVAG = replace;
                    if (freqdif)
                        container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].vagi.Vdata[listView1.SelectedIndices[0]].Frequency = freq;
                    container.sceiFiles[listadeIECS.listView1.SelectedIndices[0]].RebuildIECS();
                    container.Rebuild();
                    listView1.SelectedItems[0].ForeColor = Color.Red;
                    listadeIECS.listView1.SelectedItems[0].ForeColor = Color.Red;
                    listView1.SelectedItems[0].SubItems[2].Text = freq.ToString() + "Hz";
                    MessageBox.Show("Importado no container!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    #region Verificar VAG por header
                    byte[] replace = File.ReadAllBytes(opn.FileName);
                    if (opn.FilterIndex == 2)
                        if (Encoding.Default.GetString(ReadBlock(replace, 0, 3)) == "VAG")
                        {
                            replace = ReadBlock(replace, 0x30, (uint)replace.Length - 0x30);
                        }
                    #region Wave Format Parse
                    if (opn.FilterIndex == 1)
                    {
                        //uint pcmsize = (uint)ReadUInt(replace, 0x2a, Int.UInt32);
                        //byte[] pcmdata = ReadBlock(replace, 0x2e, pcmsize);

                        replace = ADPCM.FromPCMMono(ADPCM.WAVtoPCM(replace, sceifile.vagi.Vdata[listView1.SelectedIndices[0]].Frequency, out freqdif, out freq));
                    }
                    #endregion
                    #endregion
                    sceifile.vagi.Vdata[listView1.SelectedIndices[0]].StreamVAG = replace;
                    sceifile.RebuildIECS();
                    listView1.SelectedItems[0].ForeColor = Color.Red;
                    listView1.SelectedItems[0].SubItems[2].Text = freq.ToString() + "Hz";
                    MessageBox.Show("Importado!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public void Salvar()
        {
            {
                if (container != null)
                {
                    container.Rebuild();
                    container.SaveToELF();
                    File.WriteAllBytes(filename, container.Container);
                }
                else
                {
                    File.WriteAllBytes(filename, sceifile.Input);
                }

                //Efeito e texto
                paneSave = panel1.CreateGraphics();
                panel1.Visible = true;
                panel1.BringToFront();
                paneSave.Clear(Color.White);
                paneSave.DrawString("Arquivo salvo!", new Font(FontFamily.GenericSerif, 24f, FontStyle.Bold), Brushes.DarkGreen, 0, 0);
                timer1.Enabled = true;
                timer1.Start();
                RefreshFile();
            }
        }
        void SalvarComo()
        {
            var save = new SaveFileDialog();
            save.FileName = Path.GetFileNameWithoutExtension(filename);
            if (container != null)
                save.Filter = "Container SNDATA(*.bin)|*.bin";
            else
                save.Filter = "IECS Audio(*.bhd;*,iecs)|*.bhd;*.iecs";
            if(save.ShowDialog()==DialogResult.OK)
            {

                if (container != null)
                {
                    container.Rebuild();
                    File.WriteAllBytes(save.FileName, container.Container);
                }
                else
                {
                    File.WriteAllBytes(save.FileName, sceifile.Input);
                }
                MessageBox.Show("Salvo para: " + save.FileName, "Sucesso");
                RefreshFile();
            }
        }
        public void RefreshFile()
        {
            listView1.Items.Clear();
            ListInsert();
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
            fold.Description = "Escolha a pasta onde extrair todos os áudios:";
            if (fold.ShowDialog() == DialogResult.OK)
            {
                bool exportwav = false;

                if (MessageBox.Show("Quer exportar tudo em WAV?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    exportwav = true;
                #region Pasta Raiz
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
                #endregion

                int i = 0;
                foreach (var vag in sceifile.vagi.Vdata)
                {
                    

                    byte[] VAG = ReadBlock(sceifile.StreamData, (uint)vag.StreamOffset, (uint)vag.StreamSize);

                    string name = listView1.Items[i].SubItems[1].Text.Substring(0, listView1.Items[i].SubItems[1].Text.Length - 4);

                        if (exportwav)
                        {
                            File.WriteAllBytes(folder + @"\" + name+ ".wav", ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag.Frequency, 1));
                        }
                        else
                        {
                            File.WriteAllBytes(folder + @"\" + name + ".wav", VAG);
                        }

                    i++;
                }

                byte[] IEC = ReadBlock(container.Container, (uint)container.IECSoffsets[listadeIECS.listView1.SelectedIndices[0]], (uint)container.IECSsizes[listadeIECS.listView1.SelectedIndices[0]]);
                string saveiec = folder + @"\" + "IECS" + i.ToString() + ".bhd";
                File.WriteAllBytes(saveiec, IEC);

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
                bool exportwav = false;

                if (MessageBox.Show("Quer exportar tudo em WAV?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    exportwav = true;

                string savepath = fold.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(filename);
                if (!Directory.Exists(savepath))
                    Directory.CreateDirectory(savepath);

                #region Escrever Lista de Arquivos
                if (FileList == null)
                    WriteToXML(GetEntries(exportwav), savepath + @"\filelist.xml");
                #endregion

                for (int i = 0; i < container.fileCount; i++)
                {
                    string path = savepath + @"\IECS" + i.ToString() + @"\";

                    if (FileList != null)
                        path = savepath + @"\" + FileList.ElementAt(i).Key + @"\";

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    byte[] IEC = ReadBlock(container.Container, (uint)container.IECSoffsets[i], (uint)container.IECSsizes[i]);

                    string saveiec = path + "IECS" + i.ToString() + ".bhd";
                    if (FileList != null)
                        saveiec = path + FileList.ElementAt(i).Key + ".bhd";

                    File.WriteAllBytes(saveiec, IEC);
                    #region VAG Export
                    IECS filescei = new IECS(IEC);
                    int c = 0;
                    foreach (var vag in filescei.vagi.Vdata)
                    {
                        byte[] VAG = ReadBlock(filescei.StreamData, (uint)vag.StreamOffset, (uint)vag.StreamSize);

                        string savepathva = path + "stream" + c.ToString();

                        if (FileList != null)
                            savepathva = path + FileList.ElementAt(i).Value[c];

                        if (FileList == null)
                        {
                            if (exportwav)
                            {
                                File.WriteAllBytes(savepathva + ".wav", ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag.Frequency, 1));
                            }
                            else
                            {
                                File.WriteAllBytes(savepathva + ".vag", VAG);
                            }
                        }
                        else
                        {
                            if (Path.GetExtension(FileList.ElementAt(i).Value[c]) == ".wav")
                                File.WriteAllBytes(savepathva, ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag.Frequency, 1));
                            else
                                File.WriteAllBytes(savepathva, VAG);

                        }
                        c++;
                    }
                    #endregion

                }

                MessageBox.Show("Concluído!\nExtraído para: " + savepath + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public string ArrayToString(string[] entries)
        {
            string outs = "\r\n";
            foreach (string s in entries)
            {
                outs += s + "\r\n";
            }
            return outs;
        }
        public void RecriarContainer()
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            fold.ShowNewFolderButton = false;
            bool freqd = false;
            int freq = 0;
            fold.Description = "Escolha a pasta onde estão as pastas de arquivos VAG ou WAV";
            if (fold.ShowDialog() == DialogResult.OK)
            {
                if (Directory.EnumerateDirectories(fold.SelectedPath).Count() > 1)
                {
                    Dictionary<string, string[]> entries;
                    if (FileList == null)
                    {
                        entries = ReadXML(fold.SelectedPath + @"\filelist.xml");
                    }
                    else
                    {
                        entries = FileList;
                    }

                    string openp = fold.SelectedPath + @"\";
                    var container = new List<byte>();

                    for (int f = 0; f < Directory.EnumerateDirectories(fold.SelectedPath).Count(); f++)
                    {
                        string open = openp + entries.ElementAt(f).Key + @"\";
                        IECS intern = new IECS(File.ReadAllBytes(open + entries.ElementAt(f).Key + ".bhd"));

                        int v = 0;
                        foreach (var vag in intern.vagi.Vdata)
                        {
                            byte[] sounddata = File.ReadAllBytes(open + entries.ElementAt(f).Value[v]);

                            if (Path.GetExtension(entries.ElementAt(f).Value[v]) == ".wav")
                            {
                                sounddata = ADPCM.FromPCMMono(ADPCM.WAVtoPCM(sounddata,vag.Frequency,out freqd, out freq));
                            }
                            else
                            {
                                if (Encoding.Default.GetString(ReadBlock(sounddata, 0, 3)) == "VAG")
                                {
                                    sounddata = ReadBlock(sounddata, 0x30, (uint)sounddata.Length - 0x30);
                                }
                            }
                            if (freqd)
                                vag.Frequency = freq;
                            vag.StreamVAG = sounddata;
                            v++;
                        }

                        intern.RebuildIECS();
                        container.AddRange(intern.Input);

                        while (container.Count() % 0x800 != 0)
                            container.Add(0xFF);
                    }

                    #region Save Container
                    var saveCON = new SaveFileDialog();
                    saveCON.Filter = "Arquivo BIN(*.bin)|*.bin";
                    saveCON.FileName = fold.SelectedPath.Split(new string[] { Path.GetDirectoryName(fold.SelectedPath), @"\", "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    saveCON.Title = "Escolha onde salvar o novo Container";
                    if (saveCON.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveCON.FileName, container.ToArray());
                        MessageBox.Show("Concluído!\nRecriado para: " + Path.GetDirectoryName(fold.SelectedPath) + @"\" + Path.GetFileName(saveCON.FileName), "Extração", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    #endregion
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
            switch (isDrag)
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
                        if (sceifile != null | container != null)
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
                        if (sceifile != null | container != null)
                        {
                            Fechar();
                        }
                        var openELF = new OpenFileDialog();
                        openELF.Filter = "Todos os arquivos(*.*)|*.*";
                        openELF.Title = "Escolha um arquivo ELF";
                        if (openELF.ShowDialog() == DialogResult.OK)
                        {
                            #endregion
                            filename = open.FileName;
                            fecharToolStripMenuItem1.Enabled = true;
                            #region Abrir arquivo
                            container = new BINContainer(File.ReadAllBytes(filename));
                            container.caminhoELF = openELF.FileName;
                            #endregion
                            #region Adicionar na lista de IECS
                            listadeIECS = new ListadeIECS(this);
                            listadeIECS.Show();
                            #endregion
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Botões e Eventos
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
                else if (Path.GetExtension(filename).ToLower() == ".bin")
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
            if (listView1.SelectedItems.Count > 0)
            {
                extrairToolStripMenuItem.Enabled = true;
                importarVAGToolStripMenuItem.Enabled = true;
                button1.Enabled = true;
                if(reprodz)
                {
                    if(player!=null)
                    {
                        player.Stop();
                        player = null;
                    }
                    player = new SoundPlayer(GetWav());
                    player.Play();
                }
            }
            else
            {
                extrairToolStripMenuItem.Enabled = false;
                importarVAGToolStripMenuItem.Enabled = false;
                button1.Enabled = false;
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

        private void abrirListaDeArquivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog();
            op.Filter = "ExtensibleMarkupLanguage(*.xml)|*.xml";
            op.Title = "Abra a lista com os nomes de pastas e arquivos personalizada.";
            if (op.ShowDialog() == DialogResult.OK)
            {
                FileList = new Dictionary<string, string[]>();
                FileList = ReadXML(op.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            player = new SoundPlayer(GetWav());
            player.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.Stop();
                player = null;
            }
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarComo();
        }

        private void selecionarPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ativadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ativadoToolStripMenuItem.Checked = !ativadoToolStripMenuItem.Checked;
            reprodz = ativadoToolStripMenuItem.Checked;
        }

        #endregion
    }
}
