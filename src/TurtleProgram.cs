using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// ����� ��������� ����� ����� ����������
	/// </summary>
	public static class TurtleProgram
		{
		/// <summary>
		/// ����� ����� ����������
		/// </summary>
		/// <param name="args">��������� ��������� ������</param>
		public static void Main (string[] args)
			{
			string[] Pths = {// ��� ������� �����������
                            "Background\\Back.xnb",
							"Background\\StartBack.xnb",

                            // ������
                            "Font\\BigFont.xnb",
							"Font\\DefFont.xnb",
							"Font\\MidFont.xnb",

                            // ���� ���������
                            "Messages\\MessageBack.xnb",

                            // ��� �������
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

					        // ��� tiles
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

			// ���������� �������� �� ������� ���� ����������� ������
			for (int i = 0; i < Pths.Length; i++)
				if (!File.Exists (".\\Content\\Turtle\\" + Pths[i]))
					{
					MessageBox.Show ("����������� ���� ���� \xAB" + Pths[i] +
						"\xBB\n��������� ��������� ���������� ��������� ��� ���������� � ������������",
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
					}

			using (TurtleGame game = new TurtleGame ())
				game.Run ();
			}
		}
	}
