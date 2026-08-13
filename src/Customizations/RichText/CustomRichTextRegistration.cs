using Kentico.Xperience.Admin.Base.Forms;

using DancingGoat.Customizations.RichText;

[assembly: RegisterRichTextEditorConfiguration(
    CustomRichTextRegistration.IDENTIFIER,
    typeof(CustomRichTextRegistration),
    CustomRichTextRegistration.DISPLAY_NAME)]
namespace DancingGoat.Customizations.RichText;

public class CustomRichTextRegistration : RichTextEditorConfiguration
{
    private const string CONFIGURATION_PATH = "Customizations\\RichText\\CustomRichTextConfiguration.json";


    public const string IDENTIFIER = "Sandbox.CustomRichTextConfiguration";


    public const string DISPLAY_NAME = "Custom Sandbox editor configuration";


    public CustomRichTextRegistration() : base(CONFIGURATION_PATH, "DancingGoat")
    {
    }
}
