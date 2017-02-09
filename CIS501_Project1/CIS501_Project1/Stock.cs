using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS501_Project1
{
    class Stock
    {
        /// <summary>
        /// price of stock in previous period
        /// </summary>
        private float previousPrice;

        /// <summary>
        /// The ticker, immutable
        /// </summary>
        private string ticker;

        /// <summary>
        /// Name of the company, immutable
        /// </summary>
        private string name;

        /// <summary>
        /// The stockprice
        /// </summary>
        private float stockPrice;

        /// <summary>
        /// Displays the ticker
        /// </summary>
        public string Ticker
        {
            get
            {
                return ticker;
            }
        }

        /// <summary>
        /// Gets or sets the previous price (period)
        /// </summary>
        public float PreviousPrice
        {
            get
            {
                return previousPrice;
            }
            set
            {
                previousPrice = value;
            }
        }

        /// <summary>
        /// Gets and sets the stockprice for this stock object
        /// </summary>
        public float StockPrice
        {
            get
            {
                return stockPrice;
            }
            set
            {
                stockPrice = value;
            }
        }

        /// <summary>
        /// Gets the name of the stock
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

        }


        /// <summary>
        /// Constructor for instantiating this object
        /// </summary>
        /// <param name="tick">The ticker</param>
        /// <param name="price">The price of the stock</param>
        /// <param name="name">The name of the company associated with the stock</param>
        public Stock(string tick, float price, string name)
        {
            ticker = tick;
            stockPrice = price;
            this.name = name;
            previousPrice = price;
        }

        public override string ToString()
        {
            string gainlosspercent = "";
            string gainlossvalue = "";
            float gl = (float)Math.Round(gainLossPercent(), 2);
            float glv = (float)Math.Round(gainLossValue(), 2);
            if (gl > 0)
            {
                gainlosspercent = "+" + gl;
                gainlossvalue = ", up $" + glv;
            }
            else
            {
                gainlosspercent = gl.ToString();
                if (glv == 0) gainlossvalue = "";
                else gainlossvalue = ", down $" + Math.Abs(glv);
            }
            return (ticker + " - " + name + ", $" + stockPrice + gainlossvalue + " (" + gainlosspercent + "%)");
        }

        /// <summary>
        /// Returns the period gains as a percent
        /// </summary>
        /// <returns></returns>
        private float gainLossPercent()
        {
            return (stockPrice - previousPrice) / previousPrice * 100f;
        }

        private float gainLossValue()
        {
            return (stockPrice - previousPrice);
        }
    }
}
