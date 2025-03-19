using SCEIVag_Pack.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;


namespace SCEIVag_Pack
{
    public partial class Main : Form
    {
        public IECS sceifile;
        public BINContainer container;
        public ListadeIECS listadeIECS;
        public string caminhoELF;

        public static bool VAGEndianess_Big = true;

        public SoundPlayer player;

        public string filename;
        public bool reprodz = true;
        public Dictionary<string, string[]> FileList;
        public AddForm AddForm;
        public Main()
        {
            InitializeComponent();
        }
        #region Funções
        public Dictionary<string, string[]> GetEntries(bool wav, bool ext = true, string altext = "SYSTEM")
        {
            var entries = new Dictionary<string, string[]>();
            int ind = 0;
            foreach (var scei in container.sCEI_Entries.
                Where(x => x.Pack_Offset.ToString("X2") != "FFFFFFFF").ToArray())
            {
                string key = "IECS" + ind.ToString();
                var nameslist = new List<string>();

                int vagc = 0;
                foreach (var entry in scei.scei_File._Program.Entries)
                {
                    string vagname = "S_FOLDER_" + vagc.ToString();
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
            var file = new FileStream(filelistpath, FileMode.Open);
            reader.Load(file); //Carregando o arquivo

            XmlNodeList xnList = reader.GetElementsByTagName("Container");

            foreach (XmlNode xn in xnList)
            {
                string[] files = xn.InnerText.Split(new string[] { "\r\n", "" }, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(xn.Attributes["name"].Value, files);
            }
            #endregion  
            file.Close();
            return dict;
        }
        public SCEINode GetSelected() => treeView1.SelectedNode as SCEINode;
        public Stream GetWav()
        {
            var node = GetSelected();
            byte[] vag = node.Information.VAG;
            byte[] PCM = ADPCM.ToPCMMono(vag, vag.Length);
            byte[] wav = ADPCM.PCMtoWAV(PCM, node.Information._hz, 1);
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
                XmlElement pasta = creator.CreateElement("Container");
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
            scei_layout.Visible = !scei_layout.Visible;
            salvarToolStripMenuItem.Enabled = !salvarToolStripMenuItem.Enabled;
            salvarComoToolStripMenuItem.Enabled = !salvarComoToolStripMenuItem.Enabled;
            fecharToolStripMenuItem.Enabled = !fecharToolStripMenuItem.Enabled;
            extrairTodosToolStripMenuItem.Enabled = !extrairTodosToolStripMenuItem.Enabled;
            treevmenuitem.Enabled = !treevmenuitem.Enabled;
        }
        public void TreePopulate()
        {
            int i = 0, f = 0;
            foreach (var vgs in sceifile._Program.Entries)
            {

                var item = new SCEINode();
                item.Entry = vgs;
                if (vgs.CreatedNow)
                    item.ForeColor = Color.Red;

                item.Text = $"Folder_{f}_Scei";

                if (FileList != null)
                    item.Text = FileList.ElementAt(Convert.ToInt32(listadeIECS.listView1.SelectedItems[0].SubItems[0].Text)).Value[f];

                i = 0;
                if (vgs.Extent_Count > 0)
                    foreach (var extent in vgs.Extents)
                    {
                        var subitem = new SCEINode();
                        string vagname = "Audio Stream_" + i.ToString();

                        int ssetid = extent.Index;
                        int sampleid = sceifile._SampleSets.SampleSets[ssetid].SampleID;
                        int infoid = sceifile._Samples.VAG_Samples[sampleid].VAG_Id;

                        subitem.Information = sceifile._Infos.VAG_Infos[infoid];
                        subitem.Entry = vgs;
                        subitem.Sample = sceifile._Samples.VAG_Samples[sampleid];
                        subitem.SampleSet = sceifile._SampleSets.SampleSets[ssetid];

                        subitem.Text = vagname;
                        item.Nodes.Add(subitem);
                        i++;
                    }
                if (item.Nodes.Count > 1)
                    item.Expand();
                treeView1.Nodes.Add(item);
                f++;
            }
            if (!scei_layout.Visible)
                ShowHide();
        }
        public void Fechar()
        {
            sceifile = null;
            container = null;
            extrairToolStripMenuItem.Enabled = false;
            importarVAGToolStripMenuItem.Enabled = false;
            extrairTudoToolStripMenuItem.Enabled = false;
            fecharToolStripMenuItem1.Enabled = false;
            extrairuniccon.Enabled = false;
            treeView1.Nodes.Clear();

            if (listadeIECS != null && listadeIECS.Visible)
                listadeIECS.Close();

            if (player != null)
                player.Stop();

            if (scei_layout.Visible)
                ShowHide();
        }
        public void Extrair()
        {
            var node = GetSelected();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Wave Audio(*.wav)|*.wav|PS2 ADPCM VAG(*.vag)|*.vag";
            save.Title = "Escolha onde salvar o áudio VAG selecionado";
            var file = node.Parent.Text + "_" + node.Index;
            save.FileName = file;
            if (save.ShowDialog() == DialogResult.OK)
            {
                byte[] VAG = node.Information.VAG;
                if (save.FilterIndex == 1)
                {
                    File.WriteAllBytes(save.FileName, ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), node.Information._hz, 1));
                }
                else
                {
                    var VAGbits = new List<byte>();
                    var VAGStream = new List<byte>();

                    if (VAGEndianess_Big)
                    {
                        VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20).Reverse().ToArray());
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)node.Information.VAG.Length).Reverse().ToArray());
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)node.Information._hz).Reverse().ToArray());
                    }
                    else
                    {
                        VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)node.Information.VAG.Length));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)node.Information._hz));
                    }
                    VAGbits.AddRange(BitConverter.GetBytes((UInt64)0));
                    VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                    var name = new byte[0x10];
                    VAGbits.AddRange(name);

                    VAGStream.AddRange(VAG);

                    while (VAGStream.Count < node.Information.VAG.Length)
                        VAGStream.Add(0);

                    VAGbits.AddRange(VAGStream.ToArray());

                    File.WriteAllBytes(save.FileName, VAGbits.ToArray());
                }
                MessageBox.Show("Concluído!\nExtraído para: " + Path.GetDirectoryName(save.FileName) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void Importar()
        {
            SCEINode node = GetSelected();
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
                    {
                        if (Encoding.Default.GetString(replace.ReadBytes(0, 3)) == "VAG")
                        {
                            int orig_Freq = node.Information._hz;
                            int Freq = (int)replace.ReadUInt(0x10, 32, VAGEndianess_Big);
                            freq = Freq;
                            freqdif = orig_Freq != Freq;
                            if (freqdif)
                            {
                                MessageBox.Show("A frequência do áudio importado é: " + Freq.ToString() + "Hz"
                                    + "\nSó que o áudio original tem frequência de: " + orig_Freq.ToString() + "Hz", "Opa, pera aí!");
                                container.sCEI_Entries[listadeIECS.listView1.SelectedIndices[0]].scei_File._Infos.VAG_Infos[
                                    container.sCEI_Entries[listadeIECS.listView1.SelectedIndices[0]].scei_File._Infos.VAG_Infos.ToList().IndexOf(node.Information)]
                                    ._hz = (ushort)Freq;
                            }
                            replace = replace.ReadBytes(0x30, replace.Length - 0x30);
                        }
                    }
                    #region Wave Format Parse
                    if (opn.FilterIndex == 1)
                    {
                        //replace = PS2VagTool.Vag_Functions.SonyVag.Encode(replace, false);
                    }
                    #endregion

                    #endregion
                    node.Information.VAG = replace;

                    container.sCEI_Entries[listadeIECS.listView1.SelectedIndices[0]].scei_File.RebuildIECS();
                    container.Rebuild();
                    node.ForeColor = Color.Red;
                    listadeIECS.listView1.SelectedItems[0].ForeColor = Color.Red;
                    MessageBox.Show("Importado no container!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    #region Verificar VAG por header
                    byte[] replace = File.ReadAllBytes(opn.FileName);
                    if (opn.FilterIndex == 2)
                        if (Encoding.Default.GetString(replace.ReadBytes(0, 3)) == "VAG")
                        {
                            replace = replace.ReadBytes(0x30, replace.Length - 0x30);
                        }
                    #region Wave Format Parse
                    if (opn.FilterIndex == 1)
                    {
                        //uint pcmsize = (uint)ReadUInt(replace, 0x2a, Int.UInt32);
                        //byte[] pcmdata = ReadBlock(replace, 0x2e, pcmsize);

                        replace = ADPCM.FromPCMMono(ADPCM.WAVtoPCM(replace, node.Information._hz, out freqdif, out freq));
                    }
                    #endregion
                    #endregion
                    node.Information.VAG = replace;
                    if (freqdif)
                        node.Information._hz = (ushort)freq;
                    sceifile.RebuildIECS();
                    MessageBox.Show("Importado!\nNão se esqueça de salvar!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public void Salvar()
        {
            int i = 0, f = 0;
            foreach (var vgs in sceifile._Program.Entries)
            {
                vgs.CreatedNow = false;
                i = 0;
                if (vgs.Extent_Count > 0)
                    foreach (var extent in vgs.Extents)
                    {
                        var subitem = new SCEINode();
                        string vagname = "Audio Stream_" + i.ToString();


                        int ssetid = extent.Index;
                        int sampleid = sceifile._SampleSets.SampleSets[ssetid].SampleID;
                        int infoid = sceifile._Samples.VAG_Samples[sampleid].VAG_Id;

                        subitem.Information = sceifile._Infos.VAG_Infos[infoid];
                        subitem.Entry = vgs;
                        subitem.Sample = sceifile._Samples.VAG_Samples[sampleid];
                        subitem.SampleSet = sceifile._SampleSets.SampleSets[ssetid];

                        i++;
                    }

                f++;
            }
            if (container != null)
            {
                container.Rebuild();

                File.WriteAllBytes(filename, container.Container);
                File.WriteAllBytes(container.caminhoELF, container.linkedELF.GetEdited());
                container.linkedELF.MergeXML(FileList);
            }
            else
            {
                File.WriteAllBytes(filename, sceifile.Input);
            }

            //Efeito e texto
            MessageBox.Show("Arquivo salvo!");
            RefreshFile();

        }
        void SalvarComo()
        {
            var save = new SaveFileDialog();
            save.FileName = Path.GetFileNameWithoutExtension(filename);
            if (container != null)
                save.Filter = "Container SNDATA(*.bin)|*.bin";
            else
                save.Filter = "IECS Audio(*.bhd;*,iecs)|*.bhd;*.iecs";
            if (save.ShowDialog() == DialogResult.OK)
            {

                if (container != null)
                {
                    container.Rebuild();
                    File.WriteAllBytes(save.FileName, container.Container);
                    //Save Elf
                    save.FileName = Path.GetFileNameWithoutExtension(container.caminhoELF);
                    save.Filter = "ExecutableandLinkableFormat(*.*)|*.*)";
                    if (save.ShowDialog() == DialogResult.OK)
                        File.WriteAllBytes(save.FileName, container.linkedELF.GetEdited());
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
            #region Abrir arquivo
            //string elfpath = container.caminhoELF;
            //container = new BINContainer(File.ReadAllBytes(filename),
            //File.ReadAllBytes(elfpath), elfpath);
            //if (container.SCEI_Names != null && container.SCEI_Names.Count > 0)
            //    FileList = container.SCEI_Names;
            #endregion

            treeView1.Nodes.Clear();
            TreePopulate();
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
                int Entryindex = Convert.ToInt32(listadeIECS.listView1.Items[index].SubItems[0].Text);
                byte[] BHD = container.Container.ReadBytes(container.sCEI_Entries[Entryindex].Pack_Offset,
                    container.sCEI_Entries[Entryindex].Pack_Size);
                File.WriteAllBytes(save.FileName, BHD);
                MessageBox.Show("Concluído!\nExtraído para: " + Path.GetDirectoryName(save.FileName) + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void ExtrairTudo()
        {
            SCEINode node = GetSelected();
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
                foreach (var vag in sceifile._Infos.VAG_Infos)
                {


                    byte[] VAG = vag.VAG;

                    string name = node.Text.Substring(0, node.Text.Length - 4);

                    var VAGbits = new List<byte>();
                    var VAGStream = new List<byte>();

                    if (VAGEndianess_Big)
                    {
                        VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20).Reverse().ToArray());
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag.VAG.Length).Reverse().ToArray());
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag._hz).Reverse().ToArray());
                    }
                    else
                    {
                        VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag.VAG.Length));
                        VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag._hz));
                    }
                    VAGbits.AddRange(BitConverter.GetBytes((UInt64)0));
                    VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                    var namex = new byte[0x10];
                    VAGbits.AddRange(namex);

                    VAGStream.AddRange(VAG);

                    while (VAGStream.Count < node.Information.VAG.Length)
                        VAGStream.Add(0);

                    VAGbits.AddRange(VAGStream.ToArray());

                    if (exportwav)
                    {
                        File.WriteAllBytes(folder + @"\" + name + ".wav", ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag._hz, 1));
                    }
                    else
                    {
                        File.WriteAllBytes(folder + @"\" + name + ".vag", VAGbits.ToArray());
                    }

                    i++;
                }
                int index = listadeIECS.listView1.SelectedIndices[0];
                int Entryindex = Convert.ToInt32(listadeIECS.listView1.Items[index].SubItems[0].Text);
                byte[] IEC = container.Container.ReadBytes(container.sCEI_Entries[Entryindex].Pack_Offset,
                    container.sCEI_Entries[Entryindex].Pack_Size);
                string saveiec = folder + @"\" + "IECS" + i.ToString() + ".bhd";
                File.WriteAllBytes(saveiec, IEC);

                MessageBox.Show("Concluído!\nExtraído para: " + folder + @"\", "Extração", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        public void ExtrairContainer()
        {
            SCEINode node = GetSelected();
            bool exportflist = false;
            if (MessageBox.Show("Quer exportar apenas a FileList original?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                exportflist = true;
            if (exportflist == true)
            {
                var save = new SaveFileDialog();
                save.Filter = "ExtensibleMarkupLanguage(*.xml)|*.xml";
                save.FileName = "filelist";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    #region Escrever Lista de Arquivos
                    WriteToXML(GetEntries(false, false), save.FileName);
                    #endregion
                }
            }
            else
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
                        WriteToXML(GetEntries(exportwav, false), savepath + @"\filelist.xml");
                    #endregion

                    var entriesScei = container.sCEI_Entries.Where(x => x.Pack_Offset.ToString("X2") != "FFFFFFFF").ToArray();
                    for (int i = 0; i < entriesScei.Length; i++)
                    {
                        string path = savepath + @"\IECS" + i.ToString() + @"\";

                        if (FileList != null)
                            path = savepath + @"\" + FileList.ElementAt(i).Key + @"\";

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        byte[] IEC = container.Container.ReadBytes(entriesScei[i].Pack_Offset,
                            entriesScei[i].Pack_Size);

                        string saveiec = path + "IECS" + i.ToString() + ".bhd";
                        if (FileList != null)
                            saveiec = path + FileList.ElementAt(i).Key + ".bhd";

                        File.WriteAllBytes(saveiec, IEC);
                        #region VAG Export
                        IECS filescei = new IECS(IEC);
                        int c = 0;
                        foreach (var vag in filescei._Infos.VAG_Infos)
                        {
                            byte[] VAG = vag.VAG;

                            string savepathva = path + "stream" + c.ToString();

                            var VAGbits = new List<byte>();
                            var VAGStream = new List<byte>();

                            if (VAGEndianess_Big)
                            {
                                VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20).Reverse().ToArray());
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag.VAG.Length).Reverse().ToArray());
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag._hz).Reverse().ToArray());
                            }
                            else
                            {
                                VAGbits.AddRange(Encoding.Default.GetBytes("VAGp"));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)0x20));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag.VAG.Length));
                                VAGbits.AddRange(BitConverter.GetBytes((UInt32)vag._hz));
                            }
                            VAGbits.AddRange(BitConverter.GetBytes((UInt64)0));
                            VAGbits.AddRange(BitConverter.GetBytes((UInt32)0));
                            var name = new byte[0x10];
                            VAGbits.AddRange(name);

                            VAGStream.AddRange(VAG);

                            while (VAGStream.Count < node.Information.VAG.Length)
                                VAGStream.Add(0);

                            VAGbits.AddRange(VAGStream.ToArray());

                            if (FileList != null)
                                savepathva = path + FileList.ElementAt(i).Value[c];

                            if (FileList == null)
                            {
                                if (exportwav)
                                {
                                    File.WriteAllBytes(savepathva + ".wav", ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag._hz, 1));
                                }
                                else
                                {
                                    File.WriteAllBytes(savepathva + ".vag", VAGbits.ToArray());
                                }
                            }
                            else
                            {
                                if (Path.GetExtension(FileList.ElementAt(i).Value[c]) == ".wav")
                                    File.WriteAllBytes(savepathva, ADPCM.PCMtoWAV(ADPCM.ToPCMMono(VAG, VAG.Length), vag._hz, 1));
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
                        foreach (var vag in intern._Infos.VAG_Infos)
                        {
                            byte[] sounddata = File.ReadAllBytes(open + entries.ElementAt(f).Value[v]);

                            if (Path.GetExtension(entries.ElementAt(f).Value[v]) == ".wav")
                            {
                                sounddata = ADPCM.FromPCMMono(ADPCM.WAVtoPCM(sounddata, vag._hz, out freqd, out freq));
                            }
                            else
                            {
                                if (Encoding.Default.GetString(sounddata.ReadBytes(0, 3)) == "VAG")
                                {
                                    freq = (int)sounddata.ReadUInt(0x10, 32, VAGEndianess_Big);
                                    sounddata = sounddata.ReadBytes(0x30,
                                        sounddata.Length - 0x30);
                                }
                            }
                            if (freqd)
                                vag._hz = (ushort)freq;
                            vag.VAG = sounddata;
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
        void ShowHideAnim()
        {
            animpanel.Visible = !animpanel.Visible;
        }
        async void Abrir(bool isDrag)
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
                    linkLabel1.Text = Path.GetFileName(filename);
                    #endregion
                    #region Adicionar na Árvore
                    TreePopulate();
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
                        linkLabel1.Text = Path.GetFileName(filename);
                        #endregion
                        #region Adicionar na Árvore
                        TreePopulate();
                        #endregion
                    }
                    break;
            }
        }
        async void AbrirContainer(bool isDrag, string elfpath = "none")
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
                    if (filename != null)
                    {
                        #region Abrir

                        if (elfpath == "none")
                        {
                            var openELF = new OpenFileDialog();
                            openELF.Filter = "Todos os arquivos(*.*)|*.*";
                            openELF.Title = "Escolha um arquivo ELF";
                            if (openELF.ShowDialog() == DialogResult.OK)
                            {
                                fecharToolStripMenuItem1.Enabled = true;
                                ShowHideAnim();
                                await Task.Run(() =>
                                {
                                    Cursor.Current = Cursors.WaitCursor;
                                    #region Abrir arquivo
                                    container = new BINContainer(File.ReadAllBytes(filename),
                                    File.ReadAllBytes(openELF.FileName), openELF.FileName);
                                    container.caminhoELF = openELF.FileName;
                                    if (container.SCEI_Names != null && container.SCEI_Names.Count > 0)
                                        FileList = container.SCEI_Names;
                                    #endregion
                                    #region Adicionar na lista de IECS
                                    listadeIECS = new ListadeIECS(this);
                                });
                                ShowHideAnim();
                                Cursor.Current = Cursors.Arrow;
                                listadeIECS.Show();
                                #endregion
                            }
                        }
                        else
                        {
                            fecharToolStripMenuItem1.Enabled = true;
                            ShowHideAnim();
                            await Task.Run(() =>
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                #region Abrir arquivo
                                container = new BINContainer(File.ReadAllBytes(filename),
                                File.ReadAllBytes(elfpath), elfpath);
                                container.caminhoELF = elfpath;
                                if (container.SCEI_Names != null && container.SCEI_Names.Count > 0)
                                    FileList = container.SCEI_Names;
                                #endregion
                                #region Adicionar na lista de IECS
                                listadeIECS = new ListadeIECS(this);
                            });
                            ShowHideAnim();
                            Cursor.Current = Cursors.Arrow;
                            listadeIECS.Show();
                            #endregion
                        }
                        #endregion
                    }
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
                        #endregion
                        #region Abrir
                        var openELF = new OpenFileDialog();
                        openELF.Filter = "Todos os arquivos(*.*)|*.*";
                        openELF.Title = "Escolha um arquivo ELF";
                        if (openELF.ShowDialog() == DialogResult.OK)
                        {
                            filename = open.FileName;
                            fecharToolStripMenuItem1.Enabled = true;
                            ShowHideAnim();

                            await Task.Run(() =>
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                #region Abrir arquivo
                                container = new BINContainer(File.ReadAllBytes(filename),
                                File.ReadAllBytes(openELF.FileName), openELF.FileName);
                                container.caminhoELF = openELF.FileName;
                                if (container.SCEI_Names != null && container.SCEI_Names.Count > 0)
                                    FileList = container.SCEI_Names;
                                #endregion
                                #region Adicionar na lista de IECS
                                listadeIECS = new ListadeIECS(this);
                            });
                            listadeIECS.Show();
                            ShowHideAnim();
                            Cursor.Current = Cursors.Arrow;
                            #endregion
                        }
                        #endregion
                    }
                    break;
            }
        }

        #endregion

        #region Botões e Eventos
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(filename));
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
                if (dropped.Any(x => Path.GetExtension(x).ToLower() == ".bin"))
                {
                    filename = dropped.Where(x => Path.GetExtension(x).ToLower() == ".bin").ToArray()[0];

                    AbrirContainer(true, dropped.Where(x => Path.GetExtension(x).ToLower() != ".bin").ToArray()[0]);
                }
                else
                {
                    MessageBox.Show("Apenas 1 por vez!!", "Nope", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
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
                if (container != null)
                    AbrirContainer(true);
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
                if (treeView1.SelectedNode.Level == 1)
                {
                    extrairToolStripMenuItem.Enabled = true;
                    importarVAGToolStripMenuItem.Enabled = true;
                    if (reprodz)
                    {
                        if (player != null)
                        {
                            player.Stop();
                            player = null;
                        }
                        player = new SoundPlayer(GetWav());
                        if (GetSelected().Information.Loop == false)
                            player.Play();
                        else
                            player.PlayLooping();
                    }
                    propertyGrid1.SelectedObject = GetSelected().Information;
                }
                else
                {
                    extrairToolStripMenuItem.Enabled = false;
                    importarVAGToolStripMenuItem.Enabled = false;
                }

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

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 1)
                importarVAGToolStripMenuItem.PerformClick();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (treeView1.SelectedNode.Level == 1)
                    ctx_Iecs.Show(MousePosition);
            }
        }
        private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            littleEndianToolStripMenuItem.Checked = false;
            bigEndianToolStripMenuItem.Checked = true;
            VAGEndianess_Big = true;
        }

        private void littleEndianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            littleEndianToolStripMenuItem.Checked = true;
            bigEndianToolStripMenuItem.Checked = false;
            VAGEndianess_Big = false;
        }

        private void Expand_Clicl(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }
        private void Collapse_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddForm = new AddForm(this);
            if (AddForm.ShowDialog() == DialogResult.OK)
                RefreshFile();
        }

        #endregion

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if(container!=null)
            //    container.linkedELF.MergeXML(FileList);

        }
    }
}
