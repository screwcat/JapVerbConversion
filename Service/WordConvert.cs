using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters;
namespace Service
{
    public class WordConvert
    {
        WordConvert() { }
        TransliteralConverter tc = new TransliteralConverter();
        public static string[] MaSu(string Type,string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, cd.CellCoor - 1] + "ます";
                        OutWord[1] = "将结尾假名变成它同行的前一个假名后+「ます」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "ます";
                        OutWord[1] = "去「る」＋「ます」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "します";
                        OutWord[1] = "「する」→「します」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "かきます";
                        OutWord[1] = "か变固定";
                    } 
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] NaI(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        if (Word.Substring(WordLen-1,1)=="う")
                        {
                            OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[9, 0] + "ない";
                            OutWord[1] = "以「う」结尾的将「う」变成「わ」后+「ない」";
                        }
                        else
                        {
                            OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, 0] + "ない";
                            OutWord[1] = "将结尾假名变成它同行的あ段上的假名后+「ない」";
                        }
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "ない";
                        OutWord[1] = "去「る」+「ない」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "しない";
                        OutWord[1] = "「する」→「しない」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "かこない";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] Te(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            switch (Type)
            {
                case "五段":
                    {
                        if (Word == "いく")
                        {
                            OutWord[0] = "いって";
                            OutWord[1] = "「いく」特殊，按固定格式";
                        }
                        else
                        {
                            string Suffix = Word.Substring(WordLen - 1, 1);
                            if (Suffix == "く")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "いて";
                                OutWord[1] = "最后一个假名是「く」，去「く」+「いて」";
                            }
                            if (Suffix == "ぐ")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "いで";
                                OutWord[1] = "最后一个假名是「ぐ」，去「ぐ」+「いで」";
                            }
                            if (Suffix == "う" || Suffix == "つ" || Suffix == "る")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "って";
                                OutWord[1] = "最后一个假名是「う」、「つ」或「る」，将其去掉+「って」";
                            }
                            if (Suffix == "む" || Suffix == "ぬ" || Suffix == "ぶ")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "んで";
                                OutWord[1] = "最后一个假名是「む」、「ぬ」或「ぶ」，将其去掉+「んで」";
                            }
                            if (Suffix == "す")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "して";
                                OutWord[1] = "最后一个假名是「す」，去「す」+「して」";
                            }
                        }
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "て";
                        OutWord[1] = "去「る」+「て」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "して";
                        OutWord[1] = "「する」→「して」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "きて";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] KeNeng(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, cd.CellCoor + 1] + "る";
                        OutWord[1] = "将结尾假名变成它同行的下一个假名后+「る」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "られる";
                        OutWord[1] = "去「る」+「られる」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "できる";
                        OutWord[1] = "「する」→「できる」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "こられる";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] JiaDing(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, cd.CellCoor + 1] + "ば";
                        OutWord[1] = ":将结尾假名变成它同行的下一个假名后+「ば」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "れば";
                        OutWord[1] = "去「る」+「れば」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "すれば";
                        OutWord[1] = "「する」→「すれば」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "くれば";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] YiXiang(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, 4] + "う";
                        OutWord[1] = "将结尾假名变成它同行的最后一个假名后+「う」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "よう";
                        OutWord[1] = "去「る」+「よう」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "しよう";
                        OutWord[1] = "「する」→「しよう」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "こよう";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] MingLing(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, cd.CellCoor + 1];
                        OutWord[1] = "将结尾假名变成它同行的下一个假名即可";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "ろ";
                        OutWord[1] = "去「る」+「ろ」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "しろ";
                        OutWord[1] = "「する」→「しろ」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "こい";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] BeiDong(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, 0] + "れる";
                        OutWord[1] = "将结尾假名变成它所在行的あ段上假名后+「れる」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "られる";
                        OutWord[1] = "去「る」+「られる」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "される";
                        OutWord[1] = "「する」→「される」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "こられる";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] ShiYi(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            Coordinate cd = new Coordinate();
            switch (Type)
            {
                case "五段":
                    {
                        cd = WordType.DisCoordinate(Word.Substring(WordLen - 1, 1));
                        OutWord[0] = Word.Substring(0, WordLen - 1) + Character.Hiragana[cd.RowCoor, 0] + "せる";
                        OutWord[1] = "将结尾假名变成它所在行的あ段上假名后+「せる」";
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "させる";
                        OutWord[1] = "去「る」+「させる」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "させる";
                        OutWord[1] = "「する」→「させる」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "こさせる";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string[] GuoQu(string Type, string Word)
        {
            string[] OutWord = new string[2];
            Int32 WordLen = Word.Length;
            switch (Type)
            {
                case "五段":
                    {
                        if (Word == "いく")
                        {
                            OutWord[0] = "いった";
                            OutWord[1] = "「いく」特殊，按固定格式";
                        }
                        else
                        {
                            string Suffix = Word.Substring(WordLen - 1, 1);
                            if (Suffix == "く")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "いた";
                                OutWord[1] = "最后一个假名是「く」，去「く」+「いた」";
                            }
                            if (Suffix == "ぐ")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "いだ";
                                OutWord[1] = "最后一个假名是「ぐ」，去「く」+「いだ」";
                            }
                            if (Suffix == "う" || Suffix == "つ" || Suffix == "る")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "った";
                                OutWord[1] = "最后一个假名是「う」、「つ」或「る」，将其去掉+「った」";
                            }
                            if (Suffix == "む" || Suffix == "ぬ" || Suffix == "ぶ")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "んだ";
                                OutWord[1] = "最后一个假名是「む」、「ぬ」或「ぶ」，将其去掉+「んだ」";
                            }
                            if (Suffix == "す")
                            {
                                OutWord[0] = Word.Substring(0, WordLen - 1) + "した";
                                OutWord[1] = "最后一个假名是「す」，去「す」+「した」";
                            }
                        }
                    }
                    break;
                case "一段":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 1) + "た";
                        OutWord[1] = "去「る」+「た」";
                    }
                    break;
                case "サ变":
                    {
                        OutWord[0] = Word.Substring(0, WordLen - 2) + "した";
                        OutWord[1] = "「する」→「した」";
                    }
                    break;
                case "か变":
                    {
                        OutWord[0] = "きた";
                        OutWord[1] = "か变固定";
                    }
                    break;
                default: break;
            }
            return OutWord;
        }
        public static string HalfwidthKatakanaToHiragana(string input)
        {
            return KanaConverter.HalfwidthKatakanaToHiragana(input);
        }
        public static string HalfwidthKatakanaToKatakana(string input)
        {
            return KanaConverter.HalfwidthKatakanaToKatakana(input);
        }
        public static string HiraganaToHalfwidthKatakana(string input)
        {
            return KanaConverter.HiraganaToHalfwidthKatakana(input);
        }
        public static string HiraganaToKatakana(string input)
        {
            return KanaConverter.HiraganaToKatakana(input);
        }
        public static string KatakanaToHalfwidthKatakana(string input)
        {
            return KanaConverter.KatakanaToHalfwidthKatakana(input);
        }
        public static string KatakanaToHiragana(string input)
        {
            return KanaConverter.KatakanaToHiragana(input);
        }
        public static string RomajiToHiragana(string input)
        {
            return KanaConverter.RomajiToHiragana(input);
        }
    }
}
