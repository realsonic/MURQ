using System.Text;

namespace QuestSources.FileSource;
public class FileEncodingDetector
{
    static FileEncodingDetector()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static async ValueTask<Encoding> DetectAsync(string filePath, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[5];
        await using FileStream fileStream = new(filePath, FileMode.Open);
        await fileStream.ReadAsync(buffer.AsMemory(), cancellationToken);

        return DetectEncoding(buffer);
    }

    private static Encoding DetectEncoding(byte[] buffer) => buffer switch
    {
        [0xEF, 0xBB, 0xBF, ..] => Encoding.UTF8,
        [0xFE, 0xFF, ..] => Encoding.Unicode,
        [0, 0, 0xFE, 0xFF, ..] => Encoding.UTF32,
        [0xFF, 0xFE, ..] => Encoding.GetEncoding(1200), // 1200 utf-16 Unicode
        _ => Encoding.GetEncoding("Windows-1251")
    };
}