using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/BrandingConfig", fileName = "BrandingConfig")]
public class RavelBranding : ScriptableObject
{
    //Height of topbar banner
    public const float BANNER_HEIGHT = 100f;
    
    //Font size for titles
    public const int FONT_TITLE = 30;
    
    //small spacing indent for horizontal offset
    public const float INDENT_SMALL = 10f;
    
    //spacing to highlight separation of two items.
    public const float SPACING_SMALL = 10f;
    public const float SPACING_MED = 20f;
    public const float SPACING_BIG = 100f;
    
    //size of small horizontal button (save and copy for instance)
    public const float TOOLBAR_BTN_SQUARE = 20f;
    public const float TOOLBAR_BTN_TXT_SMALL = 75f;
    
    public const float INT_FIELD_999 = 25f;
    public const float TXT_LAB_MICRO = 40f;
    public const float TXT_BTN_SMALL = 100f;
    public const float TXT_BTN_MED = 150f;
    public const float LABEL_MED = 150f;
    
    [Tooltip("This is the default banner that is shown in the top of editor windows.")]
    public Texture2D banner;
    [Tooltip("Zooms in on this point of the banner, use decimals range 0.0 t/m 1.0.")]
    public Vector2 bannerPOI = new Vector2(0.5f, 0.5f);
    [Tooltip("This default transparant logo is drawn over other images, used as banners.")]
    public Texture2D overlayLogo;
    [Tooltip("Small square logo.")]
    public Texture2D logoSquare;
}
