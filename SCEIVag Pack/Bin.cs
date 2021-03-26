using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SCEIVag_Pack
{
    public static class Bin
    {
        public static byte[] ReadBlock(byte[] s,uint offset, uint size)
        {
                byte[] bytes = new byte[size];
                var memory = new MemoryStream(s);
                var reader = new BinaryReader(memory);
                reader.BaseStream.Position = offset;
                bytes = reader.ReadBytes((int)size);
                reader.Close();
                memory.Close();
                return bytes;
            
        }
        public static byte[] ReadSequence(byte[] file, int offset, string breaker)
        {
            var sequence = new List<byte>();
            var memory = new MemoryStream(file);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            uint pointer = reader.ReadUInt32();
            reader.Close();
            memory.Close();
            for (uint i = pointer; file[i].ToString("X2") + file[i + 1].ToString("X2") != "0080"; i += 2)
            {
                //MessageBox.Show(file[i].ToString("X2") + file[i + 1].ToString("X2"));
                sequence.Add(file[i]);
                sequence.Add(file[i + 1]);
            }
            return sequence.ToArray();
        }
        public static ulong ReadUInt(byte[] s, int offset, Int type)
        {
            ulong retur = 0;
            var memory = new MemoryStream(s);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            switch (type)
            {
                case Int.UInt16:
                    retur = reader.ReadUInt16();
                    break;
                case Int.UInt32:
                    retur = reader.ReadUInt32();
                    break;
                case Int.UInt64:
                    retur = reader.ReadUInt64();
                    break;
            }
            reader.Close();
            memory.Close();
            return retur;
        }
        public enum Int
        {
            Int16,
            Int32,
            Int64,
            UInt16,
            UInt32,
            UInt64
        };
    }
}
