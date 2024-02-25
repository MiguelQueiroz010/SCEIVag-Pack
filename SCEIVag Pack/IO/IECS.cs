using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static SCEIVag_Pack.Bin;
using System.Windows.Forms;

namespace SCEIVag_Pack
{
    public class IECS
    {
        public byte[] HeaderData;
        public byte[] StreamData;
        public byte[] Input;

        public int Size;
        public int IECS_Size;
        public int VAGS_Offset;
        public int VAGS_Size;
        public int ProgSec_Offset;
        public int SsetSec_Offset;
        public int SampleSec_Offset;
        public int VAGIndexSec_Offset;

        public List<Section> Seções;
        public Version version;
        public VAGIndex vagi;
        public Sample sample;
        public Sset sset;
        public Program prog;

        public List<int> ELFIECSunk;
        public List<int> ELFVAGS_Size;
        public IECS(byte[] input)
        {
            Input = input;
            #region Separar Offsets
            int headsize = (int)ReadUInt(input, 0x18, Int.UInt32);
            HeaderData = ReadBlock(input, 0x10, (uint)headsize);
            Size = HeaderData.Length;

            IECS_Size = (int)ReadUInt(HeaderData, 0xC, Int.UInt32);
            VAGS_Size = (int)ReadUInt(HeaderData, 0x10, Int.UInt32);
            ProgSec_Offset = (int)ReadUInt(HeaderData, 0x14, Int.UInt32);
            SsetSec_Offset = (int)ReadUInt(HeaderData, 0x18, Int.UInt32);

            SampleSec_Offset = (int)ReadUInt(HeaderData, 0x1C, Int.UInt32);
            VAGIndexSec_Offset = (int)ReadUInt(HeaderData, 0x20, Int.UInt32);
            #endregion
            #region Separar Seções
            Seções = new List<Section>();
            uint versionsize = (uint)ReadUInt(input, 0x8, Int.UInt32);
            version = new Version(ReadBlock(input, 0, versionsize));

            uint vagisize = (uint)ReadUInt(input, VAGIndexSec_Offset + 8, Int.UInt32);
            vagi = new VAGIndex(ReadBlock(input, (uint)VAGIndexSec_Offset, vagisize), VAGS_Size);

            uint samplesize = (uint)ReadUInt(input, SampleSec_Offset + 8, Int.UInt32);
            sample = new Sample(ReadBlock(input, (uint)SampleSec_Offset, samplesize));

            uint ssetsize = (uint)ReadUInt(input, SsetSec_Offset + 8, Int.UInt32);
            sset = new Sset(ReadBlock(input, (uint)SsetSec_Offset, ssetsize));

            uint progsize = (uint)ReadUInt(input, ProgSec_Offset + 8, Int.UInt32);
            prog = new Program(ReadBlock(input, (uint)ProgSec_Offset, progsize));

            VAGS_Offset = IECS_Size;//Padded with 0x800
            while (VAGS_Offset % 0x800 != 0)
                VAGS_Offset++;

            StreamData = ReadBlock(input, (uint)VAGS_Offset, (uint)VAGS_Size);

            Seções.Add(version);
            Seções.Add(vagi);
            Seções.Add(sample);
            Seções.Add(sset);
            Seções.Add(prog);
            #endregion
            #region Separar Seções
            foreach (var vag in vagi.Vdata)
            {
                byte[] vags = ReadBlock(StreamData, (uint)vag.StreamOffset, (uint)vag.StreamSize);
                vag.StreamVAG = vags;
            }
            #endregion
        }
        public void RebuildIECS()
        {
            var iecs = new List<byte>();
            #region Version
            iecs.AddRange(version.SectionDATA);
            #endregion
            #region Vagi+Streams
            var streamdata = new List<byte>();
            int offset = 0;
            foreach(var vag in vagi.Vdata)
            {
                var vags = new List<byte>();
                vags.AddRange(vag.StreamVAG);
                while (vags.Count % 0x10 != 0)
                    streamdata.Add(0);//ou 0x77 wwwww

                streamdata.AddRange(vags.ToArray());
                vag.StreamOffset = offset;
                vag.StreamSize = vags.Count;
                offset += vag.StreamSize;

                #region Write to Section
                Array.Copy(BitConverter.GetBytes((UInt32)vag.StreamOffset), 0, vagi.SectionDATA, vag.PointerSecOff, 4);//Write offsets to data
                Array.Copy(BitConverter.GetBytes((UInt16)vag.Frequency), 0, vagi.SectionDATA, vag.PointerSecOff+4, 2);//Write freqs to data
                #endregion

            }
            StreamData = streamdata.ToArray();
            #endregion
            VAGS_Size = StreamData.Length;
            BitConverter.GetBytes(VAGS_Size).CopyTo(HeaderData, 0x10);
            iecs.AddRange(HeaderData);
            iecs.AddRange(vagi.SectionDATA);
            iecs.AddRange(sample.SectionDATA);
            iecs.AddRange(sset.SectionDATA);
            iecs.AddRange(prog.SectionDATA);
            while (iecs.Count % 0x800 != 0)
                iecs.Add(0xFF);
            iecs.AddRange(StreamData);
            Input = iecs.ToArray();
        }
        public class Section
        {
            public int Size;
            public byte[] SectionDATA;
            public Type tipo;
        }
        #region Sub-Seções
        public class Version : Section
        {
            public int Versão;
            public Version(byte[] secData)
            {
                tipo = Type.Version;
                SectionDATA = secData;
                Size = SectionDATA.Length;
                Versão = (int)ReadUInt(SectionDATA, 0xE, Int.UInt16);
            }

        }
        public class VAGIndex : Section
        {
            public int VAGcount;
            public int[] DataOffsets;
            public List<VAGData> Vdata;
            public VAGIndex(byte[] secData, int streamSize)
            {
                SectionDATA = secData;
                tipo = Type.VAGIndex;
                Size = SectionDATA.Length;
                VAGcount = (int)ReadUInt(SectionDATA, 0xC, Int.UInt32) + 1;
                #region Separar Offsets dos dados
                DataOffsets = new int[VAGcount];
                int offset = 0x10;
                for (int i = 0; i < VAGcount; i++)
                {
                    DataOffsets[i] = (int)ReadUInt(SectionDATA, offset, Int.UInt32);
                    offset += 4;
                }
                #endregion
                #region Separar dados VAG
                Vdata = new List<VAGData>();
                int k = 0;
                foreach (int vagoff in DataOffsets)
                {
                    int hz = (int)ReadUInt(SectionDATA, vagoff + 4, Int.UInt16);
                    int unk = (int)ReadUInt(SectionDATA, vagoff + 6, Int.UInt16);
                    int soff = (int)ReadUInt(SectionDATA, vagoff, Int.UInt32);
                    int size = 0;
                    if (k == DataOffsets.Count() - 1)
                    {
                        size = streamSize - soff;
                    }
                    else
                    {
                        size = (int)ReadUInt(SectionDATA, DataOffsets[k + 1], Int.UInt32) - soff;
                    }
                    Vdata.Add(new VAGData(hz, unk, soff, size,vagoff));
                    k++;
                }
                #endregion                
            }
            public class VAGData
            {
                public int Frequency;
                public int unk;
                public int StreamOffset, PointerSecOff;
                public int StreamSize;
                public byte[] StreamVAG;
                public VAGData(int hz, int Unk, int soffset, int size,int poff = 0, byte[] vag = null)
                {
                    Frequency = hz;
                    unk = Unk;
                    StreamOffset = soffset;
                    StreamSize = size;
                    StreamVAG = vag;
                    PointerSecOff = poff; 
                }
            }
        }
        public class Sample : Section
        {
            public Sample(byte[] secData)
            {
                SectionDATA = secData;
                tipo = Type.Sample;
                Size = SectionDATA.Length;
            }
        }
        public class Sset : Section
        {
            public Sset(byte[] secData)
            {
                SectionDATA = secData;
                tipo = Type.Sset;
                Size = SectionDATA.Length;
            }
        }
        public class Program : Section
        {
            public int EntriesCount;
            public int[] EntryOffsets;
            public List<byte[]> Entries;
            public Program(byte[] secData)
            {
                SectionDATA = secData;
                Size = SectionDATA.Length;
                tipo = Type.Program;
                //EntriesCount = (int)ReadUInt(SectionDATA, 0xC, Int.UInt32)+1;
                //#region Separar Offsets
                //EntryOffsets = new int[EntriesCount];
                //int offset = 0x10;
                //for(int i =0;i<EntriesCount;i++)
                //{
                //    EntryOffsets[i] = (int)ReadUInt(SectionDATA, offset, Int.UInt32);
                //    offset += 4;
                //}
                //#endregion
                //#region Separar Entries
                //Entries = new List<byte[]>();
                //int q = 0;
                //int size = 0;
                //foreach (int enoffset in EntryOffsets)
                //{
                //    if (q == EntryOffsets.Count()-1)
                //    {
                //        size = Size - enoffset;
                //    }
                //    else
                //    {
                //        size = EntryOffsets[q + 1] - enoffset;
                //    }
                //    Entries.Add(ReadBlock(SectionDATA, (uint)enoffset, (uint)size));
                //    q++;
                //}
                //#endregion
            }

        }
        #endregion
        public enum Type
        {
            Version,
            VAGIndex,
            Sample,
            Sset,
            Program,
            Head
        };
    }
    public class BINContainer
    {
        public List<IECS> sceiFiles;
        public int fileCount;
        public byte[] Container;
        public byte[] currentContainer;
        public string caminhoELF;
        public List<int> IECSoffsets;
        public List<int> ELFIECSoffset = new List<int>();
        public List<int> ELFIECSunk = new List<int>();
        public List<int> ELFIECSVAGS_Size = new List<int>();
        public List<int> IECSsizes;
        public BINContainer(byte[] input)
        {
            Container = input;
            #region Contagem e Offsets
            fileCount = 0;
            IECSoffsets = new List<int>();
            for (int i = 0; i < Container.Length; i += 8)
            {
                if (Encoding.Default.GetString(ReadBlock(Container, (uint)i, 8)) == "IECSsreV")
                {
                    IECSoffsets.Add(i);
                    fileCount++;
                }
            }
            #endregion
            #region Separar IECS
            sceiFiles = new List<IECS>();
            int k = 0;
            int size = 0;
            IECSsizes = new List<int>();
            foreach (var iecoff in IECSoffsets)
            {
                if (k == IECSoffsets.Count() - 1)
                {
                    size = input.Length - iecoff;
                }
                else
                {
                    size = IECSoffsets[k + 1] - iecoff;
                }
                byte[] iecs = ReadBlock(input, (uint)iecoff, (uint)size);
                IECSsizes.Add(iecs.Length);
                sceiFiles.Add(new IECS(iecs));
                k++;
            }
            #endregion
        }
        public void Rebuild()
        {
            ELFIECSoffset.Clear();
            ELFIECSunk.Clear();
            ELFIECSVAGS_Size.Clear();
            int[] indicesRemover = { 25, 26 };
            var container = new List<byte>();
            for (int i = 0; i < fileCount; i++)
            {
                container.AddRange(sceiFiles[i].Input);
                currentContainer = sceiFiles[i].Input.ToArray();
                int unk = BitConverter.ToInt32(currentContainer, 0x1C);
                int vags_size = BitConverter.ToInt32(currentContainer, 0x20);

                while (container.Count % 0x800 != 0)
                    container.Add(0xFF);


                if (!indicesRemover.Contains(i))
                {
                    ELFIECSunk.Add(unk);
                    ELFIECSVAGS_Size.Add(vags_size);
                    if (i == 0)
                    {
                        ELFIECSoffset.Add(0);
                    }
                    ELFIECSoffset.Add(container.Count);

                }
            }
            Container = container.ToArray();

            ELFIECSoffset.RemoveAt(23);
            ELFIECSunk.RemoveAt(23);
            ELFIECSVAGS_Size.RemoveAt(23);
            ELFIECSoffset.RemoveAt(24);
            ELFIECSunk.RemoveAt(24);
            ELFIECSVAGS_Size.RemoveAt(24);

            TrocarIndices(ELFIECSunk, 4, 3);
            TrocarIndices(ELFIECSVAGS_Size, 4, 3);
            TrocarIndices(ELFIECSoffset, 4, 3);

            int[] indices = { 5, 13, 25, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 };

            int novoValor = -1;

            foreach (int indice in indices)
            {
                AdicionarValorNoIndice(ELFIECSunk, indice, novoValor);

                AdicionarValorNoIndice(ELFIECSVAGS_Size, indice, novoValor);

                AdicionarValorNoIndice(ELFIECSoffset, indice, novoValor);
            }

            ELFIECSoffset.Insert(44, ELFIECSoffset[6]);
            ELFIECSunk.Insert(44, ELFIECSunk[6]);
            ELFIECSVAGS_Size.Insert(44, ELFIECSVAGS_Size[6]);

            for (int i = 6; i < 10; i++)
            {
                ELFIECSoffset.Insert(i + 46, ELFIECSoffset[i]);
                ELFIECSVAGS_Size.Insert(i + 46, ELFIECSVAGS_Size[i]);
                ELFIECSunk.Insert(i + 46, ELFIECSunk[i]);
            }

            ELFIECSoffset.Insert(56, ELFIECSoffset[19]);
            ELFIECSunk.Insert(56, ELFIECSunk[19]);
            ELFIECSVAGS_Size.Insert(56, ELFIECSVAGS_Size[19]);

            for (int i = 39; i < 44; i++)
            {
                ELFIECSoffset.Insert(i + 18, ELFIECSoffset[i]);
                ELFIECSVAGS_Size.Insert(i + 18, ELFIECSVAGS_Size[i]);
                ELFIECSunk.Insert(i + 18, ELFIECSunk[i]);
            }


            ELFIECSoffset.Insert(78, ELFIECSoffset[62]);
            ELFIECSunk.Insert(78, ELFIECSunk[62]);
            ELFIECSVAGS_Size.Insert(78, ELFIECSVAGS_Size[62]);

            ELFIECSoffset.Insert(79, -1);
            ELFIECSunk.Insert(79, -1);
            ELFIECSVAGS_Size.Insert(79, -1);

            ELFIECSoffset.Insert(80, ELFIECSoffset[68]);
            ELFIECSunk.Insert(80, ELFIECSunk[68]);
            ELFIECSVAGS_Size.Insert(80, ELFIECSVAGS_Size[68]);

            ELFIECSoffset.Insert(93, -1);
            ELFIECSunk.Insert(93, -1);
            ELFIECSVAGS_Size.Insert(93, -1);
        }

        private void TrocarIndices(List<int> lista, int indiceA, int indiceB)
        {
            int temp = lista[indiceA];
            lista[indiceA] = lista[indiceB];
            lista[indiceB] = temp;
        }

        void AdicionarValorNoIndice(List<int> lista, int indice, int valor)
        {
            lista.Insert(indice, valor);
        }

        public void SaveToELF(string caminho)
        {
            for (int i = 0; i < ELFIECSunk.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        using (FileStream fileStream = new FileStream(caminho, FileMode.Open, FileAccess.Write))
                        {
                            fileStream.Seek(0x31581C, SeekOrigin.Begin);

                            int valor = ELFIECSunk[i];
                            int valor2 = ELFIECSVAGS_Size[i];
                            int valor3 = ELFIECSoffset[i];
                            byte[] bytes = BitConverter.GetBytes(valor);
                            fileStream.Write(bytes, 0, bytes.Length);
                            byte[] bytes2 = BitConverter.GetBytes(valor2);
                            fileStream.Write(bytes2, 0, bytes2.Length);
                            byte[] bytes3 = BitConverter.GetBytes(valor3);
                            fileStream.Write(bytes3, 0, bytes3.Length);
                        }
                        break;
                    case int ValorF when ValorF >= 1 && ValorF <= 4:
                        using (FileStream fileStream = new FileStream(caminho, FileMode.Open, FileAccess.Write))
                        {
                            int value = i - 1;
                            int offsetC = value * 0xC;
                            int OffsetD = 0x315CA0 + offsetC;
                            fileStream.Seek(OffsetD, SeekOrigin.Begin);

                            int valor = ELFIECSoffset[i];
                            int valor2 = ELFIECSunk[i];
                            int valor3 = ELFIECSVAGS_Size[i];
                            byte[] bytes = BitConverter.GetBytes(valor);
                            fileStream.Write(bytes, 0, bytes.Length);
                            byte[] bytes2 = BitConverter.GetBytes(valor2);
                            fileStream.Write(bytes2, 0, bytes2.Length);
                            byte[] bytes3 = BitConverter.GetBytes(valor3);
                            fileStream.Write(bytes3, 0, bytes3.Length);
                        }
                        break;
                    case int valorF when valorF >= 5:
                        using (FileStream fileStream = new FileStream(caminho, FileMode.Open, FileAccess.Write))
                        {
                            int value = i - 5;
                            int offsetC = value * 0xC;
                            int OffsetD = 0x315830 + offsetC;
                            fileStream.Seek(OffsetD, SeekOrigin.Begin);

                            int valor = ELFIECSoffset[i];
                            int valor2 = ELFIECSunk[i];
                            int valor3 = ELFIECSVAGS_Size[i];
                            byte[] bytes = BitConverter.GetBytes(valor);
                            fileStream.Write(bytes, 0, bytes.Length);
                            byte[] bytes2 = BitConverter.GetBytes(valor2);
                            fileStream.Write(bytes2, 0, bytes2.Length);
                            byte[] bytes3 = BitConverter.GetBytes(valor3);
                            fileStream.Write(bytes3, 0, bytes3.Length);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
