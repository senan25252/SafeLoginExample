using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

namespace Backend
{
    class Program
    {
        public static List<string> authKeys = new List<string>();
        public static void Main(string[] args)
        {
            Console.WriteLine("Server Started Listening");
            Thread listener = new Thread(CheckRequests);
            listener.Start();
            while (true) { } // Sonsuz döngüde server'ın çalışmasını sağlamak
        }

        public static async void CheckRequests()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();

            while (true)
            {
                Console.WriteLine("Running");
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // CORS başlıkları ekle (Eğer gerekliyse)
                response.AddHeader("Access-Control-Allow-Origin", "*");  // Herhangi bir origin'i kabul et
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, CHECK, AUTH");  // Hangi metodlara izin verileceği
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type");  // Hangi başlıklara izin verileceği

                string responseText = "";

                if (request.HttpMethod == "AUTH")
                {
                    using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
                    {
                        var body = reader.ReadToEnd();
                        Console.WriteLine("Received data: " + body);

                        try
                        {
                            Console.WriteLine("RECEVIED");
                            var jsonDoc = JsonDocument.Parse(body);
                            var root = jsonDoc.RootElement;

                            if (root.TryGetProperty("authKey", out var passkeyElement))
                            {
                                string passkey = passkeyElement.GetString();
                                Console.WriteLine(passkey);
                                bool verified = false;

                                foreach(string s in authKeys)
                                {
                                    if(s == passkey)
                                    {
                                        verified = true;
                                    }
                                }

                                if (verified)
                                {
                                    responseText = "<label> THIS IS MAIN PAGE </label>";
                                }
                                else
                                {
                                    responseText = @"
                                                            <input type=""text"" id=""in"">
                                                            <script src=""script.js""></script>
                                                            <button type=""button"" onclick=""CheckPass()"">Login</button>";
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            // JSON çözümlemesinde hata olursa
                            responseText = "Invalid JSON format";
                            Console.WriteLine("JSON Parsing Error: " + ex.Message);
                        }
                    }
                }

                if (request.HttpMethod == "CHECK")
                {
                    responseText = "Server Is Running";
                }

                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
                    {
                        var body = reader.ReadToEnd();
                        Console.WriteLine("Received data: " + body);

                        try
                        {
                            // JSON verisini JsonDocument olarak çözümle
                            var jsonDoc = JsonDocument.Parse(body);
                            var root = jsonDoc.RootElement;

                            // passkey değerini al
                            if (root.TryGetProperty("passkey", out var passkeyElement))
                            {
                                string passkey = passkeyElement.GetString();
                                if (passkey == "pass123")
                                {
                                    string RandomChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
                                    string authKey = "";
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Random random = new Random();
                                        int randomNum = random.Next(0, 62);
                                        char cr = RandomChars[randomNum];
                                        authKey += cr.ToString();
                                    }
                                    Console.WriteLine(authKey);
                                    Program.authKeys.Add(authKey);
                                    Console.WriteLine(authKeys);
                                    responseText = authKey;
                                }
                                else
                                {
                                    responseText = "";
                                }
                            }
                            else
                            {
                                responseText = "Passkey missing";
                            }
                        }
                        catch (JsonException ex)
                        {
                            // JSON çözümlemesinde hata olursa
                            responseText = "Invalid JSON format";
                            Console.WriteLine("JSON Parsing Error: " + ex.Message);
                        }
                    }
                }

                // Yanıtı gönder
                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.Close();
            }
        }
    }
}
