using StreamWrapper;
using StreamWriter = StreamWrapper.StreamWriter;

namespace TestProject1;

public class StreamWriterTest
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void MinimalTest()
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream,Option.Java);
        
        writer.PutTag(NbtKind.Object);
        writer.WriteString("Level");
        writer.PutTag(NbtKind.EndObject);
        
        if (memoryStream.ToArray() is not [0x0A, 0x00, 0x05, 0x4C, 0x65, 0x76, 0x65, 0x6C, 0x00])
        {
            Assert.Fail();
        }
        Assert.Pass();
    }
}