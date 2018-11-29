using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeTest.PartDB
{
    public class CPart
    {
        private string m_PartNumber = "";
        private string m_ModelURN = "";
        private string m_DrawingURN = "";

        public CPart()
        {
        }

        public string PartNumber
        {
            get { return m_PartNumber; }
            set { m_PartNumber = value; }
        }
        public string ModelURN
        {
            get { return m_ModelURN; }
            set { m_ModelURN = value; }
        }
        public string DrawingURN
        {
            get { return m_DrawingURN; }
            set { m_DrawingURN = value; }
        }
    }
}
