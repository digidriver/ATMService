using ATMConsole.ATMServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool atmUserLoggedIn = true;
            RequestResponse atmResponse = new RequestResponse();
            do
            {
                ATMServiceClient client = new ATMServiceClient("BasicHttpBinding_IATMService");
                displayMainMenu();
                String consoleChoice = Console.ReadLine();
                switch (consoleChoice)
                {
                    case "1":
                        processATMOption("balance", ref atmResponse, client);
                        break;
                    case "2":
                        processATMOption("deposit", ref atmResponse, client);
                        break;
                    case "3":
                        processATMOption("withdrawal", ref atmResponse, client);
                        break;
                    case "4":
                        processATMOption("RANDOM", ref atmResponse, client);
                        break;
                    case "5":
                        Console.WriteLine("Thank you for using the direct ATM service. Have a great day!");
                        Console.ReadLine();
                        atmUserLoggedIn = false;
                        break;
                    default:
                        Console.WriteLine("Input not recognized. Please press ENTER/Return and try again.");
                        Console.ReadLine();
                        break;
                }
            } while (atmUserLoggedIn);
        }

        private static void processATMOption(string action, ref RequestResponse atmResponse, ATMServiceClient client)
        {
            string accountNumber = "";
            int convertedAccountNumber = 88888888;
            string amount = "";
            decimal convertedAmount = 0.00M;
            Console.WriteLine("Please enter your account number:");
            accountNumber = Console.ReadLine();
            convertedAccountNumber = Convert.ToInt32(accountNumber);
            if (action == "balance")
            {
                atmResponse = client.Balance(convertedAccountNumber);
            }
            else
            {
                if (action == "deposit")
                {
                    Console.WriteLine("Please enter the amount you would like to deposit:");
                    amount = Console.ReadLine();
                    if (amount != null)
                    {
                        convertedAmount = Convert.ToDecimal(amount);
                    }
                    else
                    {
                        convertedAmount = 0.00M;
                    }

                    atmResponse = client.Deposit(convertedAccountNumber, convertedAmount, "US");
                }
                else if (action == "withdrawal")
                {
                    Console.WriteLine("Please enter the amount you would like to withdraw:");
                    amount = Console.ReadLine();
                    if (amount != null)
                    {
                        convertedAmount = Convert.ToDecimal(amount);
                    }
                    else
                    {
                        convertedAmount = 0.00M;
                    }

                    atmResponse = client.Withdraw(convertedAccountNumber, convertedAmount, "US");
                }
                else if (action == "RANDOM")
                {
                    Console.WriteLine("Random deposit/withdrawal mode activated!");
                    Console.WriteLine("(A random number of deposits and withdrawals will be made.)");
                    Console.WriteLine(" ");
                    Console.WriteLine("Please press ENTER/Return to begin deposit/withdrawal testing:");
                    Console.ReadLine();

                    // Determine a semi-true random number based on current clock time
                    Random random = new Random();
                    DateTime sysTimeStamp = DateTime.Now;
                    int timeStampAsInteger = sysTimeStamp.Year + sysTimeStamp.Month + sysTimeStamp.Day +
                        sysTimeStamp.Hour + sysTimeStamp.Minute + sysTimeStamp.Second;
                    
                    int randomSeed = timeStampAsInteger / 10;

                    // Iterate deposits and withdrawals a random number of times
                    for (int i = 0; i < (random.Next(randomSeed)); i++)
                    {
                        if (i % 2 == 0)
                        {
                            atmResponse = client.Deposit(convertedAccountNumber, 50.00M, "US");
                            Console.WriteLine(" ");
                            Console.WriteLine(atmResponse.Message);
                            Console.WriteLine(" ");
                            if (atmResponse.Successful == false)
                            {
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            atmResponse = client.Withdraw(convertedAccountNumber, 50.00M, "US");
                            Console.WriteLine(" ");
                            Console.WriteLine(atmResponse.Message);
                            Console.WriteLine(" ");
                            if (atmResponse.Successful == false)
                            {
                                Console.ReadLine();
                            }
                        }
                    }

                    Console.WriteLine("Please press ENTER/Return to continue...");
                    Console.ReadLine();
                    return;
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine(atmResponse.Message);
            Console.WriteLine(" ");
            Console.WriteLine("Please press ENTER/Return to continue...");
            Console.ReadLine();            
        }

        private static void displayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("WELCOME TO YOUR DIRECT ATM SERVICE");
            Console.WriteLine("==================================");
            Console.WriteLine(" ");
            Console.WriteLine("PLEASE CHOOSE FROM THE FOLLOWING MENU OPTIONS:");
            Console.WriteLine("1. Get balance");
            Console.WriteLine("2. Make deposit");
            Console.WriteLine("3. Request withdrawal");
            Console.WriteLine("4. RANDOM number of deposits/withdrawals");
            Console.WriteLine("5. EXIT SERVICE");
            Console.WriteLine(" ");
        }
    }
}
