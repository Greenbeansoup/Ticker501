using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS501_Project1
{
    class Transaction
    {
        /// <summary>
        /// Quantity purchased
        /// </summary>
        private int quantity;

        /// <summary>
        /// either "buy" or "sell" depending on whether this is a buy or sell transaction
        /// </summary>
        private string buySell;

        /// <summary>
        /// The period when purchased
        /// </summary>
        private int period;

        /// <summary>
        /// The stock purchased
        /// </summary>
        private Stock stock;

        /// <summary>
        /// The price at purchase
        /// </summary>
        private float price;

        /// <summary>
        /// Getter for quantity
        /// </summary>
        public int Quantity
        {
            get
            {
                return quantity;
            }
        }

        /// <summary>
        /// Getter for period
        /// </summary>
        public int Period
        {
            get
            {
                return period;
            }
        }

        /// <summary>
        /// Getter for stock
        /// </summary>
        public Stock Stock
        {
            get
            {
                return stock;
            }
        }

        /// <summary>
        /// Getter for price
        /// </summary>
        public float Price
        {
            get
            {
                return price;
            }
        }

        /// <summary>
        /// Constructor for the Transaction class
        /// </summary>
        /// <param name="quant">Quantity purchased</param>
        /// <param name="per">Period purchased</param>
        /// <param name="stock">Stock purchased</param>
        /// <param name="price">Price of purchase</param>
        /// <param name="bs">Whether this is a buy or a sell</param>
        public Transaction(string bs, int quant, int per, Stock stock, float price)
        {
            quantity = quant;
            period = per;
            this.stock = stock;
            this.price = price;
            buySell = bs;
        }

        public override string ToString()
        {
            return (buySell.ToUpper() +", Period: " + period + "; " + stock.Ticker + " - " + stock.Name + ", Quantity: " + quantity + " at $" + price);
        }
    }
}
