using App_code.Library.SEWII.Xml.Serialization;
using SDS.Busniss.Entities;
using SDSPortal.Library.Base;
using SGGS.Busniss.Entities;
using SSV3_Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using SEWII = App_code.Library.SEWII;
using SEWII_Xml = App_code.Library.SEWII.Xml.Serialization;

/// <summary>
/// Summary description for BooksController
/// </summary>
public class BooksController
{
    PublishersController _publishersController;
    public BooksController()
    {
        //
        // TODO: Add constructor logic here
        //
        _publishersController = new PublishersController();
    }

    /// <summary>
    /// Add a new book instance to that product
    /// </summary>
    /// <param name="title"></param>
    /// <param name="isbn"></param>
    /// <param name="product_mode"></param>
    /// <param name="sPublisherNumber"></param>
    /// <param name="sPublisherName"></param>
    /// <param name="productID"></param>
    /// <param name="subtitle"></param>
    /// <param name="edition"></param>
    /// <param name="copyRightYear"></param>
    /// <param name="desc"></param>
    /// <param name="BindingTypeNumber"></param>
    /// <returns></returns>
    public BookInstance AddBookInstance(string title, string isbn, string product_mode, ref string sPublisherNumber, ref string sPublisherName,
                                                 decimal productID, string subtitle, string edition, string copyRightYear, string desc, string BindingTypeNumber = "")
    {
        SDSPortalApplication app = new SDSPortalApplication();
        BookInstance _bookinstance = new BookInstance();

        string[] sTitle = title.Split('^');
        if (sTitle.Length > 0 && sTitle.Length > 1 && product_mode == "existing")
        {
            _bookinstance.BOOK_TITLE = sTitle[0];
            string[] sBookNumberPublisher = sTitle[1].Split(',');
            if (sBookNumberPublisher.Length > 1)
            {
                sPublisherNumber = sBookNumberPublisher[1]; // GetPublisher(_book.BOOK_NUMBER, "book") ;
                sPublisherName = _publishersController.GetPublisherName(sPublisherNumber);

            }
        }
        else
        {
            _bookinstance.BOOK_TITLE = title;
        }
        _bookinstance.PRODUCT_ID = productID;
        _bookinstance.DATE_ADDED = DateTime.Now;
        _bookinstance.BOOK_SUBTITLE = subtitle;

        _bookinstance.BOOK_EDITION = edition;
        _bookinstance.BOOK_COPYRIGHT_YR = copyRightYear;
        _bookinstance.DESCRIPTION = desc;

        Book _book = AddBook(isbn, sPublisherNumber, sPublisherName, _bookinstance.BOOK_TITLE, BindingTypeNumber);
        _bookinstance.BOOK_ID = _book.BOOK_ID;

        _bookinstance.Add(app.SDSConnection);
        return _bookinstance;
    }

    /// <summary>
    /// Add a new book
    /// </summary>
    /// <param name="isbn"></param>
    /// <param name="sPublisherNumber"></param>
    /// <param name="sPublisherName"></param>
    /// <param name="sBookTitle"></param>
    /// <param name="BindingTypeNumber"></param>
    /// <returns></returns>
    public Book AddBook(string isbn, string sPublisherNumber, string sPublisherName, string sBookTitle, string BindingTypeNumber = "")
    {
        SDSPortalApplication app = new SDSPortalApplication();
        Book _book = new Book();
        _book.DATE_ADDED = DateTime.Now;
        _book.BOOK_IDENTIFICATION_TYPE = "ISBN";
        _book.BOOK_NUMBER = isbn;
        _book.PUBLISHER_NUMBER = sPublisherNumber;
        _book.PUBLISHER_NAME = sPublisherName;
        _book.DATE_ADDED = DateTime.Now;
        _book.BOOK_IDENTIFICATION_TYPE = "ISBN";
        _book.BOOK_TITLE = sBookTitle;
        _book.BINDING_TYPE_NUMBER = BindingTypeNumber;

        using (SDSEntities objSDS = new SDSEntities())
        {
            SDS_BOOK objBook = (from B in objSDS.SDS_BOOK
                                where B.BOOK_NUMBER == _book.BOOK_NUMBER && B.PUBLISHER_NUMBER == _book.PUBLISHER_NUMBER
                                select B).FirstOrDefault();
            if (objBook == null)
            {
                LogWriter.LogWriter.WriteEntry("Publisher Numbers :" + sPublisherNumber);
                _book.Add(app.SDSConnection);
            }
            else
            {
                _book.BOOK_ID = objBook.BOOK_ID;
            }

        }
        return _book;
    }

    /// <summary>
    /// Inser authors to a book instance
    /// </summary>
    /// <param name="bookInstanceID"></param>
    /// <param name="authors"></param>
    public void AddAuthors(decimal bookInstanceID, string authors)
    {
        SDSPortalApplication app = new SDSPortalApplication();
        BookAuthor _author = new BookAuthor();
        _author.DATE_ADDED = DateTime.Now;

        string[] Authors = authors.TrimEnd(',').Split(',');

        using (SDSEntities objSDS = new SDSEntities())
        {

            foreach (String sAuthor in Authors)
            {
                SDS_BOOK_AUTHOR objAuthor = new SDS_BOOK_AUTHOR();
                objAuthor.BOOK_INSTANCE_ID = long.Parse(bookInstanceID.ToString());
                objAuthor.AUTHOR_NAME = sAuthor;
                objAuthor.DATE_ADDED = DateTime.Now;
                objAuthor.DATE_UPDATED = DateTime.Now;
                objSDS.SDS_BOOK_AUTHOR.AddObject(objAuthor);
                objSDS.SaveChanges();
            }
        }

    }

    /// <summary>
    /// Insert a new book cover
    /// </summary>
    /// <param name="bookInstanceID"></param>
    /// <param name="productID"></param>
    /// <param name="sDropFolder"></param>
    /// <param name="sCoverFileName"></param>
    /// <returns></returns>
    public BookCover AddBookCover(decimal bookInstanceID, decimal productID, string sDropFolder, string
      sCoverFileName)
    {
        SDSPortalApplication app = new SDSPortalApplication();
        BookCover _bookcover = new BookCover();
        _bookcover.DATE_ADDED = DateTime.Now;
        _bookcover.BOOK_INSTANCE_ID = bookInstanceID;
        _bookcover.COVER_FILE_PATH = string.Format(sDropFolder + "{0}" + "Product_" + productID.ToString() + "{1}", "\\", "\\Cover");
        _bookcover.COVER_FILE_NAME = sCoverFileName;
        _bookcover.COVER_FILE_TYPE = "PDF";
        _bookcover.COVER_FILE_DATE = DateTime.Now;
        _bookcover.DATE_ADDED = DateTime.Now;

        _bookcover.Add(app.SDSConnection);
        return _bookcover;
    }

    /// <summary>
    /// Insert a new book inner work
    /// </summary>
    /// <param name="bookInstanceID"></param>
    /// <param name="productID"></param>
    /// <param name="sInnerFileName"></param>
    /// <param name="totalPages"></param>
    /// <param name="objSewFilesData"></param>
    /// <param name="sDropFolder"></param>
    /// <returns></returns>
    public BookInnerWorkItem AddBookInnerWork(decimal bookInstanceID, decimal productID, string sInnerFileName, string
        totalPages, SewFilesData objSewFilesData, string sDropFolder)
    {
        SDSPortalApplication app = new SDSPortalApplication();
        BookInnerWorkItem _bookinner = new BookInnerWorkItem();

        //Saving Journal Inner Information///////////////////////////////////////////
        _bookinner.BOOK_INSTANCE_ID = bookInstanceID;
        _bookinner.ITEM_FILE_PATH = string.Format(sDropFolder + "{0}" + "Product_" + productID.ToString() + "{1}", "\\", "\\Inner");
        _bookinner.ITEM_FILE_NAME = sInnerFileName;

        _bookinner.ITEM_FILE_TYPE = "PDF";
        LogWriter.LogWriter.WriteEntry("Inner work pages = " + totalPages);
        Decimal totPages = 0;
        if (Decimal.TryParse(totalPages, out totPages) == false)
        { totPages = 0; }
        _bookinner.INNER_WORK_NUMBER_OF_PAGES = objSewFilesData.TotalTextPages == "" ? totPages : Decimal.Parse(objSewFilesData.TotalTextPages);
        _bookinner.ITEM_FILE_DATE = DateTime.Now;
        _bookinner.DATE_ADDED = DateTime.Now;
        _bookinner.Add(app.SDSConnection);
        return _bookinner;
    }


    /// <summary>
    /// Add Preferences to new Book
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="covertype"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="BindingTypeNumber"></param>
    /// <param name="CoverPaper"></param>
    /// <param name="CoverFinish"></param>
    /// <param name="SpineWidth"></param>
    /// <param name="inner_pagecolor"></param>
    /// <param name="PrintQuality"></param>
    /// <param name="TextPaper"></param>
    /// <param name="pageTrimSize"></param>
    /// <returns></returns>
    public SGGS_BOOK_PREFERENCES AddBookPreference(long bookId
                                                     , string covertype
                                                     , string unitOfMeasure, string BindingTypeNumber, string CoverClothColor
                                                     , string CoverPaper, string CoverFinish, string SpineWidth, string SpineStampLeft, string SpineStampCenter, string SpineStampRight
                                                     , bool inner_pagecolor, string PrintQuality, string TextPaper
                                                     , string pageTrimSize, long publisherId, long profileId, bool dustJacket, string DustJacketFinish)
    {
        decimal MMinInch = Convert.ToDecimal(25.4);
        SDSPortalApplication app = new SDSPortalApplication();
        bool isnew = false;
        using (SGGSEntities objSGGS = new SGGSEntities())
        {
            SGGS_BOOK_PREFERENCES _bookpreference = objSGGS.SGGS_BOOK_PREFERENCES.Where(P => P.BOOK_ID == bookId).FirstOrDefault();

            //Get SLA_NUMBER_OF_PRODUCTION_DAYS by PUBLISHER_ID on SGGS_PUBLISHER_PREFRENCE
            SGGS_PUBLISHER_PREFRENCE _publisherPreference = objSGGS.SGGS_PUBLISHER_PREFRENCE.Where(PP => PP.PUBLISHER_ID == publisherId).FirstOrDefault();
            SGGS_PRODUCTION_PLANT _productionPlan = (from pp in objSGGS.SGGS_PRODUCTION_PLANT
                                                     join ppp in objSGGS.SGGS_PRINT_PLANT_PROFILE on pp.PRODUCTION_PLANT_ID equals ppp.PRODUCTION_PLANT_ID
                                                     where ppp.PRINT_PLANT_PROFILE_ID == profileId
                                                     select pp).FirstOrDefault();

            if (_bookpreference == null)
            {
                _bookpreference = new SGGS_BOOK_PREFERENCES();
                isnew = true;
            }

            CoverPaper = CoverPaper.Equals("-1") ? string.Empty : CoverPaper;
            CoverFinish = CoverFinish.Equals("-1") ? string.Empty : CoverFinish;
           

            _bookpreference.BOOK_ID = bookId;
            _bookpreference.BINDING_TYPE = BindingTypeNumber;
            _bookpreference.PRINT_QUALITY = PrintQuality;
            _bookpreference.COVER_CLOTH_COLOR = CoverClothColor.Equals("-1") ? string.Empty : CoverClothColor;
            _bookpreference.PROFILE_ID = profileId;
            _bookpreference.SPINE_STAMP_LEFT = SpineStampLeft;
            _bookpreference.SPINE_STAMP_CENTER = SpineStampCenter;
            _bookpreference.SPINE_STAMP_RIGHT = SpineStampRight;
            _bookpreference.POD_PAPER_COVER = CoverPaper;
            _bookpreference.COVER_FINISH = CoverFinish;
            _bookpreference.POD_PAPER_INNER_WORK = TextPaper;

            _bookpreference.ID_PAPER_COVER = CoverPaper;
            _bookpreference.ID_PAPER_INNER_WORK = TextPaper;

            _bookpreference.PRODUCTION_PLANT = _productionPlan.PRODUCTION_PLANT_NUMBER;
            _bookpreference.PRODUCTION_SITE = _publisherPreference.PRODUCTION_SITE;
            _bookpreference.DUST_JACKET_FINISH = DustJacketFinish;

            //SPINE_TYPE
            if (BindingTypeNumber.Equals("PB"))
            {
                _bookpreference.SPINE_TYPE = BindingTypeNumber;
            }

            else if (BindingTypeNumber.Equals("CF") || BindingTypeNumber.Equals("CL"))
            {

                _bookpreference.SPINE_TYPE = "SQ";
            }

            //BARCODE_STRATEGY
            _bookpreference.BARCODE_STRATEGY = "XXX";

            //SLA_NUMBER_OF_PRODUCTION_DAYS
            _bookpreference.SLA_NUMBER_OF_PRODUCTION_DAYS = _publisherPreference.SLA_NUMBER_OF_PRODUCTION_DAYS;

            //COVER TYPE
            if (covertype.Equals("soft"))
            {

                _bookpreference.COVER_TYPE = "SC";

            }

            else if (covertype.Equals("hard"))
            {

                _bookpreference.COVER_TYPE = "HC";
            }


            
            if (!string.IsNullOrEmpty(CoverPaper))
            {
                var paperCover = objSGGS.SGGS_PAPER_TYPE.Where(o => o.PAPER_NUMBER == CoverPaper).Select(p => new { PAPER_WEIGHT_CWT = p.PAPER_WEIGHT_CWT, PAPER_WEIGHT_GSM = p.PAPER_WEIGHT_GSM }).FirstOrDefault();

                if (paperCover != null)
                {
                    //insert here the CWT and GSM of cover paper
                    _bookpreference.POD_PAPER_COVER_WEIGHT_GSM = paperCover.PAPER_WEIGHT_GSM;
                    _bookpreference.POD_PAPER_COVER_WEIGHT_CWT = paperCover.PAPER_WEIGHT_CWT;

                    // OP/RP Cover Paper Stock
                    _bookpreference.ID_PAPER_COVER_WEIGHT_GSM = paperCover.PAPER_WEIGHT_GSM;
                    _bookpreference.ID_PAPER_COVER_WEIGHT_CWT = paperCover.PAPER_WEIGHT_CWT;
                }
            }

            if (!string.IsNullOrEmpty(TextPaper))
            {
                var paperText = objSGGS.SGGS_PAPER_TYPE.Where(o => o.PAPER_NUMBER == TextPaper).Select(p => new { PAPER_WEIGHT_CWT = p.PAPER_WEIGHT_CWT, PAPER_WEIGHT_GSM = p.PAPER_WEIGHT_GSM }).FirstOrDefault();

                if (paperText != null)
                {
                    //insert here the CWT and GSM of Text Paper
                    _bookpreference.POD_PAPER_INNER_WORK_WEIGHT_GSM = paperText.PAPER_WEIGHT_GSM;
                    _bookpreference.POD_PAPER_INNER_WORK_WEIGHT_CWT = paperText.PAPER_WEIGHT_CWT;
                    _bookpreference.FINANCIAL_SITE = objSGGS.SGGS_PUBLISHER_FINANCIAL_SITE.Where(p => p.PUBLISHER_ID == publisherId).Select(o => o.FINANCIAL_SITE_NUMBER).FirstOrDefault();

                    // OP/RP Text Paper Stock
                    _bookpreference.ID_PAPER_INNER_WORK_WEIGHT_CWT = paperText.PAPER_WEIGHT_CWT;
                    _bookpreference.ID_PAPER_INNER_WORK_WEIGHT_GSM = paperText.PAPER_WEIGHT_GSM;
                }
            }
            
            _bookpreference.DUST_JACKET = dustJacket;
            _bookpreference.SPINE_WIDTH_TOLERANCE = 3;
            _bookpreference.TRIM_SIZE_HEIGHT_MM_TOLERANCE = 1;
            _bookpreference.TRIM_SIZE_WIDTH_MM_TOLERANCE = 1;

            if (unitOfMeasure.Equals("inch"))
                _bookpreference.UNIT_MEASURE = "in";
            else
                _bookpreference.UNIT_MEASURE = unitOfMeasure;

            if (unitOfMeasure.Equals("mm"))
            {
                string[] trimSizeSplit = pageTrimSize.Split('X');
                _bookpreference.TRIM_FORMAT = trimSizeSplit[0].Trim() + trimSizeSplit[1].Trim();
                _bookpreference.TRIM_FORMAT_WIDTH_MM = Convert.ToDecimal(trimSizeSplit[0].Trim());
                _bookpreference.TRIM_FORMAT_HEIGHT_MM = Convert.ToDecimal(trimSizeSplit[1].Trim());
                _bookpreference.SPINE_TYPE_WIDTH_MM = Convert.ToDecimal(SpineWidth);
                _bookpreference.TRIM_FORMAT_WIDTH_IN = Math.Round(Convert.ToDecimal(trimSizeSplit[0].Trim()) / MMinInch, 4);
                _bookpreference.TRIM_FORMAT_HEIGHT_IN = Math.Round(Convert.ToDecimal(trimSizeSplit[1].Trim()) / MMinInch, 4);
                _bookpreference.SPINE_TYPE_WIDTH_IN = Math.Round(Convert.ToDecimal(SpineWidth) / MMinInch, 4);
            }
            else
            {
                string[] trimSizeSplit = pageTrimSize.Split('X');
                _bookpreference.TRIM_FORMAT_WIDTH_IN = Convert.ToDecimal(trimSizeSplit[0].Trim());
                _bookpreference.TRIM_FORMAT_HEIGHT_IN = Convert.ToDecimal(trimSizeSplit[1].Trim());
                _bookpreference.SPINE_TYPE_WIDTH_IN = Convert.ToDecimal(SpineWidth);
                _bookpreference.TRIM_FORMAT_WIDTH_MM = Math.Round(Convert.ToDecimal(trimSizeSplit[0].Trim()) * MMinInch, 0);
                _bookpreference.TRIM_FORMAT_HEIGHT_MM = Math.Round(Convert.ToDecimal(trimSizeSplit[1].Trim()) * MMinInch, 0);
                _bookpreference.SPINE_TYPE_WIDTH_MM = Math.Round(Convert.ToDecimal(SpineWidth) * MMinInch, 0);
                _bookpreference.TRIM_FORMAT = _bookpreference.TRIM_FORMAT_WIDTH_MM.ToString() + _bookpreference.TRIM_FORMAT_HEIGHT_MM.ToString();
            }

            if (!isnew)
            {
                _bookpreference.DATE_UPDATED = DateTime.Now;
                objSGGS.SaveChanges();
            }
            else
            {
                _bookpreference.DATE_ADDED = DateTime.Now;
                _bookpreference.DATE_UPDATED = DateTime.Now;
                objSGGS.SGGS_BOOK_PREFERENCES.AddObject(_bookpreference);
                objSGGS.SaveChanges();
            }

            return _bookpreference;
        }
    }


    /// <summary>
    /// Add book to the order
    /// </summary>
    /// <param name="title"></param>
    /// <param name="authors"></param>
    /// <param name="isbn"></param>
    /// <param name="sPublisherNumber"></param>
    /// <param name="productID"></param>
    /// <param name="edition"></param>
    /// <param name="bindingType"></param>
    /// <param name="copyRightYear"></param>
    public void AddOrderItem(string title, string authors, string isbn, string sPublisherNumber, decimal productID, string edition, string bindingType, string copyRightYear)
    {
        HttpContext.Current.Session[SDSPortalConstants.SESSION_PRODUCT_ID] = productID;
        DataSet ds = HttpContext.Current.Session[SDSPortalConstants.SESSION_BOOKDATASET] != null ? ((DataSet)HttpContext.Current.Session[SDSPortalConstants.SESSION_BOOKDATASET]) : new DataSet();
        DataTable dtBook = null;
        if (ds.Tables.Contains(SDSPortalConstants.TABLE_ID_BOOK) == false)
        {
            string[] addBookHeader = { "PRODUCT_NUMBER", "BOOK_TITLE", "AUTHOR_NAME", "BOOK_NUMBER", "BOOK_IDENTIFICATION_TYPE", "BOOK_EDITION", "BINDING_TYPE", "BOOK_COPYRIGHT_YR", "PUBLISHER_NUMBER" };
            dtBook = new DataTable();
            dtBook.TableName = SDSPortalConstants.TABLE_ID_BOOK;
            for (int i = 0; i < addBookHeader.Length; i++)
            {
                dtBook.Columns.Add(addBookHeader[i], typeof(string));
            }
            ds.Tables.Add(dtBook);
        }
        else
        {
            dtBook = ds.Tables[SDSPortalConstants.TABLE_ID_BOOK];// (DataTable)HttpContext.Current.Session[SDSPortalConstants.SESSION_CSVITEMS];
        }
        DataRow dr = dtBook.NewRow();

        string bindingTypeName = SDS_Helper.GetBindingTypeName(bindingType);
        string UserFriendlyAuthors = SDS_Helper.GetUserFriendlyAuthorNames(authors);

        dr["PRODUCT_NUMBER"] = productID.ToString(); ;
        dr["BOOK_TITLE"] = title;
        dr["AUTHOR_NAME"] = UserFriendlyAuthors;
        dr["BOOK_NUMBER"] = isbn;
        dr["BOOK_IDENTIFICATION_TYPE"] = "ISBN";
        dr["BOOK_EDITION"] = edition;
        dr["BINDING_TYPE"] = bindingTypeName;
        dr["BOOK_COPYRIGHT_YR"] = copyRightYear;
        dr["PUBLISHER_NUMBER"] = sPublisherNumber;

        dtBook.Rows.Add(dr);

        if (ds.Tables.Contains(SDSPortalConstants.TABLE_ID_BOOK) == false) ds.Tables.Add(dtBook);
        HttpContext.Current.Session[SDSPortalConstants.SESSION_BOOKDATASET] = ds;
    }

    /// <summary>
    /// Inserts or update a new book into sggs
    /// </summary>
    /// <param name="publisherNumber"></param>
    /// <param name="title"></param>
    /// <param name="subtitle"></param>
    /// <param name="isbn"></param>
    /// <param name="edition"></param>
    /// <param name="copyRightYear"></param>
    /// <returns></returns>
    public SGGS_BOOK InsertUpdateBook(string publisherNumber, string title, string subtitle, string isbn, string edition, string copyRightYear)
    {
        using (SGGSEntities objSGGS = new SGGSEntities())
        {
            SGGS_BOOK sggsBook = new SGGS_BOOK();
            SGGS_PUBLISHER sggsPublisher = new SGGS_PUBLISHER();
            var publisher = objSGGS.SGGS_PUBLISHER.Where(P => P.PUBLISHER_NUMBER == publisherNumber).FirstOrDefault();                                ///////
            if (publisher != null)
            {
                sggsBook = (from B in objSGGS.SGGS_BOOK
                            where B.BOOK_NUMBER == isbn && B.PUBLISHER_ID == publisher.PUBLISHER_ID
                            select B).SingleOrDefault();
                if (sggsBook == null)
                {
                    sggsBook = new SGGS_BOOK();
                    sggsBook.BOOK_TITLE = title;
                    sggsBook.DATE_UPDATED = System.DateTime.Now;
                    sggsBook.PUBLISHER_ID = publisher.PUBLISHER_ID;
                    sggsBook.BOOK_IDENTIFICATION_TYPE = "ISBN";
                    sggsBook.BOOK_NUMBER = isbn;
                    sggsBook.IS_ENABLED = true;
                    sggsBook.IS_NEW = true;
                    sggsBook.DATE_ADDED = System.DateTime.Now;
                    objSGGS.SGGS_BOOK.AddObject(sggsBook);
                    objSGGS.SaveChanges();
                   
                }
                else if (sggsBook != null)
                {
                    sggsBook.BOOK_TITLE = title;
                    sggsBook.DATE_UPDATED = System.DateTime.Now;
                    objSGGS.SaveChanges();
                }
            }
            return sggsBook;
        }
    }

    /// <summary>
    /// Create the non Sew File
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="productPath"></param>
    /// <param name="sTransactionNumber"></param>
    /// <param name="preferences"></param>
    public XmlDocument CreateNONSEWSixFile<T>(T preferences, string xml, string BookNumber, string totalpages, bool isSew)
    {
        try
        {           
            var xDoc = new XmlDocument();
            byte[] encodedString = System.Text.Encoding.UTF8.GetBytes(xml);

            // Put the byte array into a stream and rewind it to the beginning
            System.IO.MemoryStream ms = new System.IO.MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;
            xDoc.Load(ms);

            LogWriter.LogWriter.WriteEntry("Creating BookPreferences XML");


            XmlNode book = xDoc.GetElementsByTagName("BOOK").Item(0);
            XmlNode authors = xDoc.GetElementsByTagName("BOOK_AUTHORS").Item(0);
            XmlNode totalpagestag = xDoc.GetElementsByTagName("TOTAL_PAGES").Item(0);
            XmlNode itemtype = xDoc.GetElementsByTagName("ITEM_TYPE").Item(0);


            if (book != null)
            {
                XmlElement bookpreferences = xDoc.CreateElement("BOOK_PREFERENCES");
                if (authors == null)
                    book.AppendChild(bookpreferences);
                else
                    book.InsertAfter(bookpreferences, authors);

                if (totalpagestag != null)
                    totalpagestag.InnerText = totalpages;

                if (isSew)
                {
                    if (itemtype != null)
                        itemtype.InnerText = "FULL";

                    foreach (XmlNode secuence in xDoc.GetElementsByTagName("SEQUENCE_NUMBER"))
                    {
                        secuence.InnerText = "1";
                    }
                }

                if (preferences.GetType() == typeof(SGGS_BOOK_PREFERENCES))
                {
                    var pref = preferences as SGGS_BOOK_PREFERENCES;
                    var BINDING_TYPE = xDoc.CreateElement("BINDING_TYPE");
                    BINDING_TYPE.InnerText = pref.BINDING_TYPE;
                    bookpreferences.AppendChild(BINDING_TYPE);

                    var SPINE_TYPE = xDoc.CreateElement("SPINE_TYPE");
                    SPINE_TYPE.InnerText = pref.SPINE_TYPE;
                    bookpreferences.AppendChild(SPINE_TYPE);

                    var SPINE_TYPE_WIDTH_MM = xDoc.CreateElement("SPINE_TYPE_WIDTH_MM");
                    SPINE_TYPE_WIDTH_MM.InnerText = Convert.ToString(pref.SPINE_TYPE_WIDTH_MM);
                    bookpreferences.AppendChild(SPINE_TYPE_WIDTH_MM);

                    var PRINT_QUALITY = xDoc.CreateElement("PRINT_QUALITY");
                    PRINT_QUALITY.InnerText = pref.PRINT_QUALITY;
                    bookpreferences.AppendChild(PRINT_QUALITY);

                    var COVER_FINISH = xDoc.CreateElement("COVER_FINISH");
                    COVER_FINISH.InnerText = pref.COVER_FINISH;
                    bookpreferences.AppendChild(COVER_FINISH);

                    var SLA_NUMBER_OF_PRODUCTION_DAYS = xDoc.CreateElement("SLA_NUMBER_OF_PRODUCTION_DAYS");
                    SLA_NUMBER_OF_PRODUCTION_DAYS.InnerText = Convert.ToString(pref.SLA_NUMBER_OF_PRODUCTION_DAYS);
                    bookpreferences.AppendChild(SLA_NUMBER_OF_PRODUCTION_DAYS);

                    var BARCODE_STRATEGY = xDoc.CreateElement("BARCODE_STRATEGY");
                    BARCODE_STRATEGY.InnerText = pref.BARCODE_STRATEGY;
                    bookpreferences.AppendChild(BARCODE_STRATEGY);

                    var ID_PAPER_INNER_WORK = xDoc.CreateElement("ID_PAPER_INNER_WORK");
                    ID_PAPER_INNER_WORK.InnerText = pref.ID_PAPER_INNER_WORK;
                    bookpreferences.AppendChild(ID_PAPER_INNER_WORK);

                    var ID_PAPER_COVER = xDoc.CreateElement("ID_PAPER_COVER");
                    ID_PAPER_COVER.InnerText = pref.ID_PAPER_COVER;
                    bookpreferences.AppendChild(ID_PAPER_COVER);

                    var POD_PAPER_INNER_WORK = xDoc.CreateElement("POD_PAPER_INNER_WORK");
                    POD_PAPER_INNER_WORK.InnerText = pref.POD_PAPER_INNER_WORK;
                    bookpreferences.AppendChild(POD_PAPER_INNER_WORK);

                    var POD_PAPER_COVER = xDoc.CreateElement("POD_PAPER_COVER");
                    POD_PAPER_COVER.InnerText = pref.POD_PAPER_COVER;
                    bookpreferences.AppendChild(POD_PAPER_COVER);

                    var TRIM_FORMAT = xDoc.CreateElement("TRIM_FORMAT");
                    TRIM_FORMAT.InnerText = pref.TRIM_FORMAT;
                    bookpreferences.AppendChild(TRIM_FORMAT);

                    var TRIM_FORMAT_WIDTH_MM = xDoc.CreateElement("TRIM_FORMAT_WIDTH_MM");
                    TRIM_FORMAT_WIDTH_MM.InnerText = Convert.ToString(pref.TRIM_FORMAT_WIDTH_MM);
                    bookpreferences.AppendChild(TRIM_FORMAT_WIDTH_MM);

                    var TRIM_FORMAT_HEIGHT_MM = xDoc.CreateElement("TRIM_FORMAT_HEIGHT_MM");
                    TRIM_FORMAT_HEIGHT_MM.InnerText = Convert.ToString(pref.TRIM_FORMAT_HEIGHT_MM);
                    bookpreferences.AppendChild(TRIM_FORMAT_HEIGHT_MM);

                    var TRIM_SIZE_HEIGHT_MM_TOLERANCE = xDoc.CreateElement("TRIM_SIZE_HEIGHT_MM_TOLERANCE");
                    TRIM_SIZE_HEIGHT_MM_TOLERANCE.InnerText = Convert.ToString(pref.TRIM_SIZE_HEIGHT_MM_TOLERANCE);
                    bookpreferences.AppendChild(TRIM_SIZE_HEIGHT_MM_TOLERANCE);

                    var TRIM_SIZE_WIDTH_MM_TOLERANCE = xDoc.CreateElement("TRIM_SIZE_WIDTH_MM_TOLERANCE");
                    TRIM_SIZE_WIDTH_MM_TOLERANCE.InnerText = Convert.ToString(pref.TRIM_SIZE_WIDTH_MM_TOLERANCE);
                    bookpreferences.AppendChild(TRIM_SIZE_WIDTH_MM_TOLERANCE);

                    var SPINE_WIDTH_TOLERANCE = xDoc.CreateElement("SPINE_WIDTH_TOLERANCE");
                    SPINE_WIDTH_TOLERANCE.InnerText = Convert.ToString(pref.SPINE_WIDTH_TOLERANCE);
                    bookpreferences.AppendChild(SPINE_WIDTH_TOLERANCE);
                }
                else if (preferences.GetType() == typeof(SEWII.Book.Product))        
                {
                    var pref = preferences as SEWII.Book.Product;

                    var node = xDoc.CreateElement("BINDING_TYPE");
                    node.InnerText = pref.binding_type;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("SPINE_TYPE");
                    node.InnerText = GetSpineType(pref.binding_type);
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("SPINE_TYPE_WIDTH_MM");
                    node.InnerText = (pref.spine_type_width_mm == null) ? string.Empty : pref.spine_type_width_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("SPINE_TYPE_WIDTH_IN");
                    //node.InnerText = (pref.spine_type_width_mm == null) ? string.Empty : pref.spine_type_width_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("PRINT_QUALITY");
                    node.InnerText = pref.print_quality;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("COVER_FINISH");
                    node.InnerText = pref.cover_finish;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("COVER_CLOTH");
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("BARCODE_STRATEGY");
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("SLA_NUMBER_OF_PRODUCTION_DAYS");
                    node.InnerText = (pref.sla_number_of_production_days == null) ? string.Empty : pref.sla_number_of_production_days.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("ID_PAPER_INNER_WORK");
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("ID_PAPER_COVER");
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("POD_PAPER_INNER_WORK");
                    node.InnerText = pref.pod.paper_inner_work;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("POD_PAPER_COVER");
                    node.InnerText = pref.pod.paper_cover;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_FORMAT");
                    node.InnerText = pref.trim_format;
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_FORMAT_WIDTH_MM");
                    node.InnerText = (pref.trim_format_width_mm == null) ? "0" : pref.trim_format_width_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_FORMAT_HEIGHT_MM");
                    node.InnerText = (pref.trim_format_height_mm == null) ? "0" : pref.trim_format_height_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_FORMAT_WIDTH_IN");
                    node.InnerText = (pref.trim_format_width_mm == null) ? "0" : pref.trim_format_width_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_FORMAT_HEIGHT_IN");
                    node.InnerText = (pref.trim_format_height_mm == null) ? "0" : pref.trim_format_height_mm.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_SIZE_HEIGHT_MM_TOLERANCE");
                    node.InnerText = (pref.trim_size_height_mm_tolerance == null) ? "0" : pref.trim_size_height_mm_tolerance.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("TRIM_SIZE_WIDTH_MM_TOLERANCE");
                    node.InnerText = (pref.trim_size_width_mm_tolerance == null) ? "0" : pref.trim_size_width_mm_tolerance.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("SPINE_WIDTH_TOLERANCE");
                    node.InnerText = (pref.spine_width_mm_tolerance == null) ? "0" : pref.spine_width_mm_tolerance.ToString();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("HAS_STAMP");
                    //node.InnerText = (string.IsNullOrEmpty(preferences.SPINE_STAMP_CENTER) && string.IsNullOrEmpty(preferences.SPINE_STAMP_LEFT) && string.IsNullOrEmpty(preferences.SPINE_STAMP_RIGHT)) ? "false" : "true";
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("DUST_JACKET");
                    //node.InnerText = (preferences.DUST_JACKET == null) ? "false" : preferences.DUST_JACKET.ToString().ToLower();
                    bookpreferences.AppendChild(node);

                    node = xDoc.CreateElement("DUST_JACKET_FINISH");
                    //node.InnerText = (preferences.DUST_JACKET_FINISH == null) ? null : preferences.DUST_JACKET_FINISH;
                    bookpreferences.AppendChild(node);
                }
                else
                {
                    var message = xDoc.CreateElement("MESSAGE");
                    message.InnerText = "This book doesn't have preferences ISBN: " + BookNumber.ToString();
                    bookpreferences.AppendChild(message);
                }

                LogWriter.LogWriter.WriteEntry("Book preferences have been added successfully to the SIX xml");
                return xDoc;
            }
            return null;
        }
        catch (Exception ex)
        {
            LogWriter.LogWriter.WriteEntry(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Get Spine Type by binding type
    /// </summary>
    /// <param name="bindType"></param>
    /// <returns></returns>
    private string GetSpineType(string bindType)
    {
        if (bindType.ToUpper().Equals("PB"))
            return bindType;
        else if (bindType.ToUpper().Equals("CF") || bindType.ToUpper().Equals("CL"))
            return "SQ";

        return string.Empty;
    }

    /// <summary>
    /// Alter a SIX file
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="productId"></param>
    /// <param name="hasColor"></param>
    /// <param name="totalPages"></param>
    /// <returns></returns>
    public string AlterSIX(string xml, decimal productId, string hasColor, string totalPages)
    {
        using (var context = new SDSEntities())
        {
            try
            {
                var document = new XmlDocument();
                int counter = 0;
                byte[] encodedString = Encoding.UTF8.GetBytes(xml);

                // Put the byte array into a stream and rewind it to the beginning
                var ms = new MemoryStream(encodedString);
                ms.Flush();
                ms.Position = 0;
                document.Load(ms);

                var files = context.SDS_PRODUCT_FILE.Where(x => x.PRODUCT_ID == productId).ToList();

                var tempList = files.Where(x => x.FILE_PATH.ToLower().Contains("text")).ToList();
                var node = document.SelectSingleNode("//SIX/PRODUCT/BOOK/BOOK_INNER_WORK/BOOK_INNER_WORK_ITEM");
                node.ParentNode.RemoveAll();

                var innerNode = document.SelectSingleNode("//SIX/PRODUCT/BOOK/BOOK_INNER_WORK");
                foreach (var item in tempList)
                {
                    counter++;
                    var innerItem = document.CreateElement("BOOK_INNER_WORK_ITEM");
                    innerNode.AppendChild(innerItem);

                    node = document.CreateElement("SEQUENCE_NUMBER");
                    node.InnerText = counter.ToString();
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_TYPE");
                    node.InnerText = Path.GetExtension(Path.Combine(item.FILE_PATH + "\\", item.FILE_NAME)).Replace(".", "").ToUpper();
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_FILE_NAME");
                    node.InnerText = item.FILE_NAME;
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_PAGE_FIRST");
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_PAGE_LAST");
                    innerItem.AppendChild(node);

                    node = document.CreateElement("INNER_PAGE_COLOR");
                    node.InnerText = hasColor.ToLower();
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_NUMBER_OF_PAGES");
                    node.InnerText = totalPages;
                    innerItem.AppendChild(node);

                    node = document.CreateElement("ITEM_NUMBER_OF_COLOR_PAGES");
                    node.InnerText = "0";
                    innerItem.AppendChild(node);
                }

                counter = 0;
                tempList = files.Where(x => x.FILE_PATH.ToLower().Contains("cover")).ToList();

                if (tempList.Any())
                {
                    node = document.SelectSingleNode("//SIX/PRODUCT/BOOK/BOOK_COVER/BOOK_COVER_ITEM");
                    node.ParentNode.RemoveAll();

                    var coverNode = document.SelectSingleNode("//SIX/PRODUCT/BOOK/BOOK_COVER");
                    foreach (var item in tempList)
                    {
                        counter++;
                        var coverItem = document.CreateElement("BOOK_COVER_ITEM");
                        coverNode.AppendChild(coverItem);

                        node = document.CreateElement("SEQUENCE_NUMBER");
                        node.InnerText = counter.ToString();
                        coverItem.AppendChild(node);

                        node = document.CreateElement("ITEM_FILE_NAME");
                        node.InnerText = item.FILE_NAME;
                        coverItem.AppendChild(node);

                        node = document.CreateElement("ITEM_NUMBER_OF_PAGES");
                        coverItem.AppendChild(node);

                        node = document.CreateElement("ITEM_NUMBER_OF_COLOR_PAGES");
                        node.InnerText = "0";
                        coverItem.AppendChild(node);

                        node = document.CreateElement("OUTER_ITEM_NUMBER_OF_COLORS");
                        node.InnerText = "0";
                        coverItem.AppendChild(node);

                        node = document.CreateElement("INNER_ITEM_NUMBER_OF_COLORS");
                        node.InnerText = "0";
                        coverItem.AppendChild(node);
                    }
                }

                return document.InnerXml;
            }
            catch (Exception ex)
            {
                LogWriter.LogWriter.WriteEntry("Six file could not be altered");
                LogWriter.LogWriter.WriteEntry(ex.Message);
                return xml;
            }           
        }
    }

    /// <summary>
    /// Create a new SEWII Six file
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publisher"></param>
    /// <param name="files"></param>
    /// <param name="transactionNumber"></param>
    /// <returns></returns>
    public string CreateSEWIISix(BookParameters data, SEWII.Book.Publisher publisher, List<NonSewFilesData> files, string transactionNumber)
    {
        try
        {
            var six = new SEWII_Xml.SIX();
            var publisherXml = new SEWII_Xml.Publisher();
            var product = new SEWII_Xml.Product();
            var book = new SEWII_Xml.Book();
            var bookId = new SEWII_Xml.BookId();

            six.TransactionNumber = transactionNumber;
            six.Publisher = publisherXml;
            six.Product = product;

            publisherXml.Number = publisher.publisher_id;
            publisherXml.Name = publisher.publisher_name;
            publisherXml.Acronym = publisher.publisher_acronym;

            product.type = "BOOK";
            product.Book = book;
            product.SDEComplaint = "true";

            book.Title = data.title;
            book.SubTitle = data.subtitle;
            book.Id = bookId;
            book.Edition = data.edition;
            book.CopyrightYear = data.copyRightYear;
            book.TotalPages = data.totalPages;
            book.Description = data.desc;

            bookId.Type = "ISBN";
            bookId.Value = data.isbn;

            var authorList = data.authors.Split(',').Where(x => x.Trim() != string.Empty).ToArray();
            book.Autors = authorList;

            var text = files.Where(x => x.TypeFilePath.ToLower().Equals("text")).ToList();
            if (text.Any())
            {
                int index = text.Count;
                var array = new SEWII_Xml.InnerWorkItem[index];
                for (int i = 0; i < index; i++)
                {
                    var option = new FileInfo(text[i].FilePath);
                    var item = new SEWII_Xml.InnerWorkItem();
                    item.Sequence = ((i) + 1).ToString();
                    item.Type = option.Extension.Replace(".", "").ToUpper();
                    item.FileName = option.Name;
                    item.PageFirst = string.Empty;
                    item.PageLast = string.Empty;
                    item.NumberOfPages = data.totalPages;
                    item.NumberOfColorPages = "0";
                    array[i] = item;
                }

                book.InnerWork = array;
            }

            var cover = files.Where(x => x.TypeFilePath.ToLower().Equals("cover")).ToList();
            if (cover.Any())
            {
                int index = cover.Count;
                var array = new SEWII_Xml.CoverItem[index];
                for (int i = 0; i < index; i++)
                {
                    var option = new FileInfo(cover[i].FilePath);
                    var item = new SEWII_Xml.CoverItem();
                    item.Sequence = ((i) + 1).ToString();
                    item.FileName = option.Name;
                    item.NumberOfPages = string.Empty;
                    item.InnerNumberOfPages = "0";
                    item.OuterNumberOfPages = "0";
                    item.NumberOfColorPages = "0";
                    array[i] = item;
                }

                book.Cover = array;
            }

            var dustJacket = files.Where(x => x.TypeFilePath.ToLower().Equals("dust_jacket")).ToList();
            if (dustJacket.Any())
            {
                int index = dustJacket.Count;
                var array = new SEWII_Xml.DustJacketItem[index];
                for (int i = 0; i < index; i++)
                {
                    var option = new FileInfo(dustJacket[i].FilePath);
                    var item = new SEWII_Xml.DustJacketItem();
                    item.Sequence = ((i) + 1).ToString();
                    item.FileName = option.Name;
                    item.NumberOfPages = data.totalPages;
                    item.InnerNumberOfPages = "0";
                    item.OuterNumberOfPages = "0";
                    item.NumberOfColorPages = "0";
                    array[i] = item;
                }

                book.DustJacket = array;
            }

            return six.Serialize();
        }
        catch (Exception ex)
        {
            LogWriter.LogWriter.WriteEntry("Error when trying to create SEWII SIX file");
            LogWriter.LogWriter.WriteEntry(ex.Message);
            return null;
        }
    }
}