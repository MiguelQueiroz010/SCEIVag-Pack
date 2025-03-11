using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static SCEIVag_Pack.Bin;
using System.Xml.Linq;
using static SCEIVag_Pack.IECS.Sset;
using System.Media;
using System.ComponentModel;

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
        public void RebuildIECS()
        {
            var iecs = new List<byte>();
            #region Sections
            //Version
            byte[] _Vers = _Version.Rebuild();

            //Program
            byte[] _Prog = _Program.Rebuild();

            //Sample Sets
            byte[] _sset = _SampleSets.Rebuild();

            //Samples
            byte[] _samples = _Samples.Rebuild();

            //Infos
            byte[] _infos = _Infos.Rebuild();

            //Head


            #endregion

            //0x800 Sector Padding
            while (iecs.Count % 0x800 != 0)
                iecs.Add(0xFF);

            //Add all data together
            iecs.AddRange(_Version.Rebuild()); //Version



            iecs.AddRange(StreamData); //Vags Stream
            Input = iecs.ToArray();
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
            public int ProgSect_Size;
            public int PssetSect_Size;

            public int SampleSect_Offset;
            public int VAGInfosSect_Offset;
            public override Section Read(byte[] secData) => new Head()
            {
                _Type = (Type)secData.ReadULong(0),
                Size = (uint)secData.ReadUInt(8, 32),
                SectionsSize = (int)ReadUInt(secData, 0xC, Int.UInt32),
                VAGStream_Size = (int)ReadUInt(secData, 0x10, Int.UInt32),
                ProgSect_Size = (int)ReadUInt(secData, 0x14, Int.UInt32),
                PssetSect_Size = (int)ReadUInt(secData, 0x18, Int.UInt32),
                SampleSect_Offset = (int)ReadUInt(secData, 0x1c, Int.UInt32),
                VAGInfosSect_Offset = (int)ReadUInt(secData, 0x20, Int.UInt32)
            };
            public override byte[] Rebuild()
            {
                var out_data = new List<byte>();
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)SectionsSize));
                out_data.AddRange(BitConverter.GetBytes((UInt32)VAGStream_Size));
                out_data.AddRange(BitConverter.GetBytes((UInt32)ProgSect_Size));
                out_data.AddRange(BitConverter.GetBytes((UInt32)PssetSect_Size));

                out_data.AddRange(BitConverter.GetBytes((UInt32)SampleSect_Offset));
                out_data.AddRange(BitConverter.GetBytes((UInt32)VAGInfosSect_Offset));

                while (out_data.Count() < Size)
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
                return out_data.ToArray();
            }
        }
        public class VAGInfo : Section
        {
            public class Information
            {
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
                bool _Loop;
                [Category("Audio")]
                [Description("Sets if the audio will loop infinitely or not.\nTrue | False")]
                public bool Loop {
                    get => _Loop;
                    set => _Loop = value;
                }



                byte Secure_EndSignal;
                //ENDSIGNAL = 0xFF +1 Byte

                public byte[] VAG;

                public static Information Read(byte[] data) => new Information()
                {
                    Stream_Offset = (uint)data.ReadUInt(0, 32),
                    _hz = (ushort)data.ReadUInt(4, 16),
                    _Loop = Convert.ToBoolean(data[6]),
                    Secure_EndSignal = data[7] //ONLY FOR SECURITY!
                };
                public byte[] Rebuild()
                {
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
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)VAG_Count - 1));

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                infos_data.AddRange(Enumerable.Range(0, (int)VAG_Count)
                    .SelectMany(
                    x => VAG_Infos[x].Rebuild()

                    ).ToArray()
                    );

                //Create pointers
                out_data.AddRange(Enumerable.Range(0, (int)VAG_Count)
                    .SelectMany(
                    x => BitConverter.GetBytes((UInt32)((x * 8) + ((VAG_Count * 4) + 0x10)))

                    ).ToArray()
                    );

                //Add the Info
                out_data.AddRange(infos_data.ToArray());

                #endregion

                return out_data.ToArray();
            }

        }
        public class VagSamples : Section
        {
            public int Samples_Count;
            public Sample[] VAG_Samples;

            public class Sample
            {
                public bool isEmpty { get; set; }
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

                public Sample(bool empty) => isEmpty = empty;
                public static Sample ReadSample(byte[] data) => new Sample(false)
                {
                    VAG_Id = (ushort)data.ReadUInt(0, 16),
                    Volume_Id = data[2],
                    Audio_Speed = data[3],
                    Audio_Stereo_ID = data[4],
                    Volume = data[5],
                    Max_Volume = data[6],
                    _Audio_Type = (Audio_Type)data[7]
                };
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
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)Samples_Count - 1));

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                infos_data.AddRange(Enumerable.Range(0, (int)Samples_Count)
                    .SelectMany(
                    x => VAG_Samples.Where(y => y != null).ToArray()[x].Rebuild()

                    ).ToArray()
                    );

                //Create pointers
                int pos = Samples_Count * 4;
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

                #endregion

                return out_data.ToArray();
            }
        }
        public class Sset : Section
        {
            public class SampleSet
            {
                public bool isEmpty { get; set; }

                public ushort _Unk1;
                public byte Volume_Id;
                public byte _Unk2;
                public ushort SampleID;

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
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)SampleSet_Count - 1));

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();
                infos_data.AddRange(Enumerable.Range(0, (int)SampleSet_Count)
                .SelectMany(
                    x => SampleSets.Where(y => y != null).ToArray()[x].Rebuild()

                    ).ToArray()
                    );

                //Create pointers
                int pos = SampleSet_Count * 4;
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

                #endregion

                return out_data.ToArray();
            }
        };
        public class Prog : Section
        {
            public class Entry
            {
                public bool isEmpty { get; set; }
                public class Extension
                {
                    public ushort Index;
                    public byte _Unk, _Unk1, _Unk2, _Unk3, _Unk4;

                    public ushort _Unk5, _Unk6, _Unk7, _Unk8;

                    public byte _Unk9, _Unk10, _Unk11, _Unk12, _Unk13;

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

                //UNKNOW SECTION
                public byte _Unk, _Unk1, _Unk2;

                public ushort _Unk3, _Unk4;

                public byte _Unk5;

                public ushort _Unk6;

                public UInt32 _Unk7;

                public ushort _Unk8, _Unk9, _Unk10, _Unk11, _Unk12, _Unk13;

                public UInt32 _Unk14;

                public Extension[] Extents;
                public Entry(bool empty) => isEmpty = empty;
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
                            .SelectMany(x => Extents[x].Rebuild()).ToArray()
                            );

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
                out_data.AddRange(base.Rebuild());
                out_data.AddRange(BitConverter.GetBytes((UInt32)EntriesCount - 1));

                #region Create pointers and Consolidate Infos
                var infos_data = new List<byte>();

                //Create pointers
                int pos = EntriesCount * 4;
                foreach (var entry in Entries)
                {
                    byte[] entry_rebuild = entry.Rebuild();
                    infos_data.AddRange(entry_rebuild);
                    out_data.AddRange(BitConverter.GetBytes((UInt32)pos));
                    pos += entry_rebuild.Length;
                }
                #endregion

                //Add the Info
                out_data.AddRange(infos_data.ToArray());
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

            internal int[] Repeated_Indices;

            internal static SCEI_Entry Read(byte[] input, int offset) => new SCEI_Entry()
            {
                Pack_Offset = (int)input.ReadUInt(0, 32),
                VAGStream_AftrProg_Offset = (int)input.ReadUInt(4, 32),
                PackStream_Length = (int)input.ReadUInt(8, 32),

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

                }


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
            #region UNUSED
            //#region Contagem e Offsets
            //fileCount = 0;
            //IECSoffsets = new List<int>();
            //for (int i = 0; i < Container.Length; i += 8)
            //{
            //    if (Encoding.Default.GetString(ReadBlock(Container, (uint)i, 8)) == "IECSsreV")
            //    {
            //        IECSoffsets.Add(i);
            //        fileCount++;
            //    }
            //}
            //#endregion
            //#region Separar IECS
            //sceiFiles = new List<IECS>();
            //int k = 0;
            //int size = 0;
            //IECSsizes = new List<int>();
            //foreach (var iecoff in IECSoffsets)
            //{
            //    if (k == IECSoffsets.Count() - 1)
            //    {
            //        size = input.Length - iecoff;
            //    }
            //    else
            //    {
            //        size = IECSoffsets[k + 1] - iecoff;
            //    }
            //    byte[] iecs = ReadBlock(input, (uint)iecoff, (uint)size);
            //    IECSsizes.Add(iecs.Length);
            //    sceiFiles.Add(new IECS(iecs));
            //    k++;
            //}
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
                    container.AddRange(AlterableEntries[i].scei_File.Input);
                    currentContainer = AlterableEntries[i].scei_File.Input.ToArray();

                    AlterableEntries[i].PackStream_Length = AlterableEntries[i].scei_File.Size;
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
