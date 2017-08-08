using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Service;
using Service.Common.Entity;
using System.Collections;
using JapVerbConversion.App_Code;

namespace JapVerbConversion
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (txtIn.Text.Trim().Length > 1)
            {
                string[] strWordType = WordType.DistinguishType(txtIn.Text.Trim());
                if (strWordType[0]!="无法判断")
                {
                    lbCell.Text = "该词是“" + strWordType[0] + "”动词";
                }
                else
                {
                    lbCell.Text = "无法判断该词的类型！";
                }
                lbConRule.Text = "（" + strWordType[1] + "）";
                lbConRule.Location = new Point(lbCell.Location.X + lbCell.Width, lbConRule.Location.Y);
                string[] strMasu = WordConvert.MaSu(strWordType[0], txtIn.Text.Trim());
                string[] strNaI = WordConvert.NaI(strWordType[0], txtIn.Text.Trim());
                string[] strKeNeng = WordConvert.KeNeng(strWordType[0], txtIn.Text.Trim());
                string[] strTe = WordConvert.Te(strWordType[0], txtIn.Text.Trim());
                string[] strJiaDing = WordConvert.JiaDing(strWordType[0], txtIn.Text.Trim());
                string[] strYiXiang = WordConvert.YiXiang(strWordType[0], txtIn.Text.Trim());
                string[] strMingLing = WordConvert.MingLing(strWordType[0], txtIn.Text.Trim());
                string[] strBeiDong = WordConvert.BeiDong(strWordType[0], txtIn.Text.Trim());
                string[] strShiYi = WordConvert.ShiYi(strWordType[0], txtIn.Text.Trim());
                string[] strGuoQu = WordConvert.GuoQu(strWordType[0], txtIn.Text.Trim());
                panel1.Visible = true;
                lbMaSu.Text = strMasu[0];
                lbMaSuRule.Text = "（" + strMasu[1] + "）";
                lbMaSuRule.Location = new Point(lbMaSu.Location.X + lbMaSu.Width + 5, lbMaSuRule.Location.Y);
                lbNaI.Text = strNaI[0];
                lbNaIRule.Text = "（" + strNaI[1] + "）";
                lbNaIRule.Location = new Point(lbNaI.Location.X + lbNaI.Width + 5, lbNaIRule.Location.Y);
                lbTe.Text = strTe[0];
                lbTeRule.Text = "（" + strTe[1] + "）";
                lbTeRule.Location = new Point(lbTe.Location.X + lbTe.Width + 5, lbTeRule.Location.Y);
                lbKeNeng.Text = strKeNeng[0];
                lbKeNengRule.Text = "（" + strKeNeng[1] + "）";
                lbKeNengRule.Location = new Point(lbKeNeng.Location.X + lbKeNeng.Width + 5, lbKeNengRule.Location.Y);
                lbJiaDing.Text = strJiaDing[0];
                lbJiaDingRule.Text = "（" + strJiaDing[1] + "）";
                lbJiaDingRule.Location = new Point(lbJiaDing.Location.X + lbJiaDing.Width + 5, lbJiaDingRule.Location.Y);
                lbYiXiang.Text = strYiXiang[0];
                lbYiXiangRule.Text = "（" + strYiXiang[1] + "）";
                lbYiXiangRule.Location = new Point(lbYiXiang.Location.X + lbYiXiang.Width + 5, lbYiXiangRule.Location.Y);
                lbMingLing.Text = strMingLing[0];
                lbMingLingRule.Text = "（" + strMingLing[1] + "）";
                lbMingLingRule.Location = new Point(lbMingLing.Location.X + lbMingLing.Width + 5, lbMingLingRule.Location.Y);
                lbBeiDong.Text = strBeiDong[0];
                lbBeiDongRule.Text = "（" + strBeiDong[1] + "）";
                lbBeiDongRule.Location = new Point(lbBeiDong.Location.X + lbBeiDong.Width + 5, lbBeiDongRule.Location.Y);
                lbShiYi.Text = strShiYi[0];
                lbShiYiRule.Text = "（" + strShiYi[1] + "）";
                lbShiYiRule.Location = new Point(lbShiYi.Location.X + lbShiYi.Width + 5, lbShiYiRule.Location.Y);
                lbGuoQu.Text = strGuoQu[0];
                lbGuoQuRule.Text = "（" + strGuoQu[1] + "）";
                lbGuoQuRule.Location = new Point(lbGuoQu.Location.X + lbGuoQu.Width + 5, lbGuoQuRule.Location.Y);
            }
        }

        private void btnCh_Click(object sender, EventArgs e)
        {

            //cbSpell.DataSource = KanaConverter.SearchKana(txtIn1.Text.Trim());
            KanaOut.Text = KanaConverter.AnalysisWord(txtIn1.Text.Trim());
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            cbSign.DataSource = KanaConverter.SearchKana(txtSign.Text.Trim());
        }
    }
}
