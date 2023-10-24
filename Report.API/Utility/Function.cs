﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class Function
{

    /***************************************************
     * Author    : CS Developers
     * Author URI: https://www.comscidev.com
     * Facebook  : https://www.facebook.com/CSDevelopers
    ***************************************************/
    public static string ThaiBahtText(object objNumber, bool IsTrillion = false)
    {
        string BahtText = "";
        string strTrillion = "";
        string[] strThaiNumber = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
        string[] strThaiPos = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };

        
        string strNumber = objNumber.ToString();
        decimal decNumber = 0;
        decimal.TryParse(strNumber, out decNumber);

        if (decNumber == 0)
        {
            return "ศูนย์บาทถ้วน";
        }

        strNumber = decNumber.ToString("0.00");
        string strInteger = strNumber.Split('.')[0];
        string strSatang = strNumber.Split('.')[1];

        if (strInteger.Length > 13)
            throw new Exception("รองรับตัวเลขได้เพียง ล้านล้าน เท่านั้น!");

        bool _IsTrillion = strInteger.Length > 7;
        if (_IsTrillion)
        {
            strTrillion = strInteger.Substring(0, strInteger.Length - 6);
            BahtText = ThaiBahtText(strTrillion, _IsTrillion);
            strInteger = strInteger.Substring(strTrillion.Length);
        }

        int strLength = strInteger.Length;
        for (int i = 0; i < strInteger.Length; i++)
        {
            string number = strInteger.Substring(i, 1);
            if (number != "0")
            {
                if (i == strLength - 1 && number == "1" && strLength != 1)
                {
                    BahtText += "เอ็ด";
                }
                else if (i == strLength - 2 && number == "2" && strLength != 1)
                {
                    BahtText += "ยี่";
                }
                else if (i != strLength - 2 || number != "1")
                {
                    BahtText += strThaiNumber[int.Parse(number)];
                }

                BahtText += strThaiPos[(strLength - i) - 1];
            }
        }

        if (IsTrillion)
        {
            return BahtText + "ล้าน";
        }

        if (strInteger != "0")
        {
            BahtText += "บาท";
        }

        if (strSatang == "00")
        {
            BahtText += "ถ้วน";
        }
        else
        {
            strLength = strSatang.Length;
            for (int i = 0; i < strSatang.Length; i++)
            {
                string number = strSatang.Substring(i, 1);
                if (number != "0")
                {
                    if (i == strLength - 1 && number == "1" && strSatang[0].ToString() != "0")
                    {
                        BahtText += "เอ็ด";
                    }
                    else if (i == strLength - 2 && number == "2" && strSatang[0].ToString() != "0")
                    {
                        BahtText += "ยี่";
                    }
                    else if (i != strLength - 2 || number != "1")
                    {
                        BahtText += strThaiNumber[int.Parse(number)];
                    }

                    BahtText += strThaiPos[(strLength - i) - 1];
                }
            }

            BahtText += "สตางค์";
        }

        return BahtText;
    }
}

