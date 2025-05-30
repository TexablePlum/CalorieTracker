namespace CalorieTracker.Domain.Enums
{
	/// <summary>
	/// Jednostki miary produktów spożywczych
	/// </summary>
	public enum ProductUnit
	{
		/// <summary>Gramy - dla większości produktów spożywczych</summary>
		Gram,

		/// <summary>Mililitry - dla płynów</summary>
		Milliliter,

		/// <summary>Sztuki - dla produktów liczonych w sztukach (jajka, bułki)</summary>
		Piece,

		/// <summary>Porcje - dla gotowych porcji/opakowań</summary>
		Portion
	}
}
