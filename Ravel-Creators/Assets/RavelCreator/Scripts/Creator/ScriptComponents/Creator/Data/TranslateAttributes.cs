using System;

namespace Base.Ravel.TranslateAttributes
{
	[Flags]
	public enum TransformAttribute
	{
		None = 0,
		Position = 1,
		Rotation = 2,
		Scale = 4
	}

	public enum TransformSpace
	{
		World, // global application
		Self, // local application
		//TODO: add delta
		//Delta   // only apply delta values
	}
}