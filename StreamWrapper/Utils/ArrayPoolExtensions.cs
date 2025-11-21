using System.Buffers;
using System.Runtime.CompilerServices;

namespace StreamWrapper;

internal static class ArrayPoolExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayOwner<T> Use<T>(this ArrayPool<T> pool, int lenght )
    {
        return new ArrayOwner<T>(pool.Rent(lenght),pool);
    }
    
    public readonly record struct ArrayOwner<T>(T[] Array,in ArrayPool<T> Owner) : IDisposable
    {
        public void Dispose()
        {
            Owner.Return(Array);
        }
    }
}