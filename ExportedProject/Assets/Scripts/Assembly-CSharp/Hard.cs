internal class Hard : DifficultLevel
{
	public Hard()
	{
		name = "HARD";
		lvl = 2;
		monsterMakeDamage = 2f;
		monsterTakeDamage = 0.75f;
		winnigText = "You're a damn good hunter! You managed to track down and catch an incredibly strong and ferocious beast. With the strength of which could only be strengthened by your perseverance and the will to win.\n\n";
		winnigText += "You win with hard settings. Try a higher level of difficulty.";
	}
}
