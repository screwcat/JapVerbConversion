using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common.Entity
{
	/// <summary>
	/// CharToKana:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
    //[Serializable]
    [TableAttribute("CommonLog")]
    public class CharToKana : EntityBase<CharToKana>
	{
		public CharToKana()
		{}
		#region Model
		private int _id;
		private string _character;
		private string _kana1;
		private string _kana2;
		private string _kana3;
		private string _kana4;
		private string _kana5;
		private string _kana6;
		private string _kana7;
		private string _kana8;
		private string _kana9;
		private string _kana10;
		private string _kana11;
		private string _kana12;
		/// <summary>
		/// 
		/// </summary>
        [PrimaryKey("ID", IsAuto = false)]
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string Character
		{
			set{ _character=value;}
			get{return _character;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana1
		{
			set{ _kana1=value;}
			get{return _kana1;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana2
		{
			set{ _kana2=value;}
			get{return _kana2;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana3
		{
			set{ _kana3=value;}
			get{return _kana3;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana4
		{
			set{ _kana4=value;}
			get{return _kana4;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana5
		{
			set{ _kana5=value;}
			get{return _kana5;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana6
		{
			set{ _kana6=value;}
			get{return _kana6;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana7
		{
			set{ _kana7=value;}
			get{return _kana7;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana8
		{
			set{ _kana8=value;}
			get{return _kana8;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana9
		{
			set{ _kana9=value;}
			get{return _kana9;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana10
		{
			set{ _kana10=value;}
			get{return _kana10;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana11
		{
			set{ _kana11=value;}
			get{return _kana11;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Property]
		public string kana12
		{
			set{ _kana12=value;}
			get{return _kana12;}
		}
		#endregion Model

	}
}

