using System.IO;
using MathBuddy.Strings;
using UnityEditor;
using UnityEngine;

public class ImageState : CreatorWindowState
{
	//references for file output format
	private const float ASPECT_W = 16;
	private const float ASPECT_H = 9;

	//margin px for drawing check marks
	private const float CHECK_MARGIN = 2;
	
	private const int RESOLUTION_X = 1920;
	private const int RESOLUTION_Y = 1080;

	public override CreatorWindow.State State {
		get { return CreatorWindow.State.LoadingImage; }
	}
	protected override Vector2 MinSize {
		get { return new Vector2(230, 230); }
	}
	private Vector2 _scroll;
	
	//position in the image, and what part of the image is outputted
	private Vector2 _poi = new Vector2(0.5f, 0.5f);
	//rect size of image in window
	private Rect _imgRect;
	//current selected image
	private Texture2D _image;
	//is image of correct size?
	private bool _sizeMatch;
	
	public ImageState(CreatorWindow wnd) : base(wnd) { }

	public override void OnStateClosed() {
		base.OnStateClosed();
		//re-enable banner when closing the window
		_wnd.EnableBanner(true);
	}

	public override void OnGUI(Rect position) {
		_scroll = EditorGUILayout.BeginScrollView(_scroll);
		
		if (_image) {
			GUIImageOutput(position);
			GUIImageInfo();
			
			GUIImageTools();
		}
		GUISelectImage();
		
		EditorGUILayout.EndScrollView();
	}

	/// <summary>
	/// Load file on path, and save it in the image variable 
	/// </summary>
	private void LoadFile(string path) {
		_image = new Texture2D(2, 2);
		if (!_image.LoadImage(File.ReadAllBytes(path))) {
			Debug.LogError("Error loading image");
			
			_image = null;
			return;
		}

		_image.name = Path.GetFileNameWithoutExtension(path);
		_image.Apply();
	}

	/// <summary>
	/// Scales image according to the preview image shown, and saves that image. This call automatically selects the saved image.
	/// </summary>
	/// <param name="path">path including filename, where image will be saved.</param>
	private void ScaleCropAndSaveImage(string path) {
		Texture2D edit = new Texture2D(RESOLUTION_X, RESOLUTION_Y);
		edit.hideFlags = HideFlags.HideAndDontSave;
		//determine how to crop the image
		Rect coords = RavelEditor.GetScaleCropCoords(new Rect(0, 0, RESOLUTION_X, RESOLUTION_Y), _image, _poi);
		Color[] pixels = new Color[RESOLUTION_X * RESOLUTION_Y];
		
		//determine where to start copying the image, based on coords
		float xStart, yStart;
		if (coords.x <= MathBuddy.FloatingPoints.LABDA) {
			xStart = 0;
			yStart = _image.height * coords.y;
		}
		else {
			xStart = _image.width * coords.x;
			yStart = 0;
		}

		//determine amount of pixels that will be skipped, or compressed in one coordinate
		float xStep = (_image.width * coords.width) / RESOLUTION_X;
		float yStep = (_image.height * coords.height) / RESOLUTION_Y;

		float x = xStart;
		float y = yStart;
		for (int iy = 0; iy < RESOLUTION_Y; iy++) {
			for (int ix = 0; ix < RESOLUTION_X; ix++) {
				pixels[ix + iy * RESOLUTION_X] = _image.GetPixel(Mathf.RoundToInt(x),Mathf.RoundToInt(y));

				x += xStep;
			}

			y += yStep;
			x = xStart;
		}
		
		edit.SetPixels(pixels);
		edit.Apply(false);

		byte[] data = edit.EncodeToJPG();
		File.WriteAllBytes(path, data); 
		
		AssetDatabase.Refresh();

		if (path.IsSubpathOf(Application.dataPath)) {
			//split path from asset folder (-6 ensures the asset folder is included in the path).
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(Application.dataPath.Length - 6));
		}
		LoadFile(path);
	}

#region draw GUI methods
	/// <summary>
	/// Draws the selected images in the right aspect and enables the user to drag the image to adjust it.
	/// </summary>
	/// <param name="position">gui rect</param>
	private void GUIImageOutput(Rect position) {
		//determine height, based on width and aspect
		float h = position.width / ASPECT_W * ASPECT_H;
		//set image size
		_imgRect = new Rect(0, 0, position.width, h);
			
		//reset gui coordinates
		GUI.BeginGroup(_imgRect);
		Rect scaleCoords = RavelEditor.DrawTextureScaledCropGUI(_imgRect, _image, _poi);
			
		if (Event.current.type == EventType.MouseDrag) {
			float halfSize;
			if (1f - scaleCoords.width > MathBuddy.FloatingPoints.LABDA) {
				halfSize = scaleCoords.width / 2f;
				_poi.x = Mathf.Clamp(_poi.x - (Event.current.delta.x / _imgRect.width) * scaleCoords.width, halfSize, 1f - halfSize);
			}
			else {
				_poi.x = 0.5f;
			}

			if (1f - scaleCoords.height > MathBuddy.FloatingPoints.LABDA) {
				halfSize = scaleCoords.height / 2f;
				_poi.y = Mathf.Clamp(_poi.y + (Event.current.delta.y / _imgRect.height) * scaleCoords.height, halfSize, 1f - halfSize);
			}
			else {
				_poi.y = 0.5f;
			}
				
			_wnd.Repaint();
		}
		GUI.EndGroup();
	}
	
	/// <summary>
	/// Checks the image for requirements and shows label that explains moving the image
	/// </summary>
	private void GUIImageInfo() {
		float imgY = GUILayoutUtility.GetLastRect().yMax;
		EditorGUILayout.BeginHorizontal();
		//check image size
		MessageType result;
		string msg;
		if (_image.width == RESOLUTION_X && _image.height == RESOLUTION_Y) {
			_sizeMatch = true;
			result = MessageType.Info;
			msg = "size is correct. This image can be uploaded as is.";
		} else
		{
			_sizeMatch = false;
			if (_image.width < 1920 || _image.height < 1080) {
				result = MessageType.Error;
				msg = "size is too small. This image will need to be rescaled and will lose quality.";
			}
			else {
				result = MessageType.Warning;
				msg = "size is too big. This image will need to be cropped, but won't lose quality.";
			}
		}
		//draw green/red image of correct size
		GUIDrawCheckMark(result, $"Resolution: ({_image.width},{_image.height})", imgY);
		if (!_sizeMatch) {
			GUILayout.Label("Drag the image to change the crop", new GUIStyle("label") {alignment = TextAnchor.MiddleRight});
			
		}
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.HelpBox(msg, result);
	}
	
	/// <summary>
	/// Draws one single checkmark with variable checkmark sprite and label
	/// </summary>
	/// <param name="pass">True or False sprite?</param>
	/// <param name="label">Text label after checkmark</param>
	/// <param name="imgY">(top) y position of the image.</param>
	private void GUIDrawCheckMark(MessageType passType, string label, float imgY) {
		RavelEditor.DrawTextureScaleWidthGUI(new Vector2(CHECK_MARGIN, imgY), RavelEditorStying.GUI_SPACING_MILLI,
			RavelEditor.Branding.GetCheck(passType), false);
		//EditorGUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);

		GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI + CHECK_MARGIN);
		GUILayout.Label(label, new GUIStyle("label") {alignment = TextAnchor.MiddleLeft});
	}

	/// <summary>
	/// Shows tooling options for when there is an image.
	/// </summary>
	private void GUIImageTools() {
		//crop only enabled when the image is not already correctly sized.
		GUI.enabled = !_sizeMatch;
		if (GUILayout.Button("Scale/Crop image")) {
			string cropPath = RavelEditorSettings.Get().GetFilePath();
			//Save file on location
			cropPath = EditorUtility.SaveFilePanel("Scale/Crop image", cropPath, $"{_image.name}_Crop", "jpg");
			if (!string.IsNullOrEmpty(cropPath)) {
				RavelEditorSettings.Get().SetFilePath(cropPath);
				ScaleCropAndSaveImage(cropPath);
			}
		}
		GUI.enabled = true;
			
		//remove image
		if (GUILayout.Button("Clear image")) {
			_image = null;
			_wnd.EnableBanner(true);
		}
	}

	/// <summary>
	/// Select image button with window.
	/// </summary>
	private void GUISelectImage() {
		if (GUILayout.Button("Select file")) {
			string selectPath = RavelEditorSettings.Get().GetFilePath();

			selectPath = EditorUtility.OpenFilePanel("Select image", selectPath, RavelEditorStying.IMAGE_EXTENSIONS);
			if (!string.IsNullOrEmpty(selectPath)) {
				RavelEditorSettings.Get().SetFilePath(selectPath);
				LoadFile(selectPath);
				
				_wnd.EnableBanner(_image == null);
			}
		}
	}
#endregion
}
