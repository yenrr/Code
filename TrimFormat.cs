using Common.Utils;
using SGGS.Busniss.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SGGSManagement.Library.Administration
{
    public class TrimFormat : ICloneable
    {
        public TrimFormat()
        {
            DATE_ADDED = DateTime.Now;
            DATE_UPDATED = DateTime.Now;
            TRIM_FORMAT_HEIGHT_IN = 0;
            TRIM_FORMAT_HEIGHT_MM = 0;
            TRIM_FORMAT_NAME = string.Empty;
            TRIM_FORMAT_NUMBER = string.Empty;
            TRIM_FORMAT_WIDTH_IN = 0;
            TRIM_FORMAT_WIDTH_MM = 0;
            USAGE_HARDCOVER = false;
            USAGE_PAPERBACK = false;
        }

        public bool SOFT_COVER { get; set; }
        public bool HARD_COVER { get; set; }
        public DateTime DATE_ADDED { get; set; }
        public DateTime DATE_UPDATED { get; set; }
        public decimal TRIM_FORMAT_HEIGHT_IN { get; set; }
        public decimal TRIM_FORMAT_HEIGHT_MM { get; set; }
        public long? TRIM_FORMAT_ID { get; set; }
        public string TRIM_FORMAT_NAME { get; set; }
        public string TRIM_FORMAT_NUMBER { get; set; }
        public decimal TRIM_FORMAT_WIDTH_IN { get; set; }
        public decimal TRIM_FORMAT_WIDTH_MM { get; set; }
        public bool USAGE_HARDCOVER { get; set; }
        public bool USAGE_PAPERBACK { get; set; }

        //Print plant Profiles
        public long PRINT_PLANT_PROFILE_ID { get; set; }
        public long SUPPORTED_PAPER_TYPE { get; set; }
        public long SUPPORTED_BINDING_TYPE { get; set; }
        public long PAPER_TYPE_ID { get; set; }
        public long BINDING_TYPE_ID { get; set; }
        public string SUPPORTED_PRINT_QUALITY { get; set; }
        public long? SUPPORTED_PRINT_QUALITY_ID { get; set; }
        public int? MIN_PAGES { get; set; }
        public int? MAX_PAGES { get; set; }
        public decimal? TRIM_FORMAT_MIN_WIDTH { get; set; }
        public decimal? TRIM_FORMAT_MAX_WIDTH { get; set; }
        public decimal? TRIM_FORMAT_MIN_HEIGHT { get; set; }
        public decimal? TRIM_FORMAT_MAX_HEIGHT { get; set; }
        public bool OVERRIDE_AUTO_CALCULATION { get; set; }
        private IList<SGGS_SUPPORTED_TRIM_FORMATS> EXCLUDED_PAPER_TYPES;

        /// <summary>
        /// Method to insert record
        /// </summary>
        /// <returns></returns>
        public bool Add()
        {
            using (var context = new SGGSEntities())
            {
                var trim = new SGGS_CODE_TRIM_FORMAT
                {
                    DATE_ADDED = DateTime.Now,
                    DATE_UPDATED = DateTime.Now,
                    TRIM_FORMAT_HEIGHT_IN = TRIM_FORMAT_HEIGHT_IN,
                    TRIM_FORMAT_HEIGHT_MM = TRIM_FORMAT_HEIGHT_MM,
                    TRIM_FORMAT_NAME = TRIM_FORMAT_NAME,
                    TRIM_FORMAT_NUMBER = TRIM_FORMAT_NUMBER,
                    TRIM_FORMAT_WIDTH_IN = TRIM_FORMAT_WIDTH_IN,
                    TRIM_FORMAT_WIDTH_MM = TRIM_FORMAT_WIDTH_MM,
                    USAGE_HARDCOVER = USAGE_HARDCOVER,
                    USAGE_PAPERBACK = USAGE_PAPERBACK,
                    SOFT_COVER = SOFT_COVER,
                    HARD_COVER = HARD_COVER
                };

                context.SGGS_CODE_TRIM_FORMAT.AddObject(trim);
                context.SaveChanges();
                TRIM_FORMAT_ID = trim.TRIM_FORMAT_ID;
                return true;
            }
        }

        /// <summary>
        /// Method to insert record
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            using (var context = new SGGSEntities())
            {
                var trim = context.SGGS_CODE_TRIM_FORMAT.FirstOrDefault(x => x.TRIM_FORMAT_ID == TRIM_FORMAT_ID);
                trim.DATE_UPDATED = DateTime.Now;
                trim.TRIM_FORMAT_HEIGHT_IN = TRIM_FORMAT_HEIGHT_IN;
                trim.TRIM_FORMAT_HEIGHT_MM = TRIM_FORMAT_HEIGHT_MM;
                trim.TRIM_FORMAT_NAME = TRIM_FORMAT_NAME;
                trim.TRIM_FORMAT_NUMBER = TRIM_FORMAT_NUMBER;
                trim.TRIM_FORMAT_WIDTH_IN = TRIM_FORMAT_WIDTH_IN;
                trim.TRIM_FORMAT_WIDTH_MM = TRIM_FORMAT_WIDTH_MM;
                trim.USAGE_HARDCOVER = USAGE_HARDCOVER;
                trim.USAGE_PAPERBACK = USAGE_PAPERBACK;
                trim.SOFT_COVER = SOFT_COVER;
                trim.HARD_COVER = HARD_COVER;
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Method to Load values from DB
        /// </summary>
        /// <param name="TRIM_FORMAT_ID"></param>
        /// <returns></returns>
        public bool Load(long TRIM_FORMAT_ID)
        {
            using (var context = new SGGSEntities())
            {
                var result = context.SGGS_CODE_TRIM_FORMAT.Where(x => x.TRIM_FORMAT_ID == TRIM_FORMAT_ID)
                    .Select(s => new
                    {
                        s.DATE_ADDED,
                        s.DATE_UPDATED,
                        s.TRIM_FORMAT_HEIGHT_IN,
                        s.TRIM_FORMAT_HEIGHT_MM,
                        s.TRIM_FORMAT_ID,
                        s.TRIM_FORMAT_NAME,
                        s.TRIM_FORMAT_NUMBER,
                        s.TRIM_FORMAT_WIDTH_IN,
                        s.TRIM_FORMAT_WIDTH_MM,
                        s.USAGE_HARDCOVER,
                        s.USAGE_PAPERBACK,
                        s.HARD_COVER,
                        s.SOFT_COVER
                    }).FirstOrDefault();

                if (result != null)
                {
                    this.DATE_ADDED = result.DATE_ADDED;
                    this.DATE_UPDATED = result.DATE_UPDATED.Value;
                    this.TRIM_FORMAT_HEIGHT_IN = result.TRIM_FORMAT_HEIGHT_IN;
                    this.TRIM_FORMAT_HEIGHT_MM = result.TRIM_FORMAT_HEIGHT_MM;
                    this.TRIM_FORMAT_ID = result.TRIM_FORMAT_ID;
                    this.TRIM_FORMAT_NAME = result.TRIM_FORMAT_NAME;
                    this.TRIM_FORMAT_NUMBER = result.TRIM_FORMAT_NUMBER;
                    this.TRIM_FORMAT_WIDTH_IN = result.TRIM_FORMAT_WIDTH_IN;
                    this.TRIM_FORMAT_WIDTH_MM = result.TRIM_FORMAT_WIDTH_MM;
                    this.USAGE_HARDCOVER = result.USAGE_HARDCOVER;
                    this.USAGE_PAPERBACK = result.USAGE_PAPERBACK;
                    this.HARD_COVER = result.HARD_COVER ?? false;
                    this.SOFT_COVER = result.SOFT_COVER ?? false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Method to Delete Record
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            using (var context = new SGGSEntities())
            {
                var trim = context.SGGS_CODE_TRIM_FORMAT.FirstOrDefault(x => x.TRIM_FORMAT_ID == TRIM_FORMAT_ID);
                context.SGGS_CODE_TRIM_FORMAT.DeleteObject(trim);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Method to Delete Record by SuptrimformatID
        /// </summary>
        /// <param name="suppTrimFormatID"></param>
        /// <returns></returns>
        public bool DeleteSupportTrimFormat(long suppTrimFormatID)
        {
            using (var context = new SGGSEntities())
            {
                var suppTrim = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(x => x.SUPPORTED_TRIM_FORMATS_ID == suppTrimFormatID);
                context.SGGS_SUPPORTED_TRIM_FORMATS.DeleteObject(suppTrim);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Method to Delete Record by SuptrimformatID
        /// </summary>
        /// <returns></returns>
        public DataTable GetTrimFormat()
        {
            using (var context = new SGGSEntities())
            {
                var result = context.SGGS_CODE_TRIM_FORMAT
                    .Select(s => new
                    {
                        s.TRIM_FORMAT_ID,
                        s.TRIM_FORMAT_NAME,
                        s.TRIM_FORMAT_NUMBER,
                        s.TRIM_FORMAT_WIDTH_MM,
                        s.TRIM_FORMAT_WIDTH_IN,
                        s.TRIM_FORMAT_HEIGHT_MM,
                        s.TRIM_FORMAT_HEIGHT_IN,
                        USAGE_PAPERBACK = s.USAGE_PAPERBACK == true ? "Yes" : "No",
                        USAGE_HARDCOVER = s.USAGE_HARDCOVER == true ? "Yes" : "No"
                    }).ToList();

                return CommonUtils.ToDataTable(result, "TrimFormat");
            }
        }

        /// <summary>
        /// Get Trim format by printPlantProfileId
        /// </summary>
        /// <param name="printPlantProfileId"></param>
        /// <returns></returns>
        public DataTable GetTrimFormat(long printPlantProfileId)
        {
            using (var context = new SGGSEntities())
            {
                var profile = (from ppp in context.SGGS_PRINT_PLANT_PROFILE
                               where ppp.PRINT_PLANT_PROFILE_ID == printPlantProfileId
                               select ppp).FirstOrDefault();
                if (profile != null)
                {
                    if (profile.COVER_TYPE.ToUpper().Equals("S"))
                    {
                        var result = context.SGGS_CODE_TRIM_FORMAT
                            .Where(x => x.SOFT_COVER == true)
                        .Select(s => new
                        {
                            s.TRIM_FORMAT_ID,
                            TRIM_FORMAT_NAME = s.TRIM_FORMAT_NAME + " (" + ((double)s.TRIM_FORMAT_WIDTH_IN).ToString() + " x " + ((double)s.TRIM_FORMAT_HEIGHT_IN).ToString() + ")"
                        }).OrderBy(tf => tf.TRIM_FORMAT_NAME).ToList();

                        return CommonUtils.ToDataTable(result, "TrimFormat");
                    }

                    if (profile.COVER_TYPE.ToUpper().Equals("H"))
                    {
                        var result = context.SGGS_CODE_TRIM_FORMAT
                            .Where(x => x.HARD_COVER == true)
                            .Select(s => new
                            {
                                s.TRIM_FORMAT_ID,
                                TRIM_FORMAT_NAME = s.TRIM_FORMAT_NAME + " (" + ((double)s.TRIM_FORMAT_WIDTH_IN).ToString() + " x " + ((double)s.TRIM_FORMAT_HEIGHT_IN).ToString() + ")"
                            }).OrderBy(tf => tf.TRIM_FORMAT_NAME).ToList();

                        return CommonUtils.ToDataTable(result, "TrimFormat");
                    }
                    return null;
                }

                return null;
            }
        }

        /// <summary>
        /// Method that adds new trim format to the print plant profile
        /// </summary>
        /// <param name="formats">New trim formats added</param>
        /// <returns></returns>
        public bool AddSupportedTrimFormats(IList<TrimFormat> formats, bool supportRanges)
        {
            if (formats.Count <= 0) return false;

            using (var context = new SGGSEntities())
            {
                foreach (var item in formats)
                {
                    var sggsCodePrintQuality = context.SGGS_CODE_PRINT_QUALITY.FirstOrDefault(q => q.PRINT_QUALITY_NUMBER == item.SUPPORTED_PRINT_QUALITY);

                    var trimFormat = new SGGS_SUPPORTED_TRIM_FORMATS
                    {
                        SUPPORTED_PAPER_TYPE_ID = item.SUPPORTED_PAPER_TYPE,
                        SUPPORTED_BIND_TYPE_ID = item.SUPPORTED_BINDING_TYPE,
                        PRINT_PLANT_PROFILE_ID = item.PRINT_PLANT_PROFILE_ID,
                        TRIM_FORMAT_ID = item.TRIM_FORMAT_ID,
                        TEXT_PAGE_COUNT_MAX = item.MAX_PAGES,
                        TEXT_PAGE_COUNT_MIN = item.MIN_PAGES,
                        TRIM_FORMAT_MAX_HEIGHT = item.TRIM_FORMAT_MAX_HEIGHT,
                        TRIM_FORMAT_MIN_HEIGHT = item.TRIM_FORMAT_MIN_HEIGHT,
                        TRIM_FORMAT_MIN_WIDTH = item.TRIM_FORMAT_MIN_WIDTH,
                        TRIM_FORMAT_MAX_WIDTH = item.TRIM_FORMAT_MAX_WIDTH,                       
                        OVERRIDE_AUTO_CALCULATION = item.OVERRIDE_AUTO_CALCULATION
                    };
                    if (sggsCodePrintQuality != null)
                        trimFormat.SUPPORTED_PRINT_QUALITY = sggsCodePrintQuality.PRINT_QUALITY_ID;

                    if (!CheckTrimFormatIsValid(trimFormat, supportRanges))
                        context.SGGS_SUPPORTED_TRIM_FORMATS.AddObject(trimFormat);

                }
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Method that adds new trim format to the duplicated print plant profile
        /// </summary>
        /// <returns></returns>
        public bool AddDuplicatedTrimFormats()
        {
            using (var context = new SGGSEntities())
            {
                var trimFormat = new SGGS_SUPPORTED_TRIM_FORMATS
                {
                    SUPPORTED_PAPER_TYPE_ID = SUPPORTED_PAPER_TYPE,
                    SUPPORTED_BIND_TYPE_ID = SUPPORTED_BINDING_TYPE,
                    PRINT_PLANT_PROFILE_ID = PRINT_PLANT_PROFILE_ID,
                    TRIM_FORMAT_ID = TRIM_FORMAT_ID,
                    TEXT_PAGE_COUNT_MAX = MAX_PAGES,
                    TEXT_PAGE_COUNT_MIN = MIN_PAGES,
                    TRIM_FORMAT_MIN_WIDTH = TRIM_FORMAT_MIN_WIDTH,
                    TRIM_FORMAT_MAX_WIDTH = TRIM_FORMAT_MAX_WIDTH,
                    TRIM_FORMAT_MAX_HEIGHT = TRIM_FORMAT_MAX_HEIGHT,
                    TRIM_FORMAT_MIN_HEIGHT = TRIM_FORMAT_MIN_HEIGHT,
                    OVERRIDE_AUTO_CALCULATION = OVERRIDE_AUTO_CALCULATION,
                    SUPPORTED_PRINT_QUALITY = SUPPORTED_PRINT_QUALITY_ID
                };


                context.SGGS_SUPPORTED_TRIM_FORMATS.AddObject(trimFormat);
                context.SaveChanges();
                return true;
            }
        }
        /// <summary>
        /// Load Supported TrimFormats by printPlantProfileId
        /// </summary>
        /// <param name="printPlantProfileId"></param>
        /// <param name="supportRanges"></param>
        /// <returns></returns>
        public DataTable LoadSupportedTrimFormats(long printPlantProfileId, bool supportRanges)
        {
            using (var context = new SGGSEntities())
            {
                if (supportRanges)
                {
                    var trimFormats = context.SGGS_SUPPORTED_TRIM_FORMATS.AsEnumerable()
                        .Where(
                            x =>
                                x.PRINT_PLANT_PROFILE_ID == printPlantProfileId && x.TRIM_FORMAT_MAX_HEIGHT != null &&
                                x.TRIM_FORMAT_MIN_HEIGHT != null && x.TRIM_FORMAT_MAX_WIDTH != null &&
                                x.TRIM_FORMAT_MIN_WIDTH != null)
                        .Select(s => new
                        {
                            TRIM_FORMAT_ID = s.SUPPORTED_TRIM_FORMATS_ID,

                            PAPER_TYPE = (from p in context.SGGS_PAPER_TYPE
                                          join sp in context.SGGS_SUPPORTED_PAPER_TYPE on p.PAPER_TYPE_ID equals sp.PAPER_TYPE_ID
                                          where sp.SUPPORTED_PAPER_TYPE_ID == s.SUPPORTED_PAPER_TYPE_ID
                                          select p.PAPER_NAME).FirstOrDefault(),

                            BINDING_TYPE = (from b in context.SGGS_CODE_BINDING_TYPE
                                            join sb in context.SGGS_SUPPORTED_BIND_TYPE on b.BINDING_TYPE_ID equals
                                                sb.BINDING_TYPE_ID
                                            where sb.SUPPORTED_BIND_TYPE_ID == s.SUPPORTED_BIND_TYPE_ID
                                            select b.BINDING_TYPE_NAME).FirstOrDefault(),

                            TRIM_FORMAT =
                                context.SGGS_CODE_TRIM_FORMAT.Where(t => t.TRIM_FORMAT_ID == s.TRIM_FORMAT_ID)
                                    .Select(ts => ts.TRIM_FORMAT_NAME)
                                    .FirstOrDefault(),
                            PRINT_QUALITY =
                                context.SGGS_CODE_PRINT_QUALITY.Where(
                                    q => q.PRINT_QUALITY_ID == s.SUPPORTED_PRINT_QUALITY)
                                    .Select(qs => qs.PRINT_QUALITY_NAME)
                                    .FirstOrDefault(),
                            MIN_PAGES = s.TEXT_PAGE_COUNT_MIN,
                            MAX_PAGES = s.TEXT_PAGE_COUNT_MAX,
                            WIDTH = (s.TRIM_FORMAT_MIN_WIDTH.HasValue ? s.TRIM_FORMAT_MIN_WIDTH.Value.ToString("G29") : "") + " - " + (s.TRIM_FORMAT_MAX_WIDTH.HasValue ? s.TRIM_FORMAT_MAX_WIDTH.Value.ToString("G29") : ""),
                            HEIGHT = (s.TRIM_FORMAT_MIN_HEIGHT.HasValue ? s.TRIM_FORMAT_MIN_HEIGHT.Value.ToString("G29") :"") + " - " + (s.TRIM_FORMAT_MAX_HEIGHT.HasValue ? s.TRIM_FORMAT_MAX_HEIGHT.Value.ToString("G29"):"")
                        }).OrderBy(tf => tf.WIDTH).ThenBy(tf => tf.HEIGHT).ThenBy(tf => tf.PAPER_TYPE).ThenBy(tf => tf.BINDING_TYPE).ThenBy(tf => tf.PRINT_QUALITY).ToList();
                    
                    return CommonUtils.ToDataTable(trimFormats, "TrimFormats");
                }
                else
                {
                    var trimFormats = context.SGGS_SUPPORTED_TRIM_FORMATS.AsEnumerable()
                        .Where(
                            x =>
                                x.PRINT_PLANT_PROFILE_ID == printPlantProfileId && x.TRIM_FORMAT_MAX_HEIGHT == null &&
                                x.TRIM_FORMAT_MIN_HEIGHT == null && x.TRIM_FORMAT_MAX_WIDTH == null &&
                                x.TRIM_FORMAT_MIN_WIDTH == null)
                        .Select(s => new
                        {
                            TRIM_FORMAT_ID = s.SUPPORTED_TRIM_FORMATS_ID,

                            PAPER_TYPE = (from p in context.SGGS_PAPER_TYPE
                                          join sp in context.SGGS_SUPPORTED_PAPER_TYPE on p.PAPER_TYPE_ID equals sp.PAPER_TYPE_ID
                                          where sp.SUPPORTED_PAPER_TYPE_ID == s.SUPPORTED_PAPER_TYPE_ID
                                          select p.PAPER_NAME).FirstOrDefault(),

                            BINDING_TYPE = (from b in context.SGGS_CODE_BINDING_TYPE
                                            join sb in context.SGGS_SUPPORTED_BIND_TYPE on b.BINDING_TYPE_ID equals
                                                sb.BINDING_TYPE_ID
                                            where sb.SUPPORTED_BIND_TYPE_ID == s.SUPPORTED_BIND_TYPE_ID
                                            select b.BINDING_TYPE_NAME).FirstOrDefault(),

                            TRIM_FORMAT =
                                context.SGGS_CODE_TRIM_FORMAT.Where(t => t.TRIM_FORMAT_ID == s.TRIM_FORMAT_ID)
                                    .Select(ts => ts.TRIM_FORMAT_NAME)
                                    .FirstOrDefault(),
                            PRINT_QUALITY =
                                context.SGGS_CODE_PRINT_QUALITY.Where(
                                    q => q.PRINT_QUALITY_ID == s.SUPPORTED_PRINT_QUALITY)
                                    .Select(qs => qs.PRINT_QUALITY_NAME)
                                    .FirstOrDefault(),
                            MIN_PAGES = s.TEXT_PAGE_COUNT_MIN,
                            MAX_PAGES = s.TEXT_PAGE_COUNT_MAX,
                            WIDTH = (s.TRIM_FORMAT_MIN_WIDTH.HasValue ? s.TRIM_FORMAT_MIN_WIDTH.Value.ToString("G29") : "") + " - " + (s.TRIM_FORMAT_MAX_WIDTH.HasValue ? s.TRIM_FORMAT_MAX_WIDTH.Value.ToString("G29"):""),
                            HEIGHT = (s.TRIM_FORMAT_MIN_HEIGHT.HasValue ? s.TRIM_FORMAT_MIN_HEIGHT.Value.ToString("G29"):"") + " - " + (s.TRIM_FORMAT_MAX_HEIGHT.HasValue ? s.TRIM_FORMAT_MAX_HEIGHT.Value.ToString("G29"):"")
                        }).OrderBy(tf => tf.TRIM_FORMAT).ThenBy(tf => tf.PAPER_TYPE).ThenBy(tf => tf.BINDING_TYPE).ThenBy(tf => tf.PRINT_QUALITY).ToList();;

                    return CommonUtils.ToDataTable(trimFormats, "TrimFormats");
                }

            }
        }

        /// <summary>
        /// Get Supported TrimFormat Information
        /// </summary>
        /// <param name="trimFormatId"></param>
        /// <returns></returns>
        public dynamic GetSupportedTrimFormatInformation(long trimFormatId)
        {
            using (var context = new SGGSEntities())
            {
                var trimFormats = context.SGGS_SUPPORTED_TRIM_FORMATS.AsEnumerable()
                    .Where(x => x.SUPPORTED_TRIM_FORMATS_ID == trimFormatId)
                    .Select(s => new
                    {
                        TRIM_FORMAT_ID = s.SUPPORTED_TRIM_FORMATS_ID,

                        PAPER_TYPE = (from p in context.SGGS_PAPER_TYPE
                                      join sp in context.SGGS_SUPPORTED_PAPER_TYPE on p.PAPER_TYPE_ID equals sp.PAPER_TYPE_ID
                                      where sp.SUPPORTED_PAPER_TYPE_ID == s.SUPPORTED_PAPER_TYPE_ID
                                      select p.PAPER_NAME).FirstOrDefault(),

                        BINDING_TYPE = (from b in context.SGGS_CODE_BINDING_TYPE
                                        join sb in context.SGGS_SUPPORTED_BIND_TYPE on b.BINDING_TYPE_ID equals sb.BINDING_TYPE_ID
                                        where sb.SUPPORTED_BIND_TYPE_ID == s.SUPPORTED_BIND_TYPE_ID
                                        select b.BINDING_TYPE_NAME).FirstOrDefault(),

                        TRIM_FORMAT = context.SGGS_CODE_TRIM_FORMAT.Where(t => t.TRIM_FORMAT_ID == s.TRIM_FORMAT_ID).Select(ts => ts.TRIM_FORMAT_NAME).FirstOrDefault(),
                        PRINT_QUALITY = context.SGGS_CODE_PRINT_QUALITY.Where(q => q.PRINT_QUALITY_ID == s.SUPPORTED_PRINT_QUALITY).Select(qs => qs.PRINT_QUALITY_NAME).FirstOrDefault(),
                        MIN_PAGES = s.TEXT_PAGE_COUNT_MIN,
                        MAX_PAGES = s.TEXT_PAGE_COUNT_MAX,
                        WIDTH = (s.TRIM_FORMAT_MIN_WIDTH.HasValue ? s.TRIM_FORMAT_MIN_WIDTH.Value.ToString("G29"):"") + " - " + (s.TRIM_FORMAT_MAX_WIDTH.HasValue ? s.TRIM_FORMAT_MAX_WIDTH.Value.ToString("G29"):""),
                        HEIGHT = (s.TRIM_FORMAT_MIN_HEIGHT.HasValue ? s.TRIM_FORMAT_MIN_HEIGHT.Value.ToString("G29"):"") + " - " + (s.TRIM_FORMAT_MAX_HEIGHT.HasValue? s.TRIM_FORMAT_MAX_HEIGHT.Value.ToString("G29"):""),
                        OVERRIDE = s.OVERRIDE_AUTO_CALCULATION
                    }).FirstOrDefault();

                return trimFormats;
            }
        }

        /// <summary>
        /// Update TrimFormat Update
        /// </summary>
        /// <param name="supportedTrimFormatId"></param>
        /// <param name="minPages"></param>
        /// <param name="maxPages"></param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <param name="overriding"></param>
        /// <returns></returns>
        public string Update(long supportedTrimFormatId, int? minPages, int? maxPages, decimal? minWidth, decimal? maxWidth,
        decimal? minHeight, decimal? maxHeight, bool overriding)
        {
            using (var context = new SGGSEntities())
            {
                var trimFormats = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(x => x.SUPPORTED_TRIM_FORMATS_ID == supportedTrimFormatId);

                if (trimFormats == null) return "notSaved";

                trimFormats.TEXT_PAGE_COUNT_MIN = minPages;
                trimFormats.TEXT_PAGE_COUNT_MAX = maxPages;
                trimFormats.TRIM_FORMAT_MIN_WIDTH = minWidth;
                trimFormats.TRIM_FORMAT_MAX_WIDTH = maxWidth;
                trimFormats.TRIM_FORMAT_MIN_HEIGHT = minHeight;
                trimFormats.TRIM_FORMAT_MAX_HEIGHT = maxHeight;
                trimFormats.OVERRIDE_AUTO_CALCULATION = overriding;

                context.SaveChanges();
                return "saved";
            }
        }

        /// <summary>
        /// Remove a TrimFormat by trimFormatId
        /// </summary>
        /// <param name="trimFormatId"></param>
        /// <returns></returns>
        public string RemoveTrimFormat(long trimFormatId)
        {
            using (var context = new SGGSEntities())
            {
                var trimFormat = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(x => x.SUPPORTED_TRIM_FORMATS_ID == trimFormatId);

                if (trimFormat == null) return "notRemoved";

                context.SGGS_SUPPORTED_TRIM_FORMATS.DeleteObject(trimFormat);
                context.SaveChanges();
                return "removed";
            }
        }

        /// <summary>
        /// Check if the new Trim Format exists or the paper type is correct.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public bool CheckTrimFormatIsValid(SGGS_SUPPORTED_TRIM_FORMATS format, bool supportRanges)
        {
            var IsExcluded = false;

            using (var context = new SGGSEntities())
            {
                SGGS_SUPPORTED_TRIM_FORMATS isFound = null;
                if (!supportRanges)
                {
                    isFound = context.SGGS_SUPPORTED_TRIM_FORMATS.Where(
                        tf => tf.PRINT_PLANT_PROFILE_ID == format.PRINT_PLANT_PROFILE_ID &&
                              tf.SUPPORTED_PAPER_TYPE_ID == format.SUPPORTED_PAPER_TYPE_ID &&
                              tf.SUPPORTED_BIND_TYPE_ID == format.SUPPORTED_BIND_TYPE_ID &&
                              tf.TRIM_FORMAT_ID == format.TRIM_FORMAT_ID &&
                              tf.SUPPORTED_PRINT_QUALITY == format.SUPPORTED_PRINT_QUALITY)
                        .Select(s => s).FirstOrDefault();
                }
                else
                {
                    isFound = context.SGGS_SUPPORTED_TRIM_FORMATS.Where(
                            tf => tf.PRINT_PLANT_PROFILE_ID == format.PRINT_PLANT_PROFILE_ID &&
                                  tf.SUPPORTED_PAPER_TYPE_ID == format.SUPPORTED_PAPER_TYPE_ID &&
                                  tf.SUPPORTED_BIND_TYPE_ID == format.SUPPORTED_BIND_TYPE_ID &&
                                  tf.SUPPORTED_PRINT_QUALITY == format.SUPPORTED_PRINT_QUALITY &&
                                  tf.TRIM_FORMAT_MAX_HEIGHT == format.TRIM_FORMAT_MAX_HEIGHT &&
                                  tf.TRIM_FORMAT_MIN_HEIGHT == format.TRIM_FORMAT_MIN_HEIGHT &&
                                  tf.TRIM_FORMAT_MAX_WIDTH == format.TRIM_FORMAT_MAX_WIDTH &&
                                  tf.TRIM_FORMAT_MIN_WIDTH == format.TRIM_FORMAT_MIN_WIDTH)
                            .Select(s => s).FirstOrDefault();
                }


                if (isFound != null)
                {
                    if (HttpContext.Current.Session["ExcludedTrimFormats"] == null)
                        EXCLUDED_PAPER_TYPES = new List<SGGS_SUPPORTED_TRIM_FORMATS>();
                    else
                        EXCLUDED_PAPER_TYPES = HttpContext.Current.Session["ExcludedTrimFormats"] as List<SGGS_SUPPORTED_TRIM_FORMATS>;

                    EXCLUDED_PAPER_TYPES.Add(isFound);
                    HttpContext.Current.Session["ExcludedTrimFormats"] = EXCLUDED_PAPER_TYPES;

                    IsExcluded = true;
                }

                //Check if Supported Paper Type is validates for the Trim Format Quality
                else
                {
                    var suppPaperType = context.SGGS_SUPPORTED_PAPER_TYPE.FirstOrDefault(x => x.SUPPORTED_PAPER_TYPE_ID == format.SUPPORTED_PAPER_TYPE_ID);

                    var trimFormatquality = (from cpq in context.SGGS_CODE_PRINT_QUALITY
                                             where cpq.PRINT_QUALITY_ID == format.SUPPORTED_PRINT_QUALITY
                                             select cpq.PRINT_QUALITY_NUMBER).FirstOrDefault();
                    if (trimFormatquality != null)
                    {
                        if (trimFormatquality.Equals("HQ") && suppPaperType.TEXT_HIGH_QUALITY == false)
                        {
                            if (HttpContext.Current.Session["ExcludedTrimFormats"] == null)
                                EXCLUDED_PAPER_TYPES = new List<SGGS_SUPPORTED_TRIM_FORMATS>();
                            else
                                EXCLUDED_PAPER_TYPES = HttpContext.Current.Session["ExcludedTrimFormats"] as List<SGGS_SUPPORTED_TRIM_FORMATS>;

                            EXCLUDED_PAPER_TYPES.Add(format);
                            HttpContext.Current.Session["ExcludedTrimFormats"] = EXCLUDED_PAPER_TYPES;
                            IsExcluded = true;
                        }

                        else if (trimFormatquality.Equals("NQ") && suppPaperType.TEXT_NORMAL_QUALITY == false)
                        {
                            if (HttpContext.Current.Session["ExcludedTrimFormats"] == null)
                                EXCLUDED_PAPER_TYPES = new List<SGGS_SUPPORTED_TRIM_FORMATS>();
                            else
                                EXCLUDED_PAPER_TYPES = HttpContext.Current.Session["ExcludedTrimFormats"] as List<SGGS_SUPPORTED_TRIM_FORMATS>;

                            EXCLUDED_PAPER_TYPES.Add(format);
                            HttpContext.Current.Session["ExcludedTrimFormats"] = EXCLUDED_PAPER_TYPES;

                            IsExcluded = true;
                        }
                    }
                    else if (suppPaperType.TEXT_HIGH_QUALITY == true || suppPaperType.TEXT_NORMAL_QUALITY == true) {
                        if (HttpContext.Current.Session["ExcludedTrimFormats"] == null)
                            EXCLUDED_PAPER_TYPES = new List<SGGS_SUPPORTED_TRIM_FORMATS>();
                        else
                            EXCLUDED_PAPER_TYPES = HttpContext.Current.Session["ExcludedTrimFormats"] as List<SGGS_SUPPORTED_TRIM_FORMATS>;

                        EXCLUDED_PAPER_TYPES.Add(format);
                        HttpContext.Current.Session["ExcludedTrimFormats"] = EXCLUDED_PAPER_TYPES;

                        IsExcluded = true;
                    }

                }

            }

            return IsExcluded;
        }

        /// <summary>
        /// Update TrimFormat
        /// </summary>
        /// <param name="supportedTrimFormatId"></param>
        /// <param name="minPages"></param>
        /// <param name="maxPages"></param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <param name="overriding"></param>
        /// <param name="supportRanges"></param>
        /// <returns></returns>
        public string UpdatingTrimFormat(long supportedTrimFormatId, string minPages, string maxPages, string minWidth, string maxWidth,
        string minHeight, string maxHeight, bool overriding, bool supportRanges)
        {
            using (var context = new SGGSEntities())
            {
                SGGS_SUPPORTED_TRIM_FORMATS dataFound = null;
                var trimFormat = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(x => x.SUPPORTED_TRIM_FORMATS_ID == supportedTrimFormatId);

                if (trimFormat == null) return "notExists";

                if (!supportRanges && overriding)
                {
                    var maxP = Convert.ToInt32(maxPages);
                    var minP = Convert.ToInt32(minPages);

                    dataFound = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(tf =>
                        tf.SUPPORTED_TRIM_FORMATS_ID != trimFormat.SUPPORTED_TRIM_FORMATS_ID &&
                        tf.PRINT_PLANT_PROFILE_ID == trimFormat.PRINT_PLANT_PROFILE_ID &&
                        tf.SUPPORTED_PAPER_TYPE_ID == trimFormat.SUPPORTED_PAPER_TYPE_ID &&
                        tf.SUPPORTED_BIND_TYPE_ID == trimFormat.SUPPORTED_BIND_TYPE_ID &&
                        tf.TRIM_FORMAT_ID == trimFormat.TRIM_FORMAT_ID &&
                        tf.SUPPORTED_PRINT_QUALITY == trimFormat.SUPPORTED_PRINT_QUALITY &&
                        tf.TEXT_PAGE_COUNT_MAX == maxP &&
                        tf.TEXT_PAGE_COUNT_MIN == minP &&
                        tf.TRIM_FORMAT_MIN_WIDTH == null &&
                        tf.TRIM_FORMAT_MAX_WIDTH == null &&
                        tf.TRIM_FORMAT_MIN_HEIGHT == null &&
                        tf.TRIM_FORMAT_MAX_HEIGHT == null &&
                        tf.OVERRIDE_AUTO_CALCULATION == overriding);

                    if (dataFound == null)
                        return Update(supportedTrimFormatId, Convert.ToInt32(minPages), Convert.ToInt32(maxPages), null, null, null, null, overriding);

                }
                else if (!supportRanges && !overriding)
                {
                    dataFound = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(tf =>
                        tf.SUPPORTED_TRIM_FORMATS_ID != trimFormat.SUPPORTED_TRIM_FORMATS_ID &&
                        tf.PRINT_PLANT_PROFILE_ID == trimFormat.PRINT_PLANT_PROFILE_ID &&
                        tf.SUPPORTED_PAPER_TYPE_ID == trimFormat.SUPPORTED_PAPER_TYPE_ID &&
                        tf.SUPPORTED_BIND_TYPE_ID == trimFormat.SUPPORTED_BIND_TYPE_ID &&
                        tf.TRIM_FORMAT_ID == trimFormat.TRIM_FORMAT_ID &&
                        tf.SUPPORTED_PRINT_QUALITY == trimFormat.SUPPORTED_PRINT_QUALITY &&
                        tf.TEXT_PAGE_COUNT_MAX == null &&
                        tf.TEXT_PAGE_COUNT_MIN == null &&
                        tf.TRIM_FORMAT_MIN_WIDTH == null &&
                        tf.TRIM_FORMAT_MAX_WIDTH == null &&
                        tf.TRIM_FORMAT_MIN_HEIGHT == null &&
                        tf.TRIM_FORMAT_MAX_HEIGHT == null &&
                        tf.OVERRIDE_AUTO_CALCULATION == overriding);

                    if (dataFound == null)
                        return Update(supportedTrimFormatId, null, null, null, null, null, null, overriding);
                }
                else if (supportRanges && !overriding)
                {
                    var maxW = Math.Round(Convert.ToDecimal(maxWidth),5);
                    var minW = Math.Round(Convert.ToDecimal(minWidth),5);
                    var minH = Math.Round(Convert.ToDecimal(minHeight),5);
                    var maxH = Math.Round(Convert.ToDecimal(maxHeight),5);

                    dataFound = context.SGGS_SUPPORTED_TRIM_FORMATS.FirstOrDefault(tf =>
                        tf.SUPPORTED_TRIM_FORMATS_ID != trimFormat.SUPPORTED_TRIM_FORMATS_ID &&
                        tf.PRINT_PLANT_PROFILE_ID == trimFormat.PRINT_PLANT_PROFILE_ID &&
                        tf.SUPPORTED_PAPER_TYPE_ID == trimFormat.SUPPORTED_PAPER_TYPE_ID &&
                        tf.SUPPORTED_BIND_TYPE_ID == trimFormat.SUPPORTED_BIND_TYPE_ID &&
                        tf.TRIM_FORMAT_ID == null &&
                        tf.SUPPORTED_PRINT_QUALITY == trimFormat.SUPPORTED_PRINT_QUALITY &&
                        tf.TEXT_PAGE_COUNT_MAX == null &&
                        tf.TEXT_PAGE_COUNT_MIN == null &&
                        tf.TRIM_FORMAT_MIN_WIDTH == minW &&
                        tf.TRIM_FORMAT_MAX_WIDTH == maxW &&
                        tf.TRIM_FORMAT_MIN_HEIGHT == minH &&
                        tf.TRIM_FORMAT_MAX_HEIGHT == maxH &&
                        tf.OVERRIDE_AUTO_CALCULATION == overriding);

                    if (dataFound == null)
                        return Update(supportedTrimFormatId, null, null, Math.Round(Convert.ToDecimal(minWidth),5), Math.Round(Convert.ToDecimal(maxWidth),5), Math.Round(Convert.ToDecimal(minHeight),5),
                        Math.Round(Convert.ToDecimal(maxHeight),5), overriding);
                }


                return dataFound != null ? "exists" : string.Empty;
            }
        }
        /// <summary>
        /// Add Duplicate Supported TrimFormat
        /// </summary>
        /// <param name="oldPrintPlantProfileId"></param>
        /// <param name="newPrintPlantProfile"></param>
        public void DuplicateSupportedTrimFormat(long oldPrintPlantProfileId, long newPrintPlantProfile)
        {
            using (var context = new SGGSEntities())
            {
                var supportedTrimFormats = (from tf in context.SGGS_SUPPORTED_TRIM_FORMATS
                                            join pt in context.SGGS_SUPPORTED_PAPER_TYPE on tf.SUPPORTED_PAPER_TYPE_ID equals pt.SUPPORTED_PAPER_TYPE_ID
                                            join bt in context.SGGS_SUPPORTED_BIND_TYPE on tf.SUPPORTED_BIND_TYPE_ID equals bt.SUPPORTED_BIND_TYPE_ID
                                            where tf.PRINT_PLANT_PROFILE_ID == oldPrintPlantProfileId
                                            select new TrimFormat
                               {
                                   SUPPORTED_PAPER_TYPE = tf.SUPPORTED_PAPER_TYPE_ID,
                                   SUPPORTED_BINDING_TYPE = tf.SUPPORTED_BIND_TYPE_ID,
                                   PAPER_TYPE_ID = pt.PAPER_TYPE_ID,
                                   BINDING_TYPE_ID = bt.BINDING_TYPE_ID,
                                   PRINT_PLANT_PROFILE_ID = tf.PRINT_PLANT_PROFILE_ID,
                                   TRIM_FORMAT_ID = tf.TRIM_FORMAT_ID,
                                   MAX_PAGES = tf.TEXT_PAGE_COUNT_MAX,
                                   MIN_PAGES = tf.TEXT_PAGE_COUNT_MIN,
                                   TRIM_FORMAT_MIN_WIDTH = tf.TRIM_FORMAT_MIN_WIDTH,
                                   TRIM_FORMAT_MAX_WIDTH = tf.TRIM_FORMAT_MAX_WIDTH,
                                   TRIM_FORMAT_MAX_HEIGHT = tf.TRIM_FORMAT_MAX_HEIGHT,
                                   TRIM_FORMAT_MIN_HEIGHT = tf.TRIM_FORMAT_MIN_HEIGHT,
                                   OVERRIDE_AUTO_CALCULATION = tf.OVERRIDE_AUTO_CALCULATION,
                                   SUPPORTED_PRINT_QUALITY_ID = tf.SUPPORTED_PRINT_QUALITY
                               }).ToList();


                foreach (var item in supportedTrimFormats)
                {
                    SUPPORTED_PAPER_TYPE = (from bt in context.SGGS_SUPPORTED_PAPER_TYPE                                          
                                            where bt.PRINT_PLANT_PROFILE_ID == newPrintPlantProfile && bt.PAPER_TYPE_ID == item.PAPER_TYPE_ID
                                            select bt.SUPPORTED_PAPER_TYPE_ID).FirstOrDefault();
                    SUPPORTED_BINDING_TYPE = (from bt in context.SGGS_SUPPORTED_BIND_TYPE
                                              where bt.PRINT_PLANT_PROFILE_ID == newPrintPlantProfile && bt.BINDING_TYPE_ID == item.BINDING_TYPE_ID
                                              select bt.SUPPORTED_BIND_TYPE_ID).FirstOrDefault();
                    PRINT_PLANT_PROFILE_ID = newPrintPlantProfile;
                    TRIM_FORMAT_ID = item.TRIM_FORMAT_ID;
                    MAX_PAGES = item.MAX_PAGES;
                    MIN_PAGES = item.MIN_PAGES;
                    TRIM_FORMAT_MIN_WIDTH = item.TRIM_FORMAT_MIN_WIDTH;
                    TRIM_FORMAT_MAX_WIDTH = item.TRIM_FORMAT_MAX_WIDTH;
                    TRIM_FORMAT_MAX_HEIGHT = item.TRIM_FORMAT_MAX_HEIGHT;
                    TRIM_FORMAT_MIN_HEIGHT = item.TRIM_FORMAT_MIN_HEIGHT;
                    OVERRIDE_AUTO_CALCULATION = item.OVERRIDE_AUTO_CALCULATION;
                    SUPPORTED_PRINT_QUALITY_ID = item.SUPPORTED_PRINT_QUALITY_ID;
                    AddDuplicatedTrimFormats();
                }
            }
        }

        /// <summary>
        /// To delete data that was duplicated
        /// </summary>
        /// <param name="printPlantProfileId">print plant profile identifier</param>
        public void DeleteDuplicatedSupportedTrimFormat(long printPlantProfileId)
        {
            using (var context = new SGGSEntities())
            {
                var supportedTrimFormats = context.SGGS_SUPPORTED_TRIM_FORMATS
                    .Where(x => x.PRINT_PLANT_PROFILE_ID == printPlantProfileId)
                    .Select(s => new
                    {
                        s.SUPPORTED_TRIM_FORMATS_ID
                    }).ToList();

                foreach (var item in supportedTrimFormats)
                {
                    DeleteSupportTrimFormat(item.SUPPORTED_TRIM_FORMATS_ID);
                }
            }
        }

        /// <summary>
        /// Get Paper Type and Print Quality Information
        /// </summary>
        /// <param name="suppPaperTypeID"></param>
        /// <param name="PrintQualityID"></param>
        /// <returns></returns>
        public dynamic GetPaperQualityInformation(long suppPaperTypeID, long PrintQualityID)
        {
            using (var context = new SGGSEntities())
            {
                var paperQualityInformation = context.SGGS_SUPPORTED_PAPER_TYPE
                    .Where(x => x.SUPPORTED_PAPER_TYPE_ID == suppPaperTypeID)
                    .Select(s => new
                    {
                        PAPER_TYPE_NAME = (from p in context.SGGS_PAPER_TYPE
                                           where p.PAPER_TYPE_ID == s.PAPER_TYPE_ID
                                           select p.PAPER_NAME).FirstOrDefault(),

                        PrintQuality = (from pq in context.SGGS_CODE_PRINT_QUALITY
                                        where pq.PRINT_QUALITY_ID == PrintQualityID
                                        select pq.PRINT_QUALITY_NAME).FirstOrDefault(),


                    }).FirstOrDefault();

                return paperQualityInformation != null ? paperQualityInformation : null;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
