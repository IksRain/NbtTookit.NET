using System.Buffers.Binary;
using System.Text;

namespace StreamWrapper;
using static BinaryPrimitives;
public struct Writer(Stream stream, Option option)
{
    private Encoder _encoder = option.Encoding.GetEncoder();
    public void PutTag(NbtKind tag)=>stream.WriteByte((byte)tag);
    public void WriteByte(byte value)=>stream.WriteByte(value);

    public void WriteShort(short value)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        if (option.IsLittleEndian)
            WriteInt16LittleEndian(span, value);
        else
            WriteInt16BigEndian(span, value);
        stream.Write(span);
    }
    public void WriteInt(int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        if (option.IsLittleEndian)
            WriteInt32LittleEndian(span, value);
        else
            WriteInt32BigEndian(span, value);
        stream.Write(span);
    }

    public void WriteLong(long value)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        if (option.IsLittleEndian)
            WriteInt64LittleEndian(span, value);
        else
            WriteInt64BigEndian(span, value);
        stream.Write(span);
    }

    public void WriteFloat(float value)
    {
        Span<byte> span = stackalloc byte[sizeof(float)];
        if (option.IsLittleEndian)
            WriteSingleLittleEndian(span, value);
        else
            WriteSingleBigEndian(span, value);
        stream.Write(span);
    }
    public void WriteDouble(double value)
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
        if (option.IsLittleEndian)
            WriteDoubleLittleEndian(span, value);
        else
            WriteDoubleBigEndian(span, value);
        stream.Write(span);
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        var byteCount = _encoder.GetByteCount(value, true);
        WriteShort((short)byteCount);
        using var buffer = option.BufferPool.Use(byteCount);
        _encoder.GetBytes(value, buffer.Array, true);
        stream.Write(buffer.Array.AsSpan(0, byteCount));
    }
    
    public void FillWith(ReadOnlySpan<byte> fillBytes)
    {
        stream.Write(fillBytes);
    }
    
}