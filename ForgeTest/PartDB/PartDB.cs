using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ForgeTest.PartDB
{
    public static class PartDB
    {
        internal static string PART_DB_FILE_NAME = "PartDB.json";

        private static List<CPart> PART_LIST = null;

        static PartDB()
        {
        }

        public static void SavePart(CPart part)
        {
            CPart tmp;

            tmp = PART_LIST.Find(i => i.PartNumber == part.PartNumber);
            if (tmp != null)
            {
                tmp.ModelURN = part.ModelURN;
                tmp.DrawingURN = part.DrawingURN;
            }
            else
            {
                PART_LIST.Add(part);
            }
        }
        public static CPart GetPart(string partNumber)
        {
            return PART_LIST.Find(i => i.PartNumber.ToUpper() == partNumber.ToUpper());
        }
        public static void SavePartDB()
        {
            string json;

            json = JsonConvert.SerializeObject(PART_LIST);

            System.IO.File.WriteAllText(
                PART_DB_FILE_NAME,
                json);
        }

        public static void LoadPartDB()
        {
            if (System.IO.File.Exists(PART_DB_FILE_NAME))
            {
                string json;

                json = System.IO.File.ReadAllText(PART_DB_FILE_NAME);

                PART_LIST = JsonConvert.DeserializeObject<List<CPart>>(json);
            }
            else
            {
                PART_LIST = new List<CPart>();
            }
        }
    }
}
