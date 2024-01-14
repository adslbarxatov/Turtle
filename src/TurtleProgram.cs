using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает точку входа приложения
	/// </summary>
	public static class TurtleProgram
		{
		private static string[] Pths = {
			// Все фоновые изображения
			"Background\\Back.xnb",
			"Background\\StartBack.xnb",

			// Шрифты
			"Font\\BigFont.xnb",
			"Font\\DefFont.xnb",
			"Font\\MidFont.xnb",

			// Фоны сообщений
			"Messages\\MessageBack.xnb",

			// Все мелодии
			"Sounds\\Ate1.xnb",
			"Sounds\\Completed.xnb",
			"Sounds\\Failed.xnb",
			"Sounds\\Music1.wma",
			"Sounds\\Music1.xnb",
			"Sounds\\Music2.wma",
			"Sounds\\Music2.xnb",
			"Sounds\\SoundOnOff.xnb",
			"Sounds\\SStop.xnb",
			"Sounds\\SStart.xnb",

			// Все tiles
			"Tiles\\Car00.xnb",
			"Tiles\\Car01.xnb",
			"Tiles\\Car02.xnb",
			"Tiles\\Car03.xnb",
			"Tiles\\Car04.xnb",
			"Tiles\\Car05.xnb",
			"Tiles\\Car06.xnb",
			"Tiles\\Car07.xnb",
			"Tiles\\Car08.xnb",
			"Tiles\\DeadTurtle.xnb",
			"Tiles\\Eatable00.xnb",
			"Tiles\\Eatable01.xnb",
			"Tiles\\Eatable02.xnb",
			"Tiles\\Eatable03.xnb",
			"Tiles\\Eatable04.xnb",
			"Tiles\\Turtle.xnb"};

		/// <summary>
		/// Точка входа приложения
		/// </summary>
		/// <param name="args">Аргументы командной строки</param>
		public static void Main (string[] args)
			{
			// Инициализация
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Контроль XPUN
			if (!RDLocale.IsXPUNClassAcceptable)
				return;

			// Проверка запуска единственной копии
			if (!RDGenerics.IsAppInstanceUnique (true))
				return;

			// Отображение справки и запроса на принятие Политики
			if (!RDGenerics.AcceptEULA ())
				return;
			RDGenerics.ShowAbout (true);

			// Контроль прав
			bool _ = RDGenerics.IsRegistryAccessible;

			// Выполнение проверки на наличие всех необходимых файлов
			/*for (int i = 0; i < Pths.Length; i++)
				if (!File.Exists (".\\Content\\Turtle\\" + Pths[i]))
					{
					RDGenerics.LocalizedMessageBox (RDMessageTypes.Error_Center, "MissingFile");
					return;
					}*/
			if (!RDGenerics.CheckLibraries (Pths, "Content\\" + ProgramDescription.AssemblyMainName +
				"\\", true))
				return;

			using (TurtleGame game = new TurtleGame ())
				game.Run ();
			}
		}
	}
