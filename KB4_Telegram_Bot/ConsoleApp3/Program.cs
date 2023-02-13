using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Text;
using System.Windows.Input;


namespace ConsoleApp3
{
    internal class Program
    {
        public static List<string> file_names = new List<string>();
        public static InlineKeyboardMarkup keyBoard;

        static void Main(string[] args)
        {
            //считываем при запуске список файлов
            Search_File_Name();

            var client = new TelegramBotClient("6200203388:AAEEA6D76fCg-LXP24kFytsUe4P5mqD90SI");
            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var Message = update.Message;
            string Callback = "";

            if (update.CallbackQuery != null) Callback = update.CallbackQuery.Data;

            if (Message != null && Message.Text != null)
            {
                List<InlineKeyboardButton[]> Row = new List<InlineKeyboardButton[]>();
                List<InlineKeyboardButton> Col = new List<InlineKeyboardButton>();
                bool result = false;

                for (int a = 0; a < file_names.Count; a++)
                {
                    if (file_names[a].ToLower().Contains(Message.Text.ToLower()))
                    {
                        Col.Add(InlineKeyboardButton.WithCallbackData(text: file_names[a].ToString(), callbackData: file_names[a].ToString()));
                        Row.Add(Col.ToArray());
                        Col = new List<InlineKeyboardButton>();
                        result = true;
                    }
                }

                if (result)
                {
                    keyBoard = new InlineKeyboardMarkup(Row.ToArray());
                    await botClient.SendTextMessageAsync(Message.Chat.Id, "Выберите файл из списка:", replyMarkup: keyBoard, cancellationToken: token);
                    return;
                }
            }
            if (Callback != null && Callback != "")
            {

            }

        } 

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private static void Search_File_Name()
        {
            string dirName = AppDomain.CurrentDomain.BaseDirectory + "Instructions";
            // если папка существует
            if (Directory.Exists(dirName))
            {
                string[] files = Directory.GetFiles(dirName);

                foreach (string s in files)
                {
                    file_names.Add(s.Remove(0, dirName.Length + 1));
                }
            }
        }
    }
}
