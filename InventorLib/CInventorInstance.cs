using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Inventor;

namespace InventorLib
{
    public class CInventorInstance 
    {
        private InventorServer m_InventorServerObject = null;

        public CInventorInstance(InventorServer inventorServer)
        {
            m_InventorServerObject = inventorServer;
        }
        
        public bool DocumentIsOpen(string fname)
        {
            foreach (Document doc in m_InventorServerObject.Documents)
            {
                if (System.IO.Path.GetFileName(doc.FullFileName).ToUpper() == System.IO.Path.GetFileName(fname).ToUpper())
                {
                    return true;
                }
            }

            return false;
        }
        public void CloseAllDocuments()
        {
            m_InventorServerObject.Documents.CloseAll(false);
        }

        public Documents.CPartDocument OpenPartDocument(string fname)
        {
            Document doc;

            if (System.IO.File.Exists(fname) == false)
            {
                throw new ArgumentException(
                    $"'{fname}' does not exist",
                    nameof(fname));
            }

            doc = m_InventorServerObject.Documents.Open(
                fname,
                true);

            if (doc.DocumentType != DocumentTypeEnum.kPartDocumentObject)
            {
                doc.Close(true);

                Marshal.ReleaseComObject(doc);
                doc = null;

                throw new ArgumentException(
                    $"'{fname}' is not an Inventor part document",
                    nameof(fname));
            }

            return new Documents.CPartDocument(
                doc as PartDocument,
                m_InventorServerObject);
        }
        public Documents.CDrawingDocument OpenDrawingDocument(string fname)
        {
            Document doc;

            if (System.IO.File.Exists(fname) == false)
            {
                throw new ArgumentException(
                    $"'{fname}' does not exist",
                    nameof(fname));
            }

            doc = m_InventorServerObject.Documents.Open(
                fname,
                true);

            if (doc.DocumentType != DocumentTypeEnum.kDrawingDocumentObject)
            {
                doc.Close(true);

                Marshal.ReleaseComObject(doc);
                doc = null;

                throw new ArgumentException(
                    $"'{fname}' is not an Inventor drawing document",
                    nameof(fname));
            }

            return new Documents.CDrawingDocument(
                doc as DrawingDocument,
                m_InventorServerObject);
        }
    }
}
