using System.Text;

namespace MURQ.Common;

public static class CommonEncodings
{
    static CommonEncodings()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Encoding UTF8 => Encoding.UTF8;
    public static Encoding Windows => Encoding.GetEncoding("windows-1251");
    public static Encoding DOS => Encoding.GetEncoding("cp866");
}
