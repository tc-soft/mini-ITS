using System;
using System.Collections.Generic;
using System.Linq;

namespace mini_ITS.Core.Tests
{
    public class CaesarHelper
    {
        private List<char> _strLetter;
        private List<char> _strMatrix;

        public CaesarHelper()
        {
            _strLetter = Enumerable.Range(32, 95)
                .Select(x => Convert.ToChar(x))
                .Where(x => x != 39)
                .ToList();
            Random rnd = new();
            _strMatrix = _strLetter.OrderBy(x => rnd.Next()).Select(x => x).ToList();
        }
        public void PrintLetter()
        {
            _strLetter.ForEach(x => Console.Write(x));
        }
        public string Encrypt(string strToEncrypt)
        {
            var lstEncrypt = new List<char>();

            foreach (var item in strToEncrypt)
            {
                if (item == Convert.ToChar(39))
                {
                    throw new Exception($"Encrypt(string strToEncrypt): strToEncrypt contains illegal character '");
                }
                else
                {
                    var idx = _strLetter.FindIndex(x => x == item);
                    lstEncrypt.Add(_strMatrix[idx]);
                }
            }

            return string.Join(null, lstEncrypt) ;
        }
        public string Decrypt(string strToDecrypt)
        {
            var lstDecrypt = new List<char>();

            foreach (var item in strToDecrypt)
            {
                if (item == Convert.ToChar(39))
                {
                    throw new Exception($"Decrypt(string strToDecrypt): strToDecrypt contains illegal character '");
                }
                else
                {
                    var idx = _strMatrix.FindIndex(x => x == item);
                    lstDecrypt.Add(_strLetter[idx]);
                }
            }

            return string.Join(null, lstDecrypt);
        }
    }
}