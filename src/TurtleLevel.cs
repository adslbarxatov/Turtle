using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает отдельный автомобиль
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
			set
				{
				currentPosition = value;
				}
			}
		private Vector2 currentPosition;

		/// <summary>
		/// Метод задаёт ординату текущей позиции
		/// </summary>
		/// <param name="Y">Новое значение ординаты</param>
		public void SetCurrentPosY (float Y)
			{
			currentPosition.Y = Y;
			}

		/// <summary>
		/// Скорость автомобиля
		/// </summary>
		public uint Speed
			{
			get
				{
				return speed;
				}
			}
		private uint speed;

		/// <summary>
		/// Линия движения автомобиля
		/// </summary>
		public uint Line
			{
			get
				{
				return line;
				}
			}
		private uint line;

		/// <summary>
		/// Ширина текстуры автомобиля
		/// </summary>
		public const int DefaultWidth = 170;

		/// <summary>
		/// Высота текстуры автомобиля
		/// </summary>
		public const int DefaultHeight = 370;

		/// <summary>
		/// Угол поворота текстуры (в радианах) (зависит от полосы)
		/// </summary>
		public float Turn
			{
			get
				{
				return (float)(Math.PI / 2) * (1 - MoveTo.Y);
				}
			}

		/// <summary>
		/// Вектор направления движения (зависит от полосы)
		/// </summary>
		public Vector2 MoveTo
			{
			get
				{
				return new Vector2 (0, GameAuxFunctions.NNSign (Line % (TurtleGame.LinesQuantity / 2), false));
				}
			}

		/// <summary>
		/// Стартовая позиция (зависит от полосы)
		/// </summary>
		public Vector2 StartPosition
			{
			get
				{
				// Ситуация, обратная к MoveTo (чтобы "снизу" было "вверх" и наоборот)
				return new Vector2 (TurtleGame.RoadLineWidth * Line + TurtleGame.RoadLeft +
					TurtleGame.RoadLineWidth / 2, (1 - Line % (TurtleGame.LinesQuantity / 2)) *
					TurtleGame.GameFieldBottom - MoveTo.Y * DefaultHeight);
				}
			}

		/// <summary>
		/// Исходный Rect, необходимый для spriteBatch.Draw
		/// </summary>
		public Rectangle SourceRect
			{
			get
				{
				return new Rectangle (0, 0, DefaultWidth, DefaultHeight);
				}
			}

		/// <summary>
		/// Конечный Rect, необходимый для spriteBatch.Draw
		/// </summary>
		public Rectangle DestinationRect
			{
			get
				{
				return new Rectangle ((int)currentPosition.X, (int)currentPosition.Y, DefaultWidth, DefaultHeight);
				}
			}

		/// <summary>
		/// Середина текстуры автомобиля
		/// </summary>
		public Vector2 Origin
			{
			get
				{
				return new Vector2 (DefaultWidth / 2, DefaultHeight / 2);
				}
			}

		/// <summary>
		/// Используемое расстояние между машинами
		/// </summary>
		public float UsableDelay
			{
			get
				{
				return (float)(2 * DefaultHeight);
				}
			}

		/// <summary>
		/// Конструктор. Инициализирует автомобиль
		/// </summary>
		/// <param name="TextureNumber">Номер текстуры</param>
		/// <param name="VSpeed">Скорость</param>
		/// <param name="VLine">Полоса</param>
		public CarState (int TextureNumber, uint VSpeed, uint VLine)
			{
			textureNumber = TextureNumber;
			speed = VSpeed;
			line = VLine;
			currentPosition = StartPosition;
			}
		}

	/// <summary>
	/// Класс описывает съедобные объекты
	/// </summary>
	public class EatableObjectState
		{
		/// <summary>
		/// Номер используемой текстуры
		/// </summary>
		public uint TextureNumber
			{
			get
				{
				return textureNumber;
				}
			}
		private uint textureNumber;

		/// <summary>
		/// Текущая позиция
		/// </summary>
		public Vector2 Position
			{
			get
				{
				return position;
				}
			}
		private Vector2 position;

		/// <summary>
		/// Угол поворота
		/// </summary>
		public float Turn
			{
			get
				{
				return turn;
				}
			}
		private float turn;

		/// <summary>
		/// Ширина текстуры объекта
		/// </summary>
		public const int DefaultWidth = 40;

		/// <summary>
		/// Высота текстуры объекта
		/// </summary>
		public const int DefaultHeight = 40;

		/// <summary>
		/// Исходный Rect, необходимый для spriteBatch.Draw
		/// </summary>
		public Rectangle SourceRect
			{
			get
				{
				return new Rectangle (0, 0, DefaultWidth, DefaultHeight);
				}
			}

		/// <summary>
		/// Конечный Rect, необходимый для spriteBatch.Draw
		/// </summary>
		public Rectangle DestinationRect
			{
			get
				{
				return new Rectangle ((int)position.X, (int)position.Y, DefaultWidth, DefaultHeight);
				}
			}

		/// <summary>
		/// Середина текстуры объекта
		/// </summary>
		public Vector2 Origin
			{
			get
				{
				return new Vector2 (DefaultWidth / 2, DefaultHeight / 2);
				}
			}

		/// <summary>
		/// Конструктор. Создаёт съедобный объект
		/// </summary>
		/// <param name="TextureNumber">Номер текстуры</param>
		/// <param name="VPosition">Позиция</param>
		/// <param name="VTurn">Поворот текстуры</param>
		public EatableObjectState (uint TextureNumber, Vector2 VPosition, float VTurn)
			{
			textureNumber = TextureNumber;
			position = VPosition;
			turn = VTurn;
			}
		}

	/// <summary>
	/// Класс описывает уровень игры Черепашка
	/// </summary>
	public class TurtleLevel: IDisposable
		{
		/// <summary>
		/// Фон уровня
		/// </summary>
		public Texture2D Background
			{
			get
				{
				return background;
				}
			}
		private Texture2D background;

		/*private Random rnd = new Random ();*/

		/// <summary>
		/// Позиция камеры наблюдения
		/// </summary>
		public Vector2 CameraPosition
			{
			get
				{
				return cameraPosition;
				}
			}
		private Vector2 cameraPosition;

		/// <summary>
		/// Контент уровня
		/// </summary>
		public ContentManager Content
			{
			get
				{
				return content;
				}
			}
		private ContentManager content;

		/// <summary>
		/// Конструктор. Инициализирует уровень игры
		/// </summary>
		public TurtleLevel (IServiceProvider serviceProvider)
			{
			// Создание контент-менеджера для текущего уровня
			content = new ContentManager (serviceProvider, "Content/Turtle");

			// Загрузка фона уровня
			background = Content.Load<Texture2D> ("Background/Back");
			}

		/// <summary>
		/// Выгрузка контента уровня
		/// </summary>
		public void Dispose ()
			{
			Content.Unload ();
			}

		/// <summary>
		/// Метод выполняет отрисовку уровня
		/// </summary>
		/// <param name="VGameTime">Время игры</param>
		/// <param name="VSpriteBatch">Отрисовщик</param>
		/// <param name="PlayerPosition">Позиция игрока</param>
		public void Draw (GameTime VGameTime, SpriteBatch VSpriteBatch, Vector2 PlayerPosition)
			{
			// Фон
			VSpriteBatch.Draw (background, -cameraPosition, TurtleGameColors.White);

			// Выполнение смещения игрового поля при выходе за границы окна
			VSpriteBatch.End ();

			ScrollCamera (VSpriteBatch.GraphicsDevice.Viewport, PlayerPosition);
			Matrix cameraTransform = Matrix.CreateTranslation (-cameraPosition.X, -cameraPosition.Y, 0.0f);

			VSpriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
				DepthStencilState.None, RasterizerState.CullNone, null, cameraTransform);
			}

		/// <summary>
		/// Функция пересчёта камеры наблюдения
		/// </summary>
		/// <param name="PlayerPosition">Позиция игрока</param>
		private void ScrollCamera (Viewport VViewport, Vector2 PlayerPosition)
			{
			// Относительные размеры поля, при выходе за которые начинается смещение камеры
			Vector2 ViewMargin = new Vector2 (0.35f, -0.35f);

			// Абсолютные размеры этого поля и его границы
			Vector2 MarginSize = new Vector2 (VViewport.Width, VViewport.Height) * ViewMargin;
			float MarginLeft = cameraPosition.X + MarginSize.X,
				  MarginRight = cameraPosition.X + VViewport.Width - MarginSize.X,
				  MarginDown = cameraPosition.Y + VViewport.Height + MarginSize.Y,
				  MarginUp = cameraPosition.Y - MarginSize.Y;

			// Вычисление передвижения поля при приближении головы к разным границам окна
			Vector2 CamMov = new Vector2 (0.0f, 0.0f);

			if (PlayerPosition.X < MarginLeft)
				CamMov.X = PlayerPosition.X - MarginLeft;
			if (PlayerPosition.X > MarginRight)
				CamMov.X = PlayerPosition.X - MarginRight;
			if (PlayerPosition.Y > MarginDown)
				CamMov.Y = PlayerPosition.Y - MarginDown;
			if (PlayerPosition.Y < MarginUp)
				CamMov.Y = PlayerPosition.Y - MarginUp;

			// Обновление позиции камеры, но с ограничением на выход за границы уровня
			Vector2 MaxCamPos = new Vector2 (background.Width - VViewport.Width,
				background.Height - VViewport.Height);
			cameraPosition.X = MathHelper.Clamp (cameraPosition.X + CamMov.X, 0.0f, MaxCamPos.X);
			cameraPosition.Y = MathHelper.Clamp (cameraPosition.Y + CamMov.Y, 0.0f, MaxCamPos.Y);
			}
		}
	}
