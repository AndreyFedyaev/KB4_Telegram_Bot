using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;


namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Search_File_Name();
            var client = new TelegramBotClient("6200203388:AAEEA6D76fCg-LXP24kFytsUe4P5mqD90SI");
            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            Search_File_Name();
            var Message = update.Message;

            string[] filenames = { "file1", "newfile", "lastfile", "testinfilename_new" , "fileFileFile", "Плгртшо Структура Схемплан" };


            


            if (Message.Text != null)
            {
                List<InlineKeyboardButton[]> Row = new List<InlineKeyboardButton[]>();
                List<InlineKeyboardButton> Col = new List<InlineKeyboardButton>();

                for (int a = 0; a < filenames.Length; a++)
                {
                    if (filenames[a].ToLower().Contains(Message.Text.ToLower()))
                    {
                        Col.Add(InlineKeyboardButton.WithCallbackData(filenames[a].ToString()));
                        Row.Add(Col.ToArray());
                        Col = new List<InlineKeyboardButton>();
                    }
                }

                var keyBoard = new InlineKeyboardMarkup(Row);

                await botClient.SendTextMessageAsync(Message.Chat.Id, "Выберите файл из списка:", replyMarkup: keyBoard, cancellationToken: token);

                return;
            }

        } 

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private static void Search_File_Name()
        {
            //FileInfo fileInf = new FileInfo("\\Instructions");

            string dirName = "C:\\Users\\Андрей\\source\\repos\\ConsoleApp3\\ConsoleApp3\\bin\\Debug\\netcoreapp3.1\\Instructions\\";
            // если папка существует
            if (Directory.Exists(dirName))
            {
                Console.WriteLine("Подкаталоги:");
                string[] dirs = Directory.GetDirectories(dirName);
                foreach (string s in dirs)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine();
                Console.WriteLine("Файлы:");
                string[] files = Directory.GetFiles(dirName);
                foreach (string s in files)
                {
                    Console.WriteLine(s);
                }
            }


            //OpenFileDialog openFileDialog1 = new OpenFileDialog(\Instructions);
            //    openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            //    openFileDialog1.Filter = "Config Files|*.ini";
            //    openFileDialog1.Multiselect = true;

            //    if (openFileDialog1.ShowDialog() == true)
            //    {
            //        string[] result = openFileDialog1.FileNames;

            //        string[] FileName = new string[result.Length];
            //        for (int i = 0; i < result.Length; i++)
            //        {
            //            FileName[i] = System.IO.Path.GetFileNameWithoutExtension(result[i]);
            //        }
        }
    }
}
