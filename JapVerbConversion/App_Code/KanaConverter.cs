using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Service.Common.Entity;
using System.Data;
using Service;

namespace JapVerbConversion.App_Code
{
    public class KanaConverter
    {
        public static string AnalysisWord(string InWord)
        {
            //string Section = "";
            string OutKana = "";
            ArrayList KanaList = new ArrayList();
            for (int i = 0; i < InWord.Length; i++)
            {
                KanaList = SearchKana(InWord.Substring(i, 1));
                if (KanaList.Count > 0)
                {
                    for (int j = 0; j < KanaList.Count; j++)
                    {
                        if (KanaList[j].ToString().Contains("-"))
                        {
                            if (KanaList[j].ToString().Split(new Char[] { '-' })[1] == InWord.Substring(i + 1))
                            {
                                OutKana = KanaList[j].ToString().Replace("-", "");
                                goto zxc;
                            }
                        }
                    }
                    OutKana += KanaList[0];
                }
                else
                {
                    OutKana += InWord.Substring(i, 1);
                }
            }
        zxc: return Character.ConHiragana(OutKana);
        }
        public static ArrayList SearchKana(string ChnChar)
        {
            List<string[]> LiWhere = new List<string[]>();
            List<string[]> LiOrder = new List<string[]>();
            ArrayList al = new ArrayList();
            LiWhere.Add(new string[] { "Character LIKE '%{0}%'", ChnChar });
            CharToKana ctk = new Service.Common.Entity.CharToKana();
            ctk.TagData = Program.JapCharDict;
            DataTable dt = ctk.GetDataSet("SELECT ID, Character,kana1, kana2, kana3, kana4, kana5, kana6, kana7, kana8, kana9, kana10, kana11, kana12 FROM CharToKana", LiWhere);
            if (dt.Rows.Count>0)
            {
                for (int i = 2; i < 14; i++)
                {
                    al.Add(dt.Rows[0][i]);
                }
            }
            return al;
        }
    }
}
