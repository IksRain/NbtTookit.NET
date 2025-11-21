using System.Buffers;
using System.Text;
using StreamWrapper;

namespace TestProject1;

public class ReaderTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void MinimalTest()
    {
        using var fileStream = File.OpenRead("TestFile\\Minimal.nbt");
        var reader = new Reader(fileStream, Option.Java);
        
        var readTag = reader.ReadTag();
        
        Assert.That(readTag, Is.EqualTo(NbtKind.Object));
        Console.WriteLine(readTag.ToString());

        var format = new string(reader.ReadString());
        Assert.That(format, Is.EqualTo("Level"));
        Console.WriteLine(format);
        
        readTag = reader.ReadTag();
        Assert.That(readTag, Is.EqualTo(NbtKind.EndObject));
        Console.WriteLine(readTag.ToString());
        
        Assert.Pass();
    }
}