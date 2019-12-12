namespace Games.Object
{
	public interface ICharaBase
	{
		string Name { get; }
		int MaxHP { get; }
		int HP { get; }
		int Level{ get; }
		int EXP { get; }
		bool IsAlive{ get; }
		bool IsDead{ get; }
		//int Attack();
		void Damage(int damage);
		void Recover(int amount);
	}
}
