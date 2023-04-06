using UnityEngine;

/// <summary>
/// Settable settings for styling of the whole editor tool.
/// </summary>
[CreateAssetMenu(menuName = "Ravel/BrandingConfig", fileName = "BrandingConfig")]
public class RavelBranding : ScriptableObject
{
    [Tooltip("Height of all top-bar image banners")]
    public float bannerHeight = 100f;
    [Tooltip("Font for title text")]
    public const int titleFont = 30;
    
    [Space]
    [Tooltip("This is the default banner that is shown in the top of editor windows.")]
    public Texture2D banner;
    [Tooltip("Zooms in on this point of the banner, use decimals range 0.0 t/m 1.0.")]
    public Vector2 bannerPOI = new Vector2(0.5f, 0.5f);
    
    [Space]
    [Tooltip("This default transparant logo is drawn over other images, used as banners.")]
    public Texture2D overlayLogo;
    [Tooltip("Small square logo.")]
    public Texture2D logoSquare;

    [Space]
    [Tooltip("Used for checks that pass")]
    public Texture2D passCheck;
    [Tooltip("Used for checks that fail")]
    public Texture2D failCheck;
}
