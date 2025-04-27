using PdfSharpCore.Fonts;
using System.Reflection;

public class FontResolver : IFontResolver
{
    private static readonly byte[] FontData;

    static FontResolver()
    {
        // Aquí cargas el archivo TTF embebido
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "LiberationSans-Regular.ttf");
        FontData = File.ReadAllBytes(fontPath);
    }

    public string DefaultFontName => "LiberationSans"; // <<<<<<<< ESTO FALTABA

    public byte[] GetFont(string faceName)
    {
        return FontData;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        return new FontResolverInfo("LiberationSans");
    }
}
