using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using static SCEIVag_Pack.Bin;
using System.Diagnostics;

namespace SCEIVag_Pack
{
    public class MWo3
    {
        public int IDcount, Unk1, Unk2;

        public int AllocationAddress;
        public int AllocationENDAddress;
        public int DataBlockSize;
        public int DataBlockOffset;

        public string FileName, InName;

        public byte[] DataBlock, MWo3Data;

        public MWo3(string path)
        {
            FileName = path;

            MWo3Data = File.ReadAllBytes(path);

            if ((uint)ReadUInt(MWo3Data, 0, Int.UInt32) != 0x336F574D)//Magic "MWo3"
            {
                return;
            }

            IDcount = (int)ReadUInt(MWo3Data, 4, Int.UInt32);

            AllocationAddress = (int)ReadUInt(MWo3Data, 8, Int.UInt32);
            Unk1 = (int)ReadUInt(MWo3Data, 0xC, Int.UInt32);
            DataBlockSize = (int)ReadUInt(MWo3Data, 0x10, Int.UInt32);
            //4 null bytes
            AllocationENDAddress = (int)ReadUInt(MWo3Data, 0x18, Int.UInt32);

            DataBlockOffset = (int)((uint)AllocationENDAddress - (uint)AllocationAddress
                 - (uint)DataBlockSize);
            byte[] strname = ReadString(MWo3Data, 0x20);
            InName = Encoding.Default.GetString(strname);
            DataBlock = ReadBlock(MWo3Data, (uint)DataBlockOffset, (uint)DataBlockSize);
        }

        public void Save()
        {
            File.WriteAllBytes(FileName, MWo3Data);
        }

    }
    public class ELFO
    {
        public MWo3[] MWo;

        public string ElfFilename, InternalName;
        public byte[] InputElf, MWo3;

        public byte[] ELF
        {
            get;
            set;
        }


        public int padding = 0;

        public int magic, allocstart=0, allocoffset=0, allocsize=0, shift=0;
        public bool charswitch = false, mwo3mode = false, virtualmem = false;

        public ELFO(byte[] input, string path = "ELF", string[] mwo3paths = null)
        {
            ELF = input;
            InputElf = input;

            if (virtualmem == true)
            {
                ELF = new byte[allocstart+allocsize];
                Array.Copy(input, allocoffset, ELF, allocstart, allocsize);
                //File.WriteAllBytes("test.bin", ELF);
            }
            else if(mwo3paths != null)
            {
                mwo3mode = true;
                var mwlist = new List<MWo3>();
                int alocsizeMwos = 0;
                foreach(var mwo in mwo3paths)
                {
                    var mwo3 = new MWo3(mwo);
                    mwlist.Add(mwo3);
                    alocsizeMwos += mwo3.MWo3Data.Length + mwo3.AllocationAddress;
                }
                MWo = mwlist.ToArray();
                ELF = new byte[allocstart + allocsize + alocsizeMwos];
                Array.Copy(input, allocoffset, ELF, allocstart, allocsize);//Alocate ELF
                foreach (var mwo3 in MWo)
                    Array.Copy(mwo3.MWo3Data, 0, ELF, mwo3.AllocationAddress, mwo3.MWo3Data.Length);//Alocate Mwo3(s)

            }

            ElfFilename = path;
            InternalName = Path.GetFileName(path);

        }
        public void GetXML(out Dictionary<int, int> tables, out Dictionary<string, string[]> filenames)
        {
            var tablesinp = new Dictionary<int, int>();
            var tablesnam = new List<string>();
            var dict = new Dictionary<string, string[]>();
            #region Leitor XML
            Stream xml = File.OpenText($"elfbase.xml").BaseStream;
            XmlDocument reader = new XmlDocument();
            reader.Load(xml); //Carregando o arquivo

            XmlNodeList xnList = reader.GetElementsByTagName(InternalName);

            int tablec = 0;
            foreach (XmlNode xn in xnList)
            {
                foreach (XmlNode node in xn.ChildNodes)
                {
                    switch (node.Name.ToLower())
                    {
                        case "virtual":
                            virtualmem = Convert.ToBoolean(node.InnerText);
                            break;
                        case "table":
                            if (node.Attributes.Count > 0)
                            {
                                tablesnam.Add(node.Attributes["name"].Value);
                            }
                            else
                            {
                                tablesnam.Add("Tabela " + tablec.ToString());
                            }
                            string[] values = node.InnerText.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                            int start = Convert.ToInt32(values[0], 16) + shift;
                            int end = Convert.ToInt32(values[1], 16) + shift;
                            if (!tablesinp.ContainsKey(start) &&
                                !tablesinp.ContainsValue(end))
                                tablesinp.Add(start, end);
                            tablec++;//Add count table
                            break;
                        case "filelist":
                            
                            foreach (XmlElement pointCoord in node.SelectNodes("Pasta"))
                            {
                                if (pointCoord != null)
                                {
                                    string[] files = pointCoord.InnerText.Split(new string[] { "\r\n", "" }, StringSplitOptions.RemoveEmptyEntries);
                                    dict.Add(pointCoord.Attributes["name"].Value, files);
                                }
                            }
                            //foreach (XmlNode pasta in xnList)
                            //{
                            //    string[] files = pasta.InnerText.Split(new string[] { "\r\n", "" }, StringSplitOptions.RemoveEmptyEntries);
                            //    dict.Add(xn.Attributes["name"].Value, files);
                            //}

                            break;
                    }
                }
            }
            #endregion  
            tables = tablesinp;
            filenames = dict;
        }
        public static bool CheckSTR(byte[] input, int position)
        {
            bool valid = false;
            try
            {
                if (input[position - 1] == 0&& input[position] != 0)
                    valid = true;
            }
            catch (Exception) { }
            return valid;
        }

        public Dictionary<string, string> ElfVersion = new Dictionary<string, string>()
        {
            {"SLUS","Estados Unidos" },
            {"SCAJ","América/Japão" },
            {"SLAJ","América/Japão" },
            {"SCUS","Estados Unidos" },
            {"SCPS","Japão" },
            {"SLPM","Japão" },
            {"SLPS","Japão" },
            {"SLES","Europa" },
            {"SCED","Europa" },
            {"SCES","Europa" },
            {"SCKA","Coreia" },
            {"SLKA","Coreia" },
            {"SCBR","Brasil" },
            {"SLBR","Brasil" }
        };
        public string GetElfVersion()
        {
            string reto = "";
            ElfVersion.TryGetValue(Path.GetFileName(ElfFilename).Substring(0, 4), out reto);
            return reto;
        }
        public byte[] GetEdited()
        {
            if(virtualmem)
            {
                Array.Copy(ELF, allocstart, InputElf, allocoffset, allocsize);
            }
            else if(mwo3mode)
            {
                Array.Copy(ELF, allocstart, InputElf, allocoffset, allocsize);
                foreach(var mw in MWo)
                    Array.Copy(ELF, mw.AllocationAddress, mw.MWo3Data,0, mw.MWo3Data.Length);
            }
            return InputElf;
        }
    }
    public class Pointer
    {
        public int ELFoffset;
        public int value;
        public int ELFpointer;
        public Pointer() { }
        public Pointer(int pointer, int magic, bool virt, bool mwo=false)
        {
            ELFpointer = pointer;
            if (virt ||mwo)
                value = pointer;
            else
                value = pointer - magic;
        }
        public static int GetPosition(byte[] inpt, int start, int search)
        {
            int offset = 0;
            for (int i = start; (int)ReadUInt(inpt, i, Int.UInt32) == search; offset = i) ;
            return offset;
        }
    }
}
