using System.Buffers.Binary;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace StreamWrapper;

public struct Reader(Stream stream, Option option) : IDisposable
{
    private readonly Decoder _decoder = option.Encoding.GetDecoder();
    private char[] _charSpanBuffer = option.CharBufferPool.Rent(256);

    private void AllocCharsBuffer(int size)
    {
        if (_charSpanBuffer.Length >= size) return;
        option.CharBufferPool.Return(_charSpanBuffer);
        _charSpanBuffer = option.CharBufferPool.Rent(size);
    }

    public NbtKind ReadTag()
    {
        return (NbtKind)ReadByte();
    }

    public byte ReadByte()
    {
        var read = stream.ReadByte();
        if (read == -1) throw new EndOfStreamException();
        return (byte)read;
    }

    public short ReadShort()
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        stream.ReadExactly(span);
        return option.IsLittleEndian ? ReadInt16LittleEndian(span) : ReadInt16BigEndian(span);
    }

    public int ReadInt()
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        stream.ReadExactly(span);
        return option.IsLittleEndian ? ReadInt32LittleEndian(span) : ReadInt32BigEndian(span);
    }

    public long ReadLong()
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        stream.ReadExactly(span);
        return option.IsLittleEndian ? ReadInt64LittleEndian(span) : ReadInt64BigEndian(span);
    }

    public float ReadFloat()
    {
        Span<byte> span = stackalloc byte[sizeof(float)];
        stream.ReadExactly(span);
        return option.IsLittleEndian ? ReadSingleLittleEndian(span) : ReadSingleBigEndian(span);
    }

    public double ReadDouble()
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
        stream.ReadExactly(span);
        return option.IsLittleEndian ? ReadDoubleLittleEndian(span) : ReadDoubleBigEndian(span);
    }

    /// <exception cref="ArgumentOutOfRangeException">When the string length is negative or zero.</exception>
    /// <returns> A mutable <c>ReadOnlySpan&lt;char&gt;</c>, if you need an immutable string use
    /// <c>ReadOnlySpan&lt;T&gt;.CopyTo()</c>
    /// or <c>new string()</c></returns>
    public ReadOnlySpan<char> ReadString()
    {
        var strLen = ReadShort();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(strLen, "string lenght cannot be negative or zero");

        AllocCharsBuffer(strLen);
        using var use = option.BufferPool.Use(strLen);
        var span = use.Array.AsSpan(0, strLen);
        stream.ReadExactly(span);

        var charsLength = _decoder.GetChars(span, _charSpanBuffer, flush: true);
        return _charSpanBuffer.AsSpan(0, charsLength);
    }

    public void Dispose()
    {
        option.CharBufferPool.Return(_charSpanBuffer);
    }
}