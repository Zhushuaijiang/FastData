﻿using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Fast.Untility.Base
{
    /// <summary>
    /// 标签：2015.9.6，魏中针
    /// 说明：加密类
    /// </summary>
    public static class BaseSymmetric
    {
        //变量
        private static SymmetricAlgorithm mobjCryptoService = new RijndaelManaged();
        private static string key = "Guz(%&hj7x89H$yuBI012345maT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP~!@#$";
        private static string p_strKey = "Weizz_2015";

        #region 获得密钥
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：获得密钥   
        /// </summary>   
        /// <returns>密钥</returns>   
        private static byte[] GetLegalKey()
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion

        #region 获得初始向量IV
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：获得初始向量IV   
        /// </summary>   
        /// <returns>初试向量IV</returns>   
        private static byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!~!@#$%^&*(";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion

        #region 加密方法
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：加密方法   
        /// <param name="Source">要加密的字符串</param>
        /// </summary>    
        public static string Encrypto(string Source)
        {
            Source = Source ?? "";
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }
        #endregion

        #region 解密方法
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：解密方法   
        /// <param name="Source">要解密的字符串</param>
        /// </summary>    
        public static string Decrypto(string Source, string refValue = "")
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mobjCryptoService.Key = GetLegalKey();
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                return refValue;
            }
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：MD5加密
        /// </summary>
        /// <param name="code">md5加密位数，16位或32位</param>
        /// <param name="Source">要加密的字符串</param>
        /// <returns></returns>
        public static string md5(int code, string Source)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToLower().Substring(8, 16);
            }
            else//32位加密 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToLower();
            }
        }
        #endregion

        #region Des 加密 GB2312
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：Des 加密 GB2312 
        /// </summary>
        /// <param name="Source">要加密（GB2312）字符串</param>
        /// <returns></returns>
        public static string EncodeGB2312(string Source)
        {
            Source = Source ?? "";
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(p_strKey.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(p_strKey.Substring(0, 8));
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(Source);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            stream.Close();
            return builder.ToString();
        }
        #endregion

        #region Des 解密 GB2312
        /// <summary>
        /// 标签：2015.7.13，魏中针
        /// 说明：Des 解密 GB2312 
        /// </summary>
        /// <param name="Source">要解密（GB2312）的字符串</param>
        /// <returns></returns>
        public static string DecodeGB2312(string Source,string refValue="")
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.ASCII.GetBytes(p_strKey.Substring(0, 8));
                provider.IV = Encoding.ASCII.GetBytes(p_strKey.Substring(0, 8));
                byte[] buffer = new byte[Source.Length / 2];
                for (int i = 0; i < (Source.Length / 2); i++)
                {
                    int num2 = Convert.ToInt32(Source.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num2;
                }
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                stream.Close();
                return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
            }
            catch
            {
                return refValue;
            }
        }
        #endregion
    }
}