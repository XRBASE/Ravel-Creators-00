using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/BrandingConfig", fileName = "BrandingConfig")]
public class RavelBranding : ScriptableObject
{
    public Texture2D banner;
    [Tooltip("Zooms in on this point of the banner, use decimals range 0.0 t/m 1.0")]
    public Vector2 bannerPOI = new Vector2(0.5f, 0.5f);
}
