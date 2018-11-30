using System;
using System.Collections;

namespace DXFLib
{
    public class CEntityRectangle : CEntity, ICloneable
    {
        #region Private Member Data

        private double m_Width = 0;
        private double m_Height = 0;
        private double m_CenterPoint_Vertical = 0;
        private double m_CenterPoint_Horizontal = 0;
        
        #endregion

        #region Constructors

        public CEntityRectangle()
            : base()
		{
		}

		#endregion

        #region Public Properties

        public double Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }
        public double Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }
        public double CenterPoint_Vertical
        {
            get
            {
                return m_CenterPoint_Vertical;
            }
            set
            {
                m_CenterPoint_Vertical = value;
            }
        }
        public double CenterPoint_Horizontal
        {
            get
            {
                return m_CenterPoint_Horizontal;
            }
            set
            {
                m_CenterPoint_Horizontal = value;
            }
        }

        #endregion
    }
}
