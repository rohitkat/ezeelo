//-----------------------------------------------------------------------
// <copyright file="AutoSearch" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace BusinessLogicLayer
{
   public class AutoSearch : ICommonFuntionalities
    {

  
        /// <summary>
        /// Search for Shop or product depending on Search Key
        /// </summary>
        public enum SEARCHBY
        {
            /// <summary>
            /// Search by Shop/Product Keywords
            /// </summary>
            ALL = 0,
            /// <summary>
            /// Search by Shop Keywords
            /// </summary>
            SHOP=1,
            /// <summary>
            ///  Search by Product Keywords
            /// </summary>
            PRODUCT=2

        }
        /// <summary>
        /// Search by keyword related to shop 
        /// </summary>
        /// <param name="pre">Search Term</param>
        /// <param name="searchby">Search For Shop/Product</param>
        /// <returns>List of objects related to search</returns>
        // GET api/searchengine/lap/PRODUCT
        public IEnumerable<AutoSuggestViewModel> GetSearchMetaData(string pre, SEARCHBY searchby,int cityID, int? franchiseID=null)////added int? franchiseID for Multiple MCO
        {
            
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
          
            string query = "[SearchMetadata]";
            SqlCommand cmd = new SqlCommand(query);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Search_Key", pre);
            cmd.Parameters.AddWithValue("@Search_By", searchby.ToString());
            cmd.Parameters.AddWithValue("@CityID", cityID);
            cmd.Parameters.AddWithValue("@FranchiseID", franchiseID);

            DataTable dt = new DataTable();
            List<AutoSuggestViewModel> list = new List<AutoSuggestViewModel>();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);

                    foreach (DataRow r in dt.Rows)
                    {
                        AutoSuggestViewModel st = new AutoSuggestViewModel();
                        st.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(r["Keyword"].ToString().ToLower()); ;
                        st.Head = r["Head"].ToString();
                        st.Abbr = r["Abbr"].ToString();
                        st.ID = r["ID"].ToString();
                        st.Seperator = string.Empty;
                        st.Products = string.Empty;
                        list.Add(st);

                    }
                }
            }
            return list;
        }
        //yashaswi 27-9-2018
        public string GetSyllables(string word)
        {
            word = word.Trim();
            if (word.Contains(" "))
            {
                string[] splitword = word.Split(' ');
                word = splitword[0];
            }
            //word = word.Substring
            string Syllables = "";
            string remain = "";
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            string currentWord = word;
            int numVowels = 0;
            bool lastWasVowel = false;
            int currentCount = 0;
            int lastCount = 0;
            bool foundAny = false;
            foreach (char wc in currentWord)
            {
                bool foundVowel = false;
                foreach (char v in vowels)
                {
                    if (v == wc && lastWasVowel)
                    {
                        foundVowel = true;
                        lastWasVowel = true;
                        break;
                    }
                    else if (v == wc && !lastWasVowel)
                    {
                        int length = (currentCount - lastCount);
                        if (foundAny)
                        {
                            lastCount = lastCount + 1;
                        }
                        else
                        {
                            length = (currentCount - lastCount) + 1;
                        }
                        Syllables = Syllables + word.Substring(lastCount, length) + "-";
                        remain = word.Substring(lastCount + length, (word.Length) - (lastCount + length));
                        lastCount = currentCount;
                        numVowels++;
                        foundVowel = true;
                        lastWasVowel = true;
                        foundAny = true;
                        break;
                    }
                }

                //if full cycle and no vowel found, set lastWasVowel to false;
                if (!foundVowel)
                    lastWasVowel = false;

                currentCount++;
            }
            //remove es, it's _usually? silent
            if (currentWord.Length > 2 &&
                currentWord.Substring(currentWord.Length - 2) == "es")
                numVowels--;
            // remove silent e
            else if (currentWord.Length > 1 &&
                currentWord.Substring(currentWord.Length - 1) == "e")
                numVowels--;

            return Syllables + remain;
        }
        private void GetMiddleWordCombo(string middleStr, out string SecChar, out string ThirdChar, out string SecChar_, out string ThirdChar_, out string SecChar__, out string ThirdChar__)
        {

            SecChar = "";
            ThirdChar = "";
            SecChar_ = "";
            ThirdChar_ = "";
            SecChar__ = "";
            ThirdChar__ = "";
            if (middleStr.Length == 1)
            {
                SecChar = middleStr;
            }
            if (middleStr.Length == 2)
            {
                SecChar = middleStr.Substring(0, 1);
                ThirdChar = middleStr.Substring(1, 1);
            }
            if (middleStr.Length == 3)
            {
                SecChar = middleStr.Substring(0, 1);
                ThirdChar = middleStr.Substring(middleStr.Length - 1, 1);

                SecChar_ = middleStr.Substring(1, 1);
            }
            if (middleStr.Length == 4)
            {
                SecChar = middleStr.Substring(0, 1);
                ThirdChar = middleStr.Substring(middleStr.Length - 1, 1);

                SecChar_ = middleStr.Substring(0, 2);
                ThirdChar_ = middleStr.Substring(middleStr.Length - 2, 2);

                SecChar__ = middleStr.Substring(1, 2);
            }
        }
        public void GetSerachChar(string word, out string result, out string result1, out string result2, out string resultSound)
        {
            result = "";
            result1 = "";
            result2 = "";
            resultSound = "";
            if (word != "")
            {
                if (word.Substring(word.Length - 1, 1) == "-")
                {
                    word = word.Substring(0, word.Length - 1);
                }

                try
                {
                    string[] SplitWords = word.Split('-');
                    string FirstChar = "";
                    string SecChar = "";
                    string ThirdChar = "";
                    string SecChar_ = "";
                    string ThirdChar_ = "";
                    string SecChar__ = "";
                    string ThirdChar__ = "";
                    string FourthChar = "";
                    if (SplitWords.Length != 0)
                    {
                        FirstChar = word.Substring(0, 1);
                        FourthChar = word.Substring(word.Length - 1, 1);
                        resultSound = FirstChar + "%" + FourthChar;
                        switch (SplitWords.Length)
                        {
                            case 2:

                                if (SplitWords[1].Length > 1)
                                {
                                    result = FirstChar + "%" + SplitWords[1].Substring(0, 1) + "%" + FourthChar;
                                }
                                else
                                {
                                    result = FirstChar + "%" + FourthChar;
                                }

                                break;
                            case 3:
                                GetMiddleWordCombo(SplitWords[1], out  SecChar, out  ThirdChar, out  SecChar_, out  ThirdChar_, out  SecChar__, out  ThirdChar__);

                                result = FirstChar + "%" + SecChar +
                                    (ThirdChar == "" ? "" : "%" + ThirdChar)
                                    + "%" + FourthChar;

                                result1 = (SecChar_ == "" ? "" : FirstChar + "%" + SecChar_ +
                                    (ThirdChar_ == "" ? "" : "%" + ThirdChar_)
                                    + "%" + FourthChar);

                                result2 = (SecChar__ == "" ? "" : FirstChar + "%" + SecChar__ + "%" + FourthChar);

                                break;
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                GetMiddleWordCombo(SplitWords[1], out  SecChar, out  ThirdChar, out  SecChar_, out  ThirdChar_, out  SecChar__, out  ThirdChar__);

                                result = FirstChar + "%" + SecChar +
                                    (ThirdChar == "" ? "" : "%" + ThirdChar);


                                result1 = (SecChar_ == "" ? "" : FirstChar + "%" + SecChar_ +
                                    (ThirdChar_ == "" ? "" : "%" + ThirdChar_));

                                result2 = (SecChar__ == "" ? "" : FirstChar + "%" + SecChar__);

                                GetMiddleWordCombo(SplitWords[2], out  SecChar, out  ThirdChar, out  SecChar_, out  ThirdChar_, out  SecChar__, out  ThirdChar__);

                                result = (result == "" ? "" : result + "%" + (SecChar +
                                     (ThirdChar == "" ? "" : "%" + ThirdChar)));


                                result1 = (result1 == "" ? "" : result1 + "%" + (SecChar_ == "" ? "" : SecChar_ +
                                    (ThirdChar_ == "" ? "" : "%" + ThirdChar_)));

                                result2 = (result2 == "" ? "" : result2 + "%" + (SecChar__ == "" ? "" : SecChar__));

                                result = (result == "" ? "" : result + "%" + FourthChar);
                                result1 = (result1 == "" ? "" : result1 + "%" + FourthChar);
                                result2 = (result2 == "" ? "" : result2 + "%" + FourthChar);
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        //Yashaswi 27-9-2018
    }
}
