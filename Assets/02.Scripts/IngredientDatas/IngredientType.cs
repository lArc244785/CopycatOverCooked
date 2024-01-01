namespace CopycatOverCooked.Datas
{
	public enum IngredientType
	{
		None = 0,
		Trash = -9999,
		Onion = 1 << 0,
		Tomato = 1 << 1,
		Stake = 1 << 2,
		Bun = 1 << 3,
		Trimmed_Onion = 1 << 4,
		Trimmed_Tomato = 1 << 5,
		Trimmed_Stake = 1 << 6,
		Trimmed_Bun = 1 << 7,
		Grilled_Shrimp = 1 << 8,
		Soup_Tomato = 1 << 9,
		Cook_Stake = 1 << 10,



		//ÇÜ¹ö°Å
		Hamburger_Onion_Tomato = Trimmed_Onion | Trimmed_Tomato,
		Hamburger_Onion_Stake = Trimmed_Onion | Cook_Stake,
		Hamburger_Onion_Bun = Trimmed_Onion | Trimmed_Bun,

		Hamburger_Tomato_Stake = Trimmed_Tomato | Cook_Stake,
		Hamburger_Tomato_Bun = Trimmed_Tomato | Trimmed_Bun,

		Hamburger_Stake_Bun = Cook_Stake | Trimmed_Bun,

		Hamburger_Onion_Tomato_Stake = Trimmed_Onion | Trimmed_Tomato | Cook_Stake,
		Hamburger_Onion_Tomato_Bun = Trimmed_Onion | Trimmed_Tomato | Trimmed_Bun,

		Hamburger_Tomato_Stake_Bun = Trimmed_Tomato | Cook_Stake | Trimmed_Bun,

		Hamburger = Trimmed_Onion | Trimmed_Tomato | Cook_Stake | Trimmed_Bun,

	}
}