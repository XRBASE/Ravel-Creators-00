using UnityEngine;

/// <summary>
/// GUI style reference class for constant sizing and editor button style descriptors 
/// </summary>
public static class RavelEditorStying
{
	/// <summary>
	/// Width of an integer field, supports numbers up to 999 (or negative down to -99) 
	/// </summary>
	public const float INT_LAB_WIDTH = 25f;
    
	/// <summary>
	/// 10px GUI spacing, used for small spacing between (related)elements.
	/// </summary>
	public const int GUI_SPACING_MICRO = 10;
    
	/// <summary>
	/// 20px GUI spacing, used for square toobar buttons and ui indents.
	/// </summary>
	public const int GUI_SPACING_MILLI = 20;
    
	/// <summary>
	/// 40px GUI spacing, used for small text labels or buttons.
	/// </summary>
	public const int GUI_SPACING_CENTI = 40;
    
	/// <summary>
	/// 75px GUI spacing, used for text labels and buttons with two or three words.
	/// </summary>
	public const int GUI_SPACING_DECI = 75;
    
	/// <summary>
	/// 100px GUI spacing, used for bigger buttons.
	/// </summary>
	public const int GUI_SPACING = 100;
    
	/// <summary>
	/// 150px GUI spacing, used for bigger (non-stretch) text buttons, containing long words or small scentences.
	/// </summary>
	public const int GUI_SPACING_DECA = 150;
	
	public static GUIStyle imageBtn;
	public static GUIStyle txtBtnSmall;

	static RavelEditorStying() {
		imageBtn = new GUIStyle()
		{
			imagePosition = ImagePosition.ImageOnly,
			fixedWidth = GUI_SPACING_MILLI,
			padding = new RectOffset(0,0,2,0),
                
		};
            
		txtBtnSmall = new GUIStyle("Command")
		{
			//fontSize = 12,
			alignment = TextAnchor.MiddleCenter,
			imagePosition = ImagePosition.ImageAbove,
			fontStyle = FontStyle.Normal,

			fixedWidth = GUI_SPACING_DECI,
			fixedHeight = GUI_SPACING_MILLI,
			padding = new RectOffset(0,0,2,0),
		};
	}
}
