using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CIS501_Project1
{
    class Program
    {
        


        /// <summary>
        /// The list of all stocks in the provided file
        /// </summary>//
        public static List<Stock> stocks = new List<Stock>();

        /// <summary>
        /// The account this program will be dealing with
        /// </summary>
        public static Account account;

        /// <summary>
        /// The regular expression that matches company stock data to iteslf
        /// </summary>
        private static Regex regex = new Regex(@"([A-Z]+)[-](.*?)[-][$](\d*[.]\d{2})");

        /// <summary>
        /// Market volitility
        /// </summary>
        enum MarketVolatility { High, Medium, Low };

        /// <summary>
        /// The enumeration representing market volatility
        /// </summary>
        private static MarketVolatility volatility = new MarketVolatility();

        static void Main(string[] args)
        {
            string file;
            List<string> companies = new List<string>();
            try
            {
                //Opens new streamreader in a using statement
                using (StreamReader sr = new StreamReader("Ticker.txt"))
                {
                    file = sr.ReadToEnd();   
                }
                //A collection of matches that reflect all of the matches in the provided file
                MatchCollection matches = regex.Matches(file);

                //For each match in the collections, the ticker, name, and price are parsed out using the grouping feature of regexes
                foreach (Match m in matches)
                {
                    GroupCollection gc = m.Groups;
                    //Index 1 is where the first group is stored, index 0 is just the entire matching
                    string ticker = gc[1].Value;
                    string name = gc[2].Value;
                    float price = Convert.ToSingle(gc[3].Value);
                    //The parsed values are now added to stocks as a Stock
                    stocks.Add(new Stock(ticker, price, name));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error with loading the file, please restart\n\n" + ex.ToString());
            }

            //Account creation
            if (YesNoLooper("Would you like to create an account with us today?"))
            {
                Console.Write("Please Enter a name for the account: ");
                string input = Console.ReadLine();
                account = new Account(input, stocks);
                Console.WriteLine();
            }
            else
            {
                return;
            }
            bool run = true;
            while (run)
            {
                string input = MainMenu();
                if (input == "X")
                {
                    run = false;
                }
                else if (input == "CP")
                {
                    if (account.PortfolioCount < 3)
                    {
                        Console.Write("Name of portfolio: ");
                        string input2 = Console.ReadLine();
                        account.CreatePortfolio(input2);
                        Console.WriteLine("Portfolio Created");
                        PortfolioOperations(input2);
                    }
                    else
                    {
                        Console.WriteLine("You already have 3 Portfolios!");
                    }
                }
                else if (input == "SP")
                {
                    Console.WriteLine("\nPortfolios:");
                    foreach (Portfolio p in account.Portfolios)
                    {
                        Console.WriteLine(p.Name);
                    }
                    Console.Write("\nName of portfolio: ");
                    PortfolioOperations(Console.ReadLine());
                }
                else if (input == "D")
                {
                    Console.Write("Enter the amount to be deposited (A flat rate transfer fee of $" + account.TransFee + " will be applied) or enter X to cancel: ");
                    string value = Console.ReadLine();
                    Console.WriteLine();
                    if (!(value == "X" || value == "x"))
                    {
                        try
                        {
                            double amount = Convert.ToDouble(value);
                            amount = Math.Round(amount, 2);
                            if (!account.Deposit(Convert.ToSingle(amount)))
                            {
                                Console.WriteLine("Error, amount entered is less than transfer fee");
                            }
                            account.DepositedCash += Convert.ToSingle(amount) - account.TransFee;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error, invalid input");
                        }
                    }
                }
                else if (input == "W")
                {
                    bool loop = true;
                    while (loop)//Loops until a valid amount is entered. Will prompt user to sell positions in order to achieve adequate funds for trade if the account balance does not suffice
                    {
                        Console.WriteLine("Enter the amount to be withdrawn (A flat rate transfer fee of $" + account.TradeFee + " will be applied) or enter X to cancel: ");
                        string value = Console.ReadLine();
                        if (value == "X")
                        {
                            break;
                        }
                        try
                        {
                            double amount = Convert.ToDouble(value);
                            amount = Math.Round(amount, 2);
                            if (!account.Withdraw(Convert.ToSingle(amount)))
                            {

                                Console.WriteLine("Error, you don't have that much money!\nWhich positions would you like to sell to cover the transaction?\nPlease choose a portfolio or enter X to cancel: ");
                                foreach (Portfolio p in account.Portfolios)
                                {
                                    Console.WriteLine(p.Name);
                                }
                                Console.Write("Selection: ");
                                string line = Console.ReadLine();
                                PortfolioOperations(line);
                            }
                            else
                            {
                                loop = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error, invalid input");
                        }
                    }
                }
                else if (input == "BAL")
                {
                    float total = 0;
                    foreach (Portfolio p in account.Portfolios)
                    {
                        total += p.PositionsBalance;
                    }
                    Console.WriteLine("\nAvailable balance $" + Math.Round(account.CashBalance, 2));
                    Console.WriteLine("Positions balance $" + Math.Round(total,2) + "\n");
                    foreach (Portfolio p in account.Portfolios)
                    {
                        Console.WriteLine(p.Name + " - $" + Math.Round(p.PositionsBalance,2) + " (" + Math.Round(p.PositionsBalance / total * 100, 2) + "%)");
                    }
                    Console.WriteLine();
                }
                else if (input == "GL")
                {
                    float value;
                    float percent = account.GainLoss(out value);
                    string updown = "up";
                    if (value < 0)
                    {
                        updown = "down";
                        value = Math.Abs(value);
                    }
                    Console.WriteLine("\nGain/Loss: " + updown + " $" + value + " (" + percent + "%)\n");

                }
                else if (input == "DS")
                {
                    Console.WriteLine();
                    Console.WriteLine("(Percentages reflect period gains)");
                    foreach (Stock s in stocks)
                    {
                        Console.WriteLine(s.ToString());
                    }
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                else if (input == "ES")
                {
                    Simulator();
                }
                else if (input == "TH")
                {
                    account.DisplayHistory();
                }
            }


            Console.ReadLine();
        }

        /// <summary>
        /// Updates every stock once with a new random value based on market volatility
        /// </summary>
        private static void Tick()
        {
            account.Period += 1;
            foreach (Portfolio p in account.Portfolios)
            {
                p.NextPeriod();
            }
            Random rand = new Random();
            Random operation = new Random();
            int min = 1;
            int max = 4;
            if (volatility == MarketVolatility.High)
            {
                min = 3;
                max = 15;
            }
            else if (volatility == MarketVolatility.Medium)
            {
                min = 2;
                max = 8;
            }
            foreach(Stock s in stocks)
            {
                int num = rand.Next(min, max);
                int op = operation.Next(0, 3);//Determines if the stock is going up or down.
                if (op == 0)                  
                {
                    num = -num;
                }
                s.PreviousPrice = s.StockPrice;
                s.StockPrice = (float)Math.Round(s.StockPrice * (num/100.0f + 1.0f), 2);
            }
           
            
        }

        /// <summary>
        /// Runs a simulation of an active market based on user input
        /// </summary>
        private static void Simulator()
        {
            string input = "";
            while (input != "H" && input != "M" && input != "L" && input != "X")
            {
                Console.Write("Please enter a market volatility (H)igh, (M)edium, or (L)ow or press X to go back to the main menu: ");
                input = Console.ReadLine().ToUpper();
            }
            switch (input)
            {
                case "H":
                    volatility = MarketVolatility.High;
                    break;
                case "M":
                    volatility = MarketVolatility.Medium;
                    break;
                case "L":
                    volatility = MarketVolatility.Low;
                    break;
                case "X":
                    return;
            }
            Tick();
        }

        /// <summary>
        /// Gets yes no input from the user
        /// </summary>
        /// <param name="message">The massage to be displayed</param>
        /// <returns>Boolean corresponding to Y or N</returns>
        private static bool YesNoLooper(string message)
        {
            
            while (true)
            {
                Console.Write(message + " (Y/N) ");
                string input = Console.ReadLine().ToUpper();
                if (input == "Y")
                {
                    return true;
                }
                if (input == "N")
                {
                    return false;
                }
                Console.WriteLine("Invalid input!");
            }
        }

        /// <summary>
        /// Displays the main menu and handles input
        /// </summary>
        /// <returns>user input</returns>
        private static string MainMenu()
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?\nCP - Create new portfolio (You can have up to 3)\n" +
                    "SP - Select a portfolio\nD - Make a deposit\nW - Make a withdrawl\nBAL - See account balance and percentage of account of each portfolio\n" +
                    "GL - See account gains/losses\nES - enter the simulator (stock market simulation)\nDS - Display Stocks\nTH - See Transaction History\nX - Exit program");
                string input = Console.ReadLine().ToUpper().Trim();
                if ((input == "X" || input == "CP" || input == "SP" || input == "D" || input == "W" || input == "BAL" || input == "GL" || input == "ES" || input == "DS" || input == "TH"))
                {
                    return input;
                }
                Console.WriteLine("Error, invalid input, please try again.");
            }
        }

        /// <summary>
        /// Performs operations on the selected portfolio as per user desire
        /// </summary>
        /// <param name="portfolioName">The portfolio to operate on</param>
        private static void PortfolioOperations(string portfolioName)
        {
            if (portfolioName == "X")
            {
                return;
            }
            if (account.PortfolioCount < 1)
            {
                Console.WriteLine("Portfolio not found");
                return;
            }
            Portfolio selected = null;
            foreach(Portfolio p in account.Portfolios)
            {
                if (p.Name == portfolioName)
                {
                    selected = p;
                    break;
                }

            }
            if (selected == null)
            {
                Console.WriteLine("Portfolio not found");
                return;
            }
            

            while (true)//Loop until user returns (chooses X in the menu)
            {
                float positions = selected.PositionsBalance;
                float gl = selected.GainLossPercent;
                float glv = selected.PositionsBalance;//selected.GainLossValue;

                string updown = "+";
                if (glv < 0)
                {
                    updown = "";
                }
                Console.WriteLine("\nPositions Balance: $"+ positions + "\nGains/Losses: " + updown + "$" +glv+ " ("+ updown + gl + "%)" + "\nWhat would you like to do?\nB - Buy stocks\nS - Sell stocks\nV - View report\nSP - Sell this entire portfolio\nX - Return to main menu");
                string input = Console.ReadLine().ToUpper().Trim();
                Console.WriteLine();
                if (input == "B")//Buy a stock
                {
                    string selection = "";
                    Stock selectedStock = null;
                    bool loop = true;
                    while (loop)
                    {
                        Console.Write("Enter the name or the ticker of the stock to be purchased(To see a list of buyable stocks, enter 'STOCKLIST'), or enter X to cancel: ");
                        selection = Console.ReadLine();
                        Console.WriteLine();
                        if (selection == "X" || selection == "x")
                        {
                            loop = false;break;
                        }
                        if (selection == "STOCKLIST")//If the user wants to see available stocks and prices
                        {
                            foreach(Stock s in stocks)
                            {
                                Console.WriteLine(s.ToString());
                            }
                        }
                        else
                        {
                            foreach(Stock s in stocks)//search through stocks for selected stock
                            {
                                if (selection == s.Name || selection == s.Ticker || selection == s.Ticker.ToLower())
                                {
                                    selectedStock = s;
                                    loop = false;
                                    break;
                                }
                            }
                            if (selectedStock == null)//if stock was not found, indicate to the user to try again
                            {
                                Console.WriteLine("Stock not found, please try again");
                            }
                        }
                    }
                    if (selection == "X" || selection == "x")
                    {
                        break;
                    }
                    else
                    { 
                        Console.WriteLine(selectedStock.ToString());
                        Console.WriteLine("Available balance: " + account.CashBalance);
                        if (input != "X" && input != "x")
                            loop = true;
                        int quant = 0;
                        while (loop)
                        {
                            Console.Write("Enter the quantity to be purchased, or enter the dollar amount of stocks to purchase (Ex: $35.10)\n(A flat rate fee of $9.99 will be applied) or enter X to cancel: ");
                            string read = Console.ReadLine();
                            if (read == "X" || read == "x") break;
                            if (read[0] == '$')
                            {
                                try
                                {
                                    string substring = read.Substring(1);
                                    float dollars = Convert.ToSingle(substring);
                                    quant = (int)(dollars / selectedStock.StockPrice);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error, invalid input");
                                }
                            }
                            else
                            {
                                try
                                {
                                    quant = Convert.ToInt32(read);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error, invalid input");
                                }
                            }
                            int code = account.Buy(selected.Name, selectedStock.Name, quant);
                            if (code == 0)
                            {
                                loop = false;
                            }
                        }
                    }

                }
                else if (input == "S")
                {
                    StockQuantity selectedStock = null;
                    bool loop = true;
                    while (loop)
                    {
                        Console.Write("Please enter the ticker or name of the stock to be sold, or enter STOCKLIST to see available stocks or prest X to cancel: ");
                        string selection = Console.ReadLine();
                        if (selection == "X" || selection == "x") break;
                        if (selection == "STOCKLIST")//If the user wants to see available stocks and prices
                        {
                            foreach (StockQuantity s in selected.Stocks)
                            {
                                Console.WriteLine(s.ToString());
                            }
                        }
                        else
                        {
                            foreach (StockQuantity s in selected.Stocks)//search through stocks for selected stock
                            {
                                if (selection == s.Stock.Name || selection == s.Stock.Ticker)
                                {
                                    selectedStock = s;
                                    loop = false;
                                    break;
                                }
                            }
                            if (selectedStock == null)//if stock was not found, indicate to the user to try again
                            {
                                Console.WriteLine("You don't own any stock of that type");
                            }
                        }
                    }
                    loop = true;
                    int quant = 0;
                    while (loop)
                    {
                        Console.Write(selectedStock.ToString() + "\nPlease enter the quantity to sell, or enter X to cancel: ");
                        string read = Console.ReadLine();
                        if (read == "X")
                        {
                            loop = false;
                        }
                        else
                        {
                            try
                            {
                                quant = Convert.ToInt32(read);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine("Invalid input, please use integer input only");
                            }
                            if (quant > selectedStock.Quantity)
                            {
                                Console.WriteLine("You do not own that many shares");
                            }
                            else
                            {
                                account.Sell(selected.Name, selectedStock.Stock.Name, quant);
                                loop = false;
                            }
                        }
                    }
                }
                else if (input == "V")
                {
                    Console.WriteLine(selected.Report());
                }
                else if (input == "X")
                {
                    return;
                }
                else if (input == "SP")
                {
                    
                    for(int i = selected.Stocks.Count - 1; i >= 0; i--)
                    {
                        account.Sell(selected.Name, selected.Stocks[i].Stock.Name, selected.Stocks[i].Quantity);
                    }
                    account.Portfolios.Remove(selected);
                    return;
                }
                else
                {
                    Console.WriteLine("Error, invalid input");
                }
            }
        }
    }
}
