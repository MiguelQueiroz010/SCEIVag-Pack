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
        public static int IndexOff(this byte[] arrayToSearchThrough, byte[] patternToFind, int index = 0)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = index; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        public static List<int> AllIndexesOf(this byte[] value, uint str)
        {
            List<int> indexes = new List<int>();
            for (int index = 0; ;)
            {
                int idx = value.IndexOff(BitConverter.GetBytes(str), index);
                if (idx == -1)
                    return indexes;
                else
                {
                    indexes.Add(idx);
                    index = idx + 1;
                }
            }
            return indexes;
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
        public static byte[] ReadBrokeString(byte[] file, int offset, int breakeroff)
        {
            var sequence = new List<byte>();
            var memory = new MemoryStream(file);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            while (reader.BaseStream.Position != breakeroff)
            {
                sequence.Add(reader.ReadByte());
            }
            reader.Close();
            memory.Close();
            return sequence.ToArray();
        }
        public static byte[] ReadStrBlock(byte[] file, int offset)
        {
            var k = new List<byte>();
            int offs = offset;
            while (file[offs] != 0)//Catch the characters
            {
                k.Add(file[offs]);
                offs++;
            }
            while (file[offs] == 0)//Counts remaining free space
            {
                k.Add(0);
                offs++;
            }

            return k.ToArray();
        }
        public static byte[] ReadString(byte[] file, int offset)
        {
            var k = new List<byte>();
            int offs = offset;
            while (file[offs] != 0)//Catch the characters
            {
                k.Add(file[offs]);
                offs++;
            }

            return k.ToArray();
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
        public static int GetFreeSpace(byte[] input, int minimum, int start = 0, int end = 0)
        {
            var spacefree = 0;
            minimum += 2;
            if (end == 0)
                end = input.Length;
            for (int i = start; spacefree < minimum && i < end;)
            {
                byte[] find = ReadBlock(input, (uint)i, (uint)minimum);
                if (find.All(x => x == 0))
                    spacefree = i;
                i++;
            }
            spacefree++;
            if (spacefree == 1)
                spacefree = -1;
            return spacefree;
        }
        public static int GetFreeSpace(byte[] input, int minimum, Dictionary<int, int> FreeSpaces, int padd = 0)
        {
            var spacefree = 0;
            minimum += 2;
            foreach (var table in FreeSpaces)
            {
                int start = table.Key;
                while (start % padd != 0)
                    start++;
                for (int i = start; spacefree < minimum && i < table.Value;)
                {
                    byte[] find = ReadBlock(input, (uint)i, (uint)minimum);
                    if (find.All(x => x == 0))
                    {
                        if (input[i - 1] == 0)
                        {
                            spacefree = i;
                            return spacefree;
                        }
                        else
                        {
                            i += padd;
                        }
                    }
                    else
                    {
                        i += padd;
                    }
                }
            }
            return -1;
        }
        public static void CleanStrSpace(byte[] input, int offsetstart)
        {
            int i = offsetstart;
            while (input[i] != 0)
            {
                input[i] = 0;
                i++;
            }
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
