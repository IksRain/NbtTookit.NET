using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StreamWrapper;

public interface INbtReader: IDisposable
{
    /// <summary>
    /// 加载当前对象的HEADER(缓存)
    /// </summary>
    /// <returns>当前处理对象的HEADER</returns>
    NbtKind Header { get; }
    /// <summary>
    /// 加载当前对象的NAME(缓存) 长度为0说明为空
    /// </summary>
    /// <returns>当前处理对象的NAME若当前对象无名称返回0长度的Span</returns>
    ReadOnlySpan<char> PropertyName { get; }
    /// <summary>
    /// 加载简单类型的Data
    /// </summary>
    ref ReadOnlyDataHandle Data { get; }
    /// <summary>
    /// 跳过HEADER所属全部DATA
    /// </summary>
    void Skip();
    
}

public struct ReadOnlyDataHandle()
{
    public required ulong RemainingLength { get; set; }
    public required Stream Stream { private get; init; } 

    public void SetDisposed()=>IsDisposed = false;
    public bool IsDisposed { get; private set; } = false;
    

    public required bool ShouldNumberReverseEndian { private get;init; } = false;

    /// <summary>
    /// 将DataSpan直接转化为 <c>unmanaged T</c> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    public unsafe T ReadAs<T>() where T : unmanaged,allows ref struct
    {
        // 长度超出
        if(RemainingLength < (ulong)sizeof(T))
            throw new InvalidCastException($"cannot Cast To {nameof(T)} because sizeof {nameof(T)} is greater than  {nameof(RemainingLength)}:{RemainingLength}");
        
        // 直接写入
        Unsafe.SkipInit(out T value);
        var span = new Span<byte>(Unsafe.AsPointer(in value), sizeof(T));
        Stream.ReadExactly(span);
        // 减少剩余长度
        RemainingLength -= (ulong)span.Length;
        return value;
    }
    [SkipLocalsInit]
    public unsafe TNumber ReadNumber<TNumber>() where TNumber : unmanaged,INumber<TNumber>
    {
        // 长度超出
        if(RemainingLength < (ulong)sizeof(TNumber))
            throw new InvalidCastException($"cannot Read number because sizeof {nameof(TNumber)} is greater than  {nameof(RemainingLength)}:{RemainingLength}");
        
        Span<byte> span = stackalloc byte[sizeof(TNumber)];
        Stream.ReadExactly(span);
        // 如果端序正确，直接返回
        if(ShouldNumberReverseEndian is false || sizeof(TNumber) is 1) return *(TNumber*)span.GetPinnableReference();
        // 否则，反转端序
        span.Reverse();
        return *(TNumber*)span.GetPinnableReference();
    }

    public void ReadExactly(Span<byte> span)
    {
        // 长度超出
        if(RemainingLength < (ulong)span.Length)
            throw new InvalidCastException($"cannot Load To Buffer because Span.Lenght is greater than  {nameof(RemainingLength)}:{RemainingLength}");
        
        Stream.ReadExactly(span);
    }
}

public ref struct DataHandle
{
    
}