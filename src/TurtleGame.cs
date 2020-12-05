using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

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
		private GameStatus gameStatus = GameStatus.Start;   // ��������� ������ ���� (������� ����������� � Auxilitary.cs)

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
		private List<CarState> carPosition = new List<CarState> ();                     // List ������� ���� ����� �� ������
		private List<EatableObjectState> eatable = new List<EatableObjectState> ();     // List ������� ��������� ��������

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
		/// <param name="VGameTime"></param>
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
					if (isAlive)
						{
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
								SFailed.Play ();

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
							SAte.Play ();
							}
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
					(playerPosition.Y - turtleSpeed >= playerAnimation.FrameWidth / 2) /*&& (PlayerTo.Y != 1)*/)
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
					(playerPosition.Y + turtleSpeed <= TurtleGame.GameFieldBottom - playerAnimation.FrameWidth / 2)
						/*&& (PlayerTo.Y != -1)*/)
					{
					playerPosition.Y += turtleSpeed;
					playerTo.X = 0;
					playerTo.Y = 1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// �����
				if (keyboardState.IsKeyDown (Keys.Left) &&
					(playerPosition.X - turtleSpeed >= playerAnimation.FrameWidth / 2) /*&& (PlayerTo.X != 1)*/)
					{
					playerPosition.X -= turtleSpeed;
					playerTo.Y = 0;
					playerTo.X = -1;

					if (playerAnimator.VAnimation == playerStayAnimation)
						playerAnimator.PlayAnimation (playerAnimation);
					}

				// ������
				if (keyboardState.IsKeyDown (Keys.Right) /* ������ ������� - ������� ��������; ����� ��� �� ��������� */
					/*&& (PlayerTo.X != -1)*/)
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
			// ������ ��� �����������
			string S1, S2 = String.Format (" ����: {0,10:D} ", score);
			if (isWorking)
				S1 = String.Format (" ������� {0,2:D} ", levelNumber + 1);
			else
				S1 = " ����� ";

			// ������� ������� ��� ����������� ���������, ����������� �������� ������ ����������
			Vector2 V1 = new Vector2 (16, 16) + level.CameraPosition,
					V2 = new Vector2 (16, 54) + level.CameraPosition,
					V3 = new Vector2 (BackBufferWidth * 0.92f, BackBufferHeight - 32) + level.CameraPosition,
					V4 = new Vector2 (BackBufferWidth * 0.95f, BackBufferHeight - 32) + level.CameraPosition;

			DrawShadowedString (midFont, S1, V1, TurtleGameColors.Orange);
			DrawShadowedString (midFont, S2, V2, TurtleGameColors.Yellow);

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

		/// <summary>
		/// ����������� ��������� �� ������
		/// </summary>
		private void ShowLevelMessage ()
			{
			string S1 = string.Format ("������� {0,2:D}", levelNumber + 1),
					S2 = "������� ������,",
					S3 = "����� ������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S2).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.LBlue_B);
			spriteBatch.DrawString (midFont, S1, V1, TurtleGameColors.LBlue);
			spriteBatch.DrawString (defFont, S2, V2, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.Orange);
			}

		/// <summary>
		/// ����������� ��������� � ������
		/// </summary>
		private void ShowWinMessage ()
			{
			string S1 = "�������",
					S2 = "�������!",
					S3 = "������� ������",
					S4 = "��� �����������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Green_B);
			spriteBatch.DrawString (midFont, S1, V1, TurtleGameColors.Green);
			spriteBatch.DrawString (midFont, S2, V2, TurtleGameColors.Green);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, S4, V4, TurtleGameColors.Orange);
			}

		/// <summary>
		/// ����������� ��������� � ���������
		/// </summary>
		private void ShowLoseMessage ()
			{
			string S1 = "�������",
					S2 = "�� �������!",
					S3 = "������� ������,",
					S4 = "����� ����������� �����";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 60) / 2) + level.CameraPosition,
			V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
				(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Red_B);
			spriteBatch.DrawString (midFont, S1, V1, TurtleGameColors.Red);
			spriteBatch.DrawString (midFont, S2, V2, TurtleGameColors.Red);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, S4, V4, TurtleGameColors.Orange);
			}

		/// <summary>
		/// ����������� ��������� � ������ ����
		/// </summary>
		private void ShowStartMessage ()
			{
			string S1 = ProgramDescription.AssemblyTitle,
					S2 = ProgramDescription.AssemblyCopyright,
					S6 = ProgramDescription.AssemblyLastUpdate,
					S3 = "������� ������ ��� ������ ����,\n",
					S4 = "F1 ��� ������ �������",
					S5 = "��� Esc ��� ������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						120),
					V2 = new Vector2 (BackBufferWidth - defFont.MeasureString (S6).X - 20,
						BackBufferHeight - 70),
					V6 = new Vector2 (BackBufferWidth - defFont.MeasureString (S6).X - 20,
						BackBufferHeight - 40),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						BackBufferHeight / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						BackBufferHeight / 2 + 30),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S5).X) / 2,
						BackBufferHeight / 2 + 60);

			spriteBatch.Draw (startBack, Vector2.Zero, TurtleGameColors.White);
			DrawShadowedString (bigFont, S1, V1, TurtleGameColors.Orange);
			DrawShadowedString (defFont, S2, V2, TurtleGameColors.Yellow);
			DrawShadowedString (defFont, S6, V6, TurtleGameColors.Yellow);
			DrawShadowedString (defFont, S3, V3, TurtleGameColors.White);
			DrawShadowedString (defFont, S4, V4, TurtleGameColors.White);
			DrawShadowedString (defFont, S5, V5, TurtleGameColors.White);
			}

		/// <summary>
		/// ����������� ��������� � ����� ����
		/// </summary>
		private void ShowFinishMessage ()
			{
			string S1 = "�� ��������!!!",
					S2 = string.Format ("��� �������: {0,10:D} �����", score),
					S3 = "������� ������ ��� �����������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 400) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 50) / 2),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 100) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, TurtleGameColors.White);
			spriteBatch.DrawString (bigFont, S1, V1, TurtleGameColors.Orange);
			spriteBatch.DrawString (midFont, S2, V2, TurtleGameColors.Brown);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.DBlue);
			}

		/// <summary>
		/// ����������� ������� �� ������������� ������
		/// </summary>
		private void ShowExitMessage ()
			{
			string S1 = "�� ������������� ������",
					S2 = "��������� ����?",
					S3 = "������� Y, ����� ����� �� ����,",
					S4 = "��� N, ����� ���������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 180) / 2) + level.CameraPosition,
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 110) / 2) + level.CameraPosition,
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 70) / 2) + level.CameraPosition,
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						(BackBufferHeight + 110) / 2) + level.CameraPosition;

			spriteBatch.Draw (messageBack, GameAuxFunctions.CenterOf (messageBackLeftTop, level.CameraPosition),
				TurtleGameColors.Yellow_B);
			spriteBatch.DrawString (midFont, S1, V1, TurtleGameColors.Yellow);
			spriteBatch.DrawString (midFont, S2, V2, TurtleGameColors.Yellow);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.Orange);
			spriteBatch.DrawString (defFont, S4, V4, TurtleGameColors.Orange);
			}

		/// <summary>
		/// ����������� �������
		/// </summary>
		private void ShowHelpMessage ()
			{
			string S1 = "������� ����",
					S2 = "   � ���� ���������� ����� ���� ��������� ��������� ����� ������ � ��\n" +
						 "������� ��� ����������� ����������. �������� � ���������� �����������\n" +
						 "����� ����� � ������ �������. ������� �������, ����� �������� ������,\n" +
						 "������� ����� ��������� ������ ���������� �����. ������ ����, �����\n" +
						 "������ � ������ � ��� ���������� ������",
					S3 = "�����!!!",
					S4 = "����������",
					S5 = "������ - ����� / ������������� / ������ ����         S - ��������� / ���������� �����\n" +
						 "������� - ���������� ����������                            M - ��������� / ���������� ������\n" +
						 "Esc - ����� �� ���� / �� �������";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						BackBufferHeight / 2 - 290),
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S2).X) / 2,
						BackBufferHeight / 2 - 240),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						BackBufferHeight / 2 - 120),
					V4 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S4).X) / 2,
						BackBufferHeight / 2 - 60),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S5).X) / 2,
						BackBufferHeight / 2 - 10);

			spriteBatch.Draw (startBack, Vector2.Zero, TurtleGameColors.White);
			spriteBatch.DrawString (midFont, S1, V1, TurtleGameColors.White);
			spriteBatch.DrawString (defFont, S2, V2, TurtleGameColors.White);
			spriteBatch.DrawString (defFont, S3, V3, TurtleGameColors.White);
			spriteBatch.DrawString (midFont, S4, V4, TurtleGameColors.White);
			spriteBatch.DrawString (defFont, S5, V5, TurtleGameColors.White);
			}

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
					ShowHelpMessage ();

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
			string FN = "C:\\Docume~1\\Alluse~1\\Applic~1\\Microsoft\\Windows\\TurtleGame.sav";

			// ���� ��������� ������
			if (Write)
				{
				Directory.CreateDirectory (FN.Substring (0, FN.Length - 14));
				StreamWriter FL = new StreamWriter (FN, false);

				FL.Write ("{0:D}\n{1:D}\n{2:D}\n{3:D}", levelNumber, score, isMusic, isSound);

				FL.Close ();
				}
			// ���� ��������� ������, � ���� ��� ���� ����������
			else if (File.Exists (FN))
				{
				StreamReader FL = new StreamReader (FN);

				levelNumber = int.Parse (FL.ReadLine ());
				score = int.Parse (FL.ReadLine ());
				isMusic = bool.Parse (FL.ReadLine ());
				isSound = bool.Parse (FL.ReadLine ());

				FL.Close ();
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
