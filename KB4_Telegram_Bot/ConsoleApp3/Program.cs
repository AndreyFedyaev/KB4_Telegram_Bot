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
using Telegram.Bot.Types.InputFiles;
using System.Linq;

namespace ConsoleApp3
{
    internal class Program
    {
        public static List<string> all_file_names = new List<string>();
        public static List<string> search_file_names = new List<string>();
        public static InlineKeyboardMarkup keyBoard;
        public static List<string> Add_New_File = new List<string>();

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
                if (Message.Text.ToLower() == "add new file" || Message.Text.ToLower() == "addnewfile" || Message.Text.ToLower() == "add newfile")
                {
                    await botClient.SendTextMessageAsync(Message.Chat.Id, "Перетащите файл для добавления в общий список документов");

                    Add_ID(Message.Chat.Id.ToString());
                }
                else
                {
                    Delete_ID(Message.Chat.Id.ToString());

                    bool result = false;
                    string Answer_text = "";

                    //формируем список документов для формирования результата поиска
                    search_file_names.Clear();
                    for (int a = 0; a < all_file_names.Count; a++)
                    {
                        if (all_file_names[a].ToLower().Contains(Message.Text.ToLower()))
                        {
                            search_file_names.Add(all_file_names[a]);
                            result = true;
                        }
                    }
                    //формируем сообщение 
                    for (int a = 0; a < search_file_names.Count; a++)
                    {
                        Answer_text += string.Format("{0}: {1}\r\n", a + 1, search_file_names[a]);
                    }
                    //формируем кнопки
                    List<InlineKeyboardButton[]> Row = new List<InlineKeyboardButton[]>();
                    List<InlineKeyboardButton> Col = new List<InlineKeyboardButton>();
                    for (int a = 1; a < search_file_names.Count + 1; a++)
                    {
                        Col.Add(InlineKeyboardButton.WithCallbackData(text: a.ToString(), callbackData: a.ToString()));
                        if (a % 4 != 0) continue;
                        Row.Add(Col.ToArray());
                        Col = new List<InlineKeyboardButton>();
                    }
                    if (Col.Count > 0) { Row.Add(Col.ToArray()); }

                    if (result)
                    {
                        keyBoard = new InlineKeyboardMarkup(Row.ToArray());
                        await botClient.SendTextMessageAsync(Message.Chat.Id, "Результаты поиска: \r\n\r\n" + Answer_text + "\r\nВыберите номер документа, который необходимо скачать:", replyMarkup: keyBoard);
                        return;
                    }
                }
            }
            if (Callback != null && Callback != "")
            {
                string FileName = "";
                int click_number = Convert.ToInt32(Callback);
                {
                    FileName = search_file_names[click_number - 1];

                    string dirName = AppDomain.CurrentDomain.BaseDirectory + "Instructions\\" + FileName;
                    await using Stream stream = System.IO.File.OpenRead(dirName);
                    await botClient.SendDocumentAsync(update.CallbackQuery.From.Id, document: new InputOnlineFile(content: stream, fileName: FileName));
                    stream.Close();
                }
            }

            if (Message != null && Message.Document != null)
            {
                bool add_permission = Search_ID(Message.Chat.Id.ToString());
                if (add_permission)
                {
                    var fileId = update.Message.Document.FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;

                    string dirName = AppDomain.CurrentDomain.BaseDirectory + "Instructions\\" + Message.Document.FileName;
                    await using Stream stream = System.IO.File.OpenWrite(dirName);
                    await botClient.DownloadFileAsync(filePath, stream);
                    stream.Close();

                    Search_File_Name();
                    Delete_ID(Message.Chat.Id.ToString());

                    await botClient.SendTextMessageAsync(Message.Chat.Id, "Файл добавлен");
                }
                else
                {
                    await botClient.SendTextMessageAsync(Message.Chat.Id, "Для добавления файла введите сначала команду - Add New File");
                }
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

                all_file_names.Clear();
                foreach (string s in files)
                {
                    all_file_names.Add(s.Remove(0, dirName.Length + 1).ToString());
                }
            }
        }
        private static void Add_ID(string ID)
        {
            bool search = false;
            for (int i = 0; i < Add_New_File.Count; i++)
            {
                if (Add_New_File[i] == ID)
                {
                    search = true;
                    break;
                }
            }
            if (search == false)
            {
                Add_New_File.Add(ID);
            }
        }
        private static void Delete_ID(string ID)
        {
            for (int i = 0; i < Add_New_File.Count; i++)
            {
                if (Add_New_File[i] == ID)
                {
                    Add_New_File.RemoveAt(i);
                }
            }
        }
        private static bool Search_ID(string ID)
        {
            bool search = false;
            for (int i = 0; i < Add_New_File.Count; i++)
            {
                if (Add_New_File[i] == ID)
                {
                    search = true;
                    break;
                }
            }
            return search;
        }
    }
}
