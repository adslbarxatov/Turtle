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
	/// ����� ��������� ���� ���������
	/// </summary>
	public class TurtleGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ����������

		// �������� ����
		private GraphicsDeviceManager graphics;             // �������
		private SpriteBatch spriteBatch;                    // Sprite-���������
		private KeyboardState keyboardState;                // ��������� ����������
		private SpriteFont defFont, midFont, bigFont;       // ������
		private Random rnd = new Random ();                 // ���

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferWidth = 1200;

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferHeight = 640;

		/// <summary>
		/// ������ ������
		/// </summary>
		public const int GameFieldBottom = BackBufferHeight * 2;

		/// <summary>
		/// ������ �������� ������
		/// </summary>
		public const int RoadLineWidth = 200;

		/// <summary>
		/// ���������� �����
		/// </summary>
		public const int LinesQuantity = 4;

		/// <summary>
		/// ����� ������� �����
		/// </summary>
		public const int RoadLeft = 200;

		/// <summary>
		/// ������ ������� �����
		/// </summary>
		public const int RoadRight = RoadLeft + RoadLineWidth * LinesQuantity;

		// �������� ��������� ���� (������|����|�����)
		private GameStatus gameStatus = GameStatus.Start;
		// ��������� ������ ���� (������� ����������� � Auxilitary.cs)

		// ��������� ������ � ���� ���������
		private TurtleLevel level;                          // �����-��������� ������
		private int levelNumber = 0;                        // ����� �������� ������
		private Texture2D messageBack,                      // ��� ���������
						  startBack;                        // ��� �� ������
		private Vector2 messageBackLeftTop;
		private const int turtleSpeed = 4;                  // �������� ��������

		// ������� ������� �������� � �� ������� ��������
		private Vector2 playerPosition, playerTo;                   // ������� �������
		private Animation playerAnimation, playerStayAnimation,     // ����������� �������� (��������, �����, dead)
						  deadAnimation;
		private AnimationPlayer playerAnimator;                     // ������-��������

		// �������� ����� � ��������� ��������
		private Texture2D[] carTextures, eatableTextures;           // ������� �������

		// ������� ��������� ���� ����� � ��������� �������� (���������, �����������, �������� � �.�.) 
		// (����������� ������ ������� � Level.cs)

		private List<CarState> carPosition = new List<CarState> ();
		// List ������� ���� ����� �� ������
		private List<EatableObjectState> eatable = new List<EatableObjectState> ();
		// List ������� ��������� ��������

		// �������� ������� � �� ���������
		private SoundEffect SCompleted, SFailed,            // ������, ���������
							SStart, SStop, SOnOff,          // �����, �����, ���� off/on
							SAte;                           // ��������
		private bool isSound = true, isMusic = true;        // ���� � ������ � ���� on/off

		// ��������� Alive � Working
		private bool isAlive = false, isWorking = false;

		// ����
		private int score = 0;                              // �������
		private const uint scoreMultiplier = 10;            // ��������� ��� �����
		private const int penalty = 99;                     // ����� �� ��������

		// ����� ����������� ���������
		private bool showLevelMsg = false,      // ��������� � ������ ������
					 showLoseMsg = false,       // ��������� � ����������� ������
					 showWinMsg = false,        // ��������� � ���������
					 showExitMsg = false;       // ������������� ������

		// ������������� ����������
		private int kbdDelay = 1,               // ����� � Update-��������� ����� ��������� ������� ����������
					kbdDelayTimer;              // ������ ��� delay
		private const int kbdDefDelay = 25;     // ������� delay ��� ������� �������

		/// <summary>
		/// �����������. ��������� ������� ������� � ���� ����������
		/// </summary>
		public TurtleGame ()
			{
			// �������� "����" ��������� �������
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
			//graphics.ToggleFullScreen ();

			// ������� content-���������� ����
			Content.RootDirectory = "Content/Turtle";
			}

		/// <summary>
		/// �������������
		/// ������� ����������� ���� ��� �� ����, ��� � �������
		/// ����� ������������� ��� ������������� � ��������� ��������
		/// </summary>
		protected override void Initialize ()
			{
			// ��������� �������� ����������
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// �������� ����������� ��������
			// ������ �������� �����
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

			// ������ �������� ��������� ��������
			eatableTextures = new Texture2D[]   {
				Content.Load<Texture2D> ("Tiles/Eatable00"),
				Content.Load<Texture2D> ("Tiles/Eatable01"),
				Content.Load<Texture2D> ("Tiles/Eatable02"),
				Content.Load<Texture2D> ("Tiles/Eatable03"),
				Content.Load<Texture2D> ("Tiles/Eatable04")
											};

			// �������� ��� ������������ � ��������
			playerAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Turtle"), 123, 0.1f, true);
			playerStayAnimation = new Animation (Content.Load<Texture2D> ("Tiles/Turtle"), 123, 0.1f, false);
			deadAnimation = new Animation (Content.Load<Texture2D> ("Tiles/DeadTurtle"), 190, 0.1f, false);
			playerAnimator.PlayAnimation (playerStayAnimation);         // �� ��������� - �������� ��� �����

			// �������� �������� ��������
			SCompleted = Content.Load<SoundEffect> ("Sounds/Completed");
			SFailed = Content.Load<SoundEffect> ("Sounds/Failed");
			SOnOff = Content.Load<SoundEffect> ("Sounds/SoundOnOff");
			SStart = Content.Load<SoundEffect> ("Sounds/SStart");
			SStop = Content.Load<SoundEffect> ("Sounds/SStop");
			SAte = Content.Load<SoundEffect> ("Sounds/Ate1");

			// �������� �������
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");

			// �������� �������������� �������
			messageBack = Content.Load<Texture2D> ("Messages/MessageBack");
			startBack = Content.Load<Texture2D> ("Background/StartBack");
			messageBackLeftTop = new Vector2 (BackBufferWidth - messageBack.Width, BackBufferHeight - messageBack.Height);

			// ������ �������� � ����������� ����
			GameSettings (false);

			// ��������� ������
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// �������������
			base.Initialize ();
			}

		/// <summary>
		/// ����� ��������� ��������� ���� � �������� �������
		/// </summary>
		protected override void Update (GameTime VGameTime)
			{
			// ����� ���������� � ��������������� ��������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					if (!isAlive)
						break;

					// ������� (������ ������ ������ � ������)
					if (playerPosition.X > level.Background.Width - playerAnimation.FrameWidth / 2)
						{
						// ����
						MediaPlayer.Stop ();
						if (isSound)
							SCompleted.Play ();

						// ����������� ���������
						showWinMsg = true;

						// ������������ ����������
						isAlive = isWorking = false;
						playerAnimator.PlayAnimation (playerStayAnimation);

						// ���������� ������ ��������� �� ������� �������
						}

					// �������� ���� �����
					if (isWorking)
						{
						for (int i = 0; i < carPosition.Count; i++)
							{
							// ��������
							carPosition[i].CurrentPosition += carPosition[i].MoveTo * carPosition[i].Speed;

							// ����� �� ������� ������
							if ((carPosition[i].CurrentPosition.Y < -CarState.DefaultHeight) ||
								(carPosition[i].CurrentPosition.Y > TurtleGame.GameFieldBottom + CarState.DefaultHeight))
								// ��� ������ �� ������� ������ ������ ��������� �� �������, � ������ ��
								// �������� �����, �� � ��� �� ��������� � �� ��� �� ������
								carPosition[i].CurrentPosition = carPosition[i].StartPosition;
							}
						}

					// �������� ������������ � ��������
					if (IsCollapted ())
						{
						// ����
						MediaPlayer.Stop ();
						if (isSound)
							SFailed.Play ((90 + rnd.Next (10)) * 0.01f,
							(20 - rnd.Next (40)) * 0.01f, 
							(playerPosition.X - BackBufferWidth / 2.0f) / (BackBufferWidth / 2.0f));

						// ������������ ��������� ����
						isAlive = isWorking = false;
						levelNumber--;
						playerAnimator.PlayAnimation (deadAnimation);

						// ����������� ���������
						showLoseMsg = true;

						// �������� �����
						score -= penalty;           // ������ ������

						// ���������� ������ ��������� �� ������� ������� Space
						}

					// �������� ��������
					Vector2 k = IsAte ();
					if (k.X != -1.0f)
						{
						// �������� ���������� �������
						eatable.RemoveAt ((int)k.X);

						// �������� �����
						score += (int)((k.Y + 1.0f) * scoreMultiplier);

						// ����
						if (isSound)
							SAte.Play ((90 + rnd.Next (10)) * 0.01f,
							(20 - rnd.Next (40)) * 0.01f,
							(playerPosition.X - BackBufferWidth / 2.0f) / (BackBufferWidth / 2.0f));
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// ���������� ����
			base.Update (VGameTime);
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ��������������� �������
		/// </summary>
		private bool KeyboardProc ()
			{
			// ������ � ����������
			keyboardState = Keyboard.GetState ();

			// � ������������� �� ��������� ����
			// ��������� �����
			if (!showExitMsg)
				{
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// ���� ������ �������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// ����������� �����
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// �������
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// ����� ����� ���������
					if (keyboardState.IsKeyDown (Keys.L))
						{
						gameStatus = GameStatus.Language;

						return true;
						}

					// ������� �����
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// ������������ ����������
						gameStatus = GameStatus.Playing;

						// �������� ������
						levelNumber--;
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
				case GameStatus.Language:
					// �������
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// ������� ����� � �����������
					if (!showExitMsg)           // ������ ������ ������, ���� ��������� ��������� � ������
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

						// ������� ������� �����������
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// �������� �� �����
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// �����
							isWorking = false;

							// ���������
							showExitMsg = true;

							// ����
							if (isSound)
								SStart.Play ();

							return true;
							}
						}

					// ������� ������
					if (showExitMsg)
						{
						// ����� �� ���� (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// ����������� (back)
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
						// ������������
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// �� ���� �� ������ �������
			return false;
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ���������������� �������
		/// </summary>
		private void KeyboardMoveProc ()
			{
			keyboardState = Keyboard.GetState ();

			// ������� ������ ����������
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				// �����

				// ������ ������� �������� �� ���������� �������� ������ ������ ���� ����
				// ������ ������� �������� �� ������������� ��������� �� �����
				if (keyboardState.IsKeyDown (Keys.Up) &&
					(playerPosition.Y - turtleSpeed >= playerAnimation.FrameWidth / 2))
					{
					// ����� ������� ��� ��������
					playerPosition.Y -= turtleSpeed;
					playerTo.X = 0;
					playerTo.Y = -1;

					// ����� �������� �� ����������
					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ����
				if (keyboardState.IsKeyDown (Keys.Down) &&
					(playerPosition.Y + turtleSpeed <= TurtleGame.GameFieldBottom - playerAnimation.FrameWidth / 2))
					{
					playerPosition.Y += turtleSpeed;
					playerTo.X = 0;
					playerTo.Y = 1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// �����
				if (keyboardState.IsKeyDown (Keys.Left) &&
					(playerPosition.X - turtleSpeed >= playerAnimation.FrameWidth / 2))
					{
					playerPosition.X -= turtleSpeed;
					playerTo.Y = 0;
					playerTo.X = -1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ������
				if (keyboardState.IsKeyDown (Keys.Right))
					{
					playerPosition.X += turtleSpeed;
					playerTo.Y = 0;
					playerTo.X = 1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ����� �������� ��� ���������
				if ((keyboardState.IsKeyUp (Keys.Up)) && (keyboardState.IsKeyUp (Keys.Down)) &&
					(keyboardState.IsKeyUp (Keys.Left)) && (keyboardState.IsKeyUp (Keys.Right)) &&
					(playerAnimator.VAnimation == playerAnimation))
					playerAnimator.PlayAnimation (playerStayAnimation);
				}
			}

		/// <summary>
		/// ����� ���������� ���������� ���� (���� � �������)
		/// </summary>
		private void DrawInfo ()
			{
			// ������ �����
			if (string.IsNullOrWhiteSpace (stScoreLines[0]))
				{
				string[] values = Localization.GetText ("ScoreLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stScoreLines.Length; i++)
					stScoreLines[i] = values[i];
				}
			string S00 = string.Format (stScoreLines[0], score);
			string S01 = isWorking ? string.Format (stScoreLines[1], levelNumber + 1) : stScoreLines[2];

			// ������� ������� ��� ����������� ���������, ����������� �������� ������ ����������
			Vector2 V1 = new Vector2 (12, 16) + level.CameraPosition,
					V2 = new Vector2 (12, 54) + level.CameraPosition,
					V3 = new Vector2 (BackBufferWidth * 0.92f, BackBufferHeight - 32) + level.CameraPosition,
					V4 = new Vector2 (BackBufferWidth * 0.95f, BackBufferHeight - 32) + level.CameraPosition;

			DrawShadowedString (midFont, S00, V1, TurtleGameColors.Orange);
			DrawShadowedString (midFont, S01, V2, TurtleGameColors.Yellow);

			// ���� ���� ������ ��� ����, �������� ��������������� ����
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
		/// ����������� ��������� �� ������
		/// </summary>
		private void ShowLevelMessage ()
			{
			// ������ �����
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
		/// ����������� ��������� � ������
		/// </summary>
		private void ShowWinMessage ()
			{
			// ������ �����
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
		/// ����������� ��������� � ���������
		/// </summary>
		private void ShowLoseMessage ()
			{
			// ������ �����
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
		/// ����������� ��������� � ������ ����
		/// </summary>
		private void ShowStartMessage ()
			{
			// ������ �����
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
		/// ����������� ��������� � ����� ����
		/// </summary>
		private void ShowFinishMessage ()
			{
			// ������ �����
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
		/// ����������� ������� �� ������������� ������
		/// </summary>
		private void ShowExitMessage ()
			{
			// ������ �����
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
		/// ����������� ��������������� �����������
		/// </summary>
		private void ShowServiceMessage (bool Language)
			{
			// ������ �� �������������� �����
			if (showingServiceMessage)
				return;
			showingServiceMessage = true;

			// ���������� ��������� � ������ �������
			spriteBatch.End ();

			if (Language)
				RDGenerics.MessageBox ();
			else
				RDGenerics.ShowAbout (false);

			// ������� � �������� ���������
			spriteBatch.Begin ();

			// ����� � ����
			gameStatus = GameStatus.Start;
			showingServiceMessage = false;
			}
		private bool showingServiceMessage = false;

		/// <summary>
		/// ����� ������������ ������� ����
		/// </summary>
		/// <param name="VGameTime"></param>
		protected override void Draw (GameTime VGameTime)
			{
			// �������� ������� ���� � ������ ���������
			graphics.GraphicsDevice.Clear (TurtleGameColors.Black);
			spriteBatch.Begin ();

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// ���������� ��������� �����
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
					// ����������� ������
					level.Draw (VGameTime, spriteBatch, playerPosition);

					// ����������� �����������
					// ��������� �������
					for (int i = 0; i < eatable.Count; i++)
						spriteBatch.Draw (eatableTextures[eatable[i].TextureNumber],
							eatable[i].DestinationRect, eatable[i].SourceRect, TurtleGameColors.White,
							eatable[i].Turn, eatable[i].Origin, SpriteEffects.None, 0.0f);

					// ����� (��� ����)
					playerAnimator.Draw (VGameTime, spriteBatch, playerPosition, SpriteEffects.None, TurtleGameColors.White,
						// ��������� ���� �������� ��������
						Math.Acos (playerTo.X) * GameAuxFunctions.NNSign (playerTo.Y, false));

					// ���������� (��� ����)
					for (int i = 0; i < carPosition.Count; i++)
						spriteBatch.Draw (carTextures[carPosition[i].TextureNumber],
							carPosition[i].DestinationRect, carPosition[i].SourceRect, TurtleGameColors.White,
							carPosition[i].Turn, carPosition[i].Origin, SpriteEffects.None, 0.0f);

					// ����������� ���������� ������
					DrawInfo ();

					// ����������� ��������� (���� ��� �������)
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

			// ���������� ���������
			spriteBatch.End ();

			// �����������
			base.Draw (VGameTime);
			}

		/// <summary>
		/// ����� ��������� ��������� ������� ����
		/// </summary>
		private void LoadNextLevel ()
			{
			// ������������� ����
			isAlive = true;

			// ������ ������� �������
			MediaPlayer.Stop ();
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// ����� ���������� ���������� ������
			while (true)
				{
				// ����� � ������������� �� ��������� �������
				++levelNumber;
				if (levelNumber < LevelData.LevelsQuantity)
					break;

				// ���������� � �������� ������ � ����� ����
				levelNumber = -1;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// �������� ����������� ������ � �������� ������
			if (level != null)
				level.Dispose ();
			level = new TurtleLevel (Services);

			// ������������ �������� ����� � ��������� ��������
			// �������
			carPosition.Clear ();
			eatable.Clear ();
			GenerateLevelObjects ();

			// ��������� ��������� ����������
			playerAnimator.PlayAnimation (playerStayAnimation);
			playerPosition.X = playerAnimation.FrameWidth / 2;
			playerPosition.Y = TurtleGame.GameFieldBottom / 2;
			playerTo.X = 1;
			playerTo.Y = 0;

			// ����� ���������
			showWinMsg = showLoseMsg = false;
			showLevelMsg = true;

			// ������ �������� � ����������� ���� (� ����������� �� ����, ���� ��� ��� ���)
			GameSettings (true);
			}

		/// <summary>
		/// ����� ��������� ������������ � �������������
		/// </summary>
		private bool IsCollapted ()
			{
			// ����������, ����������� ��������������� �������� ��� �������� ��������; ����������
			// ��� ���������� �������� ������������ �� ��������� �������
			int TValueW = playerAnimation.FrameWidth * (int)Math.Abs (playerTo.X) +
						 playerAnimation.FrameHeight * (int)Math.Abs (playerTo.Y),
				TValueH = playerAnimation.FrameWidth * (int)Math.Abs (playerTo.Y) +
						 playerAnimation.FrameHeight * (int)Math.Abs (playerTo.X);

			// �������� �� ������������ � �������
			for (int i = 0; i < carPosition.Count; i++)
				if ((Math.Abs (carPosition[i].CurrentPosition.X - playerPosition.X) <
					(TValueW + CarState.DefaultWidth) / 2 - 40) &&                  // -5 - ������ ��� ������ �����
					(Math.Abs (carPosition[i].CurrentPosition.Y - playerPosition.Y) <
					(TValueH + CarState.DefaultHeight) / 2 - 60))                   // -10 - ������ ��� ������ �������
					return true;

			// �� ���� ������������
			return false;
			}

		/// <summary>
		/// ����� ��������� �������� �������
		/// ������� ���������� � �������� X ����� �������� ��� ��������,
		///					   � �������� Y - ����� ��������, �� �������� ������� ���������� �����
		/// </summary>
		/// <returns></returns>
		private Vector2 IsAte ()
			{
			for (int i = 0; i < eatable.Count; i++)
				if (GameAuxFunctions.VDist (playerPosition, eatable[i].Position) <
					(playerAnimation.FrameWidth + eatableTextures[0].Width) / 2 - 20)
					return new Vector2 (i, eatable[i].TextureNumber);

			// �� ���� ��������
			return new Vector2 (-1, 0);
			}

		/// <summary>
		/// ����� ������������ ��������� ������
		/// </summary>
		/// <param name="VFont">����� ������</param>
		/// <param name="VString">������ ������</param>
		/// <param name="VPosition">������� ���������</param>
		/// <param name="VColor">���� ������</param>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, 1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, -1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, 1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, -1), TurtleGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}

		/// <summary>
		/// ����� ��������� ������ / ���������� �������� ����
		/// </summary>
		/// <param name="Write">���� ������ ������ ��������</param>
		private void GameSettings (bool Write)
			{
			// ���� ��������� ������
			if (Write)
				{
				RDGenerics.SetAppSettingsValue ("Level", levelNumber.ToString ());
				RDGenerics.SetAppSettingsValue ("Score", score.ToString ());
				RDGenerics.SetAppSettingsValue ("Music", isMusic.ToString ());
				RDGenerics.SetAppSettingsValue ("Sound", isSound.ToString ());
				}
			// ���� ��������� ������, � ���� ��� ���� ����������
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
		/// ����� ���������� ����� ������� �� ���� ����
		/// </summary>
		private void GenerateLevelObjects ()
			{
			// ��������� ������� � ���� �������
			LevelData LDt = new LevelData (levelNumber);

			// ������
			for (uint j = 0; j < TurtleGame.LinesQuantity; j++)
				{
				// ��� ����� ������ �������� ���� ����� ���������
				uint s = (uint)rnd.Next (1, LDt.MaxSpeed + 1);
				for (int i = 0; i < LDt.MaxLineCars; i++)
					{
					carPosition.Add (new CarState (rnd.Next (carTextures.Length), s, j));

					// ������ ����� ��������
					carPosition[carPosition.Count - 1].SetCurrentPosY (carPosition[carPosition.Count - 1].CurrentPosition.Y +
						i * carPosition[0].UsableDelay * carPosition[carPosition.Count - 1].MoveTo.Y);
					}
				}

			// ��������� �������
			for (uint i = 0; i < LDt.MaxLevelObjects; i++)
				eatable.Add (new EatableObjectState (i % (uint)eatableTextures.Length,
					new Vector2 (rnd.Next (RoadLeft, RoadRight), rnd.Next (playerAnimation.FrameWidth / 2,
					GameFieldBottom - playerAnimation.FrameWidth / 2)),
					(float)rnd.Next ((int)(Math.PI * 2000.0)) / 1000.0f));
			}
		}
	}
