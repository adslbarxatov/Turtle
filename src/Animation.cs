using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Файл анимации, основанный на файле Animation.cs для Microsoft Platformer, был
// почти полностью переписан, т.к. обладал дефектами и недоработками, несовместимыми
// с правильной работой программы

namespace RD_AAOW
	{
	/// <summary>
	/// Класс представляет анимированную структуру.
	/// Размер кадра рассчитывается относительно размеров Tile
	/// </summary>
	public class Animation
		{
		/// <summary>
		/// Текстура анимации.
		/// PNG-анимация должна быть горизонтальной
		/// </summary>
		public Texture2D Texture
			{
			get
				{
				return texture;
				}
			}
		private Texture2D texture;

		/// <summary>
		/// Длительность отображения каждого кадра
		/// </summary>
		public float FrameTime
			{
			get
				{
				return frameTime;
				}
			}
		private float frameTime;

		/// <summary>
		/// Флаг, указывающий, следует ли повторять анимацию бесконечно или единожды?
		/// </summary>
		public bool IsLooping
			{
			get
				{
				return isLooping;
				}
			}
		private bool isLooping;

		/// <summary>
		/// Количество кадров в анимации
		/// </summary>
		public int FrameCount
			{
			get
				{
				return texture.Width / frameWidth;
				}
			}

		/// <summary>
		/// Ширина кадра
		/// </summary>
		public int FrameWidth
			{
			get
				{
				return frameWidth;
				}
			}
		private int frameWidth;

		/// <summary>
		/// Высота кадра
		/// </summary>
		public int FrameHeight
			{
			get
				{
				return texture.Height;
				}
			}

		/// <summary>
		/// Размер кадра анимации
		/// </summary>
		public Vector2 FrameSize
			{
			get
				{
				return new Vector2 (frameWidth, FrameHeight);
				}
			}

		/// <summary>
		/// Конструктор. Инициализирует анимированную текстуру
		/// </summary>
		/// <param name="InTexture">Текстура для построения анимации</param>
		/// <param name="InFrameWidth">Ширина кадра</param>
		/// <param name="InFrameTime">Длительность кадра</param>
		/// <param name="InIsLooping">Флаг зацикливания анимации</param>
		public Animation (Texture2D InTexture, int InFrameWidth, float InFrameTime, bool InIsLooping)
			{
			texture = InTexture;
			frameTime = InFrameTime;
			isLooping = InIsLooping;
			frameWidth = InFrameWidth;
			}
		}

	/// <summary>
	/// Метаданные анимации
	/// </summary>
	public struct AnimationPlayer
		{
		/// <summary>
		/// Собственно графические данные
		/// </summary>
		public Animation VAnimation;

		/// <summary>
		/// Индекс текущего кадра
		/// </summary>
		public int FrameIndex;

		/// <summary>
		/// Центровая точка кадра
		/// </summary>
		public Vector2 Origin
			{
			get
				{
				return new Vector2 (VAnimation.FrameWidth / 2.0f, VAnimation.FrameHeight / 2.0f);
				}
			}

		/// <summary>
		/// Метод запускает или продолжает анимацию
		/// </summary>
		/// <param name="animation"></param>
		public void PlayAnimation (Animation animation)
			{
			// Если уже запущена, не перезапускать
			if (VAnimation == animation)
				return;

			// Запуск новой анимации
			VAnimation = animation;
			FrameIndex = 0;
			}

		/// <summary>
		/// Метод наращивает счётчик времени и отображает текущий кадр
		/// </summary>
		/// <param name="VGameTime">Время внутри игры</param>
		/// <param name="VSpriteBatch"></param>
		/// <param name="Position">Позиция отрисовки анимации</param>
		/// <param name="VSpriteEffects">Эффекты отрисовки</param>
		/// <param name="DrColor">Цвет наложения</param>
		/// <param name="Angle">Угол поворота</param>
		public void Draw (GameTime VGameTime, SpriteBatch VSpriteBatch, Vector2 Position,
			SpriteEffects VSpriteEffects, Color DrColor, double Angle)
			{
			// Индекс кадра анимации
			FrameIndex = (int)((float)VGameTime.TotalGameTime.TotalSeconds /
				VAnimation.FrameTime);

			// Обработка случая одноразового проигрывания анимации
			if (VAnimation.IsLooping)
				{
				FrameIndex %= VAnimation.FrameCount;
				}
			else
				{
				if (FrameIndex > VAnimation.FrameCount - 1)
					FrameIndex = VAnimation.FrameCount - 1;
				}

			// Вычисление поля для анимации
			Rectangle source = new Rectangle (FrameIndex * VAnimation.FrameWidth, 0,
				VAnimation.FrameWidth, VAnimation.FrameHeight);

			// Отображение кадра
			VSpriteBatch.Draw (VAnimation.Texture, Position, source, DrColor,
				(float)Angle, Origin, 1.0f, VSpriteEffects, 0.0f);
			}
		}
	}
