using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters;
namespace Service
{
    public class Character
    {
        Character() { }
        public static string StrHiragana = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやいゆえよらりるれろわいうえをがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽぁぃぅぇぉかけゃゅょっゎん";
        public static string StrKatakana = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤイユエヨラリルレロワイウエヲガギグゲゴザジヅゼゾダヂヅデドバビブベボパピプペポァィゥェォヵヶャュョッヮン";
        public static string StrHalfKata = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔｲﾕｴﾖﾗﾘﾙﾚﾛﾜｲｳｴｦｶﾞｷﾞｸﾞｹﾞｺﾞｻﾞｼﾞﾂﾞｾﾞｿﾞﾀﾞﾁﾞﾂﾞﾃﾞﾄﾞﾊﾞﾋﾞﾌﾞﾍﾞﾎﾞﾊﾟﾋﾟﾌﾟﾍﾟﾎﾟｧｨｩｪｫｶｹｬｭｮｯﾜﾝ";
        public static string[,] Hiragana = 
        {
          { "あ", "い", "う", "え", "お" },
          { "か", "き", "く", "け", "こ" },
          { "さ", "し", "す", "せ", "そ" },
          { "た", "ち", "つ", "て", "と" },
          { "な", "に", "ぬ", "ね", "の" },
          { "は", "ひ", "ふ", "へ", "ほ" },
          { "ま", "み", "む", "め", "も" },
          { "や", "い", "ゆ", "え", "よ" },
          { "ら", "り", "る", "れ", "ろ" },
          { "わ", "い", "う", "え", "を" },
          { "が", "ぎ", "ぐ", "げ", "ご" },
          { "ざ", "じ", "ず", "ぜ", "ぞ" },
          { "だ", "ぢ", "づ", "で", "ど" },
          { "ば", "び", "ぶ", "べ", "ぼ" },
          { "ぱ", "ぴ", "ぷ", "ぺ", "ぽ" },
          { "ぁ", "ぃ", "ぅ", "ぇ", "ぉ" },
          { "か", "", "", "け", "" },
          { "ゃ", "", "ゅ", "", "ょ" },
          { "", "", "っ", "", "" },
          { "ゎ", "", "", "", "" },
          { "ん", "", "", "", "" }
        };
        static string[,] Katakana = 
        {
          { "ア", "イ", "ウ", "エ", "オ" },
          { "カ", "キ", "ク", "ケ", "コ" },
          { "サ", "シ", "ス", "セ", "ソ" },
          { "タ", "チ", "ツ", "テ", "ト" },
          { "ナ", "ニ", "ヌ", "ネ", "ノ" },
          { "ハ", "ヒ", "フ", "ヘ", "ホ" },
          { "マ", "ミ", "ム", "メ", "モ" },
          { "ヤ", "イ", "ユ", "エ", "ヨ" },
          { "ラ", "リ", "ル", "レ", "ロ" },
          { "ワ", "イ", "ウ", "エ", "ヲ" },
          { "ガ", "ギ", "グ", "ゲ", "ゴ" },
          { "ザ", "ジ", "ヅ", "ゼ", "ゾ" },
          { "ダ", "ヂ", "ヅ", "デ", "ド" },
          { "バ", "ビ", "ブ", "ベ", "ボ" },
          { "パ", "ピ", "プ", "ペ", "ポ" },
          { "ァ", "ィ", "ゥ", "ェ", "ォ" },
          { "ヵ", "", "", "ヶ", "" },
          { "ャ", "", "ュ", "", "ョ" },
          { "", "", "ッ", "", "" },
          { "ヮ", "", "", "", "" },
          { "ン", "", "", "", "" }
        };
        static string[,] KatakanaHalf = 
        {
          { "ｱ", "ｲ", "ｳ", "ｴ", "ｵ" },
          { "ｶ", "ｷ", "ｸ", "ｹ", "ｺ" },
          { "ｻ", "ｼ", "ｽ", "ｾ", "ｿ" },
          { "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ" },
          { "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ" },
          { "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ" },
          { "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ" },
          { "ﾔ", "ｲ", "ﾕ", "ｴ", "ﾖ" },
          { "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ" },
          { "ﾜ", "ｲ", "ｳ", "ｴ", "ｦ" },
          { "ｶﾞ", "ｷﾞ", "ｸﾞ", "ｹﾞ", "ｺﾞ" },
          { "ｻﾞ", "ｼﾞ", "ﾂﾞ", "ｾﾞ", "ｿﾞ" },
          { "ﾀﾞ", "ﾁﾞ", "ﾂﾞ", "ﾃﾞ", "ﾄﾞ" },
          { "ﾊﾞ", "ﾋﾞ", "ﾌﾞ", "ﾍﾞ", "ﾎﾞ" },
          { "ﾊﾟ", "ﾋﾟ", "ﾌﾟ", "ﾍﾟ", "ﾎﾟ" },
          { "ｧ", "ｨ", "ｩ", "ｪ", "ｫ" },
          { "ｶ", "", "", "ｹ", "" },
          { "ｬ", "", "ｭ", "", "ｮ" },
          { "", "", "ｯ", "", "" },
          { "ﾜ", "", "", "", "" },
          { "ﾝ", "", "", "", "" }
        };
        
        public static string ConHiragana(string InKat)
        {
            string OutHir = "";
            for (int i = 0; i < InKat.Length; i++)
            {
                OutHir += ConSingleLetter(InKat.Substring(i,1));
            }
            return OutHir;
        }


        public static string ConSingleLetter(string Kat)
        {
            string strHiragana = Kat;
            for (int i = 0; i < Katakana.GetLength(0); i++)
            {
                for (int j = 0; j < Katakana.GetLength(1); j++)
                {
                    if (Katakana[i,j] == Kat)
                    {
                        strHiragana = Hiragana[i, j];
                        break;
                    }
                    if (KatakanaHalf[i, j] == Kat)
                    {
                        strHiragana = Hiragana[i, j];
                        break;
                    }
                }
            }
            return strHiragana;
        }
    }
    public class Coordinate
    {
        int rowCoor;
        int cellCoor;
        string sensitive;

        public string Sensitive
        {
            get { return sensitive; }
            set { sensitive = value; }
        }
        public Coordinate()
        {
            sensitive = "ExistsJap";
            rowCoor = -1;
            cellCoor = -1;
        }
        public int RowCoor
        {
            get { return rowCoor; }
            set { rowCoor = value; }
        }
        public int CellCoor
        {
            get { return cellCoor; }
            set { cellCoor = value; }
        }
    }
}
