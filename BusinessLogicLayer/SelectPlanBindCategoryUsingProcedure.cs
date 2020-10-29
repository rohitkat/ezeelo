//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
namespace BusinessLogicLayer
{
    /// <summary>
    /// This class is used to Retrive data for PlanBindCategory Using Procedure
    /// </summary>
    public class SelectPlanBindCategoryUsingProcedure
    {
        /// <summary>
        /// Call PlanBindCategoryUsingProcedure
        /// </summary>
        /// <param name="server"> server Path </param>
        /// <param name="planBindID"> PlanBind ID</param>
        /// <returns>PlanBindManagement</returns>
        public PlanBindManagement SelectPlanBindCategoryForUpdate(System.Web.HttpServerUtility server,int planBindID)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);

            dt = dbOpr.GetRecords("SelectPlanBindCategoryForUpdate", planBindID);

            PlanBindManagement lp = new PlanBindManagement();
            List<PlanCommission> planCommission = new List<PlanCommission>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PlanCommission dd = new PlanCommission();
                dd.ID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                dd.CategoryID = Convert.ToInt16(dt.Rows[i]["CategoryID"].ToString());
                dd.CategoryName = dt.Rows[i]["CategoryName"].ToString();
                dd.CategoryLevel = Convert.ToInt16(dt.Rows[i]["CategoryLevel"].ToString());
                dd.parentCategoryID = Convert.ToInt16(dt.Rows[i]["parentCategoryID"].ToString()) ;
                dd.IsActive = Convert.ToBoolean(dt.Rows[i]["PlancategoryStatus"].ToString());

               // dd.isSelect = Convert.ToBoolean(dt.Rows[i]["isSelect"].ToString());

                planCommission.Add(dd);
            }

            lp.planCommission = planCommission;

            return lp;

        }

    }
}
