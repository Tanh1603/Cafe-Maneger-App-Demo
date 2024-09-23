using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    public class VietnameseToEnglish
    {
        private VietnameseToEnglish() { }
        private static VietnameseToEnglish instance;

        public static VietnameseToEnglish Instance 
        { 
            get
            {
                if(instance == null) instance = new VietnameseToEnglish();
                return instance;
            }
            private set => instance = value;
        }

        public string  TranslateVnToEng(string str)
        {
            string stFormD = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

    }
}
