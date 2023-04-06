using System;

/// <summary>
/// All image size that can be used in the backend.
/// </summary>
public enum ImageSize
{
	None = 0,
	I256 = 1,
	I512 = 2,
	I1024 = 3,
	I1280 = 4,
	I1920 = 5,
}

/// <summary>
/// Struct to save the different sizing urls in for images.
/// </summary>
[Serializable]
public struct ImageSizeUrls
{
	//Set of urls for retrieving the specific images.
	public string url256,
		url512,
		url1024,
		url1280,
		url1920;

	/// <summary>
	/// Tries to retrieve the url of the image, based on the size.
	/// </summary>
	/// <param name="size">size of the image that needs to be downloaded.</param>
	/// <param name="url">url output if it is found.</param>
	/// <returns>True/False any url was found.</returns>
	public bool TryGetUrl(ImageSize size, out string url) {
		switch (size) {
			case ImageSize.I256:
				url = url256;
				break;
			case ImageSize.I512:
				url = url512;
				break;
			case ImageSize.I1024:
				url = url1024;
				break;
			case ImageSize.I1280:
				url = url1280;
				break;
			case ImageSize.I1920:
				url = url1920;
				break;
			default:
				url = "";
				return false;
		}

		return !string.IsNullOrEmpty(url);
	}
}