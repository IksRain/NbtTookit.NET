namespace StreamWrapper;

public enum NbtKind : byte
{
    EndObject = 0,
    Byte,
    Short,
    Int,
    Long,
    Float,
    Double,
    ByteArray,
    String,
    List,
    Object,
    IntArray,
    LongArray
}

public static class NbtKindExtensions
{
    extension(NbtKind kind)
    {
        public bool IsPrimitive
            => kind is
                NbtKind.Byte or
                NbtKind.Short or
                NbtKind.Int or
                NbtKind.Long or
                NbtKind.Float or
                NbtKind.Double or
                NbtKind.String;
        public bool IsCollection
            => kind is
                NbtKind.ByteArray or
                NbtKind.List or
                NbtKind.IntArray or
                NbtKind.LongArray;
        public bool IsObject => kind is NbtKind.Object;
    }
}