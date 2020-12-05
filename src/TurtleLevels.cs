namespace RD_AAOW
	{
	/// <summary>
	/// ����� ��������� ������ ����
	/// </summary>
	public class LevelData
		{
		/// <summary>
		/// ����� �������
		/// </summary>
		public const int LevelsQuantity = 12;

		/// <summary>
		/// ������������ �������� �� ������
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
		/// ������������ ����� ����� �� ������
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
		/// ������������ ���������� �������� �� ������
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
		/// �����������. ����������� ��������� ������ ����
		/// </summary>
		/// <param name="LevelNumber">������� ����</param>
		public LevelData (int LevelNumber)
			{
			switch (LevelNumber)
				{
				// ������� 1
				case (0):
					maxSpeed = 1;
					maxLineCars = 2;        // ���������� ����� �� ����� ���������� (�� ����� 3-�)
					maxLevelObjects = 5;
					break;

				// ������� 2
				case (1):
					maxSpeed = 2;
					maxLineCars = 2;
					maxLevelObjects = 10;
					break;

				// ������� 3
				case (2):
					maxSpeed = 3;
					maxLineCars = 2;
					maxLevelObjects = 15;
					break;

				// ������� 4
				case (3):
					maxSpeed = 4;
					maxLineCars = 2;
					maxLevelObjects = 20;
					break;

				// ������� 5
				case (4):
					maxSpeed = 4;
					maxLineCars = 3;
					maxLevelObjects = 20;
					break;

				// ������� 6
				case (5):
					maxSpeed = 4;
					maxLineCars = 3;
					maxLevelObjects = 30;
					break;

				// ������� 7
				case (6):
					maxSpeed = 5;
					maxLineCars = 3;
					maxLevelObjects = 40;
					break;

				// ������� 8
				case (7):
					maxSpeed = 6;
					maxLineCars = 3;
					maxLevelObjects = 50;
					break;

				// ������� 9
				case (8):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 60;
					break;

				// ������� 10
				case (9):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 70;
					break;

				// ������� 11
				case (10):
					maxSpeed = 7;
					maxLineCars = 3;
					maxLevelObjects = 80;
					break;

				// ������� 12
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
