using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetDaysLeftController : ApiController
    {
        public int Get()
        {

            var noOfDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var currentMonthDays = 0;
            if (noOfDays == 31)
            {
                currentMonthDays = noOfDays - 6; //25
            }
            else if (noOfDays == 28)
            {
                currentMonthDays = noOfDays - 3; //25
            }
            else if (noOfDays == 29)
            {
                currentMonthDays = noOfDays - 4;
            }
            else
                currentMonthDays = noOfDays - 5;
            var DifferentDays = 0;
            if (DateTime.Now.Day > 25)
            {
                DateTime StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
                DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
                DifferentDays =(int)(EndDate - StartDate).TotalDays;
            }
            else
            {
                DateTime StartDate = new DateTime();
                if (DateTime.Now.Month == 1)
                {
                    StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
                }
                else
                {
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
                }
                DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
                DifferentDays = (int)(EndDate - StartDate).TotalDays;
            }
            var LeftDays = 0;
            if (DateTime.Now.Day > currentMonthDays)
            {
                //var Days = DateTime.Now.Day - currentMonthDays;   //30 Jan 2019
                //LeftDays = currentMonthDays + Days;   //30 days

                var RemoveDays = DateTime.Now.Day - 25;
                LeftDays = DifferentDays - RemoveDays;
            }
            else
            {
                LeftDays = currentMonthDays - DateTime.Now.Day;  //04-feb 2019
            }
            //var currentMonthDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - 6;  //amit
            //return currentMonthDays - DateTime.Now.Day; //amit
            return LeftDays; //21 days
            //int LeftDays = 0;
            //var RemoveDays = 0;
            //DateTime StartDate = new DateTime();
            //if (DateTime.Now.Day > 25)
            //{
            //    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //    DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
            //    var NoOfDays = (EndDate - StartDate).TotalDays;
            //    var CurrentMonthDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //    if (DateTime.Now.Day > 25)
            //    {
            //        RemoveDays = DateTime.Now.Day - 25;
            //        LeftDays = (int)NoOfDays - RemoveDays;
            //    }
            //    else
            //    {
            //        LeftDays = CurrentMonthDays - DateTime.Now.Day;  //04-feb 2019
            //    }
            //}
            //else
            //{
            //    if (DateTime.Now.Month == 1)
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
            //    }
            //    else
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
            //    }
            //    DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //    var NoOfDays = (EndDate - StartDate).TotalDays;
            //    var CurrentMonthDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //    if (DateTime.Now.Day > 25)
            //    {
            //        RemoveDays = DateTime.Now.Day - 25;
            //        LeftDays = (int)NoOfDays - RemoveDays;

            //    }
            //    else
            //    {
            //        LeftDays = (int)NoOfDays - DateTime.Now.Day;  //04-feb 2019
            //    }

            //}
            //return LeftDays;
        }
    }
}
