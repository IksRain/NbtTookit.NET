using System.Buffers;
using System.Text;

namespace StreamWrapper;

public record struct Option(
    bool IsLittleEndian,
    Encoding Encoding,
    ArrayPool<byte> BufferPool,
    ArrayPool<char> CharBufferPool)
{
    public static Option Java { get; } = new Option(
        IsLittleEndian: false,
        Encoding: Encoding.UTF8,
        BufferPool: ArrayPool<byte>.Shared,
        CharBufferPool: ArrayPool<char>.Shared);
    
}