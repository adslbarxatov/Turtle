namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает уровни игры
	/// </summary>
	public class LevelData
		{
		/// <summary>
		/// Число уровней
		/// </summary>
		public const int LevelsQuantity = 12;

		/// <summary>
		/// Максимальная скорость на уровне
		/// </summary>
		public int MaxSpeed
			{
			get
				{
				return maxSpeed;
				}
			}
		private int maxSpeed;

		/// <summary>
		/// Максимальное число машин на полосе
		/// </summary>
		public int MaxLineCars
			{
			get
				{
				return maxLineCars;
				}
			}
		private int maxLineCars;

		/// <summary>
		/// Максимальное количество объектов на уровне
		/// </summary>
		public int MaxLevelObjects
			{
			get
				{
				return maxLevelObjects;
				}
			}
		private int maxLevelObjects;

		/// <summary>
		/// Конструктор. Запрашивает параметры уровня игры
		/// </summary>
		/// <param name="LevelNumber">Уровень игры</param>
		public LevelData (int LevelNumber)
			{
			switch (LevelNumber)
				{
				// Уровень 1
				case (0):
					maxSpeed = 1;
					maxLineCars = 2;        // Количество машин на линии ограничено (не более 3-х)
					maxLevelObjects = 5;
					break;

				// Уровень 2
				case (1):
					maxSpeed = 2;
					maxLineCars = 2;
					maxLevelObjects = 10;
					break;

				// Уровень 3
				case (2):
					maxSpeed = 3;
					maxLineCars = 2;
					maxLevelObjects = 15;
					break;

				// Уровень 4
				case (3):
					maxSpeed = 4;
					maxLineCars = 2;
					maxLevelObjects = 20;
					break;

				// Уровень 5
				case (4):
					maxSpeed = 4;
					maxLineCars = 3;
					maxLevelObjects = 20;
					break;

				// Уровень 6
				case (5):
					maxSpeed = 4;
					maxLineCars = 3;
					maxLevelObjects = 30;
					break;

				// Уровень 7
				case (6):
					maxSpeed = 5;
					maxLineCars = 3;
					maxLevelObjects = 40;
					break;

				// Уровень 8
				case (7):
					maxSpeed = 6;
					maxLineCars = 3;
					maxLevelObjects = 50;
					break;

				// Уровень 9
				case (8):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 60;
					break;

				// Уровень 10
				case (9):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 70;
					break;

				// Уровень 11
				case (10):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 80;
					break;

				// Уровень 12
				case (11):
					maxSpeed = 8;
					maxLineCars = 3;
					maxLevelObjects = 100;
					break;

				default:
					maxSpeed = maxLineCars = maxLevelObjects = 0;
					break;
				}
			}
		}
	}
