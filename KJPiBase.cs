using System.Diagnostics;
using System;
using System.Globalization;
using System.Linq;

namespace KJPi.Base
{
    public static class KJPiString
    {
        /// <summary>
        /// Метод, занимающийся проверкой является ли строка числом.
        /// </summary>
        /// <param name="input">На вход подаётся проверяемая строка</param>
        /// <returns>Возвращает результат, сравнения является ли строка числом.</returns>
        public static bool IsNumeric(string input)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
        }
    }

    public static class KJPiTime
    {
        /// <summary>
        /// Метод, занимающийся получением времени Wav файла
        /// </summary>
        /// <param name="workDirectory">На вход подаётся место, где хранится файл.</param>
        /// <param name="toOutputWavFile">На вход подаётся путь до WAV файла.</param>
        /// <returns>Возвращает время Wav файла.</returns>
        public static TimeSpan GetTotalTime(string workDirectory, string toOutputWavFile)
        {
            string command = $"ffprobe -i \"{toOutputWavFile}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";

            // Создаем новый процесс
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = workDirectory,
                }
            };

            // Запускаем процесс
            process.Start();

            // Передаем команду в стандартный ввод процесса
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            string output = process.StandardOutput.ReadToEnd();
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] linesWithOnlyNumbers = lines.Where(line => KJPiString.IsNumeric(line)).ToArray();
            double.TryParse(linesWithOnlyNumbers[linesWithOnlyNumbers.Length - 1],
                NumberStyles.Any, CultureInfo.InvariantCulture, out double r);
            TimeSpan timeSpan = TimeSpan.FromSeconds(r);

            // Ожидаем завершения процесса
            process.WaitForExit();

            return timeSpan;
        }
    }
}
