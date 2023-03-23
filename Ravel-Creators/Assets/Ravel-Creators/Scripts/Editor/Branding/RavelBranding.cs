using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/BrandingConfig", fileName = "BrandingConfig")]
public class RavelBranding : ScriptableObject
{
    public const float BANNER_HEIGHT = 100f;
    
    public const int FONT_TITLE = 30;
    
    public const float INDENT_SMALL = 10f;
    public const float SPACING_SMALL = 20f;
    public const float HORI_BTN_SMALL = 100f;
    
    public Texture2D banner;
    [Tooltip("Zooms in on this point of the banner, use decimals range 0.0 t/m 1.0")]
    public Vector2 bannerPOI = new Vector2(0.5f, 0.5f);
    public Texture2D overlayLogo;
    
    public Texture2D daan;
}
