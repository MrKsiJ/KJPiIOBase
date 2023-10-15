using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace KJPi.IO
{
    public static class KJPiDirectory
    {
        /// <summary>
        /// Метод, занимающийся созданием директории
        /// </summary>
        /// <param name="workDirectory">На вход подаётся место, где будет создана новая директория.</param>
        /// <param name="nameCreatedFolder">На вход подаётся имя создаваемой директории.</param>
        public static void CreateDirectory(string workDirectory, string nameCreatedFolder)
        {
            string command = $"mkdir \"{nameCreatedFolder}\"";

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

            // Ожидаем завершения процесса
            process.WaitForExit();
        }
        /// <summary>
        /// Метод, занимающийся получением директорий
        /// </summary>
        /// <param name="workDirectory">На вход подаётся директория, в которой будет произведён поиск всех директорий</param>
        /// <returns>Возвращает пути к найденным директориям</returns>
        public static string[] GetDirectories(string workDirectory)
        {
            string homePath = workDirectory;
            // Формируем строку команды, объединяя две команды через символ &
            string command1 = "chcp 65001";
            string command2 = "dir";

            // Создаем новый процесс
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = homePath,
                }
            };

            // Запускаем процесс
            process.Start();

            // Передаем команду в стандартный ввод процесса
            process.StandardInput.WriteLine(command1);
            process.StandardInput.WriteLine(command2);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            // Читаем результаты выполнения команды (выходной поток)
            string output = process.StandardOutput.ReadToEnd();
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int index = Array.FindIndex(lines, line => line.Contains("<DIR>"));
            // Создаем массив для хранения директорий
            List<string> dirs = new List<string>();

            // Заполняем массив директорий
            for (int i = index; i < lines.Length; i++)
            {
                if (lines[i].Contains("<DIR>"))
                {
                    // Разделите строку по пробелам
                    string[] parts = lines[i].Split(' ');

                    // Создайте новую строку, начиная с 9-го элемента
                    string desiredText = string.Join(" ", parts, 9, parts.Length - 9).TrimStart();
                    if (desiredText != "." && desiredText != "..")
                        dirs.Add(desiredText);
                }

            }
            // Ожидаем завершения процесса
            process.WaitForExit();
            return dirs.ToArray();
        }
    }

    public static class KJPiFile
    {
        /// <summary>
        /// Метод, занимающийся чтением всех строк из файла
        /// </summary>
        /// <param name="workDirectory">На вход подаётся место, где храниться файл</param>
        /// <param name="nameFile">На вход подаётся имя читаемого файла</param>
        /// <returns>Возвращает содержимое файла</returns>
        public static string ReadAllFileText(string filePath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.Arguments = "/C " + $"type \"{filePath}\" .";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        /// <summary>
        /// Метод, занимающийся удалением файла
        /// </summary>
        /// <param name="filePath">На вход подаётся файл, который требуется удалить</param>
        public static void DeleteFile(string filePath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + $"del \"{filePath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Метод, занимающийся поиском файлов, по их типу
        /// </summary>
        /// <param name="workDirectory">На вход подаётся место, где хранятся файлы</param>
        /// <param name="expansion">На вход подаётся тип файла (например: mp3,metadata и т.д) </param>
        /// <returns>Возращает массив найденных файлов с заданным типом</returns>
        public static string[] GetFilesByExpansion(string workDirectory, string expansion)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.Arguments = "/C " + $"dir /b *.{expansion}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = workDirectory;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string[] buffer = output.Split('\n');
            process.WaitForExit();

            return buffer = buffer.Select(line => line.Replace("\r", "")).ToArray();
        }
        /// <summary>
        /// Метод, занимающийся записью контента в файл
        /// </summary>
        /// <param name="workDirectory">На вход подаётся место, где будет создан или существует файл</param>
        /// <param name="nameFile">На вход подаётся имя файла с расширением</param>
        /// <param name="content">На вход подаётся контент, который будет содержаться в файле</param>
        public static void WriteAllFileText(string workDirectory, string nameFile, string content)
        {
            string outputPath = Path.Combine(Environment.CurrentDirectory, nameFile);
            File.WriteAllText(outputPath, content);
            CopyFileToFolder(workDirectory, nameFile);
        }
        /// <summary>
        /// Метод, занимающийся скопированием файла в папку папке
        /// </summary>
        /// <param name="pathToDirectory">На вход подаётся директория, куда должен будет скопирован файл</param>
        /// <param name="outputFileName">На вход подаётся имя файла</param>
        public static void CopyFileToFolder(string pathToDirectory, string outputFileName)
        {
            string movePath = Path.Combine(Environment.CurrentDirectory, outputFileName);

            // Формируем строку команды, объединяя две команды через символ &
            string command = $"move /Y \"{movePath}\" .";

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
                    WorkingDirectory = pathToDirectory,
                }
            };

            // Запускаем процесс
            process.Start();

            // Передаем команду в стандартный ввод процесса
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            // Ожидаем завершения процесса
            process.WaitForExit();
        }
    }
    /// <summary>
    /// Метод, занимающийся получением изображения в формате строки Base 64
    /// </summary>
    /// <param name="filePath">На вход подаётся путь до изображения</param>
    /// <returns>Возвращает строку изображения в формате Base 64</returns>
    public static string GetImageBase64String(string filePath)
    {
        string outputFilePath = filePath.Replace(".png", ".txt");
    
        string command = $"-command \"[System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes('{filePath}')) | Out-File -FilePath '{outputFilePath}'\"";
    
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = command,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    
        using (Process process = new Process())
        {
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
    
            // Получаем путь к .exe файлу текущего приложения
            string exePath = Assembly.GetEntryAssembly().Location;
    
            // Получаем путь до папки, в которой находится .exe файл
            string appFolderPath = Path.GetDirectoryName(exePath);
            string file = Path.GetFileName(outputFilePath);
            string base64 = ReadAllConsoleText(outputFilePath);
            DeleteFile(outputFilePath);
            return base64;
        }
    }
}
