namespace Cor.Module.Helpers;

public class NumToWord
{
    private static readonly Random Random = new Random();
    public string IdGenerator(int cnt)
    {
        var seed = Random.Next(1, int.MaxValue);
        const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        var chars = new char[cnt];
        var rd = new Random(seed);

        for (var i = 0; i < cnt; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    public string NumGenerator(int cnt)
    {
        var seed = Random.Next(1, int.MaxValue);
        const string allowedChars = "0123456789";
        var chars = new char[cnt];
        var rd = new Random(seed);

        for (var i = 0; i < cnt; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    private static string Ones(string num)
    {
        var number = Convert.ToInt32(num);
        var name = "";
        switch (number)
        {

            case 1:
                name = "One";
                break;
            case 2:
                name = "Two";
                break;
            case 3:
                name = "Three";
                break;
            case 4:
                name = "Four";
                break;
            case 5:
                name = "Five";
                break;
            case 6:
                name = "Six";
                break;
            case 7:
                name = "Seven";
                break;
            case 8:
                name = "Eight";
                break;
            case 9:
                name = "Nine";
                break;
        }
        return name;
    }

    private static string Tens(string num)
    {
        var number = Convert.ToInt32(num);
        string name = null;
        switch (number)
        {
            case 10:
                name = "Ten";
                break;
            case 11:
                name = "Eleven";
                break;
            case 12:
                name = "Twelve";
                break;
            case 13:
                name = "Thirteen";
                break;
            case 14:
                name = "Fourteen";
                break;
            case 15:
                name = "Fifteen";
                break;
            case 16:
                name = "Sixteen";
                break;
            case 17:
                name = "Seventeen";
                break;
            case 18:
                name = "Eighteen";
                break;
            case 19:
                name = "Nineteen";
                break;
            case 20:
                name = "Twenty";
                break;
            case 30:
                name = "Thirty";
                break;
            case 40:
                name = "Forty";
                break;
            case 50:
                name = "Fifty";
                break;
            case 60:
                name = "Sixty";
                break;
            case 70:
                name = "Seventy";
                break;
            case 80:
                name = "Eighty";
                break;
            case 90:
                name = "Ninety";
                break;
            default:
                if (number > 0)
                {
                    name = Tens(num.Substring(0, 1) + "0") + " " + Ones(num.Substring(1));
                }
                break;
        }
        return name;
    }

    private static string ConToNum(string num)
    {
        var word = "";
        var isDone = false;//test if already translated    
        var dblAmt = (Convert.ToDouble(num));
        //if ((dblAmt > 0) && number.StartsWith("0"))    
        if (dblAmt > 0)
        {//test for zero or digit zero in a nuemric    
            var numDigits = num.Length;
            var pos = 0;//store digit grouping    
            var place = "";//digit grouping name:hundres,thousand,etc...    
            switch (numDigits)
            {
                case 1://ones' range    

                    word = Ones(num);
                    isDone = true;
                    break;
                case 2://tens' range    
                    word = Tens(num);
                    isDone = true;
                    break;
                case 3://hundreds' range    
                    pos = (numDigits % 3) + 1;
                    place = " Hundred ";
                    break;
                case 4://thousands' range    
                case 5:
                case 6:
                    pos = (numDigits % 4) + 1;
                    place = " Thousand ";
                    break;
                case 7://millions' range    
                case 8:
                case 9:
                    pos = (numDigits % 7) + 1;
                    place = " Million ";
                    break;
                case 10://Billions's range    
                case 11:
                case 12:

                    pos = (numDigits % 10) + 1;
                    place = " Billion ";
                    break;
                //add extra case options for anything above Billion...    
                default:
                    isDone = true;
                    break;
            }
            if (!isDone)
            {//if transalation is not done, continue...(Recursion comes in now!!)    
                if (num.Substring(0, pos) != "0" && num.Substring(pos) != "0")
                {
                    try
                    {
                        word = ConToNum(num.Substring(0, pos)) + place + ConToNum(num.Substring(pos));
                    }
                    catch { }
                }
                else
                {
                    word = ConToNum(num.Substring(0, pos)) + ConToNum(num.Substring(pos));
                }
                //check for trailing zeros    
                //if (beginsZero) word = " and " + word.Trim();    
            }
            //ignore digit grouping names    
            if (word.Trim().Equals(place.Trim())) word = "";
        }

        return word.Trim();
    }

    private static string ConToD(string number)
    {
        var cd = "";
        foreach (var t in number)
        {
            var digit = t.ToString();
            var engOne = digit.Equals("0") ? "Zero" : Ones(digit);
            cd += " " + engOne;
        }
        return cd;
    }

    private static string ConToW(string numb)
    {
        var wholeNo = numb;
        string andStr = "", pointStr = "";
        //var endStr = "Only";
        var endStr = "";
        var decimalPlace = numb.IndexOf(".", StringComparison.Ordinal);
        if (decimalPlace > 0)
        {
            wholeNo = numb.Substring(0, decimalPlace);
            var points = numb.Substring(decimalPlace + 1);
            if (Convert.ToInt32(points) > 0)
            {
                andStr = "and";// just to separate whole numbers from points/cents    
                endStr = "cents ";//Cents    
                                  //endStr = "cents " + endStr;//Cents    
                pointStr = ConToD(points);
            }
        }
        var val = $"{ConToNum(wholeNo).Trim()} {andStr}{pointStr} {endStr}";
        return val;
    }

    public string ConvertNum(double n)
    {
        var num = n.ToString();
        var isNegative = "";
        string str;
        if (num.Contains("-"))
        {
            isNegative = "Minus ";
            num = num.Substring(1, num.Length - 1);
        }
        if (num == "0")
        {
            str = "Zero";
        }
        else
        {
            str = isNegative + ConToW(num);
        }

        return str;
    }

    private static string OnesAm(string num)
    {
        var number = Convert.ToInt32(num);
        var name = "";
        switch (number)
        {

            case 1:
                name = "አንድ";
                break;
            case 2:
                name = "ሁለት";
                break;
            case 3:
                name = "ሶስት";
                break;
            case 4:
                name = "አራት";
                break;
            case 5:
                name = "አምስት";
                break;
            case 6:
                name = "ስድስት";
                break;
            case 7:
                name = "ሰባት";
                break;
            case 8:
                name = "ስምት";
                break;
            case 9:
                name = "ዘጠኝ";
                break;
        }
        return name;
    }

    private static string TensAm(string num)
    {
        var number = Convert.ToInt32(num);
        string name = null;
        switch (number)
        {
            case 10:
                name = "አስር";
                break;
            case 11:
                name = "አስራ አንድ";
                break;
            case 12:
                name = "አስራ ሁለት";
                break;
            case 13:
                name = "አስራ ሶስት";
                break;
            case 14:
                name = "አስራ አራት";
                break;
            case 15:
                name = "አስራ አምስት";
                break;
            case 16:
                name = "አስራ ስድስት";
                break;
            case 17:
                name = "አስራ ሰባት";
                break;
            case 18:
                name = "አስራ ስምንት";
                break;
            case 19:
                name = "አስራ ዘጠኝ";
                break;
            case 20:
                name = "ሃያ";
                break;
            case 30:
                name = "ሰላሳ";
                break;
            case 40:
                name = "አርባ";
                break;
            case 50:
                name = "ሃምሳ";
                break;
            case 60:
                name = "ስልሳ";
                break;
            case 70:
                name = "ሰባ";
                break;
            case 80:
                name = "ሰማንያ";
                break;
            case 90:
                name = "ዘጠና";
                break;
            default:
                if (number > 0)
                {
                    name = TensAm(num.Substring(0, 1) + "0") + " " + OnesAm(num.Substring(1));
                }
                break;
        }
        return name!;
    }

    private static string ConToNumAm(string num)
    {
        var word = "";
        var isDone = false;//test if already translated    
        var dblAmt = (Convert.ToDouble(num));
        //if ((dblAmt > 0) && number.StartsWith("0"))    
        if (dblAmt > 0)
        {//test for zero or digit zero in a nuemric    
            var numDigits = num.Length;
            var pos = 0;//store digit grouping    
            var place = "";//digit grouping name:hundres,thousand,etc...    
            switch (numDigits)
            {
                case 1://ones' range    

                    word = OnesAm(num);
                    isDone = true;
                    break;
                case 2://tens' range    
                    word = TensAm(num);
                    isDone = true;
                    break;
                case 3://hundreds' range    
                    pos = (numDigits % 3) + 1;
                    place = " መቶ ";
                    break;
                case 4://thousands' range    
                case 5:
                case 6:
                    pos = (numDigits % 4) + 1;
                    place = " ሺህ ";
                    break;
                case 7://millions' range    
                case 8:
                case 9:
                    pos = (numDigits % 7) + 1;
                    place = " ሚሊዮን ";
                    break;
                case 10://Billions's range    
                case 11:
                case 12:

                    pos = (numDigits % 10) + 1;
                    place = " ቢሊዮን ";
                    break;
                //add extra case options for anything above Billion...    
                default:
                    isDone = true;
                    break;
            }
            if (!isDone)
            {//if transalation is not done, continue...(Recursion comes in now!!)    
                if (num.Substring(0, pos) != "0" && num.Substring(pos) != "0")
                {
                    try
                    {
                        word = ConToNumAm(num.Substring(0, pos)) + place + ConToNumAm(num.Substring(pos));
                    }
                    catch { }
                }
                else
                {
                    word = ConToNumAm(num.Substring(0, pos)) + ConToNumAm(num.Substring(pos));
                }
                //check for trailing zeros    
                //if (beginsZero) word = " and " + word.Trim();    
            }
            //ignore digit grouping names    
            if (word.Trim().Equals(place.Trim())) word = "";
        }

        return word.Trim();
    }

    private static string ConToDAm(string number)
    {
        var cd = "";
        foreach (var t in number)
        {
            var digit = t.ToString();
            var engOne = digit.Equals("0") ? "ዜሮ" : OnesAm(digit);
            cd += " " + engOne;
        }
        return cd;
    }

    private static string ConToWAm(string numb)
    {
        var wholeNo = numb;
        string andStr = "", pointStr = "";
        //var endStr = "Only";
        var endStr = "";
        var decimalPlace = numb.IndexOf(".", StringComparison.Ordinal);
        if (decimalPlace > 0)
        {
            wholeNo = numb.Substring(0, decimalPlace);
            var points = numb.Substring(decimalPlace + 1);
            if (Convert.ToInt32(points) > 0)
            {
                andStr = "እና";// just to separate whole numbers from points/cents    
                endStr = "ሳንቲም ";//Cents    
                                 //endStr = "cents " + endStr;//Cents    
                pointStr = ConToDAm(points);
            }
        }
        var val = $"{ConToNumAm(wholeNo).Trim()} {andStr}{pointStr} {endStr}";
        return val;
    }

    public string ConvertNumAm(double n)
    {
        var num = n.ToString();
        var isNegative = "";
        string str;
        if (num.Contains("-"))
        {
            isNegative = "- ";
            num = num.Substring(1, num.Length - 1);
        }
        if (num == "0")
        {
            str = "ዜሮ";
        }
        else
        {
            str = isNegative + ConToWAm(num);
        }

        return str;
    }
}