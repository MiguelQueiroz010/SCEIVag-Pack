using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static SCEIVag_Pack.Bin;
using System.Xml.Linq;
using static SCEIVag_Pack.IECS.Sset;
using System.Media;
using System.ComponentModel;
using static SCEIVag_Pack.Forms.AddForm.VAG_Info;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Security.AccessControl;

namespace SCEIVag_Pack
{
    public class IECS
    {
        public byte[] StreamData;
        public byte[] Input;

        public int Size;

        public Head _Header;
        public Version _Version;
        public VAGInfo _Infos;
        public VagSamples _Samples;
        public Sset _SampleSets;
        public Prog _Program;

        public List<int> ELFIECSunk;
        public List<int> ELFVAGS_Size;

        public IECS(byte[] input)
        {
            Input = input;
            _Version = new Version().Read(input.ReadBytes(0, 0x10))
                as Version;
            _Header = new Head().Read(input.ReadBytes(0x10, (int)input.ReadUInt(0x18, 32)))
                as Head;
            _Infos = new VAGInfo().Read(input.ReadBytes(_Header.VAGInfosSect_Offset,
                (int)input.ReadUInt(_Header.VAGInfosSect_Offset + 0x8, 32)))
                as VAGInfo;
            _Samples = new VagSamples().Read(input.ReadBytes(_Header.SampleSect_Offset,
                (int)input.ReadUInt(_Header.SampleSect_Offset + 0x8, 32)))
                as VagSamples;
            _SampleSets = new Sset().Read(input.ReadBytes((int)(_Header.SampleSect_Offset +
                _Samples.Size), (int)input.ReadUInt((int)(_Header.SampleSect_Offset +
                _Samples.Size + 0x8), 32)))
                as Sset;
            _Program = new Prog().Read(input.ReadBytes((int)(_Header.SampleSect_Offset +
                _Samples.Size + _SampleSets.Size), (int)input.ReadUInt((int)(_Header.SampleSect_Offset +
                _Samples.Size + _SampleSets.Size + 0x8), 32)))
                as Prog;

            int padding = (int)(_Header.SampleSect_Offset +
                _Samples.Size + _SampleSets.Size + _Program.Size);
            while (input[padding] % 0x800 != 0)
                padding++;

            StreamData = input.ReadBytes((int)(padding), _Header.VAGStream_Size);

            #region Read VAGs to Info
            var vag_sizes = new List<int>();
            for (int i = 0; i < _Infos.VAG_Count; i++)
            {
                int size = 0;
                if (i == _Infos.VAG_Count - 1)
                {
                    size = (int)(input.Length - _Infos.VAG_Infos[i].Stream_Offset);
                }
                else
                {
                    size = (int)(_Infos.VAG_Infos[i + 1].Stream_Offset - _Infos.VAG_Infos[i].Stream_Offset);
                }

                _Infos.VAG_Infos[i].VAG = StreamData.ReadBytes((int)_Infos.VAG_Infos[i].Stream_Offset,
                    size);
            }
            #endregion
        }
        public byte[] RebuildIECS()
        {
            var iecs = new List<byte>();
            #region Sections
            //Version
            byte[] _Vers = _Version.Rebuild();

            //Program
            byte[] _Prog = _Program.Rebuild();
            _Program.Size = (uint)_Prog.Length;

            //Sample Sets
            byte[] _sset = _SampleSets.Rebuild();
            _SampleSets.Size = (uint)_sset.Length;

            //Samples
            byte[] _samples = _Samples.Rebuild();
            _Samples.Size = (uint)_samples.Length;

            //Infos
            byte[] _infos = _Infos.Rebuild();
            _Infos.Size = (uint)_infos.Length;

            #endregion

            

            //Add all data together
            iecs.AddRange(_Version.Rebuild()); //Version
            var second_data = new List<byte>();

            second_data.AddRange(_infos); //Infos
            second_data.AddRange(_samples); //Samples
            second_data.AddRange(_sset); //Samples Sets
            second_data.AddRange(_Prog); //Program

            int totalsections_size = 0x50 + second_data.Count;
            _Header.SectionsSize = totalsections_size;
            _Header.VAGInfosSect_Offset = 0x50;

            _Header.SampleSect_Offset = (int)(_Header.Size + 0x10 + _infos.Length);
            _Header.PssetSect_Offset = _Header.SampleSect_Offset + _samples.Length;
            _Header.ProgSect_Offset = _Header.PssetSect_Offset + _sset.Length;

            #region Recompile VAG_Stream
            
            var vag_data = new List<byte>();
            foreach (var info in _Infos.VAG_Infos)
                vag_data.AddRange(info.VAG);
            int vagstream_length = vag_data.Count;
            #endregion

            _Header.VAGStream_Size = vagstream_length;
            

            iecs.AddRange(_Header.Rebuild());
            iecs.AddRange(second_data.ToArray());

            //0x800 Sector Padding
            while (iecs.Count % 0x800 != 0)
                iecs.Add(0xFF);

            iecs.AddRange(vag_data.ToArray());
            return iecs.ToArray();
        }
        public class Section
        {
            public enum Type : ulong
            {
                Head = 0x4865616453434549, //"IECSdaeH"
                Version = 0x5665727353434549, //"IECSsreV"
                VAGInfo = 0x5661676953434549, //"IECSigaV"
                Samples = 0x536D706C53434549, //"IECSlpmS"
                PSset = 0x5373657453434549, //"IECStesSP"
                Program = 0x50726F6753434549 //"IECSgorP"

            };

            public Type _Type;
            public uint Size;

            public virtual Section Read(byte[] data) => new Section()
            {
                _Type = (Type)data.ReadULong(0),
                Size = (uint)data.ReadUInt(8, 32)
            };
            public virtual byte[] Rebuild()
            {
                var out_data = new List<byte>();
                out_data.AddRange(BitConverter.GetBytes((long)_Type));
                out_data.AddRange(BitConverter.GetBytes((UInt32)Size));
                return out_data.ToArray();
            }
        }
        #region Sub-Seções
        public class Head : Section
        {
            public int SectionsSize;
            public int VAGStream_Size;

            public int ProgSect_Offset;
            public int PssetSect_Offset;

            public int SampleSect_Offset;
            public int VAGInfosSect_Offset;
            public override Section Read(byte[] secData) => new Head()
            {
                _Type = (Type)secData.ReadULong(0),
                Size = (uint)secData.ReadUInt(8, 32),
                SectionsSize = (int)ReadUInt(secData, 0xC, Int.UInt32),
                VAGStream_Size = (int)ReadUInt(secData, 0x10, Int.UInt32),
                ProgSect_Offset = (int)ReadUInt(secData, 0x14, Int.UInt32),
                PssetSect_Offset = (int)ReadUInt(secData, 0x18, Int.UInt32),
                SampleSect_Offset = (int)ReadUInt(secData, 0x1c, Int.UInt32),
                VAGInfosSect_Offset = (int)ReadUInt(secData, 0x20, Int.UInt32)
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)SectionsSize));
                out_data.AddRange(BitConverter.GetBytes((UInt32)VAGStream_Size));

                out_data.AddRange(BitConverter.GetBytes((UInt32)ProgSect_Offset));
                out_data.AddRange(BitConverter.GetBytes((UInt32)PssetSect_Offset));
                out_data.AddRange(BitConverter.GetBytes((UInt32)SampleSect_Offset));

                out_data.AddRange(BitConverter.GetBytes((UInt32)VAGInfosSect_Offset));

                while (out_data.Count() < this.Size)
                    out_data.Add(0xFF);
                return out_data.ToArray();
            }
        }
        public class Version : Section
        {
            public ushort _Unk, _Version;
            public override Section Read(byte[] secData) => new Version()
            {
                _Type = (Type)secData.ReadULong(0),
                Size = (uint)secData.ReadUInt(8, 32),
                _Unk = (ushort)ReadUInt(secData, 0xC, Int.UInt16),
                _Version = (ushort)ReadUInt(secData, 0xE, Int.UInt16)
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk));
                out_data.AddRange(BitConverter.GetBytes((UInt16)_Version));
                while (out_data.Count % 0x10 != 0)
                    out_data.Add(0xff);
                return out_data.ToArray();
            }
        }
        public class VAGInfo : Section
        {
            public class Information
            {
                public Stream GetWav()
                {
                    byte[] vag = VAG;
                    byte[] PCM = ADPCM.ToPCMMono(vag, vag.Length);
                    byte[] wav = ADPCM.PCMtoWAV(PCM, _hz, 1);
                    var mem = new MemoryStream(wav);
                    return mem;
                }
                public class SoundPlayerEditor : UITypeEditor
                {
                    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
                    {
                        return UITypeEditorEditStyle.DropDown; // Exibe um menu suspenso dentro da PropertyGrid
                    }

                    public override void PaintValue(PaintValueEventArgs e)
                    {
                        // Desenha um pequeno ícone de play na célula da PropertyGrid
                        e.Graphics.DrawString("▶", new Font("Arial", 10), System.Drawing.Brushes.Black, e.Bounds);
                    }

                    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
                    {
                        // Método chamado ao clicar na propriedade dentro da PropertyGrid
                        if (context?.Instance is Information information && information.VAG != null)
                        {
                            using (Stream ms = information.GetWav())
                            {
                                SoundPlayer player = new SoundPlayer(ms);
                                if(information.Loop)
                                    player.PlayLooping();
                                else
                                    player.Play();
                            }
                        }
                        return value;
                    }
                }
                public class ButtonEditor : UITypeEditor
                {
                    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
                    {
                        return UITypeEditorEditStyle.Modal; // Define que o editor será modal (abre uma janela ao clicar)
                    }

                    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
                    {
                        if (context?.Instance is Information info) // Obtém a instância da classe
                        {
                            OpenFileDialog opn = new OpenFileDialog();
                            opn.Title = "Selecione o áudio VAG para importar e sobrescrever";
                            opn.Filter = "Wave Audio(*.wav)|*.wav|PS2 Vag Audio(*.vag)|*.vag";
                            opn.FilterIndex = 2;
                            if (opn.ShowDialog() == DialogResult.OK)
                            {
                                #region Verificar VAG por header
                                byte[] replace = File.ReadAllBytes(opn.FileName);
                                if (opn.FilterIndex == 2)
                                {
                                    if (Encoding.Default.GetString(replace.ReadBytes(0, 3)) == "VAG")
                                    {
                                        int Freq = (int)replace.ReadUInt(0x10, 32, Main.VAGEndianess_Big);
                                        info._hz = (ushort)Freq;
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
                                info.VAG = replace;
                                MessageBox.Show($"Imported audio from:\n{opn.FileName}!", "Action");
                            }
                            return value; // Retorna o valor original (ou alterado, se necessário)
                        }
                        else
                            return value;
                    }
                }

                private string _meuValor = "Import/Add Audio VAG...";
                [Editor(typeof(ButtonEditor), typeof(UITypeEditor))]
                public string ImportOption
                {
                    get { return _meuValor; }
                    set { _meuValor = value; }
                }

                [Editor(typeof(SoundPlayerEditor), typeof(UITypeEditor))]
                [Category("Audio")]
                [Description("Click")]
                public string PlayAudio { get; set; } = "▶ Play Audio";

                public uint Stream_Offset;

                public ushort _hz;
                [ReadOnly(true)] 
                [Category("Audio")]
                [Description("Shows the audio frequency in Hertz.")]
                public string Frequency
                {
                    get => Convert.ToString(_hz)+"Hz";
                    set => _hz = Convert.ToUInt16(value.Substring(0, value.Length-2));
                }
                
                public bool _Loop;
                [Category("Audio")]
                [RefreshProperties(RefreshProperties.All)]
                [ReadOnly(false)]
                [Description("Sets if the audio will loop infinitely or not.\nTrue | False")]
                public bool Loop {
                    get => _Loop;
                    set => _Loop = value;
                }



                const byte Secure_EndSignal = 0xFF;
                //ENDSIGNAL = 0xFF +1 Byte

                public byte[] VAG;

                public static Information Read(byte[] data) => new Information()
                {
                    Stream_Offset = (uint)data.ReadUInt(0, 32),
                    _hz = (ushort)data.ReadUInt(4, 16),
                    _Loop = Convert.ToBoolean(data[6])
                };
                public byte[] Rebuild(uint StreamOffset)
                {
                    Stream_Offset = StreamOffset;
                    var data_out = new List<byte>();
                    data_out.AddRange(BitConverter.GetBytes((UInt32)Stream_Offset));
                    data_out.AddRange(BitConverter.GetBytes((UInt16)_hz));
                    data_out.Add(Convert.ToByte(_Loop));
                    data_out.Add(Secure_EndSignal);
                    return data_out.ToArray();
                }
            }

            public uint VAG_Count;
            public Information[] VAG_Infos;

            public override Section Read(byte[] data) => new VAGInfo()
            {
                _Type = (Type)data.ReadULong(0),
                Size = (uint)data.ReadUInt(8, 32),
                VAG_Count = data.ReadUInt(0xC, 32) + 1,

                VAG_Infos = Enumerable.Range(0, (int)(data.ReadUInt(0xC, 32) + 1)).Select(
                    x => Information.Read(data.ReadBytes((int)data.ReadUInt(0x10 + (x * 4), 32), 8))
                    ).ToArray()
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();

                VAG_Count = (uint)VAG_Infos.Length;
                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                uint offset = 0;
                foreach(var info in VAG_Infos)
                {
                    if (info.VAG != null && info.VAG.Length > 0)
                    {
                        infos_data.AddRange(info.Rebuild(offset).ToArray());
                        offset += (uint)info.VAG.Length;
                    }
                }
                Size = (uint)(0x10 + (VAG_Count * 4) + infos_data.Count);
                while (Size % 0x10 != 0)
                    Size++;
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)VAG_Count - 1));
                //Create pointers
                out_data.AddRange(Enumerable.Range(0, (int)VAG_Count)
                    .SelectMany(
                    x => BitConverter.GetBytes((UInt32)((x * 8) + ((VAG_Count * 4) + 0x10)))

                    ).ToArray()
                    );

                //Add the Info
                out_data.AddRange(infos_data.ToArray());

                while (out_data.Count % 0x10 != 0)
                    out_data.Add(0xff);
                #endregion

                return out_data.ToArray();
            }

        }
        public class VagSamples : Section
        {
            public void Add(Sample entry)
            {
                var list = VAG_Samples.ToList();
                list.Add(entry);
                entry.CreatedNow = true;
                VAG_Samples = list.ToArray();
                Samples_Count = VAG_Samples.Length;
            }
            public void AddRange(Sample[] entries)
            {
                var list = VAG_Samples.ToList();
                list.AddRange(entries);
                VAG_Samples = list.ToArray();
                Samples_Count = VAG_Samples.Length;
            }

            public int Samples_Count;
            public Sample[] VAG_Samples;

            public class Sample
            {
                public bool CreatedNow = false;

                [Category("SampleSetting")]
                public bool Empty { get => isEmpty; set => isEmpty = value; }

                public bool isEmpty;
                public enum Audio_Type
                {
                    None = 0,
                    Left = 1,
                    Right = 2,
                    Both = 3
                };

                public UInt16 VAG_Id;
                public byte Volume_Id;
                public byte Audio_Speed;
                public byte Audio_Stereo_ID;
                public byte Volume;
                public byte Max_Volume;
                public Audio_Type _Audio_Type;
                public byte[] ExtraData;

                [Category("Audio Options")]
                [Description("Speed of the audio track.")]
                public byte Speed
                {
                    get => Audio_Speed;
                    set => Audio_Speed = value;
                }
                [Category("Audio Options")]
                [Description("Channel type of audio.\n Left | Right | Both | None")]
                public Audio_Type Channels
                {
                    get => _Audio_Type;
                    set => _Audio_Type = value;
                }

                [Category("Audio Options")]
                [DisplayName("Volume")]
                [Description("Volume of the audio track.")]
                public byte Vol
                {
                    get => Volume;
                    set => Volume = value;
                }
                [Category("Audio Options")]
                [Description("Maximum Volume of the audio track.")]
                public byte MaxVolume
                {
                    get => Max_Volume;
                    set => Max_Volume = value;
                }
                public Sample(bool empty) => isEmpty = empty;
                
                public static Sample ReadSample(byte[] data)
                {
                    var sampl = new Sample(false);
                    sampl.VAG_Id = (ushort)data.ReadUInt(0, 16);
                    sampl.Volume_Id = data[2];
                    sampl.Audio_Speed = data[3];
                    sampl.Audio_Stereo_ID = data[4];
                    sampl.Volume = data[5];
                    sampl.Max_Volume = data[6];
                    sampl._Audio_Type = (Audio_Type)data[7];
                    sampl.ExtraData = ReadBlock(data, 8, 0x22);
                    return sampl;
                }
                public byte[] Rebuild()
                {
                    var out_data = new List<byte>();
                    out_data.AddRange(BitConverter.GetBytes((UInt16)VAG_Id));
                    out_data.Add(Volume_Id);
                    out_data.Add(Audio_Speed);
                    out_data.Add(Audio_Stereo_ID);
                    out_data.Add(Volume);
                    out_data.Add(Max_Volume);
                    out_data.Add(Convert.ToByte(_Audio_Type));
                    if(ExtraData.Length>0)
                        out_data.AddRange(ExtraData);
                    return out_data.ToArray();
                }
            }

            public override Section Read(byte[] data) => new VagSamples()
            {
                _Type = (Type)data.ReadULong(0),
                Size = (uint)data.ReadUInt(8, 32),
                Samples_Count = (int)data.ReadUInt(0xC, 32) + 1,
                VAG_Samples = Enumerable.Range(0, ((int)data.ReadUInt(0xC, 32) + 1)).
                Select(x => (int)data.ReadUInt(0x10 + (x * 4), 32) == -1 ? new Sample(true) : Sample.ReadSample(data.ReadBytes((int)data.ReadUInt(0x10 + (x * 4), 32), 0x2A))).ToArray()
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                infos_data.AddRange(Enumerable.Range(0, (int)VAG_Samples.Where(y => y.isEmpty == false).Count())
                    .SelectMany(
                    x => VAG_Samples.Where(y => y.isEmpty == false).ToArray()[x].Rebuild()//!!!

                    ).ToArray()
                    );

                Size = (uint)(0x10 + (Samples_Count * 4) + infos_data.Count);
                while (Size % 0x10 != 0)
                    Size++;
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)Samples_Count - 1));
                //Create pointers
                int pos =(Samples_Count * 4) + 0x10;
                foreach (var sample in VAG_Samples)
                {
                    if (sample.isEmpty == false)
                    {
                        out_data.AddRange(BitConverter.GetBytes((UInt32)pos));
                        pos += 0x2A;
                    }
                    else
                        out_data.AddRange(BitConverter.GetBytes((UInt32)0xFFFFFFFF));
                }

                //Add the Info
                out_data.AddRange(infos_data.ToArray());
                while (out_data.Count % 0x10 != 0)
                    out_data.Add(0xff);
                #endregion

                return out_data.ToArray();
            }
        }
        public class Sset : Section
        {
            public void Add(SampleSet entry)
            {
                var list = SampleSets.ToList();
                list.Add(entry);
                entry.CreatedNow = true;
                SampleSets = list.ToArray();
                SampleSet_Count = SampleSets.Length;
            }
            public void AddRange(SampleSet[] entries)
            {
                var list = SampleSets.ToList();
                list.AddRange(entries);
                SampleSets = list.ToArray();
                SampleSet_Count = SampleSets.Length;
            }
            public class SampleSet
            {
                public bool CreatedNow = false;

                [Category("SampleSetting")]
                public bool Empty { get=>isEmpty; set=>isEmpty=value; }

                public bool isEmpty;

                public ushort _Unk1;
                public byte Volume_Id;
                public byte _Unk2;
                public ushort SampleID;

                [Category("SampleSetting")]
                public ushort Unk1 { get => _Unk1; set => _Unk1 = value; }

                [Category("SampleSetting")]
                public byte VolumeId { get => Volume_Id; set => Volume_Id = value; }

                [Category("SampleSetting")]
                public byte Unk2 { get => _Unk2; set => _Unk2 = value; }

                [Category("SampleSetting")]
                public ushort SampleIndex { get => SampleID; set => SampleID = value; }


                public SampleSet(bool empty) => isEmpty = empty;
                public static SampleSet Read(byte[] data) => new SampleSet(false)
                {

                    _Unk1 = (ushort)data.ReadUInt(0, 16),
                    Volume_Id = data[2],
                    _Unk2 = data[3],
                    SampleID = (ushort)data.ReadUInt(4, 16)
                };
                public byte[] Rebuild()
                {
                    var out_data = new List<byte>();
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk1));
                    out_data.Add(Volume_Id);
                    out_data.Add(_Unk2);
                    out_data.AddRange(BitConverter.GetBytes((UInt16)SampleID));
                    return out_data.ToArray();
                }
            }

            [Category("Sample Settings"), Description("Array of settings for each sample.")]
            public SampleSet[] Settings { get=>SampleSets; set => SampleSets = value; }

            public int SampleSet_Count;
            public SampleSet[] SampleSets;

            public override Section Read(byte[] data) => new Sset()
            {
                _Type = (Type)data.ReadULong(0),
                Size = (uint)data.ReadUInt(8, 32),
                SampleSet_Count = (int)data.ReadUInt(0xC, 32) + 1,
                SampleSets = Enumerable.Range(0, ((int)data.ReadUInt(0xC, 32) + 1)).
                Select(x => (int)data.ReadUInt(0x10 + (x * 4), 32) == -1 ? new SampleSet(true) : SampleSet.Read(data.ReadBytes((int)data.ReadUInt(0x10 + (x * 4), 32), 6))).ToArray()
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                infos_data.AddRange(Enumerable.Range(0, (int)SampleSets.Where(y => y.isEmpty == false).Count())
                .SelectMany(
                    x => SampleSets.Where(y => y.isEmpty == false).ToArray()[x].Rebuild()

                    ).ToArray()
                    );
                Size = (uint)(0x10 + (SampleSet_Count * 4) + infos_data.Count);
                while (Size % 0x10 != 0)
                    Size++;
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)SampleSet_Count - 1));
                //Create pointers
                int pos = (SampleSet_Count * 4) + 0x10;
                foreach (var sample in SampleSets)
                {
                    if (sample.isEmpty == false)
                    {
                        out_data.AddRange(BitConverter.GetBytes((UInt32)pos));
                        pos += 6;
                    }
                    else
                        out_data.AddRange(BitConverter.GetBytes((UInt32)0xFFFFFFFF));
                }

                //Add the Info
                out_data.AddRange(infos_data.ToArray());
                while (out_data.Count % 0x10 != 0)
                    out_data.Add(0xff);
                #endregion

                return out_data.ToArray();
            }
        };
        public class Prog : Section
        {
            public void Add(Entry entry)
            {
                var list = Entries.ToList();
                list.Add(entry);
                entry.CreatedNow = true;
                Entries = list.ToArray();
                EntriesCount = Entries.Length;
            }
            public void AddRange(Entry[] entries)
            {
                var list = Entries.ToList();
                list.AddRange(entries);
                Entries = list.ToArray();
                EntriesCount = Entries.Length;
            }
            public class Entry
            {
                public bool CreatedNow = false;

                [Category("SampleSetting")]
                public bool Empty { get => isEmpty; set => isEmpty = value; }

                public bool isEmpty;
                public class Extension
                {
                    public ushort Index;
                    public byte _Unk, _Unk1, _Unk2, _Unk3, _Unk4;

                    public ushort _Unk5, _Unk6, _Unk7, _Unk8;

                    public byte _Unk9, _Unk10, _Unk11, _Unk12, _Unk13;

                    [Category("Unknow")]
                    public byte Unk { get => _Unk; set => _Unk = value; }

                    [Category("Unknow")]
                    public byte Unk1 { get => _Unk1; set => _Unk1 = value; }

                    [Category("Unknow")]
                    public byte Unk2 { get => _Unk2; set => _Unk2 = value; }

                    [Category("Unknow")]
                    public byte Unk3 { get => _Unk3; set => _Unk3 = value; }

                    [Category("Unknow")]
                    public byte Unk4 { get => _Unk4; set => _Unk4 = value; }

                    [Category("Unknow")]
                    public ushort Unk5 { get => _Unk5; set => _Unk5 = value; }

                    [Category("Unknow")]
                    public ushort Unk6 { get => _Unk6; set => _Unk6 = value; }

                    [Category("Unknow")]
                    public ushort Unk7 { get => _Unk7; set => _Unk7 = value; }

                    [Category("Unknow")]
                    public ushort Unk8 { get => _Unk8; set => _Unk8 = value; }

                    [Category("Unknow")]
                    public byte Unk9 { get => _Unk9; set => _Unk9 = value; }

                    [Category("Unknow")]
                    public byte Unk10 { get => _Unk10; set => _Unk10 = value; }

                    [Category("Unknow")]
                    public byte Unk11 { get => _Unk11; set => _Unk11 = value; }

                    [Category("Unknow")]
                    public byte Unk12 { get => _Unk12; set => _Unk12 = value; }

                    [Category("Unknow")]
                    public byte Unk13 { get => _Unk13; set => _Unk13 = value; }



                    [Category("File")]
                    [Description("Index of SampleSettings for the VAG audio file.")]
                    public int SSetIndex { get => (int)Index; set => Index = (ushort)value; }

                    public static Extension Read(byte[] data) => new Extension()
                    {
                        Index = (ushort)data.ReadUInt(0, 16),
                        _Unk = data[2],
                        _Unk1 = data[3],
                        _Unk2 = data[4],
                        _Unk3 = data[5],
                        _Unk4 = data[6],

                        _Unk5 = (ushort)data.ReadUInt(7, 16),
                        _Unk6 = (ushort)data.ReadUInt(9, 16),
                        _Unk7 = (ushort)data.ReadUInt(0xB, 16),
                        _Unk8 = (ushort)data.ReadUInt(0xD, 16),

                        _Unk9 = data[0xF],
                        _Unk10 = data[0x10],
                        _Unk11 = data[0x11],
                        _Unk12 = data[0x12],
                        _Unk13 = data[0x13]
                    };
                    public byte[] Rebuild()
                    {
                        var out_data = new List<byte>();
                        out_data.AddRange(BitConverter.GetBytes((UInt16)Index));

                        out_data.Add(_Unk);
                        out_data.Add(_Unk1);
                        out_data.Add(_Unk2);
                        out_data.Add(_Unk3);
                        out_data.Add(_Unk4);

                        out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk5));
                        out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk6));
                        out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk7));
                        out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk8));

                        out_data.Add(_Unk9);
                        out_data.Add(_Unk10);
                        out_data.Add(_Unk11);
                        out_data.Add(_Unk12);
                        out_data.Add(_Unk13);

                        return out_data.ToArray();
                    }
                }

                public UInt32 Size;
                public byte Extent_Count;
                public byte Extent_Size;
                public byte Volume;

                [Category("Files")]
                [Description("Files count for the array.")]
                public int FilesCount { get=> Files.Length; }

                [Category("Files")]
                public byte FileDataSize { get=>Extent_Size; set=>Extent_Size=value; }

                [Category("Files")]
                public byte UnkVolume { get=>Volume; set=>Volume=value; }



                //UNKNOW SECTION
                public byte _Unk, _Unk1, _Unk2;

                public ushort _Unk3, _Unk4;

                public byte _Unk5;

                public ushort _Unk6;

                public UInt32 _Unk7;

                public ushort _Unk8, _Unk9, _Unk10, _Unk11, _Unk12, _Unk13;

                public UInt32 _Unk14;

                [Category("Unknow")]
                public byte Unk { get => _Unk; set => _Unk = value; }

                [Category("Unknow")]
                public byte Unk1 { get => _Unk1; set => _Unk1 = value; }

                [Category("Unknow")]
                public byte Unk2 { get => _Unk2; set => _Unk2 = value; }

                [Category("Unknow")]
                public ushort Unk3 { get => _Unk3; set => _Unk3 = value; }

                [Category("Unknow")]
                public ushort Unk4 { get => _Unk4; set => _Unk4 = value; }

                [Category("Unknow")]
                public byte Unk5 { get => _Unk5; set => _Unk5 = value; }

                [Category("Unknow")]
                public ushort Unk6 { get => _Unk6; set => _Unk6 = value; }

                [Category("Unknow")]
                public uint Unk7 { get => _Unk7; set => _Unk7 = value; }

                [Category("Unknow")]
                public ushort Unk8 { get => _Unk8; set => _Unk8 = value; }

                [Category("Unknow")]
                public ushort Unk9 { get => _Unk9; set => _Unk9 = value; }

                [Category("Unknow")]
                public ushort Unk10 { get => _Unk10; set => _Unk10 = value; }

                [Category("Unknow")]
                public ushort Unk11 { get => _Unk11; set => _Unk11 = value; }

                [Category("Unknow")]
                public ushort Unk12 { get => _Unk12; set => _Unk12 = value; }

                [Category("Unknow")]
                public ushort Unk13 { get => _Unk13; set => _Unk13 = value; }

                [Category("Unknow")]
                public uint Unk14 { get => _Unk14; set => _Unk14 = value; }

                public Extension[] Extents;

                [Category("Program Entry: Folder")]
                [Description("Folder file count.")]
                public byte Count { get => Extent_Count; }

                [Category("Files")]
                [Description("Folder file(s).")]
                public Extension[] Files { get => Extents; set => Extents = value; }
                public Entry(bool empty, bool created = false) => isEmpty = empty;
                public static Entry[] ReadAll(byte[] entire_data, int count)
                {
                    var list = new List<Entry>();
                    int offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] data = entire_data.ReadBytes(offset, entire_data[offset]);
                        int total_size = data.Length + (data[4] * data[5]);
                        data = entire_data.ReadBytes(offset, total_size);
                        list.Add(Entry.Read(data));

                        offset += total_size;
                    }
                    return list.ToArray();
                }
                public static Entry Read(byte[] data) => new Entry(false)
                {
                    Size = data.ReadUInt(0, 32),
                    Extent_Count = data[4],
                    Extent_Size = data[5],

                    _Unk = data[6],
                    _Unk1 = data[7],
                    _Unk2 = data[8],

                    _Unk3 = (ushort)data.ReadUInt(9, 16),
                    _Unk4 = (ushort)data.ReadUInt(0xB, 16),

                    _Unk5 = data[0xD],
                    _Unk6 = (ushort)data.ReadUInt(0xE, 16),

                    _Unk7 = data.ReadUInt(0x10, 32),

                    _Unk8 = (ushort)data.ReadUInt(0x14, 16),
                    _Unk9 = (ushort)data.ReadUInt(0x16, 16),
                    _Unk10 = (ushort)data.ReadUInt(0x18, 16),
                    _Unk11 = (ushort)data.ReadUInt(0x1A, 16),
                    _Unk12 = (ushort)data.ReadUInt(0x1C, 16),
                    _Unk13 = (ushort)data.ReadUInt(0x1E, 16),

                    _Unk14 = data.ReadUInt(0x20, 32),

                    Extents = Enumerable.Range(0, data[4])
                    .Select(x => Extension.Read(data.ReadBytes(0x24 + (x * data[5]), data[5])))
                    .ToArray()
                };

                public byte[] Rebuild()
                {
                    var out_data = new List<byte>();
                    out_data.AddRange(BitConverter.GetBytes((UInt32)Size));
                    out_data.Add(Extent_Count);
                    out_data.Add(Extent_Size);
                    out_data.Add(_Unk);
                    out_data.Add(_Unk1);
                    out_data.Add(_Unk2);

                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk3));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk4));

                    out_data.Add(_Unk5);

                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk6));

                    out_data.AddRange(BitConverter.GetBytes((UInt32)_Unk7));

                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk8));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk9));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk10));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk11));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk12));
                    out_data.AddRange(BitConverter.GetBytes((UInt16)_Unk13));

                    out_data.AddRange(BitConverter.GetBytes((UInt32)_Unk14));

                    //Extent Sections
                    if (Extent_Count > 0)
                        out_data.AddRange(Enumerable.Range(0, Extent_Count)
                            .SelectMany(x => Extents[x].Rebuild()).ToArray());

                    return out_data.ToArray();
                }

            }

            public int EntriesCount;
            public Entry[] Entries;

            public override Section Read(byte[] data)
            {
                var prog = new Prog();
                prog._Type = (Type)data.ReadULong(0);
                prog.Size = (uint)data.ReadUInt(8, 32);
                prog.EntriesCount = (int)data.ReadUInt(0xC, 32) + 1;
                prog.Entries = new Entry[prog.EntriesCount];
                for (int i = 0; i < prog.EntriesCount; i++)
                {
                    Entry entry;
                    int pointer = (int)data.ReadUInt(0x10 + (i * 4), 32);
                    if (pointer > -1)
                        entry = Entry.Read(data.ReadEntryBytes(pointer));
                    else
                        entry = new Entry(true);

                    prog.Entries[i] = entry;
                }
                return prog;
            }
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                var infos_data = new List<byte>();

                infos_data.AddRange(Enumerable.Range(0, (int)Entries.Where(y => y.isEmpty == false).Count())
                .SelectMany(
                    x => Entries.Where(y => y.isEmpty == false).ToArray()[x].Rebuild()
                    ).ToArray()
                    );

                #region Create pointers and Consolidate Infos
                Size = (uint)(0x10 + (EntriesCount * 4) + infos_data.Count);
                while (Size % 0x10 != 0)
                    Size++;

                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)EntriesCount - 1));

                //Create pointers
                int pos = (EntriesCount * 4) + 0x10;
                foreach (var entry in Entries)
                {
                    if (entry.isEmpty == true)
                    {
                        out_data.AddRange(BitConverter.GetBytes((UInt32)0xFFFFFFFF));
                    }
                    else
                    {
                        byte[] entry_rebuild = entry.Rebuild();
                        out_data.AddRange(BitConverter.GetBytes((UInt32)pos));
                        pos += entry_rebuild.Length;
                    }
                }
                #endregion

                //Add the Info
                out_data.AddRange(infos_data.ToArray());
                while (out_data.Count % 0x10 != 0)
                    out_data.Add(0xff);
                return out_data.ToArray();
            }
        }
        #endregion

    }
    public class BINContainer
    {
        public class SCEI_Entry
        {
            internal Int32 Pack_Offset;
            internal Int32 VAGStream_AftrProg_Offset;
            internal Int32 PackStream_Length;

            internal Int32 Pack_Size;

            internal Int32 ELFPointer_Offset;

            internal IECS scei_File;

            internal bool Excluded_Entry = false;

            internal int[] Repeated_Indices;

            internal static SCEI_Entry Read(byte[] input, int offset, bool excluded = false) => new SCEI_Entry()
            {
                Pack_Offset = (int)input.ReadUInt(0, 32),
                VAGStream_AftrProg_Offset = (int)input.ReadUInt(4, 32),
                PackStream_Length = (int)input.ReadUInt(8, 32),
                Excluded_Entry = excluded,
                Pack_Size = DefineSizes((int)input.ReadUInt(0, 32), (int)input.ReadUInt(4, 32), (int)input.ReadUInt(8, 32)),

                ELFPointer_Offset = offset
            };

            internal static Int32 DefineSizes(int poffs, int vafoffs, int pleng)
            {
                int Sc_Size = poffs + vafoffs;
                while (Sc_Size % 0x800 != 0)
                    Sc_Size++;
                Sc_Size += pleng;
                while (Sc_Size % 0x800 != 0)
                    Sc_Size++;
                Sc_Size -= poffs;

                return Sc_Size;
            }
        }


        public int fileCount;
        public byte[] Container;
        public byte[] currentContainer;
        public string caminhoELF;

        public ELFO linkedELF;
        internal List<SCEI_Entry> sCEI_Entries;
        internal Dictionary<int, int> SCEI_Tables;
        internal Dictionary<string, string[]> SCEI_Names;
        public BINContainer(byte[] input, byte[] ELF, string elfpath)
        {
            Container = input;

            #region ELF Link
            linkedELF = new ELFO(ELF, elfpath);
            sCEI_Entries = new List<SCEI_Entry>();

            #region Set Tables
            linkedELF.GetXML(out SCEI_Tables, out SCEI_Names);

            #region Read Scei Entries
            foreach (var table in SCEI_Tables)
            {
                for (int i = table.Key; i < table.Value; i += 0xC)
                {
                    byte[] Entry = ReadBlock(linkedELF.ELF, (uint)i, 0xC);
                    var newEntry = SCEI_Entry.Read(Entry, i);
                    sCEI_Entries.Add(newEntry);
                    //RESOLVER ENTRADA FANTASMA QUE NÃO ESTÁ NO ELF::Entrada removida na reconstrução
                }

#if DEBUG
                sCEI_Entries = sCEI_Entries.Take(2).ToList();
#endif
                foreach (var entry in sCEI_Entries)
                {
                    var repeatedIndices = new List<int>();
                    for (int k = 0; k < sCEI_Entries.Count; k++)
                    {
                        if (sCEI_Entries[k].Pack_Offset == entry.Pack_Offset)
                            repeatedIndices.Add(k);
                    }
                    entry.Repeated_Indices = repeatedIndices.ToArray();
                }



            }
            #endregion

            #region Read SCEIPACKS
            foreach (SCEI_Entry sc_entry in sCEI_Entries)
            {
                if (sc_entry.Pack_Offset.ToString("X2") != "FFFFFFFF")
                {
                    byte[] iecs = ReadBlock(input, (uint)sc_entry.Pack_Offset, (uint)sc_entry.Pack_Size);
                    var sceiFile = new IECS(iecs);
                    sc_entry.scei_File = sceiFile;
                }
            }

            #endregion

            #endregion
            #endregion
            
        }
        public void Rebuild()
        {
            var container = new List<byte>();
            int offset = 0;
            var repeatedSkip = new List<int>();

            var AlterableEntries = sCEI_Entries.ToList();
            AlterableEntries = AlterableEntries.GroupBy(x => x.Pack_Offset).Select(y => y.First())
                .OrderBy(x => x.Pack_Offset).ToList();

            #region Write to BIN
            for (int i = 0; i < AlterableEntries.Count; i++)
            {
                if (AlterableEntries[i].Pack_Offset.ToString("X2") != "FFFFFFFF")
                {
                    byte[] rebuilded_iecs = AlterableEntries[i].scei_File.RebuildIECS();
                    container.AddRange(rebuilded_iecs);
                    currentContainer = rebuilded_iecs;

                    AlterableEntries[i].PackStream_Length = AlterableEntries[i].scei_File._Header.VAGStream_Size;
                    AlterableEntries[i].Pack_Offset = offset;

                    while (container.Count % 0x800 != 0)
                        container.Add(0xFF);

                    offset += currentContainer.Length;
                    while (offset % 0x800 != 0)
                        offset++;
                }
            }

            #endregion

            #region ELF Tables
            foreach (var entryOrdened in AlterableEntries)
            {
                if (entryOrdened.Pack_Offset.ToString("X2") != "FFFFFFFF")
                    for (int i = 0; i < sCEI_Entries.Count; i++)
                    {
                        if (entryOrdened.ELFPointer_Offset == sCEI_Entries[i]
                            .ELFPointer_Offset)
                        {
                            sCEI_Entries[i].Pack_Offset = entryOrdened.Pack_Offset;
                            sCEI_Entries[i].PackStream_Length = entryOrdened.PackStream_Length;
                            sCEI_Entries[i].VAGStream_AftrProg_Offset = entryOrdened.VAGStream_AftrProg_Offset;

                            //Setar Entradas Repetidas
                            if (entryOrdened.Repeated_Indices != null && entryOrdened.Repeated_Indices.Count() > 0)
                            {
                                for (int r = 0; r < entryOrdened.Repeated_Indices.Count(); r++)
                                {
                                    sCEI_Entries[entryOrdened.Repeated_Indices[r]].Pack_Offset = entryOrdened.Pack_Offset;
                                    sCEI_Entries[entryOrdened.Repeated_Indices[r]].PackStream_Length = entryOrdened.PackStream_Length;
                                    sCEI_Entries[entryOrdened.Repeated_Indices[r]].VAGStream_AftrProg_Offset = entryOrdened.VAGStream_AftrProg_Offset;
                                }
                            }

                        }
                    }

            }

            #region Write to ELF
            foreach (var entry in sCEI_Entries)
            {
                Array.Copy(BitConverter.GetBytes(entry.Pack_Offset), 0, linkedELF.InputElf, entry.ELFPointer_Offset, 4);
                Array.Copy(BitConverter.GetBytes(entry.VAGStream_AftrProg_Offset), 0, linkedELF.InputElf, entry.ELFPointer_Offset + 4, 4);
                Array.Copy(BitConverter.GetBytes(entry.PackStream_Length), 0, linkedELF.InputElf, entry.ELFPointer_Offset + 8, 4);
            }

            #endregion
            #endregion
            Container = container.ToArray();

        }
    }
}
