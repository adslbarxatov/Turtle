using Microsoft.Xna.Framework;
using System;

namespace RD_AAOW
	{
	/// <summary>
	/// Перечисление состояний игры
	/// </summary>
	public enum GameStatus
		{
		/// <summary>
		/// Начало
		/// </summary>
		Start = 0,

		/// <summary>
		/// Выбор языка
		/// </summary>
		Language = 4,

		/// <summary>
		/// Отображение справки
		/// </summary>
		Help = 1,

		/// <summary>
		/// Игровой процесс
		/// </summary>
		Playing = 2,

		/// <summary>
		/// Завершение
		/// </summary>
		Finish = 3
		};

	/// <summary>
	/// Класс описывает вспомогательные функции игры
	/// </summary>
	public static class GameAuxFunctions
		{
		/// <summary>
		/// Функция определяет координаты, помещающие текстуру в центр экрана
		/// </summary>
		/// <param name="BufferSize">Размер экрана</param>
		/// <param name="CameraPosition">Позиция камеры</param>
		public static Vector2 CenterOf (Vector2 BufferSize, Vector2 CameraPosition)
			{
			return new Vector2 (BufferSize.X / 2, BufferSize.Y / 2) + CameraPosition;
			}

		/// <summary>
		/// Функция определяет знак элемента и делает его не равным нулю
		/// </summary>
		/// <param name="N">Корректируемый элемент</param>
		/// <param name="Plus">Флаг выполняет смещение нуля в положительную сторону</param>
		public static int NNSign (float N, bool Plus)
			{
			return Math.Sign (2 * Math.Sign (N) + (Plus ? 1 : -1));
			}

		/// <summary>
		/// Функция определяет расстояние между двумя точками
		/// </summary>
		public static double VDist (Vector2 Vec1, Vector2 Vec2)
			{
			return Math.Pow (Math.Pow (Vec1.X - Vec2.X, 2.0) + Math.Pow (Vec1.Y - Vec2.Y, 2.0), 0.5);
			}
		}

	/// <summary>
	/// Цвета, используемые в игре Змейка
	/// </summary>
	public struct SnakeGameColors
		{
		private const float m = 0.7f;

		public static Color LRed = new Color (255, 150, 150),
							Red = new Color (255, 0, 0),
							Yellow = new Color (255, 255, 0),
							Orange = new Color (255, 128, 0),
							Gold = new Color (255, 215, 0),
							Brown = new Color (160, 40, 40),
							Green = new Color (0, 255, 0),
							DGreen = new Color (0, 128, 0),
							LBlue = new Color (128, 200, 255),
							DBlue = new Color (0, 0, 80),
							Silver = new Color (210, 210, 210),
							Black = new Color (0, 0, 0),
							White = new Color (255, 255, 255),

							CompasRed = new Color (255, 120, 120),
							CompasGreen = new Color (120, 255, 120),

							LBlue_B = new Color ((byte)(m * LBlue.R), (byte)(m * LBlue.G), (byte)(m * LBlue.B)),
							Green_B = new Color (m * Green.R, m * Green.G, m * Green.B),
							Red_B = new Color ((byte)(m * Red.R), (byte)(m * Red.G), (byte)(m * Red.B)),
							Yellow_B = new Color ((byte)(m * Yellow.R), (byte)(m * Yellow.G), (byte)(m * Yellow.B));
		}

#if RACES
	/// <summary>
	/// Класс-описатель отдельного автомобиля
	/// </summary>
	public class CarState
		{
		/// <summary>
		/// Номер используемой текстуры
		/// </summary>
		public int TextureNumber
			{
			get
				{
				return textureNumber;
				}
			set
				{
				textureNumber = value;
				}
			}
		private int textureNumber;

		/// <summary>
		/// Текущая позиция
		/// </summary>
		public Vector2 CurrentPosition
			{
			get
				{
				return currentPosition;
				}
			}
		private Vector2 currentPosition;

		/// <summary>
		/// Метод задаёт абсциссу текущей позиции автомобиля
		/// </summary>
		/// <param name="X">Новая абсцисса</param>
		public void SetCurrentPosX (float X)
			{
			currentPosition.X = X;
			}

		/// <summary>
		/// Метод задаёт ординату текущей позиции автомобиля
		/// </summary>
		/// <param name="Y">Новая ордината</param>
		public void SetCurrentPosY (float Y)
			{
			currentPosition.Y = Y;
			}

		/// <summary>
		/// Линия движения
		/// </summary>
		public int Line
			{
			get
				{
				return line;
				}
			}
		private int line;

		/// <summary>
		/// Флаг активности автомобиля
		/// </summary>
		public int Enabled
			{
			get
				{
				return enabled;
				}
			set
				{
				enabled = value;
				}
			}
		private int enabled;

		/// <summary>
		/// Ширина текстуры автомобиля
		/// </summary>
		public const int DefWidth = 48;

		/// <summary>
		/// Высота текстуры автомобиля
		/// </summary>
		public const int DefHeight = 104;

		/// <summary>
		/// Стартовая позиция (зависит от полосы)
		/// </summary>
		public Vector2 StartPosition
			{
			get
				{
				// Ситуация, обратная к MoveTo (чтобы "снизу" было "вверх" и наоборот)
				return new Vector2 (RacesGame.RoadLineWidth * Line + RacesGame.RoadLeft +
					RacesGame.RoadLineWidth / 2, 0.0f);
				}
			}

		/// <summary>
		/// Исходный Rect для spriteBatch.Draw
		/// </summary>
		public Rectangle SourceRect
			{
			get
				{
				return new Rectangle (0, 0, DefWidth, DefHeight);
				}
			}

		/// <summary>
		/// Конечный Rect для spriteBatch.Draw
		/// </summary>
		public Rectangle DestinationRect
			{
			get
				{
				return new Rectangle ((int)currentPosition.X, (int)currentPosition.Y, DefWidth, DefHeight);
				}
			}

		/// <summary>
		/// Центр автомобиля
		/// </summary>
		public Vector2 Origin
			{
			get
				{
				return new Vector2 (DefWidth / 2, DefHeight / 2);
				}
			}

		/// <summary>
		/// Используемое расстояние между машинами
		/// </summary>
		public Vector2 UsableDelay
			{
			get
				{
				return new Vector2 (0.0f, 4 * DefHeight);
				}
			}

		/// <summary>
		/// Конструктор. Инициализирует описатель автомобиля 
		/// </summary>
		/// <param name="VTextureNumber">Номер используемой текстуры</param>
		/// <param name="VLine">Номер полосы</param>
		/// <param name="VRow">Номер линии</param>
		/// <param name="VEnabled">Флаг активности</param>
		public CarState (int VTextureNumber, int VLine, int VRow, int VEnabled)
			{
			textureNumber = VTextureNumber;
			line = VLine;
			currentPosition = StartPosition - VRow * UsableDelay;
			enabled = VEnabled;
			}
		}
#endif

	/// <summary>
	/// Цвета, используемые в игре Гонки
	/// </summary>
	public struct RacesGameColors
		{
		public static Color Red = new Color (255, 32, 0),
							Yellow = new Color (255, 255, 0),
							Orange = new Color (255, 128, 0),
							Green = new Color (0, 255, 128),
							LBlue = new Color (128, 200, 255),
							Black = new Color (0, 0, 0),
							White = new Color (255, 255, 255);
		}

	/// <summary>
	/// Цвета, используемые в игре Черепашка
	/// </summary>
	public struct TurtleGameColors
		{
		private const float m = 0.7f;

		public static Color Red = new Color (255, 32, 0),
							Yellow = new Color (255, 255, 0),
							Orange = new Color (255, 128, 0),
							Brown = new Color (160, 40, 40),
							Green = new Color (0, 255, 0),
							DGreen = new Color (0, 150, 0),
							LBlue = new Color (128, 200, 255),
							Blue = new Color (0, 0, 255),
							DBlue = new Color (0, 0, 128),
							Black = new Color (0, 0, 0),
							White = new Color (255, 255, 255),

							LBlue_B = new Color ((byte)(m * LBlue.R), (byte)(m * LBlue.G), (byte)(m * LBlue.B)),
							Green_B = new Color (m * Green.R, m * Green.G, m * Green.B),
							Red_B = new Color ((byte)(m * Red.R), (byte)(m * Red.G), (byte)(m * Red.B)),
							Yellow_B = new Color ((byte)(m * Yellow.R), (byte)(m * Yellow.G), (byte)(m * Yellow.B));
		}
	}