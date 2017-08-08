using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class WordType
    {
        WordType() { }
        public static string[] DistinguishType(string Word)
        {
            string[] WordType = new string[2];
            Word = Character.ConHiragana(Word);
            if (Word =="くる")
            {
                WordType[0] = "か变";
                WordType[1] = "固定";
                return WordType;
            }
            string SpecialWu = "あざける$あせる$いる$かえる$かぎる$きる$くつがえる$ける$さえぎる$しげる$しめる$しる$すべる$ちる$てる$にぎる$ねる$ののしる$はいる$はしる$へる$まいる$まじる$みなぎる";
            if (SpecialWu.Contains(Word))
            {
                WordType[0] = "五段";
                WordType[1] = "特殊五段";
                return WordType;
            }
            if (Word.Substring(Word.Length-2,2) == "する")
            {
                WordType[0] = "サ变";
                WordType[1] = "以「する」结尾";
                return WordType;
            }
            Coordinate cd = new Coordinate();
            string LastChar = Word.Substring(Word.Length - 1, 1);
            cd = DisCoordinate(LastChar);
            if (cd.CellCoor != -1 && cd.RowCoor != -1)
            {
                if (LastChar == "る")
                {
                    cd = DisCoordinate(Word.Substring(Word.Length - 2, 1));
                    if (cd.CellCoor != -1 && cd.RowCoor != -1)
                    {
                        if (cd.CellCoor == 1 || cd.CellCoor == 3)
                        {
                            WordType[0] = "一段";
                            WordType[1] = "以「る」结尾，「る」的前一个假名为「い」段或「え」段假名";
                        }
                        else
                        {
                            WordType[0] = "五段";
                            WordType[1] = "以「る」结尾，但其前一个假名不在「い段」或「え段」上";
                        }
                    }
                    else
                    {
                        WordType[0] = "无法判断";
                        WordType[1] = "倒数第二个字符不是日文假名";
                    }
                }
                else
                {
                    string Cell5 = "くぐすつぬぶむるう";
                    if (Cell5.Contains(LastChar))
                    {
                        WordType[0] = "五段";
                        WordType[1] = "不以「る」结尾的动词，词尾在「う」段上";
                    }
                    else
                    {
                        WordType[0] = "无法判断";
                        WordType[1] = "结尾的字符不符合动词规则";
                    }
                }
            }
            else
            {
                WordType[0] = "无法判断";
                WordType[1] = "结尾的字符不是日文假名";
            }
            return WordType;
        }
        public static Coordinate DisCoordinate(string inChar)
        {
            Coordinate cd = new Coordinate();
            cd.Sensitive = Sensitive(inChar);
            if (cd.Sensitive == "ExistsJap")
            {
                return cd;
            }
            string strHira = Character.ConSingleLetter(inChar);
            for (int i = 0; i < Character.Hiragana.GetLength(0); i++)
            {
                for (int j = 0; j < Character.Hiragana.GetLength(1); j++)
                {
                    if (Character.Hiragana[i,j]==inChar)
                    {
                        cd.RowCoor = i;
                        cd.CellCoor = j;
                        goto zxc;
                    }
                }
            }
        zxc: return cd;
        }
        public static string Sensitive(string ChKana)
        {
            if (Character.StrHiragana.Contains(ChKana))
            {
                return "Hiragana";
            }
            else if (Character.StrKatakana.Contains(ChKana))
            {
                return "Katakana";
            }
            else if (Character.StrHalfKata.Contains(ChKana))
            {
                return "HalfKata";
            }
            else
            {
                return "ExistsJap";
            }
        }
    }
}
