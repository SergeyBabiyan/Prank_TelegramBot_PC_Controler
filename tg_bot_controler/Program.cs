﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using static System.Net.WebRequestMethods;

namespace tg_bot_controler
{
    internal class Program
    {



        static string awaitingUrlChatId = null;
        static string awaitingPlaySoundChatId = null;

        static ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Open URL"},
                        new KeyboardButton[] { "Play sound" },
                    })
        {
            ResizeKeyboard = true
        };




        static void Main(string[] args)
        {
            var bot = new TelegramBotClient("7376898141:AAHjm3jtPbrkOqZJqfj_cuUhS-6hpxVLvMk");
            Console.WriteLine("Bot Started");

            bot.StartReceiving(update, error);


            Console.ReadLine();
        }

        private static async Task update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;

            //открытие URL
            if(awaitingUrlChatId != null && awaitingUrlChatId == chatId.ToString())
            {
                try
                {
                    if(CheckReturn(message.Text,chatId, client))
                    {
                        return;
                    }
                    Process.Start(message.Text);
                    Console.WriteLine("URL opened");
                    await client.SendTextMessageAsync(chatId, "URl успешно открылся");
                    awaitingUrlChatId = null;
                    ReturnToHome(chatId,client);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId,"что-то пошло нет так. Для возврата напишите /return или попробуйте вставить URl заново");
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
                        string destinationFilePath = @"C:\Users\serge\Desktop\AudioForTgBot\" + message.Audio.FileName; //путь вместе с названием файла
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



                        await client.SendTextMessageAsync(chatId,"sadadadadadadadadadadadadadadadadadda");
                        Console.WriteLine("Звук начал воспроизведение");
                        await client.SendTextMessageAsync(chatId, "Звук успешно начал воспроизведение");
                        
                        awaitingPlaySoundChatId = null;
                        ReturnToHome(chatId, client);
                        return;
                    }
                    else { await client.SendTextMessageAsync(chatId, "что-то пошло нет так. Для возврата напишите /return или попробуйте заново загрузить звук в формате mp3"); }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await client.SendTextMessageAsync(chatId, "что-то пошло нет так. Для возврата напишите /return или попробуйте заново загрузить звук в формате mp3");

                }
                return;
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
                await client.SendTextMessageAsync(chatId,"Выберите, что хотите сделать", replyMarkup: replyKeyboardMarkup);
            }

            //лог функция, выводящая последнее, текстовое сообщение пользователя
            if(message.Text != null)
            {
                Console.WriteLine("Msg: " + message.Text);

                if(message.Text == "Open URL")
                {
                    await client.SendTextMessageAsync(chatId,"напишите URL");
                    awaitingUrlChatId = chatId.ToString();
                    return;
                }

                if (message.Text == "Play sound")
                {
                    await client.SendTextMessageAsync(chatId, "отправьте звуковой файл в формате mp3");
                    awaitingPlaySoundChatId = chatId.ToString();
                    return;
                }
                return;
            }
        }


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
            await client.SendTextMessageAsync(chatID, "Выберите, что хотите сделать", replyMarkup: replyKeyboardMarkup);
            awaitingPlaySoundChatId = null;
            awaitingUrlChatId = null;
        }

        private static Task error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}