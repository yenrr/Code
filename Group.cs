using System;
using System.Data;
using System.Data.SqlClient;
using SGGSManagement.Library.Base;
using SGGSManagement.Library.Util;
using SGGS.Busniss.Entities;
using System.Linq;
using Common.Utils;
using System.Collections.Generic;

namespace SGGSManagement.Library.Administration
{
    public class Group
    {

        DateTime _DATE_ADDED = DateTime.Now;
        DateTime _DATE_UPDATED = DateTime.Now;
        String _DEFAULT_PUBLISHER_NUMBER = "";
        String _GROUP_CODE = "";
        String _GROUP_DESCRIPTION = "";
        long _GROUP_ID = 0;
        String _GROUP_NAME = "";
        public DateTime DATE_ADDED
        {
            get { return _DATE_ADDED; }
            set { _DATE_ADDED = value; }
        }

        public DateTime DATE_UPDATED
        {
            get { return _DATE_UPDATED; }
            set { _DATE_UPDATED = value; }
        }

        public String DEFAULT_PUBLISHER_NUMBER
        {
            get { return _DEFAULT_PUBLISHER_NUMBER; }
            set { _DEFAULT_PUBLISHER_NUMBER = value; }
        }

        public String GROUP_CODE
        {
            get { return _GROUP_CODE; }
            set { _GROUP_CODE = value; }
        }

        public String GROUP_DESCRIPTION
        {
            get { return _GROUP_DESCRIPTION; }
            set { _GROUP_DESCRIPTION = value; }
        }

        public long GROUP_ID
        {
            get { return _GROUP_ID; }
            set { _GROUP_ID = value; }
        }

        public String GROUP_NAME
        {
            get { return _GROUP_NAME; }
            set { _GROUP_NAME = value; }
        }


        /// <summary>
        /// Method to insert record in Group Table
        /// </summary>
        /// <returns></returns>
        public Boolean Add()
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                SGGS_GROUP group = new SGGS_GROUP();
                group.DATE_ADDED = DateTime.Now;
                group.DATE_UPDATED = DateTime.Now;
                group.DEFAULT_PUBLISHER_NUMBER = DEFAULT_PUBLISHER_NUMBER;
                group.GROUP_CODE = GROUP_CODE;
                group.GROUP_DESCRIPTION = GROUP_DESCRIPTION;
                group.GROUP_NAME = GROUP_NAME;
                context.SGGS_GROUP.AddObject(group);
                context.SaveChanges();
                GROUP_ID = group.GROUP_ID;
                return true;
            }
        }

        /// <summary>
        /// Method to update a record
        /// </summary>
        /// <returns></returns>
        public Boolean Save()
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                SGGS_GROUP group = context.SGGS_GROUP.Where(x => x.GROUP_ID == GROUP_ID).FirstOrDefault();
                group.DATE_UPDATED = DateTime.Now;
                group.DEFAULT_PUBLISHER_NUMBER = DEFAULT_PUBLISHER_NUMBER;
                group.GROUP_CODE = GROUP_CODE;
                group.GROUP_DESCRIPTION = GROUP_DESCRIPTION;
                group.GROUP_NAME = GROUP_NAME;
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Method to Load values from DB by group Id
        /// </summary>
        /// <param name="GROUP_ID"></param>
        /// <returns></returns>
        public bool Load(long GROUP_ID)
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                var result = (from g in context.SGGS_GROUP
                              where g.GROUP_ID == GROUP_ID
                              select new
                              {
                                  g.DATE_ADDED,
                                  g.DATE_UPDATED,
                                  g.DEFAULT_PUBLISHER_NUMBER,
                                  g.GROUP_CODE,
                                  g.GROUP_DESCRIPTION,
                                  g.GROUP_ID,
                                  g.GROUP_NAME
                              }).FirstOrDefault();


                if (result != null)
                {
                    this.DATE_ADDED = result.DATE_ADDED.Value;
                    this.DATE_UPDATED = result.DATE_UPDATED.Value;
                    this.DEFAULT_PUBLISHER_NUMBER = result.DEFAULT_PUBLISHER_NUMBER;
                    this.GROUP_CODE = result.GROUP_CODE;
                    this.GROUP_DESCRIPTION = result.GROUP_DESCRIPTION;
                    this.GROUP_ID = result.GROUP_ID;
                    this.GROUP_NAME = result.GROUP_NAME;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get All groups by search criteria
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public DataTable GetAllGroups(string search)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        var result = context.SGGS_GROUP.Where(x => x.GROUP_CODE.ToString().Contains(search) || x.GROUP_NAME.Contains(search) || x.DEFAULT_PUBLISHER_NUMBER.Contains(search))
                            .OrderBy(o => o.GROUP_CODE)
                            .Select(s => new
                            {
                                s.DATE_ADDED,
                                s.DATE_UPDATED,
                                DEFAULT_PUBLISHER_NUMBER = context.SGGS_PUBLISHER.Where(x => x.PUBLISHER_NUMBER == s.DEFAULT_PUBLISHER_NUMBER).Select(n => new { Name =  n.PUBLISHER_NUMBER + "-" + n.PUBLISHER_NAME }).FirstOrDefault().Name,
                                s.GROUP_CODE,
                                s.GROUP_DESCRIPTION,
                                s.GROUP_ID,
                                s.GROUP_NAME
                            }).ToList();

                        return CommonUtils.ToDataTable(result);
                    }
                    else
                    {
                        var result = context.SGGS_GROUP.OrderBy(o => o.GROUP_CODE)
                            .Select(s => new
                            {
                                s.DATE_ADDED,
                                s.DATE_UPDATED,
                                DEFAULT_PUBLISHER_NUMBER = context.SGGS_PUBLISHER.Where(x => x.PUBLISHER_NUMBER == s.DEFAULT_PUBLISHER_NUMBER).Select(n => new { Name = n.PUBLISHER_NUMBER + "-" + n.PUBLISHER_NAME }).FirstOrDefault().Name,
                                s.GROUP_CODE,
                                s.GROUP_DESCRIPTION,
                                s.GROUP_ID,
                                s.GROUP_NAME
                            }).ToList();

                        return CommonUtils.ToDataTable(result);
                    }
                    
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Publisher group by group Id
        /// </summary>
        /// <param name="GROUP_ID"></param>
        /// <returns></returns>
        public DataTable GetGroupPublishers(long GROUP_ID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    List<string> publisherIds = context.SGGS_GROUP_PUBLISHER.Where(x => x.GROUP_ID == GROUP_ID).Select(s => s.PUBLISHER_ID.ToString()).ToList();

                    var result = (from p in context.SGGS_PUBLISHER
                                  where publisherIds.Contains(p.PUBLISHER_ID.ToString())
                                  select p).AsEnumerable()
                                  .Select(s => new
                                  {
                                      s.PUBLISHER_ID,
                                      PUBLISHER_NAME = s.PUBLISHER_NUMBER + "-" + s.PUBLISHER_NAME
                                  }).ToList();

                    return CommonUtils.ToDataTable(result);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Edit Publisher group by Group ID
        /// </summary>
        /// <param name="GROUP_ID"></param>
        /// <returns></returns>
        public DataTable EditGroupPublishers(long GROUP_ID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var result = (from p in context.SGGS_PUBLISHER
                                  select p).AsEnumerable()
                                  .Select(s => new
                                  {
                                      s.PUBLISHER_ID,
                                      PUBLISHER_NAME = s.PUBLISHER_NUMBER + "-" + s.PUBLISHER_NAME,
                                      s.PUBLISHER_NUMBER,
                                      IS_GROUP_PUBLISHER = context.SGGS_GROUP_PUBLISHER.Any(gp => gp.GROUP_ID == GROUP_ID && gp.PUBLISHER_ID == s.PUBLISHER_ID)
                                  }).OrderBy(o => o.PUBLISHER_NUMBER).ToList();

                    return CommonUtils.ToDataTable(result);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Add a new pubisher group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="PUBLISHER_ID"></param>
        /// <returns></returns>
        public bool AddGroupPublishers(long groupID, long PUBLISHER_ID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var group = context.SGGS_GROUP_PUBLISHER.Where(x => x.GROUP_ID == groupID && x.PUBLISHER_ID == PUBLISHER_ID).FirstOrDefault();

                    if (group == null)
                    {
                        SGGS_GROUP_PUBLISHER newGroup = new SGGS_GROUP_PUBLISHER();
                        newGroup.PUBLISHER_ID = PUBLISHER_ID;
                        newGroup.GROUP_ID = groupID;
                        newGroup.DATE_ADDED = DateTime.Now;
                        newGroup.DATE_UPDATED = DateTime.Now;
                        context.SGGS_GROUP_PUBLISHER.AddObject(newGroup);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Delete a publisher group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="publisherID"></param>
        /// <returns></returns>
        public bool DeleteGroupPublisher(long groupID, long publisherID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    SGGS_GROUP_PUBLISHER group = context.SGGS_GROUP_PUBLISHER.Where(x => x.GROUP_ID == groupID && x.PUBLISHER_ID == publisherID).FirstOrDefault();
                    context.SGGS_GROUP_PUBLISHER.DeleteObject(group);
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// get a publiser group by GroupID
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static string GetGroupDefaultPublisher(long groupID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var result = context.SGGS_GROUP.Where(x => x.GROUP_ID == groupID).Select(s => new { s.DEFAULT_PUBLISHER_NUMBER }).FirstOrDefault();
                    if (result != null)
                        return result.DEFAULT_PUBLISHER_NUMBER;

                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Group Default Bill To Publisher
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static string GetGroupDefaultBillToPublisher(long groupID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var result = context.SGGS_GROUP.Where(x => x.GROUP_ID == groupID).Select(s => new { s.DEFAULT_BILL_TO_PUBLISHER_NUMBER }).FirstOrDefault();
                    if (result != null)
                        return result.DEFAULT_BILL_TO_PUBLISHER_NUMBER;

                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Update a group publisher
        /// </summary>
        /// <param name="publisherNumber"></param>
        /// <param name="billToPublisherNumber"></param>
        /// <returns></returns>
        public bool UpdateGroupPublishers(string publisherNumber, string billToPublisherNumber)
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                SGGS_GROUP group = context.SGGS_GROUP.Where(x => x.GROUP_ID == this.GROUP_ID).FirstOrDefault();

                if (group != null)
                {
                    group.DEFAULT_PUBLISHER_NUMBER = publisherNumber;
                    group.DEFAULT_BILL_TO_PUBLISHER_NUMBER = billToPublisherNumber;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Method to Delete Record
        /// </summary>
        /// <returns></returns>
        public Boolean Delete()
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                SGGS_GROUP group = context.SGGS_GROUP.Where(x => x.GROUP_ID == GROUP_ID).FirstOrDefault();
                context.SGGS_GROUP.DeleteObject(group);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Static method to get GroupName for a specified groupID 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static string GetGroupName(long groupID)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var result = context.SGGS_GROUP.Where(x => x.GROUP_ID == groupID).Select(s => new { s.GROUP_NAME }).FirstOrDefault();
                    if (result != null)
                        return result.GROUP_NAME;

                    return "None";
                }
            }
            catch
            {
                return "None";
            }
        }

        /// <summary>
        /// GetGroupName: return group name using Group Code
        /// </summary>
        /// <param name="db"></param>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public static string GetGroupNameUsingGroupCode(string groupCode)
        {
            try
            {
                using (SGGSEntities context = new SGGSEntities())
                {
                    var result = context.SGGS_GROUP.Where(x => x.GROUP_CODE == groupCode).Select(s => new { s.GROUP_NAME }).FirstOrDefault();
                    if (result != null)
                        return result.GROUP_NAME;

                    return "None";
                }
            }
            catch
            {
                return "None";
            }
        }

        /// <summary>
        /// Static method to get GroupCode for a specified groupID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public static string GetGroupCode(long groupID)
        {
            using (SGGSEntities context = new SGGSEntities())
            {
                SGGS_GROUP publisher = context.SGGS_GROUP.Where(x => x.GROUP_ID == groupID).FirstOrDefault();
                if (publisher != null)
                    return publisher.GROUP_CODE;

                return string.Empty;
            }
        }


    }
}