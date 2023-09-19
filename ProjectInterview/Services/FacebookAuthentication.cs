using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImpactWorks.FBGraph.Connector;
using ImpactWorks.FBGraph.Core;

namespace ProjectInterview.Services
{
    public class FacebookAuthentication
    {
        public Facebook FacebookAuth(string callBack)
        {
            //Setting up the facebook object
            Facebook facebook = new Facebook();
            facebook.AppID = "---------------";
            facebook.CallBackURL = "http://localhost:4637/" + callBack + "/FacebookPostSuccess/";
            facebook.Secret = "-------------";

            //Setting up the permissions
            List<FBPermissions> permissions = new List<FBPermissions>() {
                FBPermissions.user_about_me, // to read about me               
                FBPermissions.user_events,
                FBPermissions.user_status,
                FBPermissions.read_stream,
                FBPermissions.friends_events,
                FBPermissions.publish_stream
            };

            //Pass the permissions object to facebook instance
            facebook.Permissions = permissions;
            return facebook;
        }
    }
}
