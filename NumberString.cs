using System;
using System.Collections.Generic;
using System.Text;
/* Copyright 2015 Glen Boothby http://www.codehack.uk */
namespace glboothby.Models
{
    public class NumberString
    {
        #region Properties
        public bool IsNegative { get; set; }
        public bool LongScale { get; set; }
        public List<char> IntegerPart { get; set; }
        public List<char> DecimalPart { get; set; }
        public StringBuilder IntergerWords { get; set; }
        public StringBuilder DecimalWords { get; set; }
        public bool HasDecimal { get { return this.DecimalPart.Count > 0; } }
        public bool HasInteger { get { return this.IntegerPart.Count > 0; } }
        public bool DecimalAllZero
        {
            get
            {
                bool returnValue = true;
                string[] parts = this.DecimalWords.ToString().Split(' ');
                foreach (string part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part) && part.Trim() != "zero")
                    {
                        returnValue = false;
                        break;
                    }
                }
                return returnValue;
            }
        }
        #endregion

        #region Constructors
        public NumberString(bool longScale = false)
        {
            Clear();
            this.LongScale = longScale;
        }
        #endregion

        public void Clear()
        {
            this.LongScale = false;
            this.IsNegative = false;
            this.IntegerPart = new List<char>();
            this.DecimalPart = new List<char>();
            this.IntergerWords = new StringBuilder();
            this.DecimalWords = new StringBuilder();
        }
        public override string ToString()
        {
            string returnValue = String.Empty;

            if (HasInteger || HasDecimal)
            {
                string i = IntergerWords.ToString();
                string d = DecimalWords.ToString();

                returnValue = String.IsNullOrWhiteSpace(i) ? "zero " : i;
                if (HasDecimal && !DecimalAllZero) { returnValue += "point " + d; }
            }
            else { returnValue = "empty"; }

            return FirstLetterUpper(returnValue);
        }
        private string FirstLetterUpper(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                input = input.Trim().ToLower();
                if (input != "zero" && input != "empty")
                {
                    if (this.IsNegative) { input = "minus " + input; }
                }
                if (input.Length > 1) { input = input.Substring(0, 1).ToUpper() + input.Substring(1); }
                else { input = input.ToUpper(); }
                input += ".";
            }
            return input;
        }
        public void MakeParts(string input)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                input = input.Trim();
                this.IsNegative = input.Substring(0, 1) == "-";
                string[] parts = input.Split('.');
                if (parts.Length > 0)
                {
                    this.IntegerPart = MakePart(parts[0]);
                    WordInteger();
                }
                if (parts.Length > 1)
                {
                    this.DecimalPart = MakePart(parts[1]);
                    WordDecimal();
                }
            }
        }
        private List<char> MakePart(string part)
        {
            List<char> returnValue = new List<char>();
            if (!String.IsNullOrWhiteSpace(part))
            {
                part = part.ToLower();
                foreach (char c in part)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z')) { returnValue.Add(c); }
                }
            }
            return returnValue;
        }
        private void WordDecimal()
        {
            foreach (char c in this.DecimalPart) { this.DecimalWords.Append(GetUnit(c)); }
        }
        private void WordInteger()
        {
            List<char[]> threes = SplitThrees(this.IntegerPart);
            int place = 0;
            bool addAnd = false;

            foreach (char[] chars in threes)
            {
                bool threeZeros = true;
                foreach (char c in chars)
                {
                    if (NonZero(c)) { threeZeros = false; break; }
                }
                if (!threeZeros)
                {
                    string seperator = " ";
                    if (addAnd)
                    {
                        this.IntergerWords.Insert(0, "and ");
                        addAnd = false;
                    }
                    else if (this.IntergerWords.Length > 1) { seperator = ", "; }

                    this.IntergerWords.Insert(0, GetPlace(place, seperator));
                    if (NonZero(chars[0]))
                    {
                        if (chars[1] != '1')
                        {
                            this.IntergerWords.Insert(0, GetUnit(chars[0]));
                            if (NonZero(chars[1])) { this.IntergerWords.Insert(0, GetTens(chars[1])); }
                        }
                        else { this.IntergerWords.Insert(0, GetTeens(chars[0])); }

                        addAnd = true;
                    }
                    else if (NonZero(chars[1]))
                    {
                        this.IntergerWords.Insert(0, GetTens(chars[1]));
                        addAnd = true;
                    }
                    if (NonZero(chars[2]))
                    {
                        if (addAnd)
                        {
                            this.IntergerWords.Insert(0, "and ");
                            addAnd = false;
                        }
                        this.IntergerWords.Insert(0, GetUnit(chars[2]) + "hundred ");
                    }
                }
                place = place + 3;
            }
        }
        private bool NonZero(char c)
        {
            return c != '0' && c != ' ';
        }
        private List<char[]> SplitThrees(List<char> list)
        {
            List<char[]> returnValue = new List<char[]>();

            for (int i = list.Count - 1; i >= 0; i = i - 3)
            {
                char[] chars = new char[3];
                chars[0] = list[i];
                if (i - 1 >= 0)
                {
                    chars[1] = list[i - 1];
                    if (i - 2 >= 0) { chars[2] = list[i - 2]; }
                    else { chars[2] = ' '; }
                }
                else
                {
                    chars[1] = ' ';
                    chars[2] = ' ';
                }
                returnValue.Add(chars);
            }

            return returnValue;
        }

        private static string GetUnit(char c)
        {
            string returnValue = String.Empty;
            if (c != ' ')
            {
                switch (c)
                {
                    case '0': returnValue = "zero"; break;
                    case '1': returnValue = "one"; break;
                    case '2': returnValue = "two"; break;
                    case '3': returnValue = "three"; break;
                    case '4': returnValue = "four"; break;
                    case '5': returnValue = "five"; break;
                    case '6': returnValue = "six"; break;
                    case '7': returnValue = "seven"; break;
                    case '8': returnValue = "eight"; break;
                    case '9': returnValue = "nine"; break;
                    default: returnValue = c.ToString(); break;
                }
                if (String.IsNullOrWhiteSpace(returnValue)) { throw new ArgumentOutOfRangeException(String.Format("Cannot conver char to unit: {0}", c)); }
            }
            return returnValue + " ";
        }
        private static string GetTens(char c)
        {
            string returnValue = string.Empty;
            if (c != ' ')
            {
                switch (c)
                {
                    case '1': returnValue = "ten"; break;
                    case '2': returnValue = "twenty"; break;
                    case '3': returnValue = "thirty"; break;
                    case '4': returnValue = "forty"; break;
                    case '5': returnValue = "fifty"; break;
                    case '6': returnValue = "sixty"; break;
                    case '7': returnValue = "seventy"; break;
                    case '8': returnValue = "eighty"; break;
                    case '9': returnValue = "ninety"; break;
                    default: returnValue = c.ToString(); break;
                }
                if (string.IsNullOrWhiteSpace(returnValue)) { throw new ArgumentOutOfRangeException(String.Format("Cannot conver char to Tens: {0}", c)); }
            }
            return returnValue + " ";
        }
        private static string GetTeens(char c)
        {
            string returnValue = string.Empty;
            switch (c)
            {
                case '1': returnValue = "eleven"; break;
                case '2': returnValue = "twelve"; break;
                case '3': returnValue = "thirteen"; break;
                case '4': returnValue = "fourteen"; break;
                case '5': returnValue = "fifteen"; break;
                case '6': returnValue = "sixteen"; break;
                case '7': returnValue = "seventeen"; break;
                case '8': returnValue = "eighteen"; break;
                case '9': returnValue = "nineteen"; break;
                default: returnValue = "1" + c.ToString(); break;
            }
            if (string.IsNullOrWhiteSpace(returnValue)) { throw new ArgumentOutOfRangeException(String.Format("Cannot convert char to Teens: {0}", c)); }
            return returnValue + " ";
        }
        private string GetPlace(int place, string seperator = " ")
        {
            string returnValue = String.Empty;
            switch (place)
            {
                case 0:  returnValue = String.Empty; break;
                case 3:  returnValue = "thousand"; break;
                case 6:  returnValue = "million"; break;
                case 9:  returnValue = this.LongScale ? "milliard" : "billion"; break;
                case 12: returnValue = this.LongScale ? "billion" : "trillion"; break;
                case 15: returnValue = this.LongScale ? "billiard" : "quadrillion"; break;
                case 18: returnValue = this.LongScale ? "trillion" : "quintillion"; break;
                case 21: returnValue = this.LongScale ? "trilliard" : "sextillion"; break;
                case 24: returnValue = this.LongScale ? "quadrillion" : "septillion"; break;
                case 27: returnValue = this.LongScale ? "quadrilliard" : "octillion"; break;
                case 30: returnValue = this.LongScale ? "quintillion" : "nonillion"; break;
                case 33: returnValue = this.LongScale ? "quintilliard" : "decillion"; break;
                case 36: returnValue = this.LongScale ? "sextillion" : "undecillion"; break;
                case 39: returnValue = this.LongScale ? "sextilliard" : "duodecillion"; break;
                case 42: returnValue = this.LongScale ? "septillion" : "tredecillion"; break;
                case 45: returnValue = this.LongScale ? "septilliard" : "quattuordecillion"; break;
                case 48: returnValue = this.LongScale ? "octillion" : "quindecillion"; break;
                case 51: returnValue = this.LongScale ? "octilliard" : "sexdecillion"; break;
                case 54: returnValue = this.LongScale ? "nonillion" : "septendecillion"; break;
                case 57: returnValue = this.LongScale ? "nonilliard" : "octodecillion"; break;
                case 60: returnValue = this.LongScale ? "decillion" : "novemdecillion"; break;
                case 63: returnValue = this.LongScale ? "decilliard" : "vigintillion"; break;
                case 66: returnValue = this.LongScale ? "undecillion" : "unvigintillion"; break;
                case 69: returnValue = this.LongScale ? "undecilliard" : "dovigintillion"; break;
                case 72: returnValue = this.LongScale ? "duodecillion" : "trevigintillion"; break;
                case 75: returnValue = this.LongScale ? "duodecilliard" : "quattuorvigintillion"; break;
                case 78: returnValue = this.LongScale ? "tredecillion" : "quinvigintillion"; break;
                case 81: returnValue = this.LongScale ? "tredecilliard" : "sexvigintillion"; break;
                case 84: returnValue = this.LongScale ? "quattuordecillion" : "septenvigintillion"; break;
                case 87: returnValue = this.LongScale ? "quattuordecilliard" : "octovigintillion"; break;
                case 90: returnValue = this.LongScale ? "quindecillion" : "novemvigintillion"; break;
                case 93: returnValue = this.LongScale ? "quindecilliard" : "trigintillion"; break;
                case 96: returnValue = this.LongScale ? "sexdecillion" : "untrigintillion"; break;
                case 99: returnValue = this.LongScale ? "sexdecilliard" : "dotrigintillion"; break;
                case 102:returnValue = this.LongScale ? "septendecillion" : "tretrigintillion"; break;
                case 105:returnValue = this.LongScale ? "septendecilliard" : "quattuortrigintillion"; break;
                case 108:returnValue = this.LongScale ? "octodecillion" : "quintrigintillion"; break;
                case 111:returnValue = this.LongScale ? "octodecilliard" : "sextrigintillion";break;
                case 114:returnValue = this.LongScale ? "novemdecillion" : "septentrigintillion";break;
                case 117:returnValue = this.LongScale ? "novemdecilliard" : "octotrigintillion"; break;
                case 120:returnValue = this.LongScale ? "vigintillion" : "novemtrigintillion"; break;
                case 123:returnValue = this.LongScale ? "vigintilliard" : "quadragintillion"; break;
                case 126: returnValue = this.LongScale ? "unvigintillion" : "unquadragintillion"; break;
                case 129: returnValue = this.LongScale ? "unvigintilliard" : "duoquadragintillion"; break;
                case 132: returnValue = this.LongScale ? "duovigintillion" : "trequadragintillion"; break;
                case 135: returnValue = this.LongScale ? "duovigintilliard" : "quattuorquadragintillion"; break;
                case 138: returnValue = this.LongScale ? "trevigintillion" : "quinquadragintillion"; break;
                case 141: returnValue = this.LongScale ? "trevigintilliard" : "sexquadragintillion"; break;
                case 144: returnValue = this.LongScale ? "quattuorvigintillion" : "septquadragintillion"; break;
                case 147: returnValue = this.LongScale ? "quattuorvigintilliard" : "octoquadragintillion"; break;
                case 150: returnValue = this.LongScale ? "quinvigintillion" : "novemquadragintillion"; break;
                case 153: returnValue = this.LongScale ? "quinvigintilliard" : "quinquagintillion"; break;
                case 156: returnValue = this.LongScale ? "sexvigintillion" : "unquinquagintillion"; break;
                case 159: returnValue = this.LongScale ? "sexvigintilliard" : "duoquinquagintillion"; break;
                case 162: returnValue = this.LongScale ? "septenvigintillion" : "trequinquagintillion"; break;
                case 165: returnValue = this.LongScale ? "septenvigintilliard" : "quattuorquinquagintillion"; break;
                case 168: returnValue = this.LongScale ? "octovigintillion" : "quinquinquagintillion"; break;
                case 171: returnValue = this.LongScale ? "octovigintilliard" : "sexquinquagintillion"; break;
                case 174: returnValue = this.LongScale ? "novemvigintillion" : "septquinquagintillion"; break;
                case 177: returnValue = this.LongScale ? "novemvigintilliard" : "octoquinquagintillion"; break;
                case 180: returnValue = this.LongScale ? "trigintillion" : "novemquinquagintillion"; break;
                case 183: returnValue = this.LongScale ? "trigintilliard" : "sexagintillion"; break;
                case 186: returnValue = this.LongScale ? "untrigintillion" : "unsexagintillion"; break;
                case 189: returnValue = this.LongScale ? "untrigintilliard" : "duosexagintillion"; break;
                case 192: returnValue = this.LongScale ? "duotrigintillion" : "tresexagintillion"; break;
                case 195: returnValue = this.LongScale ? "duotrigintilliard" : "quattuorsexagintillion"; break;
                case 198: returnValue = this.LongScale ? "tretrigintillion" : "quinsexagintillion"; break;
                case 201: returnValue = this.LongScale ? "tretrigintilliard" : "sexsexagintillion"; break;
                case 204: returnValue = this.LongScale ? "quattuortrigintillion" : "septsexagintillion"; break;
                case 207: returnValue = this.LongScale ? "quattuortrigintilliard" : "octosexagintillion"; break;
                case 210: returnValue = this.LongScale ? "quintrigintillion" : "novemsexagintillion"; break;
                case 213: returnValue = this.LongScale ? "quintrigintilliard" : "septuagintillion"; break;
                case 216: returnValue = this.LongScale ? "sextrigintillion" : "unseptuagintillion"; break;
                case 219: returnValue = this.LongScale ? "sextrigintilliard" : "duoseptuagintillion"; break;
                case 222: returnValue = this.LongScale ? "septentrigintillion" : "treseptuagintillion"; break;
                case 225: returnValue = this.LongScale ? "septentrigintilliard" : "quattuorseptuagintillion"; break;
                case 228: returnValue = this.LongScale ? "octotrigintillion" : "quinseptuagintillion"; break;
                case 231: returnValue = this.LongScale ? "octotrigintilliard" : "sexseptuagintillion"; break;
                case 234: returnValue = this.LongScale ? "octotrigintilliard" : "septseptuagintillion"; break;
                case 237: returnValue = this.LongScale ? "novemtrigintilliard" : "octoseptuagintillion"; break;
                case 240: returnValue = this.LongScale ? "quadragintillion" : "novemseptuagintillion"; break;
                case 243: returnValue = this.LongScale ? "quadragintilliard" : "octogintillion"; break;
                case 246: returnValue = this.LongScale ? "unquadragintillion" : "unoctogintillion"; break;
                case 249: returnValue = this.LongScale ? "unquadragintilliard" : "duooctogintillion"; break;
                case 252: returnValue = this.LongScale ? "duoquadragintillion" : "treoctogintillion"; break;
                case 255: returnValue = this.LongScale ? "duoquadragintilliard" : "quattuoroctogintillion"; break;
                case 258: returnValue = this.LongScale ? "trequadragintillion" : "quinoctogintillion"; break;
                case 261: returnValue = this.LongScale ? "trequadragintilliard" : "sexoctogintillion"; break;
                case 264: returnValue = this.LongScale ? "quattuorquadragintillion" : "septoctogintillion"; break;
                case 267: returnValue = this.LongScale ? "quattuorquadragintilliard" : "octooctogintillion"; break;
                case 270: returnValue = this.LongScale ? "quinquadragintillion" : "novemoctogintillion"; break;
                case 273: returnValue = this.LongScale ? "quinquadragintilliard" : "nonagintillion"; break;
                case 276: returnValue = this.LongScale ? "sexquadragintillion" : "unnonagintillion"; break;
                case 279: returnValue = this.LongScale ? "sexquadragintilliard" : "duononagintillion"; break;
                case 282: returnValue = this.LongScale ? "septenquadragintillion" : "trenonagintillion"; break;
                case 285: returnValue = this.LongScale ? "septenquadragintilliard" : "quattuornonagintillion"; break;
                case 288: returnValue = this.LongScale ? "octoquadragintillion" : "quinnonagintillion"; break;
                case 291: returnValue = this.LongScale ? "octoquadragintilliard" : "sexnonagintillion"; break;
                case 294: returnValue = this.LongScale ? "novemquadragintillion" : "septnonagintillion"; break;
                case 297: returnValue = this.LongScale ? "novemquadragintilliard" : "octononagintillion"; break;
                case 300: returnValue = this.LongScale ? "quinquagintillion" : "novemnonagintillion"; break;
                case 303: returnValue = this.LongScale ? "quinquagintilliard" : "centillion"; break;
                default: returnValue = "error"; break;
            }
            if (returnValue == "error") { throw new ArgumentOutOfRangeException(String.Format("Do not have place name for 10^{0}.", place)); }
            return returnValue + seperator;
        }
    }
}
