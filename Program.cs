using NetTelegramBotApi;
using NetTelegramBotApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirWebServices;
using NetTelegramBotApi.Types;
using System.Net;
using System.IO;
using us.kasra;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using NRSWeb.Business.Logic;
using System.Configuration;

using System.Net.Http;
using com.nirasoftware;
using System.ComponentModel;
using NRSbot;


namespace NRSbot
{

    public class WorkerThread
    {
        Update update;
        public WorkerThread(Update u)
        {
            update = u;
        }
        public void ThreadProc()
        {
            try
            {
                Program.ServiceUpdate(update);
            }
            catch (Exception e) { }
        }
    }



}



    class Program
    {
       //zagrosID :
      //  private static String botToken = "490904400:AAGt37aw_1-fINys_MUUEfM4k5_MYoBLV3c";
      
        //B9ID:
     private static String botToken = "1018630170:AAHNgH7FYxe9WF2BJLMHbX9r4cIMiZUqVxo";
       
        static int MAX_RESPONSE_LEN = 3000;

        static string ImgAddress = ConfigurationSettings.AppSettings["ImgAddress"];
        static string GraphicalBotReporUrl = ConfigurationSettings.AppSettings["GraphicalBotReporUrl"];
       // string usercount = ConfigurationSettings.AppSettings["UserCount"];


        public static String sendHttpRequest(String url)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                return responseString;
            }
            return "";
        }

        static void Main(string[] args)
        {


   

            Task.Run(() => RunBot());
            Console.ReadLine();





        }




        static TelegramBot newBot;
        static User botDetail;


        static Hashtable Session;
       

        public static async void ServiceUpdate(Update update)
        {

            try
            {
                   Console.WriteLine(
        "Hello, World! I am user {me.Id} and my name is {me.FirstName}."
      );

                Console.WriteLine(update.Message.Chat.Id.ToString() + "-"+ update.Message.Text + " in ServiceUpdate");
                List<String> cityList = new List<String>();
                cityList.Add("THR");
                cityList.Add("AWZ");
                cityList.Add("SYZ");
                cityList.Add("IFN");
                cityList.Add("KIS");
                cityList.Add("ABD");
                cityList.Add("MHD");
                cityList.Add("TBZ");
                cityList.Add("YZD");
                cityList.Add("BND");
                cityList.Add("KER");
                cityList.Add("GSM");



                var reports = new ReplyKeyboardMarkup();
                reports.ResizeKeyboard = true;
                reports.Keyboard =
                  new KeyboardButton[][]  
              {  
                new KeyboardButton[]  //1F4B0
                {  
                    new KeyboardButton("\U0001F522Route Sales Count"),  
                    new KeyboardButton("\U0001F4B5Route Sales Income"),
                    new KeyboardButton("\U0001F4C8Top Agent Sales")  
                },  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F519Return")  
                }  
              };

                var dates = new ReplyKeyboardMarkup();
                dates.ResizeKeyboard = true;
                dates.Keyboard =
                new KeyboardButton[][]  
            {  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("Today"),  
                    new KeyboardButton("Tomorrow")  ,
                    new KeyboardButton("Yesterday")  
                },  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F519Return") 
                  
                }
            };


                var pastDates = new ReplyKeyboardMarkup();
                pastDates.ResizeKeyboard = true;
                pastDates.Keyboard =
                new KeyboardButton[][]  
            {  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("Today"),  
                    new KeyboardButton("Yesterday"),
                     new KeyboardButton("Day Before Yesterday")
  
                },  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F519Return") 
                   
 
                }
            };






                var startOption = new ReplyKeyboardMarkup();
                startOption.ResizeKeyboard = true;
                startOption.Selective = true;
                startOption.Keyboard =
                       new KeyboardButton[][]  
            {  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F510Login"),  
                    new KeyboardButton("\U0001F519Return")  
                    
                }
            };






                var city = new ReplyKeyboardMarkup();
                city.ResizeKeyboard = true;
                city.Keyboard =
                 new KeyboardButton[][]  
             {  
                 new KeyboardButton[]  
                 {  
                    new KeyboardButton("THR"), new KeyboardButton("AWZ")  , 
                    new KeyboardButton("SYZ") ,new KeyboardButton("IFN")  
              
                },  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("KIS"), new KeyboardButton("ABD")  , 
                    new KeyboardButton("MHD")  ,new KeyboardButton("TBZ")  
            
                }  ,
                new KeyboardButton[]  
                {  
                    new KeyboardButton("YZD") ,new KeyboardButton("BND")  ,
                    new KeyboardButton("KER")  ,new KeyboardButton("GSM")   
                }  
            };

                var generalOption = new ReplyKeyboardMarkup();
                generalOption.ResizeKeyboard = true;
                generalOption.Keyboard =
                new KeyboardButton[][]  
            {  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F50DStart")
                   
                 
                } 
            
            };

                var reportCommand = new ReplyKeyboardMarkup();
                reportCommand.ResizeKeyboard = true;

                reportCommand.Keyboard =
                new KeyboardButton[][]  
            {  
                new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F4CAGraphical Reports"),  
                    new KeyboardButton("\U0001F4CBTerminal Commands")  
                 
                } , new KeyboardButton[]  
                {  
                    new KeyboardButton("\U0001F519Return")  
                } 
            
            };









                string Origin = "";
                string Destination = "";
                string Date = "";

                //tbSystemProperty.findWhereCondition(x => x.Name.Equals("BotAddressIMG")).First().Value;
                StringBuilder log = new StringBuilder();
                long offset = update.UpdateId + 1;

                if ((update == null) || (update.Message == null) || (update.Message.Chat == null))
                    return;

                String ChatID = update.Message.Chat.Id.ToString();
                String Request = update.Message.Text;
                var userName = update.Message.From.Username;
                //var cellphone = update.Message.Contact.PhoneNumber;

                DateTime now = DateTime.Now;


                //static String ImgAddress = tbSystemProperty.findWhereCondition(x => x.Name.Equals("BotAddressIMG")).First().Value;






                var text = update.Message.Text;


                UserSession us = (UserSession)Session[ChatID];
                if (us == null)
                {
                    us = new UserSession();
                    us.State = "";
                    Session[ChatID] = us;
                }
                if (text != null && (text.ToLower().Contains("start") || text.ToLower().Contains("return")))
                {



                    log.AppendLine("ChatID: " + ChatID + " /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                    System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                    log.Clear();



                    us = (UserSession)Session[ChatID];
                    us.State = "start";
                    Session[ChatID] = us;

                    tbBotUser userChatiId = tbBotUser.findWhereCondition(x => x.ChatID == ChatID).FirstOrDefault();


                    if (userChatiId == null)
                    {
                        us.State = "firstlogin";
                        Session[ChatID] = us;
                        String response = "\U0001F4CC Welcom to NiraBot,\n \n\U0001F4DD Please choose your selection:";
                        var req = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = startOption };
                        await newBot.MakeRequestAsync(req);

                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text, Logger.NORAML);

                        log.AppendLine("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text);
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        return;
                    }
                    else
                    {
                        us.State = "detected";
                        Session[ChatID] = us;
                        String response = "\U0001F4CC Welcom to NiraBot, dear " + userChatiId.UserID + " \n \n \U0001F4DD Please choose your selection:";
                        var req = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = reportCommand };
                        await newBot.MakeRequestAsync(req);

                        // Logger.logEvent("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text, Logger.NORAML);
                       
                        log.AppendLine("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();
                        return;
                    }



                }
                else if (us.State == "firstlogin")
                {
                    if (text.Contains("Login"))
                    {
                        us.State = "userspec";
                        Session[ChatID] = us;
                        String response = "\U0001F4DD Please enter your username and password.\n \n\U0001F50A Exam: username/password";
                        var req = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = generalOption };


                        await newBot.MakeRequestAsync(req);

                        log.AppendLine("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : userspec , /User Text : " + text,Logger.NORAML);
                        return;
                    }
                    else if (text != "Return")
                    {
                        us.State = "firstlogin";
                        Session[ChatID] = us;
                        var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect, Please use the keyboard.") { ReplyMarkup = startOption };
                        await newBot.MakeRequestAsync(req);

                        log.AppendLine("ChatID: " + ChatID + "/ State : firstlogin , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        //   Logger.logEvent("ChatID: " + ChatID + "/ State : firstlogin , /User Text : " + text, Logger.NORAML);


                        return;
                    }

                }
                else if (text != null && us.State == "userspec")
                {


                    if (text.Length > 6 && text.Contains("/"))
                    {

                        us.State = "login";
                        Session[ChatID] = us;
                        int index = text.IndexOf("/");


                        us.User = text.Substring(0, index).ToString();
                        Session[ChatID] = us;
                        string passlen = text.Substring(index + 1, text.Length - (index + 1));
                        us.Password = passlen;
                        Session[ChatID] = us;

                        tbBotUser botUser = tbBotUser.findWhereCondition(x => x.UserID.Equals(us.User)).FirstOrDefault();

                        if (botUser != null && us.Password == botUser.Password)
                        {
                            us.State = "registered";
                            Session[ChatID] = us;

                            botUser.ChatID = ChatID;
                            botUser.IssueDateTime = now.ToString();
                            botUser.ExtraInfo = "Users Regidtered.";
                            if (userName != null || userName != "")
                                botUser.BotUserName = userName;
                            botUser.store();

                            AirWS ws = new AirWS();
                            String s = ws.SendCommand("SI" + us.User + "/" + us.Password, ChatID);




                            //String response = "Welcom " + us.User + " \U0001F44C,\n\n choose your selection.";
                            String response = "Welcom " + us.User + " \U00002708,\n\n choose your selection.";
                            var req = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = reportCommand };
                            await newBot.MakeRequestAsync(req);

                            log.AppendLine("ChatID: " + ChatID + "/ State : registered , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                            System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                            log.Clear();

                            //   Logger.logEvent("ChatID: " + ChatID + "/ State : registered , /User Text : " + text, Logger.NORAML);
                            return;
                        }
                        else
                        {
                            AirWS ws = new AirWS();
                            String s = ws.SendCommand("SI" + us.User + "/" + us.Password, ChatID);
                            if (s.Contains("Succ"))
                            {
                                botUser = new tbBotUser();
                                botUser.UserID = us.User;
                                botUser.Password = us.Password;
                                botUser.ChatID = ChatID;
                                botUser.create();

                                log.AppendLine("ChatID: " + ChatID + "/ State : registered , SendCommand SI Command To Ws. /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                                System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                                log.Clear();


                                //  Logger.logEvent("ChatID: " + ChatID + "/ State : registered , SendCommand SI Command To Ws. /User Text : " + text, Logger.NORAML);
                            }
                            else
                            {
                                us.State = "userspec";
                                Session[ChatID] = us;
                                String response = "\U0001F6AB User or Password is wrong.\n \n \U0001F50A Exam:username/password";
                                var req = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = startOption };
                                await newBot.MakeRequestAsync(req);

                                log.AppendLine("ChatID: " + ChatID + "/ State : userspec , User or Password is wrong, /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                                System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                                log.Clear();

                                //   Logger.logEvent("ChatID: " + ChatID + "/ State : userspec , User or Password is wrong, /User Text : " + text, Logger.NORAML);
                            }
                            return;
                        }

                    }
                    else
                    {
                        us.State = "userspec";
                        Session[ChatID] = us;
                        var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect ,Enter your username and password.\n\U0001F50A Exam: username/password") { ReplyMarkup = startOption };
                        await newBot.MakeRequestAsync(req);
                        return;
                    }




                }

                else if (text != null && (us.State == "registered" || us.State == "detected") && text.Contains("Graphical Reports"))
                {

                    us.State = "selectreport";
                    Session[ChatID] = us;
                    var req = new SendMessage(update.Message.Chat.Id, "Please select the desired report: ") { ReplyMarkup = reports };
                    await newBot.MakeRequestAsync(req);

                    log.AppendLine("ChatID: " + ChatID + "/ State : selectreport , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                    System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                    log.Clear();


                    // Logger.logEvent("ChatID: " + ChatID + "/ State : selectreport , /User Text : " + text, Logger.NORAML);
                    return;
                }

                else if (us.State.Equals("selectreport") || us.State.Equals("reportdatacomplete"))
                {
                    us.State = "selectorigin";
                    Session[ChatID] = us;
                    //  Logger.logEvent("ChatID: " + ChatID + "/ State : selectorigin , /User Text : " + text, Logger.NORAML);
                    if (text.Contains("Route Sales Count"))
                        us.Report = "Count";
                    else if (text.Contains("Route Sales Income"))
                        us.Report = "Income";
                    else if (text.Contains("Top Agent Sales"))
                    {
                        us.Report = "TopSales";
                        us.State = "enterdate";
                        Session[ChatID] = us;


                        await newBot.MakeRequestAsync(new SendMessage(update.Message.Chat.Id, "\U0001F4C6 Date: \nSelect the keyboard or enter the date manually\n\U0001F50A Exam: 2018-01-01") { ReplyMarkup = pastDates });

                        log.AppendLine("ChatID: " + ChatID + "/ State : selectorigin , Report : TopSales, /User Text : " + text + "  " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();


                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : selectorigin , Report : TopSales, /User Text : " + text, Logger.NORAML);
                        return;

                    }
                    else
                    {
                        us.State = "selectreport";
                        Session[ChatID] = us;
                        var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect ,Please use the keyboard.") { ReplyMarkup = reports };

                        await newBot.MakeRequestAsync(req);

                        log.AppendLine("ChatID: " + ChatID + "/ State : selectreport , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : selectreport , /User Text : " + text, Logger.NORAML);
                        return;
                    }

                    Session[ChatID] = us;


                    String response = "Origin:";
                    var req2 = new SendMessage(update.Message.Chat.Id, response) { ReplyMarkup = city };
                    await newBot.MakeRequestAsync(req2);
                    return;

                }

                else if (text != null && us.State == "selectorigin")
                {
                    String cityCode = "";

                    foreach (string row in cityList)
                    {
                        if (row.Contains(text))
                        {
                            cityCode = text;
                        }
                    }

                    if (cityCode != null && cityCode != "")
                    {
                        us.State = "selectdestination";
                        us.Origin = text;
                        Session[ChatID] = us;
                        Console.WriteLine(Origin);


                        await newBot.MakeRequestAsync(new SendMessage(update.Message.Chat.Id, "Destination:") { ReplyMarkup = city });

                        log.AppendLine("ChatID: " + ChatID + "/ State : selectdestination , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();


                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : selectdestination , /User Text : " + text, Logger.NORAML);
                        return;
                    }
                    else
                    {
                        us.State = "selectorigin";
                        Session[ChatID] = us;
                        var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect ,Please use the keyboard.") { ReplyMarkup = city };
                        await newBot.MakeRequestAsync(req);
                        return;
                    }


                }
                else if (text != null && us.State == "selectdestination")
                {
                    String cityCode = "";

                    foreach (string row in cityList)
                    {
                        if (row.Contains(text))
                        {
                            cityCode = text;
                        }
                    }
                    if (cityCode != null && cityCode != "")
                    {
                        us.State = "enterdate";
                        us.Destination = text;
                        Session[ChatID] = us;
                        Console.WriteLine(Destination);
                        await newBot.MakeRequestAsync(new SendMessage(update.Message.Chat.Id, "\U0001F4C6 Date: \nSelect the keyboard or enter the date manually\n\U0001F50A Exam: 2018-01-01") { ReplyMarkup = dates });

                        log.AppendLine("ChatID: " + ChatID + "/ State : enterdate , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        //  Logger.logEvent("ChatID: " + ChatID + "/ State : enterdate , /User Text : " + text, Logger.NORAML);
                        return;
                    }
                    else
                    {
                        us.State = "selectdestination";
                        Session[ChatID] = us;
                        var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect ,Please use the keyboard.") { ReplyMarkup = city };
                        await newBot.MakeRequestAsync(req);

                        log.AppendLine("ChatID: " + ChatID + "/ State : selectdestination , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();

                        // Logger.logEvent("ChatID: " + ChatID + "/ State : selectdestination , /User Text : " + text, Logger.NORAML);
                        return;
                    }
                }
                else if (text != null && us.State == "enterdate")
                {
                    us.State = "reportdatacomplete";
                    Session[ChatID] = us;

                    log.AppendLine("ChatID: " + ChatID + "/ State : reportdatacomplete , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                    System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                    log.Clear();

                    // Logger.logEvent("ChatID: " + ChatID + "/ State : reportdatacomplete , /User Text : " + text, Logger.NORAML);

                    if ((text.Contains("Today") || text.Contains("Tomor") || text.Contains("Yester")))
                    {

                        if (text.Equals("Today"))
                            Date = now.ToString("yyyy-MM-dd");
                        if (text.Equals("Tomorrow"))
                            Date = now.AddDays(1).ToString("yyyy-MM-dd");
                        if (text.Equals("Yesterday"))
                            Date = now.AddDays(-1).ToString("yyy-MM-dd");
                        if (text.Equals("Day Before Yesterday"))
                            Date = now.AddDays(-2).ToString("yyy-MM-dd");

                    }
                    else if (com.nirasoftware.Validators.dateFormatValidate(text))
                    {
                        Date = text;

                    }
                    else
                    {
                        us.State = "enterdate";
                        Session[ChatID] = us;

                        Console.WriteLine(Destination);

                        await newBot.MakeRequestAsync(new SendMessage(update.Message.Chat.Id, "\U0001F6AB Invalid input data.\nChoose date from keyboard or enter date.\nExam: 2018-01-01") { ReplyMarkup = dates });


                        return;
                    }




                    String Img = (now.ToString("yyyy-MM-dd hh:mm:ss").Replace(":", "-") + update.Message.Chat.Id).Replace(" ", "-");
                    Console.WriteLine(Date);
                    await newBot.MakeRequestAsync(new SendMessage(update.Message.Chat.Id, "Please Wait...") { ReplyMarkup = reports });

                    String baseUrl = GraphicalBotReporUrl;//tbSystemProperty.findWhereCondition(x => x.Name.Equals("GraphicalBotReporUrl")).First().Value;


                    String url = baseUrl + "?Origin=" + us.Origin + "&Destination=" + us.Destination + "&Date=" + Date + "&Img=" + Img + "&Report=" + us.Report;//+"&ReturnDate=2018-02-19&AdultQTY=1&ChildQty=0&InfantQty=0&RoundTrip=True";

                    String resp = sendHttpRequest(url);

                    //// Logger.logEvent("ChatID: " + ChatID + "/ State : Url1 Sent To Server Url : " + url + "/User Text : " + text, Logger.NORAML);
                   
                    log.AppendLine("ChatID: " + ChatID + "/ State : Url1 Sent To Server Url : " + url + "/User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                    System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                    log.Clear();

                    int cnt = 0;
                    var fileId = @ImgAddress + Img + ".png";
                    while ((!System.IO.File.Exists(fileId)) && (cnt < 30))
                    {
                        cnt++;
                        Thread.Sleep(1000);
                    }

                    if (!System.IO.File.Exists(fileId)) return;
                    var stream2 = System.IO.File.Open(fileId, FileMode.Open);
                    FileToSend fts = new FileToSend(stream2, fileId.Split('\\').Last());
                    await newBot.MakeRequestAsync(new SendPhoto(update.Message.Chat.Id, fts) { Caption = "Sales Report: " + us.Report + " " + us.Origin + "-" + us.Destination + " Date: " + now.ToShortDateString() });

                    log.AppendLine("ChatID: " + ChatID + "/ State : Url2 Sent To Server , /User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                    System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                    log.Clear();

                    //  Logger.logEvent("ChatID: " + ChatID + "/ State : Url2 Sent To Server , /User Text : " + text, Logger.NORAML);


                    if (us.Report.Contains("TopSales"))
                    {


                        String url2 = baseUrl + "?Origin=" + us.Origin + "&Destination=" + us.Destination + "&Date=" + Date + "&Img=second" + Img + "&Report=" + us.Report;//+"&ReturnDate=2018-02-19&AdultQTY=1&ChildQty=0&InfantQty=0&RoundTrip=True";

                        String resp2 = sendHttpRequest(url2);

                      

                        log.AppendLine("ChatID: " + ChatID + "/ State : Url Sent To Server url2 : " + url2 + "/User Text : " + text + " " + now.ToString("yyyy-MM-dd hh:mm:ss") + "\n");
                        System.IO.File.AppendAllText(ImgAddress + "log" + ChatID + ".txt", log.ToString());
                        log.Clear();


                        int cnt2 = 0;
                        var fileId2 = @ImgAddress + "second" + Img + "new" + ".png";
                        while ((!System.IO.File.Exists(fileId2)) && (cnt < 30))
                        {
                            cnt2++;
                            Thread.Sleep(1000);
                        }

                        if (!System.IO.File.Exists(fileId2)) return;
                        var stream3 = System.IO.File.Open(fileId2, FileMode.Open);
                        FileToSend fts2 = new FileToSend(stream3, fileId2.Split('\\').Last());
                        await newBot.MakeRequestAsync(new SendPhoto(update.Message.Chat.Id, fts2) { Caption = "Sales Report: " + us.Report + " " + us.Origin + "-" + us.Destination + " Date: " + now.ToShortDateString() });

                        return;

                    }

                    return;



                }

                else if (us.State == "terminal" || text.Contains("Terminal Commands"))
                {
                    try
                    {
                        us.State = "terminal";
                        Session[ChatID] = us;
                        String s = "";
                        if (text.Contains("Terminal Commands"))
                        {
                            s = "Please enter Command:";
                        }
                        else
                        {
                       
                                AirWS ws =  new AirWS();
                                    s = ws.SendCommand(text, ChatID);
                                    if (s.Contains("REMOTE"))
                                        s = "Please Sign in using SI Command)";
                                
                                int n = s.Length;
                                if (n > MAX_RESPONSE_LEN)
                                    s = s.Substring(1, MAX_RESPONSE_LEN);
                    
                        }
                        var req = new SendMessage(update.Message.Chat.Id, s) { ReplyMarkup = generalOption }; ;
                        await newBot.MakeRequestAsync(req);
                    }
                    catch (Exception exe) { }

                    return;
                }
                else
                {
                    us.State = "start";
                    us.Origin = "";
                    us.Destination = "";
                    us.Report = "";
                    Session[ChatID] = us;

                    var req = new SendMessage(update.Message.Chat.Id, "\U0001F6AB Input data is incorrect.Please use the keyboard.") { ReplyMarkup = generalOption };
                    await newBot.MakeRequestAsync(req);
                    return;
                }
            }
            catch (Exception e) { }
                return;
        }

        public static async Task RunBot()
        {

            try
            {
            newBot = new TelegramBot(botToken);
        //    botDetail = (User)await newBot.MakeRequestAsync(new GetMe());
            var botDetail = await newBot.MakeRequestAsync(new GetMe());
  
            Console.WriteLine("UserName Is {0}", botDetail.Username);

            Session = new Hashtable();



            long offset = 0;
        
                while (true)
                {
                    try
                    {
                        Update[] updates = await newBot.MakeRequestAsync(new GetUpdates() { Offset = offset });

                        if (updates.Length==0) continue;
                        Console.WriteLine("");
                        if ((updates[0].Message != null))
                            Console.WriteLine(updates[0].Message.Chat.Id.ToString() + "-" + updates[0].Message.Text + " in Main Loop");
                        else
                            Console.WriteLine(updates[0].UpdateId);

                        foreach (var update in updates)
                        {

                            //  Console.ReadLine();
                            WorkerThread worker = new WorkerThread(update);

                            if ((update.Message != null))
                                Console.WriteLine(update.Message.Chat.Id.ToString() + "-" + update.Message.Text + " Worker Thread Created");
                            else
                                Console.WriteLine(update.UpdateId);

                            Thread t = new Thread(new ThreadStart(worker.ThreadProc));

                            if ((update.Message != null))
                                Console.WriteLine(update.Message.Chat.Id.ToString() + "-" + update.Message.Text + "  Thread Created");
                            else
                                Console.WriteLine(update.UpdateId);

                            t.Start();
                            offset = update.UpdateId + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" Exception:" + ex.Message);
                    }

                }
            }
            catch (Exception e)
            { Console.WriteLine(" Exception:"+ e.Message); }


        }
    
    }



