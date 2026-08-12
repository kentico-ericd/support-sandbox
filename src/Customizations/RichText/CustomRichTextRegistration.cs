using Kentico.Xperience.Admin.Base.Forms;

using DancingGoat.Customizations.RichText;

[assembly: RegisterRichTextEditorConfiguration(
    CustomRichTextEditorConfiguration.IDENTIFIER,
    typeof(CustomRichTextEditorConfiguration),
    CustomRichTextEditorConfiguration.DISPLAY_NAME)]
namespace DancingGoat.Customizations.RichText;

public class CustomRichTextEditorConfiguration : RichTextEditorConfiguration
{
    private const string CONFIGURATION_PATH = "Customizations\\RichText\\CustomRichTextConfiguration.json";


    public const string IDENTIFIER = "Sandbox.CustomRichTextConfiguration";


    public const string DISPLAY_NAME = "Custom Sandbox editor configuration";


    public CustomRichTextEditorConfiguration() : base(CONFIGURATION_PATH, "DancingGoat")
    {
    }
}
