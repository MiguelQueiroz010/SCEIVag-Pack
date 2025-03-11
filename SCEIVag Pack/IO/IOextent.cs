using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// Classe de extens�es de tipos de dados conhecidos, para usos diversos,
/// convers�es e leituras.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso n�o comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo � poder.
/// Novembro/2021
public static class IOextent
{
    #region Leitores de Dados

    #region BITS
    /// <summary>
    /// L� um bit de um determinado array[byte].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura(0-7).</param>
    /// <returns>Bool.</returns>
    public static bool ReadBit(this byte array, int offset)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra[offset];
    }

    /// <summary>
    /// L� todos os bits de um determinado array[byte].
    /// </summary>
    /// <returns>BitArray.</returns>
    public static BitArray ReadBits(this byte array)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra;
    }

    /// <summary>
    /// Converte todos os bits do array em uma string.
    /// </summary>
    /// <returns>string.</returns>
    public static string ToSTR(this BitArray array)
    {
        string result = "";
        foreach (var inte in array)
            result += Convert.ToInt32(inte).ToString();
        result = new string(result.Reverse().ToArray());
        return result;
    }
    #endregion

    #region BYTES
    /// <summary>
    /// L� uma quantia espec�fica de bytes de um determinado array[buffer].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this byte[] array, int offset, int size)
    {
        byte[] result = array.Skip(offset).ToArray().Take(size).ToArray();
        return result;
    }

    /// <summary>
    /// L� uma quantia espec�fica de bytes de um determinado array[buffer] para Prog.Entry, exclusivo do IECS[BHD SCEI].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadEntryBytes(this byte[] array, int offset)
    {
        var result = new List<byte>();
        result.AddRange(Enumerable.Range(
            0, (array[offset + 4] * array[offset + 5]) + 0x24
            ).Select(x => array[x+ offset]));
        return result.ToArray();
    }

    /// <summary>
    /// L� uma quantia espec�fica de bytes de um fluxo determinado e avan�a a posi��o na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this Stream stream, int offset, int size)
    {
        var result = new List<byte>();
        long oldoffset = stream.Position;
        stream.Position = (long)offset;

        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte()); stream.Flush();

        stream.Position = oldoffset;
        return result.ToArray();
    }
    public static List<byte[]> ReadBlocks(this Stream stream, int quant)
    {
        var ObjBlock = new List<byte>();
        bool end = false;
        while (end==false)
        {
            if (stream.Position >= stream.Length)
                end = true;
            else
            {
                if (stream.ReadBytes((int)stream.Position, 5).ConvertTo(Encoding.Default) != "[OBJ]")
                    ObjBlock.Add((byte)stream.ReadByte());
                else
                    end = true;
            }
        }

        var result = new List<byte[]>();
        var objstream = new MemoryStream(ObjBlock.ToArray());
        for(int i =0;i<quant;i++)
        {
            uint coint = 0;
            uint oldoffs = (uint)objstream.Position;
            for (; objstream.ReadBytes(4, false).ConvertTo(Encoding.Default) != "<BL>";
             coint+=4)
            {
            }
            objstream.Position = oldoffs;
            byte[] block = objstream.ReadBytes((int)coint);
            objstream.Position += 4;
            if(!result.Contains(block))
                result.Add(block);
        }
        return result;
    }
    /// <summary>
    /// L� uma quantia espec�fica de bytes de um fluxo determinado e avan�a a posi��o na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this Stream stream, int size, bool maintain = false)
    {
        long pos = stream.Position;
        var result = new List<byte>();
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        if (maintain)
            stream.Position = pos;
        return result.ToArray();
    }

    /// <summary>
    /// L� uma quantia espec�fica de bytes de um fluxo determinado e avan�a a posi��o na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytesTM2(this Stream stream)
    {
        var result = new List<byte>();
        uint size = stream.ReadUInt((int)stream.Position, 32);
        for (int i = 0; i < (int)size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }
    /// <summary>
    /// L� uma quantia espec�fica de bytes de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytesLong(this Stream stream, long offset, int size)
    {
        var result = new List<byte>();
        stream.Position = offset;
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }

    /// <summary>
    /// L� uma se��o de arquivos de um fluxo e posi��o determinado[Stream].
    /// </summary>
    /// <param name="lba">Posi��o LBA para iniciar a leitura.</param>
    /// <param name="size">Tamanho dos setores.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadFiles(this Stream stream, int lba, int size = 2048)
    {
        var result = new List<byte>();
        int offset = lba * size;
        bool stop = false;
        while(stop==false)
        {
            stream.Position = offset;
            byte sizex = (byte)stream.ReadByte();
            if (sizex == 0)
                stop = true;
            if (stop != true)
            {
                stream.Position = offset;
                byte[] section = stream.ReadBytes(offset, sizex);
                result.AddRange(section);
                offset += section.Length;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// L� um setor de um array de bytes[buffer].
    /// </summary>
    /// <param name="lba">Localiza��o de bloco l�gico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this byte[] array, int lba, int size = 2048)
    {
        byte[] result = array.ReadBytes(lba * size, size);
        return result;
    }

    /// <summary>
    /// L� um setor de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="lba">Localiza��o de bloco l�gico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this Stream stream, int lba, int size = 2048)
    {
        byte[] result = stream.ReadBytes(lba * size, size);
        return result;
    }
    #endregion

    #endregion

    #region Escrita de Dados

    /// <summary>
    /// Escreve uma quantia espec�fica de bytes em um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para iniciar a leitura.</param>
    /// <param name="offsetwrite">Posi��o para iniciar a escrita.</param>
    /// <param name="size">Tamanho da escrita em bytes.</param>
    public static void WriteBytes(this Stream stream, Stream write, long offsetwrite, long offset, long size)
    {
        stream.Position = offsetwrite;
        write.Position = offset;
        int c = 0;
        while(c<size)
        {
            stream.WriteByte((byte)write.ReadByte());
            c++;
        }
    }

    #endregion

    #region Leitores de Inteiros
    /// <summary>
    /// L� um Inteiro sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posi��o para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this byte[] array, int offset, int bits, bool bigendian = false)
    {
        var reader = new BinaryReader(new MemoryStream(array));
        reader.BaseStream.Position = offset;
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = (uint)reader.ReadByte();
                break;

            case 16:
                result = reader.ReadUInt16();
                break;

            case 32:
                result = reader.ReadUInt32();
                break;
        }
        reader.Close();
        result = bigendian ? BitConverter.ToUInt32(BitConverter.GetBytes(result).Reverse().ToArray(),0) : BitConverter.ToUInt32(BitConverter.GetBytes(result), 0);
        return result;
    }
    /// <summary>
    /// L� um float de 32 bits.[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para ler o inteiro.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>N�mero de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this Stream strean, int offset, bool bigendian = false)
    {
        strean.Position = offset;
        byte[] bitssx = strean.ReadBytes(4);
        if (bigendian == true)
            Array.Reverse(bitssx);
        Single result = 0;
        result = BitConverter.ToSingle(bitssx, 0);
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// L� um float de 32 bits e avan�a a posi��o em 4 bytes.[Stream].
    /// </summary>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>N�mero de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this Stream strean, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes(4);
        if (bigendian == true)
            Array.Reverse(bitssx);
        Single result = 0;
        result = BitConverter.ToSingle(bitssx, 0);
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// L� um Inteiro sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this Stream strean, int offset, int bits, bool bigendian = false)
    {
        strean.Position = offset;
        byte[] bitssx = strean.ReadBytes((int)(bits / 8));
        uint result = 0;
        switch(bits)
        {
            case 8:
                result = bitssx[0];
                break;
            case 16:
                result = bigendian ? BitConverter.ToUInt16(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt16(bitssx, 0);
                break;
            case 32:
                result = bigendian ? BitConverter.ToUInt32(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt32(bitssx, 0);
                break;
        }
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// L� um Inteiro sem sinal do fluxo[Stream] e avan�a a posi��o a quantia de bytes por bits.
    /// </summary>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this Stream strean, int bits, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes((int)(bits / 8));
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = bitssx[0];
                break;
            case 16:
                result = bigendian ? BitConverter.ToUInt16(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt16(bitssx, 0);
                break;
            case 32:
                result = bigendian ? BitConverter.ToUInt32(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt32(bitssx, 0);
                break;
        }
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// L� um Long sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posi��o para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this byte[] array, int offset, bool bigendian = false)
    {
        ulong result = bigendian ? BitConverter.ToUInt64(array.ReadBytes(offset, 8).Reverse().ToArray(), 0) : BitConverter.ToUInt64(array.ReadBytes(offset, 8), 0);
        return result;
    }

    /// <summary>
    /// L� um Long sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codifica��o BigEndian ao inv�s de LittleEndian padr�o.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this Stream strean, int offset, bool bigendian = false)
    {
        ulong result = bigendian ? BitConverter.ToUInt64(strean.ReadBytes(offset, 8).Reverse().ToArray(), 0) : BitConverter.ToUInt64(strean.ReadBytes(offset, 8), 0);
        return result;
    }
    #endregion

    #region Leitores de Texto

    /// <summary>
    /// L� um array de bytes enquanto diferente de uma quebra no array(algum byte)[buffer].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this byte[] file, int offset, byte breakeroff=0 )
    {
        byte[] result = file.Skip(offset).ToArray().TakeWhile(x=>x!=breakeroff).ToArray();
        return result;
    }

    /// <summary>
    /// L� um array de bytes enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this Stream file, int offset, byte breakeroff = 0)
    {
        var result = new List<byte>();
        file.Position = offset;
        while(file.ReadBytes((int)file.Position,1)[0]!=breakeroff)
        {
            result.Add((byte)file.ReadByte());
        }
        return result.ToArray();
    }

    //Stream
    /// <summary>
    /// L� uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="encoding">Codifica��o de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// L� uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Array
    /// <summary>
    /// L� uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="encoding">Codifica��o de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// L� uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posi��o para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padr�o � o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Conversores
    /// <summary>
    /// Converte uma string para uma codifica��o espec�fica[string].
    /// </summary>
    /// <param name="encoding">Codifica��o de sa�da.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this string file, Encoding encoding)
    {
        return encoding.GetString(encoding.GetBytes(file));
    }

    /// <summary>
    /// Converte uma string para uma codifica��o espec�fica[string].
    /// </summary>
    /// <param name="encoding">Codifica��o de sa�da.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this byte[] file, Encoding encoding)
    {
        return encoding.GetString(file);
    }
    #endregion

    
}
