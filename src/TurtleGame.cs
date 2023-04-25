using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает игру Черепашка
	/// </summary>
	public class TurtleGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ПЕРЕМЕННЫЕ

		// Драйвера игры
		private GraphicsDeviceManager graphics;             // Графика
		private SpriteBatch spriteBatch;                    // Sprite-отрисовка
		private KeyboardState keyboardState;                // Состояние клавиатуры
		private SpriteFont defFont, midFont, bigFont;       // Шрифты
		private Random rnd = new Random ();                 // ГСЧ

		/// <summary>
		/// Ширина окна
		/// </summary>
		public const int BackBufferWidth = 1200;

		/// <summary>
		/// Высота окна
		/// </summary>
		public const int BackBufferHeight = 640;

		/// <summary>
		/// Высота уровня
		/// </summary>
		public const int GameFieldBottom = BackBufferHeight * 2;

		/// <summary>
		/// Ширина дорожной полосы
		/// </summary>
		public const int RoadLineWidth = 200;

		/// <summary>
		/// Количество полос
		/// </summary>
		public const int LinesQuantity = 4;

		/// <summary>
		/// Левая граница полос
		/// </summary>
		public const int RoadLeft = 200;

		/// <summary>
		/// Правая граница полос
		/// </summary>
		public const int RoadRight = RoadLeft + RoadLineWidth * LinesQuantity;

		// Основное состояние игры (начало|игра|конец)
		private GameStatus gameStatus = GameStatus.Start;
		// Начальный статус игры (статусы перечислены в Auxilitary.cs)

		// Описатели уровня и окна сообщений
		private TurtleLevel level;                          // Класс-описатель уровня
		private int levelNumber = 0;                        // Номер текущего уровня
		private Texture2D messageBack,                      // Фон сообщений
						  startBack;                        // Фон на старте
		private Vector2 messageBackLeftTop;
		private const int turtleSpeed = 4;                  // Скорость черепахи

		// Текущая позиция черепахи и ее объекты анимации
		private Vector2 playerPosition, playerTo;                   // Текущая позиция
		private Animation playerAnimation, playerStayAnimation,     // Изображения анимации (движется, стоит, dead)
						  deadAnimation;
		private AnimationPlayer playerAnimator;                     // Объект-анимация

		// Текстуры машин и съедобных объектов
		private Texture2D[] carTextures, eatableTextures;           // Массивы текстур

		// Текущие состояния всех машин и съедобных объектов (положение, направление, текстура и т.д.) 
		// (структурные классы описаны в Level.cs)

		private List<CarState> carPosition = new List<CarState> ();
		// List позиций всех машин на дороге
		private List<EatableObjectState> eatable = new List<EatableObjectState> ();
		// List позиций съедобных объектов

		// Звуковые эффекты и их параметры
		private SoundEffect SCompleted, SFailed,            // Победа, поражение
							SStart, SStop, SOnOff,          // Старт, пауза, звук off/on
							SAte;                           // Съедение
		private bool isSound = true, isMusic = true;        // Звук и музыка в игре on/off

		// Параметры Alive и Working
		private bool isAlive = false, isWorking = false;

		// Очки
		private int score = 0;                              // Выигрыш
		private const uint scoreMultiplier = 10;            // Множитель для очков
		private const int penalty = 99;                     // Штраф за проигрыш

		// Флаги отображения сообщений
		private bool showLevelMsg = false,      // Сообщение о начале уровня
					 showLoseMsg = false,       // Сообщение о прохождении уровня
					 showWinMsg = false,        // Сообщение о проигрыше
					 showExitMsg = false;       // Подтверждение выхода

		// Согласователи клавиатуры
		private int kbdDelay = 1,               // Пауза в Update-итерациях перед следующим опросом клавиатуры
					kbdDelayTimer;              // Таймер для delay
		private const int kbdDefDelay = 25;     // Базовый delay при нажатии клавиши

		/// <summary>
		/// Конструктор. Формирует рабочую область и окно приложения
		/// </summary>
		public TurtleGame ()
			{
			// Создание "окна" заданного размера
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			//graphics.ToggleFullScreen ();

			// Задание content-директории игры
			Content.RootDirectory = "Content/Turtle";
			}

		/// <summary>
		/// ИНИЦИАЛИЗАЦИЯ
		/// Функция выполняется один раз за игру, при её запуске
		/// Здесь располагаются все инициализации и начальные значения
		/// </summary>
		protected override void Initialize ()
			{
			// НАСТРОЙКА АППАРАТА ПРОРИСОВКИ
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// СОЗДАНИЕ ГРАФИЧЕСКИХ ОБЪЕКТОВ
			// Разные текстуры машин
			carTextures = new Texture2D[]   {
				Content.Load<Texture2D> ("Tiles/Car00"),
				Content.Load<Texture2D> ("Tiles/Car01"),
				Content.Load<Texture2D> ("Tiles/Car02"),
				Content.Load<Texture2D> ("Tiles/Car03"),
				Content.Load<Texture2D> ("Tiles/Car04"),
				Content.Load<Texture2D> ("Tiles/Car05"),
				Content.Load<Texture2D> ("Tiles/Car06"),
				Content.Load<Texture2D> ("Tiles/Car07"),
				Content.Load<Texture2D> ("Tiles/Car08")
											};

			// Разные текстуры съедобных объектов
			eatableTextures = new Texture2D[]   {
				Content.Load<Texture2D> ("Tiles/Eatable00"),
				Content.Load<Texture2D> ("Tiles/Eatable01"),
				Content.Load<Texture2D> ("Tiles/Eatable02"),
				Content.Load<Texture2D> ("Tiles/Eatable03"),
				Content.Load<Texture2D> ("Tiles/Eatable04")
											};

			// Черепаха при столкновении и движении
			playerAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Turtle"), 123, 0.1f, true);
			playerStayAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Turtle"), 123, 0.1f, false);
			deadAnimation = new Animation (Content.Load<Texture2D> ("Tiles/DeadTurtle"), 190, 0.1f, false);
			playerAnimator.PlayAnimation (playerStayAnimation);         // По умолчанию - анимация при паузе

			// СОЗДАНИЕ ЗВУКОВЫХ ЭФФЕКТОВ
			SCompleted = Content.Load<SoundEffect> ("Sounds/Completed");
			SFailed = Content.Load<SoundEffect> ("Sounds/Failed");
			SOnOff = Content.Load<SoundEffect> ("Sounds/SoundOnOff");
			SStart = Content.Load<SoundEffect> ("Sounds/SStart");
			SStop = Content.Load<SoundEffect> ("Sounds/SStop");
			SAte = Content.Load<SoundEffect> ("Sounds/Ate1");

			// СОЗДАНИЕ ШРИФТОВ
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");

			// ЗАГРУЗКА ДОПОЛНИТЕЛЬНЫХ ТЕКСТУР
			messageBack = Content.Load<Texture2D> ("Messages/MessageBack");
			startBack = Content.Load<Texture2D> ("Background/StartBack");
			messageBackLeftTop = new Vector2 (BackBufferWidth - messageBack.Width, BackBufferHeight - messageBack.Height);

			// ЧТЕНИЕ НАСТРОЕК И РЕЗУЛЬТАТОВ ИГРЫ
			GameSettings (false);

			// НАСТРОЙКА МУЗЫКИ
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// Инициализация
			base.Initialize ();
			}

		/// <summary>
		/// Метод обновляет состояние игры в реальном времени
		/// </summary>
		protected override void Update (GameTime VGameTime)
			{
			// Опрос клавиатуры с предотвращением повторов
			kbdDelayTimer++;
			kbdDelayTimer %= kbdDelay;
			if (kbdDelayTimer == 0)
				{
				if (KeyboardProc ())
					{
					kbdDelay = kbdDefDelay;
					kbdDelayTimer = 0;
					}
				else
					{
					kbdDelay = 1;
					}
				}
			KeyboardMoveProc ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					if (!isAlive)
						break;

					// Выигрыш (запуск нового уровня с паузой)
					if (playerPosition.X > level.Background.Width - playerAnimation.FrameWidth / 2)
						{
						// Звук
						MediaPlayer.Stop ();
						if (isSound)
							SCompleted.Play ();

						// Отображение сообщения
						showWinMsg = true;

						// Переключение параметров
						isAlive = isWorking = false;
						playerAnimator.PlayAnimation (playerStayAnimation);

						// Перезапуск уровня произойдёт по нажатию клавиши
						}

					// Движение всех машин
					if (isWorking)
						{
						for (int i = 0; i < carPosition.Count; i++)
							{
							// Смещение
							carPosition[i].CurrentPosition += carPosition[i].MoveTo * carPosition[i].Speed;

							// Выход за границы уровня
							if ((carPosition[i].CurrentPosition.Y < -CarState.DefaultHeight) ||
								(carPosition[i].CurrentPosition.Y > TurtleGame.GameFieldBottom + CarState.DefaultHeight))
								// При выходе за границы уровня машина удаляется из массива, а вместо неё
								// создаётся новая, но с той же скоростью и на той же полосе
								carPosition[i].CurrentPosition = carPosition[i].StartPosition;
							}
						}

					// Проверка столкновений с машинами
					if (IsCollapted ())
						{
						// Звук
						MediaPlayer.Stop ();
						if (isSound)
							SFailed.Play ((90 + rnd.Next (10)) * 0.01f,
							(20 - rnd.Next (40)) * 0.01f, 
							(playerPosition.X - BackBufferWidth / 2.0f) / (BackBufferWidth / 2.0f));

						// Переключение состояния игры
						isAlive = isWorking = false;
						levelNumber--;
						playerAnimator.PlayAnimation (deadAnimation);

						// Отображение сообщения
						showLoseMsg = true;

						// Пересчёт очков
						score -= penalty;           // Размер штрафа

						// Перезапуск уровня произойдёт по нажатию клавиши Space
						}

					// Проверка съедений
					Vector2 k = IsAte ();
					if (k.X != -1.0f)
						{
						// Удаление съеденного объекта
						eatable.RemoveAt ((int)k.X);

						// Пересчёт очков
						score += (int)((k.Y + 1.0f) * scoreMultiplier);

						// Звук
						if (isSound)
							SAte.Play ((90 + rnd.Next (10)) * 0.01f,
							(20 - rnd.Next (40)) * 0.01f,
							(playerPosition.X - BackBufferWidth / 2.0f) / (BackBufferWidth / 2.0f));
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// Обновление игры
			base.Update (VGameTime);
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Низкоскоростные события
		/// </summary>
		private bool KeyboardProc ()
			{
			// Запрос к клавиатуре
			keyboardState = Keyboard.GetState ();

			// В НЕЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			// Настройки звука
			if (!showExitMsg)
				{
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// Была нажата клавиша
					return true;
					}

				if (keyboardState.IsKeyDown (Keys.M))
					{
					if (isMusic)                            // Music on/off
						{
						isMusic = false;
						MediaPlayer.Stop ();
						}
					else
						{
						isMusic = true;
						MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));
						}
					SOnOff.Play ();

					return true;
					}
				}

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// Немедленный выход
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// Справка
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// Выбор языка интерфеса
					if (keyboardState.IsKeyDown (Keys.L))
						{
						gameStatus = GameStatus.Language;

						return true;
						}

					// Переход далее
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение параметров
						gameStatus = GameStatus.Playing;

						// Загрузка уровня
						levelNumber--;
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
				case GameStatus.Language:
					// Возврат
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// Нажатие паузы и продолжения
					if (!showExitMsg)           // Нельзя ничего делать, если появилось сообщение о выходе
						{
						if (isAlive && keyboardState.IsKeyDown (Keys.Space))    // Pause
							{
							if (isWorking)
								{
								isWorking = false;

								if (isSound)
									SStop.Play ();
								}
							else                                                // Continue
								{
								showLevelMsg = false;
								isWorking = true;

								if (isSound)
									SStart.Play ();
								}

							return true;
							}

						// Нажатие клавиши продолжения
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// Проверка на выход
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// Пауза
							isWorking = false;

							// Сообщение
							showExitMsg = true;

							// Звук
							if (isSound)
								SStart.Play ();

							return true;
							}
						}

					// Попытка выхода
					if (showExitMsg)
						{
						// Выход из игры (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// Продолжение (back)
						if (keyboardState.IsKeyDown (Keys.N))
							{
							showExitMsg = false;

							return true;
							}
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Finish:
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// Не было ни одного нажатия
			return false;
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Высокоскоростные события
		/// </summary>
		private void KeyboardMoveProc ()
			{
			keyboardState = Keyboard.GetState ();

			// Нажатие клавиш управления
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				// ВВЕРХ

				// Второе условие отвечает на нахождение черепахи всегда внутри поля игры
				// Третье условие отвечает за невозможность разворота на месте
				if (keyboardState.IsKeyDown (Keys.Up) &&
					(playerPosition.Y - turtleSpeed >= playerAnimation.FrameWidth / 2))
					{
					// Смена вектора для текстуры
					playerPosition.Y -= turtleSpeed;
					playerTo.X = 0;
					playerTo.Y = -1;

					// Смена анимации на движущуюся
					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ВНИЗ
				if (keyboardState.IsKeyDown (Keys.Down) &&
					(playerPosition.Y + turtleSpeed <= TurtleGame.GameFieldBottom - playerAnimation.FrameWidth / 2))
					{
					playerPosition.Y += turtleSpeed;
					playerTo.X = 0;
					playerTo.Y = 1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ВЛЕВО
				if (keyboardState.IsKeyDown (Keys.Left) &&
					(playerPosition.X - turtleSpeed >= playerAnimation.FrameWidth / 2))
					{
					playerPosition.X -= turtleSpeed;
					playerTo.Y = 0;
					playerTo.X = -1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ВПРАВО
				if (keyboardState.IsKeyDown (Keys.Right))
					{
					playerPosition.X += turtleSpeed;
					playerTo.Y = 0;
					playerTo.X = 1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// Смена анимации при остановке
				if ((keyboardState.IsKeyUp (Keys.Up)) && (keyboardState.IsKeyUp (Keys.Down)) &&
					(keyboardState.IsKeyUp (Keys.Left)) && (keyboardState.IsKeyUp (Keys.Right)) &&
					(playerAnimator.VAnimation == playerAnimation))
					playerAnimator.PlayAnimation (playerStayAnimation);
				}
			}

		/// <summary>
		/// Метод отображает информацию игры (очки и уровень)
		/// </summary>
		private void DrawInfo ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stScoreLines[0]))
				{
				string[] values = Localization.GetText ("ScoreLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stScoreLines.Length; i++)
					stScoreLines[i] = values[i];
				}
			string S00 = string.Format (stScoreLines[0], score);
			string S01 = isWorking ? string.Format (stScoreLines[1], levelNumber + 1) : stScoreLines[2];

			// Векторы позиций для отображения элементов, учитывающие смещение камеры наблюдения
			Vector2 V1 = new Vector2 (12, 16) + level.CameraPosition,
					V2 = new Vector2 (12, 54) + level.CameraPosition,
					V3 = new Vector2 (BackBufferWidth * 0.92f, BackBufferHeight - 32) + level.CameraPosition,
					V4 = new Vector2 (BackBufferWidth * 0.95f, BackBufferHeight - 32) + level.CameraPosition;

			DrawShadowedString (midFont, S00, V1, TurtleGameColors.Orange);
			DrawShadowedString (midFont, S01, V2, TurtleGameColors.Yellow);

			// Если есть музыка или звук, выводить соответствующий знак
			if (isMusic)
				DrawShadowedString (defFont, "[\x266B]", V3, TurtleGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V3, TurtleGameColors.Black);

			if (isSound)
				DrawShadowedString (defFont, "[\x266A]", V4, TurtleGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266A]", V4, TurtleGameColors.Black);
			}
		private string[] stScoreLines = new string[3];
		private char[] splitter = new char[] { '\t' };

		/// <summary>
		/// Отображение сообщения об уровне
		/// </summary>
		private void ShowLevelMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stLevelLines[0]))
				{
				string[] values = Localization.GetText ("LevelLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLevelLines.Length; i++)
					stLevelLines[i] = values[i];
				}
			string S00 = string.Format (stLevelLines[0], levelNumber + 1);

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S00).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLevelLines[1]).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLevelLines[2]).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.LBlue_B);
			spriteBatch.DrawString (midFont, S00, V1, TurtleGameColors.LBlue);
			spriteBatch.DrawString (defFont, stLevelLines[1], V2, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, stLevelLines[2], V3, TurtleGameColors.Orange);
			}
		private string[] stLevelLines = new string[3];

		/// <summary>
		/// Отображение сообщения о победе
		/// </summary>
		private void ShowWinMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stSuccessLines[0]))
				{
				string[] values = Localization.GetText ("SuccessLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stSuccessLines.Length; i++)
					stSuccessLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stSuccessLines[0]).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stSuccessLines[1]).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stSuccessLines[2]).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stSuccessLines[3]).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Green_B);
			spriteBatch.DrawString (midFont, stSuccessLines[0], V1, TurtleGameColors.Green);
			spriteBatch.DrawString (midFont, stSuccessLines[1], V2, TurtleGameColors.Green);
			spriteBatch.DrawString (defFont, stSuccessLines[2], V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, stSuccessLines[3], V4, TurtleGameColors.Orange);
			}
		private string[] stSuccessLines = new string[4];

		/// <summary>
		/// Отображение сообщения о проигрыше
		/// </summary>
		private void ShowLoseMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stLoseLines[0]))
				{
				string[] values = Localization.GetText ("LoseLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLoseLines.Length; i++)
					stLoseLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stLoseLines[0]).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stLoseLines[1]).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLoseLines[2]).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stLoseLines[3]).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Red_B);
			spriteBatch.DrawString (midFont, stLoseLines[0], V1, TurtleGameColors.Red);
			spriteBatch.DrawString (midFont, stLoseLines[1], V2, TurtleGameColors.Red);
			spriteBatch.DrawString (defFont, stLoseLines[2], V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, stLoseLines[3], V4, TurtleGameColors.Orange);
			}
		private string[] stLoseLines = new string[4];

		/// <summary>
		/// Отображение сообщения о начале игры
		/// </summary>
		private void ShowStartMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stStartLines[0]))
				{
				string[] values = Localization.GetText ("StartLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stStartLines.Length - 1; i++)
					stStartLines[i] = values[i];
				stStartLines[3] = ProgramDescription.AssemblyTitle;
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stStartLines[3]).X) / 2,
						120),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[0]).X) / 2,
						BackBufferHeight / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[1]).X) / 2,
						BackBufferHeight / 2 + 30),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[2]).X) / 2,
						BackBufferHeight / 2 + 60);

			spriteBatch.Draw (startBack, Vector2.Zero, TurtleGameColors.White);
			DrawShadowedString (bigFont, stStartLines[3], V1, TurtleGameColors.Orange);
			DrawShadowedString (defFont, stStartLines[0], V3, TurtleGameColors.White);
			DrawShadowedString (defFont, stStartLines[1], V4, TurtleGameColors.White);
			DrawShadowedString (defFont, stStartLines[2], V5, TurtleGameColors.White);
			}
		private string[] stStartLines = new string[4];

		/// <summary>
		/// Отображение сообщения о конце игры
		/// </summary>
		private void ShowFinishMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stFinishLines[0]))
				{
				string[] values = Localization.GetText ("FinishLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stFinishLines.Length; i++)
					stFinishLines[i] = values[i];
				}
			string S01 = string.Format (stFinishLines[1], score);

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stFinishLines[0]).X) / 2,
						(BackBufferHeight - 400) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S01).X) / 2,
						(BackBufferHeight - 50) / 2),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stFinishLines[2]).X) / 2,
						(BackBufferHeight + 100) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, TurtleGameColors.White);
			spriteBatch.DrawString (bigFont, stFinishLines[0], V1, TurtleGameColors.Orange);
			spriteBatch.DrawString (midFont, S01, V2, TurtleGameColors.Yellow);
			spriteBatch.DrawString (defFont, stFinishLines[2], V3, TurtleGameColors.DBlue);
			}
		private string[] stFinishLines = new string[3];

		/// <summary>
		/// Отображение запроса на подтверждение выхода
		/// </summary>
		private void ShowExitMessage ()
			{
			// Сборка строк
			if (string.IsNullOrWhiteSpace (stExitLines[0]))
				{
				string[] values = Localization.GetText ("ExitLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stExitLines.Length; i++)
					stExitLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stExitLines[0]).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (stExitLines[1]).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stExitLines[2]).X) / 2,
						(BackBufferHeight + 70) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stExitLines[3]).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Yellow_B);
			spriteBatch.DrawString (midFont, stExitLines[0], V1, TurtleGameColors.Yellow);
			spriteBatch.DrawString (midFont, stExitLines[1], V2, TurtleGameColors.Yellow);
			spriteBatch.DrawString (defFont, stExitLines[2], V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, stExitLines[3], V4, TurtleGameColors.Orange);
			}
		private string[] stExitLines = new string[4];

		/// <summary>
		/// Отображение вспомогательных интерфейсов
		/// </summary>
		private void ShowServiceMessage (bool Language)
			{
			// Защита от множественного входа
			if (showingServiceMessage)
				return;
			showingServiceMessage = true;

			// Блокировка отрисовки и запуск справки
			spriteBatch.End ();

			if (Language)
				RDGenerics.MessageBox ();
			else
				RDGenerics.ShowAbout (false);

			// Возврат в исходное состояние
			spriteBatch.Begin ();

			// Выход в меню
			gameStatus = GameStatus.Start;
			showingServiceMessage = false;
			}
		private bool showingServiceMessage = false;

		/// <summary>
		/// Метод отрисовывает уровень игры
		/// </summary>
		/// <param name="VGameTime"></param>
		protected override void Draw (GameTime VGameTime)
			{
			// Создание чистого окна и запуск рисования
			graphics.GraphicsDevice.Clear (TurtleGameColors.Black);
			spriteBatch.Begin ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// Отображает стартовый экран
					ShowStartMessage ();

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					ShowServiceMessage (false);
					break;

				case GameStatus.Language:
					ShowServiceMessage (true);
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ОТОБРАЖЕНИЕ УРОВНЯ
					level.Draw (VGameTime, spriteBatch, playerPosition);

					// ОТОБРАЖЕНИЕ ИЗОБРАЖЕНИЙ
					// Съедобные объекты
					for (int i = 0; i < eatable.Count; i++)
						spriteBatch.Draw (eatableTextures[eatable[i].TextureNumber],
							eatable[i].DestinationRect, eatable[i].SourceRect, TurtleGameColors.White,
							eatable[i].Turn, eatable[i].Origin, SpriteEffects.None, 0.0f);

					// Игрок (над ними)
					playerAnimator.Draw (VGameTime, spriteBatch, playerPosition, SpriteEffects.None, TurtleGameColors.White,
						// Изменение угла поворота текстуры
						Math.Acos (playerTo.X) * GameAuxFunctions.NNSign (playerTo.Y, false));

					// Автомобили (ещё выше)
					for (int i = 0; i < carPosition.Count; i++)
						spriteBatch.Draw (carTextures[carPosition[i].TextureNumber],
							carPosition[i].DestinationRect, carPosition[i].SourceRect, TurtleGameColors.White,
							carPosition[i].Turn, carPosition[i].Origin, SpriteEffects.None, 0.0f);

					// ОТОБРАЖЕНИЕ ИНФОРМАЦИИ УРОВНЯ
					DrawInfo ();

					// Отображение сообщений (если они вызваны)
					if (showLevelMsg)
						ShowLevelMessage ();

					if (showWinMsg)
						ShowWinMessage ();

					if (showLoseMsg)
						ShowLoseMessage ();

					if (showExitMsg)
						ShowExitMessage ();

					break;

				//////////////////////////////////////////////////////////////////

				case GameStatus.Finish:
					ShowFinishMessage ();

					break;
				}

			// Завершение рисования
			spriteBatch.End ();

			// Перерисовка
			base.Draw (VGameTime);
			}

		/// <summary>
		/// Метод загружает следующий уровень игры
		/// </summary>
		private void LoadNextLevel ()
			{
			// Возобновление игры
			isAlive = true;

			// Запуск фоновой мелодии
			MediaPlayer.Stop ();
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// Поиск следующего имеющегося уровня
			while (true)
				{
				// Поиск С АВТОСМЕЩЕНИЕМ НА СЛЕДУЮЩИЙ УРОВЕНЬ
				++levelNumber;
				if (levelNumber < LevelData.LevelsQuantity)
					break;

				// Перезапуск с нулевого уровня в конце игры
				levelNumber = -1;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// Выгрузка предыдущего уровня и загрузка нового
			if (level != null)
				level.Dispose ();
			level = new TurtleLevel (Services);

			// ПЕРЕЗАГРУЗКА МАССИВОВ МАШИН И СЪЕДОБНЫХ ОБЪЕКТОВ
			// Очистка
			carPosition.Clear ();
			eatable.Clear ();
			GenerateLevelObjects ();

			// Установка стартовых параметров
			playerAnimator.PlayAnimation (playerStayAnimation);
			playerPosition.X = playerAnimation.FrameWidth / 2;
			playerPosition.Y = TurtleGame.GameFieldBottom / 2;
			playerTo.X = 1;
			playerTo.Y = 0;

			// Смена сообщения
			showWinMsg = showLoseMsg = false;
			showLevelMsg = true;

			// Запись настроек и результатов игры (в зависимости от того, есть они или нет)
			GameSettings (true);
			}

		/// <summary>
		/// Метод проверяет столкновение с препятствиями
		/// </summary>
		private bool IsCollapted ()
			{
			// Переменные, принимающие противоположные значения при повороте черепахи; необходимы
			// для корректной проверки столкновений по периметру объекта
			int TValueW = playerAnimation.FrameWidth * (int)Math.Abs (playerTo.X) +
						 playerAnimation.FrameHeight * (int)Math.Abs (playerTo.Y),
				TValueH = playerAnimation.FrameWidth * (int)Math.Abs (playerTo.Y) +
						 playerAnimation.FrameHeight * (int)Math.Abs (playerTo.X);

			// Проверка на столкновение с машиной
			for (int i = 0; i < carPosition.Count; i++)
				if ((Math.Abs (carPosition[i].CurrentPosition.X - playerPosition.X) <
					(TValueW + CarState.DefaultWidth) / 2 - 40) &&                  // -5 - допуск под машину сбоку
					(Math.Abs (carPosition[i].CurrentPosition.Y - playerPosition.Y) <
					(TValueH + CarState.DefaultHeight) / 2 - 60))                   // -10 - допуск под машину спереди
					return true;

			// Не было столкновений
			return false;
			}

		/// <summary>
		/// Метод проверяет съедение объекта
		/// Функция возвращает в качестве X номер элемента для удаления,
		///					   в качестве Y - номер текстуры, от которого зависит количество очков
		/// </summary>
		/// <returns></returns>
		private Vector2 IsAte ()
			{
			for (int i = 0; i < eatable.Count; i++)
				if (GameAuxFunctions.VDist (playerPosition, eatable[i].Position) <
					(playerAnimation.FrameWidth + eatableTextures[0].Width) / 2 - 20)
					return new Vector2 (i, eatable[i].TextureNumber);

			// Не было съедения
			return new Vector2 (-1, 0);
			}

		/// <summary>
		/// Метод отрисовывает текстовую строку
		/// </summary>
		/// <param name="VFont">Шрифт текста</param>
		/// <param name="VString">Строка текста</param>
		/// <param name="VPosition">Позиция отрисовки</param>
		/// <param name="VColor">Цвет текста</param>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, 1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, -1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, 1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, -1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}

		/// <summary>
		/// Метод выполняет чтение / сохранение настроек игры
		/// </summary>
		/// <param name="Write">Флаг режима записи настроек</param>
		private void GameSettings (bool Write)
			{
			// Если требуется запись
			if (Write)
				{
				RDGenerics.SetAppSettingsValue ("Level", levelNumber.ToString ());
				RDGenerics.SetAppSettingsValue ("Score", score.ToString ());
				RDGenerics.SetAppSettingsValue ("Music", isMusic.ToString ());
				RDGenerics.SetAppSettingsValue ("Sound", isSound.ToString ());
				}
			// Если требуется чтение, и файл при этом существует
			else
				{
				try
					{
					levelNumber = int.Parse (RDGenerics.GetAppSettingsValue ("Level"));
					score = int.Parse (RDGenerics.GetAppSettingsValue ("Score"));
					isMusic = bool.Parse (RDGenerics.GetAppSettingsValue ("Music"));
					isSound = bool.Parse (RDGenerics.GetAppSettingsValue ("Sound"));
					}
				catch { }
				}
			}

		/// <summary>
		/// Метод генерирует новые объекты на поле игры
		/// </summary>
		private void GenerateLevelObjects ()
			{
			// Получение доступа к базе уровней
			LevelData LDt = new LevelData (levelNumber);

			// Машины
			for (uint j = 0; j < TurtleGame.LinesQuantity; j++)
				{
				// Для одной полосы скорость всех машин одинакова
				uint s = (uint)rnd.Next (1, LDt.MaxSpeed + 1);
				for (int i = 0; i < LDt.MaxLineCars; i++)
					{
					carPosition.Add (new CarState (rnd.Next (carTextures.Length), s, j));

					// Отступ между машинами
					carPosition[carPosition.Count - 1].SetCurrentPosY (carPosition[carPosition.Count - 1].CurrentPosition.Y +
						i * carPosition[0].UsableDelay * carPosition[carPosition.Count - 1].MoveTo.Y);
					}
				}

			// Съедобные объекты
			for (uint i = 0; i < LDt.MaxLevelObjects; i++)
				eatable.Add (new EatableObjectState (i % (uint)eatableTextures.Length,
					new Vector2 (rnd.Next (RoadLeft, RoadRight), rnd.Next (playerAnimation.FrameWidth / 2,
					GameFieldBottom - playerAnimation.FrameWidth / 2)),
					(float)rnd.Next ((int)(Math.PI * 2000.0)) / 1000.0f));
			}
		}
	}
