internal class Normal : DifficultLevel
{
	public Normal()
	{
		name = "NORMAL";
		lvl = 1;
		monsterMakeDamage = 1.5f;
		monsterTakeDamage = 1f;
		winnigText = "Congratulations! The monster fell. The truth will be revealed. Years of searching, finally, were crowned with success.\n\n";
		winnigText += "Try a harder difficult level.";
	}
}
