namespace Base.Ravel.CharacterAnimation {
	/// <summary>
	/// Settings for determining character animation behaviour.
	/// </summary>
	public struct CharacterSettings {
		/// <summary>
		/// Describes the blend between gendered animation 0 equals feminine and 1 describes masculine.
		/// </summary>
		public float gender;

		/// <param name="gender">Describes the blend between gendered animation 0 equals feminine and 1 describes masculine.</param>
		public CharacterSettings(float gender) {
			this.gender = gender;
		}
	}
}