using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS501_Project1
{
    class Portfolio
    {
        /// <summary>
        /// Stocks contained in this portfolio
        /// </summary>
        private List<StockQuantity> stocks;

        /// <summary>
        /// Gets the list of stocks contained in this portfolio
        /// </summary>
        public List<StockQuantity> Stocks
        {
            get
            {
                return stocks;
            }
        }

        /// <summary>
        /// The name of this portfolio
        /// </summary>
        private string name;

        /// <summary>
        /// Gets the name of this portfolio
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Value at the beginning of the current period
        /// </summary>
        private float periodStartValue;

        /// <summary>
        /// Returns price at the beginning of the current period
        /// </summary>
        public float PeriodStartValue
        {
            get
            {
                return periodStartValue;
            }
            set
            {
                periodStartValue = value;
            }
        }

 /*       /// <summary>
        /// Gets the cash balance of this portfolio as a float and sets the cash balance
        /// </summary>
        public float CashBalance
        {
            get
            {
                return cashBalance;
            }
            set
            {
                cashBalance = value;
            }
        }*/

        /// <summary>
        /// Gets the positions balance as a float
        /// </summary>
        public float PositionsBalance
        {
            get
            {
               float returnvalue = 0;
               foreach (StockQuantity sq in stocks)
                {
                    returnvalue += (sq.Stock.StockPrice * sq.Quantity);
                }
                return (float)Math.Round(returnvalue, 2);
            }
        }

        /// <summary>
        /// The value of this portfolio if everything was back to the buying price
        /// </summary>
        private float originPrices;

        public float OriginPrices
        {
            get
            {
                return originPrices;
            }
            set
            {
                originPrices = value;
            }
        }


        /// <summary>
        /// Returns the value of the portfolio as if the stocks had never changed value
        /// </summary>
        /*public float OriginalValue
        {
            get
            {
                float value = 0;
                foreach(StockQuantity s in stocks)
                {
                    value += s.OriginalPrice;
                }
                return value;
            }
        }*/
        /// <summary>
        /// Returns the value gain/loss of this portfolio (including purchases)
        /// </summary>
        public float GainLossValue
        {
            get
            {
                /*float returnvalue = 0;
                foreach (StockQuantity sq in stocks)
                {
                    returnvalue += (sq.GainLossValue * sq.Quantity);
                }
                return returnvalue;*/
                return (float)Math.Round((PositionsBalance - periodStartValue), 2);
            }
        }

        /// <summary>
        /// Returns the value gain/loss of this portfolio (excluding the value of trades)
        /// </summary>
        public float NPGainLossValue
        {
            get
            {
                return PositionsBalance - originPrices;
            }
        }

        /// <summary>
        /// Gets number of stocks in portfolio
        /// </summary>
        public int Count
        {
            get
            {
                return stocks.Count;
            }
        }

        /// <summary>
        /// Finds the gain/loss as a percent for the entire portfolio
        /// </summary>
        public float GainLossPercent
        {
            get
            {
                /*float num = 0;
                int totalUnits = 0;
                foreach (StockQuantity sq in stocks)
                {
                    num += sq.PeriodStartPrice * sq.Quantity;
                    totalUnits += sq.Quantity;
                }
                return (GainLossValue) / totalUnits *10;*/
                if (PeriodStartValue != 0)
                {
                    return (PositionsBalance - PeriodStartValue) / PeriodStartValue * 100;
                }
                else
                {
                    return PositionsBalance;
                }
            }
        }

        /// <summary>
        /// Constructor that only sets the name of the portfolio
        /// </summary>
        /// <param name="name">The name of the portfolio</param>
        public Portfolio(string name)
        {
            this.name = name;
            periodStartValue = 0;
            originPrices = 0;
            stocks = new List<StockQuantity>();
        }

        /// <summary>
        /// Called when moving to the next period. Changes the start value of the portfolio to the current value
        /// </summary>
        public void NextPeriod()
        {
            periodStartValue = PositionsBalance;
            foreach (StockQuantity s in stocks)
            {
                s.PeriodStartPrice = s.Stock.StockPrice;
            }
        }

        /// <summary>
        /// Adds a stock to the list of stocks contained
        /// </summary>
        /// <param name="stock">The stock to add</param>
        /// <param name="quantity">The number of stocks to buy</param>
        /// <returns>The value of the purchase</returns>
        public float Add(Stock stock, int quantity)
        {
            bool exists = false;
            StockQuantity sq = null;
            foreach (StockQuantity s in stocks)
            {
                if (s.Stock.Name == stock.Name)
                {
                    exists = true;
                    sq = s;
                    break;
                }
            }
            if (!exists)//if the stock does not exist, add a new one to the list
            {
                stocks.Add(new StockQuantity(stock, quantity));
                originPrices += stock.StockPrice * quantity;
            }
            else//If it does exist, just add the quantity
            {
                sq.Quantity += quantity;
                originPrices += sq.PriceAtPurchase * quantity;
            }
            
            return stock.StockPrice * quantity;
        }


        /// <summary>
        /// Removes a stock from the portfolio based on the ticker value
        /// </summary>
        /// <param name="ticker">String identifier</param>
        /// <returns>The value of the sale in dollars</returns>
        public float sell(string ticker, int quantity)
        {
            float returnValue = 0;
            StockQuantity selectedStock = null;
            foreach (StockQuantity s in stocks)//Iterates through every stock, searching for the ticker
            {
                if (s.Stock.Ticker == ticker)
                {
                    selectedStock = s;
                }
            }
            if (selectedStock.Quantity < quantity)//If the quantity to be sold exceeds the owned amount, the user is notified and taken back to the previous screen
            {
                Console.WriteLine("You don't own that many stocks!");
                return returnValue;
            }
            else
            {
                originPrices -= selectedStock.PriceAtPurchase * quantity;
                returnValue = selectedStock.Stock.StockPrice * quantity;
                if (selectedStock.Quantity == quantity)
                {
                    stocks.Remove(selectedStock);

                }
                else
                {
                    selectedStock.Quantity -= quantity;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Removes every stock in the portfolio
        /// </summary>
        /// <retruns>The value of every stock in the portfolio in dollars</retruns>
        public float sellAll()
        {
            float returnValue = 0;
            foreach(StockQuantity s in stocks)
            {
                returnValue += s.Stock.StockPrice * s.Quantity;
                originPrices -= s.PriceAtPurchase * s.Quantity;
            }
            stocks = new List<StockQuantity>();
            return returnValue;
        }

        /// <summary>
        /// Generates a portfolio report of all stocks within it and the percent of the portfolio that each accounts for
        /// </summary>
        /// <returns>the report</returns>
        public string Report()
        {
            int stockCount = stocks.Count;
            List<string> details = new List<string>();

            int total = 0;
            foreach (StockQuantity s in stocks)
            {
                details.Add(s.ToString());

                total += s.Quantity;
            }
            string output = "";
            for (int i = 0; i < stocks.Count; i++)
            {
                output += ("\nPercent of portfolio: "+ Math.Round(((float)stocks[i].Quantity/total*100), 2) + "% " + details[i]);
            }
            return output;
        }
    }
}
