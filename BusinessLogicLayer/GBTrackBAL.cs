using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Web;
using ModelLayer.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
namespace BusinessLogicLayer
{
    public class GBTrackBAL
    {
        private static EzeeloDBContext db = new EzeeloDBContext();
        public static void SaveGBTrack(String CurrentURL, long UserloginId)
        {
            try
            {
                System.DateTime lCurrentDtTime = DateTime.UtcNow.AddHours(5.5);
                GBTrack gBTrack = new GBTrack();
                gBTrack.PageURL = CurrentURL;
                gBTrack.UserLoginId = UserloginId;
                gBTrack.InTime = lCurrentDtTime;
                gBTrack.OutTime = lCurrentDtTime;
                gBTrack.NetworkIP = CommonFunctions.GetClientIP();
                gBTrack.DeviceType = "";
                gBTrack.DeviceID = "";
                db.GBTracks.Add(gBTrack);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
